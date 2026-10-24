# mission.tasks.subsidies.mission_task_guide_subsidies

Definition file: `base/tealdef/mission/tasks/subsidies/mission_task_guide_subsidies.d.tl` (in the game folder).

Decorator for the subsidies task that queues an instruction when a subsidy is won or lost.

## MissionTaskGuideSubsidies (record)

```teal
global record MissionTaskGuideSubsidies is MissionInterface.TaskScript<DecoratorState<MissionTaskSubsidies.State, MissionTaskGuideSubsidies.State>, DecoratorParam<MissionTaskSubsidies.Params, MissionTaskGuideSubsidies.Param>>
```

Decorator for `MissionTaskSubsidies` that queues the instruction `wins<n>` or `fails<n>` when the n-th subsidy completes or fails.

### MissionTaskGuideSubsidies.Param (record)

```teal
record Param
```

Empty decorator params.

### MissionTaskGuideSubsidies.State (record)

```teal
record State
```

Decorator state.

- `uids : {number}`: Subsidy uids in spawn order, used for the instruction number.
- `isInitialized : boolean`: The uid list was filled.
