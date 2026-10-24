# game_mechanics.industries.industries

Definition file: `base/tealdef/game_mechanics/industries/industries.d.tl` (in the game folder).

State and config of the industries game script (expansion and closure of industries).

## IndustriesState (record)

```teal
global record IndustriesState
```

State of the industries game script.

- `toSpawn : integer`
- `industryFailedExtensions : {Engine.Entity : FailedExpansion}`: Industry -> last failed expansion.
- `pauseIndustryClosure : boolean`: While `true`, industries without depots do not start their closure countdown, and running countdowns of unowned industries are reset. The tutorial turns it on through the `setPauseIndustryClosure` event of the `Industries` script and turns it off when it ends.
- `version : integer`: Version of the saved state (1 at present). `handleLegacy` upgrades older saves; a save without a version counts as 0 and pauses the industry closure if the tutorial is active.

### IndustriesState.FailedExpansion (record)

```teal
record FailedExpansion
```

A failed expansion of an industry.

- `triangles : {{Vec2f, Vec2f, Vec2f}}`: Triangles of the area that blocked the expansion.
- `tick : integer`: Update count at which the expansion failed.

## IndustriesConfig (record)

```teal
global record IndustriesConfig
```

Config of the industries game script (`game_script_config` resource `industries_config.res`).

- `extendTimeSpanYears : number`: Game years between expansion attempts (0.5).
- `unusedTimeSpanYears : number`: Game years without consumed or shipped goods after which an industry may close (5).
- `closureCountdownTimeSpanYears : number`: Game years from the closure notice until the industry despawns (2); the closure is cancelled if the industry is used again.
