# mission.tasks.waypoints.mission_task_waypoints

Definition file: `base/tealdef/mission/tasks/waypoints/mission_task_waypoints.d.tl` (in the game folder).

Task that waits for a line from a station group that passes given waypoints.

## MissionTaskWaypoints (record)

```teal
global record MissionTaskWaypoints is MissionInterface.TaskScript<MissionTaskWaypoints.State, MissionTaskWaypoints.Params>
```
Load: `local waypoints = ug_require "::/mission/tasks/waypoints/waypoints.tl" as MissionTaskWaypoints`

Task (`waypoints`) that completes when a line serving the station group passes all waypoints. It draws the next missing waypoint and the station as zones.

### MissionTaskWaypoints.Params (record)

```teal
record Params
```

Params of the task.

- `stationGroup : Engine.Entity`: Station group the line must serve.
- `waypoints : {MissionInterface.PosRadius}`: Areas the line must pass.
- `tolerance : number`: Distance tolerance in metres (default 5).

### MissionTaskWaypoints.State (record)

```teal
record State
```

State of the task.

- `result : Engine.Entity`: Line that passes all waypoints.
- `waypointsCompleted : {boolean}`: Per waypoint, whether some line passes it.
