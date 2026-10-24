# gui.entity_window.view_manager

Definition file: `base/tealdef/gui/entity_window/view_manager.d.tl` (in the game folder).

Types of the entity window manager (`/gui/entity_window/view_manager.tl`) and of the functions that
build entity window contents (`make_entity_window.tl`, `make_non_entity_window.tl`). Also the
return type that a construction's or model's `entityWindowScript` must produce.

## IEntityWindowParam (interface)

```teal
global interface IEntityWindowParam
```

Base parameters passed to every entity window content recipe (vehicle, town, industry, station ... windows).

Notes:

Content recipes are created by `make_entity_window.makeEntityWindowContent` and returned inside a
`ViewManager.MakeEntityWindowResult`. A content recipe typically calls `params.setActionFn(...)` to set
the selector action of the window (e.g. `selector_react_util.makeDefaultSelectorAction(params.gameCtx, layerConfig, true)`).

- `meta : Meta`: React meta data of the content; the base game sets a `tag` such as `"temp.view.entity.content.vehicle"`.
- `keyEntity : Engine.Entity`: The entity which the window is displaying (mutually exclusive with keyNonEntity)
- `replacedEntity : Engine.Entity`: Subconstruction the window was opened for when the window shows its parent construction; used to pre-select the matching tab.
- `keyNonEntity : string`: Special case where EntityWindow tool is used for non-entities (mutually exclusive with keyEntity)
- `gameCtx : GameContext`: Game context of the GUI.
- `simple : boolean`: Set to true when the content is shown as a tab inside a `ConstructionWindow`.
- `isMapEditor : boolean`: True when the window is opened in the map editor.
- `isSandboxMode : boolean`: True when the game runs in sandbox mode.
- `setActionFn : function(actionFn? : (function() : TreeNodeId), key2? : string)`: Sets the action function (the active selector or tool shown while the window is focused); `key2` is an optional key that identifies the action.
- `setActionFn_compareProxy : any`: Value compared instead of `setActionFn` when React checks whether the parameters changed.
- `closeFn : function()`: Closes the window.
- `closeFn_compareProxy : any`: Value compared instead of `closeFn` when React checks whether the parameters changed.

## SetActionFnAndProxy (record)

```teal
global record SetActionFnAndProxy<T>
```

An action setter function together with its comparison proxy, returned by `ViewManager.MakeEntityWindowContentFn.makeSetActionFnWithCombinedKey`.

- `fn : function(actionFn? : (function() : TreeNodeId), key2? : string)`: Action setter to pass as `IEntityWindowParam.setActionFn`.
- `proxy : any`: Comparison proxy to pass as `IEntityWindowParam.setActionFn_compareProxy`.

## ConstructionWindowParam (record)

```teal
global record ConstructionWindowParam is IEntityWindowParam
```

Parameters of `ViewManager.MakeEntityWindowContentFn.ConstructionWindow`, the window of a construction with several parts shown as tabs.

- `initialSubconstruction : Engine.Entity`: Subconstruction whose tab is selected initially.
- `customTabs : {ConData}`: Extra tabs added before the subconstruction tabs, e.g. the "Landmark" tab added by the landmark `entityWindowScript`.
- `filter : function(Engine.Entity) : boolean`: Only subconstructions for which this returns true get a tab.

### ConstructionWindowParam.ConData (record)

```teal
record ConData
```

One tab of a `ConstructionWindow`.

- `entity : Engine.Entity`: Subconstruction (or construction) entity shown in the tab.
- `name : string`: Entity name of the tab's entity.
- `description : string`: Tab label, e.g. "Train Station", "Depot", "Industry", "Landmark".
- `subconstructionIndex : integer`: 1-based index in the construction's `subconstructions`; -1 means the construction itself.
- `sortKey : integer`: Sort key of the tab (lower first); the base game uses values such as 6000 for industries and 11000+ for stations.
- `customTabData : ViewManager.MakeEntityWindowResult`: Ready-made window content for the tab; when nil, the content of the subconstruction is built with `makeEntityWindowContent`.

## ViewManager (record)

```teal
global record ViewManager
```
Load: `local view_manager = ug_require "::/gui/entity_window/view_manager.tl" as ViewManager`

Manager of the entity windows; opens a window when an entity is selected.

Notes:

The `ViewManager` recipe listens to the React events `selectEntity` (`{entity, stack, dontBlink}`),
`selectViewKey` (`{entity, nonEntity, stack, dontBlink, extraParam}`, for windows of non-entities such as
missions and subsidies) and `closeAllWindows`. To open the window of an entity from GUI code, fire
`react.fireEvent(nil, "selectEntity", { entity = entity, stack = true })`; with `stack = false` other
unpinned windows are closed first.

- `ViewManager : Recipe<ViewManagerParam>`: Recipe that opens and closes entity windows in reaction to the `selectEntity`, `selectViewKey` and `closeAllWindows` events; renders nothing itself.
    - used 2x in the base game, e.g. `base/content/gui/gui/main/game.tl:101`: `local refViewManager = react.useNodeRef(view_manager.ViewManager)`
- `getEntityDetailsDefaultTool : function(gameCtx : GameContext, isMapEditor : boolean, isSandboxMode : boolean, sharedStateRef : ReactRefT<ViewManager.ViewManagerParam.SharedState>) : ReactToolDefinition<any>, any`  (call as `view_manager.getEntityDetailsDefaultTool(…)`): get the EntityDetailsTool + parameters to be used for the special case of it being the default tool
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:398`: `local defaultTool, defaultToolParam = view_manager.getEntityDetailsDefaultTool(gameCtx, params.isMapEditor, params. isSandboxMode, sharedStateRef)`

### ViewManager.MakeEntityWindowResult (record)

```teal
record MakeEntityWindowResult
```

Description of the content of an entity window: which recipe to show with which parameter, plus title,
locate and auto-close behaviour. Also the return value of an `entityWindowScript` function.

- `recipe : Recipe<IEntityWindowParam>`: Content recipe, called with `param`.
- `builtin : Recipe<Builtin.ICompParam>`: Built-in component recipe used when `recipe` is nil; called with `{entity = param.keyEntity}`.
- `param : IEntityWindowParam`: parameter used only as argument to the recipe (i.e. you can put a subclass here and downcast inside the recipe)
- `replacedEntity : Engine.Entity`: Entity renamed by editing the window title instead of the window's key entity (e.g. the construction for a construction window).
- `autoCloseCondition : function() : boolean`: Function returning true when the window should close (the base game closes it when the entity no longer exists or its revision changed).
- `autoCloseCondition_compareProxy : any`: Value compared instead of `autoCloseCondition` when React checks whether the parameters changed.
- `fallbackTitle : string`: Window title used when the entity has no name, e.g. "Vehicle" or "Construction".
- `locate : function()`: Called by the window's locate button; by default the camera follows the key entity.
- `locate_compareProxy : any`: Value compared instead of `locate` when React checks whether the parameters changed.

### ViewManager.MakeEntityWindowContentFn (record)

```teal
record MakeEntityWindowContentFn
```
Load: `local make_entity_window = ug_require "::/gui/entity_window/make_entity_window.tl" as MakeEntityWindowContentFn`

Interface of `/gui/entity_window/make_entity_window.tl`, which picks the content recipe for an entity.

- `makeEntityWindowContent : function(entity : Engine.Entity, gameCtx : GameContext, isMapEditor : boolean, isSandboxMode : boolean, skipConstruction : boolean, setActionFnAndProxy : {function(actionFn? : (function() : TreeNodeId), key2? : string), any}, entityRevisionCloseConditionAndProxy : ({(function() : boolean), any}), closeFnAndProxy : {function(), any}, filter? : (function(Engine.Entity) : boolean), replacedEntity? : Engine.Entity) : MakeEntityWindowResult`  (call as `make_entity_window.makeEntityWindowContent(…)`): Builds the window content for an entity, choosing the recipe by the entity's components.
  In game: "Construction", "Residential Building", "Rail Crossing" (German: „Konstruktion“, „Wohngebäude“, „Bahnübergang“)
    - param `entity`: Entity to show.
    - param `gameCtx`: Game context.
    - param `isMapEditor`: True in the map editor.
    - param `isSandboxMode`: True in sandbox mode.
    - param `skipConstruction`: Do not resolve the construction of the entity (used for the tabs of a construction window).
    - param `setActionFnAndProxy`: Action setter and its comparison proxy.
    - param `entityRevisionCloseConditionAndProxy`: Auto-close condition and its comparison proxy; may be nil.
    - param `closeFnAndProxy`: Close function and its comparison proxy.
    - param `filter`: Only subconstructions passing this filter are shown.
    - param `replacedEntity`: Subconstruction whose tab is selected initially.
    - returns: The content description, or nil if no window exists for the entity.
    - notes: If the entity belongs to a construction whose description has an `entityWindowScript`, that script function is called instead (with the script's `params`, the construction entity, a tag prefix, `gameCtx`, `isMapEditor`, `isSandboxMode`, `setActionFnAndProxy` and `entityRevisionCloseConditionAndProxy`) and must return a `MakeEntityWindowResult`; the landmarks and the pollution cleanup facility use this. Models of custom-state entities can name such a script in `metadata.metadata.entityWindowScript`. Constructions with more than one relevant subconstruction get a `ConstructionWindow` with tabs.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/view_manager.tl:181`: `return make_entity_window.makeEntityWindowContent(params.keyEntity, params.gameCtx, params.isMapEditor,`
- `ConstructionWindow : Recipe<IEntityWindowParam>`: Window content of a construction with one tab per relevant subconstruction (station, depot, industry, warehouse) and optional custom tabs; the parameter is a `ConstructionWindowParam`.
  In game: "Train Station", "Depot", "Warehouse" (German: „Bahnhof“, „Depot“, „Lager“)
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:35`: `local actionFnAndProxy = make_entity_window.makeSetActionFnWithCombinedKey(make_entity_window.ConstructionWindow, setActionFnAndProxy)`
- `makeSetActionFnWithCombinedKey : function<T>(recipe : Recipe<T>, innerSetActionFnAndProxy : {function(actionFn? : (function() : TreeNodeId), key2? : string), any}) : SetActionFnAndProxy<T>`  (call as `make_entity_window.makeSetActionFnWithCombinedKey(…)`): Wraps an action setter so that the action key is prefixed with the recipe name.
    - param `recipe`: Content recipe whose name is used as key prefix.
    - param `innerSetActionFnAndProxy`: Outer action setter and its comparison proxy.
    - returns: The wrapped setter and a matching comparison proxy, to pass as `setActionFn` and `setActionFn_compareProxy`.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:35`: `local actionFnAndProxy = make_entity_window.makeSetActionFnWithCombinedKey(make_entity_window.ConstructionWindow, setActionFnAndProxy)`

### ViewManager.MakeNonEntityWindowContentFn (record)

```teal
record MakeNonEntityWindowContentFn
```
Load: `local make_non_entity_window = ug_require "::/gui/entity_window/make_non_entity_window.tl" as MakeNonEntityWindowContentFn`

Interface of `/gui/entity_window/make_non_entity_window.tl`, which builds windows that do not show an entity.

- `makeNonEntityWindowContent : function(nonEntity : string, gameCtx : GameContext, isMapEditor : boolean, isSandboxMode : boolean, setActionFnAndProxy : {function(actionFn? : (function() : TreeNodeId), key2? : string), any}, closeFnAndProxy : {function(), any}, extraParam : any) : MakeEntityWindowResult`  (call as `make_non_entity_window.makeNonEntityWindowContent(…)`): Builds the window content for a non-entity key.
    - param `nonEntity`: Window key; the base game handles `"mission"` and keys starting with `"subsidy_"`.
    - param `extraParam`: Extra parameter passed with the `selectViewKey` event.
    - returns: The content description, or nil for unknown keys.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/view_manager.tl:184`: `return make_non_entity_window.makeNonEntityWindowContent(params.keyNonEntity, params.gameCtx, params.isMapEditor, params.isSandboxMode, {setActionFn, setActionF`

### ViewManager.EntityWindowState (record)

```teal
record EntityWindowState
```

Bookkeeping of one open entity window in `ViewManagerParam.SharedState.openEntityWindows`.

- `revision : Engine.Revision`: Revision of the entity when the window was opened.
- `pinned : boolean`: True when the window is pinned (stays open when other windows open).
- `handoverToPinned : boolean`: Set when the window was pinned; the window is then handed over to the default entity window tool.
- `extraParam : any`: Extra parameter the window was opened with (`selectViewKey` event).
- `actionFn : function() : TreeNodeId`: Action function set by the window content with `setActionFn`; applied while the window is active.
- `actionFnKey : string`: Key of `actionFn`.

### ViewManager.ViewManagerParam (record)

```teal
record ViewManagerParam
```

Parameters of the `ViewManager` recipe.

- `refRc : ReactNodeRefApiT<Builtin.RendererComponent>`
- `gameCtx : GameContext`: Game context.
- `isMapEditor : boolean`: True in the map editor.
- `isSandboxMode : boolean`: True in sandbox mode.
- `sharedStateRef : ReactRefT<SharedState>`: these need to be owned by the parent

#### ViewManager.ViewManagerParam.SharedState (record)

```teal
record SharedState
```

State of the open entity windows, shared between the `ViewManager` and the entity window tool.

- `openEntityWindows : {(Engine.Entity | string) : EntityWindowState}`: Open windows by key (entity id, or the non-entity key string).
- `activeEntityWindow : (Engine.Entity | string)`: Key of the window that has focus.
- `windowOrder : {(Engine.Entity | string)}`: Keys of the open windows; the front-most window is last.
- `forceBlink : boolean`
