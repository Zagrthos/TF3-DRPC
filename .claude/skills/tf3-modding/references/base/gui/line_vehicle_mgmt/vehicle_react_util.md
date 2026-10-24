# gui.line_vehicle_mgmt.vehicle_react_util

Definition file: `base/tealdef/gui/line_vehicle_mgmt/vehicle_react_util.d.tl` (in the game folder).

GUI recipes for vehicles (icons, condition, balance) and `HandleVehicleChanges`, which turns store results into buy, replace and sell commands.

## VehicleReactUtil (record)

```teal
global record VehicleReactUtil
```
Load: `local vehicle_react_util = ug_require "::/gui/line_vehicle_mgmt/vehicle_react_util.tl" as VehicleReactUtil`

GUI recipes for vehicles and the helper that applies vehicle store results.

- `makeVehicleIconData : function(VehicleImageParams) : VehicleIconData`  (call as `vehicle_react_util.makeVehicleIconData(…)`): Builds icon data for one part of a vehicle from its model description.
    - returns: Icon data for `VehicleImage`.
- `ConditionTextView : Recipe<Engine.Entity>`: Recipe taking a vehicle entity; shows its condition text (`VehicleUtil.getConditionText`) as a button that opens the vehicle's entity window. Updated every step.
- `ConditionIcon : Recipe<Engine.Entity>`: Recipe taking a vehicle entity; shows its condition icon with the condition text as tooltip. Clicking selects the vehicle.
- `VehicleImage : Recipe<VehicleIconData>`: Recipe drawing one vehicle part icon; tinted with `color` through `cblendFileName` when both are set.
- `VehicleWidget : Recipe<VehicleWidgetParams>`: Recipe showing the icons of one or more vehicles in a horizontal scroll area, grouping equal compositions and equal consecutive parts with counters.
    - used 6x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:1939`: `vehicle_react_util.VehicleWidget { vehicleEntities = {entityId} }`
- `getVehicleIconType : function(Engine.Entity) : VehicleIconType`  (call as `vehicle_react_util.getVehicleIconType(…)`): Picks the `VehicleIconType` of a vehicle from its carrier, transport modes, engine type (rail) and helicopter flag (air).
    - returns: The icon type, or nil for unknown carriers.
- `getVehicleIconPathBig : function(Engine.Entity) : string`  (call as `vehicle_react_util.getVehicleIconPathBig(…)`): Path of the large HUD vehicle icon (`::/gui/hud/icons/vehicle_*.tga`) for a vehicle, based on `getVehicleIconType`.
    - returns: Icon path, or nil.
- `HandleVehicleChanges : function(changes : {VehicleReactUtil.VehicleChange}, protectedEntities : {Engine.Entity : GameContext.Filters.ProtectionConfig | boolean}, getLineAndDepot : function(vehicleEntity : Engine.Entity) : {Engine.Entity, Engine.Entity}, getFirstStopToSendTo : function(vehicleToSendToLine : Engine.Entity, lineToSendTo : Engine.Entity) : integer, onBuy : function(resultEntities : {{Engine.Entity, Engine.Revision}}), onFail? : function(reason : string))`  (call as `vehicle_react_util.HandleVehicleChanges(…)`): Sends the commands for a list of vehicle store changes - buy new vehicles, replace vehicle configurations, or sell vehicles.
  In game: "Could not clone vehicles (not enough money).", "Could not find suitable depot." (German: „Konnte Fahrzeuge nicht duplizieren (nicht genügend Geld)“, „Kein passendes Depot gefunden.“)
    - param `changes`: Changes to apply. Parts with `purchaseTime <= 0` get the current game time; auto load config is switched on for all compartments.
    - param `protectedEntities`: Vehicles that must not be sold (from `gameCtx.filters`); may be nil.
    - param `getLineAndDepot`: For new vehicles (negative `vehicleEntity`) returns `{line, depot}`; the vehicle is bought in the depot and, if the line is >= 0, sent to it.
    - param `getFirstStopToSendTo`: Returns the stop index a newly bought vehicle is sent to on its line; 0 if nil.
    - param `onBuy`: Called with `{{entity, revision}}` of each bought vehicle.
    - param `onFail`: Called with a translated reason if buying fails or no depot is found.
    - notes: Uses `api.cmd.makeVehicleBuyCmd`, `makeVehicleSetLineCmd`, `makeVehicleReplaceCmd` and `makeVehicleSellCmd`. Replacing a vehicle spawns a happy emote HUD image above it unless event icons are hidden in the HUD filter.
- `VehicleBalance : Recipe<VehicleBalanceParams>`: Recipe showing a vehicle's balance over the last game year (`api.engine.util.finance.calculateBalance`), styled `negative` or `positive`.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:764`: `vehicle_react_util.VehicleBalance{`

### VehicleReactUtil.VehicleImageParams (record)

```teal
record VehicleImageParams
```

Selects one part of a vehicle for `makeVehicleIconData`.

- `vehicleEntity : Engine.Entity`: Vehicle entity (with a `TRANSPORT_VEHICLE` component).
- `vehicleIndex : integer`: One-based index into `transportVehicleConfig.vehicles` (the part, e.g. a wagon).

### VehicleReactUtil.VehicleIconData (record)

```teal
record VehicleIconData
```

Data for drawing one vehicle part icon with `VehicleImage`.

- `fileName : string`: Icon image path (the model's `description.icon20`).
- `cblendFileName : string`: Colour-blend mask (`description.icon20cblend`); with `color` set the icon is tinted through this mask.
- `name : string`: Model name, used as tooltip.
- `flippedHorizontally : boolean`: True if the part is reversed in the consist.
- `color : Vec3f`: Part colour; nil if the part has no custom colour (colour x < 0).
- `tooltipOverride : string`: Tooltip used instead of `name`.

### VehicleReactUtil.VehicleWidgetParams (record)

```teal
record VehicleWidgetParams
```

Parameters of `VehicleWidget`.

- `vehicleEntities : {Engine.Entity}`: Vehicles to show; vehicles with the same composition are grouped with an `Nx` counter.
- `onClick : function()`: If set, the widget is a button calling this function.
- `class : string`: Extra style classes.
- `blinking : boolean`: Adds the `blinking` style class to the button (only with `onClick`).
- `tooltipOverride : string`: Tooltip for all part icons instead of the model names.

### VehicleReactUtil.MaintenanceSelectorParams (record)

```teal
record MaintenanceSelectorParams
```

Declared parameter record; not used by base-game code.

- `onClose : function()`
- `vehicles : {EntityUtil.EntityAndRevision}`
- `onlySlider : boolean`

### VehicleReactUtil.VehicleChange (record)

```teal
record VehicleChange
```

One change produced by the vehicle store and applied by `HandleVehicleChanges`.

- `vehicleEntity : Engine.Entity`: Vehicle to replace or sell; a negative value means a new vehicle is bought.
- `config : TransportVehicleConfig`: New vehicle configuration; an empty `vehicles` list sells the vehicle.

### VehicleReactUtil.VehicleBalanceParams (record)

```teal
record VehicleBalanceParams
```

Parameters of `VehicleBalance`.

- `entity : Engine.Entity`: Vehicle whose balance is shown.
- `stepTimer : boolean`: If true, refreshes on a step timer instead of every step.
