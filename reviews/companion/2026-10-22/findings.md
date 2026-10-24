# Companion code review, 22 October 2026

Reviewed: the changes merged into `main` on 22 October 2026, as of the last of them
("Build, test and release on GitHub Actions"):

| Change | Content |
|---|---|
| Game build | the game build in the settings window |
| Merged stack | the merged stack of the three preceding changes, recorded in AGENTS.md |
| Monitor in Core | the monitor tick in Core (`GameMonitor`) with its tests, the `MonitorService` catch, the footer buttons, the game-test fix, the README's size section |
| Release preparation | the release preparation: `tools/build-release.ps1`, the copyright line, the mod's metadata, CHANGELOG.md, the SmartScreen section |
| CI and release | CI and release on GitHub Actions (`ci.yml`, `release.yml`) |

The review was split into three slices (Core, the MAUI app, build/release/CI tooling with the
release-facing docs). Each finding was reproduced by the slice's reviewer and then
**reproduced again independently** on a fresh export of the reviewed code; the outputs
below are from that second run, except where a section says otherwise (R01's 3-second
variants). Production source was not changed by the review.

A finding counts as valid only with an executed reproduction. **Fifteen findings are valid:
three P2 and twelve P3.** Further suspicions were tested and did not hold up; they are listed
under [Not valid](#not-valid) with the test that refuted them.

| ID | Priority | Area | Finding | Reproduction |
|---|---|---|---|---|
| R01 | P2 | App | Opening a footer link with the keyboard moves the focus to Exit | Published app, UI Automation, Space on "GitHub" |
| R02 | P2 | Release | The release notes link to the README with a relative link that is dead on the release page and on mod.io | `tooling/repro-release.ps1`, GitHub's Markdown API |
| R03 | P2 | Release | The Release workflow publishes a tag on any commit, not only on `main` | `tooling/repro-release.ps1`, throwaway repository |
| R04 | P3 | Release | The tag name is pasted into the scripts, so a crafted tag runs code | `tooling/repro-release.ps1` |
| R05 | P3 | Release | An empty changelog section gives release notes made of headings | `tooling/repro-release.ps1` |
| R06 | P3 | Release | The changelog check accepts a heading without a date, although the docs say it requires one | `tooling/repro-release.ps1` |
| R07 | P3 | Release | `build-release.ps1` zips files left from an earlier publish | `tooling/repro-stale-publish.ps1`, real publish |
| R08 | P3 | Docs | The docs say CI runs on every push and the release on any `v*` tag | `grep`, check step with `v0.1.0-rc1` |
| R09 | P3 | Docs | "93 files" in the release zip are 92 files and one folder | `tooling/repro-stale-publish.ps1` |
| R10 | P3 | Core tests | Several `GameMonitor` branches are untested, and one test doesn't test what its name says | `core/mutate.py`, `core/GameMonitorReviewTests.cs` |
| R11 | P3 | Core | "The game's files were not found yet" is never logged when the paths are missing from the start | `core/GameMonitorPathsNotFoundReviewTests.cs` |
| R12 | P3 | Core | Core types stay public that the app no longer uses; AGENTS.md says interfaces are internal | `grep`, build with the types internal |
| R13 | P3 | App | A failing `OpenUrl` is silent: no log entry, no feedback | Published variant with a malformed URL |
| R14 | P3 | App | The game build line's visibility is a view-model property, against "XAML first" | `git grep` |
| R15 | P3 | Docs | docs/companion.md's project table leaves out the monitor | `grep` |

P2 means wrong behaviour a user or the release process can run into; P3 is maintainability,
tests, documentation, hardening or style. There is no P1.

## Environment

Windows 11, .NET SDK `11.0.100-rc.2.26504.105` with the workload set pinned in
`companion/global.json` and `maui-windows`, PowerShell 7, git, and `gh` (only for R02's
Markdown rendering). `<repo>` is the repository root and `<scratch>` an empty folder outside
it. A copy of the reviewed code is exported into `<scratch>/src`. The game was not running;
Discord was. Nothing was pushed, tagged or released.

| Folder | Holds |
|---|---|
| [core/](core/) | review tests for `Tf3Drpc.Core.Tests` and the mutation script |
| [app/](app/) | UI Automation scripts for the published app, and three variant diffs |
| [tooling/](tooling/) | scripts that run the release workflow's steps and the release script |
| [evidence/](evidence/) | screenshots for the "Not valid" list |

**Running the Core files:** copy `core/*.cs` into `<scratch>/src/companion/Tf3Drpc.Core.Tests/Review/`
and run `dotnet test Tf3Drpc.Core.Tests/Tf3Drpc.Core.Tests.csproj -- --filter-class '*ReviewTests'`
from `<scratch>/src/companion`. They are not part of the test project, so they don't build
with the solution.

**Running the app scripts:** publish the copy (`dotnet publish Tf3Drpc.App/Tf3Drpc.App.csproj -c Release -r win-x64`),
copy `artifacts/publish/Tf3Drpc.App/release_win-x64` to `<scratch>/run`, start `TF3-DRPC.exe`
there twice (the second start shows the settings window), then run the scripts with
`powershell -File`. They send keys only when the app's button has the keyboard focus. The
texts are for the German UI ("Beenden" is Exit). A variant is built by applying its diff to
the copy (`git apply app/<name>.diff`) before publishing.

---

## R01 — P2: Opening a footer link with the keyboard moves the focus to Exit

**Location:** `companion/Tf3Drpc.App/ViewModels/SettingsViewModel.cs:199-201`
(`[RelayCommand] private static Task<bool> OpenUrlAsync(string url)`), bound by both footer
buttons in `Views/SettingsPage.xaml:197-210`. Introduced by the monitor-in-Core change.

**Trigger:** a keyboard or screen-reader user focuses "GitHub" or "mod.io" and presses Space
or Enter.

**Cause:** `[RelayCommand]` on a `Task`-returning method generates an `AsyncRelayCommand<string>`
with `AllowConcurrentExecutions = false`. While `Launcher.OpenAsync` runs, `CanExecute` is
false, so both buttons (they share the command) are disabled. WinUI moves the keyboard focus
off the disabled button to the next tab stop, Exit. When the buttons are enabled again, the
focus stays on Exit.

**Expected:** the focus stays on the pressed button, and the other link stays usable.
**Observed:** for about 50 ms both buttons are disabled and the focus is on "Beenden" (Exit).
After the browser opens and the user switches back to the app, the focus is still on Exit.

**Impact:** a keyboard user who opens a link and comes back lands on Exit; one more Space
quits the app. With a slow browser start, both links stay disabled longer.

**Correction direction:** `[RelayCommand(AllowConcurrentExecutions = true)]`, or a synchronous
command that starts the launch and doesn't wait for it. With `AllowConcurrentExecutions = true`
and a launch that takes 3 s (`app/delay-variant-fixed.diff`), the focus stays on "GitHub" and
both buttons stay enabled.

**Reproduction** (opens the repository page in the default browser once):

```powershell
powershell -File app/focus-trace.ps1 -Name GitHub
powershell -File app/focus-on-return.ps1
```

```
focused element: GitHub
pressed Space at 19:27:03 in process <pid>
    6 ms  focus=app:'Beenden' GitHub.enabled=False mod.io.enabled=False
   57 ms  focus=app:'Beenden' GitHub.enabled=True mod.io.enabled=True
   90 ms  focus=another app GitHub.enabled=True mod.io.enabled=True
before: focus in app = False
SetForegroundWindow: True
after return: focus in app = True, focused = 'Beenden' (ControlType.Button)
```

With a launch that takes 3 s (`app/delay-variant.diff`), `app/focus-after-open.ps1` shows the
state while the command runs; the fixed variant (`app/delay-variant-fixed.diff`) keeps the
focus. These two outputs are from the slice reviewer's run; the second run repeated only the
unmodified app above:

```
--- 0.5 s after Space
focused: name='Beenden' type=ControlType.Button sameProcess=True
Button  name="GitHub" ... enabled=False focusable=False
Button  name="mod.io" ... enabled=False focusable=False
--- fixed variant, focus-trace.ps1
    6 ms  focus=app:'GitHub' GitHub.enabled=True mod.io.enabled=True
```

## R02 — P2: The release notes link to the README with a relative link that is dead on the release page and on mod.io

**Location:** `CHANGELOG.md:31` (`the [README](README.md#3-get-the-companion-app) explains
what to do`); `.github/workflows/release.yml:109-117` copies the section into the release
body. `docs/README.md` names CHANGELOG.md as the text for the Mod Hub upload too. From the
release preparation.

**Trigger:** any release.

**Expected:** a working link to the SmartScreen and Gatekeeper steps.
**Observed:** the release body keeps the relative link, and GitHub renders it unchanged
(`href="README.md#…"`). On `…/releases/tag/v0.1.0` the browser resolves it to
`…/releases/tag/README.md#3-get-the-companion-app`, a release page for a tag named
`README.md`, which doesn't exist. On mod.io a relative link points nowhere.

**Impact:** the one link that tells players how to start the unsigned app is dead in exactly
the two places they read the release text.

**Correction direction:** an absolute link in CHANGELOG.md
(`https://github.com/<owner>/<repo>#3-get-the-companion-app`), or a notes step that rewrites
relative links.

**Reproduction:**

```powershell
pwsh -File tooling/repro-release.ps1 -Repo <repo> -OutFolder <scratch>/release   # section R02
gh api markdown -f text='the [README](README.md#3-get-the-companion-app) explains' -f mode=gfm -f context=<owner>/<repo>
```

```
=== R02: the release notes keep the changelog's relative README link
exit=0
exit=0
  start; the [README](README.md#3-get-the-companion-app) explains what to do.
Resolved against the release page: https://github.com/OWNER/REPO/releases/tag/README.md#3-get-the-companion-app
<p dir="auto">the <a href="README.md#3-get-the-companion-app">README</a> explains</p>
```

An HTTP check of the resolved address says nothing while the repository is private (every
unauthenticated request gets a 404), so the evidence is the unchanged `href` and how the
browser resolves it.

## R03 — P2: The Release workflow publishes a tag on any commit, not only on `main`

**Location:** `.github/workflows/release.yml:19-40` (the check job) and `:119-127` (the
release). From the CI and release change.

**Trigger:** a `v0.1.0` tag pushed on a commit that isn't on `main`, for example on a feature
branch before its pull request is merged.

**Expected:** docs/README.md (step 3.3) says to merge first and tag on `main`; a tag
elsewhere should be refused.
**Observed:** the check compares only the tag with `VersionPrefix` and the changelog heading,
and `gh release create --verify-tag` only checks that the tag exists. The release is built
and published from the unmerged commit.

**Impact:** a public release from code that never went through a pull request.

**Correction direction:** in the check job, check out with `fetch-depth: 0`, fetch `main`
and require `git merge-base --is-ancestor "$GITHUB_SHA" origin/main`; or protect `v*` tags
with a ruleset.

**Reproduction:** section R03 of `tooling/repro-release.ps1` builds a throwaway repository
with a `main` branch and a tagged commit on another branch, then runs the check step on it.

```
=== R03: a tag on a commit that is not on main passes the check
tag v0.1.0 is on main: no
exit=0
version=0.1.0
release.yml mentions main or ancestry: False
```

## R04 — P3: The tag name is pasted into the scripts, so a crafted tag runs code

**Location:** `.github/workflows/release.yml:35` (`$tag = '${{ github.ref_name }}'`, pwsh),
`:109` (`'${{ needs.check.outputs.version }}'`) and `:123`
(`gh release create '${{ github.ref_name }}' …`, bash, in the job with `contents: write`).

**Trigger:** a tag name containing `'`; git accepts `'`, `;`, `(`, `)` and `$` in tag names.

**Expected:** the tag is data. **Observed:** the runner pastes it into the script text before
the shell parses it, so the injected PowerShell runs and the check still passes. Line 123 in
bash, with the expressions substituted the same way and `gh` replaced by a function that
prints its arguments, runs an injected `echo` in the job that holds the write token.

**Impact:** low: only people who can push tags can do this, and they can already change
workflows. It is GitHub's documented script-injection pattern, and it costs nothing to remove.

**Correction direction:** pass the values through `env:` (`TAG: ${{ github.ref_name }}`) and
read `$env:TAG` / `"$TAG"`.

**Reproduction:** section R04 of `tooling/repro-release.ps1` for pwsh; for bash, the
`gh release create` lines of `release.yml` with the tag `v0.1.0';echo${IFS}INJECTED;'`
substituted, saved as `release-step.sh` and run with
`bash -c 'gh(){ echo "gh called with: $*"; }; source ./release-step.sh'`.

```
=== R04: a tag name with a quote runs code in the check step
git accepts the tag name (exit 0): v0.1.0';Write-Host('INJECTED-'+'PWSH');'
INJECTED-PWSH
exit=0
--- bash
gh release create 'v0.1.0';echo${IFS}INJECTED;'' dist/*.zip … --verify-tag
gh called with: release create v0.1.0
INJECTED
./release-step.sh: line 1: : command not found
```

## R05 — P3: An empty changelog section gives release notes made of headings

**Location:** `.github/workflows/release.yml:111-114`. When the section has no body,
`$start + 1` is greater than `$end - 1`, and PowerShell's range operator counts down.

**Trigger:** a version heading directly followed by the next `## ` heading (or at the end of
the file). The check step accepts it.

**Expected:** the step fails, or the notes are empty. **Observed:** the notes are the next
section's heading followed by the version's own heading.

**Impact:** a release published with wrong notes.

**Correction direction:** fail when `$end -le $start + 1`, or take the lines with
`Select-Object -Skip ($start + 1) -First ($end - $start - 1)`.

**Reproduction:** section R05 of `tooling/repro-release.ps1` (a `## 0.2.0 — 2026-12-01`
heading directly above 0.1.0, `VersionPrefix` 0.2.0, tag `v0.2.0`).

```
=== R05: an empty changelog section produces notes made of headings
exit=0
exit=0
release-notes.md, first lines:
## 0.1.0 (app) and revision 1 (mod) — 2026-10-30
## 0.2.0 — 2026-12-01
**SHA-256**
```

## R06 — P3: The changelog check accepts a heading without a date, although the docs say it requires one

**Location:** `.github/workflows/release.yml:37-39` rejects only the words "not released yet".
AGENTS.md:32 ("checking that CHANGELOG.md has a dated section") and docs/README.md:219 ("the
changelog's dated section") describe a date check.

**Trigger:** a heading without a date, or with another placeholder.

**Expected:** rejected. **Observed:** accepted.

**Impact:** an undated release text, and docs that promise a check that doesn't exist.

**Correction direction:** require a date (`$heading -match '\d{4}-\d{2}-\d{2}\s*$'`), or reword
the docs.

**Reproduction:** section R06 of `tooling/repro-release.ps1`.

```
=== R06: the check accepts a heading without a date
## 0.1.0 (app) and revision 1 (mod)
exit=0
## 0.1.0 (app) and revision 1 (mod) — TBD
exit=0
```

## R07 — P3: `build-release.ps1` zips files left from an earlier publish

**Location:** `tools/build-release.ps1:62-68` copies everything in
`companion/artifacts/publish/Tf3Drpc.App/release_win-x64` except `DATA`; nothing empties that
folder before `dotnet publish`, and `dotnet publish` doesn't remove files it no longer
produces. From the release preparation.

**Trigger:** a local release build after an earlier publish produced a file the current one
doesn't (a removed dependency, a renamed resource, a file a developer put there).

**Expected:** the zip holds only the current publish. **Observed:** the old file ships.

**Impact:** low in CI, where runners start clean; real for local release builds, which the
script's header and AGENTS.md describe.

**Correction direction:** delete the publish folder before publishing, or publish with
`--output` into a fresh folder.

**Reproduction** (real Native AOT publish of the reviewed code, a few minutes):

```powershell
pwsh -File tooling/repro-stale-publish.ps1 -Repo <repo> -Commit HEAD -OutFolder <scratch>/stale
```

```
=== first run (clean publish folder)
zip entries: 93, files: 92, folders: 1 (TF3-DRPC/Microsoft.UI.Xaml/)
OldDependency.dll in the zip: False
=== second run, after a file was left in the publish folder
zip entries: 94, files: 93, folders: 1 (TF3-DRPC/Microsoft.UI.Xaml/)
OldDependency.dll in the zip: True
```

## R08 — P3: The docs say CI runs on every push and the release on any `v*` tag

**Location:**
- "every pull request and push": `docs/README.md:103`, `:148`, `:219`. `ci.yml:8-9` runs on a
  push only to `main` (its own header comment says so).
- "a `v*` tag publishes": `docs/companion.md:23`, `docs/README.md:103`, `:219`, `:237`,
  `AGENTS.md:114`. Only `v<VersionPrefix>` passes the check; a pre-release tag fails.
  AGENTS.md:31 already says `v<VersionPrefix>`.

**Correction direction:** "every pull request and every push to `main`"; "a
`v<VersionPrefix>` tag (no pre-release tags)".

**Reproduction:**

```
$ grep -n -A2 "^  push:" .github/workflows/ci.yml
8:  push:
9-    branches: [main]
PS> & tooling/Invoke-ReleaseStep.ps1 -Workflow .github/workflows/release.yml -Step 'Match the tag with the app version and the changelog' -Expressions @{ 'github.ref_name' = 'v0.1.0-rc1' } -WorkDir <scratch>/release/r02
Tag v0.1.0-rc1 doesn't match VersionPrefix 0.1.0 in companion/Directory.Build.props.
exit=1
```

## R09 — P3: "93 files" in the release zip are 92 files and one folder

**Location:** `docs/README.md:145` ("93 files in a `TF3-DRPC` folder").

**Observed:** the zip has 93 entries: 92 files and the folder entry
`TF3-DRPC/Microsoft.UI.Xaml/` (first run of R07's reproduction above:
`zip entries: 93, files: 92, folders: 1`).

**Correction direction:** "92 files".

## R10 — P3: Several `GameMonitor` branches are untested, and one test doesn't test what its name says

**Location:** `companion/Tf3Drpc.Core.Tests/Monitor/GameMonitorTests.cs:367-381`
(`RewrittenLog_ResetsTheStateAsync`) and these lines of
`companion/Tf3Drpc.Core/Monitor/GameMonitor.cs`, which no shipped test notices when removed:

| Mutant | Line | Removed behaviour |
|---|---|---|
| M3 | 135 | `_profileReadTimestamp = null` on a game start |
| M5 | 185-186 | `_stateMachine.Reset()` after a rewritten log |
| M7 | 211 | `?? _gameLanguageCode` (keep the known language when the profile can't be read) |
| M8 | 241 | the mission number in the state-change log condition |
| M13 | 161 | `_pathAttemptTimestamp` after a retry |
| M11, M12, M15 | 163-164, 244, 183 | duplicate-log suppression and the `startedAt` filter (low value: other code covers them) |

**Trigger:** remove one line at a time (mutation testing) and run the shipped `GameMonitorTests`.

**Expected:** the monitor-in-Core change says the tests cover "the path and profile retry
intervals … a rewritten log … language changes", so each mutant should fail a test. **Observed:** all stay green.
`RewrittenLog_ResetsTheStateAsync` passes even without the `Reset()` call: its rewritten log
starts with `Application startup. Hello!`, which resets the state machine on its own.

**Impact:** regressions in these branches would pass CI: the old state and build kept after a
log rewrite, a path resolution (a Steam folder scan) on every tick, a stale language after a
quick restart, a lost language when `profile.lua` is unreadable, a mission change not logged.

**Correction direction:** rewrite the rewritten-log test so the new log has no startup line
and assert that `GameBuild` is cleared; add the tests in `core/GameMonitorReviewTests.cs`.

**Reproduction:** from `<scratch>/src/companion`, with `core/*.cs` copied into
`Tf3Drpc.Core.Tests/Review/` (the script's patterns expect CRLF line endings, as a Windows
checkout has):

```
FILTER='*GameMonitorTests' python -I <repo>/reviews/companion/2026-10-22/core/mutate.py . M3 M5 M7 M8 M13 M11 M12 M15
FILTER='*GameMonitorReviewTests' python -I <repo>/reviews/companion/2026-10-22/core/mutate.py . M3 M5 M7 M8 M13
```

```
== shipped GameMonitorTests
M3 no profileReadTimestamp reset on start -> failed:0 []
M5 no reset on restarted -> failed:0 []
M7 no keep previous language code -> failed:0 []
M8 state log ignores mission -> failed:0 []
M11 no 'resolved == _paths' short-circuit -> failed:0 []
M12 build log on every change incl same -> failed:0 []
M13 no pathAttempt timestamp on retry -> failed:0 []
M15 poll without startedAt -> failed:0 []
== review tests
M3 … -> failed:1 [GameMonitorReviewTests.NewGameStart_ReadsTheProfileAtOnceAsync]
M5 … -> failed:1 [GameMonitorReviewTests.RewrittenLogWithoutAStartupLine_ResetsStateAndBuildAsync]
M7 … -> failed:1 [GameMonitorReviewTests.UnreadableProfile_KeepsTheKnownGameLanguageAsync]
M8 … -> failed:1 [GameMonitorReviewTests.MissionChangeWithinOnePoll_IsLoggedAsync]
M13 … -> failed:1 [GameMonitorReviewTests.IncompletePaths_WaitTheRetryIntervalAgainAfterARetryAsync]
GameMonitor.cs restored: yes
```

The five review tests pass on the unchanged code.

## R11 — P3: "The game's files were not found yet" is never logged when the paths are missing from the start

**Location:** `companion/Tf3Drpc.Core/Monitor/GameMonitor.cs:163-170`: `if (resolved == _paths)
return;` comes before the null check that logs `GamePathsNotFound`. On a game start (:133-138)
the paths are logged at Information level even when they are null. The behaviour came over
unchanged from the old `MonitorService`; the monitor-in-Core change moved it into the tested
Core.

**Trigger:** the Steam folder or the user data can't be found from the start (`LogPath` null on
every resolution).

**Expected:** the documented warning appears once. **Observed:** never; every retry gives
`resolved == _paths` and returns early. The warning is reachable only when paths that were
found disappear later. After 2 minutes the only warning is
`The game log (null) gave no usable lines for 2 minutes; showing Playing.`

**Impact:** diagnostics only, but for the most common support case: the user's log doesn't say
why the status only shows the fallback.

**Correction direction:** check `LogPath is null` before the equality return (once per change,
for example with a flag reset when a log path is found), also after the resolution in
`HandleProcessChange`.

**Reproduction:** `core/GameMonitorPathsNotFoundReviewTests.cs`.

```
failed …NoGamePathsAtAll_WithoutTheGame_LogsTheNotFoundWarningAsync
  Expected: 1
  Actual:   0
failed …NoGamePathsAtAll_WhileTheGameRuns_LogsTheNotFoundWarningAsync
  Expected: 1
  Actual:   0
```

The test's first assertion (10 resolutions) passes, so the retries happen; only the warning is
missing.

## R12 — P3: Core types stay public that the app no longer uses; AGENTS.md says interfaces are internal

**Location:** `companion/Tf3Drpc.Core/GameLog/GameLogReader.cs:15`,
`State/GameStateMachine.cs:14`, `State/GameProcessTracker.cs:8`; the six `public interface`
declarations in `Tf3Drpc.Core/Monitor/`; `AGENTS.md:145-147` ("in `Tf3Drpc.Core` only what
the app uses" is public; "Interfaces are internal too").

**Observed:** since the monitor-in-Core change only `GameMonitor` uses the three classes, but
they are still public (the tests see internals through `InternalsVisibleTo`). The six interfaces must be public,
because the app's services inherit them, but AGENTS.md has no exception for that.

**Correction direction:** make the three classes `internal sealed`; add the public monitor
interfaces as an exception to AGENTS.md:147.

**Reproduction:**

```
GameLogReader: declared public sealed class GameLogReader | used in Tf3Drpc.App: 0 files
GameStateMachine: declared public sealed class GameStateMachine | used in Tf3Drpc.App: 0 files
GameProcessTracker: declared public sealed class GameProcessTracker | used in Tf3Drpc.App: 0 files
before that change the app used them in: GameLogService.cs, GameStateService.cs, MonitorService.cs
147:  Interfaces are internal too. …
Tf3Drpc.Core/Monitor/IGamePathResolver.cs:8:public interface IGamePathResolver   (and five more)
```

With the three classes changed to `internal sealed` in a copy:
`dotnet build TF3-DRPC.slnx` → `0 Warning(s) 0 Error(s)`; `dotnet test TF3-DRPC.slnx` →
`total: 643, failed: 0`.

## R13 — P3: A failing `OpenUrl` is silent: no log entry, no feedback

**Location:** `companion/Tf3Drpc.App/ViewModels/SettingsViewModel.cs:199-201`. From the
monitor-in-Core change.

**Trigger:** `Launcher.OpenAsync` returns false or throws, for example a `UriFormatException`
after `AppConstants.ModIoUrl` is edited (its comment says it will be, after the first upload).

**Expected:** the failure is logged through a `[LoggerMessage]`, like the app's other failure
paths. **Observed:** the `bool` result is discarded by the command; an exception ends in the
`AsyncRelayCommand` and is lost. Nothing happens visibly, nothing is logged, and the app keeps
running.

**Correction direction:** try/catch around the launch; log an exception and a `false` result
(a new `[LoggerMessage]` on `ILogger<SettingsViewModel>`); build the `Uri` in a local.

**Reproduction:** a published variant with only the mod.io URL malformed
(`app/openurl-variant.diff`), started from a fresh folder; Space on "mod.io":

```
foreground is app window: True
focused element: mod.io
pressed Space at 19:28:16 in process <pid>
TF3-DRPC processes after Space: 1
browser windows showing mod.io: 0
--- app log (level, event):
INFO [AppStarted]
INFO [TrayCreated]
INFO [GamePathsResolved]
INFO [LanguageChanged]
INFO [GameTextsLoaded]
```

`app/uia-invoke.ps1 -Name mod.io` returns the exception to the automation client:
`Exception calling "Invoke" with "0" argument(s): "One of the identified items was in an invalid format."`

## R14 — P3: The game build line's visibility is a view-model property, against "XAML first"

**Location:** `companion/Tf3Drpc.App/ViewModels/SettingsViewModel.cs:169-173`, `:399-400`;
`Views/SettingsPage.xaml:31-35`. From the game-build change. AGENTS.md:186 puts visibility
rules into XAML, and the page already hides other labels with `DataTrigger`s (`SettingsPage.xaml:50`, `:67`, `:112`).

**Correction direction:** an `int? GameBuild` property, no `IsGameBuildVisible`, and a
`DataTrigger` on `GameBuild` with `Value="{x:Null}"` that hides the label.

**Reproduction:**

```
$ grep -rn -E "GameBuild =|IsGameBuildVisible|partial int GameBuild" companion/Tf3Drpc.App
SettingsViewModel.cs:170:    public partial int GameBuild { get; set; }
SettingsViewModel.cs:173:    public partial bool IsGameBuildVisible { get; set; }
SettingsViewModel.cs:399:        GameBuild = status.Game.GameBuild ?? 0;
SettingsViewModel.cs:400:        IsGameBuildVisible = status.Game.GameBuild is not null;
SettingsPage.xaml:32:                        IsVisible="{Binding IsGameBuildVisible}"
AGENTS.md:186: - **XAML first:** UI that XAML can express (window and title bar properties, visibility rules, …
```

## R15 — P3: docs/companion.md's project table leaves out the monitor

**Location:** `docs/companion.md:214` and `:216`. The monitor-in-Core change added the monitor
to the folder table and the architecture text, but the project table still lists Core as "log reader and parser,
states, presence texts, `.mo` reader, settings file, Discord IPC client", and the tests row
as "every Core part: …" without `GameMonitor`.

**Correction direction:** add "the monitor tick (`GameMonitor`)" to both rows.

**Reproduction:**

```
$ grep -n -E "^\| \`Tf3Drpc.Core\` |^\| \`Tf3Drpc.Core.Tests\` " docs/companion.md
214:| `Tf3Drpc.Core` (`net11.0`, `IsAotCompatible`) | log reader and parser, states, presence texts, `.mo` reader, settings file, Discord IPC client |
216:| `Tf3Drpc.Core.Tests` (`net11.0`) | every Core part: parser and follower …, state machine, presence texts, `.mo` reader, languages, settings, Discord frames and client …
```

---

## Fixes

All fifteen findings are fixed. Each fix was checked with the finding's own reproduction, run
again against the fixed code:

| ID | Fix | Check on the fixed code |
|---|---|---|
| R01 | `[RelayCommand(AllowConcurrentExecutions = true)]` | `app/focus-trace.ps1`: the focus stays on "GitHub", both buttons stay enabled; `app/focus-on-return.ps1`: back in the window, the focus is on "GitHub" |
| R02 | CHANGELOG.md links the README absolutely; the check refuses a relative link in the section | `tooling/repro-release.ps1`: the notes carry the absolute link; with a relative link put back, the check stops with "has a relative link" |
| R03 | The check job fetches the history and requires `git merge-base --is-ancestor $GITHUB_SHA origin/main` | R03 case: "Tag v0.1.0 is on a commit that isn't on main", `exit=1` |
| R04 | Tag and version reach the scripts only through `env:` | R04 case: no `INJECTED-PWSH`; the crafted tag fails the version check as data |
| R05 | The check refuses an empty section; the notes take the lines with `Select-Object` | R05 case: "section for 0.2.0 is empty", `exit=1` |
| R06 | The heading must end with `YYYY-MM-DD` | R06 cases: both headings refused |
| R07 | `build-release.ps1` deletes the platform's bin and publish folders first | `tooling/repro-stale-publish.ps1` on the fixed code: `OldDependency.dll in the zip: False` in both runs |
| R08 | The docs say "every push to `main`" and "a `v<VersionPrefix>` tag on `main`" | `grep` for the old wording finds nothing |
| R09 | "92 files" | first run of R07's script: 92 files, 1 folder |
| R10 | The rewritten-log test without a startup line; tests for M3, M7, M8, M12, M13, M15 | `core/mutate.py` on all mutants: every one fails a shipped test, except M11, whose line the R11 fix removed |
| R11 | `GameMonitor` logs the not-found warning once per lost log path, also on a game start | `core/GameMonitorPathsNotFoundReviewTests.cs` passes (14 of 14 review tests) |
| R12 | The three classes are `internal sealed`; AGENTS.md names the public Monitor interfaces as the exception | the solution builds with 0 warnings |
| R13 | `OpenUrl` catches and logs (`UrlOpenFailed`, `UrlNotOpened`) | the malformed-URL variant: Space on "mod.io" logs `FAIL [UrlOpenFailed] … UriFormatException`; the app keeps running |
| R14 | `int? GameBuild`, visibility through `IsNotNullConverter` in XAML | without the game, no "Build" text in the window |
| R15 | The project table names the monitor tick | `grep` |

The first R14 fix used a `DataTrigger` on `{x:Null}`, as the correction direction suggested.
It didn't work: the published app showed "Transport Fever 3 · Build " without a number, because
a `DataTrigger` only runs when its bound value changes away from its own default, which is null
as well. The converter replaced it.

With the fixes: `dotnet build TF3-DRPC.slnx` in Debug and Release with IDE0005 as a warning,
0 warnings; `dotnet test TF3-DRPC.slnx`, 653 total, 0 failed; the Native AOT publish shows only
the known MAUI IL2070 and the 20 WebView2 ILC messages; actionlint passes apart from the
`xcode-27` label.

---

## Not valid

Each was tested and refuted by the slice reviewers; the second run repeated the review tests
of the first two rows:

| Suspicion | Test and result |
|---|---|
| The monitor-in-Core change changed the monitor's behaviour | The old tick (`MonitorService.cs` before the change) with fields renamed, diffed against `GameMonitor`: the only differences are the returned status and the message send moving to the app, where `SendStatus` catches and logs it |
| A log path that changes while the game runs keeps the old file's position or state | `core/GameMonitorLogSwitchReviewTests.cs`: the follower restarts on a path change and the state goes Playing → MainMenu; passes |
| Cancellation is swallowed, or a cancellation without a cancelled token escapes `TickAsync` | The shipped tests `CancelledToken_PropagatesTheCancellationAsync` and `CancellationWithoutACancelledToken_IsLoggedLikeAnyOtherFailureAsync` pass |
| The build regex accepts a negative or overflowing build, or the `Build: Build 40420 …` echo line | The shipped theory `GameBuildLineWithoutAUsableNumber_IsNotClassified` covers `-5`, `99999999999`, empty, `abc`, no `build name` and the `Build:` line, and passes |
| The game build survives a new process or a log rewrite | `Reset()` clears it; the shipped `GameBuild_IsClearedBy…` tests and the review test `RewrittenLogWithoutAStartupLine_ResetsStateAndBuildAsync` pass |
| The build line suppresses the 2-minute fallback | In the real start (`docs/log-samples/startup.log`) the build line follows `Application startup. Hello!`, which counts as an event already; `StartupLog_ProducesStartupThenTheGameBuild` passes |
| The footer overlaps at the minimum window size or with the longest Exit text | 400 × 500 DIP in German and Dutch ("Afsluiten", the longest of the 13): no overlap ([German](evidence/footer-minimum-size-de.png), [Dutch](evidence/footer-minimum-size-nl.png)) |
| The new buttons are hard to reach for assistive technology | UI Automation: names "GitHub" and "mod.io", the URL as help text, keyboard focusable, order GitHub, mod.io, Exit |
| Unused usings or dead code after removing `IGameLogService` and `IGameStateService` | IDE0005 as a warning (AGENTS.md's check), Debug and Release: 0 warnings; every remaining `AppConstants` member is used |
| New trim or AOT warnings from the footer buttons | The Native AOT publish prints only the known MAUI IL2070 and the 20 known WebView2 ILC messages |
| The action pins don't match their version comments | Each tag resolved through `gh api …/git/ref/tags/<v>` (annotated tags dereferenced): all four match |
| `working-directory: companion` breaks the step that runs before the checkout | `ci.yml` sets `working-directory: .` on that step; the pull request run and the push run passed |
| The changelog match `-like "## $version *"` also matches 0.1.0x or 0.1.01, or a last section without a following heading fails | A space is required after the version, so neither matches; a last section uses `$end = $lines.Count` and works |
| `CopyrightYears` is wrong in a later year | Evaluated with a first year of 2025: `© 2025–2026 Zagrthos. MIT Licence.`; with 2026: `© 2026 Zagrthos. MIT Licence.` |
| The release zip's DATA folder or a hidden file ships | No DATA entries in the CI zip. `Compress-Archive` does drop hidden files, but the publish output has none |
| `ubuntu-slim` lacks PowerShell or the GitHub CLI | The image's readme lists PowerShell 7.6.7 and GitHub CLI 2.103.0 |
| `game-tests.ps1` `Get-GameStatus` misreads without the build line or when not connected | Fed crafted text lists: returns the status line in each case |
| The size and file statements in the README are wrong | The CI zip unpacks to 82.6 MB ("about 83 MB") with a 24.6 MB exe ("about 25 MB") |

Not tested, because they need real GitHub runs or a running game, which were out of bounds:
`gh release create` on a re-run after the release exists or on a tag pushed twice; a queued
push run replaced under `concurrency`; the build line while the game runs.
