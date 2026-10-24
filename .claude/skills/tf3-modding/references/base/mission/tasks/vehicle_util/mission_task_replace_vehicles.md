# mission.tasks.vehicle_util.mission_task_replace_vehicles

Definition file: `base/tealdef/mission/tasks/vehicle_util/mission_task_replace_vehicles.d.tl` (in the game folder).

Task that replaces vehicles with a single-part vehicle of another model.

## MissionTaskReplaceVehicles (record)

```teal
global record MissionTaskReplaceVehicles is MissionInterface.TaskScript<MissionTaskReplaceVehicles.State, MissionTaskReplaceVehicles.Params>
```
Load: `local replace_vehicles = ug_require "::/mission/tasks/vehicle_util/replace_vehicles.tl" as MissionTaskReplaceVehicles`

Task (`replace_vehicles`) that replaces each vehicle with a single-part vehicle of one model and completes at once.

### MissionTaskReplaceVehicles.Params (record)

```teal
record Params
```

Params of the task.

- `entities : {Engine.Entity}`: Vehicles to replace.
- `modelName : string`: Model file of the new vehicle (used when `modelId` is not set).
- `modelId : integer`: Model id of the new vehicle.
- `skipInvalidEntities : boolean`: Skips entities that do not exist or are not vehicles.

### MissionTaskReplaceVehicles.State (record)

```teal
record State
```

Empty state.
