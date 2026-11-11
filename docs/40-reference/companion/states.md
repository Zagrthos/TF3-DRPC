---
title: Game states
type: reference
audience: [contributor, agent]
area: [app]
covers:
  - companion/Tf3Drpc.Core/State/**
  - companion/Tf3Drpc.Core/Constants/GameStateConstants.cs
last-reviewed: 2026-11-11
---

# Game states

The states of the [companion app](README.md) are built from the process check and the [log lines](../game-log.md#lines-that-matter):

| State | Entered on | Discord |
|---|---|---|
| Not running | no game process | cleared |
| Starting | process found, `Application startup. Hello!` | "Starting the game" |
| Main menu | `Main menu is ready` | "In the main menu" |
| Loading | `Loading game from file`, `Campaign/mission:` | the game's `Loading...` |
| Mission | `Game is ready` after `Campaign/mission:` | "Campaign · <mission name>" |
| Playing | `Game is ready` otherwise | design A with a fresh mod status, otherwise "Playing" |
| Playing (fallback) | the process runs for 2 min and no usable log line arrived | "Playing", timer since the process start |
| Closing | `Normal shutdown. Bye!`, `Ungraceful exit` (not `Quit requested`, after which the game can stay open) | cleared |

- In Playing the latest mod status is kept.
  The mod writes every 10 s, with `speedup` 0 while paused with the pause button, and nothing while the game menu is open, during loading or in missions.
  So the game counts as paused when `speedup` is 0 or no new status line came for more than 20 s.
  The next line with a speed ends the pause.
- Without the game process the state is "Not running", whatever the log says.
- A game process that replaces the old one between two checks (a different start time) counts as a new start: the state resets and the app finds the paths and reads the log from the start again.
- Lines from before the game process started (more than 2 s) belong to the previous session and are ignored; see [reading the game log](../game-log.md#reading-it).
  Once a line of the current session was accepted, the check ends for that process, so a clock set back later doesn't make the running session look old.
  A log dated more than a minute in the future isn't accepted as the running session's (and loses the account choice to a current log), so the monitor keeps resolving the paths until the right one turns up.
- **Timer:** since `Game is ready` of the current game; in the main menu since the game start.
