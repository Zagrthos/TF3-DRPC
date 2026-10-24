# gui.line_vehicle_mgmt.line_react_util

Definition file: `base/tealdef/gui/line_vehicle_mgmt/line_react_util.d.tl` (in the game folder).

Small React widgets and helpers used by the Line & Vehicle Manager and the entity windows: line colour dots and colour picker, name labels with rename and locate buttons, line balance, transported items, cargo display and vehicle line/depot indicators.

## LineReactUtil (record)

```teal
global record LineReactUtil
```
Load: `local line_react_util = ug_require "::/gui/line_vehicle_mgmt/line_react_util.tl" as LineReactUtil`

React widgets for lines and vehicles. Load with `ug_require "/gui/line_vehicle_mgmt/line_react_util.tl"`.

- `ManagerNotificationWidget : Recipe<Engine.Entity>`: Icons of the entity's persisting "Problem" and "Caution" notifications, as shown in the Line & Vehicle Manager.
- `VehicleInstructionsIcon : Recipe<Engine.Entity>`: For a vehicle, the colour dot of its line (en route or at terminal) or the icon of its depot.
  In game: "Assigned Line" (German: „Zugewiesene Linie“)
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistic_vehicles.tl:50`: `line_react_util.VehicleInstructionsIcon(entity),`
- `VehicleInstructionsName : Recipe2<Engine.Entity, boolean>`: For a vehicle, its line name (en route or at terminal) or "Going to Depot" / "In Depot".
  In game: "Going to Depot", "In Depot" (German: „Unterwegs ins Depot“, „Im Depot“)
    - notes: Parameters: vehicle entity, and whether a locate button is shown next to the line name.
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistic_vehicles.tl:51`: `line_react_util.VehicleInstructionsName(entity, false),`
- `getSlimDepotIconPath : function(depotEntity : Engine.Entity) : string`  (call as `line_react_util.getSlimDepotIconPath(…)`): Returns the slim depot icon matching the depot's transport modes (bus/truck, tram, train, ...), or a generic depot icon.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:931`: `path = line_react_util.getSlimDepotIconPath(vehicleLineState:old().lineOrDepotEntity),`
- `makeTooltip : function(text : string) : TreeNodeId`  (call as `line_react_util.makeTooltip(…)`): Returns a tooltip node with the text in the manager tooltip style (inline input hints supported).
- `makeTerminalIndicator : function(terminalNumber : integer, slim : boolean, labelOverride? : string) : TreeNodeId`  (call as `line_react_util.makeTerminalIndicator(…)`): Returns a text node with a terminal number in the terminal indicator style.
    - param `terminalNumber`: Terminal number shown.
    - param `slim`: Use the slim style.
    - param `labelOverride`: Text shown instead of the number.
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:244`: `children[#children + 1] = line_react_util.makeTerminalIndicator(param.displayNumber, true)`
- `makeTerminalStopIndicator : function(terminalNumber : integer, stopName : string) : TreeNodeId`  (call as `line_react_util.makeTerminalStopIndicator(…)`): Returns a text node "<stop name> - <terminal number>" in the terminal stop indicator style.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_hud_util.tl:549`: `table.insert(vboxItems, line_react_util.makeTerminalStopIndicator(`
- `LocateButton : Recipe<LocateButtonParam>`: Button that moves the camera to an entity, position or edge position.
  In game: "Locate" (German: „Anpeilen“)
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:206`: `line_react_util.LocateButton{`
- `makeEntityWindowButton : function(text : string, entity : Engine.Entity, tabIndex? : integer, content? : TreeNodeId, stack? : boolean, onClickOverride? : function(Engine.Entity), tooltip? : string, styleClass? : string) : TreeNodeId`  (call as `line_react_util.makeEntityWindowButton(…)`): Returns a button (with the text or the given content) that opens the entity window of an entity via the `selectEntity` event.
    - param `text`: Button text, used when `content` is nil.
    - param `entity`: Entity to open.
    - param `tabIndex`: Not used.
    - param `content`: Node shown instead of the text.
    - param `stack`: Whether the window opens stacked (default true).
    - param `onClickOverride`: Called with the entity instead of opening the window.
    - param `tooltip`: Button tooltip.
    - param `styleClass`: Style class of the text (default `font-scale-body`).
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:209`: `line_react_util.makeEntityWindowButton(nameState:old(), otherEntity)`
- `ColorWidget : Recipe<ColorWidgetParam>`: Colour dot of a line (or other entity with a colour).
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1627`: `line_react_util.ColorWidget({`
- `NameTextView : Recipe<NameTextViewParams>`: Name of an entity as a button that opens its entity window, with optional locate and rename buttons.
  In game: "Rename" (German: „Umbenennen“)
    - used 19x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1167`: `table.insert(children, line_react_util.NameTextView{`
- `LineOrDepotButton : Recipe3<Engine.Entity, function(Engine.Entity, boolean), string>`: For a vehicle, a button showing its line colour, or a depot icon when it is going to or in a depot.
  In game: "Going to {depotName}" (German: „Unterwegs zu {depotName}“)
    - notes: Parameters: vehicle entity, an optional click handler (called with the line entity and true, or the depot entity and false) and an optional tooltip for the line case.
- `LineColorPicker : Recipe2<Engine.Entity, boolean>`: Colour chooser button that changes a line's colour (palette from `line_colors.gres` plus grayscale) with `api.cmd.makeEntitySetColorCmd`.
  In game: "Change Color" (German: „Farbe ändern“)
    - notes: Parameters: line entity and `forceFocusable`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/line/line_eow.script.tl:538`: `child = line_react_util.LineColorPicker(params.entityId, true)`
- `LineBalance : Recipe<LineBalanceParams>`: Balance of a line over the last game year (`api.engine.util.finance.calculateBalance`), formatted as money and styled positive or negative.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/line/line_eow.script.tl:308`: `line_react_util.LineBalance{`
- `LineTransported : Recipe<LineTransportedParams>`: Items transported by a line per year (logbook value `itemsTransported`).
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/line/line_eow.script.tl:91`: `line_react_util.LineTransported{`
- `LineCargoDisplay : Recipe<LineCargoDisplayParams>`: Passenger icon and up to three cargo icons of a line or of one stop, on the line colour.
  In game: "Configure Stop" (German: „Haltestelle konfigurieren“)
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_manager_panel.tl:915`: `via.stop and line_react_util.LineCargoDisplay{`

### LineReactUtil.NameTextViewParams (record)

```teal
record NameTextViewParams
```

Parameters of `LineReactUtil.NameTextView`.

- `entity : Engine.Entity`: Entity whose name is shown and renamed; clicking opens its entity window.
- `locationButton : boolean`: Shows a locate button before the name.
- `stackEntityOpen : boolean`: Whether the entity window opens stacked (default true).
- `nameEntity : Engine.Entity`: Entity whose name is displayed instead of `entity`'s.
- `onClickOverride : function(Engine.Entity)`: Called with the entity instead of opening its entity window.
- `editMode : boolean`: Shows a rename (pencil) button that turns the name into a text field.
- `tooltip : string`: Tooltip of the name button.
- `styleClass : string`: Style class of the name text (default `font-scale-body`).
- `maxLength : integer`: Maximum name length in the text field (default 50).

### LineReactUtil.LocateButtonParam (record)

```teal
record LocateButtonParam
```

Parameters of `LineReactUtil.LocateButton`; set one of `entity`, `position` or `edgePos`.

- `entity : Engine.Entity`: Entity the camera follows.
- `position : Vec3f`: World position the camera focuses.
- `edgePos : EdgePos`: Position on a transport network edge the camera focuses.
- `tooltip : string`: Tooltip (default "Locate").
- `iconPathOverride : string`: Icon instead of `::/gui/line_vehicle_mgmt/icons/symbol_locate.tga`.

### LineReactUtil.ColorWidgetParam (record)

```teal
record ColorWidgetParam
```

Parameters of `LineReactUtil.ColorWidget`.

- `entity : Engine.Entity`: Entity with a `COLOR` component; its colour is shown and kept up to date when `color` is nil.
- `color : Vec3f`: Fixed colour to show instead of the entity's.
- `tooltip : string`: Tooltip of the dot.
- `onClick : function()`: If set, the dot becomes a button calling it.

### LineReactUtil.LineBalanceParams (record)

```teal
record LineBalanceParams
```

Parameters of `LineReactUtil.LineBalance`.

- `entity : Engine.Entity`: The line entity.
- `stepTimer : boolean`: Update on the step timer instead of every step.

### LineReactUtil.LineTransportedParams (record)

```teal
record LineTransportedParams
```

Parameters of `LineReactUtil.LineTransported`.

- `entity : Engine.Entity`: The line entity.
- `stepTimer : boolean`: Update on the step timer instead of every step.

### LineReactUtil.LineCargoDisplayParams (record)

```teal
record LineCargoDisplayParams
```

Parameters of `LineReactUtil.LineCargoDisplay`.

- `lineEntity : Engine.Entity`: The line entity.
- `stopIndex : integer`: 0-based stop index, used when `stopCargoDisplay` is true.
- `neutralBackground : boolean`: Use a neutral background instead of the line colour.
- `stopCargoDisplay : boolean`: otherwise whole line
- `configureStopButton : boolean`: Makes the cargo icons a toggle button ("Configure Stop").
- `onValueChange : function(val : boolean)`: Called when the configure toggle changes.
- `toggled : boolean`: State of the configure toggle.
