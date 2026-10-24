# mission.tasks.buy_vehicle.mission_task_clone_vehicle

Definition file: `base/tealdef/mission/tasks/buy_vehicle/mission_task_clone_vehicle.d.tl` (in the game folder).

Task that waits until the player clones a suitable vehicle in the vehicle store.

## MissionTaskCloneVehicle (record)

```teal
global record MissionTaskCloneVehicle is MissionInterface.TaskScript<MissionTaskCloneVehicle.State, MissionTaskCloneVehicle.Params>
```
Load: `local clone_vehicle = ug_require "::/mission/tasks/buy_vehicle/clone_vehicle.tl" as MissionTaskCloneVehicle`

Task (`clone_vehicle`) that completes when the player clones a vehicle of the carrier with capacity for the cargo type in the vehicle store.

### MissionTaskCloneVehicle.Params (record)

```teal
record Params
```

Params of the task.

- `carrier : Carrier`: Carrier the cloned vehicle must have.
- `cargoTypeRes : string`: Cargo type the cloned vehicle must carry.

### MissionTaskCloneVehicle.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: A matching vehicle was cloned.
