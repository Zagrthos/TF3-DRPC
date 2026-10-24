# mission.tasks.town_rating.mission_task_town_rating_observer

Definition file: `base/tealdef/mission/tasks/town_rating/mission_task_town_rating_observer.d.tl` (in the game folder).

Task that turns several town-rating tasks into one rising or falling progress value.

## MissionTaskTownRatingObserver (record)

```teal
global record MissionTaskTownRatingObserver is MissionInterface.TaskScript<MissionTaskTownRatingObserver.State, MissionTaskTownRatingObserver.Params>
```
Load: `local town_rating_observer = ug_require "::/mission/tasks/town_rating/town_rating_observer.tl" as MissionTaskTownRatingObserver`

Task (`town_rating_observer`) that sums +1 for each passing and -1 for each failing rating task every update. A positive sum adds twice its value to the progress, otherwise the progress drops by 1. It completes with success at 100 % and with failure at 0 %, starting at 50 %. A star icon drops one level every two game years.

### MissionTaskTownRatingObserver.Params (record)

```teal
record Params
```

Params of the task.

- `ratingTasks : {string}`: Unique keys of the `town_rating` tasks to watch.
- `simTimeMinutesToComplete : integer`

### MissionTaskTownRatingObserver.State (record)

```teal
record State
```

State of the task.

- `endTick : integer`: Value of `cumulativeRating` that means 100 %.
- `cumulativeRating : integer`: Current accumulated value.
- `success : boolean`: `true` or `false` once the result is decided, `nil` before.
- `initialSimTimeMs : integer`: Game time (ms) at which the current star period started.
- `endSimTimeMs : integer`: Game time (ms) at which the current star period ends (two game years).
- `currentSimTimeMs : integer`: Game time (ms) of the last update.
- `stars : integer`: Stars still reachable (3 at start).
