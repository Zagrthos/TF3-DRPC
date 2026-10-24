# mission.tutorial.tasks.mission_task_tutorial_init

Definition file: `base/tealdef/mission/tutorial/tasks/mission_task_tutorial_init.d.tl` (in the game folder).

Task that prepares the map at the start of the tutorial.

## MissionTaskTutorialInit (record)

```teal
global record MissionTaskTutorialInit is MissionInterface.TaskScript<MissionTaskTutorialInit.State, MissionTaskTutorialInit.Params>
```
Load: `local tutorial_init = ug_require "::/mission/tutorial/tasks/tutorial_init.tl" as MissionTaskTutorialInit`

First task of the base-game tutorial. Picks a story branch from the industries on the map, adjusts the capital's cargo needs, spawns missing industries and removes the road of the industry the player must connect.

### MissionTaskTutorialInit.BranchId (enum)

```teal
enum BranchId
```

Tutorial story branch.

- `"A"`: Branch for maps with an oil platform (farm, oil refinery; vegetables and fuel).
- `"B"`: Branch for maps with fishing grounds (forest, saw mill; fish and planks).

### MissionTaskTutorialInit.PreparationState (enum)

```teal
enum PreparationState
```

Progress of the map preparation.

- `"Waiting"`: Industries were requested; waiting one update.
- `"Ready"`: Subventions paused; roads are removed next.
- `"Finished"`: Preparation done; the task completes.
- `"Error"`: The map does not support the tutorial; the task completes.

### MissionTaskTutorialInit.Params (record)

```teal
record Params
```

Params of the tutorial init task.

- `capital : Engine.Entity`: Town in which the tutorial takes place.

### MissionTaskTutorialInit.State (record)

```teal
record State
```

State of the tutorial init task.

- `branch : BranchId`: Chosen story branch.
- `preparationState : PreparationState`: Progress of the preparation.
- `industrySnapshot : EntityUtil.EntityAndRevision`: The industry whose road was removed (farm or saw mill), with its revision. `TutorialStoryUtil.getIndustryToConnect` reads it.
- `industryResolved : boolean`: `true` once the industry was picked, even if the snapshot is `nil` because the industry is gone.
