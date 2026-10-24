# mission.tasks.utility.mission_task_check_landmark_level

Definition file: `base/tealdef/mission/tasks/utility/mission_task_check_landmark_level.d.tl` (in the game folder).

Task that waits until a landmark construction reaches a level.

## MissionTaskCheckLandmarkLevel (record)

```teal
global record MissionTaskCheckLandmarkLevel is MissionInterface.TaskScript<MissionTaskCheckLandmarkLevel.State, MissionTaskCheckLandmarkLevel.Params>
```

Task (`check_landmark_level`) that completes when the construction of a landmark has the given level.

### MissionTaskCheckLandmarkLevel.Params (record)

```teal
record Params
```

Params of the task.

- `entity : Engine.Entity`: Landmark (subconstruction) entity.
- `level : integer`: Level to reach.

### MissionTaskCheckLandmarkLevel.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: The level was reached.
