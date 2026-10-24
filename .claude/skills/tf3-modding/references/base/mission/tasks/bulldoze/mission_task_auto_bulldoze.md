# mission.tasks.bulldoze.mission_task_auto_bulldoze

Definition file: `base/tealdef/mission/tasks/bulldoze/mission_task_auto_bulldoze.d.tl` (in the game folder).

Task that removes constructions.

## MissionTaskAutoBulldoze (record)

```teal
global record MissionTaskAutoBulldoze is MissionInterface.TaskScript<MissionTaskAutoBulldoze.State, MissionTaskAutoBulldoze.Params>
```
Load: `local auto_bulldoze = ug_require "::/mission/tasks/bulldoze/auto_bulldoze.tl" as MissionTaskAutoBulldoze`

Task (`auto_bulldoze`) that removes the given constructions that still exist and completes.

### MissionTaskAutoBulldoze.Params (record)

```teal
record Params
```

Params of the task.

- `constructionEntities : {Engine.Entity}`: Constructions to remove.

### MissionTaskAutoBulldoze.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: The removal was sent.
