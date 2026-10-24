# gui.main.tool_react_util

Definition file: `base/tealdef/gui/main/tool_react_util.d.tl` (in the game folder).

Helpers for tools of the tool stack (`gameCtx.toolStack`): register a tool that opens a
window while it is active, and delegating tools that are pushed through React events.

## ToolReactUtil (record)

```teal
global record ToolReactUtil
```
Load: `local tool_react_util = ug_require "::/gui/main/tool_react_util.tl" as ToolReactUtil`

Tool registration helpers.

- `registerToolWithWindow : function<TP is IToolParam, WP is IToolWindowParam<TP>>(toolName : string, windowId : string, windowRecipe : Recipe<WP>, getDebugString? : (function(toolParam : TP) : string)) : ReactToolDefinition<TP>`  (call as `tool_react_util.registerToolWithWindow(…)`): Registers a tool that shows a singleton window while it is on the tool stack.
    - param `toolName`: Name of the tool.
    - param `windowId`: GUI id of the window; it is hidden while the tool is shelved.
    - param `windowRecipe`: Recipe of the window; it receives an `IToolWindowParam`.
    - param `getDebugString`: Optional; returns a debug description of the tool parameter.
    - returns: The tool definition, to push with `gameCtx.toolStack:get():getApi().push(tool, key, param)`.

    ```lua
    -- from music_player.tl
    local musicPlayerWindowId = "musicPlayerWindow"
    local MusicPlayerWindow = react.RegisterWrapperRecipe("MusicPlayerWindow", builtin.Window, function(params)
        return builtin.Window{
            title = _("Music Player"),
            closable = true,
            onClose = params.onClose,
            content = MusicPlayer(params.payload),
            id = musicPlayerWindowId,
        }
    end)
    local MusicPlayerTool = tool_react_util.registerToolWithWindow("MusicPlayer", musicPlayerWindowId, MusicPlayerWindow)
    -- game_bar.tl:
    getToolStackApi(gameCtx).push(MusicPlayerTool, nil, {gameCtx = gameCtx})
    ```

    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:965`: `local CompanyTool = tool_react_util.registerToolWithWindow("Company", companyWindowId, CompanyWindow, getDebugString)`
- `registerVariantToolWithWindow : function<TP is IToolParam, WP is IToolWindowParam<TP>>(toolName : string, windowId : string, windowRecipe : Recipe<WP>, getActiveVariant : (function(toolParam : TP) : string), getDebugString? : (function(toolParam : TP) : string)) : ReactToolDefinition<TP>`  (call as `tool_react_util.registerVariantToolWithWindow(…)`): Like `registerToolWithWindow`, for a tool with variants; `getActiveVariant` returns the active variant for a tool parameter.
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistics.tl:668`: `local StatisticsTool = tool_react_util.registerVariantToolWithWindow("Statistics", statisticsWindowId, StatisticsWindow, getActiveVariant, getDebugString)`
- `registerVariantToolWithPersistentWindows : function<TP is IToolParam, WP is IToolWindowParam<TP>>(toolName : string, getWindowId : (function(TP) : string), windowRecipe : Recipe<WP>, getActiveVariant : (function(toolParam : TP) : string), getDebugString? : (function(toolParam : TP) : string)) : ReactToolDefinition<TP>`  (call as `tool_react_util.registerVariantToolWithPersistentWindows(…)`): Like `registerVariantToolWithWindow`, but with one window per window id (from `getWindowId`) that is only hidden, not removed, when the tool is popped.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:2912`: `map_editor.MapEditorTool = tool_react_util.registerVariantToolWithPersistentWindows("MapEditor", getWindowId, MapEditorWindow, getActiveVariant, getDebugString)`

same as registerToolWithWindow, but without the call to react.RegisterTool()
- `prepareDefaultToolWithWindowDefinition : function<TP is ToolReactUtil.IToolParam, WP is ToolReactUtil.IToolWindowParam<TP>>(toolName : string, windowId : string, windowRecipe : Recipe<WP>, getActiveVariant? : (function(toolParam : TP) : string), getDebugString? : (function(toolParam : TP) : string), getWindowIdIfPersistent? : (function(TP) : string)) : ReactToolDefinition<TP>`  (call as `tool_react_util.prepareDefaultToolWithWindowDefinition(…)`)
    - used 1x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:428`: `local pauseMenuToolImpl = tool_react_util.prepareDefaultToolWithWindowDefinition("PauseMenu", pauseMenuWindowId, PauseMenu)`
- `registerDelegatingTool : function<PublicParam, InternalParam>(def : ReactToolDefinition<InternalParam>) : DelegatingToolDefinition<PublicParam, InternalParam>`  (call as `tool_react_util.registerDelegatingTool(…)`): Registers `def` as tool and wraps it as `DelegatingToolDefinition`.
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layers_button_ridge.tl:120`: `local LayerTool : ToolReactUtil.DelegatingToolDefinition<LayersButtonRidge.LayerToolParam, LayerToolParamInternal> = tool_react_util.registerDelegatingTool{`
- `firePushToolEvent : function<TP, DP>(evtParam : PushToolEventParam<TP, DP>)`  (call as `tool_react_util.firePushToolEvent(…)`): Fires the React event `PushTool` for a delegating tool.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:371`: `tool_react_util.firePushToolEvent{tool = selector_react_util.InspectorTool}`
- `firePopToolEvent : function<TP, DP>(evtParam : PopToolEventParam<TP, DP>)`  (call as `tool_react_util.firePopToolEvent(…)`): Fires the React event `PopTool` for a delegating tool.
    - used 5x in the base game, e.g. `base/content/gui/gui/layers/layers_button_ridge.tl:181`: `tool_react_util.firePopToolEvent{tool = LayerTool}`
- `onPushToolEvent : function<TP, DP>(tool : DelegatingToolDefinition<TP, DP>, onPush : function(evtParam : ToolReactUtil.PushToolEventParam<TP, DP>))`  (call as `tool_react_util.onPushToolEvent(…)`): Hook that calls `onPush` when `PushTool` is fired for the given delegating tool.
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layers_button_ridge.tl:538`: `tool_react_util.onPushToolEvent(LayerTool, function(pushEvt : ToolReactUtil.PushToolEventParam<LayersButtonRidge.LayerToolParam, LayerToolParamInternal>)`
- `onPopToolEvent : function<TP, DP>(tool : DelegatingToolDefinition<TP, DP>, onPop : function(evtParam : ToolReactUtil.PopToolEventParam<TP, DP>))`  (call as `tool_react_util.onPopToolEvent(…)`): Hook that calls `onPop` when `PopTool` is fired for the given delegating tool.
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layers_button_ridge.tl:542`: `tool_react_util.onPopToolEvent(LayerTool, function(popEvt : ToolReactUtil.PopToolEventParam<LayersButtonRidge.LayerToolParam, LayerToolParamInternal>)`

### ToolReactUtil.IToolParam (interface)

```teal
interface IToolParam
```

input parameters required for most tools

- `gameCtx : GameContext`: Game context; its window container hosts the tool window.

### ToolReactUtil.ToolWindowParamPayload (interface)

```teal
interface ToolWindowParamPayload<TP is IToolParam>
```

'output' that can be passed from the windowWrapper recipe to the windowContent recipe

- `toolParam : TP`: The parameter the tool was pushed with.
- `setActionFn : function(actionFn : (function() : TreeNodeId), key2? : string)`: Sets the action function of the tool (the action descriptor shown while the tool is active), e.g. from `SelectorReactUtil.makeDefaultSelectorAction`.
- `setVariantOverride : function(variant : string)`: Overrides the active variant of the tool.

### ToolReactUtil.IToolWindowParam (interface)

```teal
interface IToolWindowParam<TP>
```

parameters requires to be passed into the window

- `onClose : function()`: Closes the window by popping the tool.
- `payload : ToolWindowParamPayload<TP>`: Tool parameter and tool callbacks, to pass on to the window content.

### ToolReactUtil.DelegatingToolDefinition (interface)

```teal
interface DelegatingToolDefinition<PublicParam, InternalParam>
```

A tool that is pushed and popped through React events instead of directly; the recipe that handles the events pushes the real tool `delegate`.

- `delegate : ReactToolDefinition<InternalParam>`: The registered tool that is actually pushed onto the tool stack.

### ToolReactUtil.PushToolEventParam (record)

```teal
record PushToolEventParam<TP, DP>
```

Parameter of `firePushToolEvent` and of `onPushToolEvent` handlers.

- `tool : DelegatingToolDefinition<TP, DP>`: The delegating tool to push.
- `key : string`: Key of the tool instance.
- `param : TP`: Public tool parameter.
- `allowStacking : boolean`: Whether the tool may be stacked on top of the current one.

### ToolReactUtil.PopToolEventParam (record)

```teal
record PopToolEventParam<TP, DP>
```

Parameter of `firePopToolEvent` and of `onPopToolEvent` handlers.

- `tool : DelegatingToolDefinition<TP, DP>`: The delegating tool to pop.
- `key : string`: Key of the tool instance.
