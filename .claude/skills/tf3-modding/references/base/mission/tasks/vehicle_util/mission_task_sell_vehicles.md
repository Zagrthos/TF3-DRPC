# mission.tasks.vehicle_util.mission_task_sell_vehicles

Definition file: `base/tealdef/mission/tasks/vehicle_util/mission_task_sell_vehicles.d.tl` (in the game folder).

Task that sells vehicles, optionally deleting their lines.

## MissionTaskSellVehicles (record)

```teal
global record MissionTaskSellVehicles is MissionInterface.TaskScript<MissionTaskSellVehicles.State, MissionTaskSellVehicles.Params>
```
Load: `local sell_vehicles = ug_require "::/mission/tasks/vehicle_util/sell_vehicles.tl" as MissionTaskSellVehicles`

Task (`sell_vehicles`) that sells vehicles and completes at once.

### MissionTaskSellVehicles.Params (record)

```teal
record Params
```

Params of the task.

- `entities : {Engine.Entity}`: Vehicles to sell.
- `deleteLines : boolean`: Also deletes the lines of the sold vehicles.
- `skipInvalidEntities : boolean`: Skips entities that do not exist.

### MissionTaskSellVehicles.State (record)

```teal
record State
```

Empty state.
