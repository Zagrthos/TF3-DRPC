# Companion review and reproductions

The [report](findings.md) contains thirteen reproduced findings, their exact production
source locations, triggers, expected/observed behaviour, impact and correction directions.

Reviewed: the companion app as of 2026-10-15.
The existing AGENTS.md modification and untracked mod-review files were left alone.
The [source manifest](source-manifest.json) records SHA-256 for all 135 original companion
source/configuration/resource files, excluding generated artifacts. The final comparison
found no changes to those files.

## Run the reproductions

The `harness/` folder was removed on 2026-10-20, after every finding was fixed: it targets
Windows only and broke loading the projects on macOS.

Use Windows and the SDK pinned in companion/global.json. The harness requires no MAUI
workload, game or Discord client; restore obtains its pinned dependencies if they are
not already cached. Windows is required for the real named-pipe and file-sharing cases.

From the repository root in PowerShell:

```powershell
Push-Location .\reviews\companion\2026-10-15\harness
dotnet run --project ReviewHarness.csproj
Pop-Location
```

To run one harness reproduction, pass its ID after `--` (F11 uses smoke.ps1):

```powershell
Push-Location .\reviews\companion\2026-10-15\harness
dotnet run --project ReviewHarness.csproj -- F06
Pop-Location
```

`REPRODUCED` means the case asserted the defective behaviour. `PASS` means a control
worked. `FAILED` means a reproduction/control expectation did not hold, or the harness
failed; it must not be interpreted as a reproduced production defect.

Fresh runs write `artifacts/latest-results-windows.json` and temporary fixtures beneath
this review folder, without overwriting the recorded [results](results-windows.json).
Fixtures are removed after execution. Generated output is ignored by git.

## What executes

The harness references the unchanged Core project. Its assembly name grants the same
internal access as the existing Core tests. It links the actual app monitor, state/log/
presence services, translation and settings services, settings ViewModel, lifecycle,
Windows path service, log-cleanup utility, interfaces, structs, messages and logging.
It reuses the existing fake Discord peer.

Only these framework boundaries are substituted:

- MAUI main-thread dispatch executes synchronously in the headless harness.
- MAUI Application.Quit records that the framework entry point was reached.
- Unused CompanionApplication/WindowsTrayService type markers satisfy the unchanged
  logging declarations; those classes' logic is not substituted as tested production code.

Process discovery and game paths use controlled services in monitor cases. File reads,
settings writes, locks and the F06 named pipe are real Windows I/O. Controlled clocks
change only the fixture's time, never the machine's system clock. The monitor tick is
invoked through reflection without editing it, and C08 exercises its actual periodic loop.
The real pinned NReco provider executes in F12.

## Published-app checks

First publish using the pinned SDK/workload from companion/:

```powershell
Push-Location .\companion
dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r win-x64
Pop-Location
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\reviews\companion\2026-10-15\smoke.ps1
```

The script refuses to start if either TF3-DRPC or the game already runs. It creates private
copies of the publish beneath the ignored review artifacts directory. It starts and
stops only its own processes, removes its copies, and records:

- Intact-copy startup, TrayCreated, and a second-instance/settings-window control.
- F11: startup with tray.ico missing from that private copy.
- F12: startup with a regular file occupying that private copy's DATA/logs path.
- A storage-fallback control with a file occupying DATA itself: the app remains running
  and shows its settings window.

Fresh runs write `artifacts/latest-smoke-results-windows.json`. The recorded
[smoke-results-windows.json](smoke-results-windows.json) records the published-app
observations. `crash-events.ps1` reads matching Windows Application crash events for
these private blocked-log-folder copies; the recorded output is
[startup-crash-events.txt](startup-crash-events.txt).

## Validation and controls

- Existing Core suite: **265 passed, 0 failed, 1 skipped**. The skipped case requires
  explicitly enabled live Discord. Command, run from companion/:
  `dotnet test --project Tf3Drpc.Core.Tests/Tf3Drpc.Core.Tests.csproj`.
- Debug and Release solution builds: **0 warnings, 0 errors**.
- The documented temporary IDE0005 analyser check in Debug and Release:
  **0 warnings, 0 errors**; the temporary companion/.globalconfig was removed.
- Native AOT win-x64 publish succeeded. It reports the documented upstream MAUI IL2070
  warning in Windows CollectionView drag/drop. The excluded, unused WebView methods
  also produce informational ILC messages; the app uses no WebView.
- Publish contained 86,140,658 bytes, no PDB files, and one occurrence of the German
  "Spiel wird gestartet" resource in the native executable.
- F01–F10, F12 and F13 execute in the harness; F11 executes in the published-app script.
  F12 is independently confirmed by both.
- C01: ordinary appends, partial lines and observed truncation.
- C02: observed game stop/restart, replay of the new log and presence clearing.
- C03: valid German .mo and malformed-header rejection.
- C04: successful settings persistence and invalid-language rejection.
- C05: lifecycle ordering/idempotence at the substituted framework Quit boundary.
- C06: profile parsing, status classification and expected Career presence.
- C07: 20,000 deterministic .mo mutations without an uncaught exception.
- C08: actual monitor-loop start/stop, idempotence, no later ticks and restart.

The IPC envelope and ERROR reply shape were checked against the
[official Discord RPC reference](https://docs.discord.com/developers/topics/rpc).
Steam account identifiers in the recorded desktop log strings are redacted.

Machine-readable command summaries and integrity checks are in
[validation.json](validation.json). Build/publish/runner logs are generated evidence and
are ignored by the repository's log rule.

## Limits

No live game/Discord end-to-end run or macOS run was performed. The actual published
Windows app ran for startup, tray-failure, storage-failure and single-instance/window
activation checks. Headless ViewModel/lifecycle controls do not verify native rendering,
accessibility, DPI/layout or actual UI-thread scheduling. OS pipe tests use a controlled
peer, and protocol ERROR tests use the existing fake peer.

The findings require their stated conditions. Large/corrupt-file tests do not establish
that normal game input has those shapes; clock tests do not establish that a clock
correction happened on this machine. Planned macOS support, presets/templates, CI and
other documented future work were not counted as defects. Unreproduced possibilities
were not promoted to findings.
