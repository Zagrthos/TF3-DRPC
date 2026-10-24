# mission.tasks.transport_cargo.mission_task_transport_cargo

Definition file: `base/tealdef/mission/tasks/transport_cargo/mission_task_transport_cargo.d.tl` (in the game folder).

Task that waits until an amount of cargo was delivered to targets.

## MissionTaskTransportCargo (record)

```teal
global record MissionTaskTransportCargo is MissionInterface.TaskScript<MissionTaskTransportCargo.State, MissionTaskTransportCargo.Params>
```
Load: `local transport_cargo = ug_require "::/mission/tasks/transport_cargo/transport_cargo.tl" as MissionTaskTransportCargo`

Task (`transport_cargo`) that counts deliveries of a cargo type to the targets (optionally only from given sources) through the arrival tracker and completes at `minCount`. Shows a counter with the cargo icon.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = transport_cargo as MissionInterface.TaskScript<any, any>,
taskParams : MissionTaskTransportCargo.Params = {
    cargoTypeRes = "::/cargos/logs/logs.cargo",
    targets = { params.entity_hotelTarget },
    minCount = 6,
},
showInHistory = true,
```

### MissionTaskTransportCargo.Params (record)

```teal
record Params
```

Params of the task.

- `cargoTypeRes : string`: Cargo type resource to count (e.g. `"::/cargos/logs/logs.cargo"`).
- `targets : {Engine.Entity}`: Delivery targets (towns, constructions or their entities).
- `getTargets : function() : {Engine.Entity}`: Returns the targets when the task starts; takes precedence over `targets`.
    - returns: Target entities.
- `sources : {Engine.Entity}`: Only cargo from these sources counts.
- `minCount : integer`: Amount to deliver.
- `fakeMinCount : integer`: Maximum shown in the progress instead of `minCount`.
- `getMinCount : function(readOnlyApi : MissionInterface.MissionTaskReadOnlyApi) : integer`: Returns the amount to deliver when `minCount` is not set.
    - param `readOnlyApi`: Read-only mission API.
    - returns: Amount to deliver.

### MissionTaskTransportCargo.State (record)

```teal
record State
```

State of the task.

- `result : integer`: Amount delivered so far.
- `minCount : integer`: Amount to deliver.
- `cargoTypeId : CargoTypeId`: Id of the cargo type.
- `ids : {string : {Engine.Entity}}`: Arrival-tracker filter name -> `{target, source}`.
