# mission.tasks.guide.mission_task_guide

Definition file: `base/tealdef/mission/tasks/guide/mission_task_guide.d.tl` (in the game folder).

Task that plays one queued instruction and completes when it has finished.

## MissionTaskGuide (record)

```teal
global record MissionTaskGuide is MissionInterface.TaskScript<MissionTaskGuide.State, MissionTaskGuide.Params>
```
Load: `local guide = ug_require "::/mission/tasks/guide/guide.tl" as MissionTaskGuide`

Task (`guide`) that queues the instruction `instructionKey` and completes when it has played.

### MissionTaskGuide.Params (record)

```teal
record Params
```

Params of the task.

- `instructionKey : string`: Key passed to the task info's `makeInstructionData`.

### MissionTaskGuide.State (record)

```teal
record State
```

State of the task.

- `wip : boolean`: The instruction was queued.
- `complete : boolean`
