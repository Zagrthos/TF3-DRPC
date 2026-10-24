# Review of companion/ of 2026-10-16

Two findings, both P2, each reproduced with the unchanged code. Production files were not
changed.

The `harness/` folder was removed on 2026-10-20, after every finding was fixed: it targets
Windows only and broke loading the projects on macOS.

Run from `reviews/companion/2026-10-16/harness/` with the SDK pinned in `companion/global.json`:

```powershell
dotnet run --project ReviewHarness.csproj            # all cases
dotnet run --project ReviewHarness.csproj -- N1a     # one case
dotnet run --project ReviewHarness.csproj -- probe   # real game data: .mo keys, profile.lua, dates
dotnet run --project ReviewHarness.csproj -- dst     # start-time conversion at the DST change
```

The harness compiles `Tf3Drpc.Core` and the app's `MonitorService`, `TranslationService`,
`GameLogService`, `GameStateService`, `PresenceService`, constants, interfaces and structs from
the repository unchanged. It fakes only the process check, the path service, the settings and
the Discord service, and stubs MAUI's main-thread dispatch. `MonitorService.TickAsync` is
called through reflection; the real loop only adds the 1 s timer around it. The clock can move
its wall time independently of its monotonic timestamp. `REPRODUCED` asserts the defect; each
finding has a control that passes.

## N1 - P2: Three schedules still run on the wall clock

**Locations:**
- `Tf3Drpc.App/Services/MonitorService.cs:148` takes `now` from `GetUtcNow()`, which drives
  the path retry (`:173`, `:186`, `:196`) and the profile re-read (`:241`, `:245`).
- `Tf3Drpc.Core/State/GameStateMachine.cs:142` decides the pause from
  `GetUtcNow() - _statusTimestamp`.

The review fix F08 moved the Discord send and reconnect intervals to the monotonic clock, and
F04 did the same for the translation retry. These three intervals were left on the wall clock,
so a clock set back (a manual change or an NTP correction) suspends them until the wall clock
catches up again.

| Case | Trigger | Expected | Observed | Control (no rollback) |
|---|---|---|---|---|
| `N1a` | game language changed in `profile.lua` (de to fr) after a 1 h rollback, 60 s of real time | `fr` within 30 s (`ProfileReadInterval`) | still `de`; unchanged for up to one hour | `fr` |
| `N1b` | game files not found at first, present after a 1 h rollback, 60 s | found within 15 s (`PathRetryInterval`) | 0 resolve calls, still not found | found after 1 call |
| `N1c` | Playing, then no status line for 30 s after a 1 h rollback | paused after 20 s (`PauseAfter`) | `IsPaused` false; Discord never shows "Paused" for an hour | paused |

N1b also covers the wrong-account check in `EnsurePaths`, which is behind the same
`_nextPathAttempt` gate.

**Correction:** keep the two monitor schedules as `GetTimestamp()` values compared with
`GetElapsedTime`, as F04 and F08 do. For the pause, record a monotonic timestamp when a
status line is applied and measure the 20 s from it, instead of comparing the wall clock
with the log time.

## N2 - P2: Without a recognised log line the presence says "Starting the game" all session

**Location:** `Tf3Drpc.Core/State/GameStateMachine.cs:19` and `:54`: a running process starts
in `Starting`, and only recognised log events move it on. `MonitorService.cs:215-216` skips the
poll when no log path was found.

`docs/game-log.md:84` promises the opposite: "the app skips unknown lines and falls back to
'Playing' while the process runs". The state only leaves `Starting` through
`Main menu is ready`, `Loading game from file`, `Game is ready` and the like. If none of them can
be read, Discord says "Starting the game" for the whole session. That happens, for example:

- after a game update changes the log header (the log isn't a documented interface, as
  the same document says);
- when the user data isn't found (no `SteamPath` in the registry, another store, a game
  library elsewhere with its log elsewhere);
- when the log stays unreadable or looks like the previous session.

| Case | Trigger | Observed after 10 min of the game running |
|---|---|---|
| `N2a` | the real session's lines with milliseconds added to the log time | state `Starting`, Discord "Starting the game" |
| `N2b` | no log path (user data not found) | state `Starting`, Discord "Starting the game" |
| `N2-control` | the same session in today's format | `Playing`, Discord "Playing" after one tick |

**Correction:** fall back to `Playing` (the text "Playing") once the process has run for a
while (for example 2 min) and no recognised event has arrived since its start. Or treat a
process with no usable log as Playing from the start. Log once that the game log couldn't be
used.

## Checked and not flagged

- **Game texts against the installed game (build 40420):** all 33 keys the presence uses are
  read from `base.mo` in all 13 languages (none only with a msgctxt). The English file lacks
  the plain keys, but those are their own English fallback (`probe`).
- **Real `profile.lua`:** parses to `de` (`probe`).
- **Presence in all 13 cultures:** `PresenceBuilder` gives the game's rank name and a
  Gregorian month and year, for example `1905年9月 · 一時停止`,
  `setembro de 1905 · Pausado` (`probe`).
- **Process start time at the DST change:** `new DateTimeOffset(process.StartTime)` converts the
  first 02:30 of 2026-10-25 in W. Europe correctly to 00:30Z, because `FromFileTime` keeps the
  ambiguous-DST flag (`dst`).
- **The model editor** runs as `ModelEditor.exe`, not as `TransportFever3`, so it isn't
  mistaken for the game.
- **Log levels:** the per-tick and per-attempt messages (`DiscordSendDeferred`,
  `DiscordNotAvailable`, `GameLogPolled`) are Debug and stay out of the Release log.
- **The remaining wall-clock uses** (`MauiProgram.cs:69`, `:77`) only date the log files and
  their retention, which is correct.
- **Already covered elsewhere:** the earlier fixes F01-F13, with their tests and the published
  smoke run. Rank versus potential rank is mod finding R1 (`reviews/mod/2026-10-16`).
- **Not verifiable here:** the MAUI settings window and tray interaction (no UI automation in
  this review), macOS, and Discord's real text-length limits.
