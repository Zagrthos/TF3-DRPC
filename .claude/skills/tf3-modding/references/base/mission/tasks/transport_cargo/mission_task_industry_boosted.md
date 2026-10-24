# mission.tasks.transport_cargo.mission_task_industry_boosted

Definition file: `base/tealdef/mission/tasks/transport_cargo/mission_task_industry_boosted.d.tl` (in the game folder).

Task that waits until an industry is boosted.

## MissionTaskIndustryBoosted (record)

```teal
global record MissionTaskIndustryBoosted is MissionInterface.TaskScript<MissionTaskIndustryBoosted.State, MissionTaskIndustryBoosted.Params>
```
Load: `local industry_boosted = ug_require "::/mission/tasks/transport_cargo/industry_boosted.tl" as MissionTaskIndustryBoosted`

Task (`industry_boosted`) that completes when any industry of the given constructions has the given booster active (or, without `boosterIndex`, a boosted person capacity).

### MissionTaskIndustryBoosted.Params (record)

```teal
record Params
```

Params of the task.

- `boosterIndex : integer`: Index of the booster to check; `nil` checks the person-capacity boost.
- `constructionEntities : {Engine.Entity}`: Industry constructions to watch.

### MissionTaskIndustryBoosted.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: A boosted industry was found.
