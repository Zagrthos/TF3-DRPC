# mission.tasks.assign_vehicle.mission_task_assign_vehicle

Definition file: `base/tealdef/mission/tasks/assign_vehicle/mission_task_assign_vehicle.d.tl` (in the game folder).

Task that waits until player vehicles run on a given line or on a line between two areas.

## MissionTaskAssignVehicle (record)

```teal
global record MissionTaskAssignVehicle is MissionInterface.TaskScript<MissionTaskAssignVehicle.State, MissionTaskAssignVehicle.Params>
```
Load: `local assign_vehicle = ug_require "::/mission/tasks/assign_vehicle/assign_vehicle.tl" as MissionTaskAssignVehicle`

Task (`assign_vehicle`) that completes when at least `minCount` player vehicles of the carrier run on a matching player line. The line is `line`, any line between a `pos1` area and a `pos2` area, or the line returned by `getRuntimeLine`; `cargoTypeRes` limits the count to vehicles that can carry that cargo.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = assign_vehicle as MissionInterface.TaskScript<any, any>,
taskParams : MissionTaskAssignVehicle.Params = {
    pos1 = { { pos = params.stationForestPos.pos, radius = params.stationForestPos.radius } },
    pos2 = { { pos = params.stationHotelPos.pos, radius = params.stationHotelPos.radius } },
},
showInHistory = true,
```

### MissionTaskAssignVehicle.Params (record)

```teal
record Params
```

Params of the task.

- `line : Engine.Entity`: either specific line

or any line with these multiple start and end positions
- `pos1 : {MissionInterface.PosRadius}`: Start areas of the line. Used when `line` is not set; any player line between a `pos1` and a `pos2` area counts.
- `pos2 : {MissionInterface.PosRadius}`: End areas of the line.
- `getPos1 : function() : {MissionInterface.PosRadius}`: Returns the start areas; takes precedence over `pos1`.
    - returns: Areas.
- `getPos2 : function() : {MissionInterface.PosRadius}`: Returns the end areas; takes precedence over `pos2`.
    - returns: Areas.

or any line from a previous task
- `getRuntimeLine : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : Engine.Entity`
- `carrier : Carrier`: Carrier of the vehicles (default road).
- `minCount : integer`: Number of vehicles (default 1).
- `draw : boolean`: Draws the `pos1` and `pos2` areas as zones.
- `cargoTypeRes : ResName`: Counts only vehicles with capacity for this cargo type.

### MissionTaskAssignVehicle.State (record)

```teal
record State
```

State of the task.

- `runtimeLines : {Engine.Entity}`: runtime specific line
- `result : {Engine.Entity}`: Matching vehicles found at the last update.
