# mission.tasks.town_size.town_size_task_util

Definition file: `base/tealdef/mission/tasks/town_size/town_size_task_util.d.tl` (in the game folder).

Shared update function of the town-size tasks.

## Local types (only visible in this file)

- `S : MissionTaskTownSize.State`
- `P : MissionTaskTownSize.Params`
- `TaskContext : MissionInterface.TaskContext`

## TownSizeTaskUtil (record)

```teal
global record TownSizeTaskUtil
```
Load: `local town_size_task_util = ug_require "::/mission/tasks/town_size/town_size_task_util.tl" as TownSizeTaskUtil`

Shared code of the town-size tasks.

- `onUpdate : function(ctx : TaskContext<S, P>, _readOnlyApi : MissionInterface.MissionTaskReadOnlyApi)`  (call as `town_size_task_util.onUpdate(…)`): Updates the town size and sets the task progress (counter up to `minSize`, 750 or 1000, with a 1-3 star icon).
    - param `ctx`: Context of the task.
    - param `_readOnlyApi`: Unused.
    - used 2x in the base game, e.g. `base/content/mission/mission/tasks/town_size/town_size.tl:15`: `onUpdate = town_size_task_util.onUpdate`
