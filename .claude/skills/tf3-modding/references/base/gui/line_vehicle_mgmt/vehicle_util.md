# gui.line_vehicle_mgmt.vehicle_util

Definition file: `base/tealdef/gui/line_vehicle_mgmt/vehicle_util.d.tl` (in the game folder).

Small helpers for vehicles and depots - lifespan and age, condition text and icons, carrier of a depot, vehicle type, capacity checks and the year-dependent value ranges used to scale the store's progress bars.

## VehicleUtil (record)

```teal
global record VehicleUtil
```
Load: `local vehicle_util = ug_require "::/gui/line_vehicle_mgmt/vehicle_util.tl" as VehicleUtil`

Small helpers for vehicles and depots.

- `makePart : function(modelId : integer, forward : boolean, color : Vec3f) : TransportVehiclePart`  (call as `vehicle_util.makePart(…)`): Creates a `TransportVehiclePart` for a model, with load config 0 for every compartment.
    - param `modelId`: Model id of a vehicle.
    - param `forward`: False to reverse the part.
    - param `color`: Optional part colour.
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:3967`: `local part = vehicle_util.makePart(modelId, true, color)`
- `getLifespan : function(integer) : integer`  (call as `vehicle_util.getLifespan(…)`): Lifespan of a vehicle model.
    - returns: `maintenance.lifespan` times 1000; compared with part purchase times in game time.
- `getMinPurchaseTimeAndLifespan : function(Engine.Component.TransportVehicle) : {integer, integer}`  (call as `vehicle_util.getMinPurchaseTimeAndLifespan(…)`): Purchase time and lifespan of the oldest part of a transport vehicle (shortest lifespan on ties).
    - returns: `{purchaseTime, lifespan}`.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/line/line_eow.script.tl:126`: `return vehicle_util.getMinPurchaseTimeAndLifespan(transportVehicle)`
- `getDepotCarriers : function(Engine.Entity) : {Carrier}`  (call as `vehicle_util.getDepotCarriers(…)`): Carriers of the transport modes enabled in a vehicle depot.
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/constructionHudIconMaster.tl:84`: `carriers = vehicle_util.getDepotCarriers(param.entity)`
- `getDepotCarrier : function(Engine.Entity) : Carrier`  (call as `vehicle_util.getDepotCarrier(…)`): The single carrier of a vehicle depot.
    - returns: The carrier, or nil (with a warning in the log) if the depot has none or several.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:7141`: `carrier = vehicle_util.getDepotCarrier(depotEntity),`
- `getDepotTransportModes : function(Engine.Entity) : {TransportMode : boolean}`  (call as `vehicle_util.getDepotTransportModes(…)`): Transport modes of a vehicle depot.
    - returns: The depot's `transportModes`, or an empty table if the depot has no in or out nodes.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/availability.script.tl:99`: `local depotTransportModes = vehicle_util.getDepotTransportModes(e)`
- `getVehicleTypeFromModel : function(model : Model) : VehicleType`  (call as `vehicle_util.getVehicleTypeFromModel(…)`): Vehicle type of a model from its `transportVehicle.transportModes`; `Train` if nothing matches.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/availability.script.tl:118`: `vehicleType = vehicle_util.getVehicleTypeFromModel(model)`
- `getVehicleTypeFromEntity : function(ent : Engine.Entity) : VehicleType`  (call as `vehicle_util.getVehicleTypeFromEntity(…)`): Vehicle type of a vehicle entity from its carrier (bus vs. truck from the transport modes, helicopter vs. airplane from the first part's model).
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/vehicle_warning.sim.script.tl:14`: `old.typeMapping[entityAndRev.entity] = vehicle_util.getVehicleTypeFromEntity(entityAndRev.entity)`
- `ConditionLevels : {string}`: Translated condition names, worst to best - Very Bad, Bad, Mediocre, Good, Very Good.
- `getConditionText : function(number) : string`  (call as `vehicle_util.getConditionText(…)`): Condition name for a maintenance state from 0 to 1 (split into five equal steps).
    - used 6x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:1999`: `text = vehicle_util.getConditionText(state:old()[1]),`
- `getConditionIcon : function(number) : string`  (call as `vehicle_util.getConditionIcon(…)`): Condition icon path (`::/gui/line_vehicle_mgmt/icons/condition_*.tga`) for a maintenance state from 0 to 1.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_react_util.tl:54`: `return {vehicle_util.getConditionIcon(condition), vehicle_util.getConditionText(condition)}`
- `MaintenanceLevels : {string}`: Declared, but not assigned by the base-game implementation (nil at runtime).
- `getNoise01 : function(emissions : number, carrier : Carrier) : number`  (call as `vehicle_util.getNoise01(…)`): Maps a noise emission value to 0 to 1, by carrier (0 to 200 for rail, 0 to 300 for air, 0 to 100 otherwise).
- `getPollution01 : function(emissions : number, carrier : Carrier) : number`  (call as `vehicle_util.getPollution01(…)`): Maps a pollution emission value to 0 to 1, by carrier (0 to 200 for rail, 0 to 300 for air, 0 to 100 otherwise).
- `isAbleToLoadCargo : function(tv : Engine.Component.TransportVehicle) : boolean`  (call as `vehicle_util.isAbleToLoadCargo(…)`): True if the transport vehicle has capacity for any cargo other than passengers.
- `isAbleToLoadPassenger : function(tv : Engine.Component.TransportVehicle) : boolean`  (call as `vehicle_util.isAbleToLoadPassenger(…)`): True if the transport vehicle has passenger capacity (any load config).
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:4461`: `if vehicle_util.isAbleToLoadPassenger(tv) then`
- `getRanges : function(carrier : Carrier, onlyPassengers : boolean, year : integer) : Ranges`  (call as `vehicle_util.getRanges(…)`): Value ranges for a carrier and game year.
    - param `carrier`: Vehicle carrier.
    - param `onlyPassengers`: True for vehicles that carry only passengers.
    - param `year`: Current game year.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/subvention_util.tl:487`: `local maxSpeed = vehicle_util.getRanges(api.type["enum"].Carrier.RAIL, true, api.engine.util.getYear()).maxSpeed`
- `getAvgMaintenanceState : function(Engine.Entity) : number, number`  (call as `vehicle_util.getAvgMaintenanceState(…)`): Average maintenance state of all parts of a vehicle.
    - returns: Average state (1.0 if the vehicle has no parts) and average maintenance change.
    - used 11x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:1990`: `local state, change = vehicle_util.getAvgMaintenanceState(params.entityId)`
- `getAge : function(Engine.Entity) : AgeInfo`  (call as `vehicle_util.getAge(…)`): Age information of a vehicle, based on its oldest part and the current game time.
    - used 2x in the base game, e.g. `base/content/gui/gui/statistics/statistic_vehicles.tl:237`: `return vehicle_util.getAge(entity)`
- `GetVehicleModelIds : function(Engine.Entity) : {integer}`  (call as `vehicle_util.GetVehicleModelIds(…)`): Model ids of all parts of a vehicle, in consist order.
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_list_react_util.tl:560`: `return vehicle_util.GetVehicleModelIds(rowKey)`
- `getPowerRatingTextAndToolTip : function(weight : number, power : number, tractiveEffort : number, topSpeed : number, rollingFriction : number) : {string, string, number}`  (call as `vehicle_util.getPowerRatingTextAndToolTip(…)`): Power rating of a land vehicle (Poor, Mediocre, Good, Excellent) with a tooltip listing the top speed reachable on flat, medium and high slopes.
  In game: "Top Speed for Slopes:", "{name}: Not Enough Power" (German: „Höchstgeschwindigkeit in Steigungen:“, „{name}: nicht genug Leistung“)
    - param `weight`: Vehicle weight.
    - param `power`: Engine power.
    - param `tractiveEffort`: Tractive effort.
    - param `topSpeed`: Top speed.
    - param `rollingFriction`: Rolling friction.
    - returns: `{ratingText, tooltip, rating01}`, or nil if the acceleration integration fails.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:3737`: `local textAndTooltipAndValue = vehicle_util.getPowerRatingTextAndToolTip(`
- `isCurrentlyAbleToHoldPassengers : function(vehicleEntity : Engine.Entity) : boolean`  (call as `vehicle_util.isCurrentlyAbleToHoldPassengers(…)`): True if the vehicle's current configuration has passenger capacity.
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_passenger_flow.tl:52`: `if not vehicleUtil.isCurrentlyAbleToHoldPassengers(params.entity) then`

### VehicleUtil.AgeInfo (record)

```teal
record AgeInfo
```

Age information of a vehicle, from `getAge`.

- `age : string`: Formatted age since the oldest part was bought.
- `purchaseTime : integer`: Game time at which the oldest part was bought.
- `agePercent : number`: Age as rounded percentage of the lifespan (may exceed 100).
- `timeRemaining : string | nil`: Formatted time until the end of the lifespan, or nil if it is over.

### VehicleUtil.VehicleType (enum)

```teal
enum VehicleType
```

Coarse vehicle type, derived from transport modes or carrier.

- `"Train"`: Train (also the fallback).
- `"Tram"`: Tram.
- `"Bus"`: Bus.
- `"Truck"`: Truck.
- `"Ship"`: Ship (large or small).
- `"Airplane"`: Aircraft (large or small).
- `"Helicopter"`: Helicopter.

### VehicleUtil.Ranges (record)

```teal
record Ranges
```

Value ranges used by the vehicle store and vehicle window to scale property bars, from `getRanges`.

Notes:

The base game keeps a small table per carrier with entries for certain years
(e.g. rail 1900, 1921, 1941, 1981); the last entry whose year is reached wins.

- `year : integer`: First year the entry applies to.
- `onlyPassengers : boolean | nil`: If set, the entry only applies to passenger-only (true) or other (false) vehicles.
- `minSpeed : number`: Lower end of the speed bar, in m/s.
- `maxSpeed : number`: Upper end of the speed bar, in m/s (default 200 km/h).
- `maxPower : number`: Upper end of the power bar (default 15000).
- `maxThrust : number`: Upper end of the thrust bar (default 500000).
- `maxRunningCosts : number`: Upper end of the running-cost bar (1000000).
- `pollutionScoreRange : {number, number}`: `{min, max}` of the pollution bar.
- `noiseScoreRange : {number, number}`: `{min, max}` of the noise bar.
- `comfortScoreRange : {number, number}`: `{min, max}` of the comfort bar.
- `loadSpeedScoreRange : {number, number}`: `{min, max}` of the loading-speed bar.

### VehicleUtil.Emissions (record)

```teal
record Emissions
```

Declared record; not used by base-game code.

- `score : integer`
- `currentBase : integer`
- `currentExtra : integer`
- `conditionIndex : integer`
