# The mod

`mod/zagrthos_drpc/` (mod id `zagrthos_drpc`): `mod.json`, `_metadata/modinfo.json`,
`content/drpc/drpc.gs.lua` (registers the game script) and `content/drpc/drpc.script.lua`
(the code). It writes the [status line](status-line.md).

## How it works

- A game script with one function, `guiUpdate`. It runs once per frame on the game's main
  thread and changes nothing in the simulation. With the pause button it keeps running and
  the status says `speedup` 0; while the game menu (Escape) is open it doesn't run, so no
  lines are written and the companion infers the pause (seen on 2026-10-17).
- Every ~10 s of real time it collects the game state and writes one status line to the
  game log with `log.message`.
- It never calls `state:set`, so it keeps no state of its own: `severityRemove: "None"` and
  `cosmetic: true` (achievements stay available). The game itself stores an entity for the
  registered game script with the savegame (`Creating entity for newly added GameScript
  zagrthos_drpc::/drpc/drpc.gs` in the log) and removes it when a save is loaded without the
  mod (`Removing entity 90284 for GameScript zagrthos_drpc::/drpc/drpc.gs which is no
  longer available`, followed by `No resources are missing!`; seen on 2026-10-17).
- Players turn it on per game: in the mod list of a new game, or through a savegame's gear
  button in the load dialogue. Adding it to an existing save and removing it again both work.
- `"autoActivate": true` in `mod.json` (the DLCs use it) doesn't turn the mod on: tested on
  2026-10-17 with a new game and an old save, the mod stayed unticked in both. So `mod.json`
  doesn't set it.
- `rank` is the earned rank (`potentialLevel`), which the game bar shows; `level` only
  follows once the player applies the promotion in the company window.
- Campaign missions start with only their own mod, so the mod doesn't run there.

## Facts from the test runs

- The engine side of a game script (`update`, `handleEvent`) runs in several Lua states,
  one per simulation thread, without shared module variables. The GUI side has one state.
- In the GUI state `io` and `package.loadlib` are `nil`; `os` and `app` exist.
- `os.clock()` is wall-clock time on Windows but CPU time on macOS, so the interval uses
  `api.util.getLocalTime()` (POSIX seconds).
- A `.script.lua` file returns its functions from `function data()`; the `.gs.lua` file
  refers to them as `drpc.script@guiUpdate`.
- After a script change, loading a game is enough; no game restart.
- `guiHandleEvent` gets no selection events, so the mod can't tell what the player clicked.
- Collecting the status takes under 1 ms (save with 74 stations, 118 vehicles).
- **Why the log and not a file:** `app.saveUserdata` only accepts the directories
  `towns_industries`, `heightmaps` and `mod_presets`, and no other API function writes a
  file of our own.

## Testing

`tools/install-mod.ps1` links `mod/zagrthos_drpc/` into the staging area
(`<userdata>\staging_area\zagrthos_drpc`, "My Mods" in the game); `-Remove` deletes the
link. Activate "Discord Rich Presence" for a game **and save once**, or the activation is
lost with the next load. Then look for `[drpc-status]` lines in the [game log](game-log.md).
The full procedure, with what to look for, is in [testing.md](testing.md). On macOS the
script makes a symbolic link instead of a directory junction; the user data folder is the same
as on Windows (see [game-log.md](game-log.md#finding-the-game-and-its-log)).

The `getLocalTime()` interval works in game: one line every 10 s (end-to-end tests on
2026-10-14 and 2026-10-17). The reviewed build, with the hardening after the first review
(NaN and infinity left out, the clock read inside `pcall`, a clock set back) and the fixes of
the second review (64-bit-safe integers, locale-independent numbers and strings, one `pcall`
per value), ran in game on 2026-10-17 without a `[drpc]` warning. Its edge cases didn't
occur there (the balance stayed below 2^31, the clock wasn't set back).

The second review ([reviews/mod/2026-10-15/](../reviews/mod/2026-10-15/)) runs the
unchanged script under Lua 5.2 with a fixture of the game API. Its `review.py` asserts the
defects as they were before the fixes, so after them its four `F` checks fail by design;
its eight controls must still pass.

The third review ([reviews/mod/2026-10-17/](../reviews/mod/2026-10-17/)) led to more fault
isolation: each date field (`year`, `month`, `day`) has its own `pcall` after one read of
the calendar record, a value of an unexpected type is left out of the line and logged once
per key, a clock value that is not a finite number is treated like a failed read, and a
malformed configuration pair is skipped without losing the valid ones. Its `review.py`
(Lupa 2.8, see its README) likewise asserts the old behaviour: after the fixes `R1`, `O1`,
`O2` and `O3` fail by design, while its controls `C01`–`C13` must still pass. These changes
ran in game on 2026-10-18: all 46 keys, the date and climate present, no `[drpc]` warning.

## References

Game paths are relative to `steamapps/common/Transport Fever 3/` (Windows); "Reference"
paths are pages of the TF3 modding reference.

- Game: `api/tealdef/api/engine.d.tl`, `api/tealdef/api/gui.d.tl`
- Game: `base/content/gui.zip` → `gui/main/game.tl`, `gui/game_bar/game_bar.tl`,
  `gui/game_bar/game_bar_widgets.tl` (how the game bar reads date, speed, rank and money)
- Game: `base/content/game_mechanics.zip` → `company/company_progression_util.tl`,
  `company/company_static_util.tl`
- Game: `mods/release/urbangames_sandbox`, `urbangames_tycoon` (how the modes are set)
- Reference: `guides/scripting.md`, `guides/mod-structure.md`
- Wiki: <https://wiki.transportfever3.com/doku.php?id=modding:start>
