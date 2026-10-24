# mission.mission_framework.mission_framework_util_line

Definition file: `base/tealdef/mission/mission_framework/mission_framework_util_line.d.tl` (in the game folder).

Finds lines that connect stations in two areas.

## MissionFrameworkUtilLine (record)

```teal
global record MissionFrameworkUtilLine
```
Load: `local mission_framework_util_line = ug_require "::/mission/mission_framework/mission_framework_util_line.tl" as MissionFrameworkUtilLine`

Finds lines between stations in two areas.

- `getAllLinesBetweenStations : function(from : MissionInterface.PosRadius, to : MissionInterface.PosRadius) : {Engine.Entity}`  (call as `mission_framework_util_line.getAllLinesBetweenStations(…)`): Returns every line that serves a station group in both areas.
    - param `from`: First area.
    - param `to`: Second area.
    - returns: Line entities (may contain duplicates).
    - used 11x in the base game, e.g. `base/content/mission/mission/tasks/assign_vehicle/assign_vehicle.tl:36`: `local connectingLines = mission_framework_util_line.getAllLinesBetweenStations(startPos, endPos)`
- `getAnyLineBetweenZones : function(from : MissionInterface.PosRadius, to : MissionInterface.PosRadius) : Engine.Entity`  (call as `mission_framework_util_line.getAnyLineBetweenZones(…)`): Returns one line that serves a station in both areas.
    - param `from`: First area.
    - param `to`: Second area.
    - returns: A line entity, or `nil`.
