# game_mechanics.emission.emissions

Definition file: `base/tealdef/game_mechanics/emission/emissions.d.tl` (in the game folder).

State of the emissions game script (eco levels of industries).

## EmissionSimState (record)

```teal
global record EmissionSimState
```

State of the emissions game script.

- `ecoLevels : {Engine.Entity : {EntityUtil.EntityAndRevision, integer}}`: Industry construction -> `{ entity and revision, eco level }`; set to level 1 by the industry-greenify perk.
- `version : integer`: Layout version of the state.

## EmissionLegacyUtil (record)

```teal
global record EmissionLegacyUtil
```

Upgrades an emissions state saved by an older game version.

- `currentVersion : integer`: Current layout version.
- `updateLegacyState : function(townsState : EmissionSimState)`: Upgrades the state in place (e.g. converts old eco levels to `{entity and revision, level}`).
    - param `townsState`: State to upgrade.
