# mission.tasks.wait.mission_task_wait_event

Definition file: `base/tealdef/mission/tasks/wait/mission_task_wait_event.d.tl` (in the game folder).

Task that completes when a matching engine-side script event arrives.

## MissionTaskWaitEvent (record)

```teal
global record MissionTaskWaitEvent is MissionInterface.TaskScript<MissionTaskWaitEvent.State, MissionTaskWaitEvent.Params>
```
Load: `local wait_event = ug_require "::/mission/tasks/wait/wait_event.tl" as MissionTaskWaitEvent`

Task (`wait_event`) that completes when an engine-side script event matches one of the candidates.

Example:
```lua
-- urbangames_campaign_mission_02: wait until a dialogue has finished
taskScript = wait_event as MissionInterface.TaskScript<any, any>,
taskParams = {
    candidates = {
        { id = "mission-dialogue", name = "on-finish-dialogue" },
    },
},
```

### MissionTaskWaitEvent.EventDesc (record)

```teal
record EventDesc
```

Event pattern; `nil` fields match anything, `param` is compared deeply.

- `src : string`: Event source.
- `id : string`: Event id.
- `name : string`: Event name.
- `param : any`: Event parameter.

### MissionTaskWaitEvent.Params (record)

```teal
record Params
```

Params of the task.

- `candidates : {EventDesc}`: Event patterns; any match completes the task.

### MissionTaskWaitEvent.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: A matching event arrived.
