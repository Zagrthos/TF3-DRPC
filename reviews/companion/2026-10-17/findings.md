# Companion code review, 17 October 2026

Reviewed: the companion app as of 2026-10-17.
Production source was not changed. File hashes are in [source-manifest.json](source-manifest.json).
See [README.md](README.md) for the full scope, validation, harness boundaries and commands.

Every finding below has an executed reproduction. `REPRODUCED` means that the test
asserted the defective behaviour at this commit; it does not mean that the behaviour is
correct. Controls assert the corresponding successful behaviour. This review adds
**12 findings: eight P2 and four P3**. No P0 or P1 finding was reproduced.

| ID | Priority | Finding | Executed reproduction |
|---|---|---|---|
| R01 | P2 | A wrong-type language or preset resets all settings | Harness `R01`, eight fixtures; `C01` control |
| R02 | P2 | Tray Exit loses a pending template edit | Harness `R02`, actual tray callback, window and timer controls |
| R03 | P2 | Truncation completely removes the pause indication | Harness `R03`; `C03` control |
| R04 | P2 | Accepted vehicle counts can break both the monitor and tooltip rendering | Harness `R04`, parser, monitor and ViewModel callback |
| R05 | P2 | Short ignored log lines amplify allocations and can permanently lose events | Harness `R05`, bounded child process |
| R06 | P3 | Repairing DATA after a failed startup probe never restores saving | Harness `R06`, actual Windows path service and real files |
| R07 | P2 | Settings controls and Exit have no accessible names | Published executable, `desktop-check.ps1` |
| R08 | P2 | Long previews push settings and Exit outside the minimum window | Published executable, `desktop-check.ps1`, screenshots |
| R09 | P3 | A commented-out language overrides the active Lua setting | Harness `R09`, real profile file reads |
| R10 | P3 | An unusable daily log gives no storage warning | Published executable, `desktop-check.ps1 -Scenario LockedLog` |
| R11 | P2 | A future-dated old account log permanently takes over the running session | Harness `R11`, actual account selector and monitor |
| R12 | P3 | Secondary text has insufficient contrast in the tested dark theme | Published executable, UI Automation and `contrast-check.ps1` |

Paths below are relative to the repository root. Line numbers refer to the reviewed commit.

## R01 — P2: One wrong-type settings field resets unrelated valid settings

**Locations:** `companion/Tf3Drpc.Core/Settings/SettingsStore.cs:36–42`;
`companion/Tf3Drpc.Core/Settings/AppSettings.cs:13–17`.

The normaliser can repair an unknown language or preset string independently, but it is
never reached when either property has a different JSON type. Deserialisation throws and
the catch replaces the entire settings record. This differs from the deliberately lenient
handling of templates and the store button, and from the documented per-setting fallback.

One executed fixture is:

```json
{"language":"fr","preset":0,"templates":{"details":"KEEP ME"},"storeButton":true}
```

Expected: Career replaces the invalid preset; French, the custom template and the button
choice survive. Observed: language `game`, preset `career`, default templates, button off.
The harness also tested numbers, booleans, arrays and objects in both `language` and
`preset`: all eight fixtures reset everything. The `C01` control puts an object in
`storeButton` and confirms that the other settings survive that error.

**Why flag it:** a local settings edit or corruption of one value discards unrelated user
customisation; the next ordinary save can overwrite the recoverable original values.
This requires malformed field types, not an ordinary file produced by the UI.

**Reproduce:** run harness case `R01`. It calls the production `SettingsStore.Load` on
real files and asserts the complete returned record.

**Correction direction:** read language and preset leniently too, preserving the other
properties before normalisation. Add mixed valid/invalid-field cases rather than testing
each invalid field only in isolation.

## R02 — P2: Tray Exit bypasses the pending template save

**Locations:** `companion/Tf3Drpc.App/Platforms/Windows/WindowsTrayService.cs:184–185`;
`companion/Tf3Drpc.App/ViewModels/SettingsViewModel.cs:170–174,227–245,266–273`;
`companion/Tf3Drpc.App/Services/AppLifecycleService.cs:31–42`.

Typing schedules a save one second later. The settings window's Exit command explicitly
flushes that edit, but the tray's Exit callback calls the lifecycle service directly.
That service never flushes the draft and quits before the timer runs.

The executed reproduction creates the real settings service, ViewModel, lifecycle service
and Windows tray service. It changes the details template while the actual ViewModel's
save timer is pending, then invokes the actual tray Exit callback. Reloading the settings
file gives the old template. The same edit survives the window Exit command and survives
the timer firing. The native published-app window Exit was independently successful too.

**Why flag it:** an orderly exit should preserve an edit the player has just made. The two
advertised Exit routes currently have different data-loss behaviour. The trigger is exiting
through the tray within the debounce interval; edits already saved are unaffected.

**Reproduce:** harness `R02`. Its manually advanced dispatcher timer represents the
one-second UI timer; native tray widgets and dispatch are boundary stubs, while the
production callback and all save/exit logic execute unchanged.

**Correction direction:** flush pending edits on the common exit route, on the UI thread,
before the application quits. Retain both Exit routes in the regression test.

## R03 — P2: A long custom second line makes paused and playing identical

**Locations:** `companion/Tf3Drpc.Core/Presence/PresenceBuilder.cs:165–174,188–195,211–231`.

The builder appends ` · Paused` before truncating the whole state from its beginning.
With a sufficiently long custom state, truncation removes the entire suffix.

The fixture is a valid 150-character custom second-line template consisting of `S`.
Both playing and paused produce 127 `S` characters followed by `…`. The harness asserts
that the complete `PresenceContentStruct` values are equal, not merely that the suffix is
missing. Consequently the existing Discord de-duplication treats the pause as no change.
The short-state control returns `September 1905 · Paused` correctly.

**Why flag it:** pausing stops being visible for an allowed template, for both explicit and
inferred pauses. This is ordinary user-authored text within the 512-character input limit.

**Reproduce:** harness `R03`, with the production builder and otherwise identical snapshots.

**Correction direction:** reserve the translated pause suffix's length first, then truncate
the rendered user text to the remaining budget without splitting text elements.

## R04 — P2: Vehicle aggregation throws after the parser accepted the status

**Locations:** `companion/Tf3Drpc.Core/Utilities/PresenceValueUtils.cs:73–84`;
`companion/Tf3Drpc.App/Services/PresenceService.cs:41–43,60–61`;
`companion/Tf3Drpc.App/ViewModels/SettingsViewModel.cs:295–299`;
`companion/Tf3Drpc.App/Platforms/Windows/WindowsPickerHandler.cs:80–92`.

The five vehicle counts are added as checked `long` values. Each individual count can be
accepted by the integer converter while their sum exceeds `long.MaxValue`.

```json
{"v":1,"company_roadVehicles":9223372036854775807,"company_railVehicles":1,"rank":2,"year":1905,"month":9}
```

The production classifier accepts this fixture. Building Network then throws
`OverflowException`. Two actual `TickSafelyAsync` calls both fail before calling the
Discord service. Changing to Career restores the monitor's Discord calls. However, the
actual Network preset description callback still throws while the current preset is
Career, because its preview uses the same remembered status.

**Why flag it:** log input is explicitly untrusted. One bad derived statistic should cost
that statistic rather than repeatedly preventing the whole update or letting an exception
escape the UI description callback. These are deliberately extreme/corrupt values; this
review did not observe the game producing them. A native process crash was not asserted.

**Reproduce:** harness `R04`. It tests deserialisation, the checked arithmetic, the real
monitor path and the real ViewModel's tooltip-description delegate.

**Correction direction:** validate derived totals, or sum in a wider representation and
return an unavailable value when it cannot be represented. Keep the remaining status usable.

## R05 — P2: Ignored short log lines reserve large event arrays and can lose the session

**Locations:** `companion/Tf3Drpc.Core/GameLog/GameLogReader.cs:64–72`;
`companion/Tf3Drpc.Core/GameLog/GameLogFollower.cs:76–84,154–157`;
`companion/Tf3Drpc.Core/Constants/GameLogConstants.cs:8`.

The follower reserves one string-array slot per newline. The reader then reserves one
48-byte `TimedGameLogEventStruct` per line, before knowing whether any line is an event.
One MiB of blank lines, which produces zero events, allocated approximately 59.8 MB in
the executed run. At the allowed 64 MiB poll size, event capacity alone can request 3 GiB.

The second reproduction runs in a separate process with a 128 MiB managed-heap limit.
A four MiB blank log followed by a valid `Game is ready` line raises
`OutOfMemoryException`. After collection, the next poll returns zero events: the follower
has already advanced to EOF. A subsequent normal mod status is read, but the real state
machine remains Starting even after three minutes because the ready event was consumed
without reaching it.

**Why flag it:** the one-MiB line bound does not bound allocations caused by large numbers
of short ignored lines. Memory pressure can turn this into permanent event loss for the
current game. The heap limit makes the failure safely reproducible; it is not a claim that
this machine normally has only 128 MiB available or that an ordinary game log is all blank.

**Reproduce:** harness `R05`. It runs the allocation measurement and automatically starts
the bounded `R05-bounded` worker. The worker checks the effective heap limit, the failure,
the lost ready event and the later failure to recover. No machine-wide setting changes.

**Correction direction:** cap batch/line counts, avoid reserving an event for every raw
line, and ensure failed processing cannot silently commit the read position. The
[runtime heap-limit documentation](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector#heap-hard-limit)
describes the per-process switch used by this reproduction.

## R06 — P3: A failed startup DATA probe disables saving for the whole run

**Locations:** `companion/Tf3Drpc.App/Platforms/Windows/WindowsPathService.cs:28–40`;
`companion/Tf3Drpc.App/Services/SettingsService.cs:30–31,110–120`.

`IsDataWritable` is a constructor-only result. When it is false, `Save` short-circuits the
write and reports success, leaving `LastSaveFailed` false. It never probes again.

The fixture initially occupies the harness executable's private DATA path with a file.
The real Windows path service reports unwritable. The test then removes that blocker and
creates a writable DATA directory. `SetLanguage("fr")` returns true, `CanSave` remains false
and no settings file is created. A newly constructed path service immediately recognises
the same directory as writable. The test refuses to replace any pre-existing harness DATA.

**Why flag it:** fixing the folder while the app runs cannot restore persistence, and the
save result says success without writing. Retrying the same selection does not help because
no failed save was recorded. The existing storage banner remains visible; this is not a
claim that the failure is completely hidden. Restarting is a workaround.

**Reproduce:** harness `R06`, using actual Windows filesystem probes and settings writes.

**Correction direction:** allow an explicit save/retry to refresh storage availability,
and base success on the attempted write. Refresh the displayed storage state as well.

## R07 — P2: Seven visible-purpose controls expose no accessible name

**Locations:** `companion/Tf3Drpc.App/Views/SettingsPage.xaml:65–73,83–88,123–132,143–146`.

The labels beside the controls are separate elements. The two pickers, three template
entries and store checkbox expose empty UI Automation `Name` values and no `LabeledBy`
relationship. The Exit button also exposes an empty name in the published Native AOT
build, although the captured image visibly says Exit and invoking it works.

The native test opens the real settings window through second-instance activation and
reads the accessibility properties. It also scrolls the checkbox into view and confirms
that its name is still empty while `IsOffscreen` is false. This is one grouped finding
covering seven controls, not seven independently counted findings.

**Why flag it:** assistive software cannot identify these controls' purposes from their
accessible names. Nearby visual text is not a programmatic label relationship. The test
checked UI Automation; it did not claim to record Narrator's spoken output.

**Reproduce:** publish the app, then run `desktop-check.ps1`. The recorded
[desktop results](desktop-results-windows.json) contain all six input names/relationships,
the visible checkbox and the Exit name. Tested on Windows 11.

**Correction direction:** bind semantic descriptions to the localised labels, including an
explicit semantic name for Exit in this build. Microsoft's
[MAUI accessibility guidance](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/accessibility?view=net-maui-10.0)
describes semantic properties for this purpose. Keep a native accessibility regression check.

## R08 — P2: Legal custom text pushes the settings and Exit out of the minimum window

**Locations:** `companion/Tf3Drpc.App/Views/SettingsPage.xaml:14–15,26–27,54–59,61,139–146`;
`companion/Tf3Drpc.App/Views/SettingsWindow.xaml:13`.

The status/preview header is an unbounded Auto row. The three preview strings wrap and
can consume more height than the whole client area. The settings scroll view is in the
remaining star row, but the header itself cannot scroll or shrink to make room for Exit.

The native test sets all three custom templates to 150 `W` characters and resizes the
window to its supported 400 × 500 DIPs. Each displayed
preview is already truncated to 128 characters, so the problem is not an oversized value
bypassing validation. Exit becomes off-screen and has an empty bounding rectangle. The
settings are also below the viewport. At 640 × 900 DIPs, Exit is visible and works.

**Why flag it:** the documented minimum size cannot support valid user content, and the
player loses access to the controls needed to shorten that content unless they resize the
window. No running game, corrupted input or unusual language is required.

**Reproduce:** `desktop-check.ps1`; inspect `normal` versus `longTemplatesAtMinimum` in
[desktop-results-windows.json](desktop-results-windows.json), and these actual captures:

- [Normal settings window](evidence/settings-normal.png)
- [Minimum window with long preview text](evidence/settings-minimum-long-text.png)

**Correction direction:** constrain or scroll the header content too, while reserving
space for Exit and a usable settings viewport. Test long details, state and hover text
together, including the store button and storage banner.

## R09 — P3: A Lua comment can select the wrong game language

**Locations:** `companion/Tf3Drpc.Core/Utilities/RegexUtils.cs:29`;
`companion/Tf3Drpc.Core/Languages/LanguageCatalog.cs:79–86`.

The regex scans comments as though they were active Lua fields. This valid profile
fragment has German as the active setting:

```lua
profile = {
    -- language = { code = "fr" },
    language = { code = "de" },
}
```

The actual translation service reads the file and returns `fr`; resolving Same as the
game therefore selects French. The test also reproduced the same problem with a Lua block
comment. Removing the comment returns German.

**Why flag it:** a commented-out setting in a manually edited profile should have no
effect. Here it overrides the active setting for the app and presence. This is a conditional
configuration-parser issue; no captured game-generated profile was claimed to contain it.

**Reproduce:** harness `R09`, with real profile files and the unchanged translation service.

**Correction direction:** skip Lua comments and quoted strings before recognising the
language field, without executing the profile. Preserve the existing bounds and timeout.

## R10 — P3: Failure to open the daily log is silently hidden from the settings UI

**Locations:** `companion/Tf3Drpc.App/MauiProgram.cs:78–88`;
`companion/Tf3Drpc.App/ViewModels/SettingsViewModel.cs:340`;
`companion/Tf3Drpc.App/Platforms/Windows/WindowsPathService.cs:28–29`.

The directory probe tests a different temporary filename, so it succeeds when the daily
log itself is locked. `HandleFileError` then discards the provider's error. No shared
storage state is updated, so the window's warning condition remains false. The constructor
catch similarly has no mechanism to make its comment about showing the warning come true.

The executed native case holds today's log open with `FileShare.None` before starting a
private publish. The app runs and exits successfully, its settings file is saved, the
locked daily log remains zero bytes, and the settings window shows no storage warning.

**Why flag it:** the diagnostic channel can be unavailable while the app presents storage
as healthy, contrary to the documented fallback behaviour. This affects diagnosing game
and Discord problems. The reproduced failure is a locked daily file, not an asserted
provider-constructor exception or a recurrence of the earlier startup-crash finding.

**Reproduce:** `desktop-check.ps1 -Scenario LockedLog`; see
[desktop-locked-log-windows.json](desktop-locked-log-windows.json), especially `logBytes`,
`storageWarningVisible` and `R10`.

**Correction direction:** propagate file-logger failure through a storage/diagnostic status
the UI observes. Keep startup resilient, and distinguish settings persistence from logging.

## R11 — P2: A future-dated old Steam account log permanently wins over the live account

**Locations:** `companion/Tf3Drpc.Core/Utilities/UserDataUtils.cs:25–31`;
`companion/Tf3Drpc.App/Platforms/Windows/WindowsPathService.cs:81–91`;
`companion/Tf3Drpc.Core/GameLog/GameLogReader.cs:24–26,34–41`;
`companion/Tf3Drpc.App/Services/MonitorService.cs:200–204`.

After a clock rollback, a previous account's log can have a timestamp later than the
currently running account's log. The newest-mtime selector chooses that old account.
The stale-session check rejects only timestamps before the process start, so a future
timestamp passes and is remembered as current. Complete paths then stop being resolved.

The fixture creates two real Steam account directory layouts. The old account's log and
shutdown line are an hour in the future; the active account has a current ready line.
The actual private Windows account-selection method chooses the old account. Its shutdown
drives the actual monitor to Closing, with no presence, through 20 further ticks over five
minutes. After two more simulated hours, the active log is newer and the actual account
selector would choose it; the already-running monitor still never resolves again and
continues Closing. Ordinary timestamps choose the active account in the control.

**Why flag it:** this breaks the intended clock-rollback and multi-account handling and
can suppress the presence for the entire process lifetime. Both conditions are required.
Only fixture timestamps and the injected clock change; the machine clock, Steam registry
and real game files are untouched. The monitor's path boundary is controlled; account
selection, file metadata checks, log parsing, states and monitor decisions are production code.

**Reproduce:** harness `R11`.

**Correction direction:** do not regard a future-dated file as proof that its session is
the current process. Validate candidate/session plausibility and retain a recovery path
when a selected account cannot describe the running game. Preserve the valid-session
clock-rollback controls when changing confirmation logic.

## R12 — P3: Secondary text falls below the normal-text contrast benchmark

**Location:** `companion/Tf3Drpc.App/App.xaml:10–12`; used, for example, at
`companion/Tf3Drpc.App/Views/SettingsPage.xaml:21,54,58,93–105,110,135`.

The shared style fixes the foreground to Gray, irrespective of the backdrop/theme.
For the actual published app in the tested dark theme, the foreground is `#808080` and
the background under the disconnected-status label is `#272727`. Their contrast is
**3.7824147848863596:1**, below the 4.5:1 normal-text benchmark. UI Automation reports a
font size of 14; the label is not large or bold text. The same style supplies placeholder
descriptions, the personal-data hint and other explanatory text.

**Why flag it:** these are readable, active pieces of information rather than disabled
controls or decoration. Faint labels make the connection state and template explanations
harder to read for people with reduced contrast sensitivity. This is a measured accessibility
finding for this theme, not a claim about every possible backdrop or legal compliance.

**Reproduce:** run `desktop-check.ps1`, then `contrast-check.ps1`. The checker reads the
declared foreground, samples the blank background within that label's captured bounds,
confirms 425 pixels with the exact foreground colour, and computes the luminance ratio.
An independent black/white control returns 21:1. Results are in
[contrast-results-windows.json](contrast-results-windows.json).

The threshold and calculation follow the
[W3C contrast guidance](https://www.w3.org/WAI/WCAG22/Understanding/contrast-minimum.html).
Foreground colour comes from the source style rather than anti-aliased edge pixels.

**Correction direction:** use an appropriate theme-aware semantic text colour and verify
contrast against the supported backdrops. Keep secondary information visually distinct
without making it too faint to read.
