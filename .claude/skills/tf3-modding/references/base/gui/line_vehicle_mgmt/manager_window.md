# gui.line_vehicle_mgmt.manager_window

Definition file: `base/tealdef/gui/line_vehicle_mgmt/manager_window.d.tl` (in the game folder).

The Line & Vehicle Manager window and tool, its line/depot list and vehicle list APIs, and the data types of the auto-rename and "first stop to send" schemes (loaded from `rename_scheme`, `rename_scheme_component` and `firstStopToSend_scheme` resources).

## Globals

- `LVMMode` (type): defined in line_util.d.tl

Mode of the Line & Vehicle Manager tool, stored as `{mode, param}`.

## LVMFilterConfig (record)

```teal
global record LVMFilterConfig
```

Category filter configuration of the Line & Vehicle Manager's line list.

- `categories : {StatisticsRecipeParams.Category}`: Filter categories offered (carrier categories).
- `onlyVisible : StatisticsRecipeParams.OnlyVisible`: Configuration of the "only visible" filter.

## VehicleStoreOpenInfo (record)

```teal
global record VehicleStoreOpenInfo
```

Depot and line the vehicle store window was opened for from the Line & Vehicle Manager.

- `depot : Engine.Entity`: Depot entity.
- `line : Engine.Entity`: Line that bought vehicles are sent to, or nil.
- `vehicleStoreMode : VehicleStore.VehicleStoreMode`: Mode of the vehicle store (e.g. "Buy").

## LineAndDepotListParams (record)

```teal
global record LineAndDepotListParams
```

Parameters of the line and depot list of the Line & Vehicle Manager.

- `filterState : ReactStateT<StatisticsReactUtil.FilterState>`: Category, search and visibility filter.
- `filterConfigState : ReactStateT<LVMFilterConfig>`: Filter configuration.
- `lineManagerActionButtonsRef : ReactRefWrap`: Node of the line manager action buttons.
- `preferredInitialSelectionRowKey : ReactStateT<Engine.Entity>`: Row (entity) to select initially.
- `scrollToNewLineRef : ReactRefT<Engine.Entity>`: Line to scroll to once its row exists (e.g. after creating it).
- `commonParams : CommonActionParams`: Shared manager state and actions.

## LineAndDepotListApi (record)

```teal
global record LineAndDepotListApi
```

API of the line and depot list, reached through `CommonActionParams.lineAndDepotListRef`.

- `scrollToRow : function(entity : Engine.Entity, forceTopAlign : boolean)`: Scrolls the list to the row of the entity, optionally aligning it at the top.
- `isLineSelected : function(lineEntity : Engine.Entity) : boolean`: True if the line is selected.
- `isDepotSelected : function(depotEntity : Engine.Entity) : boolean`: True if the depot is selected.
- `selectLine : function(lineEntity : Engine.Entity, selectEntity : boolean, selectSegment : integer)`: Selects or deselects a line; with exactly one line selected the manager switches to `SEGMENT` mode (at `selectSegment` or the last segment).
- `selectDepot : function(depotEntity : Engine.Entity, selectEntity : boolean)`: Selects or deselects a depot.
- `deselectAllLinesAndDepots : function()`: Clears the line and depot selection.
- `removeLineFromList : function(lineEntity : Engine.Entity, deleteLine : boolean)`: Deselects a line and returns to `MAIN` mode; with `deleteLine` the line is also dropped from the list.

## VehicleManagerParams (record)

```teal
global record VehicleManagerParams
```

Parameters of the vehicle manager (vehicle list) of the Line & Vehicle Manager.

- `gameCtx : GameContext`: The game context.
- `vehicleManagerActionButtonsRef : ReactRefWrap`: Node of the vehicle manager action buttons.
- `vehicleListRef : ReactRefWrap`: Node of the vehicle list.
- `firstStopToSendSchemesRef : ReactRefT<{FirstStopToSend.Scheme}>`: Available "first stop to send" schemes.
- `firstStopToSendSchemeMapRef : ReactRefT<{Carrier : FirstStopToSend.Schemes}>`: Scheme names to use per carrier.
- `afterSendingToLineFixup : function(sentEntities : {Engine.Entity})`: Called with the vehicles that were sent to a line.
- `scrollToRow : function(row : Engine.Entity, forceTopAlign : boolean)`: Scrolls the vehicle list to a vehicle.
- `sendToLineDisabledState : ReactStateT<boolean>`: True while sending to a line is blocked.
- `vehicleStoreEventParamState : ReactStateT<VehicleStoreWindow.VehicleStoreEventParam>`: Parameters of the vehicle store window to open.
- `commonParams : CommonActionParams`: Shared manager state and actions.

## VehicleManagerApi (record)

```teal
global record VehicleManagerApi
```

API of the vehicle manager, reached through `CommonActionParams.vehicleManagerRef`.

- `addVehiclesToVehicleListAndSelect : function(vehicleEntities : {Engine.Entity})`: Adds vehicles that are not yet in the list, as selected.
- `removeVehiclesFromVehicleList : function(vehicleEntities : {Engine.Entity})`: Removes vehicles from the list.
- `isVehicleInVehicleList : function(vehicleEntity : Engine.Entity) : boolean`: True if the vehicle is in the list (selected or not).
- `areAnyVehiclesInVehicleList : function() : boolean`: True if the list is not empty.
- `selectVehicles : function(vehicleEntities : {Engine.Entity}, selectEntities : boolean)`: Selects or deselects vehicles that are in the list.
- `isVehicleSelected : function(vehicleEntity : Engine.Entity) : boolean`: True if the vehicle is selected.
- `numberSelectedVehicles : function() : integer`: Number of selected vehicles.
- `sendVehiclesToLine : function(selection : {Engine.Entity}, lineEntity : Engine.Entity, sendScheme : FirstStopToSend.Scheme, callback : function(sentEntities : {Engine.Entity}))`: Sends the vehicles to the line with `api.cmd.makeVehicleSetLineCmd`, starting at the stop chosen by the scheme; `callback` receives the vehicles sent.
- `sendVehiclesToDepot : function(selection : {Engine.Entity}, depotEntity : Engine.Entity)`: Sends each vehicle to a depot with `api.cmd.makeVehicleSendToDepotCmd` (the depot argument is not used).
- `clearVehicleList : function()`: Empties the vehicle list.
- `closeVehicleStore : function()`: Closes the vehicle store window.

## VehicleManagerState (record)

```teal
global record VehicleManagerState
```

Contents of the vehicle list; kept between openings of the manager.

- `vehicleListEntitiesSelected : {EntityUtil.EntityAndRevision}`: Selected vehicles in the list.
- `vehicleListEntitiesUnselected : {EntityUtil.EntityAndRevision}`: Unselected vehicles in the list.
- `carriers : {Carrier}`: Carriers of the vehicles in the list.

## LineManagerState (record)

```teal
global record LineManagerState
```

Selection of the line and depot list.

- `lineListEntitiesSelected : {EntityUtil.EntityAndRevision}`: Selected lines.
- `lineListEntitiesUnselected : {EntityUtil.EntityAndRevision}`: Unselected lines.
- `depotListEntitiesSelected : {EntityUtil.EntityAndRevision}`: Selected depots.
- `depotListEntitiesUnselected : {EntityUtil.EntityAndRevision}`: Unselected depots.

## AutoRename (record)

```teal
global record AutoRename
```

Data types of the line auto-rename schemes (resources of type `rename_scheme` and `rename_scheme_component`).

Notes:

A scheme's `formatString` (e.g. `"{key1} - {key2}"`) is filled with `lang_util.format`; each
key in `formatParams` names a scheme component whose `renameFnDefinition` script function
returns that part of the name. Base-game components: `town`, `cargo`, `carrier`, `firststop`,
`laststop` (`gui/line_vehicle_mgmt/auto_rename/`).

Example:
```lua
function data()
	return {
		type = "rename_scheme",
		data = {
			schemeNameRaw = "town_cargo",
			schemeNameTranslated = _("Town - Cargo"),
			sortKey = 21,
			formatString = "{key1} - {key2}",
			formatParams = {
				["key1"] = "town",
				["key2"] = "cargo",
			},
		},
	}
end
```

### AutoRename.RenameParams (record)

```teal
record RenameParams
```

Argument of a scheme component's rename function.

- `lineEntity : Engine.Entity`: Line being renamed.
- `reactLine : Line.ReactLine`: The line as `Line.ReactLine` (from `LineUtil.getReactLineFromGameState`).

### AutoRename.SchemeComponent (record)

```teal
record SchemeComponent
```

Data of a `rename_scheme_component` resource; one part of an auto-generated line name.

- `schemeComponentNameRaw : string`: Identifier that schemes reference in `formatParams`.
- `schemeComponentNameTranslated : string`: Display name.
- `renameFnDefinition : ScriptRef`: Script function `function(AutoRename.RenameParams) : string` returning the name part.

### AutoRename.Scheme (record)

```teal
record Scheme
```

Data of a `rename_scheme` resource; a pattern for auto-generated line names.

- `schemeNameRaw : string`: Identifier of the scheme.
- `schemeNameTranslated : string`: Display name.
- `sortKey : integer`: Sort order in the scheme list (then by `schemeNameRaw`); the first scheme is the default.
- `formatString : string`: Name pattern with `{key}` placeholders.
- `formatParams : {string : string}`: key : schemeComponentNameRaw

## FirstStopToSend (record)

```teal
global record FirstStopToSend
```

Data types of the "first stop to send" schemes (resources of type `firstStopToSend_scheme`), which decide at which stop vehicles start when sent to a line.

Notes:

Base-game schemes: `evenlydistributed`, `firstcargocompatiblestop`, `firststop`,
`nextreachable` (`gui/line_vehicle_mgmt/first_stop_to_send/`). The manager picks the scheme per
carrier and by whether the vehicles can carry passengers (road, air and water:
`evenlydistributed` for passengers, `firststop` for cargo only; rail and tram: `nextreachable`).

Example:
```lua
local Ret = {
	firstStopToSendFn = function(sendParams : FirstStopToSend.SendParams) : {Engine.Entity : integer}
		local vehicle2stopIndex : {Engine.Entity : integer} = {}
		for __, vehicleEntity in ipairs(sendParams.vehicleEntities) do
			vehicle2stopIndex[vehicleEntity] = 0
		end
		return vehicle2stopIndex
	end,
}
return Ret
```

### FirstStopToSend.SendParams (record)

```teal
record SendParams
```

Argument of a scheme's send function, which returns the stop index per vehicle (`{Engine.Entity : integer}`, 0 = first stop).

- `vehicleEntities : {Engine.Entity}`: Vehicles being sent.
- `lineEntity : Engine.Entity`: Target line.

### FirstStopToSend.Scheme (record)

```teal
record Scheme
```

Data of a `firstStopToSend_scheme` resource.

- `schemeNameRaw : string`: Identifier of the scheme.
- `schemeNameTranslated : string`: Display name.
- `sortKey : integer`: Sort order (then by `schemeNameRaw`); the first scheme is the fallback.
- `firstStopToSendFnDefinition : ScriptRef`: Script function `function(FirstStopToSend.SendParams) : {Engine.Entity : integer}`.

### FirstStopToSend.Schemes (record)

```teal
record Schemes
```

schemeNameRaw

- `supportsPassengers : string`: Scheme name used when the vehicles can carry passengers.
- `cargoOnly : string`: Scheme name used for cargo-only vehicles.

### FirstStopToSend.VehicleData (record)

```teal
record VehicleData
```

Properties of the vehicles being sent, used to choose the scheme.

- `carrier : Carrier`: Carrier of the first existing vehicle.
- `supportsPassengers : boolean`: True if any of the vehicles can load passengers.

## DuplicateVehiclesParam (record)

```teal
global record DuplicateVehiclesParam
```

Parameter of the `duplicateVehicles` event, which buys copies of vehicles (new purchase time, full maintenance state) and sends them to the best line and depot.

Example:
```lua
react.fireEvent(nil, "duplicateVehicles", {
	vehicleEntities = {vehicleEntityId},
	addFeedback = addFeedback,
	onBuy = function(__ : {{Engine.Entity, Engine.Revision}}) end,
})
```

- `vehicleEntities : {Engine.Entity}`: Vehicles to duplicate.
- `addFeedback : function(message : string, mode : string, dialogData : FeedbackListUtil.FeedbackDialogParam, id : number)`: Shows an error message (e.g. a mission refusing the purchase).
- `onBuy : function(resultEntities : {{Engine.Entity, Engine.Revision}})`: Called with the bought vehicle entities and revisions.

## ManagerWindow (record)

```teal
global record ManagerWindow
```
Load: `local manager_window = ug_require "::/gui/line_vehicle_mgmt/manager_window.tl" as ManagerWindow`

The Line & Vehicle Manager tool and window. Open it by firing the `openVehicleManager` event; `closeVehicleManager` closes it.

Example:
```lua
local openManagerParam : ManagerWindow.ManagerWindowEventParam = {
	openWithVehicleEntities = {vehicleEntityId},
	sendToLineMode = true,
}
react.fireEvent(nil, "openVehicleManager", openManagerParam)
```

- `ManagerEntryPoint : Recipe<ManagerEntryPointParam>`: Invisible recipe placed in the game GUI (`gui/main/game.tl`) that loads the scheme resources and handles the `openVehicleManager`, `closeVehicleManager` and `duplicateVehicles` events.
    - used 2x in the base game, e.g. `base/content/gui/gui/main/game.tl:279`: `local managerEntryPointRef = react.useNodeRef(manager_window.ManagerEntryPoint)`
- `ManagerTool : ReactToolDefinition<ManagerToolParam>`: Tool definition of the Line & Vehicle Manager, pushed onto the game context's tool stack.
- `ManagerToolWindow : Recipe<ToolReactUtil.IToolWindowParam<ManagerToolParam>>`: Singleton window of the manager tool.
- `ManagerWindowContent : Recipe<ToolReactUtil.ToolWindowParamPayload<ManagerToolParam>>`: Content of the manager window.
  In game: "Creating New Line", "Send To Line", "Buy Vehicles" (German: „Neue Linie erstellen“, „Linie zuweisen“, „Fahrzeuge kaufen“)

### ManagerWindow.ManagerWindowEventParam (record)

```teal
record ManagerWindowEventParam
```

Parameter of the `openVehicleManager` event.

- `openWithLineEntity : Engine.Entity`: Line to select when the manager opens.
- `openWithDepotEntity : Engine.Entity`: Depot to select when the manager opens.
- `openWithVehicleEntities : {Engine.Entity}`: Vehicles to put in the vehicle list when the manager opens.
- `sendToLineMode : boolean`: Open in send-to-line mode for the given vehicles.

### ManagerWindow.ManagerToolParam (record)

```teal
record ManagerToolParam is ToolReactUtil.IToolParam
```

Parameters the entry point pushes onto the tool stack with `ManagerWindow.ManagerTool`.

- `openWithLineEntity : Engine.Entity`: From `ManagerWindowEventParam.openWithLineEntity`.
- `openWithVehicleEntities : {Engine.Entity}`: From `ManagerWindowEventParam.openWithVehicleEntities`.
- `openWithDepotEntity : Engine.Entity`: From `ManagerWindowEventParam.openWithDepotEntity`.
- `sendToLineMode : boolean`: From `ManagerWindowEventParam.sendToLineMode`.
- `vehicleManagerStateRef : ReactRefT<VehicleManagerState>`: Vehicle list contents, owned by the entry point.
- `lineManagerStateRef : ReactRefT<LineManagerState>`: Line and depot selection, owned by the entry point.
- `lvmFilterStateRef : ReactRefT<StatisticsReactUtil.FilterState>`: Line list filter (category, search, only visible).
- `storedLVMModeRef : ReactRefT<{LVMMode, any}>`: Last manager mode.
- `autoRenameSchemeComponentsMapRef : ReactRefT<{string : AutoRename.SchemeComponent}>`: All `rename_scheme_component` resources by `schemeComponentNameRaw`.
- `autoRenameSchemesRef : ReactRefT<{AutoRename.Scheme}>`: All `rename_scheme` resources, sorted by `sortKey`.
- `autoRenameSchemeRef : ReactRefT<AutoRename.Scheme>`: Currently chosen auto-rename scheme.
- `firstStopToSendSchemesRef : ReactRefT<{FirstStopToSend.Scheme}>`: All `firstStopToSend_scheme` resources, sorted by `sortKey`.
- `firstStopToSendSchemeMapRef : ReactRefT<{Carrier : FirstStopToSend.Schemes}>`: Scheme names per carrier.
- `needsToUpdateThenConsumeUpdate : function() : boolean`: Returns true once after each `openVehicleManager` event.
- `onBack : function()`

### ManagerWindow.ManagerEntryPointParam (record)

```teal
record ManagerEntryPointParam
```

Parameters of `ManagerWindow.ManagerEntryPoint`.

- `gameCtx : GameContext`: The game context.
