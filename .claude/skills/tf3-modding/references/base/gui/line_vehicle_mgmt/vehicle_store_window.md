# gui.line_vehicle_mgmt.vehicle_store_window

Definition file: `base/tealdef/gui/line_vehicle_mgmt/vehicle_store_window.d.tl` (in the game folder).

The vehicle store window and its entry point, which listens for the `buyVehicles`, `replaceVehicles` and `modifyVehicles` GUI events.

## VehicleStoreWindow (record)

```teal
global record VehicleStoreWindow
```
Load: `local vehicle_store_window = ug_require "::/gui/line_vehicle_mgmt/vehicle_store_window.tl" as VehicleStoreWindow`

Vehicle store window and its entry point.

- `VehicleStoreEntryPoint : Recipe<VehicleStoreEntryPointParam>`: Invisible recipe that precomputes the store's vehicle data and opens the store window on the `buyVehicles`, `replaceVehicles` and `modifyVehicles` events.
  In game: "Replace Vehicles", "Modify Vehicles" (German: „Fahrzeuge ersetzen“, „Fahrzeuge modifizieren“)
    - notes: Each event removes any open store window and adds a new one. The event payload is a `VehicleStoreEventParam`.
    - used 2x in the base game, e.g. `base/content/gui/gui/main/game.tl:284`: `local vehicleStoreEntryPointRef = react.useNodeRef(vehicle_store_window.VehicleStoreEntryPoint)`
- `VehicleStoreWindow : Recipe<VehicleStoreWindowParam>`: The vehicle store window recipe (window id `vehiclestore.window`).
    - used 17x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:467`: `wc.removeAllWindows(vehicle_store_window.VehicleStoreWindow)`

### VehicleStoreWindow.LayoutStyle (enum)

```teal
enum LayoutStyle
```

Layout of the store window; switched with a button and forced to `TwoPaneSplit` in gamepad mode.

- `"Table"`: Vehicles in a sortable table.
- `"TwoPaneSplit"`: Vehicle list with a details pane (default).

### VehicleStoreWindow.MuAndId (record)

```teal
record MuAndId
```

Reference to a vehicle variant.

- `multipleUnit : boolean`: True if `id` is a multiple-unit id, false for a model id.
- `id : integer`: Model id or multiple-unit id.

### VehicleStoreWindow.VehiclesStaticData (record)

```teal
record VehiclesStaticData
```

Vehicle data computed once by the entry point for all models with `transportVehicle` metadata and all multiple units.

- `repId2elementStatic : {integer : VehicleStoreUtil.ElementStatic}`: Static store element per model id or multiple-unit id.
- `variantGroup2muAndVariants : {string : {MuAndId}}`: Variants per variant group (the `groupFileName` of the transport vehicle or multiple unit).

### VehicleStoreWindow.VehicleStoreEventParam (record)

```teal
record VehicleStoreEventParam
```

Payload of the `buyVehicles`, `replaceVehicles` and `modifyVehicles` GUI events that open the store window.

Example:
```lua
-- from entity_window/vehicle/vehicle.tl
local tmc = line_util.getLineAndVehicleCompatibleTransportModesAndCarrier({vehicleEntityId})
react.fireEvent(nil, "replaceVehicles", {
    vehicleEntities = {vehicleEntityId},
    carrier = tmc.carrier,
    transportModes = tmc.transportModes,
    filterTags = vehicle_store_util.getVehicleFilterTags({vehicleEntityId}),
    openedFromEOW = true,
    onClose = function() replaceWindowOpen:set(false) end,
})
```

- `title : string`: Window title (used by `buyVehicles`; replace and modify use fixed titles).
- `vehicleEntities : {Engine.Entity}`: Vehicles to replace or modify.
- `depot : Engine.Entity`: Depot in which new vehicles are bought (Buy mode).
- `carrier : Carrier`: Carrier of the vehicles offered.
- `transportModes : {TransportMode : boolean}`: Transport modes the offered vehicles must support.
- `filterTags : {string}`: Filter tags the offered vehicles must match.
- `openedFromEOW : boolean`: True if opened from an entity window; changes the window style and initial position.
- `getFirstStopToSendTo : function(vehicleToSendToLine : Engine.Entity, lineToSendTo : Engine.Entity) : integer`: Returns the stop index a newly bought vehicle is sent to on its line.
- `onLocate : function()`: Called by the window's locate button (Buy mode only).
- `onClose : function()`: Called when the window closes.
- `buyCallback : function(resultEntities : {{Engine.Entity, Engine.Revision}})`: Called with `{{entity, revision}}` of bought vehicles (Buy mode only).

### VehicleStoreWindow.VehicleStoreWindowParam (record)

```teal
record VehicleStoreWindowParam
```

Parameters of the `VehicleStoreWindow` recipe, built by the entry point from a `VehicleStoreEventParam`.

- `gameCtx : GameContext`: Game context.
- `layoutStyleRef : ReactRefT<VehicleStoreWindow.LayoutStyle>`: Shared ref holding the current layout style.
- `vehiclesStaticData : VehiclesStaticData`: Precomputed vehicle data.
- `colorChooserColorsStaticData : {Vec3f}`: Colours offered by the colour chooser (line colours plus grayscale).
- `title : string`: Window title.
- `vehicleEntities : {Engine.Entity}`: Vehicles to replace or modify; the window closes when none of them exists any more.
- `depot : Engine.Entity`: Depot for new vehicles; if nil, `api.engine.util.vehicle.findBestLineAndDepotForVehicle` chooses one.
- `carrier : Carrier`: Carrier of the vehicles offered; rail and tram allow multi-part consists in the cart, other carriers buy immediately.
- `transportModes : {TransportMode : boolean}`: Transport modes the offered vehicles must support.
- `filterTags : {string}`: Filter tags the offered vehicles must match.
- `vehicleStoreMode : VehicleStore.VehicleStoreMode`: Buy, Replace or Modify.
- `locateVisible : boolean`: Shows the window's locate button.
- `openedFromEOW : boolean`: True if opened from an entity window.
- `getFirstStopToSendTo : function(vehicleToSendToLine : Engine.Entity, lineToSendTo : Engine.Entity) : integer`: Returns the stop index a newly bought vehicle is sent to on its line.
- `onLocate : function()`: Called by the locate button.
- `onClose : function()`: Called when the window closes.
- `onAccepted : function({VehicleReactUtil.VehicleChange})`: Called with the accepted changes after they were passed to `VehicleReactUtil.HandleVehicleChanges`.
- `buyCallback : function(resultEntities : {{Engine.Entity, Engine.Revision}})`: Called with `{{entity, revision}}` of bought vehicles.

### VehicleStoreWindow.VehicleStoreEntryPointParam (record)

```teal
record VehicleStoreEntryPointParam
```

Parameters of `VehicleStoreEntryPoint`.

- `gameCtx : GameContext`: Game context; its window container hosts the store window.
