# gui.main.cargo_util

Definition file: `base/tealdef/gui/main/cargo_util.d.tl` (in the game folder).

Functions that collect cargo data for the UI: stock contents of industries, warehouses and
town buildings, cargo loaded in vehicles and on lines, cargo type icons, names and colours,
and cargo flow counts. All functions read engine state and do not change it. Load it with
`ug_require "/gui/main/cargo_util.tl"`.

## CargoUtil (record)

```teal
global record CargoUtil
```
Load: `local cargo_util = ug_require "::/gui/main/cargo_util.tl" as CargoUtil`

Read-only helpers for cargo types, stocks and vehicle cargo in the UI.

Notes:

Cargo type ids (`CargoTypeId`) are 0-based. Engine arrays indexed by cargo type, such as
`TransportVehicle.config.capacities`, are 1-based Lua arrays, so entry `i` belongs to cargo
type `i - 1`.

- `getCargoTypesCount : function(producedOnly : boolean) : integer`  (call as `cargo_util.getCargoTypesCount(…)`): producedOnly: only current types in game if true, all available from rep if false
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/account.tl:35`: `return cargo_util.getCargoTypesCount(true)`
- `getMaxCaroTypeId : function() : integer`  (call as `cargo_util.getMaxCaroTypeId(…)`): Returns the highest cargo type id in `api.res.cargoTypeRep.getAll(true)`.
    - notes: The name is spelled `Caro` in the game files.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:282`: `return cargo_util.getMaxCaroTypeId() + 1`
- `getPassengerCargoTypeId : function() : CargoTypeId`  (call as `cargo_util.getPassengerCargoTypeId(…)`): Returns the cargo type id of passengers (`api.res.cargoTypeRep.getPassengerCargoTypeId()`).
    - used 41x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_cargo_lifetime.tl:243`: `cargo[cargo_util.getPassengerCargoTypeId()] = nil -- passengers are not welcome here`
- `calculateStockConfigs : function(entity : Engine.Entity, max? : integer, calculateQuality? : boolean, sortByType? : boolean, showEmpty? : boolean, groupStocks? : boolean, producedOnly? : boolean) : {CargoUtil.StockConfig}, integer, boolean`  (call as `cargo_util.calculateStockConfigs(…)`): Groups the stocks of a stock list entity and computes stored amount, capacity, tendency and optionally quality per group.
    - param `entity`: Stock list entity (e.g. `Industry.stockList` or `Warehouse.stockList`).
    - param `max`: Maximum number of cargo icons the caller wants to show per row; adjusted and returned as the second result.
    - param `calculateQuality`: If `true`, fills `quality` and `numSpoiled`.
    - param `sortByType`: If `true`, sorts the groups by stock type, then by cargo types.
    - param `showEmpty`: If `false`, leaves out empty stocks (default `true`).
    - param `groupStocks`: If `false`, makes one entry per stock instead of grouping stocks by type and cargo types (default `true`).
    - param `producedOnly`: If `true`, only lists cargo types that are produced in the current game.
    - returns: 1. List of `StockConfig`.
2. The adjusted maximum icon count.
3. `true` if at least one group is not an output stock.

    - notes: Returns an empty list if the entity does not exist.
    - used 7x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:508`: `configs, maxIcons, invert = cargo_util.calculateStockConfigs(`
- `sortConfigsByStockType : function({CargoUtil.StockConfig}, boolean) : {CargoUtil.StockConfig}`  (call as `cargo_util.sortConfigsByStockType(…)`): input-, output-, storage stock
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:741`: `stockConfigs = cargo_util.sortConfigsByStockType(stockConfigs, false)`
- `calculateStockCargoCount : function(Engine.Entity, ?boolean, ?{StockId}, ?boolean) : {StockCargoCount}`  (call as `cargo_util.calculateStockCargoCount(…)`): stock ids: [optional] must be stocks of given industry/warehouse entity,
if given count is calculated only for those, otherwise for all stocks of given industry/warehouse
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:1760`: `local stockCargoCount = cargo_util.calculateStockCargoCount(entity, false, stockConfig.ids)`
- `calculateSortedVehicleCargoInfo : function(Engine.Entity, calculateQuality? : boolean) : {VehicleCargo}`  (call as `cargo_util.calculateSortedVehicleCargoInfo(…)`): Lists the cargo capacities of a vehicle, sorted by capacity (highest first), then by cargo type.
    - param `#1`: Vehicle entity with a `TRANSPORT_VEHICLE` component.
    - param `calculateQuality`: If `true`, fills `hasGoodCargo` and `hasBadCargo`.
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistic_vehicles.tl:453`: `local cargoInfo : {CargoUtil.VehicleCargo} = cargo_util.calculateSortedVehicleCargoInfo(key)`

get value usable to for sorting (so that cargoType is most significant)
- `calculateSortedVehicleCargoInfoCompareValue : function(Engine.Entity) : {{CargoTypeId, integer, integer}}`  (call as `cargo_util.calculateSortedVehicleCargoInfoCompareValue(…)`)
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistic_vehicles.tl:363`: `{ name = _("Cargo"), recipe = VehicleCargoTypesCell, getCompareValue = cargo_util.calculateSortedVehicleCargoInfoCompareValue, weight = 2.4, },`
- `calculateSortedLineCargoInfo : function(lineEntity : Engine.Entity, calculateQuality? : boolean) : {CargoUtil.VehicleCargo}`  (call as `cargo_util.calculateSortedLineCargoInfo(…)`): Like `calculateSortedVehicleCargoInfo`, summed over all vehicles of a line.
    - param `lineEntity`: Line entity.
    - param `calculateQuality`: If `true`, fills `hasGoodCargo` and `hasBadCargo`.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:2556`: `local cargoInfo : {CargoUtil.VehicleCargo} = cargo_util.calculateSortedLineCargoInfo(key)`
- `calculateSortedLineCargoInfoCompareValue : function(lineEntity : Engine.Entity) : {{CargoTypeId, integer, integer}}`  (call as `cargo_util.calculateSortedLineCargoInfoCompareValue(…)`): Returns `{cargoType, fill, capacity}` for each cargo type of a line's vehicles, in the same order as `calculateSortedLineCargoInfo`. Used as a sort key.
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistic_lines.tl:407`: `{ name = _("Cargo"), recipe = LineCargoTypesCell, getCompareValue = cargo_util.calculateSortedLineCargoInfoCompareValue, weight = 2 },`
- `hasLineVehicleWithCapacity : function(lineEntity : Engine.Entity, cargoTypeId : CargoTypeId) : boolean`  (call as `cargo_util.hasLineVehicleWithCapacity(…)`): Tells whether any vehicle of a line can carry the cargo type, based on `TransportVehicle.config.allCaps`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_passengers/deliver_passengers.script.tl:75`: `if cargo_util.hasLineVehicleWithCapacity(lineEntity, passengerCargoTypeId) then`
- `calculateVehicleCargoFill : function({Engine.Entity}, integer) : integer`  (call as `cargo_util.calculateVehicleCargoFill(…)`): Returns the number of loaded items of one cargo type summed over the given vehicles.
    - param `#1`: Vehicle entities.
    - param `#2`: Cargo type id.
- `getSortedProducedCargoTypes : function(locationParams : CargoLocationParams, sortOrder : CargoListSortOrder, passengersFirst? : boolean, cargoTypeFilter? : (function(CargoTypeId) : boolean), allLineCapacities? : boolean) : {CargoTypeId}`  (call as `cargo_util.getSortedProducedCargoTypes(…)`): Lists the cargo types present at a location, limited to cargo types produced in the current game, sorted by the given order.
    - param `locationParams`: Locations to count.
    - param `sortOrder`: Sort order.
    - param `passengersFirst`: If `true`, moves passengers to the front.
    - param `cargoTypeFilter`: Function that returns `true` for cargo types to keep.
    - param `allLineCapacities`: Passed to `api.engine.util.line.getLineCapacityUsages` for line locations.
    - notes: In the game's implementation `cargoTypeFilter` only takes effect in the shortcut case (only `vehicleEntity` set, `showEmpty` off, sort order `CAPACITY`); in all other cases the list is returned unfiltered.
    - used 7x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:2208`: `local cargoTypes =  cargo_util.getSortedProducedCargoTypes(locationParams, sortOrder, true, nil)`
- `getConfiguredStopCargoTypes : function(stopParams : CargoStopParams, passengersFirst? : boolean) : {CargoTypeId}`  (call as `cargo_util.getConfiguredStopCargoTypes(…)`): Lists the cargo types set to load at a line stop (`stopConfig.load`).
    - param `stopParams`: Line and stop.
    - param `passengersFirst`: If `true`, moves passengers to the front.
    - returns: Cargo type ids, or an empty list if the stop does not exist.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_react_util.tl:681`: `sortedCargoTypes = cargo_util.getConfiguredStopCargoTypes(stopParams, true)`
- `getNumberOfCargoInStock : function(stockListOwnerEntity : Engine.Entity) : integer`  (call as `cargo_util.getNumberOfCargoInStock(…)`): Returns the total number of cargo items in the stock list of an industry, warehouse or town building.
    - returns: Item count, `0` if the entity has no stock list.
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistics.tl:79`: `local storedCargoCount = cargo_util.getNumberOfCargoInStock(params.entity)`
- `getCargoTypeInfo : function(CargoTypeId, CargoLocationParams) : CargoTypeInfo`  (call as `cargo_util.getCargoTypeInfo(…)`): Collects amount, capacity, quality and, for vehicles, loading state of one cargo type at the given locations.
    - param `#1`: Cargo type id.
    - param `#2`: Locations to count; the values of all set locations are added up.
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:2215`: `local info : CargoUtil.CargoTypeInfo = cargo_util.getCargoTypeInfo(cargoType, locationParams)`
- `getSpecializationClasses : function(stockListEntity : Engine.Entity) : {string}`  (call as `cargo_util.getSpecializationClasses(…)`): Returns the cargo class names a stock list is specialized for.
    - notes: Collects the included cargo classes of storage stocks that only filter by included cargo classes (no excluded classes, no included cargo types).
- `getCargoClassesOfType : function(CargoTypeId) : {CargoClassId}`  (call as `cargo_util.getCargoClassesOfType(…)`): Returns the ids of the cargo classes of a cargo type. Unknown class tags are skipped.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:38`: `local cargoClassIds = cargo_util.getCargoClassesOfType(cargoTypeId)`
- `getWarehouseCountIndicatorTexture : function(integer) : string`  (call as `cargo_util.getWarehouseCountIndicatorTexture(…)`): Returns the arrow icon for a stock tendency value.
    - param `#1`: Tendency from `-3` to `3`; values beyond are clamped.
    - returns: Image path, e.g. `::/gui/entity_window/icons/stock_increasing2.tga`; `stock_stagnating.tga` for `0`.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:197`: `local indicatorIconPath = params.tendency ~= nil and cargo_util.getWarehouseCountIndicatorTexture(params.tendency) or nil`
- `getCargoColorById : function(CargoTypeId) : Vec4f`  (call as `cargo_util.getCargoColorById(…)`): Returns the colour of a cargo type with alpha `1`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:338`: `local col = cargo_util.getCargoColorById(cargoTypeId)`
- `getCargoColorByType : function(string) : Vec4f`  (call as `cargo_util.getCargoColorByType(…)`): Returns the colour of a cargo type given by its name in the cargo type repository.
    - notes: Returns the darkest neutral colour of `default_colors.gres` for `nil`.
- `getCargoIconById : function(CargoTypeId) : string`  (call as `cargo_util.getCargoIconById(…)`): Returns the icon path of a cargo type.
    - used 19x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:349`: `unfilteredState.icons[unfilteredIndex] = cargo_util.getCargoIconById(cargoTypeId)`
- `getCargoIconByType : function(string) : string`  (call as `cargo_util.getCargoIconByType(…)`): Returns the icon path of a cargo type given by its name in the cargo type repository; `""` for `nil`.
- `getCargoClassIconById : function(CargoClassId) : string`  (call as `cargo_util.getCargoClassIconById(…)`): Returns the icon path of a cargo class.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:41`: `children[#children + 1] = cargo_react_util.makeIcon(class and class or "", cargo_util.getCargoClassIconById(cargoClassId), cargoClass.name)`
- `getMixedCargoIcon : function() : string`  (call as `cargo_util.getMixedCargoIcon(…)`): Returns the icon for mixed or any cargo (`::/cargos/icons/cargo_mixed.tga`).
    - used 6x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3360`: `path = cargo_util.getMixedCargoIcon(),`
- `getPassengerCargoIcon : function() : string`  (call as `cargo_util.getPassengerCargoIcon(…)`): Returns the passenger icon (`::/cargos/passengers/passengers.tga`).
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3357`: `path = cargo_util.getPassengerCargoIcon(),`
- `getCargoNameById : function(CargoTypeId) : string`  (call as `cargo_util.getCargoNameById(…)`): Returns the display name of a cargo type.
    - used 17x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_notification_prospection.script.tl:36`: `local cargoName = cargoTypeId >= 0 and cargo_util.getCargoNameById(cargoTypeId) or _("Random Resource")`
- `getCargoNameByType : function(string) : string`  (call as `cargo_util.getCargoNameByType(…)`): Returns the display name of a cargo type given by its name in the cargo type repository; `""` for `nil`.
- `getStockListEntityFromOwner : function(stockListOwnerEntity : Engine.Entity) : Engine.Entity`  (call as `cargo_util.getStockListEntityFromOwner(…)`): Returns the stock list entity of a town building, industry or warehouse.
    - returns: The `stockList` of the owner's `TOWN_BUILDING`, `INDUSTRY` or `WAREHOUSE` component, or `nil`.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:365`: `local stockListEntity = cargo_util.getStockListEntityFromOwner(params.stockListOwnerEntity)`
- `getInputOutputStocksForStation : function(stationEntity : Engine.Entity, skipStorage? : boolean) : {CargoTypeId : boolean}, integer, {CargoTypeId : boolean}, integer`  (call as `cargo_util.getInputOutputStocksForStation(…)`): Collects the input and output cargo types of all stock lists in a station's catchment area.
    - param `stationEntity`: Station entity.
    - param `skipStorage`: See `getInputOutputStocksForStockList`.
    - returns: 1. Set of input cargo types.
2. Number of input entries (one per stock and cargo type, summed over all stock lists).
3. Set of output cargo types.
4. Number of output entries.

    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/cargofilter_window.tl:346`: `local inputCargoTypes, inputCount, outputCargoTypes, _outputCount = cargo_util.getInputOutputStocksForStation(stationGroup.stations[stop.station + 1])`
- `getInputOutputStocksForStockList : function(stationEntity : Engine.Entity, skipStorage? : boolean) : {CargoTypeId : boolean}, integer, {CargoTypeId : boolean}, integer`  (call as `cargo_util.getInputOutputStocksForStockList(…)`): Collects the input and output cargo types of one stock list.
    - param `stationEntity`: Stock list entity (the parameter name is misleading).
    - param `skipStorage`: If `true`, only input and output stocks count. Otherwise storage stocks count as both input and output.
    - returns: 1. Set of input cargo types.
2. Number of input entries (one per stock and cargo type).
3. Set of output cargo types.
4. Number of output entries.

- `getCargoTypesForStockList : function(stationEntity : Engine.Entity) : {CargoTypeId : CargoUtil.StockProperties}`  (call as `cargo_util.getCargoTypesForStockList(…)`): Returns input, output and booster properties per cargo type of a stock list.
    - param `stationEntity`: Stock list entity (the parameter name is misleading).
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:60`: `result.cargoTypes = cargo_util.getCargoTypesForStockList(subCon)`
- `getVehicleUnsetCapacities : function(vehicleEntity : Engine.Entity) : CargoUtil.VehicleUnsetCapacities`  (call as `cargo_util.getVehicleUnsetCapacities(…)`): Returns the capacities of a vehicle whose cargo type is not decided yet, split into single-type and multi-type capacities.
    - notes: Only cargo types produced in the current game are considered. `LocationCargoList` uses this to show empty vehicles in entity windows.
    - used 3x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:395`: `local unsetCapacities = cargo_util.getVehicleUnsetCapacities(params.locationParams.vehicleEntity)`
- `getCargoFlowCount : function(entityFrom : Engine.Entity, entityTo : Engine.Entity, cargoType : CargoTypeId, toIsTown : boolean) : integer`  (call as `cargo_util.getCargoFlowCount(…)`): Returns how many items of a cargo type were transported per year from one entity to another.
    - param `entityFrom`: Source entity.
    - param `entityTo`: Target entity.
    - param `cargoType`: Cargo type id.
    - param `toIsTown`: If `true`, the log is read on `entityTo` instead of `entityFrom`.
    - returns: Items per year from `api.engine.util.stock.getCargoLogPerYearForTarget(..., "itemsTransported", cargoType)`.
    - used 6x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:211`: `local count = cargo_util.getCargoFlowCount(entity, entityId, cargoType, town)`

### CargoUtil.StockConfig (record)

```teal
record StockConfig
```

One group of stocks of a stock list, as returned by `calculateStockConfigs`.

- `ids : {StockId}`: Stock ids (0-based) in this group.
- `stored : integer`: Number of cargo items stored in the group.
- `capacity : integer`: Sum of the capacities of the stocks in the group.
- `cargoTypesSorted : {CargoTypeId}`: Cargo types the group accepts.
- `allCargoTypes : boolean`: `true` if the group accepts all cargo types but one (`#cargoTypesSorted == getCargoTypesCount(false) - 1`).
- `tendency : integer`: Change of the stock per year.
    - notes: For input stocks: cargo delivered per year minus consumption per year. For output stocks: production per year minus cargo shipped per year. Storage stocks contribute `0`.
- `stockType : StockListType`: Type of the stocks in the group (input, output or storage).
- `quality : integer`: Average cargo quality in percent (`0`-`100`). Only set when `calculateQuality` was requested and the group holds cargo.
- `numSpoiled : integer`: Number of spoiled (bad quality) cargo items. Only counted when `calculateQuality` was requested.
- `canPinCargoType : boolean`: `true` if the group is a storage stock with cargo type filters for which a single cargo type can be pinned.
- `pinnedCargoType : boolean`: `true` if the stocks have a pinned cargo type (`stock.cargoType >= 0`). `nil` when not determined.

### CargoUtil.StockCargoCount (record)

```teal
record StockCargoCount
```

Amount of one cargo type in a stock list, as returned by `calculateStockCargoCount`.

- `cargoType : CargoTypeId`: Cargo type id.
- `count : integer`: Number of items of this cargo type in the counted stocks.
- `spoiledCount : integer`: Number of spoiled items. Only set when quality was requested.
- `tendency : integer`: Trend of the stock from `-3` (strongly falling) to `3` (strongly rising).
    - notes: Based on items unloaded minus items loaded per year: below 10 in either direction is `0`, up to 100 is `±1`, up to 200 is `±2`, more is `±3`. Pass it to `getWarehouseCountIndicatorTexture` for the matching arrow icon.
- `quality : number`: Average quality of this cargo type in the whole stock list. Only set when quality was requested.

### CargoUtil.VehicleCargo (record)

```teal
record VehicleCargo
```

Capacity and load of one cargo type summed over one or more vehicles.

- `cargoType : CargoTypeId`: Cargo type id.
- `capacity : integer`: Total capacity for this cargo type over all vehicles.
- `vehicleEntities : {Engine.Entity}`: Vehicles that have capacity for this cargo type.
- `fill : integer`: Number of loaded items. Only filled for the compare-value functions.
- `hasGoodCargo : boolean`: `true` if a vehicle carries items of this type that are not spoiled. Only set with `calculateQuality`.
- `hasBadCargo : boolean`: `true` if a vehicle carries spoiled items of this type (never set for passengers). Only set with `calculateQuality`.

### CargoUtil.CargoListSortOrder (enum)

```teal
enum CargoListSortOrder
```

Sort order of `getSortedProducedCargoTypes`. Ties are sorted by cargo type id.

- `"CAPACITY"`: Sort by capacity, ascending.
- `"COUNT"`: Sort by the number of items, ascending.
- `"ID"`: Sort by cargo type id.

### CargoUtil.CargoLocationParams (record)

```teal
record CargoLocationParams
```

Where to collect cargo for `getSortedProducedCargoTypes` and `getCargoTypeInfo`. Set one or more of the entity fields; the results of all set locations are added up.

- `vehicleEntity : Engine.Entity`: Vehicle whose load and capacity are counted.
- `stockListOwnerEntity : Engine.Entity`: Industry, warehouse or town building whose stock list is counted (see `getStockListEntityFromOwner`).
- `lineEntity : Engine.Entity`: Line whose capacity usage is counted.
- `stopIndex1 : integer`
- `getTendency : boolean`: If `true`, `getCargoTypeInfo` computes the stock tendency (only for `stockListOwnerEntity`).
- `showEmpty : boolean`: If `true`, also lists cargo types with no items.
    - notes: For vehicles, this also switches from the current configuration's capacities (`TransportVehicle.config.capacities`) to all capacities of the vehicle (`api.engine.util.vehicle.getVehicleCapacities`).
- `extraCapacity : {CargoTypeId : integer}`: Capacity added per cargo type on top of the counted locations.

### CargoUtil.CargoStopParams (record)

```teal
record CargoStopParams
```

Line stop for `getConfiguredStopCargoTypes`.

- `lineEntity : Engine.Entity`: Line entity.
- `stopIndex1 : integer`: 1-based index of the stop in `Line.stops`.
- `getTendency : boolean`
- `showEmpty : boolean`

### CargoUtil.CargoTypeInfo (record)

```teal
record CargoTypeInfo
```

Amount, capacity and quality of one cargo type at a location, as returned by `getCargoTypeInfo`.

- `cargoType : CargoTypeId`: Cargo type id. `-1` for an entry that stands for several cargo types (see `cargoTypes`).
- `cargoTypes : {CargoTypeId}`: Cargo types that share one capacity. Only set by `getVehicleUnsetCapacities`.
- `capacity : integer`: Capacity for this cargo type.
- `count : integer`: Number of items.
- `tendency : integer`: Stock trend from `-3` to `3` (see `StockCargoCount.tendency`). Only set with `getTendency`.
- `spoiledCount : integer`: Number of spoiled items (or unhappy passengers).
- `averageQuality : number`: Average quality (`0`-`1`) of the items; for passengers their happiness.
- `loadSpeedModifiers : Engine.Component.TransportVehicle.LoadSpeedModifiers`: Load speed modifiers of the vehicle for this cargo type. Only set while the vehicle is at a terminal.
- `loadState : Engine.Component.TransportVehicle.LoadState`: Loading state of the vehicle. Only set while the vehicle is at a terminal.
- `gameSpeed : number`: Current game speed-up factor (`GameSpeed.speedup`). Set for vehicles; used for the loading animation speed.

### CargoUtil.VehicleUnsetCapacities (record)

```teal
record VehicleUnsetCapacities
```

Capacities of a vehicle that are not bound to a cargo type yet, as returned by `getVehicleUnsetCapacities`.

- `multiTypes : {CargoUtil.CargoTypeInfo}`: Capacities that can take one of several cargo types, one `CargoTypeInfo` each with `cargoType = -1` and the possible types in `cargoTypes`.
- `singleCapacities : {CargoTypeId : integer}`: Capacities that can take only one cargo type, by cargo type id.

### CargoUtil.StockProperties (record)

```teal
record StockProperties
```

How a stock list uses a cargo type, as returned by `getCargoTypesForStockList`.

- `booster : boolean`: `true` if the cargo type is an input of a booster rule.
- `input : boolean`: `true` if the cargo type is an input (storage stocks count as input).
- `output : boolean`: `true` if the cargo type is an output (storage stocks count as output).

### CargoUtil.CargoFlowData (record)

```teal
record CargoFlowData
```

One supplier or consumer of a cargo flow, as built by the industry and town entity windows.

- `position : Vec3f`: Position of the supplier or consumer entity.
- `cargoType : CargoTypeId`: Cargo type of the flow.
- `cargoCount : integer`: Items transported per year (from `getCargoFlowCount`).
