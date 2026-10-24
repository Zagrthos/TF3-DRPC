# mission.tasks.utility.mission_task_protected_entities

Definition file: `base/tealdef/mission/tasks/utility/mission_task_protected_entities.d.tl` (in the game folder).

Task that protects entities from player changes.

## MissionTaskProtectedEntities (record)

```teal
global record MissionTaskProtectedEntities is MissionInterface.TaskScript<MissionTaskProtectedEntities.State, MissionTaskProtectedEntities.Params>
```
Load: `local protected_entities = ug_require "::/mission/tasks/utility/protected_entities.tl" as MissionTaskProtectedEntities`

Task (`protected_entities`) that protects entities from player changes; lines are protected together with their station groups. It completes at once, so it is used with `requiredTaskToComplete` to stay open.

Example:
```lua
-- urbangames_campaign_mission_01
if taskKey == "protect_important_entities" then
    return {
        taskScript = protected_entities as MissionInterface.TaskScript<any, any>,
        requiredTaskToComplete = mission_task_util.makeTaskKey("missionEnd", "urbangames_campaign_mission_01::/mission/mission.script"),
        taskParams : MissionTaskProtectedEntities.Params = {
            getEntities = function() : {Engine.Entity} return { ... } end,
        },
    }
end
```

### MissionTaskProtectedEntities.Params (record)

```teal
record Params
```

Params of the task.

- `getEntities : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : {Engine.Entity}`: Returns the entities to protect; called once.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Entities (lines, constructions, ...).

### MissionTaskProtectedEntities.State (record)

```teal
record State
```

State of the task.

- `protected : {Engine.Entity : GameContext.Filters.ProtectionConfig | boolean}`: Entity -> protection config (lines) or `true`.
