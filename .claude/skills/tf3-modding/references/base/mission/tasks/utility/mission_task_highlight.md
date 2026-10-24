# mission.tasks.utility.mission_task_highlight

Definition file: `base/tealdef/mission/tasks/utility/mission_task_highlight.d.tl` (in the game folder).

Task that shows markers and zones.

## MissionTaskHighlight (record)

```teal
global record MissionTaskHighlight is MissionInterface.TaskScript<nil, MissionTaskHighlight.Params>
```
Load: `local highlight = ug_require "::/mission/tasks/utility/highlight.tl" as MissionTaskHighlight`

Task (`highlight`) that shows markers and zones. It completes at once, so it is used with `requiredTaskToComplete` to stay open until another task completes.

### MissionTaskHighlight.Params (record)

```teal
record Params
```

Params of the task.

- `markers : {MissionInterface.Marker}`: Markers to show.
- `zones : {MissionInterface.Zone}`: Zones to show.
- `getMarkers : function() : {MissionInterface.Marker}`: Returns the markers to show; takes precedence over `markers`.
    - returns: Markers.
- `getZones : function() : {MissionInterface.Zone}`: Returns the zones to show; takes precedence over `zones`.
    - returns: Zones.

### MissionTaskHighlight.State (record)

```teal
record State
```

Empty state.
