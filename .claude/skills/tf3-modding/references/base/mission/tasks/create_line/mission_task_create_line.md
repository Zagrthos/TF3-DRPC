# mission.tasks.create_line.mission_task_create_line

Definition file: `base/tealdef/mission/tasks/create_line/mission_task_create_line.d.tl` (in the game folder).

Task that waits until a line serves given station areas.

## MissionTaskCreateLine (record)

```teal
global record MissionTaskCreateLine is MissionInterface.TaskScript<MissionTaskCreateLine.State, MissionTaskCreateLine.Params>
```
Load: `local create_line = ug_require "::/mission/tasks/create_line/create_line.tl" as MissionTaskCreateLine`

Task (`create_line`) that completes when one line serves all station areas, or with `target`, a line between the target and any station area. `transportModes` limits the accepted lines; `deleteLine` inverts the check.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = create_line as MissionInterface.TaskScript<any, any>,
taskParams : MissionTaskCreateLine.Params = {
    stations = {
        { pos = params.stationForestPos.pos, radius = params.stationForestPos.radius },
        { pos = params.stationHotelPos.pos, radius = params.stationHotelPos.radius },
    },
},
showInHistory = true,
```

### MissionTaskCreateLine.Params (record)

```teal
record Params
```

Params of the task.

- `target : MissionInterface.PosRadius`: Area that must be connected to the station areas.
- `stations : {MissionInterface.PosRadius}`: Station areas (at least two); drawn as zones.
- `getStations : function() : {MissionInterface.PosRadius}`: Returns the station areas; takes precedence over `stations`.
    - returns: Areas.
- `showZones : boolean`
- `deleteLine : boolean`: Completes when no such line exists any more.
- `transportModes : {TransportMode}`: Accepts only lines with one of these transport modes.

### MissionTaskCreateLine.State (record)

```teal
record State
```

State of the task.

- `result : Engine.Entity`: Line found, or `-1`.
