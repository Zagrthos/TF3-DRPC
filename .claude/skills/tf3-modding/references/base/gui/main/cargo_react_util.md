# gui.main.cargo_react_util

Definition file: `base/tealdef/gui/main/cargo_react_util.d.tl` (in the game folder).

React recipes and node builders for cargo: cargo icons, cargo lists of vehicles, lines and
warehouses (count/capacity per cargo type with tendency, quality and loading state),
the input/output arrow display of industries, and cargo class specialization badges.
Load it with `ug_require "/gui/main/cargo_react_util.tl"`. The data comes from `CargoUtil`.

## CargoReactUtil (record)

```teal
global record CargoReactUtil
```
Load: `local cargo_react_util = ug_require "::/gui/main/cargo_react_util.tl" as CargoReactUtil`

Recipes and builders that show cargo icons and cargo lists.

- `makeVehicleCargoInfo : function(VehicleCargoInfoParams) : TreeNodeId`  (call as `cargo_react_util.makeVehicleCargoInfo(…)`): Builds a `LocationCargoList` that shows the cargo of a vehicle, sorted by capacity, in a flow layout.
  In game: "Waiting For Cargo", "Average Happiness: {happiness}", "Average Remaining Delivery Time: {quality}" (German: „Warte auf Güter“, „Durchschnittliche Zufriedenheit: {happiness}“, „Durchschnittliche verbleibende Lieferzeit: {quality}“)
    - returns: The list node, with shipping state `Vehicle`.
    - used 8x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:335`: `children[#children + 1] = cargo_react_util.makeVehicleCargoInfo{`
- `LineCargoInfo : Recipe<LineCargoInfoParams>`: Shows the cargo of all vehicles of a line (count/capacity per cargo type, loading animation and unhappy/delayed indicators).
  In game: "Waiting For Cargo", "{count} Unhappy Passenger / {count} Unhappy Passengers", "{count} Delayed Delivery / {count} Delayed Deliveries" (German: „Warte auf Güter“, „{count} unzufriedener Passagier / {count} unzufriedene Passagiere“, „{count} verspätete Lieferung / {count} verspätete Lieferungen“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/line/line_eow.script.tl:507`: `cargo_react_util.LineCargoInfo {`
- `WarehouseCargoInfo : Recipe<WarehouseCargoInfoParams>`: Shows the stock of a warehouse per cargo type (count/capacity, optional tendency, unhappy/delayed indicators).
  In game: "Waiting For Cargo", "Average Remaining Delivery Time: {quality}" (German: „Warte auf Güter“, „Durchschnittliche verbleibende Lieferzeit: {quality}“)
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/warehouse/warehouse_eow.script.tl:94`: `cargo_react_util.WarehouseCargoInfo {`
- `makeIcon : function(class : string, icon : string, tooltip : string, forceFocusable? : boolean) : TreeNodeId`  (call as `cargo_react_util.makeIcon(…)`): Builds a `builtin.ImageView` with the given style class, image path and tooltip.
    - param `class`: Style class.
    - param `icon`: Image path.
    - param `tooltip`: Tooltip text.
    - param `forceFocusable`: Passed to `meta.forceFocusable`.
- `makeCargoIcon : function(CargoTypeId, ?string, ?string, withoutTooltip? : boolean, forceFocusable? : boolean, tooltipModifier? : function(string) : string) : TreeNodeId`  (call as `cargo_react_util.makeCargoIcon(…)`): Builds the icon of a cargo type with its name as tooltip.
    - param `#1`: Cargo type id.
    - param `#2`: Style class.
    - param `#3`: Image path to use instead of the cargo type's icon.
    - param `withoutTooltip`: If `true`, no tooltip is set.
    - param `forceFocusable`: Passed to `meta.forceFocusable`.
    - param `tooltipModifier`: Called with the tooltip text (or `nil`); its result becomes the tooltip.
    - used 15x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:319`: `local child = cargo_react_util.makeCargoIcon(cargoTypeId, cargoState:old().overflow and "text-icon-size-hack" or "")`
- `makeCargoClassIcon : function(CargoTypeId, ?string) : TreeNodeId`  (call as `cargo_react_util.makeCargoClassIcon(…)`): Builds a horizontal row with the icons of all cargo classes of a cargo type, each with the class name as tooltip.
    - param `#1`: Cargo type id.
    - param `#2`: Style class of the icons.
- `makeTooltipStockCompatibility : function(Engine.Component.TransportVehicle.LoadSpeedModifiers) : string`  (call as `cargo_react_util.makeTooltipStockCompatibility(…)`): Builds the tooltip text "Specialized Warehouse" with the load speed boost from warehouse specialization.
  In game: "Specialized Warehouse: {boost}" (German: „Spezialisiertes Lager: {boost}“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:2236`: `tooltip = tooltip .. cargo_react_util.makeTooltipStockCompatibility(loadSpeedModifiers)`
- `makeTooltipVehicleCompatibility : function(Engine.Component.TransportVehicle.LoadSpeedModifiers) : string`  (call as `cargo_react_util.makeTooltipVehicleCompatibility(…)`): Builds the tooltip lines that explain the terminal part of the load speed (specialized terminal, mismatched specialization, terminal too short).
  In game: "Specialized Terminal: {boost}", "Specializations Don't Match: {penalty}", "Terminal Too Short: {penalty}" (German: „Spezialisiertes Terminal: {boost}“, „Spezialisierungen stimmen nicht überein: {penalty}“, „Terminal zu kurz: {penalty}“)
    - returns: Translated lines joined with newlines.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:2230`: `tooltip = tooltip .. cargo_react_util.makeTooltipVehicleCompatibility(loadSpeedModifiers)`
- `makeCargoIconLabelIndicator : function(CargoIconLabelIndicatorParams) : TreeNodeId`  (call as `cargo_react_util.makeCargoIconLabelIndicator(…)`): Builds an icon/label indicator for a cargo type with the cargo icon and name filled in.
    - notes: Calls `GuiReactUtil.makeContentLabelIndictator` with the cargo icon(s) as `iconPath` and the cargo name(s) as `iconTooltip`.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:150`: `cargo_react_util.makeCargoIconLabelIndicator{`
- `CargoList : Recipe<CargoListParams>`
- `LocationCargoList : Recipe<LocationCargoListParams>`: Lists the cargo types at a location, one entry per cargo type built by `getCargoItemLayout`.
    - notes: The data is refreshed on a timer (`engine_react_util.useStepStateTimer`). Passengers are sorted first; the unhappy-passenger indicator is inserted right after them and the spoiled-cargo indicator at the end.
- `CargoInputOutputIndicator : Recipe<CargoInputOutputParams>`: Shows input cargo, an arrow and output cargo of a stock list, as used on industry HUD icons and in the cargo flow layer.
  In game: "Production", "Consumption", "Any Cargo" (German: „Produktion“, „Verbrauch“, „Beliebiger Gütertyp“)
    - notes: If any entry has neither `count` nor `max`, the compact form with icons only is drawn. Otherwise each entry is a cargo icon with its text label, with tooltip "Consumption" for inputs and "Production" for outputs.
    - used 13x in the base game, e.g. `base/content/gui/gui/construction/constructionHudIconMaster.tl:170`: `hudIcons[#hudIcons + 1] = cargo_react_util.CargoInputOutputIndicator{`
- `CargoIconLabelIndicator : Recipe<CargoIconLabelIndicatorParams>`: Recipe version of `makeCargoIconLabelIndicator`.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:242`: `children[#children + 1] = cargo_react_util.CargoIconLabelIndicator{`
- `makeCargoInputOutputDataForStorage : function(entity : Engine.Entity) : CargoReactUtil.CargoInputOutputData`  (call as `cargo_react_util.makeCargoInputOutputDataForStorage(…)`): Builds `CargoInputOutputData` for a storage, listing the accepted cargo types (or "any cargo") as inputs without counts.
    - param `entity`: Stock list entity.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_hud_util.tl:920`: `stockList = cargo_react_util.makeCargoInputOutputDataForStorage(hudIconParam.entity),`
- `makeCargoInputOutputDataForStocklist : function(entity : Engine.Entity, inputCount : boolean, inputMax : boolean, outputCount : boolean, outputMax : boolean) : CargoInputOutputData`  (call as `cargo_react_util.makeCargoInputOutputDataForStocklist(…)`): Builds `CargoInputOutputData` from the inputs and outputs of the rules of a stock list.
    - param `entity`: Stock list entity (e.g. `Industry.stockList`).
    - param `inputCount`: If `true`, fills `count` of inputs with the consumption per year.
    - param `inputMax`: If `true`, fills `max` of inputs with the maximum consumption per year.
    - param `outputCount`: If `true`, fills `count` of outputs with the production per year.
    - param `outputMax`: If `true`, fills `max` of outputs with the maximum production per year.

    ```lua
    -- from layer_cargo_flow.tl
    stockListData = cargo_react_util.makeCargoInputOutputDataForStocklist(param.entity, false, true, false, true)
    ```

    - used 8x in the base game, e.g. `base/content/gui/gui/construction/constructionHudIconMaster.tl:137`: `return cargo_react_util.makeCargoInputOutputDataForStocklist(param.entity, false, false, false, false)`
- `makeCargoInputOutputDataForConstructionData : function(entity : Engine.Entity) : CargoInputOutputData`  (call as `cargo_react_util.makeCargoInputOutputDataForConstructionData(…)`): Builds `CargoInputOutputData` with the cargo requirements of a construction as inputs.
    - param `entity`: Construction entity.
    - notes: Reads the requirements from the construction's persistent metadata. Inputs are only added while the construction is above level `0` and below its last level.
- `CargoClassSpecialization : Recipe<CargoSpecializationParams>`: Shows a badge per cargo class with the class name and a border in the class colour.
  In game: "Passengers", "Passengers and All Cargo Types", "All Cargo" (German: „Passagiere“, „Passagiere und alle Güterarten“, „Alle Güter“)
    - notes: `PASSENGERS` alone shows the text "Passengers"; `UNIVERSAL` together with `PASSENGERS` shows "Passengers and All Cargo Types"; `UNIVERSAL` alone is shown as "All Cargo".
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3393`: `cargo_react_util.CargoClassSpecialization {`

### CargoReactUtil.VehicleCargoInfoParams (record)

```teal
record VehicleCargoInfoParams
```

Parameters of `makeVehicleCargoInfo`.

- `vehicleEntity : Engine.Entity`: Vehicle whose cargo is shown.
- `showEmpty : boolean`: show with number 0 if empty
- `showLoadingIndicator : boolean`: If `true`, shows the loading/unloading animation while the vehicle is at a terminal (not in `EntityWindow` mode).
- `mode : GuiReactUtil.LabelMode`: Label mode of the entries. `EntityWindow` also groups unset capacities and adds the unhappy/delayed indicators.
- `cargoTypeFilter : function(CargoTypeId) : boolean`: Function that returns `true` for cargo types to show.
- `getEmptyItem : function() : TreeNodeId`: use special recipe if empty

### CargoReactUtil.LineCargoInfoParams (record)

```teal
record LineCargoInfoParams
```

Parameters of the `LineCargoInfo` recipe.

- `lineEntity : Engine.Entity`: Line whose cargo is shown.
- `makeEmptyItem : TreeNodeId`: Shown when the line carries nothing. If `nil`, empty cargo types are listed instead.
- `mode : GuiReactUtil.LabelMode`: Label mode of the entries.

### CargoReactUtil.WarehouseCargoInfoParams (record)

```teal
record WarehouseCargoInfoParams
```

Parameters of the `WarehouseCargoInfo` recipe.

- `warehouseEntity : Engine.Entity`: Warehouse (or other stock list owner) whose stock is shown.
- `makeEmptyItem : TreeNodeId`: Shown when the stock is empty. If `nil`, empty cargo types are listed instead (except in `HudIconLayer` mode).
- `showTendency : boolean`: If `true`, shows tendency arrows (not in the HUD modes).
- `mode : GuiReactUtil.LabelMode`: Label mode of the entries.

### CargoReactUtil.CargoIconLabelIndicatorParams (record)

```teal
record CargoIconLabelIndicatorParams
```

Parameters of `makeCargoIconLabelIndicator` and `CargoIconLabelIndicator`.

- `cargoType : CargoTypeId`: Cargo type whose icon is shown. Ignored when `cargoTypes` is set.
- `cargoTypes : {CargoTypeId}`: Several cargo types; their icons are shown side by side, each with its name as tooltip.
- `iconStyle : string`
- `label : string`: Text label, usually the count.
- `richLabel : GuiReactUtil.RichLabel`: Label with text and icon; takes precedence over `label`.
- `labelFontColor : GuiReactUtil.LabelFontColor`: Colour of the label.
- `labelMode : GuiReactUtil.LabelMode`: Layout mode.
- `labelTooltip : string`: Tooltip of the label.
- `indicatorIconPath : string`: Image path of a small indicator icon next to the label, e.g. a tendency arrow.
- `iconOverride : string`
- `loadingIndicator : TreeNodeId`: Node shown above the indicator, e.g. the loading animation.
- `shippingState : GuiReactUtil.ShippingState`: Where the cargo is; adds the matching style class.

### CargoReactUtil.CargoListParams (record)

```teal
record CargoListParams
```

- `gameCtx : GameContext`
- `cargoTypeLoadSpeed : {{CargoTypeId, number}}`
- `allCargo : boolean`

### CargoReactUtil.LocationCargoListParams (record)

```teal
record LocationCargoListParams
```

Parameters of the `LocationCargoList` recipe.

- `locationParams : CargoUtil.CargoLocationParams`: Where to collect the cargo (vehicle, stock list owner, line).
- `cargoTypeFilter : function(CargoTypeId) : boolean`: Passed to `CargoUtil.getSortedProducedCargoTypes` (see the note there on when it takes effect).
- `layout : LocationCargoListLayout`: Arrangement of the entries. Anything other than `FLOW` or `VERTICAL` gives a horizontal box.
- `sortOrder : CargoUtil.CargoListSortOrder`: Sort order of the cargo types.
- `groupIfAllEmpty : boolean`: If `true` and the location is a vehicle with `showEmpty`, adds the vehicle's unset capacities (single-type capacities to their cargo type, multi-type capacities as extra entries).
- `getCargoItemLayout : function(CargoUtil.CargoTypeInfo, GuiReactUtil.ShippingState) : TreeNodeId`: Builds the node of one cargo type entry.
- `getFrownyIconLayout : function(summary : UtilCargo.CargoQualityData, cargoIsPassengers : boolean, shippingState : GuiReactUtil.ShippingState) : TreeNodeId`: If set, builds the indicator for unhappy passengers and spoiled cargo; called only when the location has such items.
- `getEmptyItem : function() : TreeNodeId`: Builds the node shown when the list has no entries.
- `shippingState : GuiReactUtil.ShippingState`: Shipping state passed to the item layout functions.

#### CargoReactUtil.LocationCargoListParams.LocationCargoListLayout (enum)

```teal
enum LocationCargoListLayout
```

How the entries of a `LocationCargoList` are arranged.

- `"VERTICAL"`: Vertical box layout.
- `"HORIZONTAL"`: Horizontal box layout.
- `"FLOW"`: Flow layout that wraps entries into rows.

### CargoReactUtil.CargoInputOutputData (record)

```teal
record CargoInputOutputData
```

Input and output cargo of a stock list for `CargoInputOutputIndicator`.

- `inputs : {CargoData}`: Input cargo entries.
- `outputs : {CargoData}`: Output cargo entries.

#### CargoReactUtil.CargoInputOutputData.CargoData (record)

```teal
record CargoData
```

One cargo entry of `CargoInputOutputData`.

- `count : integer`: Current amount, e.g. consumed or produced per year. `nil` if not shown.
- `max : integer`: Maximum amount, e.g. maximum consumption or production per year. `nil` if not shown.
- `cargoType : CargoTypeId`: Cargo type id.
- `allCargoType : boolean`: `true` if the entry stands for any cargo; shown with the mixed cargo icon.

### CargoReactUtil.CargoInputOutputParams (record)

```teal
record CargoInputOutputParams
```

Parameters of the `CargoInputOutputIndicator` recipe.

- `data : CargoInputOutputData`: Input and output cargo to show.
- `inputDistrict : LandUseType`: optional, default is nil
- `outputDistrict : LandUseType`: optional, default is nil
- `showPercentage : boolean`: If `true`, entries with `count` and `max` show `count / max` as a percentage instead of `count/max`.

### CargoReactUtil.CargoSpecializationParams (record)

```teal
record CargoSpecializationParams
```

Parameters of the `CargoClassSpecialization` recipe.

- `cargoClasses : {string}`: Cargo class names, e.g. from `CargoUtil.getSpecializationClasses`. Duplicates are removed.
- `horizontalLayout : boolean`: If `true`, the badges are placed side by side, otherwise below each other.
