# mission.tasks.wait.mission_task_wait_ui_event

Definition file: `base/tealdef/mission/tasks/wait/mission_task_wait_ui_event.d.tl` (in the game folder).

Task that completes when a matching GUI event arrives.

## MissionTaskWaitUiEvent (record)

```teal
global record MissionTaskWaitUiEvent is MissionInterface.TaskScript<MissionTaskWaitUiEvent.State, MissionTaskWaitUiEvent.Params>
```
Load: `local wait_ui_event = ug_require "::/mission/tasks/wait/wait_ui_event.tl" as MissionTaskWaitUiEvent`

Task (`wait_ui_event`) that completes when a GUI event matches one of the candidates, optionally after an extra instruction has played, or after a timeout.

Example:
```lua
-- urbangames_campaign_mission_02: wait until a window is closed (at most 5 s)
taskScript = wait_ui_event as MissionInterface.TaskScript<any, any>,
taskParams : MissionTaskWaitUiEvent.Params = {
    candidates = {
        { name = "window.close" },
    },
    timeoutIfNoEvent = 5,
},
```

### MissionTaskWaitUiEvent.EventDesc (record)

```teal
record EventDesc
```

GUI event pattern; `nil` fields match anything, `param` is compared deeply.

- `src : string`: Event source.
- `id : string`: Event id (e.g. `"VehicleStoreBrowser"`).
- `name : string`: Event name (e.g. `"open"`, `"window.close"`).
- `param : any`: Event parameter.

### MissionTaskWaitUiEvent.Params (record)

```teal
record Params
```

Params of the task.

- `candidates : {EventDesc}`: GUI event patterns; any match completes the task.
- `instructionKeyOnComplete : string`: Instruction key queued after the event; the task completes when that instruction has finished playing.
- `timeoutIfNoEvent : number`: Completes the task after this many GUI seconds even without an event.
- `paramValidateFn : function(param : any) : boolean`: Extra check of the event parameter.
    - param `param`: Event parameter.
    - returns: `true` to accept the event.

### MissionTaskWaitUiEvent.State (record)

```teal
record State
```

State of the task.

- `observed : EventDesc`: The candidate that matched.
- `timeoutEndTime : number`: GUI time (seconds) of the timeout.
