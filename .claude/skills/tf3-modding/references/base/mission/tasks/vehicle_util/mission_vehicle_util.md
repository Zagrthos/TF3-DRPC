# mission.tasks.vehicle_util.mission_vehicle_util

Definition file: `base/tealdef/mission/tasks/vehicle_util/mission_vehicle_util.d.tl` (in the game folder).

Vehicle and line helpers used by mission tasks.

## MissionVehicleUtil (record)

```teal
global record MissionVehicleUtil
```
Load: `local mission_vehicle_util = ug_require "::/mission/tasks/vehicle_util/mission_vehicle_util.tl" as MissionVehicleUtil`

Vehicle and line helpers used by mission tasks.

- `createLine : function(stationGroup : Engine.Entity) : Engine.Entity`  (call as `mission_vehicle_util.createLine(…)`): Creates a one-stop line at a station group and gives it to a newly spawned AI player.
    - param `stationGroup`: Station group of the stop.
    - returns: The new line entity.
    - used 2x in the base game, e.g. `mods/release/urbangames_campaign_mission_05/content/mission/mission/tasks/drilling/observe_drilling.tl:82`: `local newLine = mission_vehicle_util.createLine(ctx.params.harborStationGroupEntity)`
- `startVehicle : function(vehicleEntity : Engine.Entity, stop? : boolean)`  (call as `mission_vehicle_util.startVehicle(…)`): Starts or stops a vehicle (user stop flag) if it is not already in that state.
    - param `vehicleEntity`: Vehicle.
    - param `stop`: `true` to stop, otherwise start.
    - used 3x in the base game, e.g. `base/content/mission/mission/tasks/vehicle_util/start_vehicles.tl:23`: `mission_vehicle_util.startVehicle(vehicle, ctx.params.stop)`
- `deleteLine : function(oldLine : Engine.Entity)`  (call as `mission_vehicle_util.deleteLine(…)`): Deletes a line if it exists; logs an error if it still has vehicles.
    - param `oldLine`: Line entity.
    - used 2x in the base game, e.g. `base/content/mission/mission/tasks/vehicle_util/sell_vehicles.tl:33`: `mission_vehicle_util.deleteLine(line)`
