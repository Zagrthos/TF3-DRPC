-- Controlled boundaries for the unchanged production Lua 5.2 chunk.
-- This models the installed API's signatures; it is not a simulation of TF3.
data = false
NOW = 1791926125
MESSAGES = {}
WARNINGS = {}
CALLS = {}
FAULTS = {}
SAVE_NAME = "Archipelago2"
IS_MAP_EDITOR = false
BALANCE = 14422756
RANK_NAME = "Mechaniker"
WORLD_ID = 1
PLAYER_ID = 2
BASE_CONFIG = { sandboxMode = false }
GROWTH_DATA = {}
GAME_TIME = { gameTime = 1, timeOfDaySec = 18620 }
GAME_SPEED = { speedup = 4 }
WORLD = { cloudCoverage = 0.522476 }
ACCOUNT = { loan = 0 }
DATE = { year = 1905, month = 9, day = 30 }
COMPANY_DATA = { level = 2, potentialLevel = 2, experience = 2673 }
TRANSPORTED = { passengersTransported = 3466, cargoTransported = 9424 }
COMPANY_VALUE = {
    totalScore = 16, totalAssets = 55956242, debt = 0,
    railVehicles = 0, trams = 54, roadVehicles = 20, aircrafts = 0, ships = 44,
    numberOfLines = 44, totalStations = 74, railStations = 0, tramStations = 68,
    roadStations = 68, aircraftStations = 0, shipStations = 6,
    trackTotalLength = 669, trackElectricLength = 0, roadTotalLength = 7352,
    bridgeTotalLength = 419, tunnelTotalLength = 0,
    suppliedTowns = 9, connectedIndustries = 14, topSpeed = 8.3333, topLength = 0,
}
CONFIG_DICT = {
    { "climate", "::/climates/tropical/tropical.clima" },
    { "economy", "::/economy/tropic.eco" },
}

local function called(name)
    CALLS[name] = (CALLS[name] or 0) + 1
    if FAULTS[name] then
        error("injected failure: " .. name)
    end
end

-- Native record getters can throw independently; ordinary records use the same
-- proxy too, so field access faults do not require rewriting production code.
local function record(name, values)
    if values == nil then
        return nil
    end
    return setmetatable({}, {
        __index = function(_, key)
            called(name .. "." .. key)
            return values[key]
        end,
        __newindex = function()
            error("production attempted to mutate " .. name)
        end,
    })
end

log = {
    message = function(message)
        called("log.message")
        assert(type(message) == "string")
        MESSAGES[#MESSAGES + 1] = message
    end,
    warning = function(message)
        called("log.warning")
        WARNINGS[#WARNINGS + 1] = message
    end,
}

api = {
    util = {
        getLocalTime = function()
            called("getLocalTime")
            return NOW
        end,
    },
    gui = { game = {
        getDefaultSavegameId = function()
            called("getDefaultSavegameId")
            return SAVE_NAME
        end,
        isMapEditor = function()
            called("isMapEditor")
            return IS_MAP_EDITOR
        end,
    } },
    res = {
        getBaseConfig = function()
            called("getBaseConfig")
            return record("baseConfig", BASE_CONFIG)
        end,
        genericRep = {
            find = function(name)
                called("genericRep.find")
                assert(name == "::/game_mechanics/company/company_growth_config.res")
                return 1
            end,
            get = function(id)
                called("genericRep.get")
                assert(id == 1)
                return { data = record("growthConfig", GROWTH_DATA) }
            end,
        },
    },
    type = { ComponentType = {
        GAME_TIME = "GAME_TIME", GAME_SPEED = "GAME_SPEED", ACCOUNT = "ACCOUNT", WORLD = "WORLD",
    } },
    engine = {
        getComponent = function(entity, kind)
            called("component_" .. kind)
            assert(entity ~= nil, "nil entity")
            if kind == "ACCOUNT" then
                assert(entity == PLAYER_ID)
                return record("account", ACCOUNT)
            end
            assert(entity == WORLD_ID)
            if kind == "GAME_TIME" then return record("gameTime", GAME_TIME) end
            if kind == "GAME_SPEED" then return record("gameSpeed", GAME_SPEED) end
            if kind == "WORLD" then return record("world", WORLD) end
            error("unexpected component: " .. tostring(kind))
        end,
        config = { getConfigDict = function()
            called("getConfigDict")
            return CONFIG_DICT
        end },
        util = {
            getWorld = function()
                called("getWorld")
                return WORLD_ID
            end,
            getPlayer = function()
                called("getPlayer")
                return PLAYER_ID
            end,
            getCalendarDate = function(time)
                called("getCalendarDate")
                assert(time == GAME_TIME.gameTime)
                return record("date", DATE)
            end,
            finance = { getPlayersBalance = function(player)
                called("getPlayersBalance")
                assert(player == PLAYER_ID)
                return BALANCE
            end },
            headquarters = {
                getTransportedData = function()
                    called("getTransportedData")
                    return record("transported", TRANSPORTED)
                end,
                getCompaniesValue = function()
                    called("getCompaniesValue")
                    return record("companyValue", COMPANY_VALUE)
                end,
            },
        },
    },
}

local progression = { getCompanyProgressionState = function(player)
    called("getCompanyProgressionState")
    assert(player == PLAYER_ID)
    return record("companyData", COMPANY_DATA)
end }
local static = { getRole = function(rank)
    called("getRole")
    if RANK_NAME ~= nil then return RANK_NAME end
    local roles = {
        "Junior", "Mechanic", "Engineer", "Coordinator", "Expert", "Team Leader",
        "Supervisor", "Manager", "Director", "Senior Director", "CEO", "Chairperson",
        "Vice President", "President", "Tycoon",
    }
    return roles[rank]
end }
ug_require = function(name)
    if name == "::/game_mechanics/company/company_progression_util.tl" then
        called("require_progression")
        return progression
    end
    if name == "::/game_mechanics/company/company_static_util.tl" then
        called("require_static")
        return static
    end
    error("unexpected resource: " .. name)
end
