# mission.tasks.utility.mission_task_send_event

Definition file: `base/tealdef/mission/tasks/utility/mission_task_send_event.d.tl` (in the game folder).

Task that sends a script event or a GUI React event.

## MissionTaskSendEvent (record)

```teal
global record MissionTaskSendEvent is MissionInterface.TaskScript<MissionTaskSendEvent.State, MissionTaskSendEvent.Params>
```
Load: `local send_event = ug_require "::/mission/tasks/utility/send_event.tl" as MissionTaskSendEvent`

Task (`send_event`) that sends one script event (engine side) or fires a React event in the GUI (`isGuiEvent`), then completes.

### MissionTaskSendEvent.Params (record)

```teal
record Params
```

Event to send.

- `src : string`: Event source (script events only).
- `id : string`: Event id (script events only).
- `name : string`: Event name; the React event name when `isGuiEvent` is set.
- `param : any`: Event parameter.
- `isGuiEvent : boolean`: Fires `api.gui.fireReactEvent(name, param)` instead of a script event.

### MissionTaskSendEvent.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: The event was sent.
