# mission.tasks.utility.mission_task_check_entities

Definition file: `base/tealdef/mission/tasks/utility/mission_task_check_entities.d.tl` (in the game folder).

Task that checks that all entities referenced by the mission params exist.

## MissionTaskCheckEntities (record)

```teal
global record MissionTaskCheckEntities is MissionInterface.TaskScript<MissionTaskCheckEntities.State, MissionTaskCheckEntities.Params>
```
Load: `local check_entities = ug_require "::/mission/tasks/utility/check_entities.tl" as MissionTaskCheckEntities`

Task (`check_entities`) that checks that every entity in a mission params table exists (keys `entity_*`, `entityList_*`, `entityListBulldozable_*`, `entityMap_*`). It completes only if all exist; otherwise an error is logged and it stays open.

Example:
```lua
-- urbangames_campaign_mission_01
if taskKey == "check_entities" then
    return {
        taskScript = check_entities as MissionInterface.TaskScript<any, any>,
        taskParams = { params = params },
    }
end
```

### MissionTaskCheckEntities.Params (record)

```teal
record Params
```

Params of the task.

- `params : table`: Mission params table to check.

### MissionTaskCheckEntities.State (record)

```teal
record State
```

State of the task.

- `pass : boolean`: All entities exist.
