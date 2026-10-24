# mission.tasks.utility.mission_task_clouds

Definition file: `base/tealdef/mission/tasks/utility/mission_task_clouds.d.tl` (in the game folder).

Task that fades the cloud coverage to a target value.

## MissionTaskClouds (record)

```teal
global record MissionTaskClouds is MissionInterface.TaskScript<MissionTaskClouds.State, MissionTaskClouds.Params>
```
Load: `local clouds = ug_require "::/mission/tasks/utility/clouds.tl" as MissionTaskClouds`

Task (`clouds`) that changes the cloud coverage linearly from its current value to `coverage` over `seconds` of GUI time.

### MissionTaskClouds.Params (record)

```teal
record Params
```

Params of the task.

- `seconds : integer`: Duration in GUI seconds.
- `coverage : number`: Target cloud coverage.

### MissionTaskClouds.State (record)

```teal
record State
```

State of the task.

- `initialTime : number`: GUI time (seconds) at which the fade started.
- `endTime : number`: GUI time (seconds) at which the fade ends and the task completes.
- `initialCoverage : number`: Cloud coverage at the start, read from the GUI.
