-- Game script of the Discord Rich Presence mod.
--
-- Every few seconds of real time, guiUpdate collects the game state and writes it as one
-- JSON line to the game log (<userdata>/crash_dump/stdout.txt):
--
--   [drpc-status] {"v":1,"saveName":"Archipelago2","year":1905,...}
--
-- The TF3-DRPC companion app reads these lines and shows them in Discord.
-- The script only reads the game state and never calls state:set, so it keeps no state of its
-- own. The game stores an entity for the registered game script with the savegame and removes
-- it on the next load without the mod.

local STATUS_VERSION = 1
local STATUS_LOG_PREFIX = "[drpc-status] "
local WRITE_INTERVAL_SEC = 10
local COMPANY_GROWTH_CONFIG = "::/game_mechanics/company/company_growth_config.res"
local COMPANY_PROGRESSION_UTIL = "::/game_mechanics/company/company_progression_util.tl"
local COMPANY_STATIC_UTIL = "::/game_mechanics/company/company_static_util.tl"
-- The tycoon mode (urbangames_tycoon) enforces the highest company rank.
local TYCOON_ENFORCED_LEVEL = 15

local COMPANY_VALUE_FIELDS = {
    "totalScore", "totalAssets", "debt",
    "railVehicles", "trams", "roadVehicles", "aircrafts", "ships",
    "numberOfLines", "totalStations", "railStations", "tramStations", "roadStations",
    "aircraftStations", "shipStations",
    "trackTotalLength", "trackElectricLength", "roadTotalLength", "bridgeTotalLength",
    "tunnelTotalLength",
    "suppliedTowns", "connectedIndustries",
    "topSpeed", "topLength",
}

local reportedErrors = {}

-- Logs a warning once per name, so a repeating problem doesn't flood the log every few seconds.
local function warnOnce(name, message)
    if not reportedErrors[name] then
        reportedErrors[name] = true
        log.warning("[drpc] " .. message)
    end
end

-- The type of each status value as docs/status-line.md defines it; every other key is a number.
local STRING_KEYS = { "saveName", "rankName", "climate", "economy" }
local BOOLEAN_KEYS = { "isMapEditor", "isSandbox", "isTycoon", "infiniteMoney" }
local EXPECTED_TYPES = {}
for _, key in ipairs(STRING_KEYS) do
    EXPECTED_TYPES[key] = "string"
end
for _, key in ipairs(BOOLEAN_KEYS) do
    EXPECTED_TYPES[key] = "boolean"
end

-- JSON ---------------------------------------------------------------------

local JSON_ESCAPES = { ['"'] = '\\"', ['\\'] = '\\\\', ['\n'] = '\\n', ['\r'] = '\\r', ['\t'] = '\\t' }

-- Escapes exactly what JSON requires (bytes 0-31, quote, backslash). Not %c: under some C
-- locales it also matches bytes above 127 and would split UTF-8 sequences.
local function encodeString(s)
    return '"' .. s:gsub('[\0-\31"\\]', function(c)
        return JSON_ESCAPES[c] or string.format("\\u%04x", c:byte())
    end) .. '"'
end

-- Numbers are formatted without %d, whose Windows formatter only takes 32-bit values, and
-- without the C locale's decimal separator, which can be a comma.
local function encodeNumber(value)
    if value == math.floor(value) and math.abs(value) < 2^53 then
        return string.format("%.0f", value)
    end
    return (string.format("%.6g", value):gsub("[^%deE%+%-]+", "."))
end

-- JSON for a flat table of strings, numbers and booleans, with sorted keys.
local function toJson(t)
    local keys = {}
    for key in pairs(t) do
        keys[#keys + 1] = key
    end
    table.sort(keys)

    local parts = {}
    for _, key in ipairs(keys) do
        local value = t[key]
        local encoded
        -- A value of another type would make the companion reject the whole line, so the key is left out.
        if type(value) ~= (EXPECTED_TYPES[key] or "number") then
            warnOnce(key .. " type", key .. " has an unexpected type " .. type(value))
        elseif type(value) == "string" then
            encoded = encodeString(value)
        elseif type(value) == "number" then
            -- NaN and infinity have no JSON form; the key is left out instead of breaking the line.
            if value == value and value ~= math.huge and value ~= -math.huge then
                encoded = encodeNumber(value)
            end
        elseif type(value) == "boolean" then
            encoded = tostring(value)
        end
        if encoded then
            parts[#parts + 1] = encodeString(key) .. ":" .. encoded
        end
    end
    return "{" .. table.concat(parts, ",") .. "}"
end

-- Status -------------------------------------------------------------------

-- Runs fn(status). A failure leaves the fields unset and is logged once per name, so one
-- broken API call after a game update doesn't take the whole status down.
local function read(status, name, fn)
    local ok, err = pcall(fn, status)
    if not ok then
        warnOnce(name, "reading " .. name .. " failed: " .. tostring(err))
    end
end

-- Returns fn() or nil; a failure is logged once per name, like read.
local function readValue(name, fn)
    local ok, value = pcall(fn)
    if ok then
        return value
    end
    warnOnce(name, "reading " .. name .. " failed: " .. tostring(value))
    return nil
end

-- getConfigDict returns a list of {key, value} pairs. A malformed pair is skipped, so it
-- doesn't cost the valid ones.
local function configDictTable()
    local result = {}
    for _, pair in ipairs(api.engine.config.getConfigDict()) do
        if type(pair) == "table" and type(pair[1]) == "string" and type(pair[2]) == "string" then
            result[pair[1]] = pair[2]
        end
    end
    return result
end

-- Each independent value has its own pcall, and a shared source is read once, so one
-- failing call only loses the values that depend on it.
local function collectStatus()
    local status = { v = STATUS_VERSION }
    -- Without them, only the reads that need them fail (each logged once).
    local world = readValue("world entity", function() return api.engine.util.getWorld() end)
    local player = readValue("player entity", function() return api.engine.util.getPlayer() end)

    status.timestamp = readValue("timestamp", function() return api.util.getLocalTime() end)
    status.saveName = readValue("saveName", function() return api.gui.game.getDefaultSavegameId() end)
    status.isMapEditor = readValue("isMapEditor", function() return api.gui.game.isMapEditor() end)
    status.isSandbox = readValue("isSandbox", function() return api.res.getBaseConfig().sandboxMode == true end)
    status.isTycoon = readValue("isTycoon", function()
        local growthConfig = api.res.genericRep.get(api.res.genericRep.find(COMPANY_GROWTH_CONFIG))
        return growthConfig.data.enforcedLevel == TYCOON_ENFORCED_LEVEL
    end)

    local gameTime = readValue("game time", function()
        return api.engine.getComponent(world, api.type.ComponentType.GAME_TIME)
    end)
    if gameTime then
        -- A failing conversion drops all three; a failing field only itself.
        local date = readValue("date", function() return api.engine.util.getCalendarDate(gameTime.gameTime) end)
        if date then
            status.year = readValue("year", function() return date.year end)
            status.month = readValue("month", function() return date.month end)
            status.day = readValue("day", function() return date.day end)
        end
        status.timeOfDaySec = readValue("timeOfDaySec", function() return gameTime.timeOfDaySec end)
    end
    status.speedup = readValue("speedup", function()
        return api.engine.getComponent(world, api.type.ComponentType.GAME_SPEED).speedup
    end)

    local progressionUtil = readValue("company progression", function() return ug_require(COMPANY_PROGRESSION_UTIL) end)
    -- nil right after a new game starts, until the company script has run.
    local companyData = progressionUtil and readValue("company progression state", function()
        return progressionUtil.getCompanyProgressionState(player)
    end)
    if companyData then
        -- The earned rank, which the game bar shows; level only follows once the player applies
        -- the promotion in the company window.
        status.rank = readValue("rank", function() return companyData.potentialLevel or companyData.level end)
        status.experience = readValue("experience", function() return companyData.experience end)
        if status.rank then
            status.rankName = readValue("rankName", function() return ug_require(COMPANY_STATIC_UTIL).getRole(status.rank) end)
        end
    end

    -- getPlayersBalance returns nil with infinite money, so both keys come from one call.
    read(status, "balance", function(s)
        local balance = api.engine.util.finance.getPlayersBalance(player)
        s.infiniteMoney = balance == nil
        s.balance = balance
    end)
    status.loan = readValue("loan", function()
        local account = api.engine.getComponent(player, api.type.ComponentType.ACCOUNT)
        return account and account.loan or nil
    end)

    local transported = readValue("transported data", function() return api.engine.util.headquarters.getTransportedData() end)
    if transported then
        status.passengersTransported = readValue("passengersTransported", function() return transported.passengersTransported end)
        status.cargoTransported = readValue("cargoTransported", function() return transported.cargoTransported end)
    end
    local companyValue = readValue("company value", function() return api.engine.util.headquarters.getCompaniesValue() end)
    if companyValue then
        for _, name in ipairs(COMPANY_VALUE_FIELDS) do
            local key = "company_" .. name
            status[key] = readValue(key, function() return companyValue[name] end)
        end
    end

    status.cloudCoverage = readValue("cloudCoverage", function()
        return api.engine.getComponent(world, api.type.ComponentType.WORLD).cloudCoverage
    end)
    local configDict = readValue("config dict", configDictTable)
    if configDict then
        status.climate = configDict.climate
        status.economy = configDict.economy
    end

    return status
end

local function writeStatus()
    local ok, err = pcall(function() log.message(STATUS_LOG_PREFIX .. toJson(collectStatus())) end)
    if not ok and not reportedErrors.write then
        reportedErrors.write = true
        log.warning("[drpc] writing the status failed: " .. tostring(err))
    end
end

-- Game script functions ------------------------------------------------------

-- Throttle by real time (POSIX seconds), also while the game is paused or runs at high
-- speed. Not os.clock(): that is wall-clock time on Windows but CPU time on macOS.
local lastWriteSec = nil

function data()
    return {
        guiUpdate = function(_userParams, _state, _guiState)
            -- An error here would repeat every frame, so even the clock is read inside pcall.
            local nowSec = readValue("time", function() return api.util.getLocalTime() end)
            -- NaN would stay in lastWriteSec and make every later comparison false, which stops the writing.
            if type(nowSec) ~= "number" or nowSec ~= nowSec or nowSec == math.huge or nowSec == -math.huge then
                return
            end
            -- A clock set back (NTP, manual change) must not stop the writing until it catches up.
            if lastWriteSec == nil or nowSec < lastWriteSec or nowSec - lastWriteSec >= WRITE_INTERVAL_SEC then
                lastWriteSec = nowSec
                writeStatus()
            end
        end,
    }
end
