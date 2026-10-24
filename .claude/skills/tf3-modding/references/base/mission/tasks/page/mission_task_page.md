# mission.tasks.page.mission_task_page

Definition file: `base/tealdef/mission/tasks/page/mission_task_page.d.tl` (in the game folder).

Task that only shows its task info and completes through an option button.

## MissionTaskPage (record)

```teal
global record MissionTaskPage is MissionInterface.TaskScript<any, nil>
```
Load: `local page = ug_require "::/mission/tasks/page/page.tl" as MissionTaskPage`

Task (`page`) without own logic. It shows its task info and completes when the player picks an option with key `"finish"`. Also used with `medals` only, to award a medal.

Example:
```lua
-- urbangames_campaign_mission_01
if taskKey == "find_alligators_end" then
    return {
        taskScript = page as MissionInterface.TaskScript<any, any>,
        taskInfo = {
            name = _("MISSION_01_TASK_FIND_ALLIGATORS_END_NAME"),
            options = { { text = _("MISSION_01_TASK_FIND_ALLIGATORS_END_BUTTON_1"), key = "finish" } },
        },
        followup = { { taskKey = "build_path_task" } },
    }
end
```

### MissionTaskPage.Params (record)

```teal
record Params
```

Empty params.

### MissionTaskPage.State (record)

```teal
record State
```

Empty state.
