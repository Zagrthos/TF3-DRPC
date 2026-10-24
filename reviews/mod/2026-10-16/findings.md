# Review of mod/ of 2026-10-16

Two findings: one P2 behaviour defect and one P3 documentation defect with a player-facing
text. Production files were not changed. Run the reproductions with
`python -I review.py <repository root> [<game folder>]` (Python 3.12+, `lupa==2.8`); they load
the unchanged script into Lua 5.2 with the fixture of the 2026-10-15 review and read the
installed game (build 40420).

## R1 - P2: The rank is the applied level, not the rank the game shows

**Location:** `mod/zagrthos_drpc/content/drpc/drpc.script.lua:157` (`companyData.level`) and
`:160` (`getRole(status.rank)`); the contract in `docs/status-line.md` (`rank` row).

The company progression state has two levels. `company_growth.script.tl:47` sets
`potentialLevel` from the experience every update; `level` only rises in `applyLevel`
(`company_growth.script.tl:60`), which runs when the player selects the new rank in the company
window (`company.tl:422` sends `applyLevel`). The game bar shows the earned rank:
`game_bar.tl:403` `result.rank = potentialLevel` and `game_bar.tl:420`
`role = company_static_util.getRole(state:old().rank)`, with level-up particles while
`applyDiff = potentialLevel - level` (`game_bar.tl:408`) is positive.

The mod writes `level`. After every promotion, until the player opens the company window,
Discord shows the previous rank while the game bar already shows the new title. The status has
no other key from which the companion could get the shown rank.

**Reproduction:** `R1_pending_promotion`. Fixture state `level = 2, potentialLevel = 3`, one
`guiUpdate`. **Expected:** rank 3 (Engineer), as in the game bar. **Observed:** `"rank":2`,
`"rankName":"Mechanic"`, no potential rank in the line. Controls (`R1_controls`): with no
pending promotion and in tycoon mode (both levels 15) the rank matches.

**Correction:** write the shown rank (`potentialLevel`) as `rank`, or add it as its own key
(for example `potentialRank`) and let the companion prefer it. Changing the meaning of `rank`
needs `v` raised (AGENTS.md). Either way `rankName` should use the same level.

## R2 - P3: The mod does leave an entity in the savegame

**Locations:** `mod/zagrthos_drpc/content/drpc/drpc.script.lua:9-10` ("nothing ends up in the
savegame and the mod can be removed at any time"), `mod/zagrthos_drpc/_metadata/modinfo.json:4`
(player-facing: "stores nothing in your savegame"), `docs/mod.md:14`.

Registering a game script makes the engine create an entity with a `GameScript` component
when a savegame is loaded with the mod; that entity is saved with the game. The real log
records it (`docs/log-samples/session.log:12`: `Creating entity for newly added GameScript
zagrthos_drpc::/drpc/drpc.gs`). The engine also has the matching clean-up
(`Removing entity {} for GameScript {} which is no longer available` in `TransportFever3.exe`),
so `severityRemove: "None"` is plausible, but the claim is wrong, and removal was never tried
(`docs/mod.md`).

**Reproduction:** `R2_savegame_entity` checks the real log line, both engine messages in the
installed executable, and the three claims. In game: load a save with the mod, then search the
game log for `Creating entity for newly added GameScript zagrthos_drpc`.

**Correction:** say that the mod keeps no state of its own in the savegame (it never calls
`state:set`) and that the game removes the script's entity when the mod is gone; test the
removal once (load a saved game without the mod and look for the `Removing entity` line) before
publishing with `severityRemove: "None"`.

## Checked and not flagged

- Every API call and field against the definition files of build 40420 (`getLocalTime`,
  `getDefaultSavegameId`, `isMapEditor`, `getBaseConfig().sandboxMode`, `genericRep`, the
  components `GAME_TIME`, `GAME_SPEED`, `WORLD`, `ACCOUNT`, `getCalendarDate`,
  `getPlayersBalance`, `getTransportedData`, all 24 `CompanyValue` fields, `getConfigDict`).
- `getConfigDict` is declared `{string : string}` in `engine.d.tl:1804`, but the game's own
  mod hooks type it `{{string, string}}` and the real status line contains the climate, so the
  pair iteration is right.
- A nil balance means infinite money (`util.d.tl:860`, `game_bar.tl:219`); the date, sandbox
  and map-editor reads match the game's own `game_bar_widgets.tl:259-260` and `game.tl:731-732`;
  tycoon detection matches `urbangames_tycoon` (`enforcedLevel = 15`).
- `saveName` is the name the pause menu shows (`pause_menu.tl:394`).
- `timestamp` is POSIX UTC: all three real status lines match their UTC log stamps on a
  machine in CEST.
- JSON and numbers under the C, German numeric and German ctype locales, 64-bit integers,
  NaN and infinity, read isolation and the 10 s cadence (the 2026-10-15 harness controls and
  `controls_previous_fixes`).
- `mod.json` and `modinfo.json` keys against the modding reference; `cosmetic` is a real key;
  the cover image `_metadata/0.png` is optional (presentation guideline only).
- Lua language server: no diagnostics.
- Not verifiable without a game session: behaviour in the map editor, and whether `guiUpdate`
  runs while the window is minimised.
