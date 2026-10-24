# mission.tasks.create_line.mission_task_stop_config

Definition file: `base/tealdef/mission/tasks/create_line/mission_task_stop_config.d.tl` (in the game folder).

Task that waits until a line loads a cargo at a stop.

## MissionTaskStopConfig (record)

```teal
global record MissionTaskStopConfig is MissionInterface.TaskScript<MissionTaskStopConfig.State, MissionTaskStopConfig.Params>
```
Load: `local stop_config = ug_require "::/mission/tasks/create_line/stop_config.tl" as MissionTaskStopConfig`

Task (`stop_config`) that completes when the stop of a line in the `loadStop` area is set to load the cargo type.

### MissionTaskStopConfig.Params (record)

```teal
record Params
```

Params of the task.

- `loadStop : MissionInterface.PosRadius`: Area of the stop.
- `getLine : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : Engine.Entity`: Returns the line to check; called until a line is stored.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Line entity.
- `cargoTypeRes : ResName`: Cargo type to load.

### MissionTaskStopConfig.State (record)

```teal
record State
```

State of the task.

- `cargoTypeId : CargoTypeId`: Id of the cargo type.
- `line : Engine.Entity`: Line being checked.
- `isComplete : boolean`: The stop loads the cargo.
