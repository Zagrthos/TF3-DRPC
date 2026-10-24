# mission.tasks.utility.mission_task_industry_activity

Definition file: `base/tealdef/mission/tasks/utility/mission_task_industry_activity.d.tl` (in the game folder).

Task that turns the stocks or the station of an industry on or off.

## MissionTaskIndustryActivity (record)

```teal
global record MissionTaskIndustryActivity is MissionInterface.TaskScript<any, MissionTaskIndustryActivity.Params>
```
Load: `local industry_activity = ug_require "::/mission/tasks/utility/industry_activity.tl" as MissionTaskIndustryActivity`

Task (`industry_activity`) that rebuilds an industry construction with the `disableStocks` and `disableStation` construction params set, keeping its name, and completes at once.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = industry_activity as MissionInterface.TaskScript<any, any>,
taskParams = {
    entity = params.entity_newOrleansForest,
    disableStocks = false,
    disableStation = false,
},
```

### MissionTaskIndustryActivity.Params (record)

```teal
record Params
```

Params of the task.

- `entity : Engine.Entity`: Industry (subconstruction) entity.
- `disableStocks : boolean`: Value of the `disableStocks` construction param.
- `disableStation : boolean`: Value of the `disableStation` construction param.

### MissionTaskIndustryActivity.State (record)

```teal
record State
```

Empty state.
