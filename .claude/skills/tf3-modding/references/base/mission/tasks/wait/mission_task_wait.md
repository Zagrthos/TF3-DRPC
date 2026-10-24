# mission.tasks.wait.mission_task_wait

Definition file: `base/tealdef/mission/tasks/wait/mission_task_wait.d.tl` (in the game folder).

Task that completes after a fixed time.

## MissionTaskWait (record)

```teal
global record MissionTaskWait is MissionInterface.TaskScript<MissionTaskWait.State, MissionTaskWait.Params>
```
Load: `local wait = ug_require "::/mission/tasks/wait/wait.tl" as MissionTaskWait`

Task (`wait`) that completes after a time span in GUI time or in simulation time.

Example:
```lua
-- urbangames_campaign_mission_03
if taskKey == "missionEnd" then
    return {
        taskScript = wait as MissionInterface.TaskScript<any, any>,
        taskParams = { guiTimeSeconds = 1 },
        followup = { { taskKey = "missionEndScreen" } },
    }
end
```

### MissionTaskWait.Params (record)

```teal
record Params
```

How long to wait; `guiTimeSeconds` takes precedence when both are set.

- `simTimeSeconds : number`: in simulation time (affected by game speed)
- `guiTimeSeconds : number`: in approximated wall clock time (not affected by game speed, except that it is delayed when the game lags)

### MissionTaskWait.State (record)

```teal
record State
```

End times computed when the task spawns.

- `endSimTimeMs : number`: Game time (ms) at which the task completes.
- `endGuiTimeSeconds : number`: Mission GUI time (seconds) at which the task completes.
