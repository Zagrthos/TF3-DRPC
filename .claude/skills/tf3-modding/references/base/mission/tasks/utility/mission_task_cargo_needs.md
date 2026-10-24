# mission.tasks.utility.mission_task_cargo_needs

Definition file: `base/tealdef/mission/tasks/utility/mission_task_cargo_needs.d.tl` (in the game folder).

Task that sets the cargo needs of a town.

## MissionTaskCargoNeeds (record)

```teal
global record MissionTaskCargoNeeds is MissionInterface.TaskScript<MissionTaskCargoNeeds.State, MissionTaskCargoNeeds.Params>
```
Load: `local cargo_needs = ug_require "::/mission/tasks/utility/cargo_needs.tl" as MissionTaskCargoNeeds`

Task (`cargo_needs`) that replaces the cargo needs of a town for the given land-use types and completes at once.

### MissionTaskCargoNeeds.Params (record)

```teal
record Params
```

Params of the task.

- `townEntity : Engine.Entity`: Town to change.
- `cargoNeeds : {ResName}`: Cargo type resources the selected land-use types need.
- `landUse : {integer}`: 1 : residential, 2 : commercial , 3 : industrial

### MissionTaskCargoNeeds.State (record)

```teal
record State
```

Empty state.
