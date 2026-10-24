# mission.tasks.vehicle_util.mission_task_vehicle_modifier

Definition file: `base/tealdef/mission/tasks/vehicle_util/mission_task_vehicle_modifier.d.tl` (in the game folder).

Task that changes vehicle modifiers (top speed, comfort, noise, pollution).

## MissionTaskVehicleModifier (record)

```teal
global record MissionTaskVehicleModifier is MissionInterface.TaskScript<MissionTaskVehicleModifier.State, MissionTaskVehicleModifier.Params>
```
Load: `local vehicle_modifier = ug_require "::/mission/tasks/vehicle_util/vehicle_modifier.tl" as MissionTaskVehicleModifier`

Task (`vehicle_modifier`) that sends `VehicleModifier` / `ChangeModifier` events for the given vehicles or for all vehicles (of a carrier), then completes.

### MissionTaskVehicleModifier.Params (record)

```teal
record Params
```

Params of the task.

- `entities : {Engine.Entity}`: Vehicles to change when the task starts.
- `all : boolean`: Also changes every other transport vehicle (filtered by `carrier`).
- `modifiers : {VehicleModifierChanger.Modification : number}`: Modification (`TopSpeed`, `Comfort`, `Noise`, `Pollution`) -> value.
- `carrier : Carrier`: Restricts `all` to vehicles of this carrier.

### MissionTaskVehicleModifier.State (record)

```teal
record State
```

State of the task.

- `knownEntities : {Engine.Entity : boolean}`: Vehicles already changed by `all`.
