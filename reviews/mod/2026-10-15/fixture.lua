-- Controlled game API fixture. The production chunk is loaded unchanged by review.py.
NOW = 1791926125
MESSAGES = {}
WARNINGS = {}
WORLD = { cloudCoverage = 0.522476 }
GAME_TIME = { gameTime = 1, timeOfDaySec = 18620 }
GAME_SPEED = { speedup = 4 }
ACCOUNT = { loan = 0 }
BASE_CONFIG = { sandboxMode = false }
GROWTH_CONFIG = { data = {} }
COMPANY_DATA = { level = 2, experience = 2673 }
COMPANY_VALUE = {
    totalScore = 16, totalAssets = 55956242, debt = 0,
    railVehicles = 0, trams = 54, roadVehicles = 20, aircrafts = 0, ships = 44,
    numberOfLines = 44, totalStations = 74, railStations = 0, tramStations = 68,
    roadStations = 68, aircraftStations = 0, shipStations = 6,
    trackTotalLength = 669, trackElectricLength = 0, roadTotalLength = 7352,
    bridgeTotalLength = 419, tunnelTotalLength = 0,
    suppliedTowns = 9, connectedIndustries = 14, topSpeed = 8.3333, topLength = 0,
}
TRANSPORTED = { passengersTransported = 3466, cargoTransported = 9424 }
BALANCE = 14422756
SAVE_NAME = 'Archipelago2'
RANK_NAME = 'Mechaniker'
IS_MAP_EDITOR = false
CONFIG_DICT = {
    {'climate', '::/climates/tropical/tropical.clima'},
    {'economy', '::/economy/tropic.eco'},
}
log = {
    message = function(message) MESSAGES[#MESSAGES + 1] = message end,
    warning = function(message) WARNINGS[#WARNINGS + 1] = message end,
}
api = {
    util = { getLocalTime = function() return NOW end },
    gui = { game = {
        getDefaultSavegameId = function() return SAVE_NAME end,
        isMapEditor = function() return IS_MAP_EDITOR end,
    } },
    res = {
        getBaseConfig = function() return BASE_CONFIG end,
        genericRep = {
            find = function(name) return 1 end,
            get = function(id) return GROWTH_CONFIG end,
        },
    },
    type = { ComponentType = {
        GAME_TIME = 'GAME_TIME', GAME_SPEED = 'GAME_SPEED', ACCOUNT = 'ACCOUNT', WORLD = 'WORLD',
    } },
    engine = {
        getComponent = function(entity, kind)
            if kind == 'GAME_TIME' then return GAME_TIME end
            if kind == 'GAME_SPEED' then return GAME_SPEED end
            if kind == 'ACCOUNT' then return ACCOUNT end
            if kind == 'WORLD' then return WORLD end
            error('unexpected component')
        end,
        config = { getConfigDict = function() return CONFIG_DICT end },
        util = {
            getWorld = function() return 1 end,
            getPlayer = function() return 2 end,
            getCalendarDate = function(time) return { year = 1905, month = 9, day = 30 } end,
            finance = { getPlayersBalance = function(player) return BALANCE end },
            headquarters = {
                getTransportedData = function() return TRANSPORTED end,
                getCompaniesValue = function() return COMPANY_VALUE end,
            },
        },
    },
}
PROGRESSION_UTIL = { getCompanyProgressionState = function(player) return COMPANY_DATA end }
STATIC_UTIL = { getRole = function(level) return RANK_NAME end }
ug_require = function(name)
    if name:find('company_progression_util', 1, true) then return PROGRESSION_UTIL end
    if name:find('company_static_util', 1, true) then return STATIC_UTIL end
    error('unexpected resource: ' .. name)
end
