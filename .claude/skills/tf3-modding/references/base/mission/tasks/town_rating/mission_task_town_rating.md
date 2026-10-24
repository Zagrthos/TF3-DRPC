# mission.tasks.town_rating.mission_task_town_rating

Definition file: `base/tealdef/mission/tasks/town_rating/mission_task_town_rating.d.tl` (in the game folder).

Task that reports whether a town rating and a cargo amount are good enough.

## MissionTaskTownRating (record)

```teal
global record MissionTaskTownRating is MissionInterface.TaskScript<MissionTaskTownRating.State, MissionTaskTownRating.Params>
```
Load: `local town_rating = ug_require "::/mission/tasks/town_rating/town_rating.tl" as MissionTaskTownRating`

Task (`town_rating`) that checks every update whether a town has a good rating (rank 5 or 6) for one rating key and receives at least `minCount` of a cargo per year. It completes at once and is read by `MissionTaskTownRatingObserver`; a disconnected town always fails the check.

### MissionTaskTownRating.Params (record)

```teal
record Params
```

Params of the task.

- `entity : Engine.Entity`: Town to check.
- `key : TownRatingKey`: Rating to check (`urban_care`, `traffic_congestion`, `people_happiness`, `cargo_delivery`, `noise`, `pollution`).
- `minCount : integer`: Amount needed. For passengers it is the arrivals per year; for other cargo the town's total supply.
- `cargoTypeRes : ResName`: Cargo type resource to count.

### MissionTaskTownRating.State (record)

```teal
record State
```

State of the task.

- `cargoTypeId : CargoTypeId`: Id of the cargo type.
- `count : integer`: Current amount.
- `canComplete : boolean`: Rating and amount are both good enough.
