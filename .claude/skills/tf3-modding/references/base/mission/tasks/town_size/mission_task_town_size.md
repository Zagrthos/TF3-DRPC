# mission.tasks.town_size.mission_task_town_size

Definition file: `base/tealdef/mission/tasks/town_size/mission_task_town_size.d.tl` (in the game folder).

Task that waits until a town reaches a residential capacity.

## MissionTaskTownSize (record)

```teal
global record MissionTaskTownSize is MissionInterface.TaskScript<MissionTaskTownSize.State, MissionTaskTownSize.Params>
```
Load: `local town_size = ug_require "::/mission/tasks/town_size/town_size.tl" as MissionTaskTownSize`

Task (`town_size`) that completes when a town's residential capacity exceeds `minSize`, showing it as a counter with a star icon (`town_size_page` shows the same progress without completing).

### MissionTaskTownSize.Params (record)

```teal
record Params
```

Params of the task.

- `entity : Engine.Entity`: Town to watch.
- `minSize : integer`: Residential capacity to exceed.

### MissionTaskTownSize.State (record)

```teal
record State
```

State of the task.

- `size : integer`: Current residential capacity.
