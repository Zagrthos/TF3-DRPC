# mission.tasks.bulldoze.mission_task_bulldoze

Definition file: `base/tealdef/mission/tasks/bulldoze/mission_task_bulldoze.d.tl` (in the game folder).

Task that waits until the player bulldozes given entities.

## MissionTaskBulldoze (record)

```teal
global record MissionTaskBulldoze is MissionInterface.TaskScript<MissionTaskBulldoze.State, MissionTaskBulldoze.Params>
```
Load: `local bulldoze = ug_require "::/mission/tasks/bulldoze/bulldoze.tl" as MissionTaskBulldoze`

Task (`bulldoze`) that completes when the player has bulldozed every listed entity with the bulldozer. Each remaining entity gets an arrow marker; with more than one entity a counter is shown.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = bulldoze as MissionInterface.TaskScript<any, any>,
taskParams : MissionTaskBulldoze.Params = {
    entitiesToBulldoze = params.entityListBulldozable_debris,
},
```

### MissionTaskBulldoze.Params (record)

```teal
record Params
```

Params of the task.

- `entitiesToBulldoze : {Engine.Entity}`: Entities the player must bulldoze (required).
- `zones : {MissionInterface.Zone}`

### MissionTaskBulldoze.State (record)

```teal
record State
```

State of the task.

- `bulldozed : {Engine.Entity : boolean}`: Entity -> bulldozed.
