# mission.tasks.assign_vehicle.mission_task_visit_stations_ordered

Definition file: `base/tealdef/mission/tasks/assign_vehicle/mission_task_visit_stations_ordered.d.tl` (in the game folder).

Task that waits until a player vehicle stops in one area and then in another.

## MissionTaskVisitStationsOrdered (record)

```teal
global record MissionTaskVisitStationsOrdered is MissionInterface.TaskScript<MissionTaskVisitStationsOrdered.State, MissionTaskVisitStationsOrdered.Params>
```
Load: `local visit_stations_ordered = ug_require "::/mission/tasks/assign_vehicle/visit_stations_ordered.tl" as MissionTaskVisitStationsOrdered`

Task (`visit_stations_ordered`) that completes when a player vehicle on a line between the two areas has stopped at a terminal in `pos1` and then at one in `pos2`.

### MissionTaskVisitStationsOrdered.Params (record)

```teal
record Params
```

Params of the task.

- `pos1 : MissionInterface.PosRadius`: First area.
- `pos2 : MissionInterface.PosRadius`: Second area.
- `carrier : Carrier`: Carrier of the vehicles.

### MissionTaskVisitStationsOrdered.State (record)

```teal
record State
```

State of the task.

- `runtimeLine : Engine.Entity`
- `visitState : {Engine.Entity : integer}`: Vehicle -> 0 (not at `pos1` yet) or 1 (stopped at `pos1`).
- `complete : boolean`: A vehicle stopped at both in order.
