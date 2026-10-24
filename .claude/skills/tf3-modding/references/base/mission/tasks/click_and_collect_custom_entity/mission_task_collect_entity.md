# mission.tasks.click_and_collect_custom_entity.mission_task_collect_entity

Definition file: `base/tealdef/mission/tasks/click_and_collect_custom_entity/mission_task_collect_entity.d.tl` (in the game folder).

Task in which the player collects custom entities by clicking them.

## MissionTaskCollectEntity (record)

```teal
global record MissionTaskCollectEntity is MissionInterface.TaskScript<MissionTaskCollectEntity.State, MissionTaskCollectEntity.Params>
```
Load: `local collect_entity = ug_require "::/mission/tasks/click_and_collect_custom_entity/collect_entity.tl" as MissionTaskCollectEntity`

Task (`collect_entity`) in which the player selects custom entities to collect them. A collected entity is removed with a dust effect and an optional sound. The task completes when all are collected; with more than one entity it shows a counter.

Notes:

The entities must be non-bulldozable. The campaign mods spawn them with `MissionTaskSpawnEntity`
and read their ids from that task in `getEntities`.

### MissionTaskCollectEntity.Params (record)

```teal
record Params
```

Make sure the entities used here are non-bulldozable!

- `markers : {MissionInterface.Marker}`
- `models : {string}`
- `positions : {{number, number, number}}`
- `zones : {MissionInterface.Zone}`: Zones shown while the task is open.
- `entities : {Engine.Entity}`: Entities to collect.
- `getEntities : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : {Engine.Entity}`: Returns the entities to collect at the first update; replaces `entities`.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Entities.
- `justRemove : boolean`: Removes all entities at once and completes.
- `soundEffectFile : string`: Sound played when an entity is collected.
- `terrainHeightCheck : boolean`: Ignores clicks on entities below the terrain.
- `useSelectorEntityFilter : boolean`: Only the entities to collect can be selected while the task is open.

### MissionTaskCollectEntity.State (record)

```teal
record State
```

State of the task.

- `observed : {Engine.Entity : boolean}`: Entity -> collected.
- `runtimeEntityIds : {Engine.Entity}`: Entities to collect.
- `removed : {Engine.Entity : boolean}`: Entities already removed.
