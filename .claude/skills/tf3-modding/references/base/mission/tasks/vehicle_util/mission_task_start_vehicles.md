# mission.tasks.vehicle_util.mission_task_start_vehicles

Definition file: `base/tealdef/mission/tasks/vehicle_util/mission_task_start_vehicles.d.tl` (in the game folder).

Task that starts or stops vehicles.

## MissionTaskStartVehicles (record)

```teal
global record MissionTaskStartVehicles is MissionInterface.TaskScript<MissionTaskStartVehicles.State, MissionTaskStartVehicles.Params>
```
Load: `local start_vehicles = ug_require "::/mission/tasks/vehicle_util/start_vehicles.tl" as MissionTaskStartVehicles`

Task (`start_vehicles`) that starts (or stops) vehicles once `conditionFn` holds, then completes.

### MissionTaskStartVehicles.Params (record)

```teal
record Params
```

Params of the task.

- `vehicles : {Engine.Entity}`: Vehicles to start or stop.
- `conditionFn : function() : boolean`: Condition checked every update; `nil` acts at once.
    - returns: `true` to start the vehicles now.
- `overruleComplete : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : boolean`: Completes the task without touching the vehicles when it returns true.
    - param `readOnlyApi`: Read-only mission API.
    - returns: `true` to complete the task.
- `stop : boolean`: Stops the vehicles instead of starting them.
- `skipInvalidEntities : boolean`: Skips entities that do not exist.

### MissionTaskStartVehicles.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: The task is done.
