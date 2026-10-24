# mission.tasks.auto_builder.mission_task_electrify

Definition file: `base/tealdef/mission/tasks/auto_builder/mission_task_electrify.d.tl` (in the game folder).

Task that electrifies the track between node pairs.

## MissionTaskElectrify (record)

```teal
global record MissionTaskElectrify is MissionInterface.TaskScript<MissionTaskElectrify.State, MissionTaskElectrify.Params>
```
Load: `local electrify = ug_require "::/mission/tasks/auto_builder/electrify.tl" as MissionTaskElectrify`

Task (`electrify`) that finds the train path between each node pair and rebuilds its edges with the `simple_catenary` track template, then completes.

### MissionTaskElectrify.Params (record)

```teal
record Params
```

Params of the task.

- `nodePairs : {{Engine.Entity, Engine.Entity}}`: Pairs of track nodes.

### MissionTaskElectrify.State (record)

```teal
record State
```

State of the task.

- `isComplete : {boolean}`: Per pair, whether it was processed.
