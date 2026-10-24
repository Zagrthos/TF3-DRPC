# mission.tasks.wait.mission_task_wait_for

Definition file: `base/tealdef/mission/tasks/wait/mission_task_wait_for.d.tl` (in the game folder).

Task that completes when a condition function returns true, optionally with a countdown.

## MissionTaskWaitFor (record)

```teal
global record MissionTaskWaitFor is MissionInterface.TaskScript<MissionTaskWaitFor.State, MissionTaskWaitFor.Params>
```
Load: `local wait_for = ug_require "::/mission/tasks/wait/wait_for.tl" as MissionTaskWaitFor`

Task (`wait_for`) that completes when `waitFn` returns true. With `countdown` it also ends when the time runs out, showing the remaining time as sub-task progress.

Example:
```lua
-- urbangames_campaign_mission_02: wait until a train reaches a zone
taskScript = wait_for as MissionInterface.TaskScript<any, any>,
taskParams = {
    waitFn = function(_readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : boolean
        local entities = api.engine.util.octree.findEntitiesInCircle(
            api.type.Vec2f.new(params.waitForMachinesZone1.pos[1], params.waitForMachinesZone1.pos[2]),
            params.waitForMachinesZone1.radius, api.type.ComponentType.CARRIAGE)
        return #entities > 0
    end,
},
```

### MissionTaskWaitFor.Params (record)

```teal
record Params
```

Params of the task.

- `waitFn : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : boolean`: Condition checked every update (required).
    - param `readOnlyApi`: Read-only mission API.
    - returns: `true` to complete the task.
- `zones : {MissionInterface.Zone}`: Zones shown while waiting.
- `markers : {MissionInterface.Marker}`: Markers shown while waiting.
- `getZones : function() : {MissionInterface.Zone}`: Returns the zones to show; takes precedence over `zones`.
    - returns: Zones.
- `getMarkers : function() : {MissionInterface.Marker}`: Returns the markers to show; takes precedence over `markers`.
    - returns: Markers.
- `simTimeMinutesToComplete : integer`: Length of the countdown in simulation minutes (game time); required with `countdown`.
- `countdown : boolean`: Ends the task (as failed) when the countdown runs out.

### MissionTaskWaitFor.State (record)

```teal
record State
```

State of the task.

- `isComplete : boolean`: Last result of `waitFn`.
- `hasFailed : boolean`: The countdown ran out.
- `currentSimTimeMs : integer`: Game time (ms) of the last update.
- `initialSimTimeMs : integer`: Game time (ms) at which the countdown started.
- `endSimTimeMs : integer`: Game time (ms) at which the countdown ends.
