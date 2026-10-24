# gui.entity_window.entity_window_util

Definition file: `base/tealdef/gui/entity_window/entity_window_util.d.tl` (in the game folder).

Shared React recipes and helpers for the entity windows (info windows of vehicles, stations,
industries, towns, persons ...): stock bars, charts, action button bars, rating and progress
bars, person tables, notification cards. Loaded with
`ug_require "/gui/entity_window/entity_window_util.tl"`.

## Globals

- `PopoverReactUtil` (type): Popover window recipes.

## EntityWindowUtil (record)

```teal
global record EntityWindowUtil
```
Load: `local entity_window_util = ug_require "::/gui/entity_window/entity_window_util.tl" as EntityWindowUtil`

Library of React recipes and helper functions shared by the entity windows.

Notes:

Most members are recipes (`Recipe<...>`): call them with their parameter record to get a
`TreeNodeId`, e.g. `entity_window_util.ProgressDisplay{ value = 0.5, text = "50%" }`.
Many of them poll engine state every step with `engine_react_util.useStepState`.

- `VehicleCell : Recipe<Builtin.TableCellParam>`: Table cell recipe showing the vehicle name and vehicle icon of the row's vehicle as a button that selects (opens) the vehicle.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station_eow.script.tl:202`: `recipe = entity_window_util.VehicleCell,`
- `VehicleTable : Recipe<VehicleTableParams>`: Sortable data table of vehicles; rows come from `getVehicleEntities` and are refreshed every step.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station_eow.script.tl:223`: `entity_window_util.VehicleTable,`
- `VehicleCondition : Recipe<VehicleConditionParams>`: Progress bar of the average vehicle condition (maintenance state) with condition text and trend arrow.
  In game: "Vehicle Condition: {amount}" (German: „Fahrzeugzustand: {amount}“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station_eow.script.tl:21`: `entity_window_util.VehicleCondition {`
- `MaintenanceBar : Recipe<MaintenanceBarParam>`: Info callout showing the upkeep or running cost of an entity per period: station group
maintenance, vehicle running cost, or the scaled maintenance of a construction (or the parent
construction of a subconstruction). Renders nothing when the cost is 0.
  In game: "Running Costs", "Upkeep" (German: „Betriebskosten“, „Unterhalt“)
    - used 5x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station.tl:193`: `local maintenanceBar = entity_window_util.MaintenanceBar{`
- `MaintenanceStationPoolProgressBar : Recipe<MaintenanceStationPoolProgressBarParams>`: Progress bar of the vehicles assigned to a maintenance station against its maintenance pool size; shows "No Maintenance Pool" when the pool is 0.
  In game: "Pool Usage: {amount}", "No Maintenance Pool" (German: „Pool-Nutzung: {amount}“, „Kein Wartungspool“)
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/constructionHudIconMaster.tl:126`: `hudIcons[#hudIcons + 1] = entity_window_util.MaintenanceStationPoolProgressBar({`
- `EntityStockCountsWidget : Recipe<StockCountsWidgetParams>`: List of an entity's stocks grouped by cargo class specialization, each with cargo icons, stored/capacity text and a fill bar.
  In game: "Specialization:", "All Cargo Types", "Discard All Cargo" (German: „Spezialisierung:“, „Alle Güterarten“, „Alle Güter entsorgen“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/warehouse/warehouse_eow.script.tl:129`: `entity_window_util.EntityStockCountsWidget,`
- `CombinedStockGroupedCounts : Recipe<CombinedStockGroupedCountsParam>`: "Input Stocks" and "Output Stocks" content cards of an entity's stock list, with compact cargo counts and an optional collapsible part.
  In game: "Input Stocks", "Output Stocks" (German: „Eingangsbestände“, „Ausgangsbestände“)
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:770`: `child = entity_window_util.CombinedStockGroupedCounts{`
- `EntityDiagramWidget : Recipe<DiagramParams>`: Chart of an entity's logbook or account history with optional legend and time scale slider; refreshed every step.
  In game: "Time Scale", "{years} Years" (German: „Zeitrahmen“, „{years} Jahre“)
    - used 17x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:699`: `entity_window_util.EntityDiagramWidget(react.ref(chartRef), {`
- `EntityWindowButton : Recipe<EntityWindowButtonParams>`: Icon button with tooltip.
- `ActionButtonBar : Recipe<ActionButtonBarParams>`: Horizontal bar of primary buttons (icon and text) followed by secondary icon buttons.
    - used 8x in the base game, e.g. `base/content/gui/gui/entity_window/empty_construction/empty_construction.tl:44`: `entity_window_util.ActionButtonBar {`
- `PersonCapacityWidget : Recipe<WorkplacesProgressBarWidgetParams>`: Person icon (tooltip "Residents", "Customers" or "Workers" by land use type) followed by a `WorkplacesProgressBarWidget`.
  In game: "Residents", "Customers", "Workers" (German: „Einwohner“, „Kunden“, „Arbeiter“)
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/perk/perk_eow.script.tl:243`: `entity_window_util.PersonCapacityWidget,`
- `GetDestinationIcon : function(DestinationIconParams) : TreeNodeId`  (call as `entity_window_util.GetDestinationIcon(…)`): Creates the icon of a sim person destination (home, shop or work).
  In game: "Home", "Shop", "Work" (German: „Wohnsitz“, „Einkaufen“, „Arbeit“)
    - param `#1`: Destination index (0 home, 1 shop, 2 work), icon style class and optional tooltip.
    - returns: An image widget.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person_eow.script.tl:83`: `entity_window_util.GetDestinationIcon{`
- `SimPersonDestinationButton : Recipe<SimPersonDestinationButtonParams>`: Button with the destination icon that selects the sim person's destination entity of the given destination index.
- `GetMoveModeIcon : function(MoveMode) : TreeNodeId`  (call as `entity_window_util.GetMoveModeIcon(…)`): Creates an icon with tooltip for a move mode (walk, car, lines).
  In game: "Walk", "Car", "Lines" (German: „Zu Fuss“, „Auto“, „Linien“)
    - returns: An image widget; with an empty path for other move modes.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person_eow.script.tl:182`: `entity_window_util.GetMoveModeIcon(param.moveMode),`
- `GetInhabitantsTitle : function(entityId : Engine.Entity, landmark? : boolean) : string`  (call as `entity_window_util.GetInhabitantsTitle(…)`): Returns the title for the persons of a building by its land use type.
  In game: "Residents", "Customers", "Visitors" (German: „Einwohner“, „Kunden“, „Besucher“)
    - param `entityId`: Entity with a `PERSON_CAPACITY` component.
    - param `landmark`: Use the landmark wording.
    - returns: "Residents" for residential; "Customers" (landmark: "Visitors") for commercial; "Workers" for industrial.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/perk/perk_eow.script.tl:174`: `local title = entity_window_util.GetInhabitantsTitle(params.state.personCapacityEntity, true)`
- `InhabitantsTable : Recipe<InhabitantsTableParam>`: Table of the sim persons of a building (name, satisfaction, whether currently there or on the way, move mode for non-residential); shows a public transport hint when empty.
  In game: "Currently Here", "On the Way", "Connect this location with public transport to enable residents to live here." (German: „Derzeit hier“, „Unterwegs“, „Erschliesse diesen Standort mit dem öffentlichen Verkehr, um den Einwohnern zu ermöglichen, dort zu leben.“)
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/perk/perk_eow.script.tl:221`: `entity_window_util.InhabitantsTable,`
- `WorkersTable : Recipe<WorkersTableParam>`: Table of the workers (visitors) of an entity with the time each has left; shows a public transport hint when empty.
  In game: "Time Left", "Connect this location with public transport to allow residents to work here." (German: „Zeit übrig“, „Erschliesse diesen Standort mit dem öffentlichen Verkehr, um den Einwohnern zu ermöglichen, dort zu arbeiten.“)
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:645`: `entity_window_util.WorkersTable,`
- `EnterCockpitButton : Recipe2<GameContext, Engine.Entity>`: Primary "Enter Cockpit" button that pushes the free camera tool in cockpit mode for the given entity.
  In game: "Enter Cockpit" (German: „Im Cockpit Platz nehmen“)
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane_entity_window.script.tl:36`: `item = entity_window_util.EnterCockpitButton(params.gameCtx, entityId)`
- `WorkplacesProgressBarWidget : Recipe<WorkplacesProgressBarWidgetParams>`: Occupied/capacity text and progress bar of a `PERSON_CAPACITY` entity (number of sim persons with it as destination).
- `PassengersWidget : Recipe<PassengerWidgetParams>`: Table of the sim persons in a vehicle with satisfaction and their other destinations; shows "Currently no passengers in this vehicle." when empty.
  In game: "Currently no passengers in this vehicle." (German: „Aktuell keine Passagiere im Fahrzeug.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:1004`: `entity_window_util.PassengersWidget,`
- `RatingBar : Recipe<RatingBarParam>`: Segmented rating bar with optional icon, trend arrow and rating text.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person_eow.script.tl:279`: `entity_window_util.RatingBar{`
- `SubconstructionBalance : Recipe<SubconstructionBalanceParams>`: Money text (styled negative or positive) of the balance of a subconstruction's parent construction over the last game year.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station_eow.script.tl:131`: `entity_window_util.SubconstructionBalance{`
- `SpeedIndicator : Recipe<Engine.Entity>`: Status lines of a vehicle or a sim person in a car: speed, "Stopped"/"Stopping", "No Path",
"Waiting for Free Path", or loading/unloading per cargo type with loading speed penalties at a terminal.
  In game: "Stopped", "No Path", "Waiting for Free Path" (German: „Angehalten“, „Kein Weg“, „Warte auf freie Wege“)
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person.tl:158`: `child = entity_window_util.SpeedIndicator(personEntityId),`
- `SendCommandButton : Recipe<SendCommandButtonParams>`: Icon button that sends the command built by `makeCommand` when clicked and is enabled according to `isActive`.
- `NoiseAndPollution : Recipe<NoiseAndPollutionParams>`: Text line with the current noise and pollution values.
  In game: "Current Noise: {formattedNoise}", "Current Pollution: {formattedPollution}" (German: „Aktueller Lärm: {formattedNoise}“, „Aktuelle Verschmutzung: {formattedPollution}“)
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/line/line_eow.script.tl:584`: `entity_window_util.NoiseAndPollution,`
- `ProgressDisplay : Recipe<ProgressDisplayParam>`: Progress bar with optional icon, centred text, a second (extra) fill value, trend arrow and extra child.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/industry_react_util.tl:194`: `entity_window_util.ProgressDisplay{`
- `makeConfigureOnClickFunction : function(constructionEntity : Engine.Entity) : function()`  (call as `entity_window_util.makeConfigureOnClickFunction(…)`): Returns a click handler that opens the module builder of a construction in the construction menu.
    - param `constructionEntity`: Construction entity to configure.
    - returns: Function that does nothing if the entity no longer exists or is not owned by the player.
    - notes: The handler fires the `setModuleBuilderEntity` and `constructionMenuSetTab` events.
    - used 6x in the base game, e.g. `base/content/gui/gui/entity_window/empty_construction/empty_construction.tl:49`: `onClick = entity_window_util.makeConfigureOnClickFunction(entityId),`
- `makeRecipeAndParam : function<T>(recipe : Recipe<T>, param : T) : RecipeAndParamErased`  (call as `entity_window_util.makeRecipeAndParam(…)`): Bundles a recipe and its parameter into a `RecipeAndParamErased`.
    - returns: The bundle, e.g. for `CombinedStockGroupedCountsParam.recipeAndParamInputCollapsible`.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:773`: `recipeAndParamInputCollapsible = entity_window_util.makeRecipeAndParam(`
- `makeStockState : function(entity : Engine.Entity) : {CargoUtil.StockConfig}`  (call as `entity_window_util.makeStockState(…)`): Calculates the stock configurations of an entity, sorted by stock type.
    - param `entity`: Entity to read the `STOCK_LIST` component from.
    - returns: Stock configurations, or nil if the entity has no stock list.
- `createStockIcon : function(stockConfigs : {CargoUtil.StockConfig}, entity : Engine.Entity) : TreeNodeId`  (call as `entity_window_util.createStockIcon(…)`): Builds cargo icons with counts for stock configurations (storage stocks per cargo type, output stocks with the stored amount).
    - param `stockConfigs`: Stock configurations, e.g. from `makeStockState`.
    - param `entity`: Entity owning the stocks.
    - returns: A vertical box with one row of icons per stock configuration.
- `getVehicleName : function(vehicleEntity : Engine.Entity) : string`  (call as `entity_window_util.getVehicleName(…)`): Returns the name of a vehicle.
    - returns: The entity name, or "Invalid" if it has none.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station_eow.script.tl:198`: `local aName = entity_window_util.getVehicleName(a)`
- `getEntityLog : function(entity : Engine.Entity, logNames : {string}, refLogNames : {string}, keepEntries : integer, hideLogsNoChange : {string}, currentState? : EntityWindowUtil.EntityLog.LoggedState) : EntityWindowUtil.EntityLog`  (call as `entity_window_util.getEntityLog(…)`): Builds a change history (newest first) from an entity's logbook.
    - param `entity`: Entity with a `LOG_BOOK` component.
    - param `logNames`: Plain logbook entries to read.
    - param `refLogNames`: Logbook entries recorded per reference entity to read.
    - param `keepEntries`: Maximum number of changes to return.
    - param `hideLogsNoChange`: Log names whose entries are skipped when the value did not change.
    - param `currentState`: Optional current value appended as the newest state.
    - returns: The change history.
    - notes: Logged values are divided by 100 and rounded. Consecutive states of the same place and log are merged and their durations added. Used by the sim person window (happiness history) and the vehicle window (maintenance history).
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person_eow.script.tl:374`: `return entity_window_util.getEntityLog(params.personEntity, {happinessWalkLogName, happinessCarLogName, happinessResidentialLogName, happinessCommercialLogName,`
- `isConstructionModifiableEngine : function(entity : Engine.Entity) : boolean`  (call as `entity_window_util.isConstructionModifiableEngine(…)`): Returns whether a construction can be modified after building.
    - param `entity`: Construction entity.
    - returns: True if the construction has module slots or one of its construction parameters is `postConstructionModifiable`.
    - notes: Returns false if the entity has no construction component.
    - used 5x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:186`: `if constructionEntity >= 0 and entity_window_util.isConstructionModifiableEngine(constructionEntity) then`
- `NotificationWidget : Recipe<Engine.Entity>`: List of cards for the persisting notifications attached to an entity; renders nothing if there are none.
    - used 8x in the base game, e.g. `base/content/gui/gui/entity_window/empty_construction/empty_construction_eow.script.tl:12`: `entity_window_util.NotificationWidget(params.entityId)`
- `ContentWidgetScrollContainer : Recipe2<{TreeNodeId}, boolean>`: Vertical scroll area for the content widgets of an entity window; the boolean parameter adds the `first` style class.
    - used 19x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane_entity_window.script.tl:45`: `entity_window_util.ContentWidgetScrollContainer({builtin.BoxLayout{`

### EntityWindowUtil.StockCountsWidgetParams (record)

```teal
record StockCountsWidgetParams
```

Parameters of `EntityWindowUtil.EntityStockCountsWidget`.

- `entityId : Engine.Entity`: Entity with a stock list (`STOCK_LIST` component) whose stocks are shown.
- `width : integer`
- `iconWidth : integer`
- `maxIcons : integer`: Maximum number of cargo icons per row before the icons wrap into two rows; defaults to 5.
- `groupStocks : boolean`: Passed to `cargo_util.calculateStockConfigs` to group stocks into combined entries.
- `addDiscardCargoButtons : boolean`: Adds a trash button to each stock that sends a `makeStockListDiscardCargoCmd` command discarding all stored cargo.
- `addButtonsToSetStockCargoType : boolean`: Turns the cargo icons into buttons that pin the stock to that cargo type (for stocks that allow it).
The pin is stored in the construction's `tagToCargoType` parameter and applied by rebuilding the construction.
- `sortByType : boolean`: Passed to `cargo_util.calculateStockConfigs` to sort the stock entries by cargo type.

### EntityWindowUtil.DiagramType (enum)

```teal
enum DiagramType
```

Data source of an `EntityWindowUtil.EntityDiagramWidget` chart.

- `"logbook"`: Values of the entity's logbook, read with `api.engine.util.logbook.getChart`; x axis labelled with years.
- `"logbookDiff"`: Differences of the entity's logbook values, read with `api.engine.util.logbook.getChartDiff`.
- `"account"`: Account (finance) chart of the entity, read with `api.engine.util.finance.getAccountChart`.

### EntityWindowUtil.DiagramParams (record)

```teal
record DiagramParams
```

Parameters of `EntityWindowUtil.EntityDiagramWidget`.

- `entityId : Engine.Entity`: Entity whose logbook or account is charted.
- `logNames : {string}`: Logbook entry names to chart (one series each); used by the `logbook` and `logbookDiff` chart types.
- `labels : {string}`: Series labels, also shown in the legend; index matches the series.
- `colors : {Vec4f}`: Series colours (`Vec4f` RGBA), also used for the legend bars.
- `types : {Builtin.SeriesParam.Type}`: Series type per series (e.g. `"BAR"`); defaults to `"BAR"`. Bars are drawn 10 wide, other types 2.
- `styleClass : string`: Extra style class added to the chart widget.
- `chartType : DiagramType`: Data source of the chart, see `EntityWindowUtil.DiagramType`.
- `yMin : number`: Lower y limit passed to `logbook.getChart` (only for the `logbook` chart type).
- `yMax : number`: Upper y limit passed to `logbook.getChart` (only for the `logbook` chart type).
- `yAxisLabelFormatter : function(number) : string`: Formats y axis labels; defaults to `lang_util.formatNumber(val, 0)`.
- `showTimeScaleSlider : boolean`: Shows a "Time Scale" slider to choose how many years are displayed; defaults to true. Hidden when fewer than 4 years can be shown.
- `showLegend : boolean`: Shows the legend (label and colour per series); defaults to true.
- `initialNumYearsToDisplay : integer`: Number of years shown initially; defaults to 5.
- `minNumYearsToDisplay : integer`: Minimum number of years (set as the chart config's `minCount` and as the slider limit); defaults to 3.
- `maxNumYearsToDisplay : integer`: Upper limit of the time scale slider, in years; defaults to 50.
- `legendOrientation : Builtin.Type.Orientation`: Orientation of the widget's outer box layout.
- `yStepScale : number`: Set as `yStepScale` of the chart config when given.
- `currentValueFns : {function() : number}`: Optional functions, one per series, returning the current value; the result replaces the last
value of that series (`api.engine.util.chart.setLastValue`), so the chart shows live values.
- `focusSlider : boolean`: When true the time scale slider can take gamepad/keyboard focus (and the chart cannot).

### EntityWindowUtil.ActionBarButton (record)

```teal
record ActionBarButton
```

One button of an `EntityWindowUtil.ActionButtonBar`.

- `isSpacer : boolean`: Inserts a horizontal spacer instead of a button.
- `icon : string`: Image path of the button icon.
- `description : string`: Button text for primary buttons; tooltip for secondary buttons.
- `onClick : function()`: Called when a normal (non-toggle) button is clicked.
- `tag : string`: Tag set on the button widget (used to find it, e.g. by the guide system).
- `customItem : TreeNodeId`: Ready-made widget used instead of a generated button.
- `sound : Sound`: Click sound, see `EntityWindowUtil.ActionBarButton.Sound`.
- `toggleButton : boolean`: Creates a toggle button instead of a normal button; use `value` and `onValueChange`.
- `onValueChange : function(boolean)`: Called with the new state when a toggle button changes.
- `value : boolean`: Current state of a toggle button.

#### EntityWindowUtil.ActionBarButton.Sound (enum)

```teal
enum Sound
```

Sound style of an action bar button (mapped to the style classes `buy-sfx`, `send-to-depot-sfx`, `sell-sfx`).

- `"Buy"`: Plays the buy sound (`buy-sfx` style class).
- `"SendToDepot"`: Plays the send-to-depot sound (`send-to-depot-sfx` style class).
- `"Sell"`: Plays the sell sound (`sell-sfx` style class).

### EntityWindowUtil.ActionButtonBarParams (record)

```teal
record ActionButtonBarParams
```

Parameters of `EntityWindowUtil.ActionButtonBar`.

- `primaryButtons : {ActionBarButton}`: Buttons shown first, with icon and text (`primary` style).
- `secondaryButtons : {ActionBarButton}`: Icon-only buttons after a spacer, with the description as tooltip (`secondary` style).
- `nextUpFocusRef : ReactRefWrap`: Widget reference that gets focus when navigating up (`IA_NAV_UP`) from the bar.

### EntityWindowUtil.EntityWindowButtonParams (record)

```teal
record EntityWindowButtonParams
```

Parameters of `EntityWindowUtil.EntityWindowButton`.

- `tooltip : string`: Tooltip of the button.
- `icon : string`: Image path of the button icon.
- `onClick : function()`: Called when the button is clicked.

### EntityWindowUtil.WorkplacesProgressBarWidgetParams (record)

```teal
record WorkplacesProgressBarWidgetParams
```

Parameters of `EntityWindowUtil.WorkplacesProgressBarWidget` and `EntityWindowUtil.PersonCapacityWidget`.

- `entityId : Engine.Entity`: Entity with a `PERSON_CAPACITY` component.
- `progressBarThresholds : {number}`: Threshold marks drawn on the progress bar (fractions 0 to 1).

### EntityWindowUtil.SimPersonDestinationButtonParams (record)

```teal
record SimPersonDestinationButtonParams
```

Parameters of `EntityWindowUtil.SimPersonDestinationButton`.

- `simPersonEntityId : Engine.Entity`: Sim person entity whose destination is shown.
- `destinationIndex : integer`: Note: this is a LandUseType
- `addIconIfMissing : boolean`: Shows the destination icon (without button) when the person has no such destination.

### EntityWindowUtil.InhabitantsTableParam (record)

```teal
record InhabitantsTableParam
```

Parameters of `EntityWindowUtil.InhabitantsTable`.

- `landmark : boolean`: Uses the landmark hint text ("... enable residents to visit it") when the table is empty.
- `headquarters : boolean`: Uses the headquarters hint text ("... allow residents to work here") when the table is empty.
- `personCapacityEntityId : Engine.Entity`: Entity with a `PERSON_CAPACITY` component whose sim persons are listed.

### EntityWindowUtil.WorkersTableParam (record)

```teal
record WorkersTableParam
```

Parameters of `EntityWindowUtil.WorkersTable`.

- `personCapacityEntityId : Engine.Entity`: Entity whose visitors (from `industry_productivity_util.getVisitors`) are listed.

### EntityWindowUtil.DestinationIconParams (record)

```teal
record DestinationIconParams
```

Parameters of `EntityWindowUtil.GetDestinationIcon`.

- `destinationIndex : integer`: Destination whose icon is shown (0 home, 1 shop, 2 work, matching the land use type).
- `iconClass : string`: Style class of the icon.
- `tooltipOverride : string`: Tooltip used instead of the default "Home" / "Shop" / "Work".

### EntityWindowUtil.PassengerWidgetParams (record)

```teal
record PassengerWidgetParams
```

Parameters of `EntityWindowUtil.PassengersWidget`.

- `vehicleEntity : Engine.Entity`: Vehicle whose passengers are listed.

### EntityWindowUtil.VehicleTableColumn (record)

```teal
record VehicleTableColumn
```

Column definition of an `EntityWindowUtil.VehicleTable`.

- `name : string`: Column header text.
- `recipe : Recipe<Builtin.TableCellParam>`: Cell recipe; the cell's `rowKey` is the vehicle entity (e.g. `EntityWindowUtil.VehicleCell`).
- `sortOrder : function(Engine.Entity, Engine.Entity) : boolean`: Comparison of two vehicle entities used when the table is sorted by this column.
- `headerStyleClass : string`: Style class of the column header; not passed on to the table by `EntityWindowUtil.VehicleTable`.
- `weight : number`: Width weight of the column; not passed on to the table by `EntityWindowUtil.VehicleTable`.

### EntityWindowUtil.VehicleTableParams (record)

```teal
record VehicleTableParams
```

Parameters of `EntityWindowUtil.VehicleTable`.

- `entityId : Engine.Entity`: Entity passed to `getVehicleEntities`, e.g. a line or depot.
- `getVehicleEntities : function(Engine.Entity) : {Engine.Entity}`: Returns the vehicles (table rows) for `entityId`; polled every step.
- `columns : {VehicleTableColumn}`: Table columns.

### EntityWindowUtil.VehicleConditionParams (record)

```teal
record VehicleConditionParams
```

Parameters of `EntityWindowUtil.VehicleCondition`.

- `entityId : Engine.Entity`: Vehicle (or line) passed to `vehicle_util.getAvgMaintenanceState`.
- `addIcon : boolean`: Shows a tools icon in front of the bar.

### EntityWindowUtil.MaintenanceStationPoolProgressBarParams (record)

```teal
record MaintenanceStationPoolProgressBarParams
```

Parameters of `EntityWindowUtil.MaintenanceStationPoolProgressBar`.

- `entityId : Engine.Entity`: Maintenance station (entity with a `VEHICLE_DEPOT` component).

### EntityWindowUtil.SubconstructionBalanceParams (record)

```teal
record SubconstructionBalanceParams
```

Parameters of `EntityWindowUtil.SubconstructionBalance`.

- `entityId : Engine.Entity`: Subconstruction; the balance of its parent construction is shown.
- `stepTimer : boolean`: Polls with `useStepStateTimer` instead of every step.
- `maintenanceType : JournalEntry.Maintenance`: Maintenance journal entry type passed to `api.engine.util.finance.calculateBalance`.

### EntityWindowUtil.SendCommandButtonParams (record)

```teal
record SendCommandButtonParams
```

Parameters of `EntityWindowUtil.SendCommandButton`.

- `iconPath : string`: Image path of the button icon.
- `tooltip : string | function() : string`: Tooltip text, or a function returning it.
- `makeCommand : function() : Command<ICommandData>`: Builds the command that is sent when the button is clicked.
- `isActive : function() : boolean`: Returns whether the button is enabled; polled every step. Without it the button is always enabled.
- `isPrimary : boolean`: Uses the `primary` button style.
- `tag : string`: Tag set on the button widget.

### EntityWindowUtil.RecipeAndParamErased (record)

```teal
record RecipeAndParamErased
```

A recipe together with its parameter, with the parameter type erased; created with `EntityWindowUtil.makeRecipeAndParam`.

- `recipe : Recipe<any>`: The recipe.
- `param : any`: Parameter to call the recipe with.

### EntityWindowUtil.EntityLog (record)

```teal
record EntityLog
```

Change history built from an entity's logbook by `EntityWindowUtil.getEntityLog`.

- `entries : {Entry}`: Changes, newest first.

#### EntityWindowUtil.EntityLog.Entry (record)

```teal
record Entry
```

One change in an `EntityWindowUtil.EntityLog`.

- `durationSeconds : integer`: Time the previous state lasted before this change, in seconds (game time).
- `change : integer`: Change of the logged value (in tenths of a percent).
- `logName : string`: Logbook entry name of the change.
- `place : Engine.Entity`: Reference entity the change was logged for; -1 for entries of the plain (non-reference) logs.

#### EntityWindowUtil.EntityLog.LoggedState (record)

```teal
record LoggedState
```

One logged value, used internally and as the optional current state passed to `getEntityLog`.

- `time : integer`: Game time of the value, in milliseconds; filled with the current game time when nil.
- `place : Engine.Entity`: Reference entity of the value; -1 for plain logs.
- `logName : string`: Logbook entry name.
- `state : integer`: 10x percent

### EntityWindowUtil.MaintenanceBarParam (record)

```teal
record MaintenanceBarParam
```

Parameters of `EntityWindowUtil.MaintenanceBar`.

- `entity : Engine.Entity`: Entity whose cost is shown; a station group (upkeep), a transport vehicle (running cost), or a construction or subconstruction (maintenance of the construction).
- `maintenancePeriodText : string`: Period text shown after the cost, as in `"{maintenanceCost}/{maintenancePeriod}"`.
- `initialCalloutText : string`: Initial text of the info callout around the bar.
- `gameCtx : GameContext`: Game context, passed to the info callout.
- `showOnRightSide : boolean`: Shows the info callout on the right side.
- `nextUpFocusRef : ReactRefWrap`: Widget reference that gets focus when navigating up (`IA_NAV_UP`).

### EntityWindowUtil.CombinedStockGroupedCountsParam (record)

```teal
record CombinedStockGroupedCountsParam
```

Parameters of `EntityWindowUtil.CombinedStockGroupedCounts`.

- `initialCalloutTextInputPermanent : string`: Callout text of the always visible part of the "Input Stocks" card.
- `initialCalloutTextInputCollapsible : string`: Callout text of the collapsible part of the "Input Stocks" card.
- `recipeAndParamInputCollapsible : RecipeAndParamErased`: Content of the collapsible part of the "Input Stocks" card.
- `initialCalloutTextOutputPermanent : string`: Callout text of the always visible part of the "Output Stocks" card.
- `initialCalloutTextOutputCollapsible : string`: Callout text of the collapsible part of the "Output Stocks" card.
- `recipeAndParamOutputCollapsible : RecipeAndParamErased`: Content of the collapsible part of the "Output Stocks" card.
- `cardKey : string`: Key prefix of the two cards; `"-Input"` / `"-Output"` is appended.
- `setCollapsibleExpanded : function(string, boolean)`: Called with the card key and the new expanded state when a collapsible part is toggled.
- `getCollapsibleExpanded : function(string)`: Returns the stored expanded state for a card key.
- `gameCtx : GameContext`: Game context, passed to the content cards.
- `showOnRightSide : boolean`: Shows the info callouts on the right side.
- `showSimplifiedOnly : boolean`: Shows only the expanded stock table per card, without the compact view and the collapsible part.
- `entityId : Engine.Entity`: Entity with a stock list, e.g. an industry.

### EntityWindowUtil.IconQualityType (enum)

```teal
enum IconQualityType
```

- `"Happy"`
- `"Aloof"`
- `"Sad"`
- `"Positive"`
- `"Negative"`

### EntityWindowUtil.RatingBarParam (record)

```teal
record RatingBarParam
```

Parameters of `EntityWindowUtil.RatingBar`.

- `icon : TreeNodeId`: Ready-made icon widget shown in front of the bar (takes precedence over `iconPath`).
- `iconPath : string`: Image path of the icon, used when `icon` is not set.
- `tooltip : string`: Tooltip of the icon built from `iconPath`.
- `ratingText : string`: Text shown after the bar.
- `getRatingLabel : function(RatingLevel) : string`: Builds the text from the rating level (`town_util.getFactorRatingLevel(value)`) when `ratingText` is not set.
- `barTooltip : string`: Tooltip of the bar.
- `value : number`: Rating from 0 to 1.
- `valueChange : number`: Trend; draws an arrow (one to three steps depending on size, thresholds 0.01 and 0.1) pointing right for positive and left for negative values.
- `numFractions : integer`: Number of segments of the bar; must be larger than 0.
- `snapValueToFractions : boolean`: Rounds the displayed value to the segments.
- `onClick : function()`: Makes the bar a button calling this function.
- `hideText : boolean`: Hides the rating text.
- `showIcon : boolean`: Shows the icon; must be `true` for the icon to appear.

### EntityWindowUtil.NoiseAndPollutionParams (record)

```teal
record NoiseAndPollutionParams
```

Parameters of `EntityWindowUtil.NoiseAndPollution`.

- `noise : integer`: Noise value shown as "Current Noise".
- `pollution : integer`: Pollution value shown as "Current Pollution".

### EntityWindowUtil.ProgressDisplayParam (record)

```teal
record ProgressDisplayParam
```

Parameters of `EntityWindowUtil.ProgressDisplay`.

- `path : string`: Image path of an icon shown in front of the bar.
- `text : string`: optional
- `tooltip : string`: Tooltip of the icon and the bar.
- `value : number`: Fill of the bar from 0 to 1.
- `extraValue : number`: optional
- `trend : number`: optional
- `extraChild : TreeNodeId`: optional
