# mission.tasks.transport_cargo.mission_task_transport_passengers

Definition file: `base/tealdef/mission/tasks/transport_cargo/mission_task_transport_passengers.d.tl` (in the game folder).

Task that counts passengers travelling by line between two towns.

## MissionTaskTransportPassengers (record)

```teal
global record MissionTaskTransportPassengers is MissionInterface.TaskScript<MissionTaskTransportPassengers.State, MissionTaskTransportPassengers.Params>
```
Load: `local transport_passengers = ug_require "::/mission/tasks/transport_cargo/transport_passengers.tl" as MissionTaskTransportPassengers`

Task (`transport_passengers`) that counts persons who start using a line between two towns (either direction) and completes at `minCount`.

### MissionTaskTransportPassengers.Params (record)

```teal
record Params
```

Params of the task.

- `fromTownEntity : Engine.Entity`: One town.
- `toTownEntity : Engine.Entity`: The other town.
- `minCount : integer`: Number of persons (default 1); a counter is shown when greater than 1.

### MissionTaskTransportPassengers.State (record)

```teal
record State
```

State of the task.

- `passengerMap : {Engine.Entity : boolean}`: Persons already counted.
- `result : integer`: Number of persons counted.
