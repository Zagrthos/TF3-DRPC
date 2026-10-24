# mission.tasks.cutscene.mission_task_cutscene

Definition file: `base/tealdef/mission/tasks/cutscene/mission_task_cutscene.d.tl` (in the game folder).

Task that plays a camera cutscene.

## MissionTaskCutscene (record)

```teal
global record MissionTaskCutscene is MissionInterface.TaskScript<MissionTaskCutscene.State, MissionTaskCutscene.Params>
```
Load: `local cutscene = ug_require "::/mission/tasks/cutscene/cutscene.tl" as MissionTaskCutscene`

Task (`cutscene`) that plays a cutscene file when it starts (`api.gui.mission.playCutscene`) and completes when the cutscene ends or is skipped. Tool actions are disabled meanwhile; afterwards the game speed is set to `gameSpeed`.

Example:
```lua
-- urbangames_campaign_mission_01
if taskKey == "build_path_task_cutscene" then
    return {
        taskScript = cutscene as MissionInterface.TaskScript<any, any>,
        taskParams = { filePath = "urbangames_campaign_mission_01::/mission/keyframes/MISSION_01_CUTSCENE_BUILD_PATH.lua" },
        allowSimultaneousUiTasks = {
            mission_task_util.makeTaskKey("build_path_task", "urbangames_campaign_mission_01::/mission/mission.script")
        },
    }
end
```

### MissionTaskCutscene.Params (record)

```teal
record Params
```

Params of the task.

- `filePath : string`: Keyframe file of the cutscene.
- `gameSpeed : integer`: Game speed set when the cutscene is over (default 1).
- `getEntityToEntityMap : function(getTaskState : function(uniqueTaskKey : string) : MissionInterface.TaskState<any, any>) : {Engine.Entity : Engine.Entity}`: Returns a map that replaces entities of the cutscene file with runtime entities.
    - param `getTaskState`: Looks up a task by its unique key.
    - returns: Entity in the file -> entity to use.

### MissionTaskCutscene.State (enum)

```teal
enum State
```

Playback state.

- `"Initial"`: Not started.
- `"Playing"`: Playing.
- `"Complete"`: Ended or skipped.
