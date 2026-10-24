# mission.tasks.finish.mission_task_finish

Definition file: `base/tealdef/mission/tasks/finish/mission_task_finish.d.tl` (in the game folder).

Task that ends the mission and shows the mission-end window.

## MissionTaskFinish (record)

```teal
global record MissionTaskFinish is MissionInterface.TaskScript<MissionTaskFinish.State, MissionTaskFinish.Params>
```
Load: `local finish = ug_require "::/mission/tasks/finish/finish.tl" as MissionTaskFinish`

Task (`finish`) that ends the mission. The mission window shows the end screen for it; the task completes when the player confirms (`MissionEndWindow` / `completeMission`).

Example:
```lua
-- urbangames_campaign_mission_03: mission lost
taskScript = finish as MissionInterface.TaskScript<any, any>,
taskParams = { success = false },
taskInfo = {},
```

### MissionTaskFinish.Params (record)

```teal
record Params
```

Params of the task; without params the mission counts as won.

- `success : boolean`: The mission was won.
- `isTutorial : boolean`: The finished mission is the tutorial.

### MissionTaskFinish.StateType (enum)

```teal
enum StateType
```

Progress of the end screen.

- `"Initial"`: Not shown yet.
- `"Showing"`: End screen shown.
- `"Complete"`: Confirmed by the player.

### MissionTaskFinish.State (record)

```teal
record State
```

State of the task.

- `state : StateType`: Progress of the end screen.
- `success : boolean`: The mission was won.
- `isTutorial : boolean`: The finished mission is the tutorial.
