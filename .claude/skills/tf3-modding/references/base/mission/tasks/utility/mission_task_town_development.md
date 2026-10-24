# mission.tasks.utility.mission_task_town_development

Definition file: `base/tealdef/mission/tasks/utility/mission_task_town_development.d.tl` (in the game folder).

Task that switches the development of towns on or off.

## MissionTaskTownDevelopment (record)

```teal
global record MissionTaskTownDevelopment is MissionInterface.TaskScript<any, MissionTaskTownDevelopment.Params>
```
Load: `local town_development = ug_require "::/mission/tasks/utility/town_development.tl" as MissionTaskTownDevelopment`

Task (`town_development`) that switches the development of towns on or off and completes at once.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = town_development as MissionInterface.TaskScript<any, any>,
taskParams = { townDevelopmentMap = params.entityMap_townDevelopment },
```

### MissionTaskTownDevelopment.Params (record)

```teal
record Params
```

Params of the task.

- `townDevelopmentMap : {Engine.Entity : boolean}`: Town entity -> development active.

### MissionTaskTownDevelopment.State (record)

```teal
record State
```

Empty state.
