# mission.tasks.transport_cargo.mission_task_transport_cargo_faster

Definition file: `base/tealdef/mission/tasks/transport_cargo/mission_task_transport_cargo_faster.d.tl` (in the game folder).

Delivery race against a simulated adversary within a time limit.

## MissionTaskTransportCargoFaster (record)

```teal
global record MissionTaskTransportCargoFaster is MissionInterface.TaskScript<MissionTaskTransportCargoFaster.State, MissionTaskTransportCargoFaster.Params>
```
Load: `local transport_cargo_faster = ug_require "::/mission/tasks/transport_cargo/transport_cargo_faster.tl" as MissionTaskTransportCargoFaster`

Task (`transport_cargo_faster`) in which the player must deliver `minCount` cargo to a target before a simulated adversary, whose count grows linearly to `minCount` over the time limit. Completes when either side reaches it.

### MissionTaskTransportCargoFaster.Params (record)

```teal
record Params
```

Params of the task.

- `cargoTypeRes : string`: Cargo type resource to count.
- `target : Engine.Entity`: Delivery target.
- `minCount : integer`: Amount to deliver.
- `getMinCount : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : integer`: Returns the amount to deliver when `minCount` is not set.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Amount to deliver.
- `simTimeMinutesToComplete : integer`: Time in simulation minutes in which the adversary reaches `minCount` (required).
- `debugInfoDeliveryRate : boolean`: Logs the adversary's delivery rate per year.
- `countdown : boolean`: Shows the remaining time instead of the adversary's counter.

### MissionTaskTransportCargoFaster.State (record)

```teal
record State
```

State of the task.

- `playerCount : integer`: Amount the player delivered.
- `minCount : integer`: Amount to deliver.
- `currentSimTimeMs : integer`: Game time (ms) of the last update.
- `initialSimTimeMs : integer`: Game time (ms) at spawn.
- `endSimTimeMs : integer`: Game time (ms) at which the adversary reaches `minCount`.
- `adversaryCount : integer`: Amount the simulated adversary delivered.
- `cargoTypeId : CargoTypeId`: Id of the cargo type.
