# mission.tasks.utility.mission_task_store_entity

Definition file: `base/tealdef/mission/tasks/utility/mission_task_store_entity.d.tl` (in the game folder).

Task that stores an entity as a snapshot for other tasks to read.

## MissionTaskStoreEntity (record)

```teal
global record MissionTaskStoreEntity is MissionInterface.TaskScript<MissionTaskStoreEntity.State, MissionTaskStoreEntity.Params>
```
Load: `local store_entity = ug_require "::/mission/tasks/utility/store_entity.tl" as MissionTaskStoreEntity`

Task (`store_entity`) that stores an entity as a snapshot (id and revision) in its state and then completes. The base-game tutorial stores its capital (`store_capital`) and second town (`store_town2`) this way and reads them with `TutorialStoryUtil.getCapital` and `TutorialStoryUtil.getSecondTown`.

Notes:

Each update calls `storeFn` until it returns an entity. The snapshot is `nil` when the entity no longer exists
or `isExpectedFn` rejects it; `resolved` is `true` in both cases and the task completes with an error in the
log. Completion waits one more update (`waitForEngineSwap`). Saves from before this task existed, which kept
the entity id in a `store_any` task, are converted by `handleLegacy`; if the stored entity no longer passes
`isExpectedFn`, it looks for a replacement among all entities.

### MissionTaskStoreEntity.Params (record)

```teal
record Params
```

Params of the task.

- `storeFn : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : Engine.Entity`: Finds the entity to store; called every update until it returns an entity.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Entity to store, or `nil` to try again.
- `isExpectedFn : function(entity : Engine.Entity) : boolean`: Optional check of the found entity, for example whether it is a town. A rejected entity is logged and no snapshot is stored.
    - param `entity`: The entity returned by `storeFn`.
    - returns: `true` if the entity is the expected kind.

### MissionTaskStoreEntity.State (record)

```teal
record State
```

State of the task.

- `snapshot : EntityUtil.EntityAndRevision`: nil if storeFn yielded no valid entity

Stored entity with its revision, or `nil` if `storeFn` yielded no valid entity.
- `resolved : boolean`: `true` once `storeFn` returned an entity, whether or not the snapshot is valid.
- `waitForEngineSwap : boolean`: Delays completion by one update after the entity was stored.
