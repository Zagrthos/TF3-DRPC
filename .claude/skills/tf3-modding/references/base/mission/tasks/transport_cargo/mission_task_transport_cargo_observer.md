# mission.tasks.transport_cargo.mission_task_transport_cargo_observer

Definition file: `base/tealdef/mission/tasks/transport_cargo/mission_task_transport_cargo_observer.d.tl` (in the game folder).

Task that shows the progress of several transport-cargo tasks and completes when all are done.

## MissionTaskTransportCargoObserver (record)

```teal
global record MissionTaskTransportCargoObserver is MissionInterface.TaskScript<MissionTaskTransportCargoObserver.State, MissionTaskTransportCargoObserver.Params>
```
Load: `local transport_cargo_observer = ug_require "::/mission/tasks/transport_cargo/transport_cargo_observer.tl" as MissionTaskTransportCargoObserver`

Task (`transport_cargo_observer`) that shows the counters of several `transport_cargo` tasks as sub-tasks and completes when all of them are complete.

### MissionTaskTransportCargoObserver.Params (record)

```teal
record Params
```

Params of the task.

- `tasks : {string}`: Unique keys of the observed tasks.

### MissionTaskTransportCargoObserver.State (record)

```teal
record State
```

State of the task.

- `success : boolean`: All observed tasks are complete.
