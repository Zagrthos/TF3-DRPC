-- Standalone reproductions: lua52 repro.lua <repository-root>
-- Use a Windows Lua 5.2 build for F1, F2 and F4. F3 also reproduces on Linux.
local root = assert(arg[1], 'pass the repository root')
local scriptDirectory = arg[0]:match('^(.*[/\\])') or './'
local source = root .. '/mod/zagrthos_drpc/content/drpc/drpc.script.lua'

local function instance()
    assert(os.setlocale('C', 'all'))
    local env = setmetatable({}, { __index = _G })
    assert(loadfile(scriptDirectory .. 'fixture.lua', 't', env))()
    assert(loadfile(source, 't', env))()
    env.callback = env.data().guiUpdate
    return env
end

local env = instance()
env.BALANCE = 2147483648
env.callback({}, {}, {})
if #env.MESSAGES == 0 then
    assert(#env.WARNINGS == 1 and env.WARNINGS[1]:find('proper range', 1, true))
    print('REPRODUCED F1: balance 2147483648 prevents the whole status line')
else
    print('SKIP F1: this Lua build supports larger %d integers')
end

env = instance()
if os.setlocale('German_Germany.1252', 'numeric') then
    env.callback({}, {}, {})
    assert(env.MESSAGES[1]:find('"cloudCoverage":0,522476', 1, true))
    assert(#env.WARNINGS == 0)
    print('REPRODUCED F2: decimal commas emitted in JSON without a warning')
else
    print('SKIP F2: Windows locale not available')
end

env = instance()
env.api.gui.game.getDefaultSavegameId = function() error('getter unavailable') end
env.IS_MAP_EDITOR = true
env.callback({}, {}, {})
assert(not env.MESSAGES[1]:find('"isMapEditor"', 1, true))
assert(env.MESSAGES[1]:find('"rank":2', 1, true))
print('REPRODUCED F3: failed save-name getter also loses working map-editor flag')

env = instance()
if os.setlocale('German_Germany.1252', 'ctype') then
    env.SAVE_NAME = '\230\157\177\228\186\172' -- UTF-8 for Tokyo
    env.callback({}, {}, {})
    assert(env.MESSAGES[1]:find('"saveName":"\230\\u009d\177\228\186\172"', 1, true))
    assert(#env.WARNINGS == 0)
    print('REPRODUCED F4: UTF-8 byte 9D in Tokyo escaped independently, corrupting text')
else
    print('SKIP F4: Windows locale not available')
end
assert(os.setlocale('C', 'all'))
