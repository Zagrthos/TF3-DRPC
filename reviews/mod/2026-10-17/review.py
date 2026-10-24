"""Execute the unchanged mod under Lua 5.2; assert findings and controls.

Python >= 3.12, lupa==2.8. Run from any directory. --case selects a test ID.
Recorded results are never overwritten: fresh output goes into ignored artifacts/.
"""
from __future__ import annotations

import argparse
import hashlib
import importlib.metadata
import json
import math
import platform
import random
import re
import subprocess
import sys
import time
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
if sys.platform == "win32" and (HERE / "artifacts/deps").exists():
    sys.path.insert(0, str(HERE / "artifacts/deps"))
from lupa.lua52 import LuaRuntime

SCRIPT = ROOT / "mod/zagrthos_drpc/content/drpc/drpc.script.lua"
SOURCE = SCRIPT.read_text(encoding="utf-8")
FIXTURE = (HERE / "fixture.lua").read_text(encoding="utf-8")
RESULTS: list[dict] = []
SAMPLES: list[dict] = []
SEED = 40420


def strict_json(text):
    def invalid(value):
        raise AssertionError("non-JSON constant: " + value)
    return json.loads(text, parse_constant=invalid)


def new(setup=""):
    lua = LuaRuntime(unpack_returned_tuples=True)
    lua.execute(FIXTURE, name="@fixture.lua")
    lua.execute('assert(os.setlocale("C", "all"))')
    before = set(lua.globals().keys())
    lua.execute(SOURCE, name="@mod/zagrthos_drpc/content/drpc/drpc.script.lua")
    assert set(lua.globals().keys()) == before, "unexpected global variable"
    lua.execute("CALLBACK = data().guiUpdate")
    if setup:
        lua.execute(setup)
    return lua


def tick(lua, now=None):
    if now is not None:
        lua.globals().NOW = now
    lua.execute("CALLBACK({}, {}, {})")


def messages(lua):
    return list(lua.globals().MESSAGES.values())


def warnings(lua):
    return list(lua.globals().WARNINGS.values())


def status(lua):
    text = messages(lua)[-1]
    assert text.startswith("[drpc-status] ")
    return strict_json(text[len("[drpc-status] "):])


def sample(case, lua, accepted, **extra):
    SAMPLES.append({"id": case, "message": messages(lua)[-1], "expectedAccepted": accepted, **extra})


def run(name, function, kind="control"):
    started = time.monotonic()
    try:
        detail = function()
        outcome = "REPRODUCED" if kind in {"finding", "boundary"} else "PASS"
        if isinstance(detail, list) and detail and all(isinstance(item, dict) and item.get("result") == "UNAVAILABLE" for item in detail):
            outcome = "SKIP"
        result = {"id": name, "kind": kind, "result": outcome, "detail": detail}
    except Exception as error:
        result = {"id": name, "kind": kind, "result": "FAILED", "error": str(error), "exception": type(error).__name__}
    result["seconds"] = round(time.monotonic() - started, 4)
    RESULTS.append(result)
    print(result["result"], name, json.dumps(result.get("detail", result.get("error")), ensure_ascii=True))


def baseline():
    lua = new()
    tick(lua)
    actual = status(lua)
    log = (ROOT / "docs/log-samples/session.log").read_text(encoding="utf-8")
    expected = strict_json(log.split("[drpc-status] ", 1)[1].splitlines()[0])
    assert actual == expected
    assert list(actual) == sorted(actual)
    assert not warnings(lua)
    sample("baseline", lua, True)
    return {"fields": len(actual), "matchesRealSample": True}


def registration_metadata():
    lua = LuaRuntime()
    lua.execute((ROOT / "mod/zagrthos_drpc/content/drpc/drpc.gs.lua").read_text())
    desc = lua.globals().data()
    assert list(desc.keys()) == ["guiUpdateScript"]
    assert desc.guiUpdateScript.fileName == "drpc.script@guiUpdate"
    assert (SCRIPT.parent / (desc.guiUpdateScript.fileName.split("@")[0] + ".lua")).exists()
    mod = strict_json((ROOT / "mod/zagrthos_drpc/mod.json").read_text())
    info = strict_json((ROOT / "mod/zagrthos_drpc/_metadata/modinfo.json").read_text())
    assert re.fullmatch(r"[a-z0-9_]+", mod["modId"])
    assert mod["modId"] == "zagrthos_drpc" and mod["revision"] >= 1
    assert mod["cosmetic"] is True and mod["severityAdd"] == mod["severityRemove"] == "None"
    assert len(info["name"]) <= 32 and len(info["summary"]) <= 100
    assert "\n" not in info["name"] + info["summary"]
    assert info["authors"] and info["tags"] == ["Script Mod"]
    for file in (ROOT / "mod").rglob("*"):
        if file.is_file():
            raw = file.read_bytes()
            raw.decode("utf-8")
            assert not raw.startswith(b"\xef\xbb\xbf")
            assert file.name == file.name.lower() and " " not in file.name
    return {"nameLength": len(info["name"]), "summaryLength": len(info["summary"]), "files": 4}


def scheduling():
    lua = new()
    start = lua.globals().NOW
    for offset in [0, 0, 1, 9, 10, 10, 19, 20]:
        tick(lua, start + offset)
    assert len(messages(lua)) == 3
    for offset, expected in [(-100, 4), (-99, 4), (-90, 5), (10000, 6), (10000, 6)]:
        tick(lua, start + offset)
        assert len(messages(lua)) == expected
    for _ in range(10000):
        tick(lua, start + 10000)
    assert len(messages(lua)) == 6
    return "10 s cadence, repeated frames, forward jump, backward jump and 10,000 paused-clock frames"


def clock_exception():
    lua = new('FAULTS.getLocalTime = true')
    for _ in range(1000):
        tick(lua)
    assert not messages(lua) and len(warnings(lua)) == 1
    lua.execute("FAULTS.getLocalTime = nil")
    tick(lua)
    assert status(lua)["year"] == 1905
    return "1,000 throwing clock calls contained; one warning; writing resumes after recovery"


def string_roundtrip():
    lua = new()
    values = ["", "".join(map(chr, range(128))), '"\\\r\n\t\0', "東京 简体中文 日本語 한국어 Русский 🛤️ e\u0301", "%d %s { } [drpc-status] "]
    rng = random.Random(SEED)
    alphabet = [chr(i) for i in range(128)] + list("ö東京😀\u2028\u2029\u0301")
    values.extend("".join(rng.choice(alphabet) for _ in range(rng.randrange(200))) for _ in range(1000))
    for index, value in enumerate(values):
        lua.globals().SAVE_NAME = value
        tick(lua, 1791926125 + 10 * index)
        assert status(lua)["saveName"] == value
        assert "\n" not in messages(lua)[-1] and "\r" not in messages(lua)[-1]
    assert not warnings(lua)
    return {"strings": len(values), "seed": SEED, "includesAllASCII": True}


def numeric_roundtrip():
    lua = new()
    exact = [0, -0.0, 1, -1, 2**31-1, 2**31, -(2**31)-1, 3_000_000_000, -(2**53)+1, 2**53-1]
    values = exact + [2**53, -(2**53), 1.25, -1.25, 5e-324, -5e-324, sys.float_info.max, sys.float_info.min]
    rng = random.Random(SEED)
    values += [math.ldexp(rng.uniform(-1, 1), rng.randint(-1020, 1023)) for _ in range(3000)]
    for index, value in enumerate(values):
        lua.globals().BALANCE = value
        tick(lua, 1791926125 + 10 * index)
        actual = status(lua)["balance"]
        assert math.isfinite(actual)
        if abs(value) < 2**53 and value == math.floor(value):
            assert actual == value, (value, actual)
        else:
            assert abs(actual-value) <= max(abs(value)*0.000006, 5e-324), (value, actual)
    assert not warnings(lua)
    return {"numbers": len(values), "seed": SEED, "exactIntegerBoundary": "abs(n) < 2^53", "fractionRelativeTolerance": 0.000006}


def nonfinite_values():
    fields = [("BALANCE", "balance"), ("WORLD.cloudCoverage", "cloudCoverage"), ("COMPANY_VALUE.topSpeed", "company_topSpeed"), ("TRANSPORTED.cargoTransported", "cargoTransported"), ("GAME_SPEED.speedup", "speedup")]
    for target, key in fields:
        for expression in ["0/0", "math.huge", "-math.huge"]:
            lua = new(target + " = " + expression)
            tick(lua)
            actual = status(lua)
            assert key not in actual and actual["year"] == 1905
            assert not warnings(lua)
    return {"cases": len(fields)*3, "omitted": [key for _, key in fields]}


def locale_roundtrip():
    candidates = ["German_Germany.1252", "de_DE.UTF-8", "de_DE.utf8", "de_DE", "French_France.1252", "fr_FR.UTF-8"]
    selected = []
    for category in ["numeric", "ctype", "all"]:
        lua = new()
        found = None
        for candidate in candidates:
            found = lua.eval("os.setlocale")(candidate, category)
            if found:
                break
        if not found:
            selected.append({"category": category, "result": "UNAVAILABLE"})
            continue
        try:
            lua.globals().SAVE_NAME = "東京 Köln Русский 🛤️"
            lua.globals().BALANCE = 3_000_000_000
            tick(lua)
            actual = status(lua)
            assert actual["saveName"] == "東京 Köln Русский 🛤️"
            assert actual["cloudCoverage"] == 0.522476 and actual["balance"] == 3_000_000_000
            assert not warnings(lua)
            selected.append({"category": category, "result": "PASS", "locale": found})
        finally:
            lua.execute('assert(os.setlocale("C", "all"))')
    if sys.platform == "win32":
        assert all(item["result"] == "PASS" for item in selected), selected
    return selected


def modes_ranks_missing():
    for setup, expected in [("", (False,False,False)), ("BASE_CONFIG.sandboxMode=true", (True,False,False)), ("GROWTH_DATA.enforcedLevel=15", (False,True,False)), ("IS_MAP_EDITOR=true", (False,False,True))]:
        lua = new(setup)
        tick(lua)
        actual = status(lua)
        assert tuple(actual[key] for key in ["isSandbox", "isTycoon", "isMapEditor"]) == expected
    for rank in range(1, 16):
        lua = new(f"COMPANY_DATA.level=1; COMPANY_DATA.potentialLevel={rank}; RANK_NAME=nil")
        tick(lua)
        assert status(lua)["rank"] == rank and isinstance(status(lua)["rankName"], str)
    lua = new("COMPANY_DATA.potentialLevel=nil; COMPANY_DATA.level=3")
    tick(lua)
    assert status(lua)["rank"] == 3
    lua = new("COMPANY_DATA=nil; BALANCE=nil; ACCOUNT=nil; GAME_TIME=nil; TRANSPORTED=nil; COMPANY_VALUE=nil")
    tick(lua)
    actual = status(lua)
    assert actual["infiniteMoney"] is True
    assert not any(key in actual for key in ["balance", "loan", "rank", "experience", "year", "cargoTransported", "company_totalAssets"])
    assert actual["speedup"] == 4 and actual["climate"].endswith("tropical.clima")
    sample("missing_optional_sources", lua, True)
    return "4 mode configurations; all 15 earned ranks, legacy level fallback, initial/missing sources and infinite money"


def failure_isolation():
    baseline_lua = new()
    tick(baseline_lua)
    baseline_status = status(baseline_lua)
    points = ["getWorld", "getPlayer", "getDefaultSavegameId", "isMapEditor", "getBaseConfig", "baseConfig.sandboxMode", "genericRep.find", "genericRep.get", "growthConfig.enforcedLevel", "component_GAME_TIME", "gameTime.gameTime", "gameTime.timeOfDaySec", "getCalendarDate", "component_GAME_SPEED", "gameSpeed.speedup", "require_progression", "getCompanyProgressionState", "companyData.potentialLevel", "companyData.level", "companyData.experience", "require_static", "getRole", "getPlayersBalance", "component_ACCOUNT", "account.loan", "getTransportedData", "transported.passengersTransported", "transported.cargoTransported", "getCompaniesValue", "component_WORLD", "world.cloudCoverage", "getConfigDict"]
    fields = re.search(r"local COMPANY_VALUE_FIELDS = \{(.*?)\n\}", SOURCE, re.S)[1]
    points += ["companyValue." + field for field in re.findall(r'"([^"]+)"', fields)]
    counts = []
    for point in points:
        lua = new()
        if point == "companyData.level":
            lua.execute("COMPANY_DATA.potentialLevel=nil")
        lua.globals().FAULTS[point] = True
        tick(lua)
        actual = status(lua)
        assert actual["v"] == 1
        if point != "getDefaultSavegameId":
            assert actual["saveName"] == "Archipelago2"
        if point.startswith("companyValue."):
            lost_key = "company_" + point.split(".", 1)[1]
            assert actual == {key: value for key, value in baseline_status.items() if key != lost_key}, point
        if point.startswith("transported."):
            lost_key = point.split(".", 1)[1]
            assert actual == {key: value for key, value in baseline_status.items() if key != lost_key}, point
        if point in {"getRole", "require_static"}:
            assert actual == {key: value for key, value in baseline_status.items() if key != "rankName"}, point
        initial_warnings = len(warnings(lua))
        assert initial_warnings >= 1, point
        tick(lua, 1791926135)
        assert len(warnings(lua)) == initial_warnings, point
        lua.globals().FAULTS[point] = None
        tick(lua, 1791926145)
        assert len(messages(lua)) == 3 and status(lua)["year"] == 1905
        counts.append({"point": point, "warnings": initial_warnings})
    return {"cases": len(points), "recovery": "all resumed", "warningCounts": counts}


def single_reads_and_no_state_writes():
    lua = new()
    lua.execute('STATE_TRAP=setmetatable({}, {__index=function() error("state access") end, __newindex=function() error("state write") end}); CALLBACK({},STATE_TRAP,STATE_TRAP)')
    actual = status(lua)
    calls = lua.globals().CALLS
    once = ["getWorld", "getPlayer", "getCalendarDate", "component_GAME_TIME", "getPlayersBalance", "getCompaniesValue", "getTransportedData", "getConfigDict", "getCompanyProgressionState"]
    assert all(calls[key] == 1 for key in once)
    assert actual["rank"] == 2 and not warnings(lua)
    return {"sharedSourcesReadOnce": once, "stateParametersUntouched": True, "clockReadsPerWrite": calls["getLocalTime"]}


def write_failure_recovery():
    lua = new("FAULTS['log.message']=true")
    tick(lua)
    tick(lua, 1791926135)
    assert not messages(lua) and len(warnings(lua)) == 1
    lua.execute("FAULTS['log.message']=nil")
    tick(lua, 1791926145)
    assert status(lua)["year"] == 1905 and len(warnings(lua)) == 1
    return "write failure contained, deduplicated and retried on the next interval"


def calendar_getters():
    outcomes = []
    for field, expected_missing in [("year", ["year", "month", "day"]), ("month", ["month", "day"]), ("day", ["day"])]:
        lua = new(f"FAULTS['date.{field}']=true")
        tick(lua)
        actual = status(lua)
        missing = [key for key in ["year", "month", "day"] if key not in actual]
        assert missing == expected_missing
        assert actual["timeOfDaySec"] == 18620 and actual["rank"] == 2
        assert len(warnings(lua)) == 1
        tick(lua, 1791926135)
        assert len(warnings(lua)) == 1
        sample("calendar_"+field, lua, True)
        if field == "year":
            retained = actual | {"month": 9, "day": 30}
            SAMPLES.append({"id": "calendar_year_retained_month_control", "message": "[drpc-status] "+json.dumps(retained), "expectedAccepted": True})
        lua.globals().FAULTS["date."+field] = None
        tick(lua, 1791926145)
        assert status(lua)["month"] == 9 and status(lua)["day"] == 30
        outcomes.append({"throwingField": field, "missingFields": missing})
    lua = new("DATE.year=nil")
    tick(lua)
    actual = status(lua)
    assert "year" not in actual and actual["month"] == 9 and actual["day"] == 30
    assert not warnings(lua)
    sample("calendar_year_nil_control", lua, True)
    return outcomes


def incompatible_scalars():
    cases = [("rankName_number", "RANK_NAME=3", "rankName"), ("vehicle_string", 'COMPANY_VALUE.roadVehicles="20"', "company_roadVehicles"), ("climate_boolean", "CONFIG_DICT[1][2]=true", "climate"), ("speed_boolean", "GAME_SPEED.speedup=false", "speedup"), ("cloud_string", 'WORLD.cloudCoverage="0.5"', "cloudCoverage")]
    outcomes = []
    for name, setup, key in cases:
        lua = new(setup)
        tick(lua)
        actual = status(lua)
        assert key in actual and actual["year"] == 1905 and actual["rank"] == 2
        assert not warnings(lua)
        sample(name, lua, False)
        corrected = {k: v for k, v in actual.items() if k != key}
        SAMPLES.append({"id": name+"_omitted_control", "message": "[drpc-status] "+json.dumps(corrected, ensure_ascii=True), "expectedAccepted": True})
        tick(lua, 1791926135)
        assert len(messages(lua)) == 2 and not warnings(lua)
        outcomes.append({"case": name, "key": key, "emittedType": type(actual[key]).__name__, "warnings": 0})
    return outcomes


def nan_clock_boundary():
    lua = new("NOW=0/0")
    tick(lua)
    assert len(messages(lua)) == 1 and "timestamp" not in status(lua)
    for now in [1791926125, 1791926135, 1791926225, 1792012525]:
        tick(lua, now)
    assert len(messages(lua)) == 1 and not warnings(lua)
    sample("nan_clock_initial_status", lua, True)
    return {"initialLines": 1, "linesAfterFiniteClockRecovers": 1, "warnings": 0, "apiContract": "getLocalTime is declared integer; NaN is deliberately outside that contract"}


def config_entry_boundary():
    lua = new("CONFIG_DICT[3]={nil, 'unused'}")
    tick(lua)
    actual = status(lua)
    assert "climate" not in actual and "economy" not in actual
    assert actual["rank"] == 2 and len(warnings(lua)) == 1
    sample("malformed_unused_config_entry", lua, True)
    return {"validEntries": ["climate", "economy"], "missingFields": ["climate", "economy"], "apiContract": "all config entries are string/string pairs; this deliberately violates that shape"}


def pause_speeds():
    lua = new()
    for index, speed in enumerate([1, 2, 4, 0, 0, 1]):
        lua.globals().GAME_SPEED.speedup = speed
        tick(lua, 1791926125 + index*10)
        assert status(lua)["speedup"] == speed
    assert len(messages(lua)) == 6 and not warnings(lua)
    return "1x, 2x, 4x, pause-button 0, continued pause and resume all keep the real-time cadence"


TESTS = [
    ("C01", baseline, "control"), ("C02", registration_metadata, "control"),
    ("C03", scheduling, "control"), ("C04", clock_exception, "control"),
    ("C05", string_roundtrip, "control"), ("C06", numeric_roundtrip, "control"),
    ("C07", nonfinite_values, "control"), ("C08", locale_roundtrip, "control"),
    ("C09", modes_ranks_missing, "control"), ("C10", failure_isolation, "control"),
    ("C11", single_reads_and_no_state_writes, "control"), ("C12", write_failure_recovery, "control"),
    ("C13", pause_speeds, "control"),
    ("R1", calendar_getters, "finding"), ("O1", incompatible_scalars, "boundary"),
    ("O2", nan_clock_boundary, "boundary"), ("O3", config_entry_boundary, "boundary"),
]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--case", choices=[entry[0] for entry in TESTS])
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()
    for name, function, kind in TESTS:
        if args.case is None or name == args.case:
            run(name, function, kind)
    output = args.output or HERE / "artifacts" / ("latest-results-" + sys.platform + ".json")
    output.parent.mkdir(parents=True, exist_ok=True)
    manifest = {str(path.relative_to(ROOT)).replace("\\", "/"): hashlib.sha256(path.read_bytes()).hexdigest() for path in sorted((ROOT / "mod").rglob("*")) if path.is_file()}
    commit = subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=ROOT, text=True).strip()
    document = {"reviewedCommit": commit, "productionSha256": manifest, "python": platform.python_version(), "platform": sys.platform, "lupa": importlib.metadata.version("lupa"), "lua": LuaRuntime().eval("_VERSION"), "results": RESULTS}
    output.write_text(json.dumps(document, indent=2, ensure_ascii=True)+"\n", encoding="utf-8")
    sample_path = output.with_name(output.stem+"-messages.json")
    sample_path.write_text(json.dumps(SAMPLES, indent=2, ensure_ascii=True)+"\n", encoding="utf-8")
    print("RESULTS", output)
    print("MESSAGES", sample_path)
    return int(any(result["result"] == "FAILED" for result in RESULTS))


if __name__ == "__main__":
    raise SystemExit(main())
