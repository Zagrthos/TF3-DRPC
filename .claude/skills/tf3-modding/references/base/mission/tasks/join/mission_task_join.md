# mission.tasks.join.mission_task_join

Definition file: `base/tealdef/mission/tasks/join/mission_task_join.d.tl` (in the game folder).

Task that waits until other tasks are complete.

## MissionTaskJoin (record)

```teal
global record MissionTaskJoin is MissionInterface.TaskScript<MissionTaskJoin.State, MissionTaskJoin.Params>
```
Load: `local join = ug_require "::/mission/tasks/join/join.tl" as MissionTaskJoin`

Task (`join`) that completes when all listed tasks are complete. Without params it never completes, which keeps its generic params (disabled menus, filters) active.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = join as MissionInterface.TaskScript<any, any>,
taskParams = mission_task_util.makeJoinParams({ "bulldoze" }, "urbangames_campaign_mission_01::/mission/mission.script"),
disabled = { "menu.managementMode", "menu.bulldozer" },
```

### MissionTaskJoin.Params (record)

```teal
record Params
```

Params of the task (see `MissionTaskUtil.makeJoinParams`).

- `taskKeys : {string}`: Unique keys of the tasks to wait for.
- `showProgress : boolean`: Shows a check mark per task as sub-task progress.

### MissionTaskJoin.State (record)

```teal
record State
```

State of the task.

- `waitingForTaskKeys : {string : boolean}`: Unique keys still awaited.
