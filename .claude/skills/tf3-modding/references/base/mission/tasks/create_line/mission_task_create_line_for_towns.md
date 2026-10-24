# mission.tasks.create_line.mission_task_create_line_for_towns

Definition file: `base/tealdef/mission/tasks/create_line/mission_task_create_line_for_towns.d.tl` (in the game folder).

Task that waits until one line serves stations in all given towns.

## MissionTaskCreateLineForTowns (record)

```teal
global record MissionTaskCreateLineForTowns is MissionInterface.TaskScript<MissionTaskCreateLineForTowns.State, MissionTaskCreateLineForTowns.Params>
```
Load: `local create_line_for_towns = ug_require "::/mission/tasks/create_line/create_line_for_towns.tl" as MissionTaskCreateLineForTowns`

Task (`create_line_for_towns`) that completes when one line without line problems, with one of the transport modes, has stations in all towns. Each town gets an arrow marker.

### MissionTaskCreateLineForTowns.Params (record)

```teal
record Params
```

Params of the task.

- `towns : {Engine.Entity}`: Towns the line must serve.
- `transportModes : {TransportMode}`: Accepted transport modes.

### MissionTaskCreateLineForTowns.State (record)

```teal
record State
```

State of the task.

- `result : Engine.Entity`: Line found, or `-1`.
