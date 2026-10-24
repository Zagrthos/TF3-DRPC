# gui.line_vehicle_mgmt.vehicle_store_util

Definition file: `base/tealdef/gui/line_vehicle_mgmt/vehicle_store_util.d.tl` (in the game folder).

Helpers of the vehicle store: collect displayable vehicle data from model metadata, filter vehicles, match depot filter tags and transport modes, and small filter widgets.

## VehicleStoreUtil (record)

```teal
global record VehicleStoreUtil
```
Load: `local vehicle_store_util = ug_require "::/gui/line_vehicle_mgmt/vehicle_store_util.tl" as VehicleStoreUtil`

Helpers of the vehicle store.

- `makeSingleVehicle : function(model : Model) : Vehicle`  (call as `vehicle_store_util.makeSingleVehicle(…)`): Collects the metadata of one vehicle model into a `Vehicle` record.
    - param `model`: Model with `transportVehicle` metadata (e.g. from `api.res.modelRep.get`).
    - used 6x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:94`: `local vehicle : VehicleStoreUtil.Vehicle = vehicle_store_util.makeSingleVehicle(model)`
- `makeMultipleVehicles : function(mu : MultipleUnit) : {Vehicle}`  (call as `vehicle_store_util.makeMultipleVehicles(…)`): Builds a `Vehicle` record for every part of a multiple unit.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:4831`: `local vehicles = vehicle_store_util.makeMultipleVehicles(mu)`
- `makeMultipleVehiclesFromParts : function(configs : {{TransportVehiclePart}}) : {Vehicle}`  (call as `vehicle_store_util.makeMultipleVehiclesFromParts(…)`): Builds `Vehicle` records for all parts of a list of parts lists (e.g. the vehicles in the cart).
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:3676`: `local vehiclePartsLists2 = vehicle_store_util.makeMultipleVehiclesFromParts(vehicleParts)`
- `collectVehicleData : function(vehicles : {Vehicle}, modifiers? : Engine.Component.TransportVehicle.Modifiers) : VehicleData`  (call as `vehicle_store_util.collectVehicleData(…)`): Combines the metadata of several vehicle parts into one `VehicleData` record (capacity, speed, power, emissions, price, years, ...).
    - param `vehicles`: Parts of the vehicle or consist.
    - param `modifiers`: Optional modifiers of an existing transport vehicle; scale noise, pollution, comfort and top speed into the `...Adjusted` fields.

    ```lua
    -- from entity_window/vehicle/vehicle_eow.script.tl
    local vehicles = {}
    for i, modelId in ipairs(modelIds) do
        local vehicle = vehicle_store_util.makeSingleVehicle(api.res.modelRep.get(modelId))
        vehicle.maintenanceState = maintenanceStates[i]
        table.insert(vehicles, vehicle)
    end
    local vehicleData = vehicle_store_util.collectVehicleData(vehicles, modifiers)
    ```

    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:483`: `local vehicleData = vehicle_store_util.collectVehicleData(vehicles, param.modifiers)`
- `goodnessToTextString : function(value : number) : string`  (call as `vehicle_store_util.goodnessToTextString(…)`): Translated rating for a 0 to 1 value - Very Poor (< 0.2), Poor, Mediocre, Good, Very Good (>= 0.8).
  In game: "Very Poor", "Mediocre", "Very Good" (German: „Sehr schlecht“, „Mittelmässig“, „Sehr gut“)
    - returns: The rating text; "" for negative values.
    - used 9x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:579`: `ratingText = vehicle_store_util.goodnessToTextString(noisePercentage),`
- `getRangesFromData : function(vehicleData : VehicleData, carrier : Carrier, year : integer) : VehicleUtil.Ranges`  (call as `vehicle_store_util.getRangesFromData(…)`): Value ranges for a vehicle via `VehicleUtil.getRanges`, using the passenger-only ranges when the vehicle carries only passengers.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:1726`: `ranges = vehicle_store_util.getRangesFromData(`
- `getPrimaryBucket : function(elem : ElementStatic) : integer`  (call as `vehicle_store_util.getPrimaryBucket(…)`): Sort bucket of a store entry - 1 no capacity, 2 passengers only, 3 passengers and cargo, 4 cargo only. The store sorts by bucket before the chosen sort mode.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:1816`: `local bucket_a = vehicle_store_util.getPrimaryBucket(a.static)`
- `vehicleFilter : function(elemStatic : VehicleStoreUtil.ElementStatic, filterParams : FilterParams, filter : VehicleStore.VehicleBrowserFilter) : boolean`  (call as `vehicle_store_util.vehicleFilter(…)`): Tells whether a store entry passes the browser filter.
    - returns: True if the entry is shown.
    - notes: Checks, in order: filter tags, multiple-unit / light-rail flags, the game context's enabled / disabled vehicle lists, single vehicle vs. multiple unit, availability years, carrier, transport modes, search text, cargo filters, capacity, engine types and `withEngine`.
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_table.tl:513`: `if vehicle_store_util.vehicleFilter(`
- `showCargoFilters : function(VehicleStore.VehicleBrowserFilter) : boolean`  (call as `vehicle_store_util.showCargoFilters(…)`): True if the cargo filter toggles are shown for the filter (`withCargoCapacity` or `withCapacity` is set).
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:2094`: `local showFilters = vehicle_store_util.showCargoFilters(param.filter)`
- `acceptsMultiVehicle : function(carrier : Carrier) : boolean`  (call as `vehicle_store_util.acceptsMultiVehicle(…)`): True for carriers whose vehicles are composed in a cart (rail and tram). For other carriers the store buys vehicles immediately.
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_table.tl:560`: `local buyImmediately = not vehicle_store_util.acceptsMultiVehicle(params.filter.carrier)`
- `makeVehicleData : function(repId : integer, multipleUnit : boolean) : VehicleData`  (call as `vehicle_store_util.makeVehicleData(…)`): `collectVehicleData` for a model id or a multiple-unit id.
    - used 3x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:4687`: `local vehicleData = vehicle_store_util.makeVehicleData(modelId, false)`
- `getVehicleNameAndIcons : function(repId : integer, multipleUnit : boolean, small : boolean) : VehicleNameAndIcon`  (call as `vehicle_store_util.getVehicleNameAndIcons(…)`): Name and part icons of a model or multiple unit.
    - param `repId`: Model id or multiple-unit id.
    - param `multipleUnit`: True if `repId` is a multiple unit.
    - param `small`: For a single model, use `icon20` instead of `iconSmall`.
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:365`: `local image : string = vehicle_store_util.getVehicleNameAndIcons(`
- `makeVehicleProgressBarRow : function(progressBarIcon : string, tooltip : string, percentage : number, progressBarText : string, comparisonPercentage? : number) : TreeNodeId`  (call as `vehicle_store_util.makeVehicleProgressBarRow(…)`): Builds a row with icon, progress bar and text for a vehicle property; with a comparison value both bars are shown, coloured good or bad.
    - param `percentage`: Bar value, clamped to 0 to 1.
    - param `comparisonPercentage`: Optional value to compare with (e.g. the vehicle being replaced).
    - used 7x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:496`: `rowsLeft[#rowsLeft + 1] = vehicle_store_util.makeVehicleProgressBarRow(`
- `makeVehiclePropertyRow : function(tooltip : string, text : string, propertyIcon : string, percentage : number, comparisonPercentage? : number, higherIsBetter? : boolean) : TreeNodeId`  (call as `vehicle_store_util.makeVehiclePropertyRow(…)`): Builds a row with icon and text for a vehicle property; the text is coloured good or bad when a comparison value differs.
    - param `higherIsBetter`: Pass false if a lower value is better; default true.
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:572`: `rowsRight[#rowsRight + 1] = vehicle_store_util.makeVehiclePropertyRow(`
- `makeVehiclePropertyPlaceholder : function() : TreeNodeId`  (call as `vehicle_store_util.makeVehiclePropertyPlaceholder(…)`): Empty component with the `property-placeholder` style, used to keep rows aligned.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:567`: `rowsLeft[#rowsLeft + 1] = vehicle_store_util.makeVehiclePropertyPlaceholder()`
- `handleRailCarrier : function(vehicleBrowserFilter : VehicleStore.VehicleBrowserFilter, id2index : {string : integer}, index : integer) : VehicleStore.VehicleBrowserFilter`  (call as `vehicle_store_util.handleRailCarrier(…)`): Sets the browser filter for the selected tab of a rail depot (locomotive, wagon, multiple unit).
    - param `id2index`: Tab id to tab index.
    - param `index`: Index of the selected tab.
    - returns: The changed filter.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:2484`: `return vehicle_store_util.handleRailCarrier(newState, id2indexLocal, index)`
- `handleTramCarrier : function(vehicleBrowserFilter : VehicleStore.VehicleBrowserFilter, id2index : {string : integer}, index : integer) : VehicleStore.VehicleBrowserFilter`  (call as `vehicle_store_util.handleTramCarrier(…)`): Sets the browser filter for the selected tab of a tram depot (railcar, locomotive, wagon).
    - returns: The changed filter.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:2486`: `return vehicle_store_util.handleTramCarrier(newState, id2indexLocal, index)`
- `handleDefaultCarrier : function(vehicleBrowserFilter : VehicleStore.VehicleBrowserFilter) : VehicleStore.VehicleBrowserFilter`  (call as `vehicle_store_util.handleDefaultCarrier(…)`): Resets the browser filter for carriers without tabs (road, water, air).
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:2488`: `return vehicle_store_util.handleDefaultCarrier(newState)`
- `getTransportModeMapForVehicles : function(vehicleEntities : {Engine.Entity}) : {TransportMode : boolean}`  (call as `vehicle_store_util.getTransportModeMapForVehicles(…)`): Union of the enabled transport modes of the given vehicles.
    - used 7x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:1532`: `local transportModeMap = vehicle_store_util.getTransportModeMapForVehicles({vehicleEntity})`
- `depotMatchesVehicleFilterTags : function(depotFilterTags : {string}, vehicleFilterTags : {string}) : boolean`  (call as `vehicle_store_util.depotMatchesVehicleFilterTags(…)`): Tells whether one filter-tag list contains all tags of another; used to match vehicles to depots.
    - returns: True if the vehicle is allowed.
    - notes: The implementation treats the second argument as the required tag list and the first as the tags to check: every tag of the second list must appear in the first. If the second argument is nil, everything matches; if it is an empty list, only a non-empty first list matches. Mind the argument order: the definition names the first parameter `depotFilterTags`, the store's `vehicleFilter` passes `(vehicle tags, depot tags)`, while the availability notification script passes `(depot tags, vehicle tags)`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/availability.script.tl:100`: `if not opened and vehicle_store_util.depotMatchesVehicleFilterTags(depotFilterTags, tv.filterTags)`
- `depotMatchesVehicleTransportModes : function(depotTransportModes : {TransportMode : boolean}, vehicleTransportModes : {TransportMode : boolean}) : boolean`  (call as `vehicle_store_util.depotMatchesVehicleTransportModes(…)`): True if the two transport-mode maps share at least one enabled mode (or the second map is nil).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/availability.script.tl:101`: `and vehicle_store_util.depotMatchesVehicleTransportModes(depotTransportModes, vehicleTransportModes) then`
- `getVehicleFilterTags : function(vehicleEntities : {Engine.Entity}) : {string}`  (call as `vehicle_store_util.getVehicleFilterTags(…)`): Filter tags of the given vehicles, from their models' `transportVehicle.filterTags` and the filter tags of their multiple units.
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:456`: `filterTags = vehicle_store_util.getVehicleFilterTags({vehicleEntityId}),`
- `getDepotFilterTags : function(depotEntity : Engine.Entity) : {string}`  (call as `vehicle_store_util.getDepotFilterTags(…)`): Filter tags of a vehicle depot (`VEHICLE_DEPOT` component).
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/availability.script.tl:98`: `local depotFilterTags = vehicle_store_util.getDepotFilterTags(e)`
- `EngineToggle : Recipe<EngineToggleParam>`: Toggle button for one engine group of the engine filter.
  In game: "Steam", "Diesel", "Electric" (German: „Dampf“, „Diesel“, „Elektrisch“)
    - used 3x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:2171`: `vehicle_store_util.EngineToggle{`
- `SizeFilter : Recipe<SizeFilterParam>`: Small / Large toggles for ships and aircraft; restricts the filter to `SMALL_SHIP`/`SMALL_AIRCRAFT` or `SHIP`/`AIRCRAFT`.
  In game: "Small", "Large" (German: „Klein“, „Gross“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:2736`: `table.insert(filters, vehicle_store_util.SizeFilter{`
- `CargoTypes : Recipe<CargoTypesParam>`: Cargo type toggles of the store (passengers, all cargo, and one toggle per cargo type).
  In game: "All Cargo" (German: „Alle Güter“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:2097`: `content = vehicle_store_util.CargoTypes{`
- `LightRailFilter : Recipe<LightRailFilterParam>`: Light Rail toggle (sets `onlyLightrail`), shown for tram depots.
  In game: "Light Rail" (German: „Stadtbahn“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:2745`: `table.insert(filters, vehicle_store_util.LightRailFilter{`

### VehicleStoreUtil.Vehicle (record)

```teal
record Vehicle
```

Metadata of one vehicle model, gathered by `makeSingleVehicle`.

- `tv : ModelMetadata.TransportVehicle`: `transportVehicle` metadata of the model.
- `landVehicle : ModelMetadata.LandVehicle`: `landVehicle` metadata, or nil.
- `waterVehicle : ModelMetadata.WaterVehicle`: `waterVehicle` metadata, or nil.
- `airVehicle : ModelMetadata.AirVehicle`: `airVehicle` metadata, or nil.
- `cost : ModelMetadata.Cost`: `cost` metadata.
- `maintenance : ModelMetadata.Maintenance`: `maintenance` metadata.
- `emissions : ModelMetadata.Emissions`: `emissions` metadata.
- `desc : ModelMetadata.Description`: `description` metadata (name and icons).
- `availability : ModelMetadata.Availability`: `availability` metadata (years).
- `length : number`: Length of the model's bounding box along x (`extent.bbMax.x - extent.bbMin.x`).
- `maintenanceState : number`: Optional maintenance state of an existing vehicle part; if set, `collectVehicleData` fills the `...Adjusted` values.

### VehicleStoreUtil.VehicleData (record)

```teal
record VehicleData
```

Combined, displayable data of a vehicle or consist, computed by `collectVehicleData`.

Notes:

Values are summed over all parts unless noted. The `...Adjusted` fields are only set when
a part has a `maintenanceState` (and the matching maintenance-effect option of the base
config is on) or when `modifiers` scale the value.

- `allCargoTypes : {CargoTypeId : integer}`: Capacity per cargo type, summed over all compartments.
- `cargoClasses : {string}`: Cargo classes the vehicle can load (without duplicates).
- `engines : {VehicleEngineType : boolean}`: Engine types of the land-vehicle engines.
- `variantGroup : string`: Variant group (`transportVehicle.groupFileName`) of the parts, or "".
- `anyCargo : boolean`: True if any part carries cargo other than passengers.
- `passengers : boolean`: True if any part carries passengers.
- `isReversible : boolean`: `transportVehicle.reversible` (of the last part).
- `isLightrail : boolean`: True if every part can run on tram tracks.
- `speed : number`: Lowest top speed of all parts.
- `speedAdjusted : number`: Top speed after maintenance penalty and `topSpeedScale` modifier.
- `maxIndividualSpeed : number`: Highest top speed of any part.
- `noise : number`: Highest noise score of the parts, as 0 to 1 (score / 100).
- `noiseAdjusted : number`: Noise after maintenance penalty and `noiseScale` modifier.
- `pollution : number`: Highest pollution score of the parts, as 0 to 1 (score / 100).
- `pollutionAdjusted : number`: Pollution after maintenance penalty and `pollutionScale` modifier.
- `comfortFactor : number`: Lowest non-zero comfort factor of the parts.
- `comfortFactorAdjusted : number`: Average comfort of the parts after the maintenance penalty, scaled by the `comfortScale` modifier.
- `runningCosts : number`: Sum of `maintenance.runningCosts`.
- `runningCostsAdjusted : number`: Running costs after the maintenance penalty.
- `numCompartments : integer`: Number of compartments.
- `loadingSpeed : number`: Highest `loadSpeed` of the parts; 0 if the vehicle has no capacity.
- `weight : number`: Sum of `weightEmpty` times 1e-3.
- `weightMaxPayload : number`: Sum of `weightMaxPayload` times 1e-3.
- `length : number`: Sum of part lengths.
- `totalCapacity : integer`: Total capacity (per compartment the largest capacity of any cargo type).
- `totalCapacityPassengers : integer`: Passenger capacity.
- `totalCapacityCargo : integer`: Cargo capacity (non-passenger).
- `price : integer`: Sum of `cost.price`.
- `tractiveEffort : number`: Sum of the first engine's tractive effort of each land-vehicle part.
- `thrust : number`: Sum of `airVehicle.maxThrust` times 1e-3.
- `thrustAdjusted : number`: Thrust after the maintenance penalty.
- `rollingFriction : number`: Sum of `landVehicle.friction` (0.1 per aircraft part).
- `power : number`: Sum of engine power (first engine of land vehicles, `availPower` times 1e-3 for ships).
- `powerAdjusted : number`: Power after the maintenance penalty.
- `yearFrom : integer`: Latest `availability.yearFrom` of the parts (-1 if none).
- `yearTo : integer`: Earliest `availability.yearTo` of the parts (5000 if none).
- `noCblendMask : boolean`: False if any part has a colour-blend mask.

### VehicleStoreUtil.ElementDynamic (record)

```teal
record ElementDynamic
```

Per-refresh data of a store entry.

- `ranges : VehicleUtil.Ranges`: Value ranges used to scale the entry's progress bars.
- `sortKey : any`: Value of the current sort mode (number, or string for alphabetical).
- `variantCountFiltered : integer`: Number of the entry's variants that pass the current filter.

### VehicleStoreUtil.ElementStatic (record)

```teal
record ElementStatic
```

Data of a store entry (model or multiple unit) computed once by `VehicleStoreEntryPoint`.

- `repId : integer`: Model id, or multiple-unit id.
- `multipleUnit : boolean`: True for a multiple unit.
- `name : string`: Display name.
- `names : {string}`: Names of the parts.
- `icons : {string}`: Small icons of the parts.
- `iconsBlend : {string}`: Colour-blend masks of the part icons.
- `iconOrientations : {boolean}`: Per part, true if the part is reversed (multiple units only).
- `vehicleData : VehicleStoreUtil.VehicleData`: Combined vehicle data.
- `engineKey : integer`: Sum of engine-type weights (horse 2, steam 4, diesel 6, electric 8).
- `path : string`: Model or multiple-unit file path; also the key into the variant groups.
- `variantCount : integer`: Number of variants whose variant group is this entry's path.
- `isVariant : boolean`: True if the vehicle belongs to a variant group.
- `variantGroupOnly : boolean`: True for a model that has variants but no filter tags; it is only shown as group head, not as a buyable entry of its own.
- `carriersMap : {Carrier : boolean}`: Carriers of the parts.
- `transportModes : {TransportMode : boolean}`: Transport modes of the parts.
- `engineTypes : {VehicleEngineType : boolean}`: Engine types of the land-vehicle parts.
- `hasEngine : boolean`: True if any land-vehicle part has an engine; always true for water and air vehicles.
- `filterTags : {string}`: Filter tags of the parts (multiple unit's own tags for multiple units).

### VehicleStoreUtil.Element (record)

```teal
record Element
```

One entry of the vehicle store list.

- `dynamic : ElementDynamic`: Per-refresh data.
- `static : ElementStatic`: Precomputed data.

### VehicleStoreUtil.FilterParams (record)

```teal
record FilterParams
```

Context for `vehicleFilter`.

- `year : integer`: Current game year; vehicles outside their availability years are hidden.
- `anyCargoFilterSet : boolean`: True if any cargo filter is selected.
- `passengerCargoTypeId : integer`: Cargo type id of passengers.

### VehicleStoreUtil.VehicleNameAndIcon (record)

```teal
record VehicleNameAndIcon
```

Result of `getVehicleNameAndIcons`.

- `name : string`: Model or multiple-unit name.
- `names : {string}`: Names of the parts.
- `icons : {string}`: Part icons (`iconSmall`, or `icon20` when small icons are requested for a single model).
- `iconsBlend : {string}`: Matching colour-blend masks.
- `iconOrientations : {boolean}`: Per part, true if reversed (multiple units only; empty for single models).
- `largeIcon : string`: The `icon3d` of the (first) model.

### VehicleStoreUtil.EngineType (enum)

```teal
enum EngineType
```

Engine groups of the store's engine filter toggles.

- `"Steam"`: Steam engines.
- `"Diesel"`: Diesel engines.
- `"Electric"`: Electric engines.

### VehicleStoreUtil.EngineToggleParam (record)

```teal
record EngineToggleParam
```

Parameters of `EngineToggle`.

- `type : EngineType`: Engine group this toggle controls.
- `modes : {EngineType : boolean}`: Current on/off state of all engine groups.
- `setModes : function({EngineType : boolean})`: Called with the updated state when the toggle changes.

### VehicleStoreUtil.SizeFilterParam (record)

```teal
record SizeFilterParam
```

Parameters of `SizeFilter`.

- `showFilters : boolean`: Initial visibility state of the filters.
- `filter : VehicleStore.VehicleBrowserFilter`: Current browser filter.
- `setFilterSort : function(newFilter : VehicleStore.VehicleBrowserFilter, newSort : VehicleStore.VehicleBrowserSort, forceUpdate : boolean)`: Applies the new filter.
- `transportModes : {TransportMode : boolean}`: Transport modes available (of the depot or vehicles).

### VehicleStoreUtil.CargoTypesParam (record)

```teal
record CargoTypesParam
```

Parameters of `CargoTypes`.

- `filter : VehicleStore.VehicleBrowserFilter`: Current browser filter.
- `setFilterSort : function(newFilter : VehicleStore.VehicleBrowserFilter, newSort : VehicleStore.VehicleBrowserSort, forceUpdate : boolean)`: Applies the new filter.
- `allCargo : {CargoTypeId : boolean}`: Cargo types offered as toggles.

### VehicleStoreUtil.LightRailFilterParam (record)

```teal
record LightRailFilterParam
```

Parameters of `LightRailFilter`.

- `showFilters : boolean`: Initial visibility state of the filters.
- `filter : VehicleStore.VehicleBrowserFilter`: Current browser filter.
- `setFilterSort : function(newFilter : VehicleStore.VehicleBrowserFilter, newSort : VehicleStore.VehicleBrowserSort, forceUpdate : boolean)`: Applies the new filter.
- `transportModes : {TransportMode : boolean}`: Transport modes available.

### VehicleStoreUtil.TransportModesAndCarrier (record)

```teal
record TransportModesAndCarrier
```

Transport modes and carrier compatible with a set of vehicles; returned by `LineUtil.getLineAndVehicleCompatibleTransportModesAndCarrier`.

- `transportModes : {TransportMode : boolean}`: Compatible transport modes.
- `carrier : Carrier`: Carrier.
