"""Reproductions for the mod review of 2026-10-16.

Usage (Windows, Python 3.12+, `pip install lupa==2.8`):
    python -I review.py <repository root> [<game folder>]

The production script is loaded unchanged into Lupa's Lua 5.2 with the controlled API fixture of
the previous review (../2026-10-15/fixture.lua). Game-file checks read the installed game
(default: the Steam library on C:). REPRODUCED means the defect was asserted; PASS is a control.
"""
import io
import json
import re
import sys
import zipfile
from pathlib import Path

from lupa.lua52 import LuaRuntime

HERE = Path(__file__).resolve().parent
ROOT = Path(sys.argv[1]).resolve() if len(sys.argv) > 1 else HERE.parents[2]
GAME = Path(sys.argv[2]) if len(sys.argv) > 2 else Path(r'C:\Program Files (x86)\Steam\steamapps\common\Transport Fever 3')
SCRIPT = ROOT / 'mod/zagrthos_drpc/content/drpc/drpc.script.lua'
FIXTURE = (ROOT / 'reviews/mod/2026-10-15/fixture.lua').read_text(encoding='utf-8')
RESULTS = []

# The game's own role list (company_static_util.tl, getRoles), English msgids.
ROLES = ['Junior', 'Mechanic', 'Engineer', 'Coordinator', 'Expert', 'Team Leader', 'Supervisor', 'Manager',
         'Director', 'Senior Director', 'CEO', 'Chairperson', 'Vice President', 'President', 'Tycoon']


def new(setup=''):
    lua = LuaRuntime(unpack_returned_tuples=True)
    lua.execute(FIXTURE)
    lua.execute('os.setlocale("C", "all")')
    lua.execute('ROLES = {' + ','.join(json.dumps(r) for r in ROLES) + '}')
    lua.execute('STATIC_UTIL.getRole = function(level) return ROLES[level] end')
    lua.execute(SCRIPT.read_text(encoding='utf-8'), name='@' + str(SCRIPT))
    lua.execute('CALLBACK = data().guiUpdate')
    if setup:
        lua.execute(setup)
    return lua


def status(lua):
    lua.execute('CALLBACK({}, {}, {})')
    return json.loads(list(lua.globals().MESSAGES.values())[-1].removeprefix('[drpc-status] '))


def record(name, result, detail):
    RESULTS.append({'test': name, 'result': result, 'detail': detail})
    print(result + ' ' + name + ': ' + detail)


def zip_text(archive, member):
    with zipfile.ZipFile(GAME / 'base/content' / archive) as z:
        return z.read(member).decode('utf-8')


def game_line(text, pattern):
    for number, line in enumerate(text.splitlines(), 1):
        if re.search(pattern, line):
            return number, line.strip()
    raise AssertionError('not found in game source: ' + pattern)


# R1 - rank is the applied level, the game shows the earned (potential) level -------------------

def r1_game_source():
    bar = zip_text('gui.zip', 'gui/game_bar/game_bar.tl')
    company = zip_text('game_mechanics.zip', 'game_mechanics/company/company.tl')
    growth = zip_text('game_mechanics.zip', 'game_mechanics/company/company_growth.script.tl')
    evidence = [
        ('game_bar.tl', *game_line(bar, r'result\.rank = potentialLevel')),
        ('game_bar.tl', *game_line(bar, r'role = company_static_util\.getRole\(state:old\(\)\.rank\)')),
        ('game_bar.tl', *game_line(bar, r'result\.applyDiff = potentialLevel - level')),
        ('company_growth.script.tl', *game_line(growth, r'companyState\[companyEntity\]\.potentialLevel = newLevel')),
        ('company_growth.script.tl', *game_line(growth, r'companyState\[companyEntity\]\.level = levelToApply')),
        ('company.tl', *game_line(company, r'makeScriptingSendEventCmd\("", "Companies", "applyLevel"')),
    ]
    record('R1_game_source', 'PASS', '; '.join(f'{f}:{n} `{l}`' for f, n, l in evidence))


def r1_pending_promotion():
    # Experience is enough for Engineer (3); the player hasn't opened the company window yet.
    lua = new('COMPANY_DATA = { level = 2, potentialLevel = 3, experience = 9000 }')
    data = status(lua)
    game_bar_role = ROLES[3 - 1]  # game_bar.tl: getRole(potentialLevel)
    assert data['rank'] == 2 and data['rankName'] == 'Mechanic', data
    assert game_bar_role == 'Engineer'
    assert 'potentialLevel' not in json.dumps(data) and 'potentialRank' not in data
    record('R1_pending_promotion', 'REPRODUCED',
           f"level 2, potentialLevel 3: status rank {data['rank']} ({data['rankName']}), "
           f"game bar shows {game_bar_role}; the status carries no potential rank")


def r1_controls():
    lua = new('COMPANY_DATA = { level = 3, potentialLevel = 3, experience = 9000 }')
    assert status(lua)['rank'] == 3
    lua = new('COMPANY_DATA = { level = 15, potentialLevel = 15, experience = 0 }; GROWTH_CONFIG.data.enforcedLevel = 15')
    data = status(lua)
    assert data['rank'] == 15 and data['isTycoon']
    record('R1_controls', 'PASS', 'no pending promotion: rank equals the game bar; tycoon (enforcedLevel 15): rank 15')


# R2 - the mod does leave an entity in the savegame ---------------------------------------------

def r2_savegame_entity():
    log = (ROOT / 'docs/log-samples/session.log').read_text(encoding='utf-8').splitlines()
    log_hits = [(i, l) for i, l in enumerate(log, 1) if 'Creating entity for newly added GameScript zagrthos_drpc::' in l]
    assert log_hits, 'real log line missing'
    exe = (GAME / 'TransportFever3.exe').read_bytes()
    strings = [b'Creating entity for newly added GameScript {}',
               b'Removing entity {} for GameScript {} which is no longer available']
    assert all(s in exe for s in strings)
    claims = []
    for path, pattern in [(SCRIPT, r'nothing ends up in'), (ROOT / 'mod/zagrthos_drpc/_metadata/modinfo.json', r'stores nothing in your savegame'),
                          (ROOT / 'docs/mod.md', r'nothing ends up in the savegame')]:
        for number, line in enumerate(path.read_text(encoding='utf-8').splitlines(), 1):
            if re.search(pattern, line):
                claims.append(f'{path.relative_to(ROOT).as_posix()}:{number}')
    assert len(claims) == 3, claims
    record('R2_savegame_entity', 'REPRODUCED',
           f'docs/log-samples/session.log:{log_hits[0][0]} `{log_hits[0][1].split("]  ", 1)[1]}`; '
           f'TransportFever3.exe contains both engine messages (create, and remove when no longer available); '
           f'claims: {", ".join(claims)}')


# Controls of the previous review's fixes, re-run on the current script -------------------------

def controls_previous_fixes():
    lua = new('BALANCE = 3000000000')
    assert status(lua)['balance'] == 3000000000
    lua = new()
    if lua.eval('os.setlocale("German_Germany.1252", "all")') is not None:
        lua.globals().SAVE_NAME = '\u6771\u4eac'
        data = status(lua)
        assert data['cloudCoverage'] == 0.522476 and data['saveName'] == '\u6771\u4eac'
    lua = new('api.gui.game.getDefaultSavegameId = function() error("x") end; IS_MAP_EDITOR = true')
    data = status(lua)
    assert 'saveName' not in data and data['isMapEditor'] is True
    record('controls_previous_fixes', 'PASS', '64-bit balance, German locale numbers and UTF-8, isolated read failure')


for check in (r1_game_source, r1_pending_promotion, r1_controls, r2_savegame_entity, controls_previous_fixes):
    check()
(Path(__import__('tempfile').gettempdir()) / 'tf3-mod-review-2026-10-16-results.json').write_text(json.dumps(RESULTS, indent=2))
