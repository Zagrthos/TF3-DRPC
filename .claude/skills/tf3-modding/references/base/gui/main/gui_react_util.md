# gui.main.gui_react_util

Definition file: `base/tealdef/gui/main/gui_react_util.d.tl` (in the game folder).

General React helpers of the game UI: icon/label indicators (the icon with a number used in
HUD icons and entity windows), small layout shortcuts, colour contrast helpers, focus and
input-action wrappers, the emote icon, and `usePlugins` for rendering the plugins of an
extension point. Load it with `ug_require "/gui/main/gui_react_util.tl"`.

## GuiReactUtil (record)

```teal
global record GuiReactUtil
```
Load: `local gui_react_util = ug_require "::/gui/main/gui_react_util.tl" as GuiReactUtil`

General React helpers (indicators, layout shortcuts, focus scopes, plugin rendering).

- `getMouseCursorInteger : function(mouseCursor : GuiReactUtil.MouseCursor) : integer`  (call as `gui_react_util.getMouseCursorInteger(…)`): Converts a `MouseCursor` name to the integer cursor id.
    - param `mouseCursor`: Cursor name. `nil` returns `0` (`Default`).
    - returns: Integer cursor id, e.g. for `mouseCursorOverride` of a component.

    ```lua
    -- from manager_hud_util.tl
    mouseCursorOverride = gui_react_util.getMouseCursorInteger("Default"),
    ```

    - used 42x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:1484`: `mouseCursor = gui_react_util.getMouseCursorInteger("Prospection"),`
- `isHudLabelMode : function(labelMode : LabelMode) : boolean`  (call as `gui_react_util.isHudLabelMode(…)`): Tells whether a label mode is one of the HUD modes.
    - returns: `true` for `HudIconMain` and `HudIconLayer`, otherwise `false`.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:307`: `getTendency = params.showTendency and not gui_react_util.isHudLabelMode(params.mode),`
- `IconLabelIndicator : Recipe<IconLabelIndicatorParams>`: Recipe version of `makeContentLabelIndictator`. Shows an icon with a label, an optional indicator icon and an optional overlay.
    - used 40x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:30`: `gui_react_util.IconLabelIndicator{`
- `makeContentLabelIndictator : function(params : IconLabelIndicatorParams) : TreeNodeId`  (call as `gui_react_util.makeContentLabelIndictator(…)`): Builds an icon/label indicator node, the icon with a number used in HUD icons, cargo lists and entity windows.
    - returns: The indicator node.
    - notes: The icon is an `EmoteIcon` if `useEmoteWithSatisfaction` is set, otherwise one or more image views from `iconPath`. The name is spelled `Indictator` in the game files.

    ```lua
    -- from hud_icon_toolbox.tl
    gui_react_util.makeContentLabelIndictator{
        iconPath = cargo_util.getPassengerCargoIcon(),
        label = lang_util.formatInt(happiness.totalWaiting),
        labelFontColor = fontColor,
        labelMode = "HudIconMain",
        shippingState = "WaitingPassenger",
    }
    ```

    - used 5x in the base game, e.g. `base/content/gui/gui/main/cargo_react_util.tl:243`: `children[#children + 1] = gui_react_util.makeContentLabelIndictator({`
- `integer2TextLayout : function(value : integer, tag? : string) : TreeNodeId`  (call as `gui_react_util.integer2TextLayout(…)`): Builds a horizontal box with one text view showing `value` formatted with `lang_util.formatInt`.
    - param `value`: Number to show.
    - param `tag`: Value for the text view's `meta.tag`.
    - returns: The layout node. The text view is focusable (`forceFocusable = true`) and uses `font-scale-body`.
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:3998`: `gui_react_util.integer2TextLayout(rateState:old(), "menu.statistics.line.rateCell"),`
- `FocusNumberInputField : Recipe<FocusInputFieldParam<number>>`: deprecated, use DoubleSpinBox instead
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_loan_gui.tl:71`: `} or gui_react_util.FocusNumberInputField{`
- `FocusTextInputField : Recipe<FocusInputFieldParam<string>>`: deprecated, use TextInputField with "displayOnly" instead
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_react_util.tl:341`: `editModeState:old() and gui_react_util.FocusTextInputField{`
- `group : function(TreeNodeId) : TreeNodeId`  (call as `gui_react_util.group(…)`): Wraps one node in a horizontal `builtin.BoxLayout`.
    - returns: The box layout node.
- `textColorForColor : function(color : Vec4f) : Vec4f`  (call as `gui_react_util.textColorForColor(…)`): Returns a text colour with enough contrast on the given background colour.
    - returns: The darkest neutral colour of `default_colors.gres` for bright colours (average of RGB above `0.5`), otherwise the lightest neutral colour.
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_manager_panel.tl:350`: `color = gui_react_util.textColorForColor(api.type.Vec4f.new(bubbleColor, 1.0)),`
- `foregroundColorClassForColor : function(color : Vec4f) : GuiReactUtil.ForegroundContrast`  (call as `gui_react_util.foregroundColorClassForColor(…)`): Tells whether dark or light foreground fits on the given colour.
    - returns: `Dark` if the average of RGB is above `0.5`, otherwise `Light`.
    - notes: The line manager maps the result to its own style classes for text on line colours.
    - used 8x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_hud_util.tl:91`: `.. contrast2FGClass[gui_react_util.foregroundColorClassForColor(lineColor)]`
- `stylingHoverClassForColor : function(color : Vec4f) : GuiReactUtil.ForegroundContrast`  (call as `gui_react_util.stylingHoverClassForColor(…)`): Same check as `foregroundColorClassForColor`, meant for choosing hover style classes.
    - returns: `Dark` if the average of RGB is above `0.5`, otherwise `Light`.
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_hud_util.tl:92`: `.. ", " .. contrast2HoverClass[gui_react_util.stylingHoverClassForColor(lineColor)],`
- `makeHorizontalSpacer : function() : TreeNodeId`  (call as `gui_react_util.makeHorizontalSpacer(…)`): Builds an empty component with style class `horizontal-spacer`.
    - used 121x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:162`: `gui_react_util.makeHorizontalSpacer(),`
- `makeVerticalSpacer : function() : TreeNodeId`  (call as `gui_react_util.makeVerticalSpacer(…)`): Builds an empty component with style class `vertical-spacer`.
    - used 38x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:243`: `gui_react_util.makeVerticalSpacer(),`
- `makeHorizontalBox : function({TreeNodeId}) : TreeNodeId`  (call as `gui_react_util.makeHorizontalBox(…)`): Builds a horizontal `builtin.BoxLayout` with the given children.
    - used 72x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:878`: `#children == 0 and refSearchText:get() ~= nil and #refSearchText:get() > 0 and gui_react_util.makeHorizontalBox {`
- `makeVerticalBox : function({TreeNodeId}) : TreeNodeId`  (call as `gui_react_util.makeVerticalBox(…)`): Builds a vertical `builtin.BoxLayout` with the given children.
    - used 37x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3274`: `layout = gui_react_util.makeVerticalBox(children)`
- `transformSetUnlessEqual : function<T>(state : ReactStateT<T>, newValue : T, equals? : (function(T, T) : boolean))`  (call as `gui_react_util.transformSetUnlessEqual(…)`): shortcut to state:transform() with an equals call for early-out
    - param `state`: The react state
    - param `newValue`: The desired value
    - param `equals`: Optional comparison function (defaults to table_util.deepEquals
- `FocusTraversalScope : Recipe<FocusTraversalScopeParam>`: Returns `layout` and registers the navigation input actions (`IA_NAV_LEFT`, `IA_NAV_RIGHT`, `IA_NAV_UP`, `IA_NAV_DOWN`) for it as focus traversal.
    - notes: Used for gamepad and keyboard navigation. Each axis is set up with `react.iaFocusTraversal`, or blocked with a handler that is always disabled.
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:648`: `child = gui_react_util.FocusTraversalScope(react.ref(focusTraversalScopeRef), {`
- `InputActionComponent : Recipe<InputActionComponentParam>`: Returns `layout` and registers the given input action handlers or forwards for it with `react.useInputAction`.
    - notes: Only actions listed in `iaOrder` are registered. Actions without a handler or forward are skipped.
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistics_react_util.tl:108`: `item = gui_react_util.InputActionComponent(react.ref(iaCompRef), {`
- `EmoteIcon : Recipe<EmoteIconParam>`: Shows a happy or unhappy face icon, e.g. for passenger happiness or cargo quality.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1516`: `gui_react_util.EmoteIcon{`
- `Clipper : Recipe<ClipperParam>`: Returns `layout` with content clipping switched on (`react.setClipContent(true)`).
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_popups.tl:381`: `notification and gui_react_util.Clipper{`
- `usePlugins : function<ParamType>(ExtensionPoint<ParamType>, detail? : PluginScopeConfig, recipeParam : ParamType) : {TreeNodeId}`  (call as `gui_react_util.usePlugins(…)`): Builds the nodes of all plugins registered for an extension point, in their order.
    - param `#1`: The extension point (registered with `react.RegisterExtensionPoint...`).
    - param `detail`: Optional refs to the first and last plugin, and fixed nodes to insert.
    - param `recipeParam`: Parameters passed to every plugin recipe.
    - returns: List of plugin nodes, possibly empty.
    - notes: Call it inside a recipe and place the returned nodes in a layout. Plugins with a `condition` function are only built when the condition returns `true` for the recipe parameters. Each plugin recipe is called with `recipeParam` (for extension points with parameters). With `detail.insertHardcoded`, fixed nodes of the host recipe are sorted in between the plugins by their order value.

    ```lua
    -- from station_group.tl
    local pluginWidgets = gui_react_util.usePlugins(station_group_eow.StationGroupEowExtensionPoint, nil, extensionParams)
    ```

    - used 18x in the base game, e.g. `base/content/gui/gui/entity_window/animal/animal.tl:44`: `local widgets = gui_react_util.usePlugins(animal_eow.AnimalEowExtensionPoint, nil, extensionParams)`
- `usePlugins : function<WrappedRecipe, ParamType>(WrapperExtensionPoint<WrappedRecipe, ParamType>, detail? : PluginScopeConfig, recipeParam : ParamType) : {TreeNodeId}`  (call as `gui_react_util.usePlugins(…)`): Builds the nodes of all plugins registered for an extension point, in their order.
    - param `#1`: The extension point (registered with `react.RegisterExtensionPoint...`).
    - param `detail`: Optional refs to the first and last plugin, and fixed nodes to insert.
    - param `recipeParam`: Parameters passed to every plugin recipe.
    - returns: List of plugin nodes, possibly empty.
    - notes: Call it inside a recipe and place the returned nodes in a layout. Plugins with a `condition` function are only built when the condition returns `true` for the recipe parameters. Each plugin recipe is called with `recipeParam` (for extension points with parameters). With `detail.insertHardcoded`, fixed nodes of the host recipe are sorted in between the plugins by their order value.

    ```lua
    -- from station_group.tl
    local pluginWidgets = gui_react_util.usePlugins(station_group_eow.StationGroupEowExtensionPoint, nil, extensionParams)
    ```

    - used 18x in the base game, e.g. `base/content/gui/gui/entity_window/animal/animal.tl:44`: `local widgets = gui_react_util.usePlugins(animal_eow.AnimalEowExtensionPoint, nil, extensionParams)`
- `usePlugins : function(ExtensionPoint0, detail? : PluginScopeConfig) : {TreeNodeId}`  (call as `gui_react_util.usePlugins(…)`): Builds the nodes of all plugins registered for an extension point, in their order.
    - param `#1`: The extension point (registered with `react.RegisterExtensionPoint...`).
    - param `detail`: Optional refs to the first and last plugin, and fixed nodes to insert.
    - returns: List of plugin nodes, possibly empty.
    - notes: Call it inside a recipe and place the returned nodes in a layout. Plugins with a `condition` function are only built when the condition returns `true` for the recipe parameters. Each plugin recipe is called with `recipeParam` (for extension points with parameters). With `detail.insertHardcoded`, fixed nodes of the host recipe are sorted in between the plugins by their order value.

    ```lua
    -- from station_group.tl
    local pluginWidgets = gui_react_util.usePlugins(station_group_eow.StationGroupEowExtensionPoint, nil, extensionParams)
    ```

    - used 18x in the base game, e.g. `base/content/gui/gui/entity_window/animal/animal.tl:44`: `local widgets = gui_react_util.usePlugins(animal_eow.AnimalEowExtensionPoint, nil, extensionParams)`
- `usePlugins : function<WrappedRecipe>(WrapperExtensionPoint0<WrappedRecipe>, detail? : PluginScopeConfig) : {TreeNodeId}`  (call as `gui_react_util.usePlugins(…)`): Builds the nodes of all plugins registered for an extension point, in their order.
    - param `#1`: The extension point (registered with `react.RegisterExtensionPoint...`).
    - param `detail`: Optional refs to the first and last plugin, and fixed nodes to insert.
    - returns: List of plugin nodes, possibly empty.
    - notes: Call it inside a recipe and place the returned nodes in a layout. Plugins with a `condition` function are only built when the condition returns `true` for the recipe parameters. Each plugin recipe is called with `recipeParam` (for extension points with parameters). With `detail.insertHardcoded`, fixed nodes of the host recipe are sorted in between the plugins by their order value.

    ```lua
    -- from station_group.tl
    local pluginWidgets = gui_react_util.usePlugins(station_group_eow.StationGroupEowExtensionPoint, nil, extensionParams)
    ```

    - used 18x in the base game, e.g. `base/content/gui/gui/entity_window/animal/animal.tl:44`: `local widgets = gui_react_util.usePlugins(animal_eow.AnimalEowExtensionPoint, nil, extensionParams)`

### GuiReactUtil.LabelFontColor (enum)

```teal
enum LabelFontColor
```

Colour of the label of an icon/label indicator. Each value maps to a style class.

Notes:

Style classes: `Default` none, `Warning` `warning`, `Error` `error-tape`, `Positive`
`positive`, `Negative` `negative`, `Normal` / `Inferior` / `Poor`
`iconlabelindicator-color-normal` / `-inferior` / `-poor`. The label mode changes some of
them: in `HudIconMain` mode `Default` and `Warning` use HUD-specific classes, and in
`EntityWindow` mode `Inferior` and `Poor` look like `Normal`.

- `"Default"`
- `"Warning"`: Warning colour. Style class `warning`, or `iconlabelindicator-color-hud-warning` in the `HudIconMain` label mode.
- `"Error"`: Error colour (style class `error-tape`).
- `"Positive"`
- `"Negative"`: Colour for negative money values (style class `negative`).
- `"Normal"`
- `"Inferior"`: Quality or happiness below normal. The cargo helpers use it when some cargo is spoiled.
- `"Poor"`: Bad quality or happiness. The cargo helpers use it when more than half of the cargo is spoiled.

### GuiReactUtil.LabelMode (enum)

```teal
enum LabelMode
```

Where an icon/label indicator is shown. Selects the layout, font size and colour classes.

Notes:

`HudIconMain` and `HudIconLayer` use a compact horizontal HUD layout with style class
`hud-icon-main` or `hud-icon-layer` and font `font-scale-title-3`; the other modes place
the label over the icon and use `font-scale-annotation`.

- `"Default"`: Standard layout with the label over the icon.
- `"EntityWindow"`: Layout for entity windows. `Inferior` and `Poor` label colours are shown as `Normal`.
- `"EntityWindowPostfix"`: Layout for extra indicators behind a cargo list in entity windows, e.g. the count of unhappy passengers or delayed deliveries.
- `"HudIconMain"`: Compact layout for the main HUD icons above entities. Only in this mode does `shippingState` tint the background.
- `"HudIconLayer"`: Compact layout for HUD icons of map layers.

### GuiReactUtil.MouseCursor (enum)

```teal
enum MouseCursor
```

Names of the mouse cursors. Convert them with `getMouseCursorInteger` where an integer cursor id is expected.

Notes:

`Default` to `Resize` map to `0`-`4`; the game cursors from `Invalid` on map to `1001`-`1027`
in declaration order.

- `"Default"`: Standard arrow cursor (id `0`).
- `"Text"`: Text cursor (id `1`).
- `"Link"`: Link cursor (id `2`).
- `"Wait"`: Wait cursor (id `3`).
- `"Resize"`: Resize cursor (id `4`).
- `"Invalid"`: User
- `"AddStopValid"`: Game cursor id `1002`.
- `"AddStopInvalid"`: Game cursor id `1003`.
- `"Camera"`: Game cursor id `1004`.
- `"Turn"`: Game cursor id `1005`.
- `"Raise"`: Game cursor id `1006`.
- `"TurnRaise"`: Game cursor id `1007`.
- `"Bulldoze"`: Game cursor id `1008`.
- `"None"`: Game cursor id `1009`.
- `"Grab"`: Game cursor id `1010`. Default cursor while dragging (see `react.d.tl`).
- `"Lock"`: Game cursor id `1011`.
- `"Unlock"`: Game cursor id `1012`.
- `"Config"`: Game cursor id `1013`.
- `"Cut"`: Game cursor id `1014`.
- `"Snap"`: Game cursor id `1015`.
- `"NoSnap"`: Game cursor id `1016`.
- `"Tangent"`: Game cursor id `1017`.
- `"Construction"`: Game cursor id `1018`.
- `"ConstructionTool"`: Game cursor id `1019`.
- `"Upgrade"`: Game cursor id `1020`.
- `"TerrainModify"`: Game cursor id `1021`.
- `"TerrainPaint"`: Game cursor id `1022`.
- `"PlaceAsset"`: Game cursor id `1023`.
- `"Rail"`: Game cursor id `1024`.
- `"StreetStraight"`: Game cursor id `1025`.
- `"Prospection"`: Game cursor id `1026`.
- `"CompanyMechanics"`: Game cursor id `1027`.

### GuiReactUtil.ForegroundContrast (enum)

```teal
enum ForegroundContrast
```

Result of the contrast helpers. Tells whether dark or light foreground fits on a given colour.

- `"Light"`: Light foreground; returned for dark colours (average of RGB at most `0.5`).
- `"Dark"`: Dark foreground; returned for bright colours (average of RGB above `0.5`).

### GuiReactUtil.ShippingState (enum)

```teal
enum ShippingState
```

Where shown cargo currently is. Adds a style class to an icon/label indicator.

Notes:

Style classes: `warehouse`, `waiting-passenger`, `vehicle`, `industry-input`,
`industry-output`. The base stylesheet uses them in `HudIconMain` mode to tint the HUD
icon background with the matching `ShippingState` gameplay colour.

- `"Warehouse"`: Cargo stored in a warehouse (style class `warehouse`).
- `"WaitingPassenger"`: Passengers waiting at a station (style class `waiting-passenger`).
- `"Vehicle"`: Cargo loaded in a vehicle or on a line (style class `vehicle`).
- `"IndustryInput"`: Cargo in an industry's input stock (style class `industry-input`).
- `"IndustryOutput"`: Cargo in an industry's output stock (style class `industry-output`).

### GuiReactUtil.RichLabel (record)

```teal
record RichLabel
```

Label made of a text, an icon or emote, and a second text after the icon. Replaces the plain `label` of an indicator.

- `text : string`: Text before the icon.
- `iconPath : string`: Image path of the icon between the two texts.
- `useEmoteWithSatisfaction : EmoteIconParam.Satisfaction`: If set, an `EmoteIcon` with this satisfaction is shown instead of `iconPath`. Only used in the HUD label modes.
- `textAfterIcon : string`: Text after the icon.

### GuiReactUtil.IconLabelIndicatorParams (record)

```teal
record IconLabelIndicatorParams
```

Parameters of `IconLabelIndicator` and `makeContentLabelIndictator`.

- `iconPath : string | {string}`: Image path of the icon, or a list of paths to show several icons side by side.
- `useEmoteWithSatisfaction : EmoteIconParam.Satisfaction`: If set, an `EmoteIcon` with this satisfaction is used as the icon instead of `iconPath`.
- `label : string`: Text label, usually a number. Ignored when `richLabel` is set.
- `richLabel : RichLabel`: Label with text and icon. Takes precedence over `label`.
- `labelFontColor : LabelFontColor`: Colour of the label (default `Default`).
- `labelMode : LabelMode`: Layout mode; `nil` behaves like `Default`.
- `indicatorIconPath : string`: Image path of a small indicator icon next to the label, e.g. the stock tendency arrows from `CargoUtil.getWarehouseCountIndicatorTexture`.
- `tooltip : string`: Tooltip of the label and indicator icon. Also used for the icon when `iconTooltip` is not set.
- `iconTooltip : string | {string}`: Tooltip of the icon, or one tooltip per icon when `iconPath` is a list.
- `overlay : TreeNodeId`: Node drawn on top of the icon (only in the non-HUD modes).
- `indicator : TreeNodeId`: Node placed above the indicator, e.g. the loading animation of a vehicle.
- `shippingState : ShippingState`: Where the cargo is; adds the matching style class.

### GuiReactUtil.ContentLabelIndicatorParams (record)

```teal
record ContentLabelIndicatorParams
```

Variant of `IconLabelIndicatorParams` with a ready-made content node instead of an icon path.

Notes:

The module uses this record internally: `makeContentLabelIndictator` builds the content
node from the icon parameters and passes the result on in this form. No public function
of the module takes this record.

- `content : TreeNodeId`: Node shown as the icon.
- `label : string`: Text label. Ignored when `richLabel` is set.
- `richLabel : RichLabel`: Label with text and icon.
- `labelFontColor : LabelFontColor`: Colour of the label.
- `labelMode : LabelMode`: Layout mode.
- `labelExtraContent : TreeNodeId`
- `indicatorIconPath : string`: Image path of a small indicator icon next to the label.
- `tooltip : string`: Tooltip of the label and indicator icon.
- `overlay : TreeNodeId`: Node drawn on top of the content.
- `indicator : TreeNodeId`: Node placed above the indicator.
- `shippingState : ShippingState`: Where the cargo is; adds the matching style class.

### GuiReactUtil.FocusInputFieldParam (record)

```teal
record FocusInputFieldParam<T>
```

Parameters of the deprecated `FocusNumberInputField` and `FocusTextInputField` recipes.

- `value : T`: Initial value of the field.
- `onValueChange : function(value : T)`: Called with the entered value when it is accepted (also on focus loss).
- `onCancel : function()`: Called when the input is cancelled.
- `class : string`: Style class of the text field (default `font-scale-body`). Only read by `FocusTextInputField`.
- `maxLength : integer`: Maximum text length. Only read by `FocusTextInputField`.

### GuiReactUtil.FocusTraversalScopeParam (record)

```teal
record FocusTraversalScopeParam
```

Parameters of the `FocusTraversalScope` recipe.

- `horizontal : boolean`: `true` lets the left/right navigation inputs move focus inside `layout`; `false` blocks them; `nil` leaves them to the ancestors.
- `vertical : boolean`: `true` lets the up/down navigation inputs move focus inside `layout`; `false` blocks them; `nil` leaves them to the ancestors.
- `allowBubbleUp : boolean`: if focus can't be moved further, allow the input action to be handled by ancestors (default: false)
- `layout : TreeNodeId`: Node to show inside the scope.

### GuiReactUtil.InputActionComponentParam (record)

```teal
record InputActionComponentParam
```

Parameters of the `InputActionComponent` recipe.

- `iaOrder : {string}`: Names of the input actions to register, in this order.
- `iaForwards : {string : React.IaForward}`: Forwards by input action name. Used when `iaHandlers` has no entry for that action.
- `iaHandlers : {string : React.IaHandler}`: Handlers by input action name.
- `layout : TreeNodeId`: Node to show.

### GuiReactUtil.EmoteIconParam (record)

```teal
record EmoteIconParam
```

Parameters of the `EmoteIcon` recipe.

- `satisfaction : Satisfaction`: Mood to show.
- `tooltip : string`: Tooltip of the icon.
- `forceFocusable : boolean`: Passed to the image's `meta.forceFocusable`.

#### GuiReactUtil.EmoteIconParam.Satisfaction (enum)

```teal
enum Satisfaction
```

Mood shown by an emote icon.

- `"Happy"`: Happy face (`symbol_emote_happy_colored.tga`).
- `"Aloof"`: Unhappy face (`symbol_emote_sad_colored.tga`, the same image as `Sad`). The cargo helpers use it for spoiled cargo and unhappy passengers.
- `"Sad"`: Unhappy face (`symbol_emote_sad_colored.tga`).

### GuiReactUtil.ClipperParam (record)

```teal
record ClipperParam
```

Parameters of the `Clipper` recipe.

- `layout : TreeNodeId`: Node whose content is clipped to the component bounds.

### GuiReactUtil.PluginScopeConfig (record)

```teal
record PluginScopeConfig
```

Parameter collection for usePlugins()

- `firstRef : ReactRefWrap`: if present, filled with a nodeRef to the first (top) plugin
- `lastRef : ReactRefWrap`: if present, filled with a nodeRef to the last (bottom) plugin
- `insertHardcoded : {integer : {TreeNodeId}}`: insert subtrees that are not plugins ( {order : {tree}} )
