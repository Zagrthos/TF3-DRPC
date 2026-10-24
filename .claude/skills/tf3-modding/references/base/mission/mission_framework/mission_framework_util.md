# mission.mission_framework.mission_framework_util

Definition file: `base/tealdef/mission/mission_framework/mission_framework_util.d.tl` (in the game folder).

Low-level access to task vtables, task contexts and the mission state.

## MissionFrameworkUtil (record)

```teal
global record MissionFrameworkUtil
```
Load: `local mission_framework_util = ug_require "::/mission/mission_framework/mission_framework_util.tl" as MissionFrameworkUtil`

Low-level access to task vtables, task contexts and the saved mission state.

- `getVTable : function(config : MissionInterface.TaskConfig) : MissionInterface.TaskVTable<any, any>`  (call as `mission_framework_util.getVTable(…)`): Resolves (and caches by unique key) the vtable of a task by calling its `getVTableScriptFn`.
    - param `config`: Task identification.
    - returns: Vtable, or `nil` if the function cannot be found.
    - used 15x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:32`: `local vtable = mission_framework_util.getVTable(task.genericState.config)`
- `makeTaskContext : function<TaskSpecificState, TaskSpecificParams>(data : MissionInterface.TaskState<TaskSpecificState, TaskSpecificParams>, guiTimeSeconds? : number) : MissionInterface.TaskContext<TaskSpecificState, TaskSpecificParams>`  (call as `mission_framework_util.makeTaskContext(…)`): Builds the context passed to task callbacks from a saved task state.
    - param `data`: Saved task state.
    - param `guiTimeSeconds`: Current mission GUI time in seconds.
    - returns: Context with state, generic state, params and GUI time.
    - used 11x in the base game, e.g. `base/content/mission/mission/guide_system/guide_system_react.tl:33`: `result.info = vtable.getGuideInfo(mission_framework_util.makeTaskContext(task, missionState.guiTimeSeconds))`
- `findTask : function(uniqueTaskKey : string) : MissionInterface.TaskState<any, any>`  (call as `mission_framework_util.findTask(…)`): Looks up a task in the state of the `::mission/mission.gs` game script.
    - param `uniqueTaskKey`: Key from `MissionTaskUtil.makeTaskKey`.
    - returns: The task's state, or `nil`.
    - used 8x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/tasks/everything/everything.tl:24`: `local waitForTask = mission_framework_util.findTask(ctx.params)`
