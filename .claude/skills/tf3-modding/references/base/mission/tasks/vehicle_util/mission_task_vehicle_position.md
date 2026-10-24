# mission.tasks.vehicle_util.mission_task_vehicle_position

Definition file: `base/tealdef/mission/tasks/vehicle_util/mission_task_vehicle_position.d.tl` (in the game folder).

Task that waits until a vehicle is near a position.

## MissionTaskVehiclePosition (record)

```teal
global record MissionTaskVehiclePosition is MissionInterface.TaskScript<MissionTaskVehiclePosition.State, MissionTaskVehiclePosition.Params>
```
Load: `local vehicle_position = ug_require "::/mission/tasks/vehicle_util/vehicle_position.tl" as MissionTaskVehiclePosition`

Task (`vehicle_position`) that completes when a vehicle is within `distance` of a position, drawing that circle as a zone.

### MissionTaskVehiclePosition.Params (record)

```teal
record Params
```

Params of the task.

- `vehicle : Engine.Entity`: Vehicle to watch.
- `position : {number, number}`: Target position `{x, y}`.
- `distance : number`: Radius in metres.

### MissionTaskVehiclePosition.State (record)

```teal
record State
```

Empty state.
