# gui.line_vehicle_mgmt.vehicle_store

Definition file: `base/tealdef/gui/line_vehicle_mgmt/vehicle_store.d.tl` (in the game folder).

Shared types of the vehicle store (buy / replace / modify vehicles window): store mode, browser filter and sort settings, and the shopping-cart API.

## VehicleStore (record)

```teal
global record VehicleStore
```

Shared types of the vehicle store window.

### VehicleStore.VehicleStoreMode (enum)

```teal
enum VehicleStoreMode
```

What the store window does with the chosen vehicles.

- `"Buy"`: Buy new vehicles (opened by the `buyVehicles` event).
- `"Modify"`: Change the composition of existing vehicles (opened by the `modifyVehicles` event).
  In game: "Modify {count} Vehicle for {cost} / Modify {count} Vehicles for {cost}" (German: „Modifiziere {count} Fahrzeug für {cost} / Modifiziere {count} Fahrzeuge für {cost}“)
- `"Replace"`: Replace existing vehicles; their depreciated value is refunded (opened by the `replaceVehicles` event).
  In game: "Vehicles To Replace:", "Replace {count} Vehicle for {cost} / Replace {count} Vehicles for {cost}" (German: „Zu ersetzende Fahrzeuge:“, „Ersetze {count} Fahrzeug für {cost} / Ersetze {count} Fahrzeuge für {cost}“)

### VehicleStore.CanBuy (record)

```teal
record CanBuy
```

Result of `VehicleCartApi.canBuyVehicle`.

In game: "Buy {count} Vehicle for {cost} / Buy {count} Vehicles for {cost}", " (Not Enough Money)", "Not Enough Money" (German: „Kaufe {count} Fahrzeug für {cost} / Kaufe {count} Fahrzeuge für {cost}“, „ (Nicht genug Geld)“, „Nicht genug Geld“)

- `canBuy : boolean`: False if the player lacks the money or a mission forbids the purchase.
- `missionError : string`: Message returned by the mission GUI script event `vehicleStore`/`mission.buyVehicle`, or nil.
- `totalPrice : integer`: Price of the model (`metadata.cost.price`) times the amount.

### VehicleStore.VehicleBrowserFilter (record)

```teal
record VehicleBrowserFilter
```

Current filter of the vehicle browser; evaluated per vehicle by `VehicleStoreUtil.vehicleFilter`.

- `text : string`: Search text; vehicles whose name does not contain it are hidden. Empty string for no search.
- `transportModes : {TransportMode : boolean}`: Transport modes of the depot or vehicles; a vehicle must share at least one enabled mode.
- `carrier : Carrier`: Carrier the vehicles must use.
- `cargoFilters : {CargoTypeId : boolean}`: Selected cargo types; a vehicle must be able to carry all of them.
- `engines : {VehicleEngineType | string : boolean}`: Selected engine types (`VehicleEngineType`); if any is set, a vehicle must have at least one of them.
- `multipleUnits : boolean`: Include multiple units.
- `onlyMultipleUnits : boolean`: Show only multiple units.
- `onlyLightrail : boolean`: Show only vehicles all of whose parts can run on tram tracks (set by the Light Rail toggle in tram depots).
- `singleVehicles : boolean`: Include single vehicles (not multiple units).
- `filterTags : {string}`: Filter tags of the depot or of the vehicles being replaced; see `VehicleStoreUtil.depotMatchesVehicleFilterTags`.
- `withEngine : boolean`: If not nil, a vehicle must (true) or must not (false) have an engine.
- `withCapacity : boolean`: If not nil, a vehicle must (true) or must not (false) have capacity.
- `withPassengerCapacity : boolean`: Set by the tab handlers to select passenger vehicles; with `withCargoCapacity` it chooses the table layout and whether cargo filters are shown.
- `withCargoCapacity : boolean`: Set by the tab handlers to select cargo vehicles; makes the cargo filters visible and chooses the cargo table layout.
- `vehicleFilter : GameContext.Filters.VehicleFilter`: Global enabled / disabled vehicle lists from the game context (matched against the model or multiple-unit path).

### VehicleStore.VehicleBrowserSort (record)

```teal
record VehicleBrowserSort
```

Sort order of the vehicle browser.

- `mode : VehicleBrowserSortMode`: Sort mode.
- `ascending : boolean`: Sort ascending.
- `groupTypes : boolean`: Set to true by the store window; not read by base-game code.

#### VehicleStore.VehicleBrowserSort.VehicleBrowserSortMode (enum)

```teal
enum VehicleBrowserSortMode
```

Value the vehicles are sorted by. Vehicles are first grouped into buckets (no capacity, passengers, mixed, cargo); see `VehicleStoreUtil.getPrimaryBucket`.

- `"Alphabetical"`: By name.
- `"Speed"`: By top speed.
- `"Capacity"`: By total capacity.
- `"Cost"`: By purchase price.
- `"RunningCosts"`: By running costs.
- `"Noise"`: By noise score.
- `"Pollution"`: By pollution score.
- `"Comfort"`: By comfort factor.
- `"Power"`: By engine power.
- `"Thrust"`: By thrust (aircraft).
- `"Weight"`: By empty weight.
- `"Length"`: By length.
- `"LoadingSpeed"`: By loading speed.
- `"YearFrom"`: By first year of availability (the store's default, ascending).

### VehicleStore.CartInfo (record)

```teal
record CartInfo
```

Price information for the buy button of a vehicle row in the store table.

- `amount : integer`: Number of vehicles bought at once (the replace amount); filled in by the table cell.
- `totalPrice : integer`: Total price.
- `refund : integer`: Depreciated value of the vehicles in the cart (and, in Replace mode, of the vehicles being replaced).
- `notEnoughMoney : boolean`: True if the player cannot pay `totalPrice`.
- `viable : boolean`: False if a vehicle in the cart has neither power nor thrust.
- `missionError : string`: Mission message that blocks the purchase, or nil; filled in by the table cell.

### VehicleStore.VehicleCartApi (record)

```teal
record VehicleCartApi
```

API of the store's shopping cart node; the browser and the table call it.

- `getVehiclesFromCart : function(index? : integer) : {{TransportVehiclePart}}`: Parts of one cart entry, grouped per parts list (one list per single vehicle or multiple unit).
    - param `index`: One-based cart entry; default 1.
    - returns: The parts lists, or nil if the cart is empty.
- `selectVehicleForBuying : function(modelId : integer, muFileName : string, color : Vec3f)`: Remembers the model, multiple-unit file and colour that the "buy immediately" action will buy.
- `canBuyVehicle : function(modelId : integer, amount : integer) : CanBuy`: Checks money and mission restrictions for buying `amount` vehicles of a model.
    - returns: A `CanBuy` record.
- `addVehicleToCart : function(modelId : integer, multipleUnit : boolean, multipleUnitFileName : string, color : Vec3f, buyImmediately : boolean, amount : integer)`: Adds a vehicle or multiple unit to the cart, or buys it immediately.
    - param `modelId`: Model id, or multiple-unit id if `multipleUnit` is true.
    - param `multipleUnit`: True if `modelId` is a multiple unit.
    - param `multipleUnitFileName`: File name of the multiple unit.
    - param `color`: Colour of the new parts.
    - param `buyImmediately`: If true, buys `amount` vehicles (or replaces all vehicles to replace) right away instead of adding to the cart.
    - param `amount`: Number of vehicles to buy immediately.
- `getReplaceAmount : function() : integer`: Number of vehicles being replaced (1 outside Replace mode).
- `getCartInfo : function(canBuy : VehicleStore.CanBuy) : VehicleStore.CartInfo`: Builds price information from a `CanBuy` result.
    - returns: `CartInfo` with `totalPrice`, `refund`, `notEnoughMoney` and `viable` set.
