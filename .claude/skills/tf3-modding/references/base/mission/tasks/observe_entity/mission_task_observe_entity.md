# mission.tasks.observe_entity.mission_task_observe_entity

Definition file: `base/tealdef/mission/tasks/observe_entity/mission_task_observe_entity.d.tl` (in the game folder).

Task that waits until a condition holds for each of a set of entities.

## MissionTaskObserveEntity (record)

```teal
global record MissionTaskObserveEntity is MissionInterface.TaskScript<MissionTaskObserveEntity.State, MissionTaskObserveEntity.Params>
```
Load: `local observe_entity = ug_require "::/mission/tasks/observe_entity/observe_entity.tl" as MissionTaskObserveEntity`

Task (`observe_entity`) that completes when `observe` has returned true once for every entity. The entities and marked entities cannot be bulldozed meanwhile.

### MissionTaskObserveEntity.Params (record)

```teal
record Params
```

Params of the task.

- `markers : {MissionInterface.Marker}`: Markers shown while waiting.
- `entities : {Engine.Entity}`: Entities to observe.
- `observe : function(Engine.Entity) : boolean`: Condition checked per entity every update until it is met.
    - param `#1`: Entity.
    - returns: `true` when the entity is done.
- `allowEntityBulldoze : boolean`: Lets the player bulldoze the entities.

### MissionTaskObserveEntity.State (record)

```teal
record State
```

State of the task.

- `observed : {Engine.Entity : boolean}`: Entity -> condition met.
