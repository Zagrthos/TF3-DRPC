# api.engine.util

Definition file: `api/tealdef/api/engine/util.d.tl` (in the game folder).

Contains definitions for of api.engine.util

## Globals

- `JournalEntry` (type): One finance journal booking (`api.type.JournalEntry`). Create it with `api.type.JournalEntry.new()`,
set `amount`, `time` and `category`, and book it with `api.cmd.makeJournalBookAssetCmd`.

## UtilTransport (record)

```teal
global record UtilTransport
```
Access: `api.engine.util.transport`

Transport-edge helpers (positions along edge geometry, ports, carriers), available as `api.engine.util.transport`.

- `calcPosition : function(eg : EdgeGeometry, t : number) : Vec3f`  (call as `api.engine.util.transport.calcPosition(…)`): Calc position along a transport edge.
    - param `eg`: edge geometry
    - param `t`: progress along the edge [ 0, 1 ]
    - returns: position on the edge
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_manager_panel.tl:1118`: `local worldPos = api.engine.util.transport.calcPosition(edge.geometry, via.waypoint.edgePos.param)`
- `calcPositionAndDirection : function(eg : EdgeGeometry, t : number, forward? : boolean) : {Vec3f}`  (call as `api.engine.util.transport.calcPositionAndDirection(…)`): Calc position along a transport edge.
pos, dir
    - param `eg`: edge geometry
    - param `t`: progress along the edge [ 0, 1 ]
    - param `forward`: optional bool (forward or backward t -> t - 1.0)
    - returns: position on the edge and direction
- `transportModesToCarriers : function(transportModes : {TransportMode}) : {Carrier}`  (call as `api.engine.util.transport.transportModesToCarriers(…)`): Transform list of transport modes to a vector of carriers.
    - param `transportModes`: list of transport modes
    - returns: carriers for a given set of transport modes
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_util.tl:62`: `local carriers = api.engine.util.transport.transportModesToCarriers(transportModes)`
- `getStartPort : function(edge : {EdgeId, boolean}) : PortId`  (call as `api.engine.util.transport.getStartPort(…)`): Obtains the start connection of this edge.
    - param `edge`: the edge with direction
    - returns: PortId at the edge's start
- `getEndPort : function(edge : {EdgeId, boolean}) : PortId`  (call as `api.engine.util.transport.getEndPort(…)`): Obtains the end connection of this edge.
    - param `edge`: the edge with direction
    - returns: PortId at the edge's end
- `isPortLocked : function(port : integer) : boolean`  (call as `api.engine.util.transport.isPortLocked(…)`): Check if a port is locked.
    - param `port`: the port
    - returns: true if the port is locked, false otherwise

## UtilStock (record)

```teal
global record UtilStock
```
Access: `api.engine.util.stock`

Stock and production statistics of stock lists (industries, warehouses), available as `api.engine.util.stock`.

- `getStockCargoTypes : function(stockListEntity : Engine.Entity, stockId : StockId) : {CargoTypeId}`  (call as `api.engine.util.stock.getStockCargoTypes(…)`): Get stock cargo types.
    - param `stockListEntity`: entity containting the stocklist component
    - param `stockId`: stock id
    - returns: set of CargoTypeId
    - used 4x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:52`: `local cargoTypes : {CargoTypeId} = api.engine.util.stock.getStockCargoTypes(entity, stockId)`
- `getCargoTypeShippedPerYear : function(stockListEntity : Engine.Entity, cargoTypeId : CargoTypeId) : integer`  (call as `api.engine.util.stock.getCargoTypeShippedPerYear(…)`): Get num cargo shipped per year
    - param `stockListEntity`: list entity
    - param `cargoTypeId`: cargo type id
    - returns: num cargo shipped per year
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:122`: `shipped = shipped + api.engine.util.stock.getCargoTypeShippedPerYear(entity, cargoType)`
- `getCargoShippedPerYear : function(stockListEntity : Engine.Entity) : integer`  (call as `api.engine.util.stock.getCargoShippedPerYear(…)`): Get num cargo shipped per year
    - param `stockListEntity`: list entity
    - returns: num cargo shipped per year
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/industries/industries.script.tl:173`: `local shippedPerYear = api.engine.util.stock.getCargoShippedPerYear(industry.stockList)`
- `getCargoTypeDeliveredPerYear : function(stockListEntity : Engine.Entity, cargoTypeId : CargoTypeId) : integer`  (call as `api.engine.util.stock.getCargoTypeDeliveredPerYear(…)`): Get num cargo delivered per year
    - param `stockListEntity`: list entity
    - param `cargoTypeId`: cargo type id
    - returns: num cargo delivered per year
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:111`: `delivered = delivered + api.engine.util.stock.getCargoTypeDeliveredPerYear(entity, cargoType)`
- `getCargoDeliveredPerYear : function(stockListEntity : Engine.Entity) : integer`  (call as `api.engine.util.stock.getCargoDeliveredPerYear(…)`): Get num cargo delivered (to entity) per year
    - param `stockListEntity`: stock list entity
    - returns: num cargo delivered per year
    - used 1x in the base game, e.g. `base/content/gui/gui/main/industry_util.tl:551`: `local numDelivered = api.engine.util.stock.getCargoDeliveredPerYear(industry.stockList)`
- `getCargoConsumedPerYear : function(stockListEntity : Engine.Entity, cargoTypeId : CargoTypeId) : integer`  (call as `api.engine.util.stock.getCargoConsumedPerYear(…)`): Get num cargo consumed per year
    - param `stockListEntity`: list entity
    - param `cargoTypeId`: cargo type id
    - returns: num cargo consumed per year
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:663`: `count = inputCount and api.engine.util.stock.getCargoConsumedPerYear(entity, inputCargoTypeId) or nil,`
- `getCargoProducedPerYear : function(stockListEntity : Engine.Entity, cargoTypeId : CargoTypeId) : integer`  (call as `api.engine.util.stock.getCargoProducedPerYear(…)`): Get num cargo produced per year
    - param `stockListEntity`: list entity
    - param `cargoTypeId`: cargo type id
    - returns: num cargo produced per year
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:672`: `count = outputCount and api.engine.util.stock.getCargoProducedPerYear(entity, outputCargoTypeId) or nil,`
- `getCargoMaxProductionPerYear : function(stockListEntity : Engine.Entity, cargoTypeId : CargoTypeId) : integer`  (call as `api.engine.util.stock.getCargoMaxProductionPerYear(…)`): Get num cargo max production per year
    - param `stockListEntity`: list entity
    - param `cargoTypeId`: cargo type id
    - returns: num cargo max production per year
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:673`: `max = outputMax and api.engine.util.stock.getCargoMaxProductionPerYear(entity, outputCargoTypeId) or nil,`
- `getCargoMaxConsumptionPerYear : function(stockListEntity : Engine.Entity, cargoTypeId : CargoTypeId) : integer`  (call as `api.engine.util.stock.getCargoMaxConsumptionPerYear(…)`): Get num cargo max consumption per year
    - param `stockListEntity`: list entity
    - param `cargoTypeId`: cargo type id
    - returns: num cargo max consumption per year
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:664`: `max = inputMax and api.engine.util.stock.getCargoMaxConsumptionPerYear(entity, inputCargoTypeId) or nil,`
- `getInputsOutputsFromRules : function(stockListEntity : Engine.Entity) : {{CargoTypeId}, {CargoTypeId}}`  (call as `api.engine.util.stock.getInputsOutputsFromRules(…)`): Get inputs and outputs from rules
    - param `stockListEntity`: list entity
    - returns: inputs and outputs cargo types
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/industries/industries.script.tl:140`: `local outputCargo = api.engine.util.stock.getInputsOutputsFromRules(industry.stockList)[2]`
- `getCargoOutputPerYear : function(stockListEntity : Engine.Entity) : integer`  (call as `api.engine.util.stock.getCargoOutputPerYear(…)`): Get the total number of cargo produced per year
    - param `stockListEntity`: list entity
    - returns: num cargo produced per year
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/industries/industries.script.tl:168`: `local outputPerYear = api.engine.util.stock.getCargoOutputPerYear(industry.stockList)`
- `getCargoLogValueDiffPerYear : function(stockListEntity : Engine.Entity, logType : string, cargoTypeId : CargoTypeId) : integer`  (call as `api.engine.util.stock.getCargoLogValueDiffPerYear(…)`): Get cargo log value in the last year for cargo type
    - param `stockListEntity`: stock list entity
    - param `logType`: the logbook type
    - param `cargoTypeId`: cargo type
    - returns: cargo log value difference in the last year for cargo type
    - used 4x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:238`: `local delta : integer = api.engine.util.stock.getCargoLogValueDiffPerYear(entity, "itemsUnloaded", cargoTypeId)`
- `getCargoLogPerYearForTarget : function(sourceStockListEntity : Engine.Entity, targetStockListEntity : Engine.Entity, logType : string, cargoTypeId : CargoTypeId) : integer`  (call as `api.engine.util.stock.getCargoLogPerYearForTarget(…)`): Get num cargo log per year
    - param `sourceStockListEntity`: stock list entity
    - param `targetStockListEntity`: stock list entity
    - param `logType`: the logbook type
    - param `cargoTypeId`: the cargo type
    - returns: num cargo shipped per year
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:227`: `local numDelivered = api.engine.util.stock.getCargoLogPerYearForTarget(`
- `getCargoLogPerYearForTargetFilter : function(sourceStockListEntity : Engine.Entity, targetFilter : function(Engine.Entity) : boolean, logType : string, cargoTypeId : CargoTypeId) : integer`  (call as `api.engine.util.stock.getCargoLogPerYearForTargetFilter(…)`): Get num cargo log per year
    - param `sourceStockListEntity`: stock list entity
    - param `targetFilter`: filter function to filter target entities
    - param `logType`: the logbook type
    - param `cargoTypeId`: the cargo type
    - returns: num cargo shipped per year
- `getProductionRating : function(stockListEntity : Engine.Entity) : number`  (call as `api.engine.util.stock.getProductionRating(…)`): Get stocklist production rating
    - param `stockListEntity`: list entity
    - returns: production rating
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/industries/industries.script.tl:172`: `local productionRating = api.engine.util.stock.getProductionRating(industry.stockList)`
- `getStockGroupsByTypeAndCargoTypes : function(stockListEntity : Engine.Entity, sortByType : boolean, showEmpty : boolean, producedOnly : boolean) : {{StockListType, {StockId}, {CargoTypeId}, integer}}`  (call as `api.engine.util.stock.getStockGroupsByTypeAndCargoTypes(…)`): Get stock groups by type and cargo types
    - param `stockListEntity`: list entity
    - param `sortByType`: sort by type
    - param `showEmpty`: show empty groups
    - returns: {StockListType, {StockId}, {CargoTypeId}, int} list of stock list type, stock ids, cargo types and count per group
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:46`: `groups = api.engine.util.stock.getStockGroupsByTypeAndCargoTypes(entity, sortByType or false, showEmpty ~= false, producedOnly == true)`
- `getStockListsWithThrownAwayCargo : function() : {Engine.Entity : integer}`  (call as `api.engine.util.stock.getStockListsWithThrownAwayCargo(…)`): Get stock lists with thrown away cargo
    - returns: map of entity to amount thrown away
- `hasAnyCompatibleStock : function(entities : {Engine.Entity}, cargoTypeId : CargoTypeId, load : boolean) : boolean`  (call as `api.engine.util.stock.hasAnyCompatibleStock(…)`): Check if any stock is compatible
    - param `entities`: list of entities to check
    - param `cargoTypeId`: cargo type
    - param `load`: whether to check for loading/input (true) or unloading/output (false)
    - returns: true if any stock is compatible
- `getAdjustedDeliveryTime : function(cargoTypeId : CargoTypeId) : integer`  (call as `api.engine.util.stock.getAdjustedDeliveryTime(…)`): Get adjusted delivery time
    - param `cargoTypeId`: cargo type
    - returns: adjusted delivery time in seconds
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_cargo_lifetime.tl:213`: `local ad = api.engine.util.stock.getAdjustedDeliveryTime(a[1])`
- `isCargoTypeCurrentlyProduced : function(cargoTypeId : CargoTypeId) : boolean`  (call as `api.engine.util.stock.isCargoTypeCurrentlyProduced(…)`): Check if cargo type is currently produced
    - param `cargoTypeId`: cargo type
    - returns: true if cargo type is currently produced
    - used 18x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:335`: `if api.engine.util.stock.isCargoTypeCurrentlyProduced(cargoTypeId) then`

## UtilLine (record)

```teal
global record UtilLine
```
Access: `api.engine.util.line`

Line helpers (capacity, frequency, line problems and issues, compatibility), available as `api.engine.util.line`.

- `getLineCapacityUsages : function(line : Engine.Entity, allCapacities : boolean) : {CargoTypeId : LineCapacityUsage}`  (call as `api.engine.util.line.getLineCapacityUsages(…)`): Get line capacity usages.
    - param `line`: line entity
    - param `allCapacities`: whether to return all capacities or the capacities for the vehicles current load config
    - returns: map of cargo type to capacity usage
    - used 2x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:709`: `local capacities = api.engine.util.line.getLineCapacityUsages(lineEntity, false)`
- `calcLineStationThroughput : function(lineEntity : Engine.Entity) : integer`  (call as `api.engine.util.line.calcLineStationThroughput(…)`): Calc line throughput
    - param `lineEntity`: line entity
    - returns: throughput value
    - used 5x in the base game, e.g. `base/content/gui/gui/entity_window/line/line_eow.script.tl:36`: `local throughput = api.engine.util.line.calcLineStationThroughput(entityId)`
- `getMaxFrequency : function(lineEntity : Engine.Entity) : number`  (call as `api.engine.util.line.getMaxFrequency(…)`): Get line max frequency
    - param `lineEntity`: line entity
    - returns: max frequency
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:1351`: `local maxFrequency = api.engine.util.line.getMaxFrequency(entityId)`
- `getDetailedLineProblems : function(lineEntity : Engine.Entity) : {{StopState}}`  (call as `api.engine.util.line.getDetailedLineProblems(…)`): Get lines with detailed problems per stop
    - param `lineEntity`: line entity
    - returns: list of stop problems
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:5993`: `local detailedLineProblems = api.engine.util.line.getDetailedLineProblems(lineEAR.entity)`
- `getLineProblems : function() : {{Engine.Entity, integer, PathProblemLocation}}`  (call as `api.engine.util.line.getLineProblems(…)`): Get lines with problems
    - returns: list of {line entity, problem type, optional info about 1st problem and its location}
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:126`: `local lineAndProblems = api.engine.util.line.getLineProblems()`
- `getLineStationProblems : function() : {{Engine.Entity, Engine.Entity, LineStationProblem}}`  (call as `api.engine.util.line.getLineStationProblems(…)`): Get station problems with lines
    - returns: list of line entity, station group entity and problem type
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:182`: `local lineStationAndProblems = api.engine.util.line.getLineStationProblems()`
- `isLineCompatibleWithCarrier : function(lineEntity : Engine.Entity, carrier : Carrier, distinguishTramFromRoad : boolean) : boolean`  (call as `api.engine.util.line.isLineCompatibleWithCarrier(…)`): Check if line is ok with provided carrier
    - param `lineEntity`: line entity
    - param `carrier`: carrier type
    - param `distinguishTramFromRoad`: if tram should not be counted as road
    - returns: true if the line is compatible with the specified carrier
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:159`: `if api.engine.util.line.isLineCompatibleWithCarrier(lineEntity, api.type["enum"].Carrier.RAIL, false) then`
- `isLineCompatibleWithAnyCarrier : function(lineEntity : Engine.Entity, carriers : {Carrier}, distinguishTramFromRoad : boolean) : boolean`  (call as `api.engine.util.line.isLineCompatibleWithAnyCarrier(…)`): Check if line is ok with any of provided carriers
    - param `lineEntity`: line entity
    - param `carriers`: list of carrier types
    - param `distinguishTramFromRoad`: if tram should not be counted as road
    - returns: true if the line is compatible with any of the specified carriers
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:2058`: `if api.engine.util.line.isLineCompatibleWithAnyCarrier(lineEntity, {c.TRAM, c.RAIL}, false) then`
- `getLineTransportModesUnion : function(lineEntity : Engine.Entity) : {TransportMode : boolean}`  (call as `api.engine.util.line.getLineTransportModesUnion(…)`): Get union of all transport modes offered by all lines stops
    - param `lineEntity`: line entity
    - returns: List of transport modes
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:2043`: `local transportModes = api.engine.util.line.getLineTransportModesUnion(lineEntity)`
- `supportsReservation : function(lineEntity : Engine.Entity) : boolean`  (call as `api.engine.util.line.supportsReservation(…)`): Check if line supports reservations
    - param `lineEntity`: line entity
    - returns: whether line supports reservations
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/line/line.tl:94`: `return api.engine.util.line.supportsReservation(entityId)`
- `getLineIssues : function(lineEntity : Engine.Entity, allIssues : boolean) : {LineIssue}`  (call as `api.engine.util.line.getLineIssues(…)`): Get line configuration problems for the line
    - param `lineEntity`: line entity
    - param `allIssues`: whether to return all issues
    - returns: all line issues of the line
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notification_legacy_util.tl:154`: `local issues = api.engine.util.line.getLineIssues(entity, false)`
- `getLinesIssues : function(playerEntity : Engine.Entity, allIssues : boolean) : {Engine.Entity : {LineIssue}}`  (call as `api.engine.util.line.getLinesIssues(…)`): Get line configuration problems for the player lines
    - param `playerEntity`: player entity
    - param `allIssues`: whether to return all issues
    - returns: all line issues of the lines belonging to the player
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:125`: `local misconfiguredLines = api.engine.util.line.getLinesIssues(api.engine.util.getPlayer(), false)`
- `getFailedPathReason : function(vehicle : Engine.Entity, line : Engine.Entity, fullLine? : boolean) : Failure`  (call as `api.engine.util.line.getFailedPathReason(…)`): Get the reason for failed pathfinding search
    - param `vehicle`: vehicle entity
    - param `line`: line entity
    - param `fullLine`: whether to check full line
    - returns: failure reason
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:4707`: `local failReason = api.engine.util.line.getFailedPathReason(`
- `getNoRoadConnectionProblems : function() : {Engine.Entity}`  (call as `api.engine.util.line.getNoRoadConnectionProblems(…)`): Get nodes (construction entities) without any road connection
    - returns: list of node entities
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:425`: `local noRoadConnectionProblem = api.engine.util.line.getNoRoadConnectionProblems()`

### UtilLine.LineCapacityUsage (record)

```teal
record LineCapacityUsage
```

Used and total capacity of a line for one cargo type, as returned per cargo type by `getLineCapacityUsages`.

- `used : number`: Currently used capacity
- `capacity : number`: Total capacity

### UtilLine.PathProblemLocation (record)

```teal
record PathProblemLocation
```

Where a line's path search failed, with the transport modes found there; attached to line problems from `getLineProblems` and to `StopState`.

Notes:

The base game's notifications script uses `nodePosition` as the map location of the line warning and passes
`okModes`, `relaxedModes` and `allowedModes` on to explain the cause (for example missing catenary or tram lanes).

Example:
```lua
for _, lineAndProblem in ipairs(api.engine.util.line.getLineProblems()) do
  local lineEntity, problem, location = lineAndProblem[1], lineAndProblem[2], lineAndProblem[3]
  if location then
    local p = location.nodePosition
    print(lineEntity, problem, p.x, p.y, p.z)
  end
end
```

- `edgeIndex : integer`
- `node : NodeId`: Transport network node at the problem location.
- `nodePosition : Vec3f`: World position of `node`; the base game uses it as the map location of the line warning.
- `okModes : {TransportMode : boolean}`: compatible transport modes present on the last compatible edge (e.g. ELECTRIC_TRAIN)
- `relaxedModes : {TransportMode : boolean}`: compatible relaxed transport modes on the first incompatible edge.
Example 1: okModes[ELECTRIC_TRAIN], relaxedModes[TRAIN] -> relaxation allowed the use of non-electric train, indicating missing catenaries
Example 2: okModes[TRAM], relaxedModes[ROAD] -> relaxation allowed the use of road instead of tram, indicating missing tram lanes
note that this can never overlap with okModes (otherwise there would not be a problem here)
- `allowedModes : {TransportMode : boolean}`: Allowed modes for the line / vehicle in general

### UtilLine.TerminalCapacityUsage (record)

```teal
record TerminalCapacityUsage
```

Used and total capacity of a station terminal or of the station's waiting pool, as returned by `UtilStation.calculateStationTerminalUsage`.

- `equals : function(TerminalCapacityUsage, TerminalCapacityUsage) : boolean`: Returns `true` when both `TerminalCapacityUsage` values are equal.
- `used : number`: Currently used capacity
- `capacity : number`: Total capacity

### UtilLine.StopState (record)

```teal
record StopState
```

Information about the problem of a line.

- `empty : boolean`: No vehicles and less than 2 stops
- `noPath : boolean`: No path between stops
- `noPathFromAlternative : {integer, PathProblemLocation}`: No path from an alternative stop {flattenedTerminalIndex, optional problem location}
- `noPathToAlternative : {integer, PathProblemLocation}`: No path to an alternative stop {flattenedTerminalIndex, optional problem location}
- `duplicateStop : boolean`: Stop is a duplicate
- `incompatibleStop : boolean`: Stop is incompatible with the line's transport mode
- `noPathLocation : PathProblemLocation`: Optional problem location in case of noPath

## UtilLogbook (record)

```teal
global record UtilLogbook
```
Access: `api.engine.util.logbook`

Read and write named logbook values and charts of entities with a logbook component, available as `api.engine.util.logbook`.

- `getLogValuePerYear : function(entity : Engine.Entity, logName : string) : integer`  (call as `api.engine.util.logbook.getLogValuePerYear(…)`): Get log value difference in the last year for cargo type
    - param `entity`: entity with logbook component
    - param `logName`: log entry name
    - returns: log value difference in the last year for cargo type

    ```lua
    local perYear = api.engine.util.logbook.getLogValuePerYear(entity, logName)
    ```

    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:736`: `local numPerYear = api.engine.util.logbook.getLogValuePerYear(params.entity, rawLogName)`
- `getLogValueMostRecent : function(entity : Engine.Entity, logName : string) : integer`  (call as `api.engine.util.logbook.getLogValueMostRecent(…)`): Get most recent log value
    - param `entity`: entity with logbook component
    - param `logName`: log entry name
    - returns: log value difference in the last year for cargo type
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:162`: `local trackTotalLength = api.engine.util.logbook.getLogValueMostRecent(world, "trackTotalLength")`
- `getLogValueOldest : function(entity : Engine.Entity, logName : string) : integer`  (call as `api.engine.util.logbook.getLogValueOldest(…)`): Get oldest log value
    - param `entity`: entity with logbook component
    - param `logName`: log entry name
    - returns: oldest log value
- `logValue : function(entity : Engine.Entity, logName : string, value : integer)`  (call as `api.engine.util.logbook.logValue(…)`): Log a value
    - param `entity`: entity with logbook component
    - param `logName`: log entry name
    - param `value`: value to log
- `getChart : function(logbookEntity : Engine.Entity, logNames : {string}, xMin? : number, xMax? : number, yMin? : number, yMax? : number, config? : ChartConfig) : ChartResult`  (call as `api.engine.util.logbook.getChart(…)`): Get chart for logbook
    - param `logbookEntity`: entity with logbook component
    - param `logNames`: log types
    - param `xMin`: Starting value of x-axis (automatically reduced to the lowest x in the data)
    - param `xMax`: End value of x-axis (automatically increased to the highest x in the data)
    - param `yMin`: Starting value of y-axis (automatically reduced to the lowest y in the data)
    - param `yMax`: End value of y-axis (automatically increased to the highest y in the data)
    - param `ChartConfig`: config of the chart to get
    - returns: chart result
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:361`: `filteredState.result = api.engine.util.logbook.getChart(player, filteredState.logNames, minTimestamp)`
- `getChartDiff : function(entity : Engine.Entity, logNames : {string}, config : ChartConfig) : ChartResult`  (call as `api.engine.util.logbook.getChartDiff(…)`): Get diff chart for logbooks
    - param `entity`: entity with logbook component
    - param `logNames`: log types
    - param `config`: chart config
    - returns: chart result
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:879`: `api.engine.util.logbook.getChartDiff(params.entityId, params.logNames, config)`

## UtilChart (record)

```teal
global record UtilChart
```
Access: `api.engine.util.chart`

Helpers for chart labels and results, available as `api.engine.util.chart`.

- `getChartLabel : function(config : ChartConfig, index : integer, abbreviated : boolean) : string`  (call as `api.engine.util.chart.getChartLabel(…)`): Get label for X axis of date charts.
    - param `config`: chart config
    - param `index`: group index of which the label should be returned
    - param `abbreviated`: if label should be for range (true) or point (false)
    - returns: chart label string
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:119`: `return api.engine.util.chart.getChartLabel(config, math.floor(val), false)`
- `setLastValue : function(chartResult : ChartResult, series : integer, value : number)`  (call as `api.engine.util.chart.setLastValue(…)`): Set the last value for a chart result.
    - param `chartResult`: chart result containing the series to be updated
    - param `series`: series index of which the last value should be set
    - param `value`: value to set as the last value of the series
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:220`: `api.engine.util.chart.setLastValue(result, 0, -currentDebt)`

## UtilOctree (record)

```teal
global record UtilOctree
```
Access: `api.engine.util.octree`

Spatial queries (entities or transport network nodes within a circle), available as `api.engine.util.octree`.

- `findEntitiesInCircle : function(center : Vec2f, radius : number, componentType : Engine.ComponentType) : {Engine.Entity}`  (call as `api.engine.util.octree.findEntitiesInCircle(…)`): Get entities in given radius and component type
    - param `center`: center position
    - param `radius`: search radius
    - param `componentType`: component type
    - returns: list of entities

    ```lua
    local stations = api.engine.util.octree.findEntitiesInCircle(api.type.Vec2f.new(x, y), radius, api.type.ComponentType.STATION)
    for _, station in ipairs(stations) do
      print(api.engine.util.getEntityName(station))
    end
    ```

    - used 14x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_util_line.tl:5`: `local stations1 = api.engine.util.octree.findEntitiesInCircle(api.type.Vec2f.new(from.pos[1], from.pos[2]), from.radius, api.type.ComponentType.STATION)`
- `findTransportNetworkNodesInCircle : function(center : Vec2f, radius : number) : {NodeId}`  (call as `api.engine.util.octree.findTransportNetworkNodesInCircle(…)`): Get transport network nodes in given radius
    - param `center`: center position
    - param `radius`: search radius
    - returns: list of node IDs
    - used 3x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:76`: `local nodes2 = api.engine.util.octree.findTransportNetworkNodesInCircle(position, radius)`
- `areWaypointsOnLine : function(waypoints : {Vec2f}, lineEntity : Engine.Entity, tolerance : number) : boolean`  (call as `api.engine.util.octree.areWaypointsOnLine(…)`): Check if waypoints are on a line
    - param `waypoints`: list of positions
    - param `lineEntity`: the line entity
    - param `tolerance`: distance tolerance
    - returns: true if waypoints are on line
    - used 2x in the base game, e.g. `base/content/mission/mission/tasks/waypoints/waypoints.tl:57`: `ctx.state.waypointsCompleted[i] = api.engine.util.octree.areWaypointsOnLine({ wp }, line, ctx.params.tolerance or 5)`

## UtilParallelStrip (record)

```teal
global record UtilParallelStrip
```
Access: `api.engine.util.parallelStrip`

Information about parallel strip entities (bridges and tunnels), available as `api.engine.util.parallelStrip`.

- `getStripInfo : function(entity : Engine.Entity) : StripInfo`  (call as `api.engine.util.parallelStrip.getStripInfo(…)`): Get strip information for a parallel strip entity
    - param `entity`: parallel strip entity
    - returns: strip information
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/bridge_and_tunnel.tl:62`: `return api.engine.util.parallelStrip.getStripInfo(param.keyEntity)`
- `isBridge : function(entity : Engine.Entity) : boolean`  (call as `api.engine.util.parallelStrip.isBridge(…)`): Check if entity is a bridge
    - param `entity`: parallel strip entity
    - returns: true if bridge
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/bridge_and_tunnel.tl:47`: `if api.engine.util.parallelStrip.isBridge(param.keyEntity) then`
- `isTunnel : function(entity : Engine.Entity) : boolean`  (call as `api.engine.util.parallelStrip.isTunnel(…)`): Check if entity is a tunnel
    - param `entity`: parallel strip entity
    - returns: true if tunnel
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/bridge_and_tunnel.tl:49`: `elseif api.engine.util.parallelStrip.isTunnel(param.keyEntity) then`

### UtilParallelStrip.StripInfo (record)

```teal
record StripInfo
```

Carrier and current bridge or tunnel type of a parallel strip entity, returned by `getStripInfo`.

Example:
```lua
local info = api.engine.util.parallelStrip.getStripInfo(entity)
local rep = api.engine.util.parallelStrip.isBridge(entity) and api.res.bridgeTypeRep or api.res.tunnelTypeRep
-- info.edgeTypeIndex is the index of the current type in that rep
```

- `carrier : Carrier`: Carrier of the strip; the base game offers only bridge or tunnel types that list this carrier.
- `edgeType : BaseEdgeType`: Base edge type of the strip (bridge or tunnel).
- `edgeTypeIndex : integer`: Index of the current bridge or tunnel type in `api.res.bridgeTypeRep` or `api.res.tunnelTypeRep`.

## UtilPathfinding (record)

```teal
global record UtilPathfinding
```
Access: `api.engine.util.pathfinding`

Path searches on the transport network and Dubins paths, available as `api.engine.util.pathfinding`.

- `findPath : function(startingEdgesAndLengths : {EdgeIdDirAndLength}, destinationNodes : {NodeId}, transportModeSet : {TransportMode}, maxLength : number) : {{EdgeId, boolean}}`  (call as `api.engine.util.pathfinding.findPath(…)`): Find a path between starting edges and end destination nodes, using a given set of transport modes.
    - param `startingEdgesAndLengths`: Vector of EdgeIdDirAndLength
    - param `destinationNodes`: Vector of NodeId
    - param `transportModeSet`: Vector of TransportMode
    - param `maxLength`: maximum path length
    - returns: a vector with pairs of EdgeId and bool values
- `findPathNodeToNode : function({NodeId}, {NodeId}, {TransportMode}) : {{EdgeId, boolean}}`  (call as `api.engine.util.pathfinding.findPathNodeToNode(…)`): Find a path between starting nodes and end destination nodes, using a given set of transport modes.
    - param `startNodes`: Vector of NodeId
    - param `destinationNodes`: Vector of NodeId
    - param `transportModeSet`: Vector of TransportMode
    - returns: a vector with pairs of EdgeId and bool values
    - used 3x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:70`: `local result = api.engine.util.pathfinding.findPathNodeToNode(nodes1, nodes2, transportModes)`
- `getTargetsByLandUse : function() : table`  (call as `api.engine.util.pathfinding.getTargetsByLandUse(…)`)
- `findDubinsPath : function(p0 : Vec3f, t0 : Vec3f, p1 : Vec3f, t1 : Vec3f, turnRadius : number) : {{EdgeGeometry, boolean}}`  (call as `api.engine.util.pathfinding.findDubinsPath(…)`): Find a Dubins path between two points with directions
    - param `p0`: first point
    - param `t0`: tangent at p0
    - param `p1`: second point
    - param `t1`: tangent at p1
    - param `turnRadius`: maximal turn radius
    - returns: A list of EdgeGeometries and direction
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:89`: `local dubins = api.engine.util.pathfinding.findDubinsPath(startPos, startDir, nextTarget3D, randomDir(), turnRadius)`

### UtilPathfinding.EdgeIdDirAndLength (record)

```teal
record EdgeIdDirAndLength
```

Directed transport edge with a length, used as a start edge for `findPath`; create it with `new(edgeId, dir, length)`.

- `edgeId : EdgeId`: Edge ID
- `dir : boolean`: Direction
- `length : number`: Length
- `new : function(edgeId : EdgeId, dir : boolean, length : number) : EdgeIdDirAndLength`: Generates a new item.

### UtilPathfinding.EdgeIdDir (record)

```teal
record EdgeIdDir
```

Directed transport edge (edge id plus direction flag); create it with `new(edgeId, dir)`.

- `edgeId : EdgeId`: Edge ID
- `dir : boolean`: Direction
- `new : function(edgeId : EdgeId, dir : boolean) : EdgeIdDir`: Generates a new item.

## UtilMaintenance (record)

```teal
global record UtilMaintenance
```
Access: `api.engine.util.maintenance`

Maintenance cost calculation for station groups and subconstructions, available as `api.engine.util.maintenance`.

- `calcMaintenanceForStationGroup : function(stationGroupEntity : Engine.Entity) : integer`  (call as `api.engine.util.maintenance.calcMaintenanceForStationGroup(…)`): Calculate maintenance cost for a station group
    - param `stationGroupEntity`: station group entity
    - returns: maintenance cost
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:158`: `return {mathutil.round(api.engine.util.maintenance.calcMaintenanceForStationGroup(param.entity)), "Upkeep"}`
- `calcMaintenanceForSubconstruction : function(subconstructionEntity : Engine.Entity) : integer`  (call as `api.engine.util.maintenance.calcMaintenanceForSubconstruction(…)`): Calculate maintenance cost for a subconstruction
    - param `subconstructionEntity`: subconstruction entity
    - returns: maintenance cost
    - used 4x in the base game, e.g. `base/content/gui/gui/statistics/statistic_depots.tl:98`: `return api.engine.util.maintenance.calcMaintenanceForSubconstruction(entity)`

## UtilVehicle (record)

```teal
global record UtilVehicle
```
Access: `api.engine.util.vehicle`

Vehicle queries (position, speed, costs, capacities, maintenance penalties, problems), available as `api.engine.util.vehicle`.

- `findBestLineAndDepotForVehicle : function(vehicleEntity : Engine.Entity) : {Engine.Entity, Engine.Entity}`  (call as `api.engine.util.vehicle.findBestLineAndDepotForVehicle(…)`): Searches the best line and depot for the vehicle
    - param `vehicleEntity`: the vehicle for which to search the best line and depot
    - returns: line and depot entities, if found
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:8553`: `return api.engine.util.vehicle.findBestLineAndDepotForVehicle(-vehicleEntity-1)`
- `findBestDepotForLine : function(carrier : Carrier, transportModes : {TransportMode}, lineEntity : Engine.Entity, position : Vec3f) : Engine.Entity`  (call as `api.engine.util.vehicle.findBestDepotForLine(…)`): Searches the best depot for the line or position (in case of air vehicles)
    - param `carrier`: the carrier to search for
    - param `transportModes`: the transport modes to check for connectivity
    - param `lineEntity`: the line to search the depot for
    - param `position`: the position from which to search (air/water vehicles only)
    - returns: best depot, if any
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:1951`: `depotEntity = api.engine.util.vehicle.findBestDepotForLine(`
- `getSpeed : function(vehicleEntity : Engine.Entity) : number`  (call as `api.engine.util.vehicle.getSpeed(…)`): Get speed of the vehicle
    - param `vehicleEntity`: vehicle entity
    - returns: speed
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:566`: `if  api.engine.util.vehicle.getSpeed(vehicleEntity) == 0 then`
- `getPosition : function(vehicleEntity : Engine.Entity) : Vec3f`  (call as `api.engine.util.vehicle.getPosition(…)`): Get the position of the vehicle
    - param `vehicleEntity`: vehicle entity
    - returns: world position
    - used 10x in the base game, e.g. `base/content/mission/mission/tasks/assign_vehicle/visit_stations_ordered.tl:49`: `local pos = api.engine.util.vehicle.getPosition(vehicle)`
- `calcPathPosData : function(vehicleEntity : Engine.Entity) : PathPosData`  (call as `api.engine.util.vehicle.calcPathPosData(…)`): Get the position, direction and curvature of the vehicle
    - param `vehicleEntity`: vehicle entity
    - returns: position and direction information for the vehicle
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:122`: `local pathPosData = api.engine.util.vehicle.calcPathPosData(plane.entity)`
- `getDirection : function(vehicleEntity : Engine.Entity) : Vec3f`  (call as `api.engine.util.vehicle.getDirection(…)`): Get the direction of the vehicle
    - param `vehicleEntity`: vehicle entity
    - returns: direction
    - used 1x in the base game, e.g. `mods/release/urbangames_campaign_mission_03/content/mission/mission/tasks/fatal_maintenance/fatal_maintenance.tl:51`: `local dir = api.engine.util.vehicle.getDirection(vehicleEntityId)`
- `getVehiclesByCarrier : function(carrier : Carrier) : {Engine.Entity}`  (call as `api.engine.util.vehicle.getVehiclesByCarrier(…)`): Gets all vehicles using by a given carrier
    - param `carrier`: kind of carrier
    - returns: list of vehicle entities

    ```lua
    for _, vehicle in ipairs(api.engine.util.vehicle.getVehiclesByCarrier(api.type["enum"].Carrier.ROAD)) do
      local tv = api.engine.getComponent(vehicle, api.type.ComponentType.TRANSPORT_VEHICLE)
    end
    ```

    - used 4x in the base game, e.g. `base/content/mission/mission/tasks/assign_vehicle/assign_vehicle.tl:57`: `local vehicles = api.engine.util.vehicle.getVehiclesByCarrier(carrier)`
- `getVehicles : function() : {Engine.Entity}`  (call as `api.engine.util.vehicle.getVehicles(…)`): Gets all vehicles
    - returns: list of vehicle entities
    - used 3x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:321`: `local vehicles = api.engine.util.vehicle.getVehicles()`
- `getRunningCost : function(vehicleEntity : Engine.Entity) : number`  (call as `api.engine.util.vehicle.getRunningCost(…)`): Get running cost of the vehicle per year
    - param `vehicleEntity`: vehicle entity
    - returns: running cost
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:162`: `return {mathutil.round(api.engine.util.vehicle.getRunningCost(param.entity)), "RunningCost"}`
- `getDepreciatedValue : function(vehicleEntity : Engine.Entity) : integer`  (call as `api.engine.util.vehicle.getDepreciatedValue(…)`): Get vehicle depreciated value
    - param `vehicleEntity`: vehicle entity
    - returns: depreciated value
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:3451`: `return api.engine.util.vehicle.getDepreciatedValue(entity)`
- `getPartPrice : function(part : TransportVehiclePart) : integer`  (call as `api.engine.util.vehicle.getPartPrice(…)`): Get vehicle part value
    - param `part`: transport vehicle part
    - returns: value
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:3690`: `totalPrice = totalPrice + api.engine.util.vehicle.getPartPrice(part)`
- `getLength : function(modelAsset : Model) : number`  (call as `api.engine.util.vehicle.getLength(…)`): Get length of the vehicle model
    - param `modelAsset`: model asset
    - returns: length
- `getAllCargoForCargoTypeSet : function(cargoTypeSet : CargoTypeSet) : {CargoTypeId}`  (call as `api.engine.util.vehicle.getAllCargoForCargoTypeSet(…)`): Get cargo in CargoTypeSet
    - param `cargoTypeSet`: cargo type set
    - returns: CargoTypeId in the set
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3747`: `local cargoIds : { CargoTypeId } = api.engine.util.vehicle.getAllCargoForCargoTypeSet(definition.cargoTypeSet)`
- `getVehicleLoadedCargoTypes : function(vehicleEntity : Engine.Entity) : {CargoTypeId}`  (call as `api.engine.util.vehicle.getVehicleLoadedCargoTypes(…)`): Get list of cargo types loaded in the vehicle
    - param `vehicleEntity`: vehicle entity
    - returns: list of cargo types loaded in the vehicle
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo.tl:76`: `local cargoTypesLoaded = api.engine.util.vehicle.getVehicleLoadedCargoTypes(params.entity)`
- `getVehicleProblems : function() : {{{Engine.Entity}, VehicleProblem}}`  (call as `api.engine.util.vehicle.getVehicleProblems(…)`): Get vehicles with problems
    - returns: vehicle entity and problem
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:262`: `local vehiclesAndProblems = api.engine.util.vehicle.getVehicleProblems()`
- `getMaintenanceEmissionPenalty : function(maintenanceState : number) : number`  (call as `api.engine.util.vehicle.getMaintenanceEmissionPenalty(…)`): Get vehicle emission penalty in percent
    - param `maintenanceState`: maintenance state
    - returns: emission penalty in percent
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_util.tl:237`: `local penality = api.engine.util.vehicle.getMaintenanceEmissionPenalty(vehicle.maintenanceState)`
- `getMaintenanceComfortPenalty : function(maintenanceState : number) : number`  (call as `api.engine.util.vehicle.getMaintenanceComfortPenalty(…)`): Get vehicle comfort penalty in percent
    - param `maintenanceState`: maintenance state
    - returns: comfort penalty in percent
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_util.tl:246`: `local penality = api.engine.util.vehicle.getMaintenanceComfortPenalty(vehicle.maintenanceState)`
- `getMaintenanceTopSpeedPenalty : function(topSpeedMaintenanceFactorPenaltyPerLevel : number, maintenanceState : number) : number`  (call as `api.engine.util.vehicle.getMaintenanceTopSpeedPenalty(…)`): Get vehicle top speed penalty in percent
    - param `topSpeedMaintenanceFactorPenaltyPerLevel`: penalty factor per level
    - param `maintenanceState`: maintenance state
    - returns: top speed penalty in percent
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_util.tl:252`: `local penality = api.engine.util.vehicle.getMaintenanceTopSpeedPenalty(topSpeedMaintenanceFactorPenaltyPerLevel, vehicle.maintenanceState)`
- `getMaintenanceRunningCostPenalty : function(maintenanceState : number) : number`  (call as `api.engine.util.vehicle.getMaintenanceRunningCostPenalty(…)`): Get vehicle running cost penalty in percent
    - param `maintenanceState`: maintenance state
    - returns: running cost penalty in percent
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_util.tl:257`: `local penality = api.engine.util.vehicle.getMaintenanceRunningCostPenalty(vehicle.maintenanceState)`
- `getVehicleType : function(vehicleEntity : Engine.Entity) : integer`  (call as `api.engine.util.vehicle.getVehicleType(…)`): Get the vehicle's type
VehicleIconType
    - param `vehicleEntity`: vehicle entity
    - returns: vehicle type
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:404`: `local vehicleType = api.engine.util.vehicle.getVehicleType(vehicleEntity) as VehicleIconType`
- `getVehicleCapacities : function(vehicleEntity : Engine.Entity) : {integer}`  (call as `api.engine.util.vehicle.getVehicleCapacities(…)`): Get vehicle capacities
    - param `vehicleEntity`: vehicle entity
    - returns: vehicle capacities
    - used 2x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:689`: `local capacities = locationParams.showEmpty and api.engine.util.vehicle.getVehicleCapacities(vehicle) or transportVehicle.config.capacities`
- `getVehicleUnsetCapacities : function(vehicleEntity : Engine.Entity) : {{integer}}`  (call as `api.engine.util.vehicle.getVehicleUnsetCapacities(…)`): Get vehicle unset capacities
    - param `vehicleEntity`: vehicle entity
    - returns: vehicle unset capacities for each unset capacity
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:944`: `local unsetTypes = api.engine.util.vehicle.getVehicleUnsetCapacities(vehicleEntity)`
- `getVehicleMaintenanceState : function(vehicleEntity : Engine.Entity) : number`  (call as `api.engine.util.vehicle.getVehicleMaintenanceState(…)`): Get vehicle maintenance state
    - param `vehicleEntity`: vehicle entity
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:310`: `local maintenanceState = api.engine.util.vehicle.getVehicleMaintenanceState(vehicleEntity)`

## UtilStation (record)

```teal
global record UtilStation
```
Access: `api.engine.util.station`

Station and station group usage and capacity helpers, available as `api.engine.util.station`.

- `calculateStationUsage : function(station : Engine.Entity) : StationUsage`  (call as `api.engine.util.station.calculateStationUsage(…)`): Get station usages
    - param `station`: station entity
    - returns: total usage, overflow and pool capacity for station
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:213`: `= api.engine.util.station.calculateStationUsage(entity)`
- `calculateStationTerminalUsage : function(station : Engine.Entity, terminalIndex : integer) : UtilLine.TerminalCapacityUsage`  (call as `api.engine.util.station.calculateStationTerminalUsage(…)`): Get station terminal usages
    - param `station`: station entity
    - param `terminalIndex`: terminal index
    - returns: capacity and usage for a terminal (or the pool if terminal is -1)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:645`: `return api.engine.util.station.calculateStationTerminalUsage(stationId, terminalIndex)`
- `calculateStationGroupCargo : function(stationGroup : Engine.Entity, flatTerminalIndex : integer) : StationGroupCapacityUsage`  (call as `api.engine.util.station.calculateStationGroupCargo(…)`): Get station group cargo usage
    - param `stationGroup`: station group entity
    - param `flatTerminalIndex`: terminal index
    - returns: capacity and usage + overflow for a StationGroup

    ```lua
    api.engine.forEachEntityWithComponent(function(stationGroup)
      if api.engine.util.station.calculateStationGroupCargo(stationGroup, -1).overflow then
        print("overcrowded", stationGroup)
      end
    end, api.type.ComponentType.STATION_GROUP)
    ```

    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:599`: `local overflowing = api.engine.util.station.calculateStationGroupCargo(stationGroupEntity, -1).overflow`
- `isStationOfType : function(stationEntity : Engine.Entity, cargo : boolean) : boolean`  (call as `api.engine.util.station.isStationOfType(…)`): Check if station supports cargo/passengers
    - param `stationEntity`: station entity
    - param `cargo`: check for cargo support or passenger support
    - returns: whether station supports cargo/passengers
    - used 9x in the base game, e.g. `base/content/gui/gui/construction/constructionHudIconMaster.tl:106`: `if api.engine.util.station.isStationOfType(station, cargo) then`
- `findStationGroupWithOverlappingCatchmentOfDifferentCarriers : function() : {{Engine.Entity, Engine.Entity}}`  (call as `api.engine.util.station.findStationGroupWithOverlappingCatchmentOfDifferentCarriers(…)`): Check for stations with overlapping catchment areas
    - returns: list of station group entity pairs

### UtilStation.StationUsage (record)

```teal
record StationUsage
```

Usage and capacity of a station, returned by `calculateStationUsage`.

Notes:

The base game's emission script computes the station's total capacity as `poolCapacity + terminalCapacity`
and its load as `totalUsed` divided by that.

- `totalUsed : integer`: Number of persons or cargo items currently using the station.
- `overflow : integer`
- `poolCapacity : integer`: Capacity of the station's waiting pool (waiting hall).
- `terminalCapacity : integer`: Capacity of the station's terminals.

### UtilStation.StationGroupCapacityUsage (record)

```teal
record StationGroupCapacityUsage
```

Capacity, usage and overflow state of a station group, returned by `calculateStationGroupCargo`.

Notes:

The base game calls `calculateStationGroupCargo(stationGroup, -1)` and uses `overflow` to raise the
station-overcrowding notification.

- `equals : function(StationGroupCapacityUsage, StationGroupCapacityUsage) : boolean`: Returns `true` when both `StationGroupCapacityUsage` values are equal.
- `capacity : integer`: Total capacity.
- `waitingHallCapacity : integer`: Capacity of the waiting halls (station pools).
- `used : integer`: Number of persons or cargo items currently waiting.
- `overflow : boolean`: True when the station group is overcrowded.

## UtilTown (record)

```teal
global record UtilTown
```
Access: `api.engine.util.town`

Town statistics (capacities, reachability, emissions, happiness, deliveries) and town lookups, available as `api.engine.util.town`.

- `computeWeightedTargetTownBuildingCapacityPerType : function(townEntity : Engine.Entity) : {{integer, integer, integer, integer}}`  (call as `api.engine.util.town.computeWeightedTargetTownBuildingCapacityPerType(…)`): Get weighted target level to capacities distribution for each land use type for the town
    - param `townEntity`: town entity
    - returns: level to capacities distribution
    - used 2x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:522`: `local weightedLevelToCapacitiesDistribution = api.engine.util.town.computeWeightedTargetTownBuildingCapacityPerType(entityId)`
- `computeUnweightedTargetTownBuildingCapacityPerType : function(townEntity : Engine.Entity) : {{integer, integer, integer, integer}}`  (call as `api.engine.util.town.computeUnweightedTargetTownBuildingCapacityPerType(…)`): Get unweighted target level to capacities distribution for each land use type for the town
    - param `townEntity`: town entity
    - returns: level to capacities distribution
    - used 1x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:521`: `local targetLevelToCapacitiesDistribution = api.engine.util.town.computeUnweightedTargetTownBuildingCapacityPerType(entityId)`
- `computeActualTownBuildingCapacityPerType : function(townEntity : Engine.Entity) : {{integer, integer, integer, integer}}`  (call as `api.engine.util.town.computeActualTownBuildingCapacityPerType(…)`): Get actual level to capacities distribution for each land use type for the town
    - param `townEntity`: town entity
    - returns: level to capacities distribution
    - used 1x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:523`: `local actualLevelToCapacitiesDistribution = api.engine.util.town.computeActualTownBuildingCapacityPerType(entityId)`
- `getLandUse2CargoTypes : function() : {LandUseType : {CargoTypeId}}`  (call as `api.engine.util.town.getLandUse2CargoTypes(…)`): Get town available cargo needs
    - returns: map of land use to cargo types
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:477`: `return api.engine.util.town.getLandUse2CargoTypes()`
- `getTownReachability : function(townEntity : Engine.Entity) : TownReachability`  (call as `api.engine.util.town.getTownReachability(…)`): Get town reachability
    - param `townEntity`: town entity
    - returns: reachability data
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:154`: `return api.engine.util.town.getTownReachability(townEntity)`
- `getTownEmission : function(townEntity : Engine.Entity) : TownEmission`  (call as `api.engine.util.town.getTownEmission(…)`): Get town emission in power
    - param `townEntity`: town entity
    - returns: emission data in power
- `getTownEmissionDB : function(townEntity : Engine.Entity) : TownEmission`  (call as `api.engine.util.town.getTownEmissionDB(…)`): Get town emission in decibel
    - param `townEntity`: town entity
    - returns: emission data in decibel
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:158`: `local townEmission = api.engine.util.town.getTownEmissionDB(townEntity)`
- `getTownCapacityUsage : function(townEntity : Engine.Entity) : {TownCapacityUsage}`  (call as `api.engine.util.town.getTownCapacityUsage(…)`): Get town capacity usage
    - param `townEntity`: town entity
    - returns: capacity usage per land use type
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:149`: `local usages : {UtilTown.TownCapacityUsage} = api.engine.util.town.getTownCapacityUsage(townEntity)`
- `getTownHappinessStats : function(townEntity : Engine.Entity, numTopLines : integer) : TownHappinessStats`  (call as `api.engine.util.town.getTownHappinessStats(…)`): Get number of unhappy people travelling by lines to or from the town
    - param `townEntity`: town entity
    - param `numTopLines`: number of top lines to consider for the stat
    - returns: number of unhappy and total people travelling by lines
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:475`: `local unhappyAndTotal = api.engine.util.town.getTownHappinessStats(entity, 0)`
- `getTownDeliveriesStats : function(townEntity : Engine.Entity, interval : integer, perLine : boolean, perCargoType : boolean) : {Engine.Entity : {CargoTypeId : {integer, integer}}}`  (call as `api.engine.util.town.getTownDeliveriesStats(…)`): Get town cargo deliveries stats
    - param `townEntity`: town entity
    - param `interval`: time interval in miliseconds for the stats
    - param `perLine`: whether to get stats per line
    - param `perCargoType`: whether to get stats per cargo type
    - returns: delivery stats
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:510`: `local deliveryStats = api.engine.util.town.getTownDeliveriesStats(entity, mathutil.round(api.util.getDefaultYearDuration() / 2), false, false)`
- `getTownStockCargo : function(townEntity : Engine.Entity) : {CargoTypeId : {integer, integer}}`  (call as `api.engine.util.town.getTownStockCargo(…)`): Obtains information about towns and cargo
    - param `townEntity`: the entity of the town
    - returns: map of cargo type to stock and capacity
    - used 1x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:446`: `local townStockCargo = api.engine.util.town.getTownStockCargo(entityAndRevisionState:old().entity)`
- `getTerrainBrushVolumeImpact : function(proposal : Proposal) : {Engine.Entity : {number, number}}`  (call as `api.engine.util.town.getTerrainBrushVolumeImpact(…)`): Get the impact of using terrain brush on towns reputation
    - param `proposal`: the terrain change proposal
    - returns: the impact change for volume added and volume removed for each town
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1245`: `local townBrushVolumeImpactChanges = api.engine.util.town.getTerrainBrushVolumeImpact(proposal)`
- `getTerrainAlignmentImpact : function(proposalData : ProposalData) : {Engine.Entity : {number, number}}`  (call as `api.engine.util.town.getTerrainAlignmentImpact(…)`): Get the terrain alignment impact on towns reputation
    - param `proposalData`: the proposal data
    - returns: the impact change for volume added and volume removed for each town
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1261`: `local terrainAlignmentImpactChanges : { Engine.Entity : { number, number }} = api.engine.util.town.getTerrainAlignmentImpact(proposalData);`
- `getAutoRemovalEntityImpact : function(proposalData : ProposalData) : {Engine.Entity : {number, number}}`  (call as `api.engine.util.town.getAutoRemovalEntityImpact(…)`): Get the impact from auto removal of trees and rocks on towns reputation
    - param `proposalData`: the proposal data
    - returns: the impact change for removed trees and rocks for each town
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1187`: `local autoRemovalEntityChanges : {Engine.Entity : {number, number}} = api.engine.util.town.getAutoRemovalEntityImpact(proposalData)`
- `getTownNoisePerDistrict : function(townEntity : Engine.Entity, landUseTypes : {LandUseType}) : {LandUseType : {number, integer}}`  (call as `api.engine.util.town.getTownNoisePerDistrict(…)`): Get town noise per district
    - param `townEntity`: town entity
    - param `landUseTypes`: land use types to check
    - returns: average noise and affected capacity per district
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1796`: `local noisePerDistrict = api.engine.util.town.getTownNoisePerDistrict(params.entity, town_util.getNoiseLandUseTypes())`
- `getImpactFromDistance : function(distance : number) : number`  (call as `api.engine.util.town.getImpactFromDistance(…)`): Get the impact for reputation the distance has for towns
    - param `distance`: distance value
    - returns: the impact from the distance
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1147`: `townStats.removedTrees = townStats.removedTrees + api.engine.util.town.getImpactFromDistance(distance) * reputationFactor`
- `computeTownsTrafficSpeedMap : function(samplesCoefficient : number) : {Engine.Entity : {number, {integer}}}`  (call as `api.engine.util.town.computeTownsTrafficSpeedMap(…)`): Get traffic speed for all towns
    - param `samplesCoefficient`: coefficient to adjust the number of samples of streets used for traffic rating calculation
    - returns: traffic speed and number of streets per congestion level
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:346`: `return api.engine.util.town.computeTownsTrafficSpeedMap(samplesCoefficient)[townEntity]`
- `getTownProblems : function() : {{{Engine.Entity}, TownProblem}}`  (call as `api.engine.util.town.getTownProblems(…)`): Get towns with problems
    - returns: town entity and problem
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:198`: `local entitiesAndProblems = api.engine.util.town.getTownProblems()`
- `isNeighbourTown : function(townEntity1 : Engine.Entity, townEntity2 : Engine.Entity) : boolean`  (call as `api.engine.util.town.isNeighbourTown(…)`): Check if town is neighbor town
    - param `townEntity1`: town entity
    - param `townEntity2`: town entity
    - returns: whether town is neighbor
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_growth.script.tl:669`: `if api.engine.util.town.isNeighbourTown(townBuildingFrom.town, townBuilding.town) then`
- `getClosestTown : function(position : Vec3f) : Engine.Entity`  (call as `api.engine.util.town.getClosestTown(…)`): Get closest town to position
    - param `position`: world position
    - returns: closest town entity
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:212`: `local townEntity = api.engine.util.town.getClosestTown(pos)`
- `getRandomTownPosition : function(townEntity : Engine.Entity, seed : integer) : Vec2f`  (call as `api.engine.util.town.getRandomTownPosition(…)`): Get a random 2d point inside the voronoi cell of the town
    - param `townEntity`: the town in which to search
    - param `seed`: a random seed for the generation
    - returns: a point inside the town voronoi cell
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:52`: `return api.engine.util.town.getRandomTownPosition(customState.townEntity, math.random(0, 10000))`
- `isLineStoppingInTown : function(lineEntity : Engine.Entity, townEntity : Engine.Entity) : boolean`  (call as `api.engine.util.town.isLineStoppingInTown(…)`): Check if Line has at least 1 stop in the town's area
    - param `lineEntity`: line entity
    - param `townEntity`: town entity
    - returns: whether line stops in town
    - used 5x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:293`: `if transportVehicle.line and api.engine.util.town.isLineStoppingInTown(transportVehicle.line, selectedTownBuilding.town) then`
- `getTownLineUsage : function(townEntity : Engine.Entity) : number`  (call as `api.engine.util.town.getTownLineUsage(…)`): Get town line usage
    - param `townEntity`: town entity
    - returns: fraction of people using lines in the town
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_growth.script.tl:377`: `local lineUsage = api.engine.util.town.getTownLineUsage(townEntity)`
- `getSourceToDestinationCount : function(townEntitiy : Engine.Entity, destinationIsTownMap : {Engine.Entity : boolean}, selectLandUse : integer) : {Engine.Entity : DestinationUseCount}`  (call as `api.engine.util.town.getSourceToDestinationCount(…)`): Get town destinations table content (work, shop and line usage info)
    - param `townEntitiy`: town entity
    - param `destinationIsTownMap`: map of entity to isTown (destinations of town)
    - param `selectLandUse`: 0 (Usage), 1 (Shopping), 2 (Work)
    - returns: destination use counts
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:483`: `return api.engine.util.town.getSourceToDestinationCount(param.entity, destinationsIsTownMap, landUseType == 0 and 0 or (landUseType -1))`
- `getTownDistrictCenter : function(entity : Engine.Entity, landUseType : LandUseType) : Vec3f`  (call as `api.engine.util.town.getTownDistrictCenter(…)`): Get the district center of the town entity
    - param `entity`: town entity
    - param `landUseTypes`: land use types to check
    - returns: destination use counts
    - used 1x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_util.tl:121`: `return api.engine.util.town.getTownDistrictCenter(town, landUseType)`

### UtilTown.TownReachability (record)

```teal
record TownReachability
```

Reachability values of a town by public and private transport, for commercial and industrial destinations; returned by `getTownReachability`.

Notes:

The town statistics window shows "Public Transport" as the rounded mean of `commercialPublic` and
`industrialPublic`, and "Private Transport" as the mean of `commercialPrivate` and `industrialPrivate`.

- `commercialPrivate : integer`: Reachability of commercial destinations by private transport.
- `commercialPublic : integer`: Reachability of commercial destinations by public transport.
- `industrialPrivate : integer`: Reachability of industrial destinations by private transport.
- `industrialPublic : integer`: Reachability of industrial destinations by public transport.

### UtilTown.TownCapacityUsage (record)

```teal
record TownCapacityUsage
```

Used and total building capacity of one land use type in a town; `getTownCapacityUsage` returns a list indexed by `LandUseType` value + 1.

Example:
```lua
local usages = api.engine.util.town.getTownCapacityUsage(townEntity)
local residential = usages[api.type["enum"].LandUseType.RESIDENTIAL + 1]
print("population", residential.capacity)
```

- `used : integer`: Used capacity.
- `capacity : integer`: Total capacity; for the residential land use the base game shows it as the town's population.

### UtilTown.TownEmission (record)

```teal
record TownEmission
```

Noise and pollution of a town plus its area; `getTownEmission` returns power values, `getTownEmissionDB` decibel values.

- `noise : number`: Noise emission (power or decibel, depending on the function that returned it).
- `pollution : number`: Pollution emission (power or decibel, depending on the function that returned it).
- `areaSquareKm : number`: Area of the town, in square kilometres.
    - notes: The base game divides it into 16 m x 16 m cells to compute the average pollution per cell for the town's pollution rating.

### UtilTown.DestinationUseCount (record)

```teal
record DestinationUseCount
```

Number of trips from a town to one destination and how many of them use lines, returned per destination by `getSourceToDestinationCount`.

- `count : integer`: Total number of trips to the destination.
- `linesUseCount : integer`: Number of those trips that use lines; the town window shows `linesUseCount / count` as a percentage.

### UtilTown.TownHappinessStats (record)

```teal
record TownHappinessStats
```

Happiness statistics of people travelling in, to and from a town, returned by `getTownHappinessStats`. Every `{integer, integer}` pair is `{unhappy, total}`.

Notes:

The base game's happiness rating weights `travellingInside` twice and adds `travellingTo` and
`travellingFrom` (residents and non-residents) to get the share of unhappy passengers.

Example:
```lua
local s = api.engine.util.town.getTownHappinessStats(townEntity, 0)
local unhappy = s.travellingInside[1] * 2 + s.travellingTo.resident[1] + s.travellingTo.nonResident[1]
  + s.travellingFrom.resident[1] + s.travellingFrom.nonResident[1]
local total = s.travellingInside[2] * 2 + s.travellingTo.resident[2] + s.travellingTo.nonResident[2]
  + s.travellingFrom.resident[2] + s.travellingFrom.nonResident[2]
```

- `travellingInside : {integer, integer}`: `{unhappy, total}` for passengers travelling within the town.
- `travellingTo : ByResident`: `{unhappy, total}` for passengers travelling to the town, by residency.
- `travellingFrom : ByResident`: `{unhappy, total}` for passengers travelling from the town, by residency.
- `atBuilding : {integer, integer}`: `{unhappy, total}` for residents of the town currently at buildings.
- `byCar : {integer, integer}`: `{unhappy, total}` for residents of the town travelling by car.
- `walking : {integer, integer}`: `{unhappy, total}` for residents of the town walking.
- `byLine : {{Engine.Entity, ByResident}}`: Per-line statistics as `{lineEntity, ByResident}` pairs for the top `numTopLines` lines; the town window labels an entry with a non-positive entity as "Other".

#### UtilTown.TownHappinessStats.ByResident (record)

```teal
record ByResident
```

`{unhappy, total}` counts split into residents of the town and non-residents.

- `resident : {integer, integer}`: `{unhappy, total}` for residents of the town.
- `nonResident : {integer, integer}`: `{unhappy, total}` for people who do not live in the town.

## UtilCargo (record)

```teal
global record UtilCargo
```
Access: `api.engine.util.cargo`

Cargo and passenger quality data (delivery time, happiness) for lines, vehicles, stations and stocks, available as `api.engine.util.cargo`.

- `isGoodQuality : function(passengerOrCargoEntity : Engine.Entity) : boolean`  (call as `api.engine.util.cargo.isGoodQuality(…)`): Check if a cargo is in time / a person is happy
    - param `sim`: cargo or person entity
    - returns: true if good quality
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/towns.script.tl:425`: `local isOnTime = api.engine.util.cargo.isGoodQuality(onCalcTicketPriceParam.simEntity)`
- `getCargoQualityDataForLine : function(lineEntity : Engine.Entity, cargoTypeId : CargoTypeId) : UtilCargo.CargoQualityData`  (call as `api.engine.util.cargo.getCargoQualityDataForLine(…)`): Get cargo type quality data in a line.
    - param `lineEntity`: line entity
    - param `cargoTypeId`: cargo type (passengers allowed)
    - returns: data about cargo quality
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:717`: `local qualityData = api.engine.util.cargo.getCargoQualityDataForLine(lineEntity, cargoTypeId)`
- `getCargoQualityDataForVehicle : function(vehicleEntity : Engine.Entity, cargoTypeId? : CargoTypeId) : UtilCargo.CargoQualityData`  (call as `api.engine.util.cargo.getCargoQualityDataForVehicle(…)`): Get cargo type quality data in a vehicle.
    - param `vehicleEntity`: vehicle entity
    - param `cargoTypeId`: cargo type (passengers allowed)
    - returns: data about cargo quality
    - used 5x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:624`: `local cargoQualityData = api.engine.util.cargo.getCargoQualityDataForVehicle(params.vehicleEntity)`
- `getCargoQualityDataAtStation : function(stationEntity : Engine.Entity, cargoTypeId : CargoTypeId) : UtilCargo.CargoQualityData`  (call as `api.engine.util.cargo.getCargoQualityDataAtStation(…)`): Get all persons waiting at a station plus the number of unhappy ones.
    - param `stationEntity`: station entity
    - param `cargoTypeId`: the cargo type (passengers allowed)
    - returns: cargo quality data
- `getCargoQualityDataAtStationGroup : function(stationGroupEntity : Engine.Entity, CargoTypeId) : UtilCargo.CargoQualityData`  (call as `api.engine.util.cargo.getCargoQualityDataAtStationGroup(…)`): Get all persons waiting at station group plus the number of unhappy ones.
    - param `stationGroupEntity`: station group entity
    - param `cargoTypeId`: the cargo type (passengers allowed)
    - returns: cargo quality data
    - used 4x in the base game, e.g. `base/content/gui/gui/layers/layer_public_transport.tl:114`: `local waitingAtGroup = api.engine.util.cargo.getCargoQualityDataAtStationGroup(params.entity, cargo_util.getPassengerCargoTypeId())`
- `getCargoQualityDataAtStop : function(lineEntity : Engine.Entity, stopIndex : integer, CargoTypeId) : UtilCargo.CargoQualityData`  (call as `api.engine.util.cargo.getCargoQualityDataAtStop(…)`): Get all persons waiting at a certain line stop plus the number of unhappy ones.
    - param `lineEntity`: line entity
    - param `stopIndex`: index of stop
    - param `cargoTypeId`: the cargo type (passengers allowed)
    - returns: cargo quality data
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:705`: `local qualityData = api.engine.util.cargo.getCargoQualityDataAtStop(lineEntity, stopIndex0, passengerCargoTypeId)`
- `getCargoQualityDataAtTerminal : function(stationGroupEntity : Engine.Entity, stationTerminal : StationTerminal, CargoTypeId) : UtilCargo.CargoQualityData`  (call as `api.engine.util.cargo.getCargoQualityDataAtTerminal(…)`): Get all persons waiting at a certain terminal plus the number of unhappy ones.
    - param `stationGroupEntity`: station group entity
    - param `stationTerminal`: the affected station and terminal indices
    - param `cargoTypeId`: the cargo type (passengers allowed)
    - returns: cargo quality data
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_public_transport.tl:107`: `local waitingAtTerminal = api.engine.util.cargo.getCargoQualityDataAtTerminal(params.entity, stationSelectionDetails.stationTerminal, cargo_util.getPassengerCar`
- `getCargoQualityDataForStockList : function(stockListEntity : Engine.Entity, CargoTypeId) : UtilCargo.CargoQualityData`  (call as `api.engine.util.cargo.getCargoQualityDataForStockList(…)`): Get stock list cargo type quality.
    - param `stockListEntity`: stockList entity
    - param `cargoTypeId`: the cargo type (passengers allowed but doesn't make sense)
    - returns: cargo quality data
    - used 2x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:262`: `local qualityData = api.engine.util.cargo.getCargoQualityDataForStockList(entity, cargoTypeId)`
- `getCargoQualityDataForStock : function(stockListEntity : Engine.Entity, StockId, ?CargoTypeId) : UtilCargo.CargoQualityData`  (call as `api.engine.util.cargo.getCargoQualityDataForStock(…)`): Get stock cargo type quality (over all cargo types).
    - param `stockListEntity`: stockList entity
    - param `stockId`: stock id inside the stock list
    - param `cargoTypeId`: the cargo type (passing nil will aggregate over all cargo types)
    - returns: cargo quality data
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:127`: `local qualityData = api.engine.util.cargo.getCargoQualityDataForStock(entity, stockId)`
- `getSummarizedCargoQualityDataForStockList : function(stockListEntity : Engine.Entity) : UtilCargo.SummarizedCargoQualityData`  (call as `api.engine.util.cargo.getSummarizedCargoQualityDataForStockList(…)`): Get stock list cargo type quality (over all cargo types).
    - param `stockListEntity`: stockList entity
    - returns: cargo quality data
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:367`: `summarizedInfo = api.engine.util.cargo.getSummarizedCargoQualityDataForStockList(stockListEntity)`
- `getSummarizedCargoQualityDataForLine : function(lineEntity : Engine.Entity) : UtilCargo.SummarizedCargoQualityData`  (call as `api.engine.util.cargo.getSummarizedCargoQualityDataForLine(…)`): Get cargo type quality summary.
    - param `lineEntity`: line entity
    - returns: cargo quality data
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:369`: `summarizedInfo = api.engine.util.cargo.getSummarizedCargoQualityDataForLine(params.lineEntity)`
- `getSummarizedCargoQualityDataForVehicle : function(vehicleEntity : Engine.Entity) : UtilCargo.SummarizedCargoQualityData`  (call as `api.engine.util.cargo.getSummarizedCargoQualityDataForVehicle(…)`): Get cargo type quality summary.
    - param `vehicleEntity`: vehicle entity
    - returns: cargo quality data
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:371`: `summarizedInfo = api.engine.util.cargo.getSummarizedCargoQualityDataForVehicle(params.vehicleEntity)`
- `getNumCargoPerTypeInVehicle : function(vehicleEntity : Engine.Entity) : {integer}`  (call as `api.engine.util.cargo.getNumCargoPerTypeInVehicle(…)`): Get total amount of cargo loaded in the vehicle
    - param `vehicleEntity`: vehicle entity
    - returns: number of cargo items per type

### UtilCargo.CargoQualityData (record)

```teal
record CargoQualityData
```

Quality summary of a set of passengers or cargo items (happiness for passengers, remaining delivery time for cargo), returned by the `getCargoQualityData*` functions.

Example:
```lua
local q = api.engine.util.cargo.getCargoQualityDataForVehicle(vehicleEntity)
print(q.countBad .. " of " .. q.countTotal .. " unhappy or late")
if q.averageQuality ~= nil then
  print(api.util.toStringPercentPrecision(q.averageQuality, 0))
end
```

- `clone : function(CargoQualityData) : CargoQualityData`: Returns a copy of the quality data.
- `countBad : integer`: Number of unhappy passengers or late cargo items.
- `countTotal : integer`: Total number of passengers or cargo items counted.
- `averageQuality : number | nil`: Average quality (fraction, shown as a percentage), or nil when there is nothing to average; shown as average happiness for passengers and average remaining delivery time for cargo.
- `isVeryBad : boolean`: True when the quality is very bad; the base game uses it to show a stronger warning style.

### UtilCargo.SummarizedCargoQualityData (record)

```teal
record SummarizedCargoQualityData
```

Quality data split into passengers and cargo, returned by the `getSummarizedCargoQualityData*` functions.

- `clone : function(SummarizedCargoQualityData) : SummarizedCargoQualityData`: Returns a copy of the summarized quality data.
- `passengers : CargoQualityData`: Quality data of the passengers.
- `cargo : CargoQualityData`: Quality data of the cargo.

## UtilConstruction (record)

```teal
global record UtilConstruction
```
Access: `api.engine.util.construction`

Construction helpers (owning construction entity, slots, construction results, cost multipliers), available as `api.engine.util.construction`.

- `getNumAndFilledAutoFillSlots : function(entity : Engine.Entity) : {integer, integer}`  (call as `api.engine.util.construction.getNumAndFilledAutoFillSlots(…)`): Get total number of auto fill slots and number of filled auto fill slots.
    - param `entity`: construction entity
    - returns: num filled auto fill slots
    - used 1x in the base game, e.g. `base/content/gui/gui/main/industry_util.tl:383`: `local numAndFilled = api.engine.util.construction.getNumAndFilledAutoFillSlots(constructionEntity)`
- `getConstructionEntity : function(depotEntity : Engine.Entity) : Engine.Entity`  (call as `api.engine.util.construction.getConstructionEntity(…)`): Gets construction entity for the entity.
    - param `depotEntity`: the entity for which the containing construction should be fetched
    - returns: construction entity
    - used 5x in the base game, e.g. `base/content/mission/mission/mission_sim.script.tl:778`: `local actual = api.engine.util.construction.getConstructionEntity(k)`
- `getSlotTypes : function(entity : Engine.Entity) : {string}`  (call as `api.engine.util.construction.getSlotTypes(…)`): Get slot types of construction.
    - param `entity`: construction entity
    - returns: slot types
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:4116`: `local slotTypes = api.engine.util.construction.getSlotTypes(refEntity:get())`
- `getConstructionResult : function(constructionResName : ResName, constructionTemplate : integer, parameters : table) : ConstructionResult`  (call as `api.engine.util.construction.getConstructionResult(…)`): Get construction result for a given construction.
    - param `constructionResName`: resource name of the construction
    - param `constructionTemplate`: template index
    - param `parameters`: parameters for the updateFn
    - returns: construction result
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3650`: `local constructionResult = api.engine.util.construction.getConstructionResult(constr, templateIndex, paramsAll)`
- `getGlobalConstructionParams : function() : {string : number}`  (call as `api.engine.util.construction.getGlobalConstructionParams(…)`): Get global construction params
    - returns: global construction parameters
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3663`: `local paramsAll = api.engine.util.construction.getGlobalConstructionParams()`
- `getCostMultiplierForYear : function(costMultipliers : {integer : number}, currentYear : integer) : number`  (call as `api.engine.util.construction.getCostMultiplierForYear(…)`): Get cost multiplier for year
    - param `costMultipliers`: cost multipliers by year
    - param `currentYear`: current year
    - returns: cost multiplier
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3592`: `local costScaleFactor = api.engine.util.construction.getCostMultiplierForYear(baseConfig.treeCostYearMultipliers, year)`
- `findSegmentsToDelete : function(constructionEntity : Engine.Entity, keepIndustryRoads? : boolean) : {Engine.Entity}`  (call as `api.engine.util.construction.findSegmentsToDelete(…)`): Get street segments to remove when a construction entity is removed
    - param `constructionEntity`: the construction entity we want to find segments for
    - param `keepIndustryRoads`: (default = false) whether to keep the segments that are built with the industry
    - returns: the street segments to remove
    - used 3x in the base game, e.g. `base/content/gui/gui/main/industry_util.tl:566`: `local segments = api.engine.util.construction.findSegmentsToDelete(construction)`

## UtilFinance (record)

```teal
global record UtilFinance
```
Access: `api.engine.util.finance`

Account, balance and finance chart helpers, available as `api.engine.util.finance`.

- `getAccountChart : function(entity : Engine.Entity, config : ChartConfig) : ChartResult`  (call as `api.engine.util.finance.getAccountChart(…)`): Get chart for account (income and costs).
    - param `entity`: entity with account component
    - param `config`: chart config
    - returns: chart result
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:196`: `return api.engine.util.finance.getAccountChart(player, config)`
- `getDebtChart : function(entity : Engine.Entity, config : ChartConfig, currentDebt : integer) : ChartResult`  (call as `api.engine.util.finance.getDebtChart(…)`): Get chart for debts.
    - param `entity`: entity with account component
    - param `config`: chart config
    - returns: chart result
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:219`: `local result = api.engine.util.finance.getDebtChart(player, config, currentDebt)`
- `calculateBalance : function(entities : {Engine.Entity}, startTime : integer, endTime : integer, maintenanceIncomeOnly : boolean, maintenanceTye? : JournalEntry.Maintenance) : integer`  (call as `api.engine.util.finance.calculateBalance(…)`): Get (summed) balance of entities
    - param `entities`: (having an account)
    - param `begin`: time (interval)
    - param `end`: time (interval)
    - param `true`: if only maintenance/income entries are to be considered
    - param `only`: consider for given maintenance type (use nil for all)
    - returns: (summed) balance over given interval's time
    - used 5x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:2177`: `return api.engine.util.finance.calculateBalance({construction}, fromTime, toTime, true, params.maintenanceType)`
- `calculateEarnings : function(playerEntity : Engine.Entity) : integer`  (call as `api.engine.util.finance.calculateEarnings(…)`): Get the earnings from beginning of the year to now
    - param `playerEntity`: player entity
    - returns: earnings
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.script.tl:11`: `return api.engine.util.finance.calculateEarnings(api.engine.util.getPlayer())`
- `computeFinanceTable : function(playerEntity : Engine.Entity, config : ChartConfig) : FinanceData`  (call as `api.engine.util.finance.computeFinanceTable(…)`): Get current state of fincance table.
    - param `playerEntity`: player entity
    - param `config`: config of chart behind the table
    - returns: struct holding all the relevant values
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:573`: `return api.engine.util.finance.computeFinanceTable(player, config)`
- `getPlayersBalance : function(playerEntity : Engine.Entity) : integer`  (call as `api.engine.util.finance.getPlayersBalance(…)`): Get balance of the player's bank account.
    - param `playerEntity`: the player entity
    - returns: balance or nil, if player has won the lottery and money is infinite

    ```lua
    local balance = api.engine.util.finance.getPlayersBalance(api.engine.util.getPlayer())
    local infiniteMoney = balance == nil
    ```

    - used 17x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/account.tl:16`: `return api.engine.util.finance.getPlayersBalance(api.engine.util.getPlayer()) ~= nil`
- `calcIncomeSince : function(time : number, playerEntity : Engine.Entity) : number`  (call as `api.engine.util.finance.calcIncomeSince(…)`): The income since the specified time (non-inclusive).
    - param `playerEntity`: the player entity
    - param `current`: game time
    - returns: The income
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:202`: `local incomeSinceLastUpdate = api.engine.util.finance.calcIncomeSince(achievementState.lastIncomeUpdateTime, player)`
- `getLastIncomeTime : function(playerEntity : Engine.Entity) : integer`  (call as `api.engine.util.finance.getLastIncomeTime(…)`): The time of the last income.
    - param `playerEntity`: the player entity
    - returns: The time of the last income entry
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:121`: `local lastIncomeUpdateTime = api.engine.util.finance.getLastIncomeTime(player)`
- `getFirstIncomeTime : function(playerEntity : Engine.Entity) : integer`  (call as `api.engine.util.finance.getFirstIncomeTime(…)`): The time of the first income.
    - param `playerEntity`: the player entity
    - returns: The time of the first income entry
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:360`: `local minTimestamp = api.engine.util.finance.getFirstIncomeTime(player)`

## UtilEmission (record)

```teal
global record UtilEmission
```
Access: `api.engine.util.emission`

Emission values and noise / pollution emitters near settlements, available as `api.engine.util.emission`.

- `getCurrentEmission : function(entity : Engine.Entity, type : Engine.Component.EmissionGrid.Type) : number`  (call as `api.engine.util.emission.getCurrentEmission(…)`): Get current emission at entity.
    - param `entity`: the entity with Emission component
    - param `type`: emission type
    - returns: emission value
- `getPollutionEmittersInSettlementArea : function(entity : Engine.Entity) : {{Engine.Entity, number}}`  (call as `api.engine.util.emission.getPollutionEmittersInSettlementArea(…)`): Get all pollution emitters in settlement area.
    - param `entity`: the town entity
    - returns: list of emitters with emission values
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1765`: `local emitters = pollution and api.engine.util.emission.getPollutionEmittersInSettlementArea(townEntity) or`
- `getNoiseEmittersNearSettlementLandUses : function(entity : Engine.Entity, distance : number, landUseTypes : {LandUseType}) : {{Engine.Entity, number}}`  (call as `api.engine.util.emission.getNoiseEmittersNearSettlementLandUses(…)`): Get all noise emitters near land uses of a settlement.
    - param `entity`: the settlement entity
    - param `distance`: maximum distance
    - param `landUseTypes`: land use type filter
    - returns: list of emitters with emission values
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1766`: `api.engine.util.emission.getNoiseEmittersNearSettlementLandUses(townEntity, 150, town_util.getNoiseLandUseTypes())`

## UtilHeadquarters (record)

```teal
global record UtilHeadquarters
```
Access: `api.engine.util.headquarters`

Company transport statistics and company value, available as `api.engine.util.headquarters`.

- `getTransportedData : function() : TransportationStats`  (call as `api.engine.util.headquarters.getTransportedData(…)`): Obtains the transportation data.
    - returns: some transportation statistics for the company
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_display_transported_plugin/game_bar_display_transported.script.tl:16`: `local transportedData = api.engine.util.headquarters.getTransportedData()`
- `getCompaniesValue : function() : CompanyValue`  (call as `api.engine.util.headquarters.getCompaniesValue(…)`): Obtains the companie's value
    - returns: company value data
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:463`: `local companyValue = api.engine.util.headquarters.getCompaniesValue()`

## UtilIndustry (record)

```teal
global record UtilIndustry
```
Access: `api.engine.util.industry`

Industry productivity, closing industries and economy cargo dependencies, available as `api.engine.util.industry`.

- `getIndustryProductivityInfo : function(entity : Engine.Entity) : IndustryProductivityInfo`  (call as `api.engine.util.industry.getIndustryProductivityInfo(…)`): Get the industry's productivity
    - param `entity`: the industry entity
    - returns: productivity info
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emissions.script.tl:154`: `= api.engine.util.industry.getIndustryProductivityInfo(entity)`
- `getClosingIndustries : function() : {{Engine.Entity, ResName}}`  (call as `api.engine.util.industry.getClosingIndustries(…)`): Get list of industries that are closing
    - returns: list of industry entities and their resource names
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:439`: `local closingIndustries = api.engine.util.industry.getClosingIndustries()`
- `getEconomyCargoDependencies : function(economyId : integer) : {string : {string : number}}`  (call as `api.engine.util.industry.getEconomyCargoDependencies(…)`): Get the cargo dependencies of an economy
    - param `economyId`: the economy id
    - returns: A map from cargoTag to a map of industryTag to count

### UtilIndustry.IndustryProductivityInfo (record)

```teal
record IndustryProductivityInfo
```

Production state of an industry, returned by `getIndustryProductivityInfo`.

Notes:

The base game's emission script raises an industry's emission level by one step for `producing`, and one
more step each for `boostFromRule` and `boostFromPersonCapacity` while it is producing.

- `producing : boolean`: True while the industry is producing.
- `boostFromRule : boolean`: True when production is boosted by the industry's rule.
- `boostFromPersonCapacity : boolean`: True when production is boosted by its workers (person capacity).

## UtilProposal (record)

```teal
global record UtilProposal
```
Access: `api.engine.util.proposal`

Builds `Proposal`s (industry spawn and extension, removal, replacement, bridges and tunnels, traffic lights, refresh), available as `api.engine.util.proposal`.

- `makeProposalData : function(proposal : Proposal, context? : Context) : ProposalData`  (call as `api.engine.util.proposal.makeProposalData(…)`): Processes a `Proposal` into its `ProposalData` (collision info, error state, costs, construction data, terrain preview and the parallel-strip and link proposals).
    - param `proposal`: The proposal to process.
    - param `context`: Optional construction context (`api.type.Context`).
    - returns: The processed proposal data.
- `makeIndustrySpawnProposal : function(typeTag : string, townEntity? : Engine.Entity) : Proposal`  (call as `api.engine.util.proposal.makeIndustrySpawnProposal(…)`): Tries to generate a proposal for randomly placing an industry.
    - param `townEntity`: the town designating the target voronoi cell (or nil to spawn anywhere in the world)
    - param `industryTypeTag`: The type of the industry
    - returns: the generated proposal, or nil
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:202`: `proposal = api.engine.util.proposal.makeIndustrySpawnProposal(industryType, (not entity_util.entityChanged0(pending.townEntity)) and pending.townEntity.entity o`
- `makeSegmentsRemoveProposal : function(segmentsToRemove : {Engine.Entity}) : Proposal`  (call as `api.engine.util.proposal.makeSegmentsRemoveProposal(…)`): Makes a proposal to remove segments
    - param `segmentsToRemove`: list of segments to remove
    - returns: the generated proposal
    - used 3x in the base game, e.g. `base/content/gui/gui/main/industry_util.tl:584`: `local removeEdgesProposal = api.engine.util.proposal.makeSegmentsRemoveProposal(segmentsToRemove)`
- `createProposalRemove : function(entity : Engine.Entity, context : Context) : Proposal`  (call as `api.engine.util.proposal.createProposalRemove(…)`): Tries to generate a proposal to remove a construction
    - param `entity`: the entity to remove
    - param `context`: construction context
    - returns: the generated proposal, or nil
    - used 2x in the base game, e.g. `base/content/gui/gui/main/industry_util.tl:590`: `local removeIndustryProposal = api.engine.util.proposal.createProposalRemove(industryConstruction, context)`
- `createProposalReplaceConstruction : function(entity : Engine.Entity, params : table, newResName? : ResName) : Proposal`  (call as `api.engine.util.proposal.createProposalReplaceConstruction(…)`): Replaces an existing construction with a new one
    - param `entity`: the entity to replace
    - param `params`: parameters for the new construction
    - param `newResName`: resource name of the new construction (if nil, the same as the old one is used)
    - returns: the generated proposal, or nil
    - used 5x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:1295`: `local proposal = api.engine.util.proposal.createProposalReplaceConstruction(paramState:old().entity, modifiedParams as table)`
- `createIndustryProposal : function(seed : string, industryRectangle : IndustryInfo, context : Context) : Proposal`  (call as `api.engine.util.proposal.createIndustryProposal(…)`): Tries to generate a proposal to build an industry
    - param `seed`: random seed
    - param `industryRectangle`: industry rectangle data
    - param `context`: construction context
    - returns: the generated proposal, or nil
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:1579`: `local newIndustryProposal = api.engine.util.proposal.createIndustryProposal(seed or "", industry, context)`
- `createBridgeOrTunnelProposal : function(parallelStripEntity : Engine.Entity, newTypeIndex : integer) : Proposal`  (call as `api.engine.util.proposal.createBridgeOrTunnelProposal(…)`): Creates a bridge or tunnel proposal for changing the type of a parallel strip entity.
    - param `parallelStripEntity`: parallel strip entity
    - param `newTypeIndex`: new resource type index for bridge or tunnel
    - returns: the generated proposal
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/bridge_and_tunnel.tl:69`: `local proposal = api.engine.util.proposal.createBridgeOrTunnelProposal(param.keyEntity, id)`
- `createDoubleSlipSwitchProposal : function(entity : Engine.Entity, doubleSlipSwitch : boolean) : Proposal`  (call as `api.engine.util.proposal.createDoubleSlipSwitchProposal(…)`): Creates a double slip switch proposal for changing the switch type of a base node configuration entity.
    - param `entity`: base node entity
    - param `doubleSlipSwitch`: double slip switch true to create double slip switch, false for single slip switch
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:648`: `local proposal = api.engine.util.proposal.createDoubleSlipSwitchProposal(param.keyEntity, (value == "yes") and true or false)`
- `createTrafficLightProposal : function(entity : Engine.Entity, trafficLightConfig : Engine.Component.TrafficLightConfig, trafficLightPreference : TrafficLightPreference) : Proposal`  (call as `api.engine.util.proposal.createTrafficLightProposal(…)`): Creates a proposal that changes the traffic light configuration of a base node
    - param `entity`: base node entity
    - param `trafficLightConfig`: the configuration of traffic light, including phases and type
    - param `trafficLightPreference`: whether a traffic light should be generated or not (or automtically decided)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:56`: `local proposal = api.engine.util.proposal.createTrafficLightProposal(param.keyEntity, baseNodeState.trafficLightConfig, baseNodeState.trafficLightPreference)`
- `getBridgeOrTunnelRelatedRevisions : function(parallelStripEntity : Engine.Entity) : {Engine.Entity : Engine.Revision}`  (call as `api.engine.util.proposal.getBridgeOrTunnelRelatedRevisions(…)`): Gets bridge or tunnel related revisions for a parallel strip entity.
    - param `parallelStripEntity`: parallel strip entity
    - returns: map of entity to revision
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/bridge_and_tunnel.tl:67`: `local revisions = api.engine.util.proposal.getBridgeOrTunnelRelatedRevisions(param.keyEntity)`
- `didBridgeOrTunnelRelatedRevisionsChange : function(parallelStripEntity : Engine.Entity, revisions : {Engine.Entity : Engine.Revision}) : boolean`  (call as `api.engine.util.proposal.didBridgeOrTunnelRelatedRevisionsChange(…)`): Checks if bridge or tunnel related revisions changed for a parallel strip entity.
    - param `parallelStripEntity`: parallel strip entity
    - param `revisions`: map of entity to revision
    - returns: true if any related revision changed
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/bridge_and_tunnel.tl:117`: `elseif api.engine.util.proposal.didBridgeOrTunnelRelatedRevisionsChange(param.keyEntity, revisionsRef:get()) then`
- `makeReforestationProposal : function(seed : integer, numTilesToUpdate : integer, maxTreesPerTile : integer) : Proposal`  (call as `api.engine.util.proposal.makeReforestationProposal(…)`): Creates a reforestation proposal.
    - param `seed`: random seed
    - param `numTilesToUpdate`: maximum number of tiles being updated
    - param `maxTreesPerTile`: maximum number of trees planted per tile
    - returns: the generated proposal
    - used 1x in the base game, e.g. `base/content/terrain/reforestation.script.tl:55`: `local proposal = api.engine.util.proposal.makeReforestationProposal(seed, numTilesToUpdate, MaxTreesPerTile)`
- `refreshConstruction : function(entity : Engine.Entity, constructionFilepath? : string) : Proposal`  (call as `api.engine.util.proposal.refreshConstruction(…)`): Refreshes a construction. This returns a proposal that needs to be applied with a `makeWorldBuildProposalCmd`
    - param `entity`: the entity with a construction to refresh
    - param `constructionFilepath`: the construction to refresh the entity with
    - returns: a proposal for the refreshed construction

    ```lua
    local proposal = api.engine.util.proposal.refreshConstruction(constructionEntity)
    api.cmd.sendCommand(api.cmd.makeWorldBuildProposalCmd(proposal, nil, true, true))
    ```

    - used 7x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:357`: `local proposal = api.engine.util.proposal.refreshConstruction(e)`
- `replaceSegment : function(entity : Engine.Entity, streetTemplate? : ResName) : Proposal`  (call as `api.engine.util.proposal.replaceSegment(…)`): Refreshes a street/track segment (base edge). This returns a proposal that needs to be applied with a `makeWorldBuildProposalCmd`
    - param `entity`: the entity with a construction to refresh
    - param `streetTemplate`: the street template file path to refresh the entity with (optionally). Use with caution.
    - returns: a proposal for the refreshed segment
    - used 1x in the base game, e.g. `mods/release/urbangames_campaign_mission_03/content/init/init/init.script.tl:31`: `local proposal = api.engine.util.proposal.replaceSegment(entity)`
- `checkProposalConnectsNodes : function(startNode : Engine.Entity, startEdge : Engine.Entity, targetNode : Engine.Entity, streetProposal : Proposal.StreetProposal, isApply : boolean, maxDepth? : number) : boolean`  (call as `api.engine.util.proposal.checkProposalConnectsNodes(…)`): Check if a street proposal connects the start node to the target node.
    - param `startNode`: the starting base node entity
    - param `startEdge`: the starting base edge entity
    - param `targetNode`: the target node entity
    - param `streetProposal`: the street proposal to check
    - param `isApply`: whether the proposal is being applied
    - param `maxDepth`: (optional) specify a maximum distance which is traversed in the network
    - returns: true if the proposal connects the nodes
    - used 1x in the base game, e.g. `base/content/mission/mission/tasks/connect_nodes/connect_nodes.tl:72`: `return api.engine.util.proposal.checkProposalConnectsNodes(config.startNode, config.startEdge, config.targetNode, streetProposal, isApply)`
- `makeIndustryExtendProposal : function(industry : Engine.Entity, trySlots : integer, numExtendSlots : integer) : {Proposal, {integer}}`  (call as `api.engine.util.proposal.makeIndustryExtendProposal(…)`): Create proposal to extend industry
    - param `industry`: industry entity
    - param `trySlots`: number of slots to try extending to
    - param `numExtendSlots`: number of slots to extend
    - returns: the proposal and (if it is colliding) indices of colliding subconstructions
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/industries/industries.script.tl:15`: `local proposalAndCollidingSubconstructions = api.engine.util.proposal.makeIndustryExtendProposal(entityId, 0, 1)`
- `createSubconstructionLotList : function(subconstruction : Proposal.Subconstruction, transf : Mat4f) : Engine.Component.LotList`  (call as `api.engine.util.proposal.createSubconstructionLotList(…)`): Builds the ground lots (terrain texture areas) of a proposal subconstruction, transformed by the given matrix.
    - param `subconstruction`: Subconstruction of a proposal, e.g. `proposal.toAdd_native:at(1).construction.subconstructions_native:at(i)`.
    - param `transf`: Transformation applied to the lots, e.g. the construction's `transf`.
    - returns: A `LotList` whose `lots` hold `vertices` and `triangles` (0-based vertex indices, in groups of three).
    - notes: The base game uses it to draw the footprint of colliding industry-extension subconstructions that have no colliders.

    ```lua
    local lotList = api.engine.util.proposal.createSubconstructionLotList(subconstruction, constructionTransf)
    for _, lot in ipairs(lotList.lots) do
      for i = 1, #lot.triangles, 3 do
        local a = lot.vertices[lot.triangles[i] + 1]
        -- ...
      end
    end
    ```

    - used 1x in the base game, e.g. `base/content/gui/gui/main/industry_util.tl:685`: `local lotList = api.engine.util.proposal.createSubconstructionLotList(subconstruction, constructionTransf)`
- `getConstructionArea : function(proposal : Proposal) : number`  (call as `api.engine.util.proposal.getConstructionArea(…)`): Calculate the area of new constructions in the proposal.
    - param `proposal`: the proposal to calculate the area for
    - returns: the approximate area of new constructions in the proposal in square meters
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1257`: `local area = api.engine.util.proposal.getConstructionArea(proposal)`

## EngineUtil (record)

```teal
global record EngineUtil
```
Access: `api.engine.util`

Collection of engine specific utils grouped by system

Forward declaration of the `EngineUtil` record (defined in `api/engine/util.d.tl`), the type of `api.engine.util`.

- `getPlayer : function() : Engine.Entity`  (call as `api.engine.util.getPlayer(…)`): Obtains the player entity.
    - returns: entity id

    ```lua
    local balance = api.engine.util.finance.getPlayersBalance(api.engine.util.getPlayer())
    ```

    - used 116x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:120`: `local player = api.engine.util.getPlayer()`
- `getWorld : function() : Engine.Entity`  (call as `api.engine.util.getWorld(…)`): Obtains the world entity.
    - returns: entity id
    - used 169x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:161`: `local world = api.engine.util.getWorld()`
- `getCalendarDate : function(gameTime : number) : Date`  (call as `api.engine.util.getCalendarDate(…)`): Return the calendar date of the gametime, assuming current ingame speed.
    - param `gameTime`: gametime for which to obtain the date
    - returns: calendar date matching the game time at the current speed
    - used 27x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:373`: `local date = api.engine.util.getCalendarDate(val)`
- `getYear : function() : integer`  (call as `api.engine.util.getYear(…)`): Return year of the gametime
    - returns: calendar year matching the game time at the current speed
    - used 19x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:72`: `local year = api.engine.util.getYear()`
- `hasStartedAsMission : function() : boolean`  (call as `api.engine.util.hasStartedAsMission(…)`): Returns true if currently an active mission, started from the menu, is running
    - returns: true if mission is running
    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:140`: `if api.engine.util.hasStartedAsMission() then`
- `getEntityName : function(entity : Engine.Entity) : string`  (call as `api.engine.util.getEntityName(…)`): Get name of an (existing) entity, computing it from the base construction if needed.
    - returns: string Entity name, or nil
    - used 125x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:935`: `local playerName = api.engine.util.getEntityName(playerEntity)`
- `getEntityNameStem : function(entity : Engine.Entity) : {string, Engine.Entity}`  (call as `api.engine.util.getEntityNameStem(…)`): Get name stem of an (existing) entity
    - returns: string Entity name and Entity where the stem came from (both can be nil)
    - used 6x in the base game, e.g. `base/content/mission/mission/name_util.tl:17`: `local stem = api.engine.util.getEntityNameStem(id)[1] or "<nil>"`
- `cargo : UtilCargo`: `UtilCargo` helpers: cargo and passenger quality data.
- `construction : UtilConstruction`: `UtilConstruction` helpers: construction entities, slots and results.
- `chart : UtilChart`: `UtilChart` helpers: chart labels and values.
- `finance : UtilFinance`: `UtilFinance` helpers: balances, earnings and finance charts.
- `headquarters : UtilHeadquarters`: `UtilHeadquarters` helpers: company statistics and value.
- `line : UtilLine`: `UtilLine` helpers: line capacity, problems and issues.
- `logbook : UtilLogbook`: `UtilLogbook` helpers: logbook values and charts.
- `maintenance : UtilMaintenance`: `UtilMaintenance` helpers: maintenance costs.
- `octree : UtilOctree`: `UtilOctree` helpers: spatial queries within a circle.
- `parallelStrip : UtilParallelStrip`: `UtilParallelStrip` helpers: bridge and tunnel information.
- `pathfinding : UtilPathfinding`: `UtilPathfinding` helpers: path searches.
- `station : UtilStation`: `UtilStation` helpers: station usage and capacity.
- `stock : UtilStock`: `UtilStock` helpers: stock and production statistics.
- `town : UtilTown`: `UtilTown` helpers: town statistics and lookups.
- `transport : UtilTransport`: `UtilTransport` helpers: transport edges and ports.
- `vehicle : UtilVehicle`: `UtilVehicle` helpers: vehicle queries.
- `industry : UtilIndustry`: `UtilIndustry` helpers: industry productivity and economy data.
- `proposal : UtilProposal`: `UtilProposal` helpers: building proposals.
- `emission : UtilEmission`: `UtilEmission` helpers: emission values and emitters.
- `formatAge : function(time0 : integer, time1 : integer) : string`  (call as `api.engine.util.formatAge(…)`): Format age of a vehicle using current ingame speed.
    - param `begin`: timestamp of the time interval
    - param `end`: timestamp of the time interval
    - returns: formatted output, e.g. "6 years"
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:223`: `ageString = api.engine.util.formatAge(`
