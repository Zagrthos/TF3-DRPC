# Companion review and executed reproductions

The [report](findings.md) records **12 reproduced findings: eight P2 and four P3**.
Each has an exact production location, trigger, expected and observed behaviour, impact,
correction direction and an executed reproduction. No production source was edited.

Reviewed: the companion app as of 2026-10-17.
The [manifest](source-manifest.json) records SHA-256 for the 170 original companion files,
excluding generated output. The final integrity comparison is in [validation.json](validation.json).
The separate mod-review directory appearing during this review was left untouched.

## Scope and evidence

| Area | Review and execution |
|---|---|
| Log following, parsing and classification | Read the complete pipeline; existing real-log, partial-line, UTF-8, truncation, rewrite and malformed-input tests; allocation and event-loss reproductions; real locked-file control |
| State/process tracking and monitor | Read states, process discovery, scheduling and the complete monitor; existing process replacement, timestamp, pause and fallback tests; linked production monitor reproductions with controlled process/path/Discord boundaries |
| Presence and templates | Read every preset, placeholder, formatter, preview and description path; existing language, money, date, grapheme, optional-field and custom-template tests; pause-truncation and checked-overflow reproductions |
| Discord IPC | Read connector, frames, handshake, reader/writer lifetime, cancellation, de-duplication, rate limiting and rejected-content backoff; existing fake-peer tests execute those contracts, including stalled writes and clock rollback |
| Settings and storage | Read all converters, normalisation, save, retry, path probing, logging and cleanup code; real JSON files, blocked/repaired DATA, orderly Exit routes and a locked daily file in the published app |
| Languages and resources | Read language resolution, profile matching, .mo parsing/caches and resource lookup; real-file language reproduction; existing .mo bounds/endian/aliasing and installed-German-file test; direct parity check of all 13 resource maps and all 44 keys |
| Windows UI and lifecycle | Read XAML, code-behind, ViewModel, handlers, tray, single instance, window and lifecycle; actual Native AOT startup, second-instance activation, control values, accessibility, resizing, template persistence, Exit, all six preset tooltips and icon-failure fallbacks |
| Build/deployment | Read SDK/workload pin, solution/projects, central packages, build targets and manifest; Debug/Release builds, explicit IDE0005 checks, Native AOT publish and native resource check |
| Tests and fixtures | Read all 319 Fact/Theory methods and their helpers/fixtures; [test-ledger.json](test-ledger.json) inventories their assertions, calls, exception/state/time operations and 56 field declarations; 539 expanded test cases executed |

The earlier 15 October and 16 October companion reviews were read before classification.
Their historic reproduction harnesses intentionally assert old defects and were not treated
as current failures. The current tests and sources cover the existing fixes; the findings
here identify additional failure conditions.

## Run the harness

The `harness/` folder was removed on 2026-10-20, after every finding was fixed: it targets
Windows only and broke loading the projects on macOS.

Use Windows and the SDK pinned in `companion/global.json`. The harness does not need the
MAUI workload, game or Discord client. Restore uses nuget.org and the pinned packages;
Roslyn's inventory reader references the compiler assemblies already inside that SDK.

From the repository root in PowerShell:

```powershell
Push-Location .\reviews\companion\2026-10-17\harness
dotnet run --project ReviewHarness.csproj '-bl:../artifacts/harness-{}.binlog'
Pop-Location
```

Run one reproduction by adding its ID after `--`:

```powershell
Push-Location .\reviews\companion\2026-10-17\harness
dotnet run --project ReviewHarness.csproj '-bl:../artifacts/harness-{}.binlog' -- R02
Pop-Location
```

| Case | Meaning |
|---|---|
| R01 | Wrong-type settings fields reset unrelated values; eight independent fixtures |
| R02 | Exact production tray Exit callback loses the draft; window Exit and timer controls preserve it |
| R03 | Long paused and playing states become exactly equal |
| R04 | Valid status JSON overflows Network; actual monitor ticks and description callback are exercised |
| R05 | Allocation measurement and a separate bounded-heap worker; proves ready-event loss and failed recovery |
| R06 | Actual Windows DATA probes, repair and settings save; refuses an existing harness DATA path |
| R09 | Line/block comments override the active language during real profile reads |
| R11 | Actual Windows account selector and monitor stay on an old future-dated shutdown log |
| P01 | An exclusively locked game log returns no events without throwing; no finding |
| P02 | Sixty malformed/boundary JSON inputs produce no uncaught parser exceptions; no finding |
| C01 | Unicode settings round-trip and independent store-button fallback |
| C02 | Actual monitor/log/state/presence chain gives Mechanic / September 1905; game exit clears it |
| C03 | Monotonic pause after rollback, ordinary pause suffix and no-log fallback |

`REPRODUCED` asserts a defect, `PASS` asserts a control, and `PROBE` records a cleared
exploration. `FAILED` means the assertion did not hold or the harness failed. A successful
harness exit therefore means the recorded defective behaviours remain reproducible at
this commit. A corrected implementation should make its corresponding reproduction fail
until the expected result is rewritten as a regression test.

Fresh runs write `artifacts/latest-results-windows.json`; the recorded run is
[results-windows.json](results-windows.json). Fixtures use unique private directories and
are removed. R05 starts its own child with `DOTNET_GCHeapHardLimit=0x8000000`; it checks the
effective 134,217,728-byte limit. The setting applies only to that child.

## Harness boundaries

The project references the unchanged Core project and links the actual app services,
ViewModel/options, interfaces, messages, structs, constants, logging, cleanup utility,
Windows path service and Windows tray service. Its assembly name grants the same internal
access as the Core tests. It reuses the existing fake Discord peer types.

Only these framework/native boundaries are stubs:

- MAUI UI dispatch runs synchronously; a dispatcher timer can be fired explicitly.
- Framework Application.Quit records that the quit boundary was reached.
- Native tray/menu objects are inert boundaries; the actual production tray Exit callback
  executes. This is not a claim that a physical tray menu click was automated.
- An unused CompanionApplication type marker satisfies the logging declarations.

The process check, path provider and Discord service are controlled in monitor cases.
The production Windows account-selection method is invoked with a private fake Steam root;
the registry is not changed. Real file metadata, file I/O, JSON, arithmetic, states, retries,
callbacks and settings writes execute. Reflection calls private monitor ticks and the tray
callback without modifying source. The injected clock changes its wall/monotonic values,
never the machine clock. The harness is a managed diagnostic app, not a shippable AOT app.

## Published executable checks

First publish from `companion/` with the pinned SDK/workload:

```powershell
Push-Location .\companion
dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r win-x64 '-bl:../reviews/companion/2026-10-17/artifacts/publish-{}.binlog'
Pop-Location
```

Then, from the repository root:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\reviews\companion\2026-10-17\desktop-check.ps1
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\reviews\companion\2026-10-17\contrast-check.ps1
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\reviews\companion\2026-10-17\desktop-check.ps1 -Scenario LockedLog
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\reviews\companion\2026-10-17\desktop-check.ps1 -Scenario Tooltips
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\reviews\companion\2026-10-17\startup-check.ps1
```

These scripts refuse an already-running game or companion. They create private copies of
the publish under ignored review artifacts, create only their own DATA, and stop/remove
only their own copies. UI Automation queries the private process. The tooltip test moves
the pointer over that app's entries and restores its former position. Images capture
only the private settings-window bounds. No Discord window or game screenshot was taken.

Recorded evidence:

- [desktop-results-windows.json](desktop-results-windows.json): second instance exits 0;
  six unnamed inputs, visible unnamed checkbox, unnamed Exit; minimum-size overflow;
  actual window Exit and saved 150-character templates.
- [desktop-locked-log-windows.json](desktop-locked-log-windows.json): app remains usable
  while the daily log stays empty and no storage warning appears.
- [desktop-tooltips-windows.json](desktop-tooltips-windows.json): actual pointer/target
  coordinates and all six correct tooltip strings, including the three Alternating layouts.
- [startup-results-windows.json](startup-results-windows.json): intact startup has a tray
  and hidden window; missing, corrupt and exclusively locked icons all log the failure and
  expose the fallback window. These suspected startup faults were cleared.
- [contrast-results-windows.json](contrast-results-windows.json): declared foreground,
  measured background, UIA font size, confirmed foreground pixels and independent formula control.
- [evidence/](evidence/): normal and minimum-size settings-window captures.

Use `contrast-check.ps1` immediately after the normal desktop scenario: other desktop
scenarios also refresh the latest capture. The recorded JSON/PNG pair is retained in this
review, independently of future fresh runs.

The first synthetic cursor-movement tooltip attempts gave inconsistent observations.
They were not reported as application defects. Repeating with actual mouse input,
explicit DPI awareness, pointer-coordinate verification and independent popup openings
confirmed every preset tooltip. The final successful evidence is the recorded snapshot.

## Build, analyser and existing-test validation

The commands ran using Windows SDK `11.0.100-rc.2.26504.105`, workload set
`11.0.100-rc.2.26509.2`, on Windows 11. Native UI measurements are recorded in device-independent pixels.

- Existing Core suite: **Passed: 538, Failed: 0, Skipped: 1**, from 539 expanded cases.
  The skipped case is the explicitly enabled live Discord test.
- The documented solution test command was independently executed successfully with the
  pinned SDK; no command-format defect is claimed.
- Debug and Release solution builds: **0 warnings, 0 errors**.
- Debug and Release with the temporary IDE0005 warning configuration: **0 warnings,
  0 errors**. `companion/.globalconfig` was removed.
- Native AOT win-x64 publish succeeded. It reports the already documented upstream MAUI
  IL2070 in `MauiItemsView.DragDrop.cs`. Excluded, unused WebView paths also produce the
  documented informational ILC messages. No new application trim/AOT warning was found.
- The native executable started repeatedly; single instance, real settings edits,
  persistence and native window Exit succeeded. Resource embedding is checked in
  [validation.json](validation.json).
- All 13 direct `.resx` maps have the same 44 keys and no duplicates: see
  [resource-validation.json](resource-validation.json).

To repeat the analyser/test validation from the repository root:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\reviews\companion\2026-10-17\validate.ps1
# Optional: publish again too.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\reviews\companion\2026-10-17\validate.ps1 -IncludePublish
```

The script refuses to overwrite an existing `.globalconfig`, creates the IDE0005 setting
only for its two builds, removes it in `finally`, and records command exit codes and logs.
Binary logs, raw command logs, private publishes and fresh snapshots stay under ignored
`artifacts/`; the small recorded result files are retained beside this README.

## Existing test-code review

All 319 declared Fact/Theory methods have direct assertions. The expanded suite passes.
The per-method/field [ledger](test-ledger.json) records the inspected scope. Specific output
values, exact wire JSON, malformed inputs, boundary cases, per-test filesystem fixtures,
controlled clocks and bounded fake-peer waits provide useful behavioural checks.

No additional test-only defect was promoted without a reproduced failure. In particular,
the five-millisecond polling waits are bounded by cancellation, the live test's delay is
intentional, unique GUID paths isolate fixtures, and explicit-culture tests avoid shared
resource-language mutation. Round trips coexist with independent representation assertions.
The native UI and linked app-service reproductions cover paths the Core project cannot
directly reference. This is not a branch-coverage measurement.

## Limits

No live game/Discord end-to-end run, macOS run, Windows 10 run, Narrator speech capture,
full Process Monitor trace or native-speaker translation review was performed. Native
window tests and headless app-service tests have different boundaries, described above.
R04's extreme numeric values, R05's blank log/heap pressure, R06's repaired blocker,
R09's commented profile and R11's clock/account setup are controlled conditions, not claims
that normal game input exhibited them during this review.

Planned Mac Catalyst support, CI and other documented future work were not counted as
defects. Inconclusive automation and unreproduced possibilities were not counted either.
The [report](findings.md) identifies exactly which conditions each finding requires.
