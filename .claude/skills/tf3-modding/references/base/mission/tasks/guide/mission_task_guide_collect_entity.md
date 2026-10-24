# mission.tasks.guide.mission_task_guide_collect_entity

Definition file: `base/tealdef/mission/tasks/guide/mission_task_guide_collect_entity.d.tl` (in the game folder).

Decorator for the collect-entity task that points the player to one entity after the other.

## MissionTaskGuideCollectEntity (record)

```teal
global record MissionTaskGuideCollectEntity is MissionInterface.TaskScript<DecoratorState<MissionTaskCollectEntity.State, MissionTaskGuideCollectEntity.State>, DecoratorParam<MissionTaskCollectEntity.Params, MissionTaskGuideCollectEntity.Param>>
```

Decorator for `MissionTaskCollectEntity`. After `delay` seconds it shows the zone and marker of the first entity not collected yet and queues its instruction; the next hint follows once that entity is collected. It pauses while a voice-over or the guide window is visible.

Notes:

Combine with `TaskDecorator.decorate(collect_entity, guide_collect_entity)`. The n-th config belongs to the n-th entity.

### MissionTaskGuideCollectEntity.Param (record)

```teal
record Param
```

Decorator params.

- `delay : number`: GUI seconds after spawn before the first hint.
- `configs : {GuideConfig}`: Hints in entity order.
- `delayBetweenGuides : number`: GUI seconds between a collect and the next hint (default 2).

#### MissionTaskGuideCollectEntity.Param.GuideConfig (record)

```teal
record GuideConfig
```

Hint for one entity.

- `instructionData : MissionDialogueData.InstructionData`: Instruction of the hint.
- `zone : MissionInterface.Zone`: Zone shown with the hint.
- `marker : MissionInterface.Marker`: Marker shown with the hint; its `key` is the instruction key.

### MissionTaskGuideCollectEntity.State (record)

```teal
record State
```

Decorator state.

- `guide : {Engine.Entity : Param.GuideConfig}`: Entity -> hint.
- `triggered : {Engine.Entity : boolean}`: Entity -> hint shown.
- `isInitialized : boolean`: Hints were assigned to entities.
- `orderedEntities : {Engine.Entity}`: Entities in hint order.
- `lastNumberObserved : integer`: Number of collected entities at the last update.
- `timeStamp : number`: GUI time of the last collect.
- `lock : boolean`: Hints are paused (voice-over or guide window visible).
