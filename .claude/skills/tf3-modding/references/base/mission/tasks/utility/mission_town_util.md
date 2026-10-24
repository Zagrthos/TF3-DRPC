# mission.tasks.utility.mission_town_util

Definition file: `base/tealdef/mission/tasks/utility/mission_town_util.d.tl` (in the game folder).

Town helper used by mission tasks.

## MissionTownUtil (record)

```teal
global record MissionTownUtil
```
Load: `local mission_town_util = ug_require "::/mission/tasks/utility/mission_town_util.tl" as MissionTownUtil`

Town helper used by mission tasks.

- `patchTownCargoNeeds : function(entity : Engine.Entity, cargoNeedsForLandUseType : {integer : {ResName}}, overwrite? : boolean)`  (call as `mission_town_util.patchTownCargoNeeds(…)`): Adds cargo needs to a town per land-use type (1 residential, 2 commercial, 3 industrial), or replaces them.
    - param `entity`: Town entity.
    - param `cargoNeedsForLandUseType`: Land-use type -> cargo type resources.
    - param `overwrite`: Starts from empty needs instead of the town's current ones.
    - used 3x in the base game, e.g. `base/content/mission/mission/tasks/utility/cargo_needs.tl:25`: `mission_town_util.patchTownCargoNeeds(params.townEntity, cargoNeeds, true)`
