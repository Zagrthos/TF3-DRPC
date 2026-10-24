# The status line

The interface between the [mod](mod.md) and the [companion app](companion.md). Every ~10 s
of real time the mod writes one line to the [game log](game-log.md):

```
[2026-10-13 21:15:25Z - MESSAGE  - Main Thread      - Main           ]  [drpc-status] {"balance":14422756,"cargoTransported":9424,"climate":"::/climates/tropical/tropical.clima",...,"saveName":"Archipelago2","speedup":4,"timestamp":1791926125,"v":1,"year":1905}
```

Everything after `[drpc-status] ` is one JSON object with sorted keys. A key is missing if
the game couldn't provide it. Example values in brackets:

| Key | Type | Meaning | Source in the game API |
|---|---|---|---|
| `v` | number | Format version, `1`; raised when keys change meaning | |
| `timestamp` | number | Real time, POSIX seconds | `api.util.getLocalTime()` |
| `saveName` | string | Savegame name (`Archipelago2`); before the first save "New Game" in the game's language (`Neues Spiel`), or the map's name for a new game on a ready-made map (`Emerald Shores`, seen 2026-10-21) | `api.gui.game.getDefaultSavegameId()` |
| `isMapEditor` | boolean | Map editor (seen 2026-10-20) | `api.gui.game.isMapEditor()` |
| `isSandbox` | boolean | Sandbox mode, the built-in mod "Sandbox Mode" (seen 2026-10-20) | `api.res.getBaseConfig().sandboxMode` |
| `isTycoon` | boolean | Tycoon mode, the built-in mod "Tycoon"; `rank` is then always 15 (seen 2026-10-20) | `enforcedLevel == 15` in `company_growth_config.res` |
| `year`, `month`, `day` | number | In-game date (`1905`, `9`, `30`) | `GAME_TIME` → `api.engine.util.getCalendarDate` |
| `timeOfDaySec` | number | In-game time of day, seconds since midnight | `GAME_TIME` |
| `speedup` | number | `1`, `2` or `4`; `0` when paused with the pause button, and always `0` in the map editor. While the game menu (Escape) is open `guiUpdate` doesn't run and no line is written | `GAME_SPEED` |
| `rank` | number | Company rank 1–15 (`2`) as the game bar shows it, also before the player applies a promotion; missing right after a new game starts | `potentialLevel` (otherwise `level`) of `company_progression_util.getCompanyProgressionState` |
| `rankName` | string | Rank name in the game's language (`Mechaniker`) | `company_static_util.getRole` |
| `experience` | number | Experience points (`2673`) | as `rank` |
| `balance` | number | Money (`$` in the game); missing with infinite money | `api.engine.util.finance.getPlayersBalance` |
| `infiniteMoney` | boolean | Infinite money | as `balance` |
| `loan` | number | Loan | `ACCOUNT` |
| `passengersTransported`, `cargoTransported` | number | Totals so far | `api.engine.util.headquarters.getTransportedData()` |
| `company_<field>` | number | `totalScore`, `totalAssets`, `debt`, `railVehicles`, `trams`, `roadVehicles`, `aircrafts`, `ships`, `numberOfLines`, `totalStations`, `railStations`, `tramStations`, `roadStations`, `aircraftStations`, `shipStations`, `trackTotalLength`, `trackElectricLength`, `roadTotalLength`, `bridgeTotalLength`, `tunnelTotalLength` (metres), `suppliedTowns`, `connectedIndustries`, `topSpeed` (m/s), `topLength` (m) | `api.engine.util.headquarters.getCompaniesValue()` (the finance window's data) |
| `cloudCoverage` | number | Weather, 0 to 1 | `WORLD` |
| `climate`, `economy` | string | Resource names (`::/climates/tropical/tropical.clima`, `::/economy/tropic.eco`) | `api.engine.config.getConfigDict()` (a list of `{key, value}` pairs; a pair that isn't two strings is skipped) |

- A line is about 1.3 KB, roughly 0.5 MB of log per hour of play.
- Each value is read inside its own `pcall`, and a shared source (the game time, the company
  data) is read once: if an API call breaks after a game update, only the keys that depend
  on it are missing and the mod logs one `[drpc] reading <name> failed` warning. The date
  is converted once and `year`, `month` and `day` are then read one by one.
- A value whose type differs from the table above is left out, and the mod logs one
  `[drpc] <key> has an unexpected type <type>` warning per key. Otherwise the companion
  would reject the whole line.
- **Numbers:**
  - Whole numbers below 2^53 are written as integers with `%.0f`. Not `%d`: the game's
    Windows Lua accepts only 32-bit values there, so a balance above about 2.1 billion
    would break the whole line.
  - Other numbers are written with six significant digits (`%.6g`, e.g. `8.3333` or `1.44228e+07`). The companion reads such a value for an integer key leniently, rounding toward zero.
  - The decimal separator is always a dot, whatever the C locale's numeric setting.
  - NaN and infinity are left out, because JSON has no form for them.
- **Strings** escape exactly what JSON requires (bytes 0–31, `"` and `\`); UTF-8 bytes
  stay as they are under every C locale.
- The mod sends everything; the companion decides what Discord shows, so the mod has no
  parameters.
- `rankName` and a "New Game" `saveName` are in the game's language; see
  [presence.md](presence.md#texts-and-languages).
- Real lines: [log-samples/session.log](log-samples/session.log).
