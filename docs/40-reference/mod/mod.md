---
title: The mod
type: reference
audience: [contributor, agent]
area: [mod]
covers:
  - mod/zagrthos_drpc/**
last-reviewed: 2026-11-11
verified:
  - windows 2026-10-18
---

# The mod

`mod/zagrthos_drpc/` (mod id `zagrthos_drpc`) holds `mod.json`, `_metadata/modinfo.json`, `content/drpc/drpc.gs.lua` (registers the game script) and `content/drpc/drpc.script.lua` (the code).
It writes the [status line](status-line.md).
How to run it from a checkout is in [work on the mod](../../20-development/mod-development.md).

## How it works

- A game script with one function, `guiUpdate`.
  It runs once per frame on the game's main thread and changes nothing in the simulation.
  With the pause button it keeps running and the status says `speedup` 0.
  While the game menu (Escape) is open it doesn't run, so no lines are written and the companion infers the pause.
- Every ~10 s of real time it collects the game state and writes one status line to the game log with `log.message`.
- The clock is read inside its own `pcall`, and a clock value that is not a finite number is treated like a failed read.
- It never calls `state:set`, so it keeps no state of its own: `severityRemove: "None"` and `cosmetic: true` (achievements stay available).
  The game itself stores an entity for the registered game script with the savegame (`Creating entity for newly added GameScript zagrthos_drpc::/drpc/drpc.gs` in the log).
  The game removes that entity when a save is loaded without the mod (`Removing entity 90284 for GameScript zagrthos_drpc::/drpc/drpc.gs which is no longer available`, followed by `No resources are missing!`).
- Players turn it on per game: in the mod list of a new game, or through a savegame's gear button in the load dialogue.
  Adding it to an existing save and removing it again both work.
- `"autoActivate": true` in `mod.json` (the DLCs use it) doesn't turn the mod on: with a new game and with an old save, the mod stays unticked.
  So `mod.json` doesn't set it.
- `rank` is the earned rank (`potentialLevel`), which the game bar shows; `level` only follows once the player applies the promotion in the company window.
- Campaign missions start with only their own mod, so the mod doesn't run there.

## Facts from the test runs

- The engine side of a game script (`update`, `handleEvent`) runs in several Lua states, one per simulation thread, without shared module variables.
  The GUI side has one state.
- In the GUI state `io` and `package.loadlib` are `nil`; `os` and `app` exist.
- `os.clock()` is wall-clock time on Windows but CPU time on macOS, so the interval uses `api.util.getLocalTime()` (POSIX seconds).
  That interval works in game: one line every 10 s.
- A `.script.lua` file returns its functions from `function data()`; the `.gs.lua` file refers to them as `drpc.script@guiUpdate`.
- After a script change, loading a game is enough; no game restart.
- `guiHandleEvent` gets no selection events, so the mod can't tell what the player clicked.
- Collecting the status takes under 1 ms (save with 74 stations, 118 vehicles).
- **Why the log and not a file:** `app.saveUserdata` only accepts the directories `towns_industries`, `heightmaps` and `mod_presets`, and no other API function writes a file of our own.

## References

Game paths are relative to `steamapps/common/Transport Fever 3/` (Windows); "Reference" paths are pages of the TF3 modding reference.

- Game: `api/tealdef/api/engine.d.tl`, `api/tealdef/api/gui.d.tl`
- Game: `base/content/gui.zip` → `gui/main/game.tl`, `gui/game_bar/game_bar.tl`, `gui/game_bar/game_bar_widgets.tl` (how the game bar reads date, speed, rank and money)
- Game: `base/content/game_mechanics.zip` → `company/company_progression_util.tl`, `company/company_static_util.tl`
- Game: `mods/release/urbangames_sandbox`, `urbangames_tycoon` (how the modes are set)
- Reference: `guides/scripting.md`, `guides/mod-structure.md`
- Wiki: <https://wiki.transportfever3.com/doku.php?id=modding:start>
