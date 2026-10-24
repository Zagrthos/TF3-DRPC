# gui.line_vehicle_mgmt.line_util

Definition file: `base/tealdef/gui/line_vehicle_mgmt/line_util.d.tl` (in the game folder).

Line utilities of the Line & Vehicle Manager (LVM): converting between the engine `LINE` component and the editable `Line.ReactLine`, stop/waypoint index conversion, automatic terminal and cargo-load assignment, the selection/hover context of the LVM tool, and the shared `CommonActionParams` interface that the LVM panels pass around.

## Globals

- `VehicleStoreOpenInfo` (type): defined in manager_window.d.tl

Depot and line the vehicle store window was opened for from the Line & Vehicle Manager.
- `VehicleManagerParams` (type): defined in manager_window.d.tl

Parameters of the vehicle manager (vehicle list) of the Line & Vehicle Manager.
- `VehicleManagerApi` (type): defined in manager_window.d.tl

API of the vehicle manager, reached through `CommonActionParams.vehicleManagerRef`.
- `VehicleManagerState` (type): defined in manager_window.d.tl

Contents of the vehicle list; kept between openings of the manager.
- `LineManagerState` (type): defined in manager_window.d.tl

Selection of the line and depot list.
- `LineAndDepotListParams` (type): define in manager_window.d.tl

Parameters of the line and depot list of the Line & Vehicle Manager.
- `LineAndDepotListApi` (type): defined in manager_window.d.tl

API of the line and depot list, reached through `CommonActionParams.lineAndDepotListRef`.

## LineUtil (record)

```teal
global record LineUtil
```
Load: `local line_util = ug_require "::/gui/line_vehicle_mgmt/line_util.tl" as LineUtil`

Line utilities of the Line & Vehicle Manager. Load with `ug_require "/gui/line_vehicle_mgmt/line_util.tl"`.

- `autoAssignTerminals : function(entity : Engine.Entity, path : {Line.ReactVia}) : {Engine.Component.Line.Stop}`  (call as `line_util.autoAssignTerminals(…)`): Converts a line path into engine `Line.Stop` entries and fills in missing station/terminal indices with the engine's best line assignment.
    - param `entity`: Existing line entity, or nil for a new line.
    - param `path`: Path of a `Line.ReactLine`.
    - returns: The stops for `Engine.Component.Line.stops`, with waypoints attached to the preceding stop.
    - notes: Waypoints before the first stop are appended to the last stop. Stops without a station or terminal are assigned with `api.engine.system.lineSystem.getBestLineAssignment`; if that finds nothing, station 0 / terminal 0 is used.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:6527`: `newLineComp.stops = line_util.autoAssignTerminals(lineEntity, lineStateNew.path)`
- `makeLineColor : function(Engine.Entity) : Vec3f`  (call as `line_util.makeLineColor(…)`): Picks a colour for a new line from `::/gui/line_vehicle_mgmt/line_colors.gres`, preferring colours the player's lines use least.
    - param `#1`: Player entity.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:6535`: `lineStateNew.color = line_util.makeLineColor(playerEntity)`
- `makeLineName : function(Engine.Entity) : string`  (call as `line_util.makeLineName(…)`): Returns the first unused default line name ("Line 1", "Line 2", ...) among the player's lines.
  In game: "Line {lineNumber}", "New Line" (German: „Linie {lineNumber}“, „Neue Linie“)
    - param `#1`: Player entity.
    - returns: Translated name; "New Line" if the numbers up to 999 are all used.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:6536`: `local name = line_util.makeLineName(playerEntity)`
- `getReactLineFromGameState : function(Engine.Entity) : Line.ReactLine`  (call as `line_util.getReactLineFromGameState(…)`): Reads the `LINE` component of a line entity and returns it as an editable `Line.ReactLine`.
    - param `#1`: Line entity; nil returns nil.
    - used 6x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/auto_rename/scheme_components/scheme_component_firststop.script.tl:6`: `local reactLine =  line_util.getReactLineFromGameState(renameParams.lineEntity)`
- `computeStopAndWaypointIndex0 : function(line : Engine.Entity, stopNumber : integer) : {integer, integer}`  (call as `line_util.computeStopAndWaypointIndex0(…)`): Converts a stop number (1-based index in the flat stop-and-waypoint path) into engine indices.
    - param `line`: Line entity.
    - param `stopNumber`: 1-based index in the flat path.
    - returns: `{stopIndex0, waypointIndex}`; the waypoint index is -1 (stop without waypoints), 0 (stop with waypoints) or the 1-based waypoint number. Nil if out of range.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_manager_panel.tl:1019`: `local apiStopIndex0 = line_util.computeStopAndWaypointIndex0(params.lineEntity, params.stopNumber)[1]`
- `computeWaypointIndex0FromTag : function(line : Engine.Entity, stopIndex0 : integer, waypointTag : integer) : integer`  (call as `line_util.computeWaypointIndex0FromTag(…)`): Finds the 0-based index of a waypoint (by its tag) within a stop of the line.
    - returns: The index, -1 if the tag is nil or negative, or nil (with an error log) if not found.
- `computeStopNumber : function(line : Engine.Entity, stopIndex0 : integer, apiWaypointIndex : integer) : integer`  (call as `line_util.computeStopNumber(…)`): Converts engine indices (stop index and waypoint index) into the stop number, i.e. the 1-based index in the flat stop-and-waypoint path.
- `convertDetails : function(entity : Engine.Entity, apiDetails : Gui.SelectionDetails) : SelectionDetails`  (call as `line_util.convertDetails(…)`): Converts engine selection details (or a plain entity) into `LineUtil.SelectionDetails`.
    - param `entity`: Picked entity, used when `apiDetails` has no data.
    - param `apiDetails`: Selection details from the selector or HUD.
    - returns: Details with one field set; for plain entities: station group, station, vehicle depot, signal, or `entity`. Nil if the entity does not exist.
- `makeNewStop : function(details : StationSelectionDetails) : Line.ReactVia`  (call as `line_util.makeNewStop(…)`): Creates a new path entry for a stop at the given station, with an empty load configuration and the default waiting time.
    - returns: A `Line.ReactVia` with `stop` set.
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:6702`: `local stop = line_util.makeNewStop(newLineStartingWithStation)`
- `applyInputActionConfig : function(guiCtx : GuiContextState)`  (call as `line_util.applyInputActionConfig(…)`): Calls `react.useInputAction` for every input action configured in the context state (so it can also be applied on the owning window).
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:6251`: `line_util.applyInputActionConfig(guiCtxForInputActions:old())`
- `guiContextEquals : function(old : GuiContextState, newContext : GuiContextState) : boolean`  (call as `line_util.guiContextEquals(…)`): Compares two context states (tooltips, highlights, filters, input actions and their compare proxies); used to avoid needless state updates.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:6370`: `if line_util.guiContextEquals(old, newContext) then`
- `useContextState : function(prepareContextBuilder? : (function() : LineUtil.ContextBuilder), updateIA? : function(ctx : LineUtil.GuiContextState)) : (LineUtil.GuiContextState, function(ctx : LineUtil.GuiContextState))`  (call as `line_util.useContextState(…)`): React hook that keeps a `GuiContextState` and applies its input actions.
    - param `prepareContextBuilder`: Optional; returns the builder whose result is the initial state.
    - param `updateIA`: Optional; called with the current state each render.
    - returns: The current state and a setter that only updates when `LineUtil.guiContextEquals` reports a change.
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:287`: `= line_util.useContextState(prepareContextBuilder, commonParams.updateIA)`
- `MakeContextBuilder : function() : ContextBuilder`  (call as `line_util.MakeContextBuilder(…)`): Returns a new, empty `LineUtil.ContextBuilder`.
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:253`: `local ctx : LineUtil.ContextBuilder = line_util.MakeContextBuilder()`
- `makeLineActionDescriptor : function<CommonParams, Mode>(commonParams : CommonActionParams, gameCtx : GameContext, parent : ReactNodeRef, switchMode : function(newMode : {Mode, any | LineUtil.StopSelectionDetails | nil}), lineVisualizations : {Builtin.Type.LineVisualization}, continuousRenderLine : Engine.Entity, highlightedEntities : {Engine.Entity}, oldContext : GuiContextState, pickedEntity : function(details : LineUtil.SelectionDetails) : LineUtil.GuiContextState, processMosueEvent : function(evt : Gui.Mouse.Event) : boolean, lineSelectable : boolean, showTownDistrictColors : boolean, pushTownIconsToBack : boolean, isGamepadInputMode : boolean, lvm2D3DMode : LVM2D3DMode, horizontalPromptList : boolean, preferLineOverTNEdge : boolean) : TreeNodeId`  (call as `line_util.makeLineActionDescriptor(…)`): Builds the `builtin.ActionDescriptor` (tool "management") of the Line & Vehicle Manager, with layer config, line viewer, selector, cursor tooltip and HUD icon config.
    - returns: The action descriptor node.
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:435`: `return line_util.makeLineActionDescriptor(`
- `getStationHudPath : function(carrier : Carrier, depot : boolean, cargo : boolean) : string`  (call as `line_util.getStationHudPath(…)`): Returns the HUD icon path for a station or depot of the given carrier.
    - param `carrier`: Carrier of the station or depot.
    - param `depot`: True for a depot icon.
    - param `cargo`: True for the cargo variant of a station icon.
    - returns: Icon path under `::/gui/hud/icons/`, or an empty string for an unknown carrier.
- `getLineCarriers : function(line : Line.ReactLine) : {Carrier : boolean}`  (call as `line_util.getLineCarriers(…)`): Returns the carriers served by the first stop of the line that has a station assigned (empty if there is none).
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:468`: `carriers = line_util.getLineCarriers(commonParams.lineState:old())`
- `isWaypointTooClose : function(line : Line.ReactLine, position : Vec3f, ignoreStopNumber : integer) : boolean`  (call as `line_util.isWaypointTooClose(…)`): Checks whether a position is within 8 metres of a waypoint of the line.
    - param `ignoreStopNumber`: Path index to skip (the waypoint being moved).
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:723`: `elseif line_util.isWaypointTooClose(commonParams.lineState:old(), apiWaypoint.pos, nil) then`
- `calculateFrequencySeconds : function(entityId : Engine.Entity) : integer`  (call as `line_util.calculateFrequencySeconds(…)`): Returns the line interval in seconds (1 / `api.engine.util.line.getMaxFrequency`), or 0 if the frequency is zero.
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/line/line_eow.script.tl:58`: `return line_util.calculateFrequencySeconds(entityId)`
- `getVehicleInstructionName : function(vehicleEntity : Engine.Entity) : string`  (call as `line_util.getVehicleInstructionName(…)`): Returns the name of the vehicle's line if it is en route or at a terminal, otherwise "Going to Depot" or "In Depot".
  In game: "Going to Depot", "In Depot" (German: „Unterwegs ins Depot“, „Im Depot“)
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistic_vehicles.tl:448`: `local vehicleInstructionName = line_util.getVehicleInstructionName(key)`
- `autoLoadConfig : function(line : Line.ReactLine)`  (call as `line_util.autoLoadConfig(…)`): Sets the cargo loading of all stops automatically from the stations' supply and demand. Does nothing if the line has `customFilters`.
    - notes: Also assigns terminals first if a stop has none. Passengers are loaded at passenger stations; cargo is loaded where it is produced if another stop of the line needs it.
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:6712`: `line_util.autoLoadConfig(lineState2)`
- `isTerminalCargoTypeAllowed : function(stop : Line.ReactTerminal, cargoTypeId : CargoTypeId) : boolean`  (call as `line_util.isTerminalCargoTypeAllowed(…)`): Returns false only for passengers at a terminal that neither loads nor unloads passengers.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/cargofilter_window.tl:372`: `data[k].allowed = line_util.isTerminalCargoTypeAllowed(reactStop, cargoTypeId)`
- `getTerminalAlternative : function(details : LineUtil.StationSelectionDetails, lineState : Line.ReactLine, stopNumber : integer) : LineUtil.TerminalAlternative`  (call as `line_util.getTerminalAlternative(…)`): Checks whether a terminal can be (or already is) an alternative terminal of the given stop.
    - param `details`: The terminal under the cursor.
    - param `lineState`: The edited line.
    - param `stopNumber`: 1-based index of the stop in the line path.
- `isLineCompatibleWithTransportMode : function(lineEntity : Engine.Entity, tm : TransportMode) : boolean`  (call as `line_util.isLineCompatibleWithTransportMode(…)`): Returns true if the edges at every assigned terminal of the line allow the transport mode.
    - used 3x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:165`: `showLine = (line_util.isLineCompatibleWithTransportMode(lineEntity, tmSmAircraft)`
- `getLineHasVehicles : function(lineEntity : Engine.Entity) : LineUtil.LineHasVehicles`  (call as `line_util.getLineHasVehicles(…)`): Returns which kinds of vehicles are assigned to the line.
- `getLineAndVehicleCompatibleTransportModesAndCarrier : function(vehicleEntities : {Engine.Entity}) : VehicleStoreUtil.TransportModesAndCarrier`  (call as `line_util.getLineAndVehicleCompatibleTransportModesAndCarrier(…)`): Returns the carrier of the vehicles and the transport modes a replacement vehicle may have (used by the vehicle store).
    - returns: Empty transport modes and nil carrier when the vehicles have different carriers.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:450`: `local transportModesAndCarrier = line_util.getLineAndVehicleCompatibleTransportModesAndCarrier({vehicleEntityId})`
- `isStopProtected : function(lineEntity : Engine.Entity, stationGroupEntity : Engine.Entity, protectedEntities : {Engine.Entity : GameContext.Filters.ProtectionConfig | boolean}) : boolean`  (call as `line_util.isStopProtected(…)`): Checks whether a line (or one of its stops) is protected against changes by `GameContext.Filters.protectedEntities` (e.g. in missions).
    - param `stationGroupEntity`: Station group to check; nil checks the line as a whole.
    - used 6x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_manager_panel.tl:656`: `return line_util.isStopProtected(params.lineEntity, stationGroup, params.commonParams.gameCtx.filters:get().protectedEntities)`
- `getBestDepotForLine : function(lineEntity : Engine.Entity) : Engine.Entity`  (call as `line_util.getBestDepotForLine(…)`): Finds a depot suited to the line's carriers and vehicles with `api.engine.util.vehicle.findBestDepotForLine`; the result is negative (or nil) if none is found.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:7400`: `local depotForLine = line_util.getBestDepotForLine(singleLine)`
- `getTerminal : function(stop : Line.ReactTerminal) : Terminal`  (call as `line_util.getTerminal(…)`): Returns the `Terminal` of the stop's station and terminal, or nil if none is assigned.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/cargofilter_window.tl:344`: `local terminal = line_util.getTerminal(reactStop)`
- `getTerminalLength : function(stationEntity : Engine.Entity, terminalIndex1 : integer) : number`  (call as `line_util.getTerminalLength(…)`): Returns the largest cargo transfer length of the terminal (1-based index) of the station.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:739`: `local terminalLength = line_util.getTerminalLength(stationEntity, terminalIndex1)`
- `getRelaxationType : function(okModes : {TransportMode : boolean}, relaxedModes : {TransportMode : boolean}, allowedModes : {TransportMode : boolean}) : LineUtil.RelaxationType`  (call as `line_util.getRelaxationType(…)`): Compares the original and relaxed transport modes and returns what infrastructure is missing, or nil.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/line_warning.script.tl:90`: `local relaxationType = line_util.getRelaxationType(param.okModes, param.relaxedModes, param.allowedModes)`
- `getRelaxationText : function(relaxationType : RelaxationType) : string`  (call as `line_util.getRelaxationText(…)`): Returns the translated text for a relaxation type ("Missing catenaries", ...), or nil.
  In game: "Missing road", "Missing train tracks", "Missing catenaries" (German: „Strasse fehlt“, „Bahngleise fehlen“, „Oberleitungen fehlen“)
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/line_warning.script.tl:91`: `local relaxationText = line_util.getRelaxationText(relaxationType)`
- `filterLine : function(allowedCarriers : {Carrier}, lineEntity : Engine.Entity) : boolean`  (call as `line_util.filterLine(…)`): Returns true if the line matches the allowed carriers, based on its transport modes and the vehicles assigned to it (used by the LVM carrier filter).
    - used 3x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:241`: `if line_util.filterLine(allowedCarriers, lineEntity) then`

### LineUtil.LineSelectionDetails (record)

```teal
record LineSelectionDetails
```

A picked or highlighted line, optionally at a specific stop or waypoint.

- `entity : Engine.Entity`: The line entity.
- `stopNumber : integer`: 1-based index of the Via
- `apiStopIndex0 : integer`: 0-based stop index in the engine `LINE` component.
- `apiWaypointIndex : integer`: Waypoint index after that stop as the engine reports it (-1 or nil for the stop itself).

### LineUtil.StationSelectionDetails (record)

```teal
record StationSelectionDetails
```

A picked station group, optionally narrowed down to one station and terminal.

- `stationGroup : Engine.Entity`: The station group entity.
- `stationIndex1 : integer`: 1-based index in stationGroup
- `station : Engine.Entity`: The station entity that contains the picked terminal, or nil.
- `terminalIndex1 : integer`: 1-based index in station
- `flatStationTerminalIndex1 : integer`: 1-based index in all station/terminal pairs of the station group.

### LineUtil.SignalSelectionDetails (record)

```teal
record SignalSelectionDetails
```

A picked signal (waypoint signal list entry).

- `signal : Engine.Entity`: Entity with the `SIGNAL_LIST` component.
- `signalIndex1 : integer`: 1-based index in signalList

### LineUtil.StopSelectionDetails (record)

```teal
record StopSelectionDetails
```

A picked stop of a line, together with the station it is at.

- `station : StationSelectionDetails`: Station (group, station, terminal) of the stop.
- `line : Engine.Entity`: The line entity.
- `stopNumber : integer`: 1-based index of the Via
- `apiStopIndex0 : integer`: 0-based stop index in the engine `LINE` component.

### LineUtil.WaypointSelectionDetails (record)

```teal
record WaypointSelectionDetails
```

A picked waypoint of a line.

- `line : Engine.Entity`: The line entity.
- `stopNumber : integer`: 1-based index of the Via
- `apiStopIndex0 : integer`: 0-based index of the stop the waypoint follows in the engine `LINE` component.
- `tag : integer`: Tag of the waypoint (`Waypoint.tag`), which identifies it within the line.

### LineUtil.TransportNetworkEdgeSelectionDetails (record)

```teal
record TransportNetworkEdgeSelectionDetails
```

A picked position on a transport network edge (used to place waypoints).

- `snap : EdgePos`: Edge position the cursor snapped to.

### LineUtil.VehicleDepotDetails (record)

```teal
record VehicleDepotDetails
```

A vehicle or depot entity plus whether it should be removed from (instead of added to) the vehicle list or selection.

- `entity : Engine.Entity`: Vehicle or depot entity.
- `removeFromList : boolean`: True to remove instead of add.

### LineUtil.SelectionDetails (record)

```teal
record SelectionDetails
```

Result of `LineUtil.convertDetails`; exactly one field is set, depending on what was picked.

- `entity : Engine.Entity`: use if it is only an entity, without additional detail infos
- `line : LineSelectionDetails`: Set when a line was picked.
- `station : StationSelectionDetails`: Set when a station group, station or terminal was picked.
- `signal : SignalSelectionDetails`: Set when a signal was picked.
- `stop : StopSelectionDetails`: Set when a stop of a line was picked.
- `waypoint : WaypointSelectionDetails`: Set when a waypoint of a line was picked.
- `transportNetworkEdge : TransportNetworkEdgeSelectionDetails`: Set when a position on a transport network edge was picked.
- `world : Gui.SelectionDetails.World`: Set when the terrain was picked; the engine's `Gui.SelectionDetails.World` data.
- `vehicleDepot : VehicleDepotDetails`: Set when a vehicle depot (with in or out nodes) was picked.
- `townData : Gui.SelectionDetails.Town`: Set when a town was picked; the engine's `Gui.SelectionDetails.Town` data.

### LineUtil.TerminalAlternative (record)

```teal
record TerminalAlternative
```

Result of `LineUtil.getTerminalAlternative`.

- `alternativePossible : boolean`: True if the terminal belongs to the stop's station group but is not its main terminal.
- `alternativeSelected : boolean`: True if the terminal is already one of the stop's alternative terminals.
- `color : Vec4f`: Line colour (alpha 1) when an alternative is possible, otherwise nil.

### LineUtil.LineHasVehicles (record)

```teal
record LineHasVehicles
```

Which kinds of vehicles run on a line; result of `LineUtil.getLineHasVehicles`.

- `hasNoVehicles : boolean`: True if no vehicle is assigned to the line.
- `hasRoadVehicles : boolean`: At least one vehicle with carrier `ROAD`.
- `hasRailVehicles : boolean`: At least one vehicle with carrier `RAIL`.
- `hasTramVehicles : boolean`: At least one vehicle with carrier `TRAM`.
- `hasAirVehicles : boolean`: At least one vehicle with carrier `AIR`.
- `hasWaterVehicles : boolean`: At least one vehicle with carrier `WATER`.
- `hasPlanes : boolean`: At least one air vehicle with transport mode `AIRCRAFT`.
- `hasSmallPlanes : boolean`: At least one air vehicle with transport mode `SMALL_AIRCRAFT`.
- `hasHelicopters : boolean`: At least one air vehicle with transport mode `HELICOPTER`.
- `hasShips : boolean`: At least one water vehicle with transport mode `SHIP`.
- `hasSmallShips : boolean`: At least one water vehicle with transport mode `SMALL_SHIP`.
- `hasLightrailRail : boolean`: At least one rail vehicle that can also use tram transport modes (light rail).
- `hasLightrailTram : boolean`: At least one tram vehicle that can use tram track transport modes (light rail).

### LineUtil.RecipeAndParamErased (record)

```teal
record RecipeAndParamErased
```

A tooltip recipe together with its parameter, with the parameter type erased.

- `recipe : Recipe<any>`: Recipe that renders the tooltip.
- `param : any`: Parameter passed to the recipe.

### LineUtil.GuiContextState (record)

```teal
record GuiContextState
```

What the Line & Vehicle Manager tool shows and accepts for the current hover state (input actions, tooltip, highlights, selection filter). Built with `LineUtil.MakeContextBuilder` and consumed by `LineUtil.makeLineActionDescriptor`.

the set of input actions depend on the internal state (e.g. on what entity is hovered)
IAs are always a bit delayed, as they might go to the element hovered in the previous frame, while mouse clicks don't need the extra roundtrip to update hover
- `inputActions : {string}`
- `iaForwards : {string : React.IaForward}`: Input actions forwarded to another node, by input action key (see `ContextBuilder.iaForward`).
- `iaHandlers : {string : {React.IaHandler, any, any}}`: Input action handlers by key, each as `{handler, handlerCompareProxy, isEnabledCompareProxy}`; the proxies are used by `LineUtil.guiContextEquals`.

there is no IA for mouse clicks yet, so call this handler instead
- `handleClick : function()`
- `tooltip : RecipeAndParamErased`: Tooltip shown next to the cursor (`builtin.ActionTooltip`), or nil.
- `mouseCursor : integer`: Mouse cursor id passed to the action descriptor (0 by default).
- `terrainCirclePolicy : Builtin.TerrainCirclePolicy`: Terrain circle policy of the action descriptor; when nil, "Never" in 2D mode and a mouse/gamepad-dependent default in 3D mode.
- `highlightLine : LineUtil.LineSelectionDetails`: Line (and stop or waypoint) highlighted in the line viewer.
- `highlightStop : Gui.SelectionDetails.Stop`: Stop highlighted in the line viewer.
- `highlightTerminal : {Engine.Entity, StationTerminal}`: Station group and 0-based station terminal highlighted in the line viewer.
- `highlightWaypoint : {Engine.Entity, integer, integer}`: Waypoint highlighted in the line viewer, as `{line, apiStopIndex0, tag}`.
- `snapPreview : Gui.SelectionDetails.TransportNetworkEdge`: Edge position shown as snap preview in the line viewer (where a waypoint would go).
- `player : boolean`: When true, only entities owned by the player (or not owned at all) can be selected.
- `disableCreateNewLine : boolean`: When true, the line viewer does not show station terminals.
- `showNavigableLayer : boolean`: When true, the terrain layer colours water and navigable water (used for ship waypoints).
- `showTerminalsForLine : Engine.Entity`: Line whose terminals the line viewer shows.
- `componentsFilter : {Engine.ComponentType}`: Component types an entity must have (at least one) to be selectable; nil allows all.
- `unselectableComponents : {Engine.ComponentType}`: Component types that make an entity unselectable (only checked when `componentsFilter` is set).
- `hudMaster : Recipe2<Builtin.HudIconParam, CommonActionParams>`: HUD icon master recipe used while the tool is active, or nil for no HUD icon config.
- `carriers : {Carrier}`: Carriers whose lines and vehicles are shown and coloured; nil shows all.

### LineUtil.ContextBuilder (record)

```teal
record ContextBuilder
```

Builder for a `LineUtil.GuiContextState`, created with `LineUtil.MakeContextBuilder`. Call its functions, then read `result`.

- `iaHandler : function(iaKey : string, handler : function(), handler_compareProxy : any, isEnabled : (function() : boolean), isEnabled_compareProxy : any, promptTextOverride? : string)`: Registers a handler for an input action.
    - param `iaKey`: Input action key, e.g. `"IA_SELECT"`.
    - param `handler`: Called when the input action fires.
    - param `handler_compareProxy`: Value compared by `LineUtil.guiContextEquals` instead of the function itself.
    - param `isEnabled`: Returns whether the action is currently enabled.
    - param `isEnabled_compareProxy`: Value compared instead of the `isEnabled` function.
    - param `promptTextOverride`: Optional prompt text shown for the action.
- `iaHandlerExtended : function(iaKey : string, handler : function(Gui.InputAction.InvokeData), handler_compareProxy : any, isEnabled : (function() : boolean), isEnabled_compareProxy : any, promptTextOverride? : string)`: Like `iaHandler`, but the handler receives the `Gui.InputAction.InvokeData` of the action.
- `iaForward : function(iaKey : string, target : ReactRefWrap, asIaKey : string)`: Forwards an input action to another node (`react.iaForward`), optionally under another key.
    - param `iaKey`: Input action key to forward.
    - param `target`: Node the action is forwarded to.
    - param `asIaKey`: Key under which the target receives it.
- `iaClick : function(handleClick : function())`: note: currently, this is also triggered with IA_SELECT
- `iaClickOrSelect : function(handleClickOrSelect : function(), handleClickOrSelect_compareProxy : any, isEnabled : (function() : boolean), isEnabled_compareProxy : any)`: Sets the mouse-click handler and registers the same function for `IA_SELECT`.
- `tooltip : function<T>(recipe : Recipe<T>, param : T)`: Sets the tooltip recipe and its parameter.
- `setShowNavigableLayer : function(showNavigableLayer : boolean)`: Sets `showNavigableLayer`.
- `highlightLine : function(lineEntity : Engine.Entity, stopNumber : integer)`: Highlights a line at the given stop number (1-based index in the line path); pass a nil line to clear.
- `highlightTerminal : function(station : LineUtil.StationSelectionDetails)`: Highlights the station terminal; clears the highlight if the details contain no station and terminal index.
- `highlightStop : function(stop : LineUtil.StopSelectionDetails)`: Highlights a stop of a line in the line viewer.
- `highlightSignal : function(signal : LineUtil.SignalSelectionDetails)`: Highlights a signal (stored in `highlightTerminal`); clears it when no signal index is given.
- `highlightWaypoint : function(waypoint : LineUtil.WaypointSelectionDetails)`: Highlights a waypoint of a line.
- `showSnapPreview : function(snap : LineUtil.TransportNetworkEdgeSelectionDetails)`: Shows a snap preview at the given edge position.
- `setPlayer : function(player : boolean)`: Sets `player` (restrict selection to the player's entities).
- `showTerminalsForLine : function(lineEntity : Engine.Entity)`: Sets the line whose terminals are shown.
- `setComponentsFilter : function(passComponents : {Engine.ComponentType})`: Sets `componentsFilter`.
- `setUnselectableComponents : function(failComponents : {Engine.ComponentType})`: Sets `unselectableComponents`.
- `setHudMaster : function<T is CommonActionParams>(masterRecipe : Recipe2<Builtin.HudIconParam, T>)`: Sets the HUD icon master recipe.
- `setMouseCursor : function(mouseCursor : integer)`: Sets the mouse cursor id.
- `setTerrainCirclePolicy : function(terrainCirclePolicy : Builtin.TerrainCirclePolicy)`: Sets the terrain circle policy.
- `result : GuiContextState`: The context state being built.

### LineUtil.DepotAndLineParams (record)

```teal
record DepotAndLineParams
```

A depot and a line, e.g. to buy vehicles for the line at that depot.

- `depot : Engine.Entity`: Depot entity.
- `line : Engine.Entity`: Line entity.

### LineUtil.RelaxationType (enum)

```teal
enum RelaxationType
```

Which infrastructure is missing when a line's transport modes had to be relaxed; result of `LineUtil.getRelaxationType`.

- `"Catenary"`: Electric modes were lost (missing catenaries).
  In game: "Missing catenaries" (German: „Oberleitungen fehlen“)
- `"Road"`: Road modes were lost (missing road).
  In game: "Missing road" (German: „Strasse fehlt“)
- `"Train"`: Train modes were lost (missing train tracks).
  In game: "Missing train tracks" (German: „Bahngleise fehlen“)
- `"TramLanes"`: Tram modes were lost (missing tram lane).
  In game: "Missing tram lane" (German: „Tramspur fehlt“)
- `"TramTrack"`: Tram track modes were lost (missing tram tracks).
  In game: "Missing tram tracks" (German: „Tramgleise fehlen“)

## LVMMode (enum)

```teal
global enum LVMMode
```

Mode of the Line & Vehicle Manager tool, stored as `{mode, param}`.

- `"MAIN"`: no line selected, not sending to line; the default state
- `"SEND_TO_LINE"`: selected vehicles to send to line or depot
- `"SEGMENT"`: segment selected (can add stops here)
- `"MOVE_STOP"`: grabbed a stop to be moved
- `"MOVE_WAYPOINT"`: grabbed a waypoint to be moved

## LVM2D3DMode (enum)

```teal
global enum LVM2D3DMode
```

Whether the Line & Vehicle Manager is operated in the panels (2D) or in the 3D world view; relevant for gamepad input.

- `"2D"`: Focus in the manager panels.
- `"3D"`: Focus in the 3D world view (crosshair selection).

## LVMSection (enum)

```teal
global enum LVMSection
```

Section of the Line & Vehicle Manager that has the gamepad focus.

- `"LM"`: line manager section
- `"VM"`: vehicle manager section
- `"LP"`: line panel section

## LVMActionButtons (enum)

```teal
global enum LVMActionButtons
```

Action button bar of the Line & Vehicle Manager that has the focus.

- `"LMAB"`: line manager action buttons
- `"VMAB"`: vehicle manager action buttons
- `"LPLAB"`: line panel line action buttons
- `"LPDAB"`: line panel depot action buttons
- `"LPPAB"`: line panel placeholder action buttons
- `"NONE"`: focus not in any action buttons

## LVMCountData (record)

```teal
global record LVMCountData
```

Counts and selection ratios shown by the Line & Vehicle Manager (respecting the carrier and visibility filters).

considering all player lines
- `countLinesWithProblems : integer`
- `countLinesSelected : integer`: Number of selected lines.
- `countLinesUnselected : integer`: Number of unselected lines.
- `selectionRatioLines : number`: Selected lines divided by all counted lines (0 if there are none).

considering all player depots
- `countDepotsSelected : integer`
- `countDepotsUnselected : integer`: Number of unselected depots.
- `selectionRatioDepots : number`: Selected depots divided by all counted depots.

considering vehicles of selected lines
- `countVehiclesWithProblems : integer`
- `countVehiclesSelected : integer`: Number of selected vehicles in the vehicle list.
- `countVehiclesUnselected : integer`: Number of unselected vehicles in the vehicle list.
- `selectionRatioVehicles : number`: Selected vehicles divided by all vehicles in the list (0 if there are none).

## ProblemCache (record)

```teal
global record ProblemCache
```

Problem information the Line & Vehicle Manager refreshes a few lines per frame, to mark lines and vehicles with problems.

- `vehiclesAndProblems : {{{Engine.Entity}, VehicleProblem}}`: Result of `api.engine.util.vehicle.getVehicleProblems()`.
- `vehiclesWithRoundTrips : {Engine.Entity : integer}`: Vehicles with cargo round trips, from `api.engine.system.simEntityAtVehicleSystem.getVehiclesWithRoundTripCount()`.
- `line2misconfiguredLines : {Engine.Entity : {LineIssue}}`: Line issues of the player's lines, from `api.engine.util.line.getLinesIssues`.
- `line2detailedLineProblems : {Engine.Entity : {{UtilLine.StopState}}}`: Per line, the result of `api.engine.util.line.getDetailedLineProblems`.
- `line2problem : {Engine.Entity : boolean}`: True for lines with stop problems (no path, duplicate or incompatible stop) or line issues.
- `vehicle2problem : {Engine.Entity : boolean}`: True for vehicles with a vehicle problem or a cargo round trip.
- `line2vehicleProblemCount : {Engine.Entity : integer}`: Number of vehicles with problems per line.

## LVMDisplayData (record)

```teal
global record LVMDisplayData
```

- `notificationProblemColor : Vec4f`
- `contrastColor : Vec4f`
- `textColor : Vec4f`
- `waypointColor : Vec4f`
- `selectedLineColor : Vec4f`

## VehicleProblems (record)

```teal
global record VehicleProblems
```

- `vehicles2problem : {Engine.Entity : boolean}`
- `problemCount : integer`

## LVMButtonIAs (record)

```teal
global record LVMButtonIAs
```

Input action keys of the Line & Vehicle Manager's gamepad buttons; several buttons share a key and differ by 2D/3D mode.

- `btn2D3D_Switch : string`: Switch between 2D and 3D mode (`IA_OPTION1`).
- `btn2D_ActionButtons : string`: Focus the action buttons in 2D mode (`IA_OPTION2`).
- `btn3D_Move : string`: also locate
- `btn2D_MoreOptions : string`: Hold for more options in 2D mode (`IA_OPTION3`).
- `btn3D_Configure : string`: Configure in 3D mode (`IA_OPTION3`).
- `btn2D_NextSection : string`: Next section in 2D mode (`IA_OPTION4`).
- `btn3D_Delete : string`: Delete in 3D mode (`IA_OPTION4`).
- `btn2D_MoveStopHold : string`: Hold to move a stop in 2D mode (`IA_SELECT`).
- `btnMore_SendToLine : string`: More options, send to line (`IA_OPTION1`).
- `btnMore_NewLine : string`: More options, new line (`IA_OPTION2`).
- `btnMore_DeleteSell : string`: More options, delete line or stop / sell vehicle (`IA_MENU_BACK`).
- `btnMore_BuyVehicle : string`: More options, buy vehicle (`IA_SELECT`).
- `modifier3D_PreferWaypoint : string`: Modifier held in 3D mode to place waypoints (`IA_PRECISION_MODE`).

## LVMDeleteSell (record)

```teal
global record LVMDeleteSell
```

What the gamepad "delete / sell" button acts on, depending on the focused section (line list, vehicle list or line panel).

- `focusedLine : Engine.Entity`: Focused line in the line list (deleted).
- `focusedVehicle : Engine.Entity`: Focused vehicle in the vehicle list (sold).
- `focusedStopLine : Engine.Entity`: Line of the focused stop in the line panel.
- `focusedStopNumber : integer`: 1-based path index of the focused stop.
- `focusedApiStopIndex0 : integer`: 0-based engine stop index of the focused stop (nil for waypoints).

## BufferedLVMSendData (record)

```teal
global record BufferedLVMSendData
```

A send-to-line command that failed and is retried when the line is selected again.

- `line : Engine.Entity`: Target line.
- `vehicle : Engine.Entity`: Vehicle to send.
- `stopToSend : integer`: Stop index passed to `api.cmd.makeVehicleSetLineCmd`.

## LVMDnDData (record)

```teal
global record LVMDnDData
```

Line and revision at the start of a stop drag-and-drop in the line panel.

- `dragStartLineEntity : Engine.Entity`: Line whose stop is dragged.
- `dragStartLineRevision : Engine.Revision`: Revision of the line when the drag started.

## LVMIconPaths (record)

```teal
global record LVMIconPaths
```

Icon paths used by the Line & Vehicle Manager; `row*` for list rows (20 px), `reg*` regular size, `ab*` action buttons, `hud*` HUD icons.

- `rowRename : string`
- `rowConfigure : string`
- `rowMove : string`
- `rowDelete : string`
- `rowSelectTerminals : string`
- `rowLocate : string`
- `rowRemove : string`
- `rowCart : string`
- `problemAlert : string`
- `problemArrow : string`
- `regSelectTerminals : string`
- `regRename : string`
- `regAutoRename : string`
- `regFilterVisible : string`
- `regGradient : string`
- `regDragGrip : string`
- `abNewLine : string`
- `abSendToLine : string`
- `abBuyVehicles : string`
- `abFilterVisible : string`
- `abFilterRoad : string`
- `abFilterTram : string`
- `abFilterRail : string`
- `abFilterWater : string`
- `abFilterAir : string`
- `abReverse : string`
- `abStartStop : string`
- `abPaint : string`
- `abDuplicate : string`
- `abReplace : string`
- `abConfigure : string`
- `abSendToDepot : string`
- `abSell : string`
- `abAutoRename : string`
- `hudTerminal : string`
- `hudConfigureNone : string`
- `hudConfigure : string`
- `hudMove : string`
- `hudAdd : string`
- `hudRemove : string`

## CommonActionParams (interface)

```teal
global interface CommonActionParams
```

Shared state, refs and actions of the Line & Vehicle Manager, created once by the manager tool and passed to its panels, tooltips and HUD icons.

- `gameCtx : GameContext`: The game context.
- `parent : ReactNodeRef`: Node of the manager tool.
- `lvmButtonIAs : LVMButtonIAs`: Input action keys of the gamepad buttons.
- `iconPaths : LVMIconPaths`: Icon paths of the manager.
- `lineState : ReactStateT<Line.ReactLine>`: The line currently being edited (single selected line).
- `problemCacheState : ReactStateT<ProblemCache>`: Line and vehicle problem cache.
- `countDataState : ReactStateT<LVMCountData>`: Line, depot and vehicle counts.
- `gamepadInputModeState : ReactStateT<boolean>`: True while gamepad input is used.
- `lvm2D3DModeState : ReactStateT<LVM2D3DMode>`: Current 2D/3D mode.
- `lvmSectionState : ReactStateT<LVMSection>`: Section with the gamepad focus.
- `lvmActionButtonsState : ReactStateT<LVMActionButtons>`: Action button bar with the focus.
- `moreOptionsState : ReactStateT<boolean>`: True while the "more options" button is held.
- `sendToLineHoveredLineState : ReactStateT<Engine.Entity>`: Line hovered in the send-to-line popover.
- `addStopHoveredStationState : ReactStateT<LineUtil.StationSelectionDetails>`: Station hovered while adding a stop.
- `preferLineOverTNEdgeState : ReactStateT<boolean>`: Passed to the line viewer's `preferLineOverTransportNetworkEdge`.
- `creatingNewLineState : ReactStateT<boolean>`: True while the player picks the first station of a new line.
- `gamepadNo3DSelectionInfoState : ReactStateT<boolean>`
- `vehicleManagerStateRef : ReactRefT<VehicleManagerState>`: Vehicle list contents (selected and unselected vehicles).
- `lineManagerStateRef : ReactRefT<LineManagerState>`: Line and depot list selection.
- `vehicleManagerRef : ReactRefWrapApiT<VehicleManagerParams, VehicleManagerApi>`: Vehicle manager node; its API is `VehicleManagerApi`.
- `lineAndDepotListRef : ReactRefWrapApiT<LineAndDepotListParams, LineAndDepotListApi>`: Line and depot list node; its API is `LineAndDepotListApi`.
- `vehicleStoreOpenForDepotAndLineRef : ReactRefT<VehicleStoreOpenInfo>`: Depot and line the vehicle store is open for, or nil.
- `deleteSellRef : ReactRefT<LVMDeleteSell>`: Target of the gamepad delete / sell button.
- `forceUpdateTag : ReactRefT<string>`: Changing string used as local key to force HUD icons to rebuild.
- `bufferedSendDataRef : ReactRefT<{BufferedLVMSendData}>`: Failed send-to-line commands waiting for a retry.
- `prevLvmModeRef : ReactRefT<{LVMMode, any}>`
- `lvmDnDDataRef : ReactRefT<LVMDnDData>`: State of a running stop drag-and-drop.
- `configureLinesPopoverPosRef : ReactRefWrap`
- `sendToLinePopoverPosRef : ReactRefWrap`
- `btnBuyVehiclesRef : ReactRefWrap`
- `btnSendToLineRef : ReactRefWrap`
- `btnNewLineRef : ReactRefWrap`
- `switchMode : function(newMode : {LVMMode, any | LineUtil.StopSelectionDetails})`: Switches the manager mode, e.g. `{"SEGMENT", stopNumber}` or `{"MAIN", nil}`.
- `switchToPrevMode : function()`: Returns to the previous `SEGMENT` mode, otherwise to `MAIN`.
- `getModeState : function() : {LVMMode, any}`: Returns the current `{mode, param}`.
- `addFeedback : function(message : string, mode : string, dialogData : FeedbackListUtil.FeedbackDialogParam, id : number)`: Shows a message in the manager's feedback list (mode e.g. "Error", "Warning", "Question").
- `updateIA : function(newContext : LineUtil.GuiContextState)`: Sets the context state whose input actions the manager window applies.
- `onProcessMouseEvent : function(evt : Gui.Mouse.Event) : boolean`
- `accessNotificationCacheFn : function(entity : Engine.Entity) : {Notification}`: The game context's `accessNotificationCacheFn`.
- `carrierFn : function() : {Carrier}`: Returns the carriers allowed by the manager's category filter, or nil.
- `getDisableHudInteraction : function() : boolean`: True while HUD icons should not react (gamepad input in 2D mode).
- `resetLVMDnDData : function()`: Clears `lvmDnDDataRef`.
- `getFirstStopToSendTo : function(vehicleToSendToLine : Engine.Entity, lineToSendTo : Engine.Entity) : integer`: Returns the stop index a vehicle should start at on a line, using the "first stop to send" scheme for its carrier.
- `addDepotVehiclesToDepotVehicleCompare : function(depotEntity : Engine.Entity)`: Remembers the current vehicles of a depot for comparison.
- `removeDepotVehiclesFromDepotVehicleCompare : function(depotEntity : Engine.Entity)`: Forgets the remembered vehicles of a depot.
- `addVehicleToDepotVehicleCompare : function(vehicleEntity : Engine.Entity, depotEntity : Engine.Entity)`: Adds a vehicle to the remembered vehicles of a depot.
- `removeVehicleFromDepotVehicleCompare : function(vehicleEntity : Engine.Entity)`: Removes a vehicle from all remembered depot vehicles.
- `clearDepotVehicleCompare : function()`: Forgets all remembered depot vehicles.
- `newLine : function(newLineStartingWithStation : LineUtil.StationSelectionDetails, vehicleEntities : {Engine.Entity})`: Creates a new line starting at the station; the given vehicles are assigned to it.
- `selectLine : function(LineUtil.LineSelectionDetails, scrollToRow : boolean, selectSegment : integer)`: Selects a single line (optionally scrolling to its row and selecting a segment).
- `deleteGamepadFocusedLine : function()`: Deletes the line focused with the gamepad, asking first if it has vehicles.
- `addStop : function(station : LineUtil.StationSelectionDetails, insertAtStopNumber : integer)`: Inserts a stop at the station after the given stop number (at the end if nil) and commits the line.
- `removeStop : function(stop : LineUtil.StopSelectionDetails)`: Removes a stop; removing the last stop deletes the line if it has no vehicles.
- `moveStop : function(movingStop : LineUtil.StopSelectionDetails, station : LineUtil.StationSelectionDetails)`: Moves a stop to another station or terminal.
- `moveVia : function(oldStopNumber : integer, newStopNumber : integer)`: Moves a path entry from one stop number to another.
- `addAlternativeTerminal : function(details : LineUtil.StationSelectionDetails)`: Adds the terminal as alternative terminal of the stop selected in the current mode.
- `removeAlternativeTerminal : function(details : LineUtil.StationSelectionDetails)`: Removes the terminal from the alternative terminals of the stop selected in the current mode.
- `selectAlternativeTerminal : function(stopNumber : integer, stationIndex1 : integer, terminalIndex1 : integer, add : boolean)`: Adds (`add` true) or removes an alternative terminal of the given stop (1-based indices).
- `changeMainTerminal : function(stopNumber : integer, stationIndex1 : integer, terminalIndex1 : integer)`: Sets the main station and terminal (1-based indices) of the given stop.
- `addWaypoint : function(waypoint : Waypoint, insertAtStopNumber : integer)`: Inserts a waypoint after the given stop number.
- `removeWaypoint : function(waypoint : LineUtil.WaypointSelectionDetails)`: Removes the waypoint with the given tag.
- `moveWaypoint : function(movingWaypoint : LineUtil.WaypointSelectionDetails, apiWaypoint : Waypoint)`: Replaces the waypoint at the given stop number with a new position.
- `addOrRemoveVehicleToVehicleList : function(vehicleDepotDetails : LineUtil.VehicleDepotDetails)`: Adds a vehicle to the vehicle list (selected) or removes it.
- `addOrRemoveDepotVehiclesToVehicleList : function(vehicleDepotDetails : LineUtil.VehicleDepotDetails)`: Adds all vehicles of a depot to the vehicle list or removes them.
- `addOrRemoveLineVehiclesToVehicleList : function(lineEntity : Engine.Entity, removeFromList : boolean, clearList : boolean)`: Adds all vehicles of a line to the vehicle list or removes them, optionally clearing the list first.
- `isVehicleSelected : function(vehicleEntity : Engine.Entity) : boolean`: True if the vehicle is selected in the vehicle list.
- `isVehicleInVehicleList : function(vehicleEntity : Engine.Entity) : boolean`: True if the vehicle is in the vehicle list.
- `selectOrDeselectVehicleInVehicleList : function(vehicleEntity : Engine.Entity, selectVehicle : boolean)`: Selects or deselects a vehicle in the vehicle list.
- `areAllDepotVehiclesInVehicleList : function(vehicleDepotEntity : Engine.Entity) : boolean`: True if the depot has vehicles and all of them are in the vehicle list.
- `openVehicleDepot : function(depotEntity : Engine.Entity, lineEntity : Engine.Entity)`: Opens the vehicle store for the depot; bought vehicles are added to the list and sent to the line if one is given.
- `areAnyVehiclesInDepot : function(vehicleDepotEntity : Engine.Entity) : boolean`: True if the depot contains vehicles.
- `selectOrDeselectDepot : function(vehicleDepotDetails : LineUtil.VehicleDepotDetails)`: Selects or deselects a depot in the line and depot list.
- `sendVehiclesToLine : function(selection : {Engine.Entity}, lineEntity : Engine.Entity)`: Sends the vehicles to the line, using the "first stop to send" scheme.
- `sendVehiclesToDepot : function(selection : {Engine.Entity}, depotEntity : Engine.Entity)`: Sends the vehicles to a depot and returns to `MAIN` mode.
- `openWarehouse : function(warehouseEntity : Engine.Entity)`: Closes the vehicle store and opens the entity window of the warehouse.
- `sellVehicle : function(vehicleEntity : Engine.Entity)`: Sells the vehicle and removes it from the vehicle list.
- `openCargoFilter : function(where : LineUtil.StopSelectionDetails, pos : Vec2f, onClose? : function())`: Opens the cargo filter (stop configuration) window for a stop.
- `closeCargoFilter : function()`: Closes the cargo filter window.
- `btnNewLineEnabled : function() : boolean`: True if "New Line" is available (not already creating one and not disabled by the game context).
- `btnNewLineOnClick : function()`: Clears the selection and starts creating a new line.
- `btnSendToLineEnabled : function() : boolean`: True if vehicles are selected and sending is not blocked.
- `btnSendToLineOnClick : function()`: Opens the send-to-line popover for the selected vehicles.
- `btnBuyVehiclesEnabled : function() : boolean`: True if a single line or a depot is selected.
- `btnBuyVehiclesOnClick : function()`: Opens the vehicle store at the best depot for the selected line, or at the selected depot.

## LineManagerPanelParams (record)

```teal
global record LineManagerPanelParams
```

Parameters of the line panel of the Line & Vehicle Manager (`line_manager_panel.tl`).

- `commonParams : CommonActionParams`: Shared manager state and actions.
