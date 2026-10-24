"""Run from the repository root, or pass its path as the first argument.

Uses Lupa's Lua 5.2 implementation. For a normal installation: pip install lupa==2.8.
Tests assert the observed bugs, so a REPRODUCED line means a defect was reproduced.
"""
import hashlib
import json
import random
import sys
import tempfile
from pathlib import Path

HERE = Path(__file__).resolve().parent
from lupa.lua52 import LuaRuntime

ROOT = Path(sys.argv[1]).resolve() if len(sys.argv) > 1 else HERE.parents[2]
MOD = ROOT / 'mod/zagrthos_drpc'
SOURCE = (MOD / 'content/drpc/drpc.script.lua').read_bytes()
FIXTURE = (HERE / 'fixture.lua').read_text(encoding='utf-8')
COUNT = 0
EVIDENCE = []

def new(setup=''):
    lua = LuaRuntime(unpack_returned_tuples=True)
    lua.execute(FIXTURE)
    lua.execute('os.setlocale("C", "all")')
    globals_before = set(lua.globals().keys())
    lua.execute(SOURCE.decode(), name='@' + str(MOD / 'content/drpc/drpc.script.lua'))
    assert set(lua.globals().keys()) - globals_before == {'data'}
    lua.execute('CALLBACK = data().guiUpdate')
    if setup:
        lua.execute(setup)
    return lua

def tick(lua, time=None):
    if time is not None:
        lua.globals().NOW = time
    lua.execute('CALLBACK({}, {}, {})')

def messages(lua):
    return list(lua.globals().MESSAGES.values())

def warnings(lua):
    return list(lua.globals().WARNINGS.values())

def status(lua):
    return json.loads(messages(lua)[-1].removeprefix('[drpc-status] '))

def check(name, function, defect=False):
    global COUNT
    detail = function()
    COUNT += 1
    skipped = isinstance(detail, str) and detail.startswith(('not applicable', 'locale unavailable'))
    result = {'test': name, 'result': 'SKIP' if skipped else ('REPRODUCED' if defect else 'PASS'), 'detail': detail}
    EVIDENCE.append(result)
    print(result['result'] + ' ' + name + (': ' + str(detail) if detail is not None else ''))

def baseline():
    lua = new()
    tick(lua)
    result = status(lua)
    sample = (ROOT / 'docs/log-samples/session.log').read_text(encoding='utf-8').split('[drpc-status] ', 1)[1].splitlines()[0]
    assert result == json.loads(sample), (result, sample)
    assert not warnings(lua)
    return str(len(result)) + ' fields exactly match first real log sample'

def scheduling():
    lua = new()
    start = lua.globals().NOW
    for time in [start, start, start+1, start+9, start+10, start+10, start+19, start+20]:
        tick(lua, time)
    assert len(messages(lua)) == 3
    tick(lua, start-100)
    assert len(messages(lua)) == 4
    tick(lua, start-99)
    assert len(messages(lua)) == 4
    tick(lua, start-90)
    assert len(messages(lua)) == 5
    return '10-second cadence; no duplicates; recovery after clock moves backwards'

def strings():
    lua = new()
    test = ''.join(chr(i) for i in range(128)) + ' Holzfällerlager 简体中文 日本語 한국어 Русский 🛤️ e\u0301'
    lua.globals().SAVE_NAME = test
    tick(lua)
    assert status(lua)['saveName'] == test
    assert len(messages(lua)[0].splitlines()) == 1
    return 'ASCII including NUL/control bytes, Unicode and combining marks round-trip'

def nonfinite():
    lua = new('BALANCE = 0/0; WORLD.cloudCoverage = math.huge; COMPANY_VALUE.topSpeed = -math.huge')
    tick(lua)
    result = status(lua)
    assert 'balance' not in result and 'cloudCoverage' not in result and 'company_topSpeed' not in result
    assert result['rank'] == 2 and not warnings(lua)
    return 'NaN and both infinities omitted without losing rank/date'

def modes():
    for setup, expected in [('', (False,False,False)), ('BASE_CONFIG.sandboxMode = true', (True,False,False)),
                            ('GROWTH_CONFIG.data.enforcedLevel = 15', (False,True,False)),
                            ('IS_MAP_EDITOR = true', (False,False,True))]:
        lua = new(setup); tick(lua); result = status(lua)
        assert (result['isSandbox'], result['isTycoon'], result['isMapEditor']) == expected
    lua = new('BALANCE = nil; ACCOUNT = nil; COMPANY_DATA = nil')
    tick(lua); result = status(lua)
    assert result['infiniteMoney'] and all(k not in result for k in ['balance','loan','rank','rankName','experience'])
    return 'normal/sandbox/tycoon/editor flags; infinite money; missing account/progression'

def isolated_errors():
    targets = [
        'api.engine.util.getWorld', 'api.engine.util.getPlayer',
        'api.gui.game.getDefaultSavegameId', 'api.gui.game.isMapEditor', 'api.res.getBaseConfig',
        'api.res.genericRep.find', 'api.res.genericRep.get', 'api.engine.util.getCalendarDate',
        'PROGRESSION_UTIL.getCompanyProgressionState', 'STATIC_UTIL.getRole',
        'api.engine.util.finance.getPlayersBalance', 'api.engine.util.headquarters.getTransportedData',
        'api.engine.util.headquarters.getCompaniesValue', 'api.engine.config.getConfigDict',
    ]
    for target in targets:
        lua = new('ORIGINAL = ' + target + '; ' + target + ' = function() error("injected API failure") end')
        tick(lua); assert status(lua)['v'] == 1 and len(warnings(lua)) == 1
        tick(lua, lua.globals().NOW + 10); assert len(warnings(lua)) == 1
        lua.execute(target + ' = ORIGINAL')
        tick(lua, lua.globals().NOW + 10); assert status(lua)['v'] == 1 and len(messages(lua)) == 3
    lua = new('ORIGINAL = api.util.getLocalTime; api.util.getLocalTime = function() error("clock unavailable") end')
    for _ in range(100): tick(lua)
    assert not messages(lua) and len(warnings(lua)) == 1
    lua.execute('api.util.getLocalTime = ORIGINAL'); tick(lua)
    assert status(lua)['year'] == 1905
    return str(len(targets)) + ' API exceptions contained; warnings deduplicated; clock failure/recovery'

def fractional_numbers():
    lua = new()
    tick(lua)
    assert status(lua)['company_topSpeed'] == 8.3333
    rng = random.Random(40420)
    for _ in range(200):
        value = rng.uniform(-1e12, 1e12)
        lua.globals().BALANCE = value
        tick(lua, lua.globals().NOW + 10)
        result = status(lua)['balance']
        assert abs(result - value) <= max(abs(value)*1e-5, 0.001)
    return 'sample fractional speed and 200 deterministic fractional/exponential money values'

def grouped_reads():
    cases = [
        ('save-name getter fails', 'api.gui.game.getDefaultSavegameId = function() error("unavailable") end; IS_MAP_EDITOR = true', ['isMapEditor']),
        ('base config fails', 'api.res.getBaseConfig = function() error("unavailable") end; GROWTH_CONFIG.data.enforcedLevel = 15', ['isTycoon']),
        ('unused rank-name helper fails', 'STATIC_UTIL.getRole = function() error("unavailable") end', ['experience']),
        ('unused rank-name module fails to load', 'ORIGINAL_REQUIRE = ug_require; ug_require = function(name) if name:find("company_static_util", 1, true) then error("unavailable") end return ORIGINAL_REQUIRE(name) end', ['rank', 'experience']),
        ('WORLD component absent', 'WORLD = nil', ['climate', 'economy']),
        ('calendar conversion fails', 'api.engine.util.getCalendarDate = function() error("unavailable") end', ['timeOfDaySec']),
        ('one native company field fails', 'COMPANY_VALUE.totalAssets = nil; setmetatable(COMPANY_VALUE, { __index = function(t, key) if key == "totalAssets" then error("field removed") end end })', ['company_roadVehicles', 'company_numberOfLines', 'company_totalStations']),
    ]
    result = []
    for name, setup, missing in cases:
        lua = new(setup); tick(lua); data = status(lua)
        assert all(key not in data for key in missing)
        assert len(warnings(lua)) == 1 and data['v'] == 1
        result.append(name + ' -> loses ' + ', '.join(missing))
    return '; '.join(result)

def windows_integer_overflow():
    if sys.platform != 'win32':
        return 'not applicable on this Linux Lua build; run on Windows'
    evidence = []
    for key, expression in [('BALANCE','2147483648'), ('COMPANY_VALUE.totalAssets','3000000000'), ('BALANCE','-2147483649')]:
        lua = new(key + ' = ' + expression)
        tick(lua)
        assert not messages(lua)
        assert len(warnings(lua)) == 1 and 'proper range' in warnings(lua)[0]
        tick(lua, lua.globals().NOW + 10)
        assert not messages(lua) and len(warnings(lua)) == 1
        evidence.append(key + '=' + expression + ' -> no status lines; one warning')
    lua = new('BALANCE = 2147483647'); tick(lua); assert status(lua)['balance'] == 2147483647
    return '; '.join(evidence)

def numeric_locale():
    lua = new()
    actual = lua.eval('os.setlocale("German_Germany.1252", "numeric")')
    if actual is None:
        return 'locale unavailable; run on Windows'
    try:
        tick(lua)
        text = messages(lua)[0]
        assert '"cloudCoverage":0,522476' in text
        try:
            status(lua)
        except json.JSONDecodeError:
            pass
        else:
            raise AssertionError('JSON unexpectedly accepted a decimal comma')
        assert not warnings(lua)
        return actual + ' -> cloudCoverage:0,522476; invalid JSON emitted with no warning'
    finally:
        lua.execute('os.setlocale("C", "all")')

def ctype_locale():
    lua = new()
    actual = lua.eval('os.setlocale("German_Germany.1252", "ctype")')
    if actual is None:
        return 'locale unavailable; run on Windows'
    try:
        lua.globals().SAVE_NAME = '\u6771\u4eac'
        tick(lua)
        table = lua.eval('function() local t={}; for i=1,#MESSAGES[1] do t[i]=string.byte(MESSAGES[1],i) end return t end')()
        raw = bytes(table[i] for i in range(1,len(table)+1))
        assert b'"saveName":"\xe6\\u009d\xb1\xe4\xba\xac"' in raw
        try:
            raw.decode('utf-8')
        except UnicodeDecodeError:
            pass
        else:
            raise AssertionError('UTF-8 unexpectedly remained valid')
        # GameLogFollower's UTF8Encoding(false) uses replacement fallback.
        changed = json.loads(raw.decode('utf-8', errors='replace').removeprefix('[drpc-status] '))['saveName']
        assert changed != '\u6771\u4eac' and not warnings(lua)
        return actual + ' -> Tokyo UTF-8 bytes E6 9D B1 become E6 \\u009d B1; saveName corrupted'
    finally:
        lua.execute('os.setlocale("C", "all")')

def metadata():
    definition = json.loads((MOD/'mod.json').read_text(encoding='utf-8'))
    info = json.loads((MOD/'_metadata/modinfo.json').read_text(encoding='utf-8'))
    assert definition['cosmetic'] and definition['severityAdd'] == definition['severityRemove'] == 'None'
    assert len(info['summary']) <= 100 and '\n' not in info['summary']
    for file in MOD.rglob('*'):
        if file.is_file():
            raw = file.read_bytes(); assert not raw.startswith(b'\xef\xbb\xbf'); raw.decode('utf-8')
            assert file.name.lower() == file.name
    lua = LuaRuntime()
    lua.execute((MOD/'content/drpc/drpc.gs.lua').read_text(encoding='utf-8'))
    reference = lua.eval('data().guiUpdateScript.fileName')
    file, function = reference.split('@')
    assert (MOD/'content/drpc'/ (file+'.lua')).exists()
    assert function == 'guiUpdate'
    return 'metadata constraints, UTF-8/no BOM, lowercase filenames, valid hook reference'

print('Lua 5.2 / ' + sys.platform + ' / source SHA256 ' + hashlib.sha256(SOURCE).hexdigest())
check('baseline', baseline)
check('scheduling', scheduling)
check('strings', strings)
check('nonfinite', nonfinite)
check('modes_and_missing_values', modes)
check('API_exceptions_and_recovery', isolated_errors)
check('fractional_and_exponential_numbers', fractional_numbers)
check('metadata_and_hook', metadata)
check('F3_grouped_reads', grouped_reads, True)
check('F1_windows_integer_overflow', windows_integer_overflow, sys.platform == 'win32')
check('F2_numeric_locale', numeric_locale, sys.platform == 'win32')
check('F4_ctype_locale', ctype_locale, sys.platform == 'win32')
print(str(COUNT) + ' checks completed')
(Path(tempfile.gettempdir()) / ('tf3-mod-review-results-windows.json' if sys.platform == 'win32' else 'tf3-mod-review-results-linux.json')).write_text(json.dumps(EVIDENCE, ensure_ascii=False, indent=2)+'\n', encoding='utf-8')
