# mission.tasks.buy_vehicle.mission_task_buy_vehicle

Definition file: `base/tealdef/mission/tasks/buy_vehicle/mission_task_buy_vehicle.d.tl` (in the game folder).

Task that waits until the player owns or buys suitable vehicles.

## MissionTaskBuyVehicle (record)

```teal
global record MissionTaskBuyVehicle is MissionInterface.TaskScript<MissionTaskBuyVehicle.State, MissionTaskBuyVehicle.Params>
```
Load: `local buy_vehicle = ug_require "::/mission/tasks/buy_vehicle/buy_vehicle.tl" as MissionTaskBuyVehicle`

Task (`buy_vehicle`) that completes when the player owns `minCount` vehicles of a carrier that can carry `cargoTypeRes`, or, with `allowBuyVehicles`, has bought the listed number of each model in the vehicle store.

Example:
```lua
-- urbangames_campaign_mission_01
taskScript = buy_vehicle as MissionInterface.TaskScript<any, any>,
taskParams : MissionTaskBuyVehicle.Params = {
    carrier = api.type["enum"].Carrier.ROAD,
    cargoTypeRes = "::/cargos/logs/logs.cargo",
},
showInHistory = true,
```

### MissionTaskBuyVehicle.Params (record)

```teal
record Params
```

Params of the task. Set either `cargoTypeRes` or `allowBuyVehicles`.

- `carrier : Carrier`: Carrier of the vehicles.
- `cargoTypeRes : string`: Cargo type the vehicles must carry.
- `minCount : integer`: Number of vehicles (default 1).
- `allowBuyVehicles : {string : integer}`: Model file -> number to buy.

### MissionTaskBuyVehicle.State (record)

```teal
record State
```

State of the task.

- `result : {Engine.Entity}`: Matching vehicles found.
- `purchasedVehicles : {string : integer}`: Model file -> number bought while the task is open.
