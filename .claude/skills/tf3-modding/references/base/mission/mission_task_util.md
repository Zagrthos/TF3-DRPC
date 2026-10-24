# mission.mission_task_util

Definition file: `base/tealdef/mission/mission_task_util.d.tl` (in the game folder).

Declarative task description (`StaticTaskInfo`) used by mission stories, plus helpers for task keys and areas.

## Globals

- `MissionLocalization : {{Engine.Entity, string}}`: List of `{ entity, name }` pairs used to rename the entities of a mission savegame (see `NameUtil.setName`).

## MissionTaskUtil (record)

```teal
global record MissionTaskUtil
```
Load: `local mission_task_util = ug_require "::/mission/mission_task_util.tl" as MissionTaskUtil`

Declarative task description used by stories, plus helpers for task keys and areas.

- `makeTaskKey : function(taskKey : string, root : string) : string`  (call as `mission_task_util.makeTaskKey(…)`): Builds the unique key of a task.
    - param `taskKey`: Task key local to the story.
    - param `root`: Script key of the story (e.g. `"urbangames_campaign_mission_01::/mission/mission.script"`).
    - returns: `root .. "||" .. taskKey`.
    - used 246x in the base game, e.g. `base/content/mission/mission/mission_boot_util.tl:40`: `uniqueKey = mission_task_util.makeTaskKey(key, scriptKey)`
- `makeJoinParams : function(keys : {string}, root : string, showProgress? : boolean) : MissionTaskJoin.Params`  (call as `mission_task_util.makeJoinParams(…)`): Builds the params of a join task that waits for several tasks of the same story.
    - param `keys`: Local task keys to wait for.
    - param `root`: Script key of the story.
    - param `showProgress`: Shows progress of the join.
    - returns: Params for `MissionTaskJoin`.
    - used 37x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story.tl:757`: `taskParams = mission_task_util.makeJoinParams( { "road_stop_2_1", "road_stop_2_2" }, "::/mission/tutorial/tutorial.script", true),`
- `locationButDistance : function(entity : Engine.Entity, distance : number) : MissionInterface.PosRadius`  (call as `mission_task_util.locationButDistance(…)`): Builds a circle around an entity's position.
    - param `entity`: Entity at the centre.
    - param `distance`: Radius in metres.
    - returns: 2D circle.
- `disallowAllVehicles : function(models : {any : string}) : {string : {integer, integer}}`  (call as `mission_task_util.disallowAllVehicles(…)`): Maps every given model name to `{0, nil}`.
    - param `models`: Model names.
    - returns: Model name -> `{0, nil}`.
- `makeBoundingSphereFromEntity : function(entity : Engine.Entity) : MissionInterface.PosRadius`  (call as `mission_task_util.makeBoundingSphereFromEntity(…)`): Builds a sphere around an entity's bounding box (for vehicles, around the first carriage).
    - param `entity`: Entity to enclose.
    - returns: Sphere, or `nil` if the entity has no bounding volume.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react.tl:120`: `s = mission_task_util.makeBoundingSphereFromEntity(entity)`
- `mergeBoundingVolumes : function(entities : {Engine.Entity}, radiusOffset? : number) : MissionInterface.PosRadius`  (call as `mission_task_util.mergeBoundingVolumes(…)`): Builds a sphere that encloses the bounding boxes of several entities.
    - param `entities`: Entities to enclose; missing ones are skipped.
    - param `radiusOffset`: Extra radius in metres.
    - returns: Sphere, or an empty table if no entity has a bounding volume.
    - used 4x in the base game, e.g. `mods/release/urbangames_campaign_mission_02/content/mission/mission/mission_story.tl:205`: `return mission_task_util.mergeBoundingVolumes(validEdges, 80)`
- `mergeAreas : function(areas : {MissionInterface.PosRadius}, radiusOffset? : number) : MissionInterface.PosRadius`  (call as `mission_task_util.mergeAreas(…)`): Builds a sphere that encloses several areas.
    - param `areas`: Areas to enclose.
    - param `radiusOffset`: Extra radius in metres.
    - returns: Enclosing sphere, or `nil` for an empty list.
    - used 6x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react.tl:129`: `result.location = mission_task_util.mergeAreas(boundingSpheres, 200.0)`
- `extractGenericParams : function(taskKey : string, getTaskData : function(string) : StaticTaskInfo) : MissionInterface.GenericTaskParams`  (call as `mission_task_util.extractGenericParams(…)`): Converts a task's static description into its generic params (rule lists become maps).
    - param `taskKey`: Local task key.
    - param `getTaskData`: Task-data lookup of the story.
    - returns: Generic params, or `nil` for an unknown task.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_boot_util.tl:88`: `local insert = mission_task_util.extractGenericParams(taskKey, getTaskData)`
- `getTaskInfo : function(taskKey : string, ctx : MissionInterface.TaskContext<any, any>, getTaskData : function(string) : StaticTaskInfo) : MissionInterface.TaskInfo`  (call as `mission_task_util.getTaskInfo(…)`): Returns the static task info of a task, with runtime progress and sub-task progress copied in.
    - param `taskKey`: Local task key.
    - param `ctx`: Context of the task.
    - param `getTaskData`: Task-data lookup of the story.
    - returns: Task info, or `nil` if the task has none.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_boot_util.tl:116`: `return mission_task_util.getTaskInfo(taskKey, ctx, getTaskData)`
- `getNonBulldozableEntitiesFromParams : function(params : table) : {Engine.Entity}`  (call as `mission_task_util.getNonBulldozableEntitiesFromParams(…)`): Collects entities from a mission params table by key prefix (`entity_*` values, `entityList_*` lists, `entityMap_*` map keys).
    - param `params`: Mission params table.
    - returns: Collected entities.
    - used 8x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/mission_story.tl:129`: `local nonBulldozableEntities = mission_task_util.getNonBulldozableEntitiesFromParams(params as table)`

### MissionTaskUtil.FollowupDef (record)

```teal
record FollowupDef
```

A task to add after another task completed (or at boot), optionally only when a condition holds.

Example:
```lua
followup = {
    { taskKey = "find_alligators_end" },
    {
        taskKey = "task_alligators_medal",
        condition = function(ctx : MissionInterface.TaskContext<any, any>) : boolean
            return (ctx.guiTimeSeconds - ctx.genericState.guiSpawnTime) < 30
        end,
    },
},
```

- `taskKey : string`: Task key local to the story.
- `condition : function(MissionInterface.TaskContext<any, any>, function(string) : MissionInterface.TaskState<any, any>)`: Optional condition; the task is only added when it returns true.
    - param `#1`: Context of the completed task.
    - param `#2`: Function that returns the state of another task by its local task key.

### MissionTaskUtil.StaticTaskInfo (record)

```teal
record StaticTaskInfo
```

Static description of one task in a story. It names the script that runs the task, its params, what the GUI shows and which tasks follow.

Notes:

Returned by the story's task-data factory for each task key. The fields other than
`taskScript`, `taskParams`, `taskInfo`, `guideInfo` and `followup` become the task's
`MissionInterface.GenericTaskParams` (`visible`/`invisible`/`enabled`/`disabled` are
converted to rule maps).

Example:
```lua
-- urbangames_campaign_mission_01 mission_story.tl
if taskKey == "find_alligators_end" then
    return {
        taskScript = page as MissionInterface.TaskScript<any, any>,
        taskInfo = {
            name = _("MISSION_01_TASK_FIND_ALLIGATORS_END_NAME"),
            options = { { text = _("MISSION_01_TASK_FIND_ALLIGATORS_END_BUTTON_1"), key = "finish" } },
        },
        followup = {
            { taskKey = "build_path_task" },
            { taskKey = "build_path_task_cutscene" },
        },
        highlightRules = { {"menu.mission.task-button", "Square"} },
        notificationFilter = { ["::/landmarks/landmarks_notification.script"] = true },
    }
end
```

- `isBonusTask : boolean`: Marks the task as a bonus mission.
- `taskScript : MissionInterface.TaskScript<any, any>`: Script implementing the task (one of the task types under `mission/tasks/**` or a custom one).
- `taskKey : string`: Task key.
- `taskParams : any`: Params of the task script (its `Params` record).
- `medals : {string : boolean}`: Medal ids awarded while this task exists (e.g. `{ MISSION_01_MEDAL_1 = true }`).
- `stars : {string : boolean}`: Star ids and their state.
- `visible : {string}`: GUI classes made visible while the task is open.
- `invisible : {string}`: GUI classes hidden while the task is open.
- `enabled : {string}`: GUI ids/tags enabled while the task is open.
- `disabled : {string}`: GUI ids/tags disabled while the task is open (e.g. `"menu.construction.rail"`).
- `highlightRules : {{string, MissionInterface.HighlightStyle}}`: List of `{ GUI id/tag, highlight style }` pairs.
- `taskInfo : MissionInterface.TaskInfo`: Only to be used in a mission
- `guideInfo : MissionInterface.GuideInfo`: Only to be used in the guide system
- `omitNotificationOnComplete : boolean`: Suppresses the completion notification.
- `disableGuideSystem : boolean`: Keeps the guide system inactive while the task is open.
- `startSoundtrack : string`: Music track started when the task opens.
- `soundtrack : MissionInterface.MissionSoundtrack`: Tracklist played while the task is open.
- `playlistOverride : ResName`: Playlist used while the task is open.
- `allowBuyVehicles : {string : integer}`: (modelname -> count, minCount)
- `allowBuyVehicleGroups : {string : {{string}, integer}}`: id, -> modelnames, count
- `lockedVehicles : {Engine.Entity}`: entity ids that may not be sold
- `followup : {FollowupDef}`: Tasks added when this task completes.
- `vehicleFilter : GameContext.Filters.VehicleFilter`: Vehicle store filter while the task is open.
- `notificationFilter : {string : boolean}`: Notification types hidden while the task is open.
- `menuFilter : GameContext.Filters.MenuFilter`: Menu items enabled or disabled while the task is open.
- `selectorEntityFilter : {Engine.Entity : boolean}`: Entity selection filter while the task is open.
- `disableFeatures : {DisableFeatures : boolean}`: GUI features switched off while the task is open.
- `requiredTaskToComplete : string`: taskkey <- mission_task_util.makeTaskKey
- `allowSimultaneousUiTasks : {string}`: taskkey <- mission_task_util.makeTaskKey
- `spawnGuide : {string}`: Script events sent when the task is added (e.g. `"spawn-camera-guide"`).
- `showInHistory : boolean`: Keeps the task listed after completion and sends a completion notification.
- `suppressAutoOpen : boolean`: Keeps the mission window from opening automatically for this task.
- `financialReward : integer`: Money paid when the task completes, in game currency.
