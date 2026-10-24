# mission.tasks.utility.mission_task_relay_update

Definition file: `base/tealdef/mission/tasks/utility/mission_task_relay_update.d.tl` (in the game folder).

Task that runs a custom update function until it reports completion.

## MissionTaskRelayUpdate (record)

```teal
global record MissionTaskRelayUpdate is MissionInterface.TaskScript<MissionTaskRelayUpdate.State, MissionTaskRelayUpdate.Params>
```
Load: `local relay_update = ug_require "::/mission/tasks/utility/relay_update.tl" as MissionTaskRelayUpdate`

Task (`relay_update`) that calls `onUpdateFn` every update and completes when it returns true.

### MissionTaskRelayUpdate.Params (record)

```teal
record Params
```

Params of the task.

- `onUpdateFn : function(ctx : MissionInterface.TaskContext<MissionTaskRelayUpdate.State, MissionTaskRelayUpdate.Params>, readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : boolean`: Custom update function; may keep data in `ctx.state.customState`.
    - param `ctx`: Context of the task.
    - param `readOnlyApi`: Read-only mission API.
    - returns: `true` to complete the task.

### MissionTaskRelayUpdate.State (record)

```teal
record State
```

State of the task.

- `customState : any`: Free slot for `onUpdateFn`.
- `isComplete : boolean`: Last result of `onUpdateFn`.
