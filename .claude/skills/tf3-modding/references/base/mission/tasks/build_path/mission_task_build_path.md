# mission.tasks.build_path.mission_task_build_path

Definition file: `base/tealdef/mission/tasks/build_path/mission_task_build_path.d.tl` (in the game folder).

Task that waits until the transport network connects pairs of entities or areas.

## MissionTaskBuildPath (record)

```teal
global record MissionTaskBuildPath is MissionInterface.TaskScript<MissionTaskBuildPath.State, MissionTaskBuildPath.Params>
```
Load: `local build_path = ug_require "::/mission/tasks/build_path/build_path.tl" as MissionTaskBuildPath`

Task (`build_path`) that completes when a path exists for every pair, between the network nodes in two areas or between two entities. With more than one pair a counter is shown, and the unconnected areas are drawn.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = build_path as MissionInterface.TaskScript<any, any>,
taskParams = {
    zonesToConnect = {
        {
            { pos = params.connectStreets_1_1_zone.pos, radius = params.connectStreets_1_1_zone.radius + 5 },
            { pos = params.connectStreets_1_2_zone.pos, radius = params.connectStreets_1_2_zone.radius + 5 },
        },
    },
    transportMode = api.type["enum"].TransportMode.TRUCK,
},
```

### MissionTaskBuildPath.Params (record)

```teal
record Params
```

Params of the task. Area pairs take precedence over entity pairs.

- `entitiesToConnect : {{Engine.Entity, Engine.Entity}}`: Pairs of entities (stations, constructions, network entities) to connect.
- `zonesToConnect : {{MissionInterface.PosRadius, MissionInterface.PosRadius}}`: Pairs of areas to connect.
- `getZonesToConnect : function() : {{MissionInterface.PosRadius, MissionInterface.PosRadius}}`: Returns the area pairs; takes precedence over `zonesToConnect`.
    - returns: Area pairs.
- `renderZonesForEntities : {{MissionInterface.PosRadius, MissionInterface.PosRadius}}`: Area pairs drawn instead of the connected areas.
- `getEntitiesToConnect : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : {{Engine.Entity, Engine.Entity}}`: Returns the entity pairs every update; takes precedence over `entitiesToConnect`.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Entity pairs.
- `transportMode : TransportMode`: Transport mode the path must allow (default train).

### MissionTaskBuildPath.State (record)

```teal
record State
```

State of the task.

- `complete : {boolean}`: Per pair, whether it is connected.
- `runtimeEntitiesToConnect : {{Engine.Entity, Engine.Entity}}`: Entity pairs in use.
