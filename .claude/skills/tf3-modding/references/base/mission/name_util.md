# mission.name_util

Definition file: `base/tealdef/mission/name_util.d.tl` (in the game folder).

Helpers that rename entities of a mission savegame and check that all of them got a name.

## NameUtil (record)

```teal
global record NameUtil
```
Load: `local name_util = ug_require "::/mission/name_util.tl" as NameUtil`

Renames entities of a mission savegame.

- `setName : function(id : Engine.Entity, name : string, tagged : {Engine.Entity : boolean}, missionInitRedundancyChecks? : boolean)`  (call as `name_util.setName(…)`): Sets the name of an entity and propagates it to the stations of a station group or to the station groups of a construction.
    - param `id`: Entity to rename.
    - param `name`: New name.
    - param `tagged`: Set of renamed entities, updated in place.
    - param `missionInitRedundancyChecks`: Logs errors for names set on the wrong level (edge objects, subconstructions, station groups of named constructions).
    - used 12x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:582`: `name_util.setName(entityAndName[1], entityAndName[2], tagged, true)`
- `checkTagged : function(tagged : {Engine.Entity : boolean}, printParams? : boolean)`  (call as `name_util.checkTagged(…)`): Logs (or prints) constructions, station groups, depots, towns, vehicles and player lines that were not renamed.
    - param `tagged`: Set of renamed entities.
    - param `printParams`: Prints `{ entity, _("name") },` lines instead of warnings.
    - used 9x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:584`: `name_util.checkTagged(tagged, true)`
