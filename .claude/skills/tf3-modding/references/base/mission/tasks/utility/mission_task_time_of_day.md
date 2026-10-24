# mission.tasks.utility.mission_task_time_of_day

Definition file: `base/tealdef/mission/tasks/utility/mission_task_time_of_day.d.tl` (in the game folder).

Task that moves the time of day to a target time.

## MissionTaskTimeOfDay (record)

```teal
global record MissionTaskTimeOfDay is MissionInterface.TaskScript<MissionTaskTimeOfDay.State, MissionTaskTimeOfDay.Params>
```
Load: `local time_of_day = ug_require "::/mission/tasks/utility/time_of_day.tl" as MissionTaskTimeOfDay`

Task (`time_of_day`) that moves the time of day forward to a target time, at once or in steps per update, and completes when it is reached.

### MissionTaskTimeOfDay.Params (record)

```teal
record Params
```

Target time.

- `hours : integer`: Target hour.
- `minutes : integer`: Target minute.
- `stepInMinutes : integer`: Minutes added per update; `nil` jumps directly to the target.

### MissionTaskTimeOfDay.State (record)

```teal
record State
```

State of the task (seconds since midnight; the target may exceed one day when it wraps).

- `initialTimeOfDay : integer`: Time of day at spawn, in seconds.
- `currentTimeOfDay : integer`: Time of day set last, in seconds.
- `targetTimeOfDay : integer`: Target time of day, in seconds.
