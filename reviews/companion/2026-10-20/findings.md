# Companion code review, 20 October 2026

Reviewed: the macOS stack, with the Mac Catalyst target and the macOS platform services.
Production source was not changed by the review.

A finding counts as valid only with an executed reproduction. **Seven findings are valid:
two P2 and five P3.** Four further suspicions from the code reading were tested and did not
hold up; they are listed under [Not valid](#not-valid) with the test that refuted them.

| ID | Priority | Finding | Reproduction |
|---|---|---|---|
| R01 | P2 | Quit from the macOS app menu (Cmd+Q) skips the app's exit steps | Published app, System Events menu click; control: menu bar Exit |
| R02 | P2 | Two starts at the same moment can leave no instance running | Published app, `open -n` twice at once, 5 tries |
| R03 | P3 | `MacGameProcessService` is a copy of `WindowsGameProcessService` | `diff` with the platform names masked |
| R04 | P3 | `MacPathService` repeats most of `WindowsPathService` | `diff`, shared method list |
| R05 | P3 | An unused `GetBoolean` overload and its `objc_msgSend` declaration | `grep` |
| R06 | P3 | docs/companion.md still calls the macOS services stubs | `grep` |
| R07 | P3 | `ObjCRuntimeUtils` outside `Utilities/`, and a split field block | AGENTS.md rules, file location |

P2 means wrong behaviour a user can run into; P3 is maintainability, documentation or style.

## Environment

macOS 27, Xcode 27.1, .NET SDK 11.0.100-rc.2.26504.105 with the
pinned workload set and `maui-maccatalyst`. The app was published with
`dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r maccatalyst-arm64` from
`companion/` and copied (`ditto`) to a scratch folder, called `$APP` below, so its `DATA`
folder stays out of the repository. `$PID` is the running app's process id
(`pgrep -x TF3-DRPC`). The helpers in [tools/](tools/) are run with `swift tools/<name>.swift`
and need the Accessibility permission for the terminal; `realclick.swift` moves the real mouse.
The game was not running; Discord was.

| Helper | Does |
|---|---|
| `extras.swift <pid> [open]` | lists the app's menu bar item and, with `open`, its menu entries |
| `press.swift <pid> <title> …` | opens the menu bar menu and presses the entry at the title path through Accessibility |
| `realclick.swift <pid> <title>` | clicks the menu bar icon and the entry with the mouse (a real user action) |
| `winstate.swift <pid> [close]` | prints the app's visible windows and whether it owns the front window; `close` presses the close button |
| `axmove.swift <pid> <x> <y>` | moves the app's window to a global position in points |
| `screens.swift` | lists the displays |

## R01 — P2: Quit from the macOS app menu (Cmd+Q) skips the app's exit steps

**Location:** `companion/Tf3Drpc.App/Platforms/MacCatalyst/AppDelegate.cs` (no terminate
handling); the exit steps are in `Services/AppLifecycleService.cs` (`ExitAsync`).

Mac Catalyst gives the app a main menu even with `LSUIElement=true`: when the settings window
is focused, the menu bar shows "TF3-DRPC" with "Quit TF3-DRPC" (Cmd+Q). That item terminates
the process directly. `IAppLifecycleService.ExitAsync` doesn't run: the monitor isn't stopped,
the Discord connection isn't closed in order, and `Exiting` isn't raised, so the settings view
model doesn't save a template edit that is still waiting for its one-second save delay. The
menu bar Exit and the window's Exit button both go through `ExitAsync`.

**Reproduction:**

```sh
open -n "$APP"; sleep 8; PID=$(pgrep -x TF3-DRPC)
osascript -e "tell application \"System Events\" to tell (first process whose unix id is $PID) to get name of every menu item of menu 1 of menu bar item 2 of menu bar 1"
osascript -e "tell application \"System Events\" to tell (first process whose unix id is $PID) to click menu item \"Quit TF3-DRPC\" of menu 1 of menu bar item \"TF3-DRPC\" of menu bar 1"
sleep 3; pgrep -x TF3-DRPC || echo "process ended"
grep -c AppExiting "$(dirname "$APP")"/DATA/logs/*.log
```

**Observed:** the menu lists "About TF3-DRPC, …, Hide TF3-DRPC, Hide Others, Show All, …,
Quit TF3-DRPC"; after the click the process ends, and the log has **0** `[AppExiting]` lines.

**Control:** start the app again, then `swift tools/realclick.swift $PID Exit`: the process
ends and the log gets `[AppExiting] TF3-DRPC is exiting.` (In the recorded run the first
control click missed the menu entry and was repeated; the second click ended the app.)

**Expected:** every way of quitting runs `ExitAsync`. Correction direction: route the
terminate request through `ExitAsync` (for example by handling it in the app delegate or by
removing or replacing the Quit command in the main menu through MAUI's `BuildMenu`).

## R02 — P2: Two starts at the same moment can leave no instance running

**Location:** `companion/Tf3Drpc.App/Platforms/MacCatalyst/MacSingleInstanceService.cs`,
`HandOverToRunningInstance`.

Each new process looks for another process with the same bundle identifier and, if it finds
one, posts the hand-over notification and exits. Two processes that start together each see
the other and both exit.

**Reproduction:**

```sh
for i in 1 2 3 4 5; do
  open -n "$APP" & open -n "$APP" & wait
  sleep 10; echo "try $i: running $(pgrep -x TF3-DRPC | wc -l)"
  pkill -x TF3-DRPC; sleep 2
done
```

**Observed:** `running 1` in tries 1 and 2, `running 0` in tries 3, 4 and 5.

**Expected:** at least one instance keeps running. Correction direction: an atomic decision,
for example a lock (`flock` on a file, which the portable rule limits to `DATA`, or a named
semaphore), or a rule that only the younger process yields (compare the launch dates of
`NSRunningApplication`), and skip processes whose `isTerminated` is set.

**Not reproduced:** the related case of a start during the shutdown of the previous instance
(Exit through `press.swift`, then `open -n` at once) left one instance running in the test.

## R03 — P3: `MacGameProcessService` is a copy of `WindowsGameProcessService`

**Location:** `companion/Tf3Drpc.App/Platforms/MacCatalyst/MacGameProcessService.cs`.

Both use only `System.Diagnostics.Process`, which works on both platforms; a change to the
game check has to be made twice.

**Reproduction** (from `companion/Tf3Drpc.App`):

```sh
diff <(sed 's/Windows/X/g' Platforms/Windows/WindowsGameProcessService.cs) \
     <(sed 's/MacCatalyst/X/g; s/Mac/X/g; s/macOS/X/g' Platforms/MacCatalyst/MacGameProcessService.cs) \
  && echo "identical apart from the platform names"
```

**Observed:** `identical apart from the platform names`.

**Correction direction:** one `GameProcessService` in `Services/`, named by both aliases in
`PlatformServices.cs`.

## R04 — P3: `MacPathService` repeats most of `WindowsPathService`

**Location:** `companion/Tf3Drpc.App/Platforms/MacCatalyst/MacPathService.cs`.

Only the Steam folder, the `DATA` root and the translocation check differ; the user-data
choice, the game-folder fallback, the write probes and the log-failure bookkeeping are the
same code in both files.

**Reproduction** (from `companion/Tf3Drpc.App`):

```sh
for m in ResolveGamePaths FindGameDirectory FindUserDataDirectory ProbeDirectory RefreshDataWritable ReportLogFileFailure; do
  printf "%s: " $m; grep -l "$m" Platforms/Windows/WindowsPathService.cs Platforms/MacCatalyst/MacPathService.cs | wc -l
done
diff <(sed 's/Windows/X/g' Platforms/Windows/WindowsPathService.cs) <(sed 's/MacCatalyst/X/g; s/Mac/X/g' Platforms/MacCatalyst/MacPathService.cs) | grep -c '^[<>]'
```

**Observed:** each of the six methods is in both files; 56 differing lines across files of 147
and 181 lines.

**Correction direction:** the shared logic in a base class or a `…Utils` helper, with the Steam
folder and the portable root per platform.

## R05 — P3: An unused `GetBoolean` overload and its `objc_msgSend` declaration

**Location:** `companion/Tf3Drpc.App/Platforms/MacCatalyst/ObjCRuntimeUtils.cs`,
`GetBoolean(in nint, string, in nint)` and `MsgBoolean(nint, nint, nint)`.

**Reproduction** (from `companion/Tf3Drpc.App`):

```sh
grep -rn "GetBoolean(" Platforms/MacCatalyst/*.cs | grep -v "public static"
```

**Observed:** three calls, all with two arguments (`MacWindowService.cs` lines 65, 76 and
125); none uses the three-argument overload.

**Correction direction:** delete both declarations.

## R06 — P3: docs/companion.md still calls the macOS services stubs

**Location:** `docs/companion.md`, the project table.

**Reproduction:** `grep -n stub docs/companion.md`

**Observed:** line 161: "`Tf3Drpc.App` (MAUI; Windows, and Mac Catalyst with stub platform
services)". The macOS platform services replaced the stubs; AGENTS.md asks to update docs a
change makes stale.

**Correction direction:** drop "with stub platform services".

## R07 — P3: `ObjCRuntimeUtils` outside `Utilities/`, and a split field block

**Location:** `companion/Tf3Drpc.App/Platforms/MacCatalyst/ObjCRuntimeUtils.cs`;
`MacSingleInstanceService.cs`, lines 22–27.

AGENTS.md: "Helper statics go into `Utilities/<Name>Utils.cs`" and "Private fields form one
block without blank lines."

**Reproduction:**

```sh
grep -n "Helper statics\|one block without blank lines" AGENTS.md
ls companion/Tf3Drpc.App/Utilities/ companion/Tf3Drpc.App/Platforms/MacCatalyst/ObjCRuntimeUtils.cs
grep -n -A6 "private const uint ReopenEventId" companion/Tf3Drpc.App/Platforms/MacCatalyst/MacSingleInstanceService.cs
```

**Observed:** `Utilities/` holds only `LogFileUtils.cs`; `ObjCRuntimeUtils.cs` is in
`Platforms/MacCatalyst/`; `_target` (with its suppression attribute) and `_reopenRegistered`
are separated by blank lines.

**Correction direction:** move the helper to `Utilities/` (it is Mac-only, so behind
`#if MACCATALYST` or a project-file condition), or write the exception for platform-only
helpers into AGENTS.md; join the field block.

## Not valid

These came from reading the code and did not hold up in a test.

| Suspicion | Test | Result |
|---|---|---|
| The settings window reopens where it was last, not centred on the primary display (`-[NSWindow center]`) | Open Settings…, move the window away (`axmove.swift $PID <x> <y>`), close it, open Settings… again; `winstate.swift` after each step | Reopened centred on the primary display, the same place as the first time. Not reproduced. |
| `HandOverToRunningInstance` autoreleases objects without a pool, because it runs before `UIApplication.Main` | Run a second process with `OBJC_DEBUG_MISSING_POOLS=YES` (and `OBJC_PRINT_OPTIONS=YES` to confirm libobjc reads it) while one runs | libobjc prints "OBJC_DEBUG_MISSING_POOLS is set" and no "autoreleased with no pool" message; the .NET runtime evidently has a pool around `Main`. Refuted. |
| `MacTrayService.Show` leaks the icon image when the status item has no button | Needs `statusItemWithLength:` to return an item without a button while the icon loads; no way to produce that without changing the code | Not reproducible without fault injection; the branch is unreachable in practice. Not counted. |
| A start during the previous instance's shutdown exits without leaving an instance | Exit through `press.swift`, `open -n` at once, check after 10 s | One instance running. Not reproduced (see R02 for the reproduced variant). |

## Limits

No Windows run: a Mac can't build the Windows target. The game and the Discord presence were
not part of this review. The Accessibility helpers drive the real app; macOS grants app
activation only after real clicks, so focus results come from `realclick.swift`.
