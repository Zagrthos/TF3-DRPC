# mission.tasks.click_entity.mission_task_click_entity

Definition file: `base/tealdef/mission/tasks/click_entity/mission_task_click_entity.d.tl` (in the game folder).

Task that waits until the player clicks given entities or markers.

## MissionTaskClickEntity (record)

```teal
global record MissionTaskClickEntity is MissionInterface.TaskScript<MissionTaskClickEntity.State, MissionTaskClickEntity.Params>
```
Load: `local click_entity = ug_require "::/mission/tasks/click_entity/click_entity.tl" as MissionTaskClickEntity`

Task (`click_entity`) that completes when the player has selected every candidate entity or clicked every marker (or one of them with `clickJustOne`). The candidates cannot be bulldozed meanwhile.

### MissionTaskClickEntity.Params (record)

```teal
record Params
```

Params of the task. Use either markers or candidates.

- `markers : {MissionInterface.Marker}`: at the moment: either markers or candidates can be provided.
- `getMarkers : function() : {MissionInterface.Marker}`: Returns the markers; takes precedence over `markers`.
    - returns: Markers.
- `candidates : {Engine.Entity}`: Entities to select.
- `getCandidates : function() : {Engine.Entity}`: Returns the entities to select; takes precedence over `candidates`.
    - returns: Entities.
- `allowEntityBulldoze : boolean`: Lets the player bulldoze the candidates.
- `clickJustOne : boolean`: Completes after the first click.

### MissionTaskClickEntity.State (record)

```teal
record State
```

State of the task.

- `observed : {Result}`: Clicks so far, without duplicates.
- `error : boolean`: A candidate did not exist at spawn; the task completes at once.

#### MissionTaskClickEntity.State.Result (record)

```teal
record Result
```

One click.

- `entity : Engine.Entity`: Selected entity.
- `marker : string`: Clicked marker.
