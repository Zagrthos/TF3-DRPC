"""Check mod API references against an installed TF3 copy without running it.

Writes hashes, exact game-source line locations and short evidence snippets.
No game/user data is changed, and no game binaries are copied into the review.
"""
import argparse
import hashlib
import json
import re
from pathlib import Path
from zipfile import ZipFile

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]


def digest(path):
    with path.open("rb") as stream:
        return hashlib.file_digest(stream, "sha256").hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("game", type=Path)
    parser.add_argument("--output", type=Path, default=HERE / "artifacts/latest-game-inspection.json")
    args = parser.parse_args()
    evidence = []
    hashes = {}

    def inspect(relative, patterns, member=None):
        file = args.game / relative
        if member is None:
            raw = file.read_bytes()
            label = relative
        else:
            with ZipFile(file) as archive:
                raw = archive.read(member)
            label = relative + "!" + member
        hashes[label] = hashlib.sha256(raw).hexdigest()
        lines = raw.decode("utf-8").splitlines()
        for pattern in patterns:
            found = [{"line": index, "text": line.strip()} for index, line in enumerate(lines, 1) if re.search(pattern, line)]
            assert found, (label, pattern)
            evidence.append({"source": label, "pattern": pattern, "matches": found})
        return "\n".join(lines)

    inspect("api/tealdef/api/engine/util.d.tl", [
        r"getWorld\s*:\s*function", r"getPlayer\s*:\s*function",
        r"getCalendarDate\s*:\s*function", r"getPlayersBalance\s*:\s*function",
        r"getTransportedData\s*:\s*function", r"getCompaniesValue\s*:\s*function",
        r"balance or nil, if player has won the lottery",
    ])
    inspect("api/tealdef/api/engine.d.tl", [
        r"getComponent\s*:", r"getConfigDict\s*:",
        r"GAME_TIME\s*:", r"GAME_SPEED\s*:", r"ACCOUNT\s*:", r"WORLD\s*:",
        r"gameTime\s*:\s*integer", r"timeOfDaySec\s*:\s*integer",
        r"speedup\s*:\s*integer", r"loan\s*:\s*integer", r"cloudCoverage\s*:\s*number",
    ])
    inspect("api/tealdef/api/util.d.tl", [r"getLocalTime\s*:\s*function\(\)\s*:\s*integer"])
    inspect("api/tealdef/api/gui.d.tl", [r"getDefaultSavegameId\s*:\s*function", r"isMapEditor\s*:\s*function"])
    inspect("api/tealdef/api/res.d.tl", [r"getBaseConfig\s*:", r"find\s*:\s*function", r"get\s*:\s*function", r"genericRep\s*:"])
    inspect("api/tealdef/main.d.tl", [r"ug_require"])
    inspect("base/tealdef/scripts/log.d.tl", [r"message\s*:\s*function\(string\)", r"warning\s*:\s*function\(string\)"])
    types = inspect("api/tealdef/api/type.d.tl", [
        r"record CompanyValue", r"record TransportationStats", r"sandboxMode\s*:\s*boolean",
        r"guiUpdateScript\s*:",
    ])
    all_fields = re.search(r"global record CompanyValue\n(.*?)\nend", types, re.S)[1]
    script = (ROOT / "mod/zagrthos_drpc/content/drpc/drpc.script.lua").read_text()
    names = re.findall(r'"([^"]+)"', re.search(r"local COMPANY_VALUE_FIELDS = \{(.*?)\n\}", script, re.S)[1])
    for name in names:
        assert re.search(r"\b"+re.escape(name)+r"\s*:\s*(integer|number)\b", all_fields), name
    evidence.append({"check": "all copied CompanyValue fields exist and are numeric", "count": len(names), "fields": names})
    inspect("base/tealdef/game_mechanics/company/company_progression_util.d.tl", [r"potentialLevel\s*:", r"level\s*:", r"experience\s*:"])
    inspect("base/tealdef/game_mechanics/company/company_progression_util.d.tl", [r"getCompanyProgressionState\s*:", r"enforcedLevel\s*:"])
    inspect("base/tealdef/game_mechanics/company/company_static_util.d.tl", [r"getRole\s*:"])
    inspect("base/content/game_mechanics.zip", [r"potentialLevel = companyGrowthConfig.enforcedLevel", r"return companyData"], "game_mechanics/company/company_progression_util.tl")
    inspect("base/content/game_mechanics.zip", [r"potentialLevel = newLevel", r"level = levelToApply"], "game_mechanics/company/company_growth.script.tl")
    inspect("base/content/game_mechanics.zip", [r"company_static_util.getRole", r'\_\("Junior"\)', r'\_\("Tycoon"\)'], "game_mechanics/company/company_static_util.tl")
    inspect("base/content/gui.zip", [r"result.rank = potentialLevel", r"getCompanyProgressionState"], "gui/game_bar/game_bar.tl")
    inspect("mods/release/urbangames_tycoon/content/mod.script.tl", [r"enforcedLevel = 15", r"configDict\s*:\s*\{\{string, string\}\}"])
    inspect("mods/release/urbangames_sandbox/content/mod.script.tl", [r"sandboxMode = true"])
    inspect("base/tealdef/scripts/gamescript.d.tl", [r"guiUpdate\s*:"])

    executable = args.game / "TransportFever3.exe"
    if executable.exists():
        hashes["TransportFever3.exe"] = digest(executable)
    result = {"kind": "read-only static inspection of the installed game", "sourceHashes": hashes, "checks": evidence}
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2, ensure_ascii=True)+"\n", encoding="utf-8")
    print(f"PASS: {len(evidence)} installed-game checks; {len(names)} company fields; {args.output}")


if __name__ == "__main__":
    main()
