# api.engine.system

Definition file: `api/tealdef/api/engine/system.d.tl` (in the game folder).

Contains definitions for of api.engine.system

## ISystem (interface)

```teal
global interface ISystem
```

Common interface of engine systems that can loop over their entities with `forEach`; `AircraftMoveSystem` declares that it implements it.

Notes:

Several other systems (for example `CarriageListSystem`, `LandVehicleMoveSystem`, `ShipMoveSystem`) declare a
`forEach` member with the same signature without naming the interface. The base game scripts do not call
`forEach` on any system.

- `forEach : function(Engine.Entity)`: Loops over the entities managed by the system (on `AircraftMoveSystem`, all aircraft).
    - notes: The definition types the parameter as `Engine.Entity`, not as a callback function; no base-game script calls it, so how the loop body is passed cannot be confirmed from the game files.

## LineSystem (record)

```teal
global record LineSystem
```
Access: `api.engine.system.lineSystem`

Line System.
Contains cached information about lines.

- `getLines : function() : {Engine.Entity}`  (call as `api.engine.system.lineSystem.getLines(…)`): Gets all lines on the map.
    - returns: line entities
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:750`: `local lines = api.engine.system.lineSystem.getLines()`
- `getLinesForPlayer : function(playerEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.lineSystem.getLinesForPlayer(…)`): Gets all lines belonging to the player.
    - param `playerEntity`: the player for which the lines should be fetched
    - returns: line entities
    - used 16x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:842`: `local lines = api.engine.system.lineSystem.getLinesForPlayer(api.engine.util.getPlayer())`
- `getLinesForWaypoint : function(waypointEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.lineSystem.getLinesForWaypoint(…)`): Gets all lines going through a waypoint.
    - param `waypointEntity`: the waypoint for which the lines should be fetched
    - returns: line entities
- `getLinesForWaypointLane : function(waypointEntity : Engine.Entity, signalIndex : integer) : {{Engine.Entity, integer, integer}}`  (call as `api.engine.system.lineSystem.getLinesForWaypointLane(…)`): Gets all line waypoints at the specific signal+lane
    - param `waypointEntity`: the signalList entity
    - param `signalIndex`: the signal index inside the SignalList
- `getLineStops : function(stationGroupEntity : Engine.Entity) : {{Engine.Entity, integer}}`  (call as `api.engine.system.lineSystem.getLineStops(…)`): Gets all lines stopping at a station group.
    - param `stationGroupEntity`: the stationGroup for which the lines should be fetched
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:371`: `local line2stop = api.engine.system.lineSystem.getLineStops(stationGroupEntity)`
- `getLineStopsForStation : function(stationEntity : Engine.Entity) : {{Engine.Entity, integer}}`  (call as `api.engine.system.lineSystem.getLineStopsForStation(…)`): Gets all lines stopping at a station.
    - param `stationEntity`: the station for which the lines should be fetched
- `getLineStopsForTerminal : function(stationEntity : Engine.Entity, terminal : integer) : {{Engine.Entity, integer}}`  (call as `api.engine.system.lineSystem.getLineStopsForTerminal(…)`): Gets all lines stopping at a terminal.
    - param `stationEntity`: the station for which the lines should be fetched
    - param `terminal`: the terminal of the station for which the lines should be fetched
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:634`: `local lines = api.engine.system.lineSystem.getLineStopsForTerminal(stationEntity, terminalIndex - 1)`
- `getLinesForStationGroup : function(stationGroup : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.lineSystem.getLinesForStationGroup(…)`): Gets all lines for the given station group.
    - param `stationGroup`: the stationGroup for which the lines should be fetched
    - returns: line entities
    - used 10x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:475`: `local lines = api.engine.system.lineSystem.getLinesForStationGroup(entity)`
- `getProblemLines : function(playerEntity : Engine.Entity) : {{Engine.Entity, LineProblem}}`  (call as `api.engine.system.lineSystem.getProblemLines(…)`): Gets all lines having an issue, and the issue in question.
    - param `playerEntity`: the player for which the lines should be fetched
- `getStationGroup2LineStopsMap : function() : {Engine.Entity : {{Engine.Entity, integer}}}`  (call as `api.engine.system.lineSystem.getStationGroup2LineStopsMap(…)`): Gets all stops in a station group.
- `getTerminal2lineStops : function() : {{Engine.Entity, integer} : {{Engine.Entity, integer}}}`  (call as `api.engine.system.lineSystem.getTerminal2lineStops(…)`): Gets all stops in a terminal.
- `getBestLineAssignment : function(lineEntity : Engine.Entity, lineData : Engine.Component.Line, allowTerminalChanges : boolean) : {StationTerminal}`  (call as `api.engine.system.lineSystem.getBestLineAssignment(…)`): Gets best terminal assignments for line.
    - param `lineEntity`: the line for which the computation should happen
    - param `lineData`: the line component data, stops may have station = -1 and terminal = -1
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:224`: `local bestAssignments = api.engine.system.lineSystem.getBestLineAssignment(entity == nil and -1 or entity, newComp, true)`

## StationSystem (record)

```teal
global record StationSystem
```
Access: `api.engine.system.stationSystem`

Station System.
Contains cached information about stations.

- `getPersonNodeId2StationTerminalsMap : function() : {NodeId : {{Engine.Entity, integer}}}`  (call as `api.engine.system.stationSystem.getPersonNodeId2StationTerminalsMap(…)`): Gets all transport network nodes associated with a station terminal.
- `getStation2TownMap : function() : {Engine.Entity : Engine.Entity}`  (call as `api.engine.system.stationSystem.getStation2TownMap(…)`): Gets a map from stations to towns.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/auto_rename/scheme_components/scheme_component_town.script.tl:3`: `local station2townMap = api.engine.system.stationSystem.getStation2TownMap()`
- `getStations : function(townEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.stationSystem.getStations(…)`): Gets all stations in a town.
    - param `townEntity`: the entity of the town that is queried for
- `getStationTerminalForVehicleNode : function(nodeId : NodeId) : {Engine.Entity, integer}`  (call as `api.engine.system.stationSystem.getStationTerminalForVehicleNode(…)`): Gets the station terminal to which the node belongs.
    - param `nodeId`: the vehicle node id that is queried for
- `getStationTerminalsForPersonEdge : function(edgeId : EdgeId) : {{Engine.Entity, integer}}`  (call as `api.engine.system.stationSystem.getStationTerminalsForPersonEdge(…)`): Gets the station terminals to which the person edge belongs.
    - param `nodeId`: the edge id that is queried for
- `getStationTerminalsForPersonNode : function(nodeId : NodeId) : {{Engine.Entity, integer}}`  (call as `api.engine.system.stationSystem.getStationTerminalsForPersonNode(…)`): Gets the station terminals to which the person node belongs.
    - param `nodeId`: the person node id that is queried for
- `getTown2StationsMap : function() : {Engine.Entity : {Engine.Entity}}`  (call as `api.engine.system.stationSystem.getTown2StationsMap(…)`): Gets all station grouped by towns.
- `getTown : function(stationEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.stationSystem.getTown(…)`): Gets a the town of the station.
    - param `stationEntity`: the entity of the station that is queried for
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:242`: `local townEntity = api.engine.system.stationSystem.getTown(stationEntity)`
- `getVehicleNodeId2StationTerminalsMap : function() : {NodeId : {{Engine.Entity, integer}}}`  (call as `api.engine.system.stationSystem.getVehicleNodeId2StationTerminalsMap(…)`): Gets the mapping from vehicle node to station terminal.
- `forEach : function(Engine.Entity)`  (call as `api.engine.system.stationSystem.forEach(…)`): Loops over all stations.

## StationGroupSystem (record)

```teal
global record StationGroupSystem
```
Access: `api.engine.system.stationGroupSystem`

Station Group System.
Contains cached information about station groups.

- `getExpiredStationGroups : function(time : integer) : {Engine.Entity}`  (call as `api.engine.system.stationGroupSystem.getExpiredStationGroups(…)`): Gets station groups that no longer contain stations and are thus about to be removed.
    - param `time`: the time stamp until when the stations with this condition are removed
- `getExpiryProgress : function(stationGroupEntity : Engine.Entity, time : integer) : number`  (call as `api.engine.system.stationGroupSystem.getExpiryProgress(…)`): Gets progress on the removal of station groups.
    - param `stationGroupEntity`: the entity of the station group that is about to be removed
    - param `time`: the time for which th progress is requested
- `getStationGroup : function(stationEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.stationGroupSystem.getStationGroup(…)`): Obtains the station group to which the station belongs.
    - param `stationEntity`: the entity of the station for which the parent station group is queried
    - used 21x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:312`: `local stationGroupEntity = api.engine.system.stationGroupSystem.getStationGroup(stationEntity)`
- `getCarriers : function(stationGroup : Engine.Entity, station : integer, terminal : integer) : {{Carrier}, {TransportMode}}`  (call as `api.engine.system.stationGroupSystem.getCarriers(…)`): Gets the carriers of the station / terminal.
    - param `stationGroup`: entity
    - param `station`: The station index (or -1 to get superset over all terminals)
    - param `terminal`: The terminal index inside the station (or -1 if station == -1)
    - used 11x in the base game, e.g. `base/content/gui/gui/construction/constructionHudIconMaster.tl:82`: `carriers = api.engine.system.stationGroupSystem.getCarriers(param.entity, -1, -1)[1]`

## TownBuildingSystem (record)

```teal
global record TownBuildingSystem
```
Access: `api.engine.system.townBuildingSystem`

Town Building System.
Manages town buildings and parcels.

- `getBuilding : function(parcelEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.townBuildingSystem.getBuilding(…)`): Obtains the building (if any) that is placed on the given parcel.
    - param `parcelEntity`: the entity of the parcel
- `getCargoSupplyAndLimit : function(townEntity : Engine.Entity, landUse? : LandUseType) : {CargoTypeId : {number, number, integer}}`  (call as `api.engine.system.townBuildingSystem.getCargoSupplyAndLimit(…)`): Obtains information about towns and cargo.
    - param `townEntity`: the entity of the town
    - used 12x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo_town/deliver_cargo_town.script.tl:64`: `local limit = api.engine.system.townBuildingSystem.getCargoSupplyAndLimit(townEntity)`
- `getLandUsePersonCapacities : function(townEntity : Engine.Entity) : {integer}`  (call as `api.engine.system.townBuildingSystem.getLandUsePersonCapacities(…)`): Obtains information about land use and person capacities.
    - param `townEntity`: the entity of the town
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:117`: `local sumPersonCapacity = api.engine.system.townBuildingSystem.getLandUsePersonCapacities(townEntity)[LAND_USE_TYPE_RESIDENTIAL]`
- `getParcel2BuildingMap : function() : {Engine.Entity : Engine.Entity}`  (call as `api.engine.system.townBuildingSystem.getParcel2BuildingMap(…)`): Obtains map from parcel to building occupying the parcel.
- `getPersonCapacity2townBuildingMap : function() : {Engine.Entity : Engine.Entity}`  (call as `api.engine.system.townBuildingSystem.getPersonCapacity2townBuildingMap(…)`): Obtains map from person capacities to town buildings.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:439`: `local personCapacityToTownBuildingMap : {Engine.Entity : Engine.Entity} = api.engine.system.townBuildingSystem.getPersonCapacity2townBuildingMap()`
- `getTown2BuildingMap : function() : {Engine.Entity : {Engine.Entity}}`  (call as `api.engine.system.townBuildingSystem.getTown2BuildingMap(…)`): Obtains map from town to town buildings of the town.
    - used 4x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:510`: `local map = api.engine.system.townBuildingSystem.getTown2BuildingMap()`
- `getTown2personCapacitiesMap : function() : {Engine.Entity : {integer}}`  (call as `api.engine.system.townBuildingSystem.getTown2personCapacitiesMap(…)`): Obtains map from town to person capacities of the town.
    - returns: a map of town to an array of integer. {integer} is an array of 3 ints capa per landuse: { RESIDENTIAL, COMMERCIAL, INDUSTRIAL }
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_util.tl:367`: `for __, personCapacities in pairs(api.engine.system.townBuildingSystem.getTown2personCapacitiesMap()) do`
- `hasBuilding : function(parcelEntity : Engine.Entity) : boolean`  (call as `api.engine.system.townBuildingSystem.hasBuilding(…)`): Checks if parcel is occupied by building.
    - param `parcelEntity`: the entity of the parcel

## SimPersonSystem (record)

```teal
global record SimPersonSystem
```
Access: `api.engine.system.simPersonSystem`

Sim Person System.
System managing persons.

- `getCount : function() : integer`  (call as `api.engine.system.simPersonSystem.getCount(…)`): Gets number of people on the map.
    - returns: number of people
- `getDestination2SpMap : function() : {Engine.Entity : {Engine.Entity}}`  (call as `api.engine.system.simPersonSystem.getDestination2SpMap(…)`): Gets list of people for every destination.
- `getSimPersonsAtTerminalForTransportNetwork : function(tnEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.simPersonSystem.getSimPersonsAtTerminalForTransportNetwork(…)`): Gets all persons waiting at the given transport network.
    - param `tnEntity`: the entity of the transport network
- `getSimPersonsForDestination : function(destination : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.simPersonSystem.getSimPersonsForDestination(…)`): Gets all persons that have an entity as one of their destination.
Destinations are a home, a work and a shopping destination.
    - param `destination`: the destination entity
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:1238`: `local numOccupied = #api.engine.system.simPersonSystem.getSimPersonsForDestination(params.entityId)`
- `getSimPersonsForLine : function(lineEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.simPersonSystem.getSimPersonsForLine(…)`): Gets all persons travelling with the given line.
    - param `lineEntity`: the entity of the line
- `getSimPersonsForTarget : function(targetEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.simPersonSystem.getSimPersonsForTarget(…)`): Gets all persons currently trying to reach given target.
    - param `targetEntity`: the entity of the target
- `getSimPersonsIdleForTransportNetwork : function(tnEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.simPersonSystem.getSimPersonsIdleForTransportNetwork(…)`): Gets all idle persons on a transport network.
    - param `tnEntity`: the entity of the transport network
- `getSimPersonsMovingForTransportNetwork : function(tnEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.simPersonSystem.getSimPersonsMovingForTransportNetwork(…)`): Gets all moving persons over a transport network.
    - param `tnEntity`: the entity of the transport network

## SimEntityAtStockSystem (record)

```teal
global record SimEntityAtStockSystem
```
Access: `api.engine.system.simEntityAtStockSystem`

Sim Entity At Stock System.
System managing cargo produced by industries or transported to a warehouse.

- `getStock2SimEntityMap : function() : {{Engine.Entity, StockId} : {Engine.Entity}}`  (call as `api.engine.system.simEntityAtStockSystem.getStock2SimEntityMap(…)`): Obtains a map, mapping the stock to the cargo waiting there.
- `getStockCount : function(stockEntity : Engine.Entity, stockId : StockId) : integer`  (call as `api.engine.system.simEntityAtStockSystem.getStockCount(…)`): Obtains the amount of specific item id waiting at a given stock.
    - param `stockEntity`: the entity of the stock holding entity, e.g. industry
    - param `stockId`: the id of the stock
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:175`: `usage = usage + api.engine.system.simEntityAtStockSystem.getStockCount(entity, i-1)`
- `getStockCountForCargoType : function(stockEntity : Engine.Entity, stockId : StockId, cargoTypeId : CargoTypeId) : integer`  (call as `api.engine.system.simEntityAtStockSystem.getStockCountForCargoType(…)`): Obtains the amount of specific item id waiting at a given stock and cargo type.
    - param `stockEntity`: the entity of the stock holding entity, e.g. industry
    - param `stockId`: the id of the stock
    - param `cargoTypeId`: the id of the cargo type
    - used 4x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:222`: `api.engine.system.simEntityAtStockSystem.getStockCountForCargoType(entity, stockId, cargoTypeId) as integer`
- `getStockEntities : function(stockEntity : Engine.Entity, stockId : StockId) : {{integer, Engine.Entity}}`  (call as `api.engine.system.simEntityAtStockSystem.getStockEntities(…)`): Obtains all entities waiting at a given stock.
    - param `stockEntity`: the entity of the stock holding entity, e.g. industry
    - param `stockId`: the id of the stock
- `getStockSimEntity : function(stockEntity : Engine.Entity, stockId : StockId) : Engine.Entity`  (call as `api.engine.system.simEntityAtStockSystem.getStockSimEntity(…)`): Obtains the entity that was waiting longest at a given stock.
    - param `stockEntity`: the entity of the stock holding entity, e.g. industry
    - param `stockId`: the id of the stock

## SimEntityAtTerminalSystem (record)

```teal
global record SimEntityAtTerminalSystem
```
Access: `api.engine.system.simEntityAtTerminalSystem`

Sim Entity At Terminal System.
System managing entities waiting at terminals.

- `getLineStopSimEntitiesCount : function(entity : Engine.Entity, stopIndex : integer, cargoType : CargoTypeId) : integer`  (call as `api.engine.system.simEntityAtTerminalSystem.getLineStopSimEntitiesCount(…)`): Gets the count of entities (SimPersons) waiting at the terminal.
    - param `entity`: the line entity
    - param `stopIndex`: the index of the stop in the line
    - param `cargoType`: the id of the cargo type
    - returns: the count of entities at that line stop
- `getLineStopSimEntities : function(entity : Engine.Entity, stopIndex : integer, cargoType : CargoTypeId) : {Engine.Entity}`  (call as `api.engine.system.simEntityAtTerminalSystem.getLineStopSimEntities(…)`): Gets the count of entities (SimPersons) waiting at the terminal for the given cargo type.
    - param `entity`: the line entity
    - param `stopIndex`: the index of the stop in the line
    - param `cargoType`: the id of the cargo type
    - returns: the sim entities of the given cargo type at that line stop
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:664`: `local passengers = api.engine.system.simEntityAtTerminalSystem.getLineStopSimEntities(line, stopIndex, passengerCargoTypeId)`

## SimEntityAtVehicleSystem (record)

```teal
global record SimEntityAtVehicleSystem
```
Access: `api.engine.system.simEntityAtVehicleSystem`

Sim Entity At Vehicle System.
System managing entities travelling in a vehicle.

- `getVehicle2Cargo2SimEntitesMap : function() : {Engine.Entity : {CargoTypeId : {Engine.Entity}}}`  (call as `api.engine.system.simEntityAtVehicleSystem.getVehicle2Cargo2SimEntitesMap(…)`): Gets a map from vehicle Entity to a map from cargo to all "Sim" entities in the vehicle of the given cargo type.
    - used 2x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:562`: `local cargo2SimEntitesMap = api.engine.system.simEntityAtVehicleSystem.getVehicle2Cargo2SimEntitesMap()[e]`
- `getVehicleSimEntitiesCount : function(vehicleEntity : Engine.Entity) : integer`  (call as `api.engine.system.simEntityAtVehicleSystem.getVehicleSimEntitiesCount(…)`): Gets the count of entities in the vehicle.
    - param `vehicleEntity`: the entity of the vehicle that should be inspected
    - returns: the count of entities
    - used 1x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:604`: `local cargo2count = api.engine.system.simEntityAtVehicleSystem.getVehicleSimEntitiesCount(e)`
- `getVehicleSimEntitiesCountForCargoType : function(vehicleEntity : Engine.Entity, cargoType : CargoTypeId) : integer`  (call as `api.engine.system.simEntityAtVehicleSystem.getVehicleSimEntitiesCountForCargoType(…)`): Gets the count of entities in the vehicle for the given cargo type.
    - param `vehicleEntity`: the entity of the vehicle that should be inspected
    - param `cargoType`: the id of the cargo type
    - returns: the count of entities
    - used 7x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:297`: `fill = calculateFill and api.engine.system.simEntityAtVehicleSystem.getVehicleSimEntitiesCountForCargoType(vehicleEntity, cargoType - 1) or nil,`
- `getVehiclesWithRoundTripCount : function() : {Engine.Entity : integer}`  (call as `api.engine.system.simEntityAtVehicleSystem.getVehiclesWithRoundTripCount(…)`): Gets the vehicles with their round trip cargo counts.
    - returns: the vehicles with their round trip cargo counts
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:278`: `local vehiclesWithRoundTrips = api.engine.system.simEntityAtVehicleSystem.getVehiclesWithRoundTripCount()`
- `hasAnythingLoaded : function(Engine.Entity) : boolean`  (call as `api.engine.system.simEntityAtVehicleSystem.hasAnythingLoaded(…)`): Check if the vehicle is not empty
    - returns: if the vehicle has loaded at least 1 passenger or cargo item
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:856`: `return api.engine.system.simEntityAtVehicleSystem.hasAnythingLoaded(param.vehicleEntity)`

## SimPersonAtVehicleSystem (record)

```teal
global record SimPersonAtVehicleSystem
```
Access: `api.engine.system.simPersonAtVehicleSystem`

Sim Person At Vehicle System.
System managing persons travelling on a vehicle.

- `getVehiclePartInfoList : function(vehicleEntity : Engine.Entity) : {VehiclePartInfo}`  (call as `api.engine.system.simPersonAtVehicleSystem.getVehiclePartInfoList(…)`): Gets information about persons traveling on the given vehicle.
    - param `vehicleEntity`: the entity of the vehicle that should be inspected
    - returns: Information about each vehicle part

## StreetConnectorSystem (record)

```teal
global record StreetConnectorSystem
```
Access: `api.engine.system.streetConnectorSystem`

Street Connector System.
Manages street connections.

- `getConstructionEntityForDepot : function(depotEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.streetConnectorSystem.getConstructionEntityForDepot(…)`): Gets all sources for the destination entity.
    - param `depotEntity`: the entity for which the containing construction should be fetched
- `getConstructionEntityForEdge : function(edgeEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.streetConnectorSystem.getConstructionEntityForEdge(…)`): Gets the construction that "owns" the given edge.
    - param `edgeEntity`: the entity for which the containing construction should be fetched
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/view_manager_util.tl:249`: `local edgeConEntity = api.engine.system.streetConnectorSystem.getConstructionEntityForEdge(edge.edge)`
- `getConstructionEntityForIndustry : function(industryEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.streetConnectorSystem.getConstructionEntityForIndustry(…)`): Gets the construction for the industry.
    - param `industryEntity`: the entity for which the containing construction should be fetched
- `getConstructionEntityForNode : function(nodeEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.streetConnectorSystem.getConstructionEntityForNode(…)`): Gets the construction to which the node (a frozen node in the construction) belongs.
    - param `nodeEntity`: the entity for which the containing construction should be fetched
- `getConstructionEntityForSubconstruction : function(subconstruction : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.streetConnectorSystem.getConstructionEntityForSubconstruction(…)`): Gets the construction for the subconstruction.
    - param `subconstruction`: the entity for which the containing construction should be fetched
    - used 53x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:436`: `local constructionEntity = api.engine.system.streetConnectorSystem.getConstructionEntityForSubconstruction(stationEntity)`
- `getConstructionEntityForStation : function(stationEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.streetConnectorSystem.getConstructionEntityForStation(…)`): Gets the construction for the station.
    - param `stationEntity`: the entity for which the containing construction should be fetched
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:631`: `local construction = api.engine.system.streetConnectorSystem.getConstructionEntityForStation(station)`
- `getConstructionEntityForTownBuilding : function(townBuilding : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.streetConnectorSystem.getConstructionEntityForTownBuilding(…)`): Gets the construction for the town building.
    - param `townBuilding`: the entity for which the containing construction should be fetched
    - used 1x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:513`: `local subConEntity = api.engine.system.streetConnectorSystem.getConstructionEntityForTownBuilding(e)`
- `getNode2StreetConnectorMap : function() : {Engine.Entity : Engine.Entity}`  (call as `api.engine.system.streetConnectorSystem.getNode2StreetConnectorMap(…)`): Gets a mapping from frozen nodes to the construction they belong.
- `getStation2ConstructionMap : function() : {Engine.Entity : Engine.Entity}`  (call as `api.engine.system.streetConnectorSystem.getStation2ConstructionMap(…)`): Gets a mapping from stations to constructions.
- `forEachConstructionWithMetadata : function(metadataKey : string, constructionDescMeta : boolean, constructionInstanceMeta : boolean, fn : function(entity : Engine.Entity, construction : Engine.Component.Construction, constructionIdIfDescMatch : ConstructionId), playerOwnedOnly? : boolean)`  (call as `api.engine.system.streetConnectorSystem.forEachConstructionWithMetadata(…)`): Loop over all constructions that have the specified metadata key in their static description.
    - param `metadataKey`: metadata key
    - param `constructionDescMeta`: if static metadata should be checked (on match, the callback's constructionId is >= 0)
    - param `constructionInstanceMeta`: if instance metadata should be checked
    - param `fn`: the callback called for each entity with the given component and its entry in constructionRep
    - param `playerOwnedOnly`: if true, then skip constructions owned by other player or not owned at all
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:204`: `api.engine.system.streetConnectorSystem.forEachConstructionWithMetadata(company_metadata.getKey(), false, true, checkFn)`
- `getConstructionIdIfStaticMetadata : function(constructionEntity : Engine.Entity, metadataKey : string) : {ConstructionId, ResName}`  (call as `api.engine.system.streetConnectorSystem.getConstructionIdIfStaticMetadata(…)`): Loop over all constructions that have the specified metadata key in their static description.
    - param `constructionEntity`: the entity to check
    - param `metadataKey`: metadata key
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_util.tl:324`: `local apiResult = constructionEntity and api.engine.system.streetConnectorSystem.getConstructionIdIfStaticMetadata(constructionEntity, company_metadata.getKey()`
- `getConstructionClosestTown : function(constructionEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.streetConnectorSystem.getConstructionClosestTown(…)`): Gets the closest town to the construction.
    - param `constructionEntity`: the entity for which the closest town should be fetched
    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:386`: `local townEntity = api.engine.system.streetConnectorSystem.getConstructionClosestTown(landmarkEntity)`
- `getTownClosestConstructions : function(townEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.streetConnectorSystem.getTownClosestConstructions(…)`): Gets constructions for which the town is the closest.
    - param `townEntity`: the entity for which the closest constructions should be fetched
- `forEach : function(Engine.Entity)`  (call as `api.engine.system.streetConnectorSystem.forEach(…)`): Loops over all constructions.

## BaseParallelStripSystem (record)

```teal
global record BaseParallelStripSystem
```
Access: `api.engine.system.baseParallelStripSystem`

Base Parallel Strip System.
Contain information about parallel edge ranges.

- `getStrips : function(edgeEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.baseParallelStripSystem.getStrips(…)`): Obtains all parallel strips of a base edge entity (there might be more than one parallel strip for each base edge).
    - param `edgeEntity`: the entity of the edge that should be inspected

## StreetSystem (record)

```teal
global record StreetSystem
```
Access: `api.engine.system.streetSystem`

Street System.
Manages base edges.

- `getEdgeForEdgeObject : function(edgeEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.streetSystem.getEdgeForEdgeObject(…)`): Obtains the edge to which the edge object belongs.
    - param `edgeEntity`: the entity of the edge object
- `getEdgeObject2EdgeMap : function() : {Engine.Entity : Engine.Entity}`  (call as `api.engine.system.streetSystem.getEdgeObject2EdgeMap(…)`): Obtains the mapping from edge object to edges they belong.
- `getNode2SegmentMap : function() : {Engine.Entity : {Engine.Entity}}`  (call as `api.engine.system.streetSystem.getNode2SegmentMap(…)`): Obtains a map from all base nodes to base segments they are connected with.
- `getNode2StreetEdgeMap : function() : {Engine.Entity : {Engine.Entity}}`  (call as `api.engine.system.streetSystem.getNode2StreetEdgeMap(…)`): Obtains a map from all base nodes to base street segments they are connected with.
- `getNode2TrackEdgeMap : function() : {Engine.Entity : {Engine.Entity}}`  (call as `api.engine.system.streetSystem.getNode2TrackEdgeMap(…)`): Obtains a map from all base nodes to base track segments they are connected with.
- `isTownCrossingNode : function(Engine.Entity) : boolean`  (call as `api.engine.system.streetSystem.isTownCrossingNode(…)`): Checks if the node is a town crossing.
- `getNodeSegments : function(nodeEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.streetSystem.getNodeSegments(…)`): Returns the node segments.
    - param `nodeEntity`: the entity of the node
- `getNodeStreetSegments : function(nodeEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.streetSystem.getNodeStreetSegments(…)`): Returns the node street segments.
    - param `nodeEntity`: the entity of the node
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/view_manager_util.tl:35`: `local streetEdges = api.engine.system.streetSystem.getNodeStreetSegments(nodeEntity)`
- `getNodeTrackSegments : function(nodeEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.streetSystem.getNodeTrackSegments(…)`): Returns the node track segments.
    - param `nodeEntity`: the entity of the node
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/view_manager_util.tl:8`: `local trackEdges = api.engine.system.streetSystem.getNodeTrackSegments(nodeEntity)`

## GameScriptSystem (record)

```teal
global record GameScriptSystem
```
Access: `api.engine.system.gameScriptSystem`

Game Script System
Manages entities and components for gamescript.

- `getEntityForGameScript : function(name : string) : Engine.Entity`  (call as `api.engine.system.gameScriptSystem.getEntityForGameScript(…)`): Gets the entity onto which the gamescript state is attached
    - param `name`: the name of the gamescript

    ```lua
    local entity = api.engine.system.gameScriptSystem.getEntityForGameScript("::/game_mechanics/game_time/game_time.gs")
    local gameScript = api.engine.getComponent(entity, api.type.ComponentType.GAME_SCRIPT)
    ```

    - used 43x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:485`: `local emissionsScriptEntity = api.engine.system.gameScriptSystem.getEntityForGameScript("::/game_mechanics/emission/emissions.gs")`

## CarriageListSystem (record)

```teal
global record CarriageListSystem
```
Access: `api.engine.system.carriageListSystem`

Carriage list System.
Manages vehicles that are groups of multiple carriages.

- `getVehicleInfo : function(vehicleEntity : Engine.Entity) : CarriageInfo`  (call as `api.engine.system.carriageListSystem.getVehicleInfo(…)`): Gets more information about a train carriage/engine.
    - param `vehicleEntity`: the entity of the vehicle.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1770`: `local owningVehicleInfo = api.engine.system.carriageListSystem.getVehicleInfo(emitter)`
- `getVehicleData : function(vehicleEntity : Engine.Entity) : VehicleData`  (call as `api.engine.system.carriageListSystem.getVehicleData(…)`): Gets more information about a train carriage/engine.
    - param `vehicleEntity`: the entity of the vehicle.
- `forEach : function(Engine.Entity)`  (call as `api.engine.system.carriageListSystem.forEach(…)`): Loops over all carriage lists.

### CarriageListSystem.CarriageInfo (record)

```teal
record CarriageInfo
```

Carriage-list data of one carriage or engine, returned by `api.engine.system.carriageListSystem.getVehicleInfo(entity)`.

Notes:

The base game uses it to go from a picked carriage (an entity with a `CARRIAGE` component) to the vehicle
that owns it, which carries the `TRANSPORT_VEHICLE` component.

Example:
```lua
local carriage = api.engine.getComponent(entity, api.type.ComponentType.CARRIAGE)
if carriage then
  local info = api.engine.system.carriageListSystem.getVehicleInfo(entity)
  if info ~= nil and info.owningVehicle >= 0 then
    local vehicle = api.engine.getComponent(info.owningVehicle, api.type.ComponentType.TRANSPORT_VEHICLE)
  end
end
```

- `owningVehicle : Engine.Entity`: Entity of the vehicle (with the `TRANSPORT_VEHICLE` component) this carriage belongs to; base-game code checks it with `>= 0` before use.
- `index : integer`: Index of the carriage within its owning vehicle's carriage list.
- `offset : number`
- `reversed : boolean`
- `crew : boolean`

## TransportVehicleSystem (record)

```teal
global record TransportVehicleSystem
```
Access: `api.engine.system.transportVehicleSystem`

Transport Vehicle System.
Manages the movement and behaviour of transport vehicles. This covers all vehicles transporting cargo and passengers.

- `getDepotVehicles : function(depotEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.transportVehicleSystem.getDepotVehicles(…)`): Gets all vehicles that are in a depot.
    - param `depotEntity`: the entity of the depot
    - used 13x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_hud_util.tl:1271`: `local allDepotVehicles = api.engine.system.transportVehicleSystem.getDepotVehicles(hudIconParam.entity)`
- `getGoingToDepotVehicles : function(depotEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.transportVehicleSystem.getGoingToDepotVehicles(…)`): Gets all vehicles heading for the depot.
    - param `depotEntity`: the entity of the depot
- `getInfo : function(transportVehicleEntity : Engine.Entity) : any`  (call as `api.engine.system.transportVehicleSystem.getInfo(…)`): Gets additional information about the vehicle.
    - param `transportVehicleEntity`: the entity of the transport vehicle
    - returns: info about the transport vehicle
- `getLine2VehicleMap : function() : {Engine.Entity : {Engine.Entity}}`  (call as `api.engine.system.transportVehicleSystem.getLine2VehicleMap(…)`): Gets a map of lines to the vehicles using the line.
- `getLineStopVehicles : function(lineEntity : Engine.Entity, stopIndex : number) : {Engine.Entity}`  (call as `api.engine.system.transportVehicleSystem.getLineStopVehicles(…)`): Gets the vehicle stopped at the stop of a line.
    - param `lineEntity`: the entity of the line
    - param `stopIndex`: the index of the stop on the line
- `checkLineStopForVehicleOverlength : function(lineEntity : Engine.Entity, stopIndex : number) : {StationTerminal : boolean}`  (call as `api.engine.system.transportVehicleSystem.checkLineStopForVehicleOverlength(…)`): Checks whether the specified stop has terminals that are too short for optimal loading.
    - param `lineEntity`: the entity of the line
    - param `stopIndex`: the index of the stop on the line
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:712`: `local stationTerminal2overlength = api.engine.system.transportVehicleSystem.checkLineStopForVehicleOverlength(`
- `getLineVehicles : function(lineEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.transportVehicleSystem.getLineVehicles(…)`): Gets all vehicles using a given line.
    - param `lineEntity`: the entity of the line
    - used 31x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:757`: `local lineVehicles = api.engine.system.transportVehicleSystem.getLineVehicles(lineEntity);`
- `getNoPathVehicles : function() : {Engine.Entity}`  (call as `api.engine.system.transportVehicleSystem.getNoPathVehicles(…)`): Gets all vehicles that cannot find a path to their destination.
- `getVehicleNames : function(carrier : Carrier) : {string}`  (call as `api.engine.system.transportVehicleSystem.getVehicleNames(…)`): Gets the vehicle names of all vehicles for the given carrier.
    - param `carrier`: the carrier for which the names should be fetched
- `getVehicles : function(tnEntities : {Engine.Entity}, wholePath : boolean) : {Engine.Entity}`  (call as `api.engine.system.transportVehicleSystem.getVehicles(…)`): Gets the vehicle using a certain transport network.
    - param `tnEntities`: list of transport network entities
    - param `wholePath`: true if the vehicle has to use the whole path
- `getVehiclesWithState : function(state : TransportVehicleState) : {Engine.Entity}`  (call as `api.engine.system.transportVehicleSystem.getVehiclesWithState(…)`): Gets all vehicles that are in the given state. This can be used, for instance, to obtain a list of all vehicles
currently waiting at a terminal.
    - param `state`: A state to filter the vehicles
- `getVehiclesForMaintenanceStation : function(maintenanceStation : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.transportVehicleSystem.getVehiclesForMaintenanceStation(…)`): Gets all vehicles that are maintained by the given maintenance station.
    - param `maintenanceStation`: the entity of the maintenance station
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:227`: `local poolUsage = #api.engine.system.transportVehicleSystem.getVehiclesForMaintenanceStation(entity)`
- `isVehicleMaintainedByStation : function(vehicle : Engine.Entity, maintenanceStation : Engine.Entity) : boolean`  (call as `api.engine.system.transportVehicleSystem.isVehicleMaintainedByStation(…)`): Checks if the vehicle is maintained by the given maintenance station.
    - param `vehicle`: the entity of the vehicle
    - param `maintenanceStation`: the entity of the maintenance station
- `getLineCargoInfo : function(lineEntity : Engine.Entity, cargoTypeId : CargoTypeId) : LineCargoInfo`  (call as `api.engine.system.transportVehicleSystem.getLineCargoInfo(…)`): Gets cargo info for a line.
    - param `lineEntity`: the entity of the line
    - param `cargoTypeId`: the id of the cargo type

## LandVehicleMoveSystem (record)

```teal
global record LandVehicleMoveSystem
```
Access: `api.engine.system.landVehicleMoveSystem`

Land Vehicle Move System.
Manages the movement of land vehicles.

- `getBlockedTrains : function() : {{Engine.Entity, Engine.Entity}}`  (call as `api.engine.system.landVehicleMoveSystem.getBlockedTrains(…)`): Gets all trains that are blocked by another train.
- `getVehicleInfo : function(vehicleEntity : Engine.Entity) : LandVehicleMoveInfo`  (call as `api.engine.system.landVehicleMoveSystem.getVehicleInfo(…)`): Gets more information about a vehicle.
    - param `vehicleEntity`: the entity of the vehicle.
- `isTrainWaitingForFreePath : function(trainEntity : Engine.Entity) : boolean`  (call as `api.engine.system.landVehicleMoveSystem.isTrainWaitingForFreePath(…)`): Checks if a train is waiting for a free path.
    - param `trainEntity`: the entity of the train.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:2339`: `elseif api.engine.system.landVehicleMoveSystem.isTrainWaitingForFreePath(entityId) then`
- `forEach : function(Engine.Entity)`  (call as `api.engine.system.landVehicleMoveSystem.forEach(…)`): Loops over all trains.

## AircraftMoveSystem (record)

```teal
global record AircraftMoveSystem is ISystem
```
Access: `api.engine.system.aircraftMoveSystem`

Aircraft Move System.
System managing the movement of airplanes.

- `getAirCraftInfo : function(vehicleEntity : Engine.Entity) : AircraftInfo`  (call as `api.engine.system.aircraftMoveSystem.getAirCraftInfo(…)`): Obtains Aircraft information for the given entity.
    - param `vehicleEntity`: the entity id of the aircraft that should be inspected
- `isReserved : function(edgeId : EdgeId) : boolean`  (call as `api.engine.system.aircraftMoveSystem.isReserved(…)`): Checks if a given edge is reserved by an aircraft.
    - param `edgeId`: the edgeId that should be inspected
    - returns: true if reserved
- `forEach : function(Engine.Entity)`  (call as `api.engine.system.aircraftMoveSystem.forEach(…)`): Loops over all aircraft.

## GridCollisionSystem (record)

```teal
global record GridCollisionSystem
```

Grid Collision System.
Single instance of the grid collision system.

- `getCollisionDataAt : function(GridCollisionSystem, Vec2f) : number`  (call as `gridCollisionSystem:getCollisionDataAt(…) (method)`): Gets collision data at a given position (0 = no collision)
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo_town/deliver_cargo_town.script.tl:277`: `local baseWeight = (gics:getCollisionDataAt(api.type.Vec2f.new(position.x, position.y)) + 1) / #missingSupply`

## CatchmentAreaSystem (record)

```teal
global record CatchmentAreaSystem
```
Access: `api.engine.system.catchmentAreaSystem`

Catchment Area System.
Contains cached information about reachability of stations and destinations.

- `getEdge2stationsMap : function() : {EdgeId : {{Engine.Entity, integer}}}`  (call as `api.engine.system.catchmentAreaSystem.getEdge2stationsMap(…)`): Obtains a map mapping transport network edges to stations that can be reached from that edge.
- `getStation2edgesMap : function() : {Engine.Entity : {{EdgeId, integer}}}`  (call as `api.engine.system.catchmentAreaSystem.getStation2edgesMap(…)`): Obtains a map mapping stations to all reachable edges from that station (the actual catchment area of the station).
- `getStation2stationsAndDistancesMap : function() : {Engine.Entity : {Engine.Entity : integer}}`  (call as `api.engine.system.catchmentAreaSystem.getStation2stationsAndDistancesMap(…)`): Obtains a map, mapping station to all reachable stations in the catchment area, and, additionally, the distance of the
shortest path.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:574`: `local reachableStations = api.engine.system.catchmentAreaSystem.getStation2stationsAndDistancesMap()[station]`
- `getStationCatchables : function(stationEntity : Engine.Entity, cargo : boolean) : {Engine.Entity}`  (call as `api.engine.system.catchmentAreaSystem.getStationCatchables(…)`): Obtains a list of all catchables (entities holding a StockList or PersonCapacity component) for the station.
    - param `stationEntity`: the entity of the station
    - param `cargo`: wether to return cargo or passenger catchables
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:611`: `local catchables = api.engine.system.catchmentAreaSystem.getStationCatchables(stationEntity, true)`

## TransportNetworkSystem (record)

```teal
global record TransportNetworkSystem
```
Access: `api.engine.system.transportNetworkSystem`

Transport Network System.
Manages transport networks (and transport network edges and nodes).

- `getIntersections : function(tnEntity : Engine.Entity) : {any}`  (call as `api.engine.system.transportNetworkSystem.getIntersections(…)`): Gets all intersections within a transport network (e.g. all intersections in a base node).
    - param `tnEntity`: the entity of the transport network
- `getTpNetData : function() : TpNetData`  (call as `api.engine.system.transportNetworkSystem.getTpNetData(…)`): Gets a structure representing the graph of the transport network.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:1453`: `local tpNetData = api.engine.system.transportNetworkSystem.getTpNetData()`

## EmissionGridSystem (record)

```teal
global record EmissionGridSystem
```
Access: `api.engine.system.emissionGridSystem`

Emission grid system.
Manages emission grid data.

- `getInterpolatedEmission : function(pos : Vec2f, useAverage : boolean, type : Engine.Component.EmissionGrid.Type) : number`  (call as `api.engine.system.emissionGridSystem.getInterpolatedEmission(…)`): Returns the interpolated emission at a given position.
    - param `pos`: the position at which the emission should be sampled
    - param `useAverage`: whether to use average or not
    - param `type`: the type of emission to query
    - returns: the interpolated emission value
    - used 5x in the base game, e.g. `base/content/gui/gui/entity_window/town_building/town_building.tl:168`: `local emission = api.engine.system.emissionGridSystem.getInterpolatedEmission(api.type.Vec2f.new(pos.x, pos.y), average, api.type.EmissionGrid.Type.Noise)`
- `getEmissionGridEntity : function(noise : boolean) : Engine.Entity`  (call as `api.engine.system.emissionGridSystem.getEmissionGridEntity(…)`): Returns the EmissionGrid entity for the noise or pollution
    - param `noise`: Which type of emission: noise or pollution
    - returns: an entity with the EmissionGrid component
    - used 1x in the base game, e.g. `dlcs/urbangames_deluxe_upgrade_pack/content/fun_elements/fun_elements/balloon.script.tl:68`: `local windEntity = api.engine.system.emissionGridSystem.getEmissionGridEntity(false)`

## RailRoadCrossingSystem (record)

```teal
global record RailRoadCrossingSystem
```
Access: `api.engine.system.railRoadCrossingSystem`

Railroad Crossing System.
Contains information about raiload crossings.

- `getRailroadCrossingForEdge : function(edgeEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.railRoadCrossingSystem.getRailroadCrossingForEdge(…)`): Gets all railroad crossings of the given base edge entity.
    - param `edgeEntity`: the entity id of the edge that should be inspected
- `getRailroadCrossingForNode : function(nodeEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.system.railRoadCrossingSystem.getRailroadCrossingForNode(…)`): Gets all railroad crossings of the given base node entity.
    - param `nodeEntity`: the entity id of the node that should be inspected

## RiverSystem (record)

```teal
global record RiverSystem
```
Access: `api.engine.system.riverSystem`

River System.
Contains information about water in the map.

- `getWaterMeshEntities : function(tile0 : Vec2i, tile1 : Vec2i) : {Engine.Entity}`  (call as `api.engine.system.riverSystem.getWaterMeshEntities(…)`): Gets all water meshes within the bounds.

## RunwaySystem (record)

```teal
global record RunwaySystem
```
Access: `api.engine.system.runwaySystem`

Runway System.
Contains information about runway edges.

- `getLandingNodeIdMap : function() : {NodeId : Engine.Entity}`  (call as `api.engine.system.runwaySystem.getLandingNodeIdMap(…)`): Gets all stations reachable from that landing node.
- `getTakeoffNodeIdMap : function() : {NodeId : Engine.Entity}`  (call as `api.engine.system.runwaySystem.getTakeoffNodeIdMap(…)`): Gets all station from which the takeoff node can reached.

## ShipMoveSystem (record)

```teal
global record ShipMoveSystem
```
Access: `api.engine.system.shipMoveSystem`

Ship Move System.
Contains information about the movement of ships.

- `getShipInfo : function(vehicleEntity : Engine.Entity) : ShipInfo`  (call as `api.engine.system.shipMoveSystem.getShipInfo(…)`): Gets more path and movement information about ships.
    - param `vehicleEntity`: the entity id of the ship that should be inspected
- `forEach : function(Engine.Entity)`  (call as `api.engine.system.shipMoveSystem.forEach(…)`): Loops over all ships.

## SignalSystem (record)

```teal
global record SignalSystem
```
Access: `api.engine.system.signalSystem`

Signal System.
Contains information about Signals (waypoints, path and one way signals)

- `getSignal : function(edgeId : EdgeId, reversed : boolean) : SignalId`  (call as `api.engine.system.signalSystem.getSignal(…)`): Gets the signal from an @{api.type.EdgeId} at the given direction.
Notice: there can be at most one signal per direction per transport network edge.
    - param `edgeId`: the id of the edge hosting the signal
    - param `reversed`: false if signal in direction of edge is requested

## SimCargoSystem (record)

```teal
global record SimCargoSystem
```
Access: `api.engine.system.simCargoSystem`

Cargo System.
System managing cargo.

## SimPersonAtBuildingSystem (record)

```teal
global record SimPersonAtBuildingSystem
```
Access: `api.engine.system.simPersonAtBuildingSystem`

Person At Building System.
System managing persons at a building.

- `getSimPersonAtBuilding : function(simBuildingEntity : Engine.Entity) : {Engine.Entity}`  (call as `api.engine.system.simPersonAtBuildingSystem.getSimPersonAtBuilding(…)`): Gets list of entities currently at the building
    - param `simBuildingEntity`: the entity of the building that should be inspected

## SimPersonAtTerminalSystem (record)

```teal
global record SimPersonAtTerminalSystem
```
Access: `api.engine.system.simPersonAtTerminalSystem`

Person At Terminal System.
System managing persons waiting at terminal.

- `getEdgeInfoMap : function() : {EdgeId : any}`  (call as `api.engine.system.simPersonAtTerminalSystem.getEdgeInfoMap(…)`): Gets information about persons waiting at the terminals.
- `getNumFreePlaces : function(edgeId : EdgeId) : integer`  (call as `api.engine.system.simPersonAtTerminalSystem.getNumFreePlaces(…)`): Gets number of free waiting places for the given transport network edge.
    - param `edgeId`: the id of the edge that should be inspected
- `getPos01 : function(edgeId : EdgeId, place : integer) : {number, number}`  (call as `api.engine.system.simPersonAtTerminalSystem.getPos01(…)`): Gets the position on the transport network edge for the given place.
    - param `edgeId`: the id of the edge that should be inspected
    - param `place`: the waiting place number
    - returns: the position

## StockListSystem (record)

```teal
global record StockListSystem
```
Access: `api.engine.system.stockListSystem`

Stock List System.
Manages stocks.

## ParcelSystem (record)

```teal
global record ParcelSystem
```
Access: `api.engine.system.parcelSystem`

Parcel System.
Manages and returns information about parcels.

- `getParcelData : function(segmentEntity : Engine.Entity) : any`  (call as `api.engine.system.parcelSystem.getParcelData(…)`): Obtains all parcels belonging to a segment.
    - param `segmentEntity`: the segment of which to obtain all attached parcels
- `getSegment2ParcelData : function() : {Engine.Entity : any}`  (call as `api.engine.system.parcelSystem.getSegment2ParcelData(…)`): Obtains a map of all segments to the parcel data of the segment.

## TpNetLinkSystem (record)

```teal
global record TpNetLinkSystem
```
Access: `api.engine.system.tpNetLinkSystem`

Transport Network Link System.
Manages transport networks links (small links between roads and constructions).

- `getEdgeId2linkEntities : function() : {EdgeId : {Engine.Entity}}`  (call as `api.engine.system.tpNetLinkSystem.getEdgeId2linkEntities(…)`): Gets a map from all edges to the links touching the edge.
- `getLinkEntities : function(edgeId : EdgeId) : {Engine.Entity}`  (call as `api.engine.system.tpNetLinkSystem.getLinkEntities(…)`): Gets all links within a transport network edge.
    - param `edgeId`: the edge id for which the links should be fetched

## VehicleDepotSystem (record)

```teal
global record VehicleDepotSystem
```
Access: `api.engine.system.vehicleDepotSystem`

Vehicle Depot System.
Manages depots and vehicles in depots.

- `getDepotForInNodeId : function(nodeId : NodeId) : Engine.Entity`  (call as `api.engine.system.vehicleDepotSystem.getDepotForInNodeId(…)`): Gets the depot entity to which the node belongs.
    - param `nodeId`: the id of the node for which the depot should be fetched
- `forEach : function(Engine.Entity)`  (call as `api.engine.system.vehicleDepotSystem.forEach(…)`): Loops over all depots.

## AnimalMoveSystem (record)

```teal
global record AnimalMoveSystem
```
Access: `api.engine.system.animalMoveSystem`

Animal Move System.
Manages animals.

- `forEach : function(Engine.Entity)`  (call as `api.engine.system.animalMoveSystem.forEach(…)`): Loops over all animals.

## GridCollision (record)

```teal
global record GridCollision
```
Access: `api.engine.system.gridCollision`

Contains multiple grids holding data
Contains multiple grids holding diverse data

- `getGridCollisionSystem : function(index : integer) : GridCollisionSystem`  (call as `api.engine.system.gridCollision.getGridCollisionSystem(…)`): Gets the grid collision system with the given index.
    - param `index`: the index of the system to get
- `getPlayerInfrastructureCollisionSystem : function() : GridCollisionSystem`  (call as `api.engine.system.gridCollision.getPlayerInfrastructureCollisionSystem(…)`): Gets the grid collision system tracking player owned structures.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo_town/deliver_cargo_town.script.tl:264`: `local gics = api.engine.system.gridCollision.getPlayerInfrastructureCollisionSystem()`

## OctreeSystem (record)

```teal
global record OctreeSystem
```
Access: `api.engine.system.octreeSystem`

Octree System.
An octree that contains all entities with a bounding box. It enables fast query and location of entities in the map.

- `findIntersectingEntities : function(box : Box3f, callback : function(Engine.Entity, Engine.Component.BoundingVolume))`  (call as `api.engine.system.octreeSystem.findIntersectingEntities(…)`): Fetches all entities in a box and runs a callback for them.
    - param `box`: Box in which all entities are searched.
    - param `callback`: A function called for each matching entity within box. The callback obtains an entity and a bounding volume.

## LandmarkSystem (record)

```teal
global record LandmarkSystem
```
Access: `api.engine.system.landmarkSystem`

Landmark system.
Manages landmarks data.

- `getLandmarkData : function(landmarkEntity : Engine.Entity) : {EdgeId : number}`  (call as `api.engine.system.landmarkSystem.getLandmarkData(…)`): Returns distances from landmark entity to transport network edges.
    - param `landmarkEntity`: entity of the landmark

## System (record)

```teal
global record System
```
Access: `api.engine.system`

A system is a simulation object that controls entities with specific components or caches additional computed data
for entities after listening for component changes

Forward declaration of the `System` record (defined in `api/engine/system.d.tl`), the type of `api.engine.system`.

- `animalMoveSystem : AnimalMoveSystem`: Controls animal simulation and movement
- `aircraftMoveSystem : AircraftMoveSystem`: Controls aircraft (helicopters and airplanes) simulation (reservation, pathing, ...) and movement
- `baseParallelStripSystem : BaseParallelStripSystem`: A cache of data that is computed from parallel strip data
- `carriageListSystem : CarriageListSystem`: A cache of data for carriage lists
- `catchmentAreaSystem : CatchmentAreaSystem`: `CatchmentAreaSystem`: cached reachability of stations and destinations.
- `emissionGridSystem : EmissionGridSystem`: `EmissionGridSystem`: manages the emission (noise, pollution) grid data.
- `gameScriptSystem : GameScriptSystem`: `GameScriptSystem`: manages the entities that hold game-script state; use `getEntityForGameScript(name)` to find them.
- `landVehicleMoveSystem : LandVehicleMoveSystem`: `LandVehicleMoveSystem`: manages the movement of land vehicles.
- `landmarkSystem : LandmarkSystem`: `LandmarkSystem`: manages landmark data, such as distances from a landmark to transport network edges.
- `lineSystem : LineSystem`: Manages all line data
- `octreeSystem : OctreeSystem`: Efficient way to collect entities in a specific region of space
- `parcelSystem : ParcelSystem`: Manages parcels for towns, and provides inverse relation from segment to parcels
- `railRoadCrossingSystem : RailRoadCrossingSystem`: `RailRoadCrossingSystem`: information about railroad crossings.
- `riverSystem : RiverSystem`: Create a vector field of waterfronts for street and construction alignment
- `runwaySystem : RunwaySystem`: `RunwaySystem`: information about runway edges.
- `shipMoveSystem : ShipMoveSystem`: `ShipMoveSystem`: information about the movement of ships.
- `signalSystem : SignalSystem`: `SignalSystem`: information about signals (waypoints, path signals and one-way signals).
- `simCargoSystem : SimCargoSystem`: `SimCargoSystem`: manages cargo items.
- `simPersonSystem : SimPersonSystem`: `SimPersonSystem`: manages persons.
- `simPersonAtBuildingSystem : SimPersonAtBuildingSystem`: `SimPersonAtBuildingSystem`: manages persons that are at a building.
- `simPersonAtTerminalSystem : SimPersonAtTerminalSystem`: `SimPersonAtTerminalSystem`: manages persons waiting at a terminal.
- `simPersonAtVehicleSystem : SimPersonAtVehicleSystem`: `SimPersonAtVehicleSystem`: manages persons travelling on a vehicle.
- `simEntityAtStockSystem : SimEntityAtStockSystem`: `SimEntityAtStockSystem`: manages cargo produced by industries or delivered to a warehouse (stock counts per cargo type).
- `simEntityAtTerminalSystem : SimEntityAtTerminalSystem`: `SimEntityAtTerminalSystem`: manages persons and cargo waiting at terminals, e.g. per line stop.
- `simEntityAtVehicleSystem : SimEntityAtVehicleSystem`: `SimEntityAtVehicleSystem`: manages persons and cargo travelling in a vehicle.
- `stationSystem : StationSystem`: `StationSystem`: cached information about stations.
- `stationGroupSystem : StationGroupSystem`: `StationGroupSystem`: cached information about station groups, e.g. the group of a station.
- `stockListSystem : StockListSystem`: `StockListSystem`: manages stocks.
- `streetSystem : StreetSystem`: `StreetSystem`: manages base edges (street and track segments) and their nodes.
- `streetConnectorSystem : StreetConnectorSystem`: Caches relations between constructions and base edges
- `townBuildingSystem : TownBuildingSystem`: `TownBuildingSystem`: manages town buildings and parcels, including their cargo supply and limits.
- `tpNetLinkSystem : TpNetLinkSystem`: `TpNetLinkSystem`: manages transport network links, the short links between roads and constructions.
- `transportNetworkSystem : TransportNetworkSystem`: `TransportNetworkSystem`: manages transport networks and their edges and nodes.
- `transportVehicleSystem : TransportVehicleSystem`: Manages cargo loading and unloading, as well as arrival of any transport vehicle
- `vehicleDepotSystem : VehicleDepotSystem`: Manages door animation of depots
- `gridCollision : GridCollision`: A generic system representing a 2d grid in the world of miscelaneous data
