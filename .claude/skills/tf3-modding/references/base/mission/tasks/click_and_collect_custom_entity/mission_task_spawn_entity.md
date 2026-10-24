# mission.tasks.click_and_collect_custom_entity.mission_task_spawn_entity

Definition file: `base/tealdef/mission/tasks/click_and_collect_custom_entity/mission_task_spawn_entity.d.tl` (in the game folder).

Task that spawns custom entities for the player to collect.

## MissionTaskSpawnEntity (record)

```teal
global record MissionTaskSpawnEntity is MissionInterface.TaskScript<MissionTaskSpawnEntity.State, MissionTaskSpawnEntity.Params>
```
Load: `local spawn_entity = ug_require "::/mission/tasks/click_and_collect_custom_entity/spawn_entity.tl" as MissionTaskSpawnEntity`

Task (`spawn_entity`) that spawns one custom entity per position, names it with `tooltip` and completes at once.

Example:
```lua
-- urbangames_campaign_mission_01: alligators to catch
taskScript = spawn_entity as MissionInterface.TaskScript<any, any>,
taskParams = {
    models = params.crocodileModels,
    positions = params.crocodilePositions,
    alignTerrain = true,
    tooltip = _("Catch Me!"),
},
```

### MissionTaskSpawnEntity.Params (record)

```teal
record Params
```

Params of the task.

- `models : {string}`: Model per position (same length as `positions`).
- `positions : {{number, number, number}}`: Spawn positions `{x, y, z}`.
- `alignTerrain : boolean`: Places the entities on the terrain instead of at the given height.
- `tooltip : string`: Name shown for the entities (default "Select Me!").
- `rotateZ : number`: Rotation around the vertical axis, in radians.

### MissionTaskSpawnEntity.State (record)

```teal
record State
```

State of the task.

- `runtimeEntityIds : {Engine.Entity}`: Spawned entities, in the order of `positions`.
