# game_mechanics.towns.town_cargo

Definition file: `base/tealdef/game_mechanics/towns/town_cargo.d.tl` (in the game folder).

Town experience and level state of the town growth script (`town_cargo.gs`) and its configuration.

## TownsGrowthState (record)

```teal
global record TownsGrowthState
```

State of the town growth game script `town_cargo.gs` (`town_growth.script`). Read it with `TownCargoUtil.getTownCargoState`.

- `townState : {Engine.Entity : TownData}`: Growth data per town entity.
- `cargoDemandsSorted : {CargoTypeId}`: Sorted ids of all cargo types that any town demands. When the list changes, the script sends the `Towns`/`CargoDemandsChanged` event.
- `disabledCelebratoryFireworks : boolean`: True after the `DisableCelebratoryFireworks` event; level-ups then start no fireworks.

### TownsGrowthState.TownData (record)

```teal
record TownData
```

Experience and level of one town.

- `experience : integer`: Experience points the town has collected in total.
- `initialLevel : integer`: Town level when the state was first created. `TownCargoUtil.getGrowthFactor` counts levels from here.
- `level : integer`: Current town level. Each increase sends the `Towns`/`OnLevelUp` script event and a level-up notification.
- `cargoDelivered : boolean`: True once any cargo item was delivered to a building of the town. Until then a level-up adds no new cargo demands.
- `accumulatedExperience : {string : {integer : TownsGrowthState.TownData.AddedExperience}}`: per category (cargo, passenger) and tier

#### TownsGrowthState.TownData.AddedExperience (record)

```teal
record AddedExperience
```

Experience gained from one source and tier since the last journal log.

- `rawExperience : integer`: Experience before the town's experience factor (`TownUtil.getXpFactor`) was applied. Logged as `experience_raw_<tier>_<source>`.
- `multipliedExperience : integer`: Experience after the experience factor was applied, as added to `experience`. Logged as `experience_gain_<tier>_<source>`.

## TownCargoUtil (record)

```teal
global record TownCargoUtil
```
Load: `local town_cargo_util = ug_require "::/game_mechanics/towns/town_cargo_util.tl" as TownCargoUtil`

Constants and state access of the town growth script (`game_mechanics/towns/town_cargo_util.tl`).

- `growthPerTick : number`: Experience added to every town on each script update. The base game sets it to `0` (off).
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_growth.script.tl:197`: `if town_cargo_util.growthPerTick > 0 then`
- `levelUpSizeFactor : number`: Set to `1.25` with the code comment "Level up size increase (compound)". No base-game script reads it.
- `levelUpFactorRange : number`: Set to `0.02` with the code comment "Level up size increase (compound)". No base-game script reads it.
- `townExpScaling : number`: Ratio between the experience needed for a level and for the level before it. The base game sets it to `1.5`.
- `authority2xpFactor : {RatingLevel : number}`: Experience multiplier per town growth level, from `0.0` at `VeryPoor` to `1.0` at `Excellent` in steps of `0.2`.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:958`: `local xpFactor = town_cargo_util.authority2xpFactor[growthLevel]`
- `ensureState : function(scriptState : TownsGrowthState, townEntity : Engine.Entity)`  (call as `town_cargo_util.ensureState(…)`): Creates the growth data of a town if it is missing. A new town starts at level 2, 3 or 4 when its initial residential capacity is above 100, 150 or 200 (otherwise level 1), with the experience that level needs.
    - param `scriptState`: State of the town growth script; changed in place.
    - param `townEntity`: Town entity.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_growth.script.tl:195`: `town_cargo_util.ensureState(scriptState, townEntity)`
- `getTownCargoState : function(entity : Engine.Entity) : TownsGrowthState.TownData`  (call as `town_cargo_util.getTownCargoState(…)`): Reads the growth data of a town from the `town_cargo.gs` game script component.
    - param `entity`: Town entity.
    - returns: The town's experience and level data.
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:882`: `local state = town_cargo_util.getTownCargoState(params.entity)`
- `getLevelAndFraction : function(exp : integer) : integer, number, integer, integer`  (call as `town_cargo_util.getLevelAndFraction(…)`): return: current level, fraction/percentage reached of current level, experience needed for current level, experience needed for next level
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_growth.script.tl:87`: `local newLevel = town_cargo_util.getLevelAndFraction(exp)`
- `getGrowthFactor : function(townEntity : Engine.Entity, scriptState : TownsGrowthState) : number`  (call as `town_cargo_util.getGrowthFactor(…)`): Growth factor of a town from `TownGrowthFunction.getGrowthFactor`, based on the levels gained since `initialLevel` plus the fraction of the current level.
    - param `townEntity`: Town entity.
    - param `scriptState`: State of the town growth script.
    - returns: Growth factor of the town.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_growth.script.tl:373`: `local baseFactor = town_cargo_util.getGrowthFactor(townEntity, scriptState)`

## TownGrowthConfig (record)

```teal
global record TownGrowthConfig
```

Experience settings of the town growth script, loaded from `town_growth_config.res`.

- `expPerCargoItemToTown : integer`: Base experience for each cargo item delivered to a town building (`10`). Multiplied by `2^tier` of the cargo category and by the town's experience factor.
- `expPerPersonArriveAtTown : integer`: Base experience for each person who arrives in the town by line (`5`). Multiplied by `2^tier`, where the tier is 0 for trips inside the town, 1 from a neighbour town and 2 from any other town.
- `expBaseTownLevel : integer`: Experience needed to reach level 2 (`1000`). Each further level needs `TownCargoUtil.townExpScaling` times as much as the one before.
