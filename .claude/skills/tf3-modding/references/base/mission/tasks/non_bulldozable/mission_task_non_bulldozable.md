# mission.tasks.non_bulldozable.mission_task_non_bulldozable

Definition file: `base/tealdef/mission/tasks/non_bulldozable/mission_task_non_bulldozable.d.tl` (in the game folder).

Task that keeps entities from being bulldozed.

## MissionTaskNonBulldozable (record)

```teal
global record MissionTaskNonBulldozable is MissionInterface.TaskScript<nil, MissionTaskNonBulldozable.Params>
```
Load: `local non_bulldozable = ug_require "::/mission/tasks/non_bulldozable/non_bulldozable.tl" as MissionTaskNonBulldozable`

Task (`non_bulldozable`) that never completes and keeps its entities from being bulldozed for the rest of the mission.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = non_bulldozable as MissionInterface.TaskScript<any, any>,
taskParams = {
    nonBulldozableEntities = mission_task_util.getNonBulldozableEntitiesFromParams(params as table),
    getNonBulldozableEntities = getNonBulldozableEntities,
},
```

### MissionTaskNonBulldozable.Params (record)

```teal
record Params
```

Params of the task.

- `nonBulldozableEntities : {Engine.Entity}`: Fixed entities; nothing is protected when this is `nil`.
- `getNonBulldozableEntities : function() : {Engine.Entity}`: Returns more entities every update.
    - returns: Entities.

### MissionTaskNonBulldozable.State (record)

```teal
record State
```

Empty state.
