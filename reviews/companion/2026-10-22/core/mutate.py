import sys, subprocess, pathlib, re
root = pathlib.Path(sys.argv[1])
f = root / "Tf3Drpc.Core/Monitor/GameMonitor.cs"
orig = f.read_bytes()
muts = {
 "M1 no logReader.Restart": ("            _logReader.Restart();\r\n", ""),
 "M2 no fallbackLogged reset": ("            _fallbackLogged = false;\r\n", ""),
 "M3 no profileReadTimestamp reset on start": ("            _profileReadTimestamp = null;\r\n", ""),
 "M4 no wrongLog": ("if (_paths.IsComplete && !wrongLog)", "if (_paths.IsComplete)"),
 "M5 no reset on restarted": ("        if (result.Restarted)\r\n            _stateMachine.Reset();\r\n", ""),
 "M6 profile read regardless of running": ("&& (running || (_gameLanguageCode is null))", ""),
 "M7 no keep previous language code": ("_languageResolver.ReadGameLanguageCode(_paths.ProfilePath) ?? _gameLanguageCode", "_languageResolver.ReadGameLanguageCode(_paths.ProfilePath)"),
 "M8 state log ignores mission": (" || (status.Game.MissionNumber != previous.Game.MissionNumber)", ""),
 "M9 no path timestamp on start": ("            _pathAttemptTimestamp = _timeProvider.GetTimestamp();\r\n            _profileReadTimestamp = null;", "            _profileReadTimestamp = null;"),
 "M10 game texts with chosen lang twice": ("_languageResolver.LoadGameTexts(_paths.GameDirectory, _language, gameLanguage);", "_languageResolver.LoadGameTexts(_paths.GameDirectory, _language, _language);"),
 "M11 no 'resolved == _paths' short-circuit": ("        if (resolved == _paths)\r\n            return;\r\n", ""),
 "M12 build log on every change incl same": ("&& (status.Game.GameBuild != previous.Game.GameBuild)", ""),
 "M13 no pathAttempt timestamp on retry": ("        GamePathsStruct resolved = _pathResolver.ResolveGamePaths(process.ExecutablePath);\r\n        _pathAttemptTimestamp = _timeProvider.GetTimestamp();", "        GamePathsStruct resolved = _pathResolver.ResolveGamePaths(process.ExecutablePath);"),
 "M14 SetProcessRunning without start time": ("_stateMachine.SetProcessRunning(process.IsRunning, process.StartTime);", "_stateMachine.SetProcessRunning(process.IsRunning);"),
 "M15 poll without startedAt": ("_logReader.PollAsync(_paths.LogPath, startedAt, cancellationToken)", "_logReader.PollAsync(_paths.LogPath, null, cancellationToken)"),
 "M16 stopped log removed": ("            _logger.GameStopped();\r\n", ""),
}
import os
FILTER = os.environ.get("FILTER","*GameMonitorTests")
only = sys.argv[2:]
for name,(a,b) in muts.items():
    if only and name.split()[0] not in only: continue
    s = orig.decode("utf-8")
    if a not in s:
        print(name, "PATTERN NOT FOUND"); continue
    f.write_bytes(s.replace(a,b,1).encode("utf-8"))
    try:
        r = subprocess.run(["dotnet","test","Tf3Drpc.Core.Tests/Tf3Drpc.Core.Tests.csproj","--","--filter-class",FILTER],cwd=root,capture_output=True,text=True)
        out = r.stdout + r.stderr
        m = re.search(r"failed: (\d+)", out)
        fails = re.findall(r"failed (\S+)", out)
        print(name, "->", "failed:"+(m.group(1) if m else "?"), sorted(set(fails))[:6], "" if m else out[-1500:])
    finally:
        f.write_bytes(orig)
