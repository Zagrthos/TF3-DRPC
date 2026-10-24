# mission.mission_boot_util

Definition file: `base/tealdef/mission/mission_boot_util.d.tl` (in the game folder).

Helpers that turn a task-data factory and a list of boot tasks into the script module that the mission or guide-system game script boots from.

## MissionBootUtil (record)

```teal
global record MissionBootUtil
```
Load: `local mission_boot_util = ug_require "::/mission/mission_boot_util.tl" as MissionBootUtil`

Builds the script module (vtable factory and boot function) of a mission or guide-system story from a task-data factory.

Notes:

The story is a function `taskKey -> MissionTaskUtil.StaticTaskInfo`. Each task lists its
`followup` tasks; `bootTasks` are the first ones. Task keys are local to the story; the boot
util turns them into unique keys with `MissionTaskUtil.makeTaskKey(taskKey, scriptKey)`.

Example:
```lua
-- urbangames_campaign_mission_01: mission/mission.tl
local mission_boot_util = ug_require "::/mission/mission_boot_util.tl" as MissionBootUtil
local taskDataFactory = ug_require "mission_story.tl" as function(taskKey : string) : MissionTaskUtil.StaticTaskInfo

return mission_boot_util.makeMission({
    scriptKey = "urbangames_campaign_mission_01::/mission/mission.script",
    taskDataFactory = taskDataFactory,
    bootTasks = {
        { taskKey = "check_entities" },
        { taskKey = "intro" },
        { taskKey = "intro_cutscene" },
        { taskKey = "non_bulldozable" },
    },
})
```

- `makeMission : function(config : MissionConfig) : Module`  (call as `mission_boot_util.makeMission(…)`): Builds the module of a mission story; vtables provide `getTaskInfo` from the static `taskInfo`.
    - param `config`: Mission configuration.
    - returns: Module to export as `mission` from the script named in `scriptKey`.
    - used 9x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial.tl:5`: `return mission_boot_util.makeMission({`
- `makeGuide : function(config : GuideConfig) : Module`  (call as `mission_boot_util.makeGuide(…)`): Builds the module of a guide-system story; vtables provide `getGuideInfo` from the static `guideInfo`.
    - param `config`: Guide configuration.
    - returns: Module to export as `guide_system` from the script named in `scriptKey`.
    - used 6x in the base game, e.g. `base/content/mission/mission/guide_system/main/guide_system.tl:5`: `return mission_boot_util.makeGuide({`

### MissionBootUtil.MissionConfig (record)

```teal
record MissionConfig
```

Configuration of a mission story for `makeMission`.

- `scriptKey : string`: Script that exposes the module as `mission` (e.g. `"urbangames_campaign_mission_01::/mission/mission.script"`); also the root of the task keys.
- `taskDataFactory : function(taskKey : string) : MissionTaskUtil.StaticTaskInfo`: Returns the static description of a task key, or `nil` for unknown keys.
    - param `taskKey`: Task key local to the story.
    - returns: Static task description.
- `bootTasks : {MissionTaskUtil.FollowupDef}`: Tasks added when the mission starts.

### MissionBootUtil.GuideConfig (record)

```teal
record GuideConfig
```

Configuration of a guide-system story for `makeGuide`.

- `scriptKey : string`: Script that exposes the module as `guide_system`; also the root of the task keys.
- `taskDataFactory : function(taskKey : string) : MissionTaskUtil.StaticTaskInfo`: Returns the static description (with `guideInfo`) of a guide task key.
    - param `taskKey`: Task key local to the story.
    - returns: Static task description.
- `bootTasks : {MissionTaskUtil.FollowupDef}`: Guide tasks added at start.
- `skipIfStartedAsMission : boolean`: Does not add the boot guides when the game was started as a mission.

### MissionBootUtil.Module (record)

```teal
record Module
```

Script module returned by `makeMission` / `makeGuide`; exported by the story's `.script` file so the framework can call it by name.

- `getVTable : function(taskKey : string, uniqueKey : string) : MissionInterface.TaskVTable<any, any>`: Builds the vtable of a task (target of `TaskConfig.getVTableScriptFn`).
    - param `taskKey`: Task key local to the story.
    - param `uniqueKey`: Unique task key.
    - returns: Vtable of the task.
- `spawnBootTasks : function(taskKey : string, ctx : MissionInterface.TaskContext<any, any>, missionApi : MissionInterface.MissionTaskApi)`: Adds the boot tasks (when `taskKey` is `nil`) or the follow-ups of `taskKey` whose condition holds. Target of `BootstrapDesc.bootstrapScriptFn`.
    - param `taskKey`: `nil` for the boot tasks, otherwise the completed task's key.
    - param `ctx`: Context of the completed task (`nil` at boot).
    - param `missionApi`: API used to add the tasks.
