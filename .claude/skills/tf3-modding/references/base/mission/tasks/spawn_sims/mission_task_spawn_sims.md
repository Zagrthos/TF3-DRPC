# mission.tasks.spawn_sims.mission_task_spawn_sims

Definition file: `base/tealdef/mission/tasks/spawn_sims/mission_task_spawn_sims.d.tl` (in the game folder).

Task that places custom person entities on footpaths.

## MissionTaskSpawnSims (record)

```teal
global record MissionTaskSpawnSims is MissionInterface.TaskScript<MissionTaskSpawnSims.State, MissionTaskSpawnSims.Params>
```
Load: `local spawn_sims = ug_require "::/mission/tasks/spawn_sims/spawn_sims.tl" as MissionTaskSpawnSims`

Task (`spawn_sims`) that spawns custom person entities on the pedestrian lanes of the given network entities, spread by Poisson-disk sampling, then completes.

### MissionTaskSpawnSims.Params (record)

```teal
record Params
```

Params of the task.

- `models : {string}`: Models to choose from at random for each entity.
- `transportNetworkIds : {Engine.Entity}`: Entities with a transport network (streets, constructions) to fill.
- `seed : integer`: Random seed of the sampling.

### MissionTaskSpawnSims.State (record)

```teal
record State
```

State of the task.

- `runtimeEntityIds : {Engine.Entity}`: Spawned entities.
