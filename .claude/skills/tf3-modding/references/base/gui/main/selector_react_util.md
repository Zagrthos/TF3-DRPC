# gui.main.selector_react_util

Definition file: `base/tealdef/gui/main/selector_react_util.d.tl` (in the game folder).

The default entity selector of the game view: picking entities with the mouse (hover
highlight, tooltips, selecting an entity opens its window), the HUD icon configuration
that goes with it, and the gamepad inspector tool.

## SelectorReactUtil (record)

```teal
global record SelectorReactUtil
```
Load: `local selector_react_util = ug_require "::/gui/main/selector_react_util.tl" as SelectorReactUtil`

Default entity selector, action descriptors and the inspector tool of the game view.

- `defaultHudIconFilter : {Engine.ComponentType}`: Component types whose entities get HUD icons by default (transport vehicles, industries, warehouses, station groups, towns, base nodes, subconstructions).
- `defaultHudIconFilterTvCarrier : {Carrier}`: Carriers of transport vehicles that get HUD icons by default (road, rail, tram, air, water).
- `makeDefaultSelector : function(refRC : ReactRefWrapApiT<Builtin.RendererComponentParam, Builtin.RenderComponentAPI>, selectedEntityRef : ReactRefT<Engine.Entity>, allowStacking? : boolean, filters? : ReactRefT<GameContext.Filters>, invertedSelectionColors? : boolean) : TreeNodeId`  (call as `selector_react_util.makeDefaultSelector(…)`): Creates the default `builtin.Selector` of the game view.
    - param `refRC`: Renderer component in which entities are picked.
    - param `selectedEntityRef`: Ref that receives the hovered entity (used by the tooltips).
    - param `allowStacking`: Whether a selected entity's window is stacked onto the open ones; `nil` means "stack in gamepad mode".
    - param `filters`: Optional game context filters; a non-empty `selectorEntityFilter` limits which entities can be picked.
    - param `invertedSelectionColors`: Uses the dark selection colours.
    - notes: Selecting an entity fires the React event `selectEntity` (which opens the entity window) and the GUI script event `select` on `mainView` (plus `select-world` for terrain/water picks); clicking a marker fires `selectMarker`. Edges and base nodes without a configurable switch or traffic light cannot be picked.
    - used 9x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo.tl:318`: `selector_react_util.makeDefaultSelector(param.toolParam.gameCtx.rendererComponent, selectedEntityRef, true, nil, true),`
- `makeDefaultSelectorTooltips : function(selectedEntityRef : ReactRefT<Engine.Entity>) : TreeNodeId`  (call as `selector_react_util.makeDefaultSelectorTooltips(…)`): Creates the action tooltip that shows the default entity tooltip for the entity in `selectedEntityRef`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:956`: `selector_react_util.makeDefaultSelectorTooltips(selectedEntityRef),`
- `makeDefaultSelectorIfMouse : function(refRC : ReactRefWrapApiT<Builtin.RendererComponentParam, Builtin.RenderComponentAPI>, selectedEntityRef : ReactRefT<Engine.Entity>, invertedSelectionColors? : boolean) : TreeNodeId`  (call as `selector_react_util.makeDefaultSelectorIfMouse(…)`): Like `makeDefaultSelector`, but returns `nil` in gamepad mode.
    - used 11x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:104`: `selector_react_util.makeDefaultSelectorIfMouse(gameCtx.rendererComponent, selectedEntityRef),`
- `makeDefaultSelectorTooltipsIfMouse : function(selectedEntityRef : ReactRefT<Engine.Entity>) : TreeNodeId`  (call as `selector_react_util.makeDefaultSelectorTooltipsIfMouse(…)`): Like `makeDefaultSelectorTooltips`, but returns `nil` in gamepad mode.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/subventions_gui.tl:459`: `selector_react_util.makeDefaultSelectorTooltipsIfMouse(selectedEntityRef),`
- `DefaultSelectorComp : Recipe<DefaultSelectorCompParams>`: Focusable placeholder node used by `InspectorSelector`; takes focus on mount in inspector mode.
- `makeDefaultSelectorCombinedFn : function(params : DefaultSelectorCompParams) : function() : TreeNodeId`  (call as `selector_react_util.makeDefaultSelectorCombinedFn(…)`): Returns an action function that builds the HUD icon configuration, the default selector and its tooltips in one action descriptor.
    - notes: The selector part is left out while the React event `preventReactSelector` is in effect (until `allowReactSelector`). HUD icons respect the HUD filter saved in the GUI save data.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/view_manager.tl:57`: `local actionFn = selector_react_util.makeDefaultSelectorCombinedFn{refRC = gameCtx.rendererComponent, gameCtx = gameCtx}`
- `InspectorSelector : Recipe<DefaultSelectorCompParams>`: Recipe behind `InspectorTool`. While active (gamepad), it sets the selector action in inspector mode; `IA_INSPECTOR` or `IA_ABORT` close it.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:493`: `item = selector_react_util.InspectorSelector{`
- `makeDefaultSelectorAction : function(gameCtx : GameContext, layerConfig? : LayerConfig, horizontalPromptList? : boolean, makeCustomActionDescriptorChild? : (function() : TreeNodeId), invertedSelectionColors? : boolean) : function() : TreeNodeId`  (call as `selector_react_util.makeDefaultSelectorAction(…)`): Returns the standard action function for windows and tools that keep the normal entity selection active. The action has the HUD icons, the default selector and its tooltips.
    - param `gameCtx`: Game context.
    - param `layerConfig`: Optional layer configuration added to the action descriptor.
    - param `horizontalPromptList`: Shows the input prompts in a horizontal list.
    - param `makeCustomActionDescriptorChild`: Optional; builds an extra child of the action descriptor.
    - param `invertedSelectionColors`: Uses the dark selection colours.
    - returns: A function for `setActionFn` that returns the action descriptor.

    ```lua
    -- from sim_person.tl (an entity window)
    params.setActionFn(selector_react_util.makeDefaultSelectorAction(params.gameCtx, nil, true))
    ```

    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane_entity_window.script.tl:10`: `params.setActionFn(selector_react_util.makeDefaultSelectorAction(params.gameCtx, nil, true))`
- `makeDefaultSelectorActionIfMouse : function(gameCtx : GameContext, layerConfig? : LayerConfig, horizontalPromptList? : boolean, makeCustomActionDescriptorChild? : (function() : TreeNodeId), invertedSelectionColors? : boolean) : function() : TreeNodeId`  (call as `selector_react_util.makeDefaultSelectorActionIfMouse(…)`): Like `makeDefaultSelectorAction`, but leaves out the selector and its tooltips in gamepad mode.
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:4445`: `return selector_react_util.makeDefaultSelectorActionIfMouse(params.gameCtx)()`
- `InspectorTool : ToolReactUtil.DelegatingToolDefinition<InspectorToolParam, InspectorToolParamInternal>`: Delegating tool (name `"Inspector"`) for picking entities with the gamepad; push it with `tool_react_util.firePushToolEvent{tool = selector_react_util.InspectorTool}`.
    - used 2x in the base game, e.g. `base/content/gui/gui/main/game.tl:371`: `tool_react_util.firePushToolEvent{tool = selector_react_util.InspectorTool}`

### SelectorReactUtil.DefaultSelectorCompParams (record)

```teal
record DefaultSelectorCompParams
```

Parameters of `DefaultSelectorComp`, `InspectorSelector` and `makeDefaultSelectorCombinedFn`.

- `meta : Meta`: Meta data of the node.
- `refRC : ReactRefWrapApiT<Builtin.RendererComponentParam, Builtin.RenderComponentAPI>`: Reference to the renderer component (3D view) in which entities are picked.
- `inspectorMode : boolean`: Inspector mode (gamepad); shows a crosshair and the terrain circle at the focused selection, and the component takes focus on mount.
- `gameCtx : GameContext`: Game context; supplies the notification cache and the selector filters.
- `invertedSelectionColors : boolean`: Uses the dark (`inverted`) selection colours of `SelectionGuiConfig`.

### SelectorReactUtil.InspectorToolParam (record)

```teal
record InspectorToolParam
```

empty

Public tool parameter of `InspectorTool` (empty).

### SelectorReactUtil.InspectorToolParamInternal (record)

```teal
record InspectorToolParamInternal
```

- `activate : function(setActionFn : function((function() : TreeNodeId), ?string))`: Activates the inspector selector; receives the tool stack's `setActionFn`.
- `deactivate : function()`: Deactivates the inspector selector and clears its action.
