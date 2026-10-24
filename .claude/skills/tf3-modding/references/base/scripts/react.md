# scripts.react

Definition file: `base/tealdef/scripts/react.d.tl` (in the game folder).

Types of the GUI's React-style library, `::/gui/main/react.lua` (`React`). A recipe is a function that
returns GUI nodes; it is registered once with `React.RegisterRecipe` and called like a function to
create nodes. Inside a recipe, hooks such as `useState`, `useRef`, `onStep` and `onEvent` keep state
between renders. The built-in widgets are in `::/gui/main/builtin.lua` (`Builtin`).

## Globals

- `Builtin` (type): The built-in widget library returned by `ug_require "::/gui/main/builtin.lua"`: recipes for layouts,
text, images, buttons, inputs, windows, charts and the game's tool and renderer components. Most widget
parameters are engine classes, so a parameter table may only contain the declared fields (an unknown key
raises an error). Enumerations are in `builtin.type`.

## RefFill (record)

```teal
global record RefFill
```

Marker returned by `React.ref`; pass it as the first argument of a recipe call to fill a node ref with the created node.

## ReactStyle (record)

```teal
record ReactStyle
```

Marker returned by `React.style`; pass it before the parameters of a recipe call to set style classes.

## IRecipeParamWithMeta (interface)

```teal
global interface IRecipeParamWithMeta
```

Recipe parameters that accept a `meta` table.

- `meta : Meta`: Common node settings (style class, local key, tooltip, focus, tag); see `Meta`.

## TreeNodeId (record)

```teal
global record TreeNodeId
```

Handle of a GUI node created by a recipe call. Recipes return one, a list of them, or nil.

## IRecipe (interface)

```teal
global interface IRecipe
```

Anything that can be called as a recipe.

- `__call : function(IRecipe, any) : TreeNodeId`  (call as `IRecipe(…)`): Creates a node of this recipe.

## Recipe0 (record)

```teal
global record Recipe0 is IRecipe
```

A recipe without parameters, as returned by `RegisterRecipe` for a function without arguments.

- `__call : function(Recipe0) : TreeNodeId`  (call as `Recipe0(…)`): Creates a node: `MyRecipe()`. Optional leading arguments are a `React.ref(...)` and a `React.style{...}`.
- `__call : function(Recipe0, RefFill) : TreeNodeId`  (call as `Recipe0(…)`): Creates a node: `MyRecipe()`. Optional leading arguments are a `React.ref(...)` and a `React.style{...}`.
- `__call : function(Recipe0, ReactStyle) : TreeNodeId`  (call as `Recipe0(…)`): Creates a node: `MyRecipe()`. Optional leading arguments are a `React.ref(...)` and a `React.style{...}`.
- `__call : function(Recipe0, RefFill, ReactStyle) : TreeNodeId`  (call as `Recipe0(…)`): Creates a node: `MyRecipe()`. Optional leading arguments are a `React.ref(...)` and a `React.style{...}`.

## Recipe (record)

```teal
global record Recipe<T> is IRecipe
```

A recipe with one parameter table `T`.

- `__call : function(Recipe<T>, ?T) : TreeNodeId`  (call as `Recipe(…)`): Creates a node: `MyRecipe(params)`. Optional leading arguments are a `React.ref(...)` and a
`React.style{...}`, in either order. A `meta` field in `params` is taken out and applied to the node.
- `__call : function(Recipe<T>, ?RefFill, ?T) : TreeNodeId`  (call as `Recipe(…)`): Creates a node: `MyRecipe(params)`. Optional leading arguments are a `React.ref(...)` and a
`React.style{...}`, in either order. A `meta` field in `params` is taken out and applied to the node.
- `__call : function(Recipe<T>, ?ReactStyle, ?T) : TreeNodeId`  (call as `Recipe(…)`): Creates a node: `MyRecipe(params)`. Optional leading arguments are a `React.ref(...)` and a
`React.style{...}`, in either order. A `meta` field in `params` is taken out and applied to the node.
- `__call : function(Recipe<T>, ?RefFill, ?ReactStyle, ?T) : TreeNodeId`  (call as `Recipe(…)`): Creates a node: `MyRecipe(params)`. Optional leading arguments are a `React.ref(...)` and a
`React.style{...}`, in either order. A `meta` field in `params` is taken out and applied to the node.

## Recipe2 (record)

```teal
global record Recipe2<T, T2> is IRecipe
```

A recipe with two parameters.

- `__call : function(Recipe2<T, T2>, T, T2) : TreeNodeId`  (call as `Recipe2(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(Recipe2<T, T2>, RefFill, T, T2) : TreeNodeId`  (call as `Recipe2(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(Recipe2<T, T2>, ReactStyle, T, T2) : TreeNodeId`  (call as `Recipe2(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(Recipe2<T, T2>, RefFill, ReactStyle, T, T2) : TreeNodeId`  (call as `Recipe2(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.

## Recipe3 (record)

```teal
global record Recipe3<T, T2, T3> is IRecipe
```

A recipe with three parameters.

- `__call : function(Recipe3<T, T2, T3>, T, T2, T3) : TreeNodeId`  (call as `Recipe3(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(Recipe3<T, T2, T3>, RefFill, T, T2, T3) : TreeNodeId`  (call as `Recipe3(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(Recipe3<T, T2, T3>, ReactStyle, T, T2, T3) : TreeNodeId`  (call as `Recipe3(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(Recipe3<T, T2, T3>, RefFill, ReactStyle, T, T2, T3) : TreeNodeId`  (call as `Recipe3(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.

## Recipe4 (record)

```teal
global record Recipe4<T, T2, T3, T4> is IRecipe
```

A recipe with four parameters.

- `__call : function(Recipe4<T, T2, T3, T4>, T, T2, T3, T4) : TreeNodeId`  (call as `Recipe4(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(Recipe4<T, T2, T3, T4>, RefFill, T, T2, T3, T4) : TreeNodeId`  (call as `Recipe4(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(Recipe4<T, T2, T3, T4>, ReactStyle, T, T2, T3, T4) : TreeNodeId`  (call as `Recipe4(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(Recipe4<T, T2, T3, T4>, RefFill, ReactStyle, T, T2, T3, T4) : TreeNodeId`  (call as `Recipe4(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.

## RecipeWithApi0 (record)

```teal
global record RecipeWithApi0<Api>
```

A recipe without parameters whose nodes offer an API (set with `React.provideApi`), reached through a node ref's `getApi()`.

- `__call : function(RecipeWithApi0<Api>) : TreeNodeId`  (call as `RecipeWithApi0(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments.
- `__call : function(RecipeWithApi0<Api>, RefFill) : TreeNodeId`  (call as `RecipeWithApi0(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments.
- `__call : function(RecipeWithApi0<Api>, ReactStyle) : TreeNodeId`  (call as `RecipeWithApi0(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments.
- `__call : function(RecipeWithApi0<Api>, RefFill, ReactStyle) : TreeNodeId`  (call as `RecipeWithApi0(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments.

## RecipeWithApi (record)

```teal
global record RecipeWithApi<T, Api> is IRecipe
```

A recipe with one parameter whose nodes offer an API (set with `React.provideApi`).

- `__call : function(RecipeWithApi<T, Api>, T) : TreeNodeId`  (call as `RecipeWithApi(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(RecipeWithApi<T, Api>, RefFill, T) : TreeNodeId`  (call as `RecipeWithApi(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(RecipeWithApi<T, Api>, ReactStyle, T) : TreeNodeId`  (call as `RecipeWithApi(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(RecipeWithApi<T, Api>, RefFill, ReactStyle, T) : TreeNodeId`  (call as `RecipeWithApi(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.

## RecipeWithApi2 (record)

```teal
global record RecipeWithApi2<T, T2, Api>
```

A recipe with two parameters whose nodes offer an API.

- `__call : function(RecipeWithApi2<T, T2, Api>, T, T2) : TreeNodeId`  (call as `RecipeWithApi2(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(RecipeWithApi2<T, T2, Api>, RefFill, T, T2) : TreeNodeId`  (call as `RecipeWithApi2(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(RecipeWithApi2<T, T2, Api>, ReactStyle, T, T2) : TreeNodeId`  (call as `RecipeWithApi2(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(RecipeWithApi2<T, T2, Api>, RefFill, ReactStyle, T, T2) : TreeNodeId`  (call as `RecipeWithApi2(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.

## RecipeWithApi3 (record)

```teal
global record RecipeWithApi3<T, T2, T3, Api>
```

A recipe with three parameters whose nodes offer an API.

- `__call : function(RecipeWithApi3<T, T2, T3, Api>, T, T2, T3) : TreeNodeId`  (call as `RecipeWithApi3(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(RecipeWithApi3<T, T2, T3, Api>, RefFill, T, T2, T3) : TreeNodeId`  (call as `RecipeWithApi3(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(RecipeWithApi3<T, T2, T3, Api>, ReactStyle, T, T2, T3) : TreeNodeId`  (call as `RecipeWithApi3(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.
- `__call : function(RecipeWithApi3<T, T2, T3, Api>, RefFill, ReactStyle, T, T2, T3) : TreeNodeId`  (call as `RecipeWithApi3(…)`): Creates a node; optional `React.ref(...)` and `React.style{...}` arguments go before the parameters.

## ExtensionPoint0 (record)

```teal
global record ExtensionPoint0
```

A named place in the GUI where other mods can add plugin recipes without parameters. Created with
`React.RegisterExtensionPoint0`.

- `id : string`: Full name, `"<modId>::<name>"`.

## ExtensionPoint (record)

```teal
global record ExtensionPoint<RecipeParam>
```

A named place in the GUI where plugin recipes with the parameter `RecipeParam` can be added.

- `id : string`: Full name, `"<modId>::<name>"`.

## WrapperExtensionPoint0 (record)

```teal
global record WrapperExtensionPoint0<WrappedRecipe>
```

An extension point whose plugins must return a node of `wrappedRecipe` (or nothing).

- `wrappedRecipe : WrappedRecipe`: The recipe every plugin must return.
- `id : string`: Full name, `"<modId>::<name>"`.

## WrapperExtensionPoint (record)

```teal
global record WrapperExtensionPoint<WrappedRecipe, RecipeParam>
```

An extension point with parameters whose plugins must return a node of `wrappedRecipe`.

- `wrappedRecipe : WrappedRecipe`: The recipe every plugin must return.
- `id : string`: Full name, `"<modId>::<name>"`.

## Meta (record)

```teal
global record Meta
```

Settings in the `meta` field of recipe parameters. `class` and `localKey` apply to the node itself;
the other fields wrap the node in a `Builtin.WithComponentParams` node.

- `localKey : string`: Key that keeps the node's identity (and state) between renders when its position among its siblings changes.
- `id : string`: Element id of the node, as used by `api.gui.byId` (e.g. window ids such as `"menu.pause"`).
- `tag : string`: Tag of the node; the context help looks up its help page by tag (e.g. `"menu.finances.charts"`).
- `tooltip : string`: Tooltip text.
- `tooltipModifierInputAction : string`: append this IA's key to the tooltip (regardless of what the key currently does)
- `tooltipSource : ReactRefWrap`: Node whose tooltip is shown for this node.
- `class : string`: Style class(es) of the node; overrides a `React.style{class = ...}` argument (with a warning).
- `forceFocusable : boolean`: Makes the node focusable even if it would not be.
- `disableFocusableRecursive : boolean`: Makes the node and all its children non-focusable.
- `enabled : boolean`: False disables the node.
- `styleSheet : StyleSheet`: Style sheet applied to the node.
- `onMouseOver : function(isHovered : boolean)`: Called when the mouse starts or stops hovering the node.
- `onFocusChange : function(hasFocus : boolean)`: Called when the node gains or loses focus.
- `onAttention : function(attention : boolean)`: Callback when "attention" changes.
With "attention" we refer to the node having focus (in gamepad mode) or being hovered with the mouse.
Note that this is not directly tied to :hover pseudo-class (you can have attention even with pseudo-class :active, but :hover would be ruled out by :active)
- `mouseCursorOverride : number`: Mouse cursor shown over the node, e.g. `gui_react_util.getMouseCursorInteger("Default")`.
- `discardNodeIdentity : boolean`: force the node to be fully regenerated (it will lose its state and its UI elements).

## ReactState (record)

```teal
global record ReactState
```

State created with `React.useState`. A change with `set` or `transform` renders the recipe again.
`old()` returns the value of the current render.

- `old : function(ReactState) : any`: Returns the value of the current render.
- `set : function(ReactState, any)`: Sets a new value; the recipe renders again with it.
- `transform : function(ReactState, function(any) : any)`: Sets the value to `fn(current value)`.
- `hasExpired : function(ReactState) : boolean`: True if the node that owns the state no longer exists (e.g. when called from a callback after unmount).

    ```lua
    -- from base/content/gui/gui/layers/layer_cargo.tl
    getSelectedCargoTypeId = function() : CargoTypeId
        if dropDownSelectionState:hasExpired() then
            return -1
        end
        return dropDownSelectionState:old().selectedCargoTypeId
    end,
    ```

## ReactStateT (record)

```teal
global record ReactStateT<T>
```

Typed state created with `React.useState(initialValue)`; see `ReactState`.

- `old : function(ReactStateT<T>) : T`  (call as `reactStateT:old(…) (method)`): Returns the value of the current render.
    - used 1593x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:199`: `local s = state:old()`
- `set : function(ReactStateT<T>, T)`  (call as `reactStateT:set(…) (method)`): Sets a new value; the recipe renders again with it.
    - used 667x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:227`: `state:set(s)`
- `transform : function(ReactStateT<T>, function(T) : T)`  (call as `reactStateT:transform(…) (method)`): Sets the value to `fn(current value)`.
    - used 41x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:154`: `state:transform(function(currentState : CelebrationState) : CelebrationState`
- `hasExpired : function(ReactStateT<T>) : boolean`  (call as `reactStateT:hasExpired(…) (method)`): True if the node that owns the state no longer exists.
    - used 9x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:921`: `if diagramWidgetState:hasExpired() then`

## ReactRefT (record)

```teal
global record ReactRefT<T>
```

A value kept between renders, created with `React.useRef`. Unlike a state, changing it does not
render the recipe again.

- `get : function(ReactRefT<T>) : T`  (call as `reactRefT:get(…) (method)`): Returns the stored value.
    - used 535x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:190`: `if calloutStartTimeRef:get() then`
- `set : function(ReactRefT<T>, T)`  (call as `reactRefT:set(…) (method)`): Stores a value without rendering again.
    - used 292x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:186`: `calloutStartTimeRef:set(nil)`
- `transform : function(ReactRefT, function(T) : T)`  (call as `reactRefT:transform(…) (method)`): Stores `fn(current value)`.
    - used 9x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:6640`: `depot2VehiclesCompareRef:transform(function(curr : {Engine.Entity : {Engine.Entity}}) : {Engine.Entity : {Engine.Entity}}`
- `hasExpired : function(ReactRefT<T>) : boolean`  (call as `reactRefT:hasExpired(…) (method)`): True if the node that owns the ref no longer exists.
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:1289`: `return tooltipHolder:hasExpired()`

## ReactRefWrap (record)

```teal
global record ReactRefWrap
```

A ref to a GUI node, from `React.useNodeRef()` or `React.useSelfRef()`.

- `get : function(ReactRefWrap) : ReactNodeRef`  (call as `reactRefWrap:get(…) (method)`): Returns the node, or nil before the node exists.
    - used 39x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_popups.tl:152`: `if notificationPopupRef and notificationPopupRef:get() ~= nil and not notificationPopupRef:hasExpired() then`
- `set : function(ReactRefWrap, any)`: Stores a node; `React.ref(...)` does this when the node is created.
- `transform : function(ReactRefWrap, function(any) : any)`: Stores `fn(current value)`.
- `hasExpired : function(ReactRefWrap) : boolean`  (call as `reactRefWrap:hasExpired(…) (method)`): True if the node that owns the ref no longer exists.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_popups.tl:152`: `if notificationPopupRef and notificationPopupRef:get() ~= nil and not notificationPopupRef:hasExpired() then`

## ReactNodeRefT (record)

```teal
global record ReactNodeRefT<T>
```

Access to a created node of a recipe with parameter type `T`.

- `focus : function(ReactNodeRefT<T>)`: Gives the node the focus.
- `containsPosition : function(ReactNodeRefT<T>, position : Vec2i) : boolean`: True if the screen position lies inside the node.
- `getPosition : function(ReactNodeRefT<T>, gravityX : number, gravityY : number) : Vec2f`: Screen position of a point of the node; `gravityX`/`gravityY` 0 is the left/top edge and 1 the right/bottom edge.

    ```lua
    -- width of a node (base/content/game_mechanics/game_mechanics/notifications/gui/notification_popups.tl)
    local popupWidth = notificationPopupRef:get():getPosition(1.0, 0.0).x - notificationPopupRef:get():getPosition(0.0, 0.0).x
    ```

- `ensureVisible : function(ReactNodeRefT<T>)`: Scrolls the surrounding scroll areas so the node is visible.
- `setSelectedRecursive : function(ReactNodeRefT<T>, selected : boolean)`: Sets the selected state of the node and its children.

## ReactNodeRefApiT (record)

```teal
global record ReactNodeRefApiT<T, Api>
```

Access to a created node of a recipe with an API.

- `getApi : function(ReactNodeRefApiT<T, Api>) : Api`: Returns the API the recipe set with `React.provideApi`.
- `focus : function(ReactNodeRefApiT<T, Api>)`: Gives the node the focus.
- `containsPosition : function(ReactNodeRefApiT<T, Api>, position : Vec2i) : boolean`: True if the screen position lies inside the node.
- `getPosition : function(ReactNodeRefApiT<T, Api>, gravityX : number, gravityY : number) : Vec2f`: Screen position of a point of the node; `gravityX`/`gravityY` 0 is the left/top edge and 1 the right/bottom edge.
- `getIdentity : function(ReactNodeRefApiT<T, Api>) : integer`: Identity number of the node; nil when the node does not exist (the base game checks it before `getApi()`).
- `ensureVisible : function(ReactNodeRefApiT<T, Api>)`: Scrolls the surrounding scroll areas so the node is visible.
- `setSelectedRecursive : function(ReactNodeRefApiT<T, Api>, selected : boolean)`: Sets the selected state of the node and its children.

## ReactNodeRefApi0 (record)

```teal
global record ReactNodeRefApi0<Api>
```

Access to a created node of a parameterless recipe with an API.

- `getApi : function(ReactNodeRefApi0<Api>) : Api`: Returns the API the recipe set with `React.provideApi`.
- `focus : function(ReactNodeRefApi0<Api>)`: Gives the node the focus.
- `containsPosition : function(ReactNodeRefApi0<Api>, position : Vec2i) : boolean`: True if the screen position lies inside the node.
- `getPosition : function(ReactNodeRefApi0<Api>, gravityX : number, gravityY : number) : Vec2f`: Screen position of a point of the node; `gravityX`/`gravityY` 0 is the left/top edge and 1 the right/bottom edge.
- `getIdentity : function(ReactNodeRefApi0<Api>) : integer`: Identity number of the node.
- `ensureVisible : function(ReactNodeRefApi0<Api>)`: Scrolls the surrounding scroll areas so the node is visible.
- `setSelectedRecursive : function(ReactNodeRefApi0<Api>, selected : boolean)`: Sets the selected state of the node and its children.

## ReactRefWrapT (record)

```teal
global record ReactRefWrapT<T>
```

Typed node ref from `React.useNodeRef(SomeRecipe)`.

- `get : function(ReactRefWrapT<T>) : ReactNodeRefT<T>`  (call as `reactRefWrapT:get(…) (method)`): Returns the node, or nil before the node exists.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:2416`: `categoryFilterPanelRef:get():focus()`
- `set : function(ReactRefWrapT<T>, ReactNodeRefT<T>)`: Stores a node; `React.ref(...)` does this when the node is created.
- `transform : function(ReactRefWrapT<T>, function(ReactNodeRefT<T>) : ReactNodeRefT<T>)`: Stores `fn(current value)`.
- `hasExpired : function(ReactRefWrapT<T>) : boolean`: True if the node that owns the ref no longer exists.

## ReactRefWrapApiT (record)

```teal
global record ReactRefWrapApiT<T, Api>
```

Node ref from `React.useNodeRef(SomeRecipeWithApi)`; its node has `getApi()`.

- `get : function(ReactRefWrapApiT<T, Api>) : ReactNodeRefApiT<T, Api>`  (call as `reactRefWrapApiT:get(…) (method)`): Returns the node, or nil before the node exists.
    - used 86x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:1503`: `refSelf:get():focus()`
- `set : function(ReactRefWrapApiT<T, Api>, ReactNodeRefApiT<T, Api>)`: Stores a node.
- `transform : function(ReactRefWrapApiT<T, Api>, function(ReactNodeRefApiT<T, Api>) : ReactNodeRefApiT<T, Api>)`: Stores `fn(current value)`.
- `hasExpired : function(ReactRefWrapApiT<T, Api>) : boolean`  (call as `reactRefWrapApiT:hasExpired(…) (method)`): True if the node that owns the ref no longer exists.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:3619`: `if (false == refSublistParams:hasExpired()) and refSublistParams:get() ~= nil and refSublistParams:get():getIdentity() ~= nil and refSublistParams:get():getApi(`

## ReactRefWrapApi0 (record)

```teal
global record ReactRefWrapApi0<Api>
```

Node ref for a parameterless recipe with an API.

- `get : function(ReactRefWrapApi0<Api>) : ReactNodeRefApi0<Api>`  (call as `reactRefWrapApi0:get(…) (method)`): Returns the node, or nil before the node exists.
    - used 6x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1937`: `if galleryRef:get() == nil or not params.logoImage or params.logoImage == "" then`
- `set : function(ReactRefWrapApi0<Api>, ReactNodeRefApi0<Api>)`: Stores a node.
- `transform : function(ReactRefWrapApi0<Api>, function(ReactNodeRefApi0<Api>) : ReactNodeRefApi0<Api>)`: Stores `fn(current value)`.
- `hasExpired : function(ReactRefWrapApi0<Api>) : boolean`: True if the node that owns the ref no longer exists.

## ReactNodeRef (record)

```teal
global record ReactNodeRef
```

Access to a created node, from an untyped `ReactRefWrap`.

- `focus : function(ReactNodeRef)`: Gives the node the focus.
- `containsPosition : function(ReactNodeRef, position : Vec2i) : boolean`: True if the screen position lies inside the node.
- `getPosition : function(ReactNodeRef, gravityX : number, gravityY : number) : Vec2f`: Screen position of a point of the node; `gravityX`/`gravityY` 0 is the left/top edge and 1 the right/bottom edge.
- `getIdentity : function(ReactNodeRef) : integer`: Identity number of the node.

## ReactStyleParams (record)

```teal
global record ReactStyleParams
```

Parameters of `React.style`.

- `class : string`: Style class(es) for the node.

## IReactToolDefinition (interface)

```teal
global interface IReactToolDefinition
```

A tool registered with `React.RegisterTool` (the tool stack of the game GUI, e.g. windows such as the pause menu).

- `name : string`: Unique tool name; `RegisterTool` fails if it is missing or already taken.

## ReactToolDefinition (record)

```teal
global record ReactToolDefinition<P> is IReactToolDefinition
```

Callbacks of a tool on the tool stack, with tool parameters `P`. `tool_react_util` builds the usual
window tools with `prepareDefaultToolWithWindowDefinition`.

- `push : function(Builtin.ToolStackContext<P>, P)`: Called when the tool is pushed onto the tool stack; window tools open their window here.
- `pop : function(Builtin.ToolStackContext<P>, P)`: Called when the tool is removed from the stack; window tools close their window here.
- `shelve : function(Builtin.ToolStackContext<P>, P, shelve : boolean)`: Called with `true` when another tool covers this one and with `false` when it is on top again; window tools hide and show their window.
- `getActiveVariant : function(Builtin.ToolStackContext<P>, P) : string`: Optional; name of the tool's active variant.
- `getDebugString : function(Builtin.ToolStackContext<P>, P) : string`: Optional; text for debugging output.

## DndGrabContext (record)

```teal
global record DndGrabContext
```

Context for adding drag-and-drop items.

- `addItem : function(itemType : ResName, itemData : any)`: Adds an item of the given type to the dragged items.

## DndDropContext (record)

```teal
global record DndDropContext
```

Items available when something is dropped on a node.

- `getItems : function() : {Gui.DataTransferItem}`: get filtered items (if filter is set), otherwise same as getAllItems
- `getAllItems : function() : {Gui.DataTransferItem}`: All dragged items.
- `anyMatch : function() : boolean`: True if at least one item passes the handler's `filter`.

## DndDragContext (record)

```teal
global record DndDragContext
```

Items being dragged over a node (given to `enter` and `exit`).

- `getItems : function() : {Gui.DataTransferItem}`: get filtered items
- `getAllItems : function() : {Gui.DataTransferItem}`: get all items that are currently being dragged
- `anyMatch : function() : boolean`: check if at least one item matches the filter if filter

## DragAndDropHandler (record)

```teal
global record DragAndDropHandler
```

Callbacks given to `React.onDragAndDrop`.

- `grab : function() : {Gui.DataTransferItem}, integer`: return what you started to drag, {} to deny grabbing, nil/false to let parent handle it.
second return value: initial dragging mouse cursor (defaults to gui_react_util.getMouseCursorInteger("Grab"))
- `filter : function(type : ResName, data : any) : boolean`: optional filter, return true if an item should be accepted. If not set, all items are accepted.
- `drop : function(dropContext : DndDropContext) : boolean`: Called when items are dropped on the node; return true if handled.
- `enter : function(dropContext : DndDragContext) : boolean`: Called when dragged items enter the node; return true if handled.
- `exit : function(dropContext : DndDragContext) : boolean`: Called when dragged items leave the node; return true if handled.

## React (record)

```teal
global record React
```

The GUI library returned by `ug_require "::/gui/main/react.lua"`.

- `RegisterRecipe : function(string, function() : TreeNodeId) : Recipe0`: Registers a recipe under a name and returns it; calling the result creates nodes.
    - param `#1`: Recipe name, used for debugging and recipe ids.
    - param `#2`: The render function; it returns a node, a list of nodes or nil.
    - notes: Register recipes once at file level, not inside other recipes. Hooks (`useState`, `onStep`, ...) work only while the render function runs.
- `RegisterRecipe : function<T1>(string, function(T1) : TreeNodeId) : Recipe<T1>`: Registers a recipe under a name and returns it; calling the result creates nodes.
    - param `#1`: Recipe name, used for debugging and recipe ids.
    - param `#2`: The render function; it returns a node, a list of nodes or nil.
    - notes: Register recipes once at file level, not inside other recipes. Hooks (`useState`, `onStep`, ...) work only while the render function runs.
- `RegisterRecipe : function<T1, T2>(string, function(T1, T2) : TreeNodeId) : Recipe2<T1, T2>`: Registers a recipe under a name and returns it; calling the result creates nodes.
    - param `#1`: Recipe name, used for debugging and recipe ids.
    - param `#2`: The render function; it returns a node, a list of nodes or nil.
    - notes: Register recipes once at file level, not inside other recipes. Hooks (`useState`, `onStep`, ...) work only while the render function runs.
- `RegisterRecipe : function<T1, T2, T3>(string, function(T1, T2, T3) : TreeNodeId) : Recipe3<T1, T2, T3>`: Registers a recipe under a name and returns it; calling the result creates nodes.
    - param `#1`: Recipe name, used for debugging and recipe ids.
    - param `#2`: The render function; it returns a node, a list of nodes or nil.
    - notes: Register recipes once at file level, not inside other recipes. Hooks (`useState`, `onStep`, ...) work only while the render function runs.
- `RegisterRecipe : function<T1, T2, T3, T4>(string, function(T1, T2, T3, T4) : TreeNodeId) : Recipe4<T1, T2, T3, T4>`: Registers a recipe under a name and returns it; calling the result creates nodes.
    - param `#1`: Recipe name, used for debugging and recipe ids.
    - param `#2`: The render function; it returns a node, a list of nodes or nil.
    - notes: Register recipes once at file level, not inside other recipes. Hooks (`useState`, `onStep`, ...) work only while the render function runs.
- `RegisterWrapperRecipe : function<T, Params, WrappedParams>(string, Recipe<WrappedParams>, (function(T, Params) : TreeNodeId)) : Recipe<T>`: Registers a recipe that must return exactly one node of the wrapped recipe (or none), e.g. a window recipe built on `builtin.Window`.

    ```lua
    -- from base/content/gui/gui/main/pause_menu.tl
    local PauseMenu = react.RegisterWrapperRecipe("PauseMenu", builtin.Window, function(params : ToolReactUtil.IToolWindowParam<ToolReactUtil.IToolParam>) : TreeNodeId
        return builtin.Window{ id = pauseMenuWindowId, closable = false, content = PauseMenuContent(params) }
    end)
    ```

register a recipe that is a plugin for a specific extensionPoint
- `RegisterPluginRecipe : function(extensionPoint : ExtensionPoint0, name : string, fn : function() : TreeNodeId) : Recipe0`: Registers a recipe as a plugin for an extension point.
    - notes: Registering the recipe does not add it to the extension point. The mod also needs a generic resource (`.res.lua`) of type `"react-plugin <extension point id>"` whose `data.filePath` names the recipe as `"file.script@RecipeName"`. For a wrapper extension point the recipe must return a node of the wrapped recipe.

    ```lua
    -- base/content/gui/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.res.lua
    function data()
        return {
            type = "react-plugin ::GameBarInfoDisplayExtension",
            data = {
                filePath = "::/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.script@GameBarEarningsPlugin",
            }
        }
    end
    ```

- `RegisterPluginRecipe : function<ParamType>(extensionPoint : ExtensionPoint<ParamType>, name : string, fn : function(ParamType) : TreeNodeId) : Recipe<ParamType>`: Registers a recipe as a plugin for an extension point.
    - notes: Registering the recipe does not add it to the extension point. The mod also needs a generic resource (`.res.lua`) of type `"react-plugin <extension point id>"` whose `data.filePath` names the recipe as `"file.script@RecipeName"`. For a wrapper extension point the recipe must return a node of the wrapped recipe.

    ```lua
    -- base/content/gui/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.res.lua
    function data()
        return {
            type = "react-plugin ::GameBarInfoDisplayExtension",
            data = {
                filePath = "::/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.script@GameBarEarningsPlugin",
            }
        }
    end
    ```

- `RegisterPluginRecipe : function<WrappedRecipe>(extensionPoint : WrapperExtensionPoint0<WrappedRecipe>, name : string, fn : function() : TreeNodeId) : Recipe0`: Registers a recipe as a plugin for an extension point.
    - notes: Registering the recipe does not add it to the extension point. The mod also needs a generic resource (`.res.lua`) of type `"react-plugin <extension point id>"` whose `data.filePath` names the recipe as `"file.script@RecipeName"`. For a wrapper extension point the recipe must return a node of the wrapped recipe.

    ```lua
    -- base/content/gui/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.res.lua
    function data()
        return {
            type = "react-plugin ::GameBarInfoDisplayExtension",
            data = {
                filePath = "::/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.script@GameBarEarningsPlugin",
            }
        }
    end
    ```

- `RegisterPluginRecipe : function<WrappedRecipe, ParamType>(extensionPoint : WrapperExtensionPoint<WrappedRecipe, ParamType>, name : string, fn : function(ParamType) : TreeNodeId) : Recipe<ParamType>`: Registers a recipe as a plugin for an extension point.
    - notes: Registering the recipe does not add it to the extension point. The mod also needs a generic resource (`.res.lua`) of type `"react-plugin <extension point id>"` whose `data.filePath` names the recipe as `"file.script@RecipeName"`. For a wrapper extension point the recipe must return a node of the wrapped recipe.

    ```lua
    -- base/content/gui/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.res.lua
    function data()
        return {
            type = "react-plugin ::GameBarInfoDisplayExtension",
            data = {
                filePath = "::/gui/game_bar/game_bar_display_earnings_plugin/game_bar_display_earnings.script@GameBarEarningsPlugin",
            }
        }
    end
    ```

register an extensionPoint (its name must be unique, otherwise the call returns nil)
- `RegisterExtensionPoint0 : function(modId : string, extensionPointName : string) : ExtensionPoint0`: Creates an extension point without parameters under `"<modId>::<name>"`.
    - returns: The extension point, or nil (with a warning) if the name is already taken.

    ```lua
    -- from base/content/gui/gui/main/main_mod_button_area.tl
    main_mod_button_area.MainModButtonAreaExtension = react.RegisterExtensionPoint0(getOwningModId(), "MainModButtonAreaExtension")
    -- inside the recipe:
    local children = gui_react_util.usePlugins(main_mod_button_area.MainModButtonAreaExtension)
    ```

- `RegisterExtensionPoint : function<RecipeParam>(modId : string, extensionPointName : string) : ExtensionPoint<RecipeParam>`: Creates an extension point whose plugins take a parameter, under `"<modId>::<name>"`.
    - returns: The extension point, or nil (with a warning) if the name is already taken.
- `RegisterWrapperExtensionPoint0 : function<WrappedRecipe, RecipeParam>(modId : string, extensionPointName : string, wrappedRecipe : WrappedRecipe) : WrapperExtensionPoint<WrappedRecipe, RecipeParam>`: Creates an extension point whose plugins must return a node of `wrappedRecipe`.
    - returns: The extension point, or nil if the name is already taken.
- `RegisterWrapperExtensionPoint : function<WrappedRecipe, RecipeParam>(modId : string, extensionPointName : string, wrappedRecipe : WrappedRecipe) : WrapperExtensionPoint<WrappedRecipe, RecipeParam>`: Creates an extension point with parameters whose plugins must return a node of `wrappedRecipe`.
    - returns: The extension point, or nil if the name is already taken.

get all registered plugins for the extensionPoint (returns {} if extensionPoint is nil or otherwise unknown)
- `getPlugins : function(extensionPoint? : ExtensionPoint0) : {ReactPlugin0}`: Loads the plugins of an extension point from all `"react-plugin <id>"` resources, sorted by `order`.
    - returns: The plugins with recipe, order and condition; `{}` for nil or unknown extension points.
    - notes: `gui_react_util.usePlugins` calls this, evaluates the conditions and creates the plugin nodes.
- `getPlugins : function<RecipeParam>(extensionPoint? : ExtensionPoint<RecipeParam>) : {ReactPlugin<RecipeParam>}`: Loads the plugins of an extension point from all `"react-plugin <id>"` resources, sorted by `order`.
    - returns: The plugins with recipe, order and condition; `{}` for nil or unknown extension points.
    - notes: `gui_react_util.usePlugins` calls this, evaluates the conditions and creates the plugin nodes.
- `getPlugins : function<WrappedRecipe>(extensionPoint? : WrapperExtensionPoint0<WrappedRecipe>) : {ReactPlugin0}`: Loads the plugins of an extension point from all `"react-plugin <id>"` resources, sorted by `order`.
    - returns: The plugins with recipe, order and condition; `{}` for nil or unknown extension points.
    - notes: `gui_react_util.usePlugins` calls this, evaluates the conditions and creates the plugin nodes.
- `getPlugins : function<WrappedRecipe, RecipeParam>(extensionPoint? : WrapperExtensionPoint<WrappedRecipe, RecipeParam>) : {ReactPlugin<RecipeParam>}`: Loads the plugins of an extension point from all `"react-plugin <id>"` resources, sorted by `order`.
    - returns: The plugins with recipe, order and condition; `{}` for nil or unknown extension points.
    - notes: `gui_react_util.usePlugins` calls this, evaluates the conditions and creates the plugin nodes.

Call the original recipe, ignoring replacement configurations. Note that this will *not* work for builtins.
- `CallOriginalRecipe : function(possiblyReplaced : Recipe0) : TreeNodeId`: Calls the original recipe even if a replacement is registered for it; use it inside the replacement.
    - notes: Not allowed for builtins (logs an error and returns nil).

    ```lua
    -- from mods/release/urbangames_campaign_mission_05/content/mission/mission/perk_icons_replacement.script.tl
    local PerkHudIconRepalcement = react.RegisterRecipe("PerkHudIcon", function(param : HudIconToolbox.PerkHudIconParam) : TreeNodeId
        if game_react_globals.getDisableFeatures()["PerkHudIcons"] then
            return nil
        end
        return builtin.BoxLayout{ children = { react.CallOriginalRecipe(hud_icon_toolbox.PerkHudIcon, param) } }
    end)
    ```

- `CallOriginalRecipe : function<Api>(possiblyReplaced : RecipeWithApi0<Api>) : TreeNodeId`: Calls the original recipe even if a replacement is registered for it; use it inside the replacement.
    - notes: Not allowed for builtins (logs an error and returns nil).

    ```lua
    -- from mods/release/urbangames_campaign_mission_05/content/mission/mission/perk_icons_replacement.script.tl
    local PerkHudIconRepalcement = react.RegisterRecipe("PerkHudIcon", function(param : HudIconToolbox.PerkHudIconParam) : TreeNodeId
        if game_react_globals.getDisableFeatures()["PerkHudIcons"] then
            return nil
        end
        return builtin.BoxLayout{ children = { react.CallOriginalRecipe(hud_icon_toolbox.PerkHudIcon, param) } }
    end)
    ```

- `CallOriginalRecipe : function<RecipeParam>(possiblyReplaced : Recipe<RecipeParam>, params : RecipeParam) : TreeNodeId`: Calls the original recipe even if a replacement is registered for it; use it inside the replacement.
    - notes: Not allowed for builtins (logs an error and returns nil).

    ```lua
    -- from mods/release/urbangames_campaign_mission_05/content/mission/mission/perk_icons_replacement.script.tl
    local PerkHudIconRepalcement = react.RegisterRecipe("PerkHudIcon", function(param : HudIconToolbox.PerkHudIconParam) : TreeNodeId
        if game_react_globals.getDisableFeatures()["PerkHudIcons"] then
            return nil
        end
        return builtin.BoxLayout{ children = { react.CallOriginalRecipe(hud_icon_toolbox.PerkHudIcon, param) } }
    end)
    ```

- `CallOriginalRecipe : function<RecipeParam, Api>(possiblyReplaced : RecipeWithApi<RecipeParam, Api>, params : RecipeParam) : TreeNodeId`: Calls the original recipe even if a replacement is registered for it; use it inside the replacement.
    - notes: Not allowed for builtins (logs an error and returns nil).

    ```lua
    -- from mods/release/urbangames_campaign_mission_05/content/mission/mission/perk_icons_replacement.script.tl
    local PerkHudIconRepalcement = react.RegisterRecipe("PerkHudIcon", function(param : HudIconToolbox.PerkHudIconParam) : TreeNodeId
        if game_react_globals.getDisableFeatures()["PerkHudIcons"] then
            return nil
        end
        return builtin.BoxLayout{ children = { react.CallOriginalRecipe(hud_icon_toolbox.PerkHudIcon, param) } }
    end)
    ```

- `CallOriginalRecipe : function<RecipeParam, UserParam>(possiblyReplaced : Recipe2<RecipeParam, UserParam>, params : RecipeParam, userParam : UserParam) : TreeNodeId`: Calls the original recipe even if a replacement is registered for it; use it inside the replacement.
    - notes: Not allowed for builtins (logs an error and returns nil).

    ```lua
    -- from mods/release/urbangames_campaign_mission_05/content/mission/mission/perk_icons_replacement.script.tl
    local PerkHudIconRepalcement = react.RegisterRecipe("PerkHudIcon", function(param : HudIconToolbox.PerkHudIconParam) : TreeNodeId
        if game_react_globals.getDisableFeatures()["PerkHudIcons"] then
            return nil
        end
        return builtin.BoxLayout{ children = { react.CallOriginalRecipe(hud_icon_toolbox.PerkHudIcon, param) } }
    end)
    ```

- `RegisterTool : function<P>(toolDef : ReactToolDefinition<P>) : ReactToolDefinition<P>`: Registers a tool definition for the tool stack.
    - returns: The same definition.
    - notes: Raises an error if the definition has no `name` or the name is already registered.
- `GetRecipeId : function(IRecipe) : number`: Internal id of a registered recipe.
- `GetRecipeName : function<T>(Recipe<T>) : string`: for debugging, as name is not necessarily unique

Name a recipe was registered with (for debugging; names need not be unique).
- `useState : function(?any) : ReactState`: Creates a state for the current node; the first render uses the initial value.

    ```lua
    local focusableState = react.useState(false)
    react.setDisableFocusable(not focusableState:old())
    ```

- `useState : function<T>(?T) : ReactStateT<T>`: Creates a state for the current node; the first render uses the initial value.

    ```lua
    local focusableState = react.useState(false)
    react.setDisableFocusable(not focusableState:old())
    ```

- `useStateLazy : function<T>(function() : T) : ReactStateT<T>`: Like `useState`, but the initial value comes from a function called only on the first render.

Create a State that is automatically updated when the source changes.
Otherwise, you might need to either read the ref in onStep, or make it a full-fledged State to force child updates.
- `useMirrorState : function<T>(sourceRef : ReactRefT<T>) : ReactStateT<T>`: Creates a state that follows a source ref or state and renders the recipe again when the source changes.
- `useMirrorState : function<T>(sourceState : ReactStateT<T>) : ReactStateT<T>`: Creates a state that follows a source ref or state and renders the recipe again when the source changes.
- `useDependentState : function<D, S>(sourceRef : ReactRefT<S>, transform : function(curDependent : D, source : S) : D) : ReactStateT<D>`: Creates a state that is recomputed with `transform(current, source)` whenever the source ref or state changes.
- `useDependentState : function<D, S>(sourceState : ReactStateT<S>, transform : function(curDependent : D, source : S) : D) : ReactStateT<D>`: Creates a state that is recomputed with `transform(current, source)` whenever the source ref or state changes.
- `onStep : function(function())`: Registers a function called on every GUI step while the node exists.
- `onStepTimer : function(function(), intervalSeconds? : number, jitter? : boolean)`: jitter: true by default

Registers a function called at an interval (default 0.5 seconds); the call is deferred to after rendering.
    - param `intervalSeconds`: Interval in seconds.
    - param `jitter`: Spread the calls of many nodes over time; true by default.

Act if step mode has changed (due to writing into useChildPauseModes()'s result)
Normally only used by builtins, e.g. builtin.TabWidgetChild to pause steps of invisible tab content
- `onPausedChange : function(function(paused : boolean))`: note: current implementation calls this at least once even if mode did not change

Registers a function called when the node's step mode is paused or resumed (used by builtins such as tab widgets).
- `useChildPausedModes : function(paused : {integer : boolean}) : ReactRefWrapT<{integer : boolean}>`: Create a ref where you can store map childIndex->childPaused

Creates a ref that maps child index to paused, to pause the steps of hidden children.
- `onEvent : function(filterForEventName : string, function(string, any))`: Registers a handler for events with the given name sent with `React.fireEvent`.
    - param `filterForEventName`: Event name; must be the first argument.

    ```lua
    react.onEvent("focusMainModButtonArea", function()
        focusableState:set(true)
        selfRef:get():focus()
    end)
    ```

- `useSelfRef : function() : ReactRefWrap`: Returns a node ref to the node of the current recipe.
- `useRef : function<T>(?T) : ReactRefT<T>`: Creates a ref (a value kept between renders) with an initial value.
- `useRefLazy : function<T>(function() : T) : ReactRefT<T>`: Like `useRef`, but the initial value comes from a function called only on the first render.
- `useMirrorRef : function<T>(ReactStateT<T>) : ReactRefT<T>`: Creates a ref that follows a source state or ref.
- `useMirrorRef : function<T>(ReactRefT<T>) : ReactRefT<T>`: Creates a ref that follows a source state or ref.
- `useDependentRef : function<D, S>(sourceRef : ReactRefT<S>, transform : function(curDependent : D, source : S) : D) : ReactRefT<D>`: Creates a ref that is recomputed with `transform(current, source)` whenever the source changes.
- `useDependentRef : function<D, S>(sourceState : ReactStateT<S>, transform : function(curDependent : D, source : S) : D) : ReactRefT<D>`: Creates a ref that is recomputed with `transform(current, source)` whenever the source changes.
- `useNodeRef : function() : ReactRefWrap`: Creates an empty node ref; pass it with `React.ref(...)` to a recipe call to get access to the created node.
    - notes: The recipe argument only selects the ref's type; the implementation ignores it.

    ```lua
    -- from base/content/game_mechanics/game_mechanics/notifications/gui/notification_button.tl
    local toolButtonRef = react.useNodeRef(builtin.ToolButton)
    react.useInputAction("selectNotifications", react.iaForward(toolButtonRef, "ACTION_CLICK"))
    return builtin.ToolButton(react.ref(toolButtonRef), { ... })
    ```

- `useNodeRef : function<T>(Recipe<T>) : ReactRefWrapT<T>`: Creates an empty node ref; pass it with `React.ref(...)` to a recipe call to get access to the created node.
    - notes: The recipe argument only selects the ref's type; the implementation ignores it.

    ```lua
    -- from base/content/game_mechanics/game_mechanics/notifications/gui/notification_button.tl
    local toolButtonRef = react.useNodeRef(builtin.ToolButton)
    react.useInputAction("selectNotifications", react.iaForward(toolButtonRef, "ACTION_CLICK"))
    return builtin.ToolButton(react.ref(toolButtonRef), { ... })
    ```

- `useNodeRef : function<T, Api>(RecipeWithApi<T, Api>) : ReactRefWrapApiT<T, Api>`: Creates an empty node ref; pass it with `React.ref(...)` to a recipe call to get access to the created node.
    - notes: The recipe argument only selects the ref's type; the implementation ignores it.

    ```lua
    -- from base/content/game_mechanics/game_mechanics/notifications/gui/notification_button.tl
    local toolButtonRef = react.useNodeRef(builtin.ToolButton)
    react.useInputAction("selectNotifications", react.iaForward(toolButtonRef, "ACTION_CLICK"))
    return builtin.ToolButton(react.ref(toolButtonRef), { ... })
    ```

- `useNodeRef : function<Api>(RecipeWithApi0<Api>) : ReactRefWrapApi0<Api>`: Creates an empty node ref; pass it with `React.ref(...)` to a recipe call to get access to the created node.
    - notes: The recipe argument only selects the ref's type; the implementation ignores it.

    ```lua
    -- from base/content/game_mechanics/game_mechanics/notifications/gui/notification_button.tl
    local toolButtonRef = react.useNodeRef(builtin.ToolButton)
    react.useInputAction("selectNotifications", react.iaForward(toolButtonRef, "ACTION_CLICK"))
    return builtin.ToolButton(react.ref(toolButtonRef), { ... })
    ```

- `ref : function<T, Api>(ReactRefWrapApiT<T, Api>) : RefFill`: Wraps one node ref (or a list of them) for passing as the first argument of a recipe call; the refs receive the created node.
- `ref : function<T>(ReactRefWrapT<T>) : RefFill`: Wraps one node ref (or a list of them) for passing as the first argument of a recipe call; the refs receive the created node.
- `ref : function<Api>(ReactRefWrapApi0<Api>) : RefFill`: Wraps one node ref (or a list of them) for passing as the first argument of a recipe call; the refs receive the created node.
- `ref : function(ReactRefWrap) : RefFill`: Wraps one node ref (or a list of them) for passing as the first argument of a recipe call; the refs receive the created node.
- `ref : function<T, Api>({ReactRefWrapApiT<T, Api>}) : RefFill`: Wraps one node ref (or a list of them) for passing as the first argument of a recipe call; the refs receive the created node.
- `ref : function<T>({ReactRefWrapT<T>}) : RefFill`: Wraps one node ref (or a list of them) for passing as the first argument of a recipe call; the refs receive the created node.
- `ref : function<Api>({ReactRefWrapApi0<Api>}) : RefFill`: Wraps one node ref (or a list of them) for passing as the first argument of a recipe call; the refs receive the created node.
- `ref : function({ReactRefWrap}) : RefFill`: Wraps one node ref (or a list of them) for passing as the first argument of a recipe call; the refs receive the created node.
- `findOldChildByLocalKey : function(string) : TreeNodeId`: Returns the child node of the previous render that had the given `localKey`.
- `onMount : function(function(any))`: Registers a function called when the node has been created.
- `onUnmount : function(function(any))`: Registers a function called when the node is removed.
- `onMouseEvent : function(function(Gui.Mouse.Event) : boolean)`: Registers a mouse event listener for the node; return true if the event was handled.
- `onDragAndDrop : function(handlers : DragAndDropHandler)`: Makes the node a drag source and/or drop target with the given handlers.
- `style : function(ReactStyleParams) : ReactStyle`: Wraps style parameters for passing before the parameters of a recipe call; the base game sets classes with `meta = { class = ... }` instead.
- `useInputAction : function(string, IaHandler)`: Binds an input action (e.g. `"IA_MENU_BACK"`) of the current node to a handler, a focus traversal or a forward.

    ```lua
    react.useInputAction("IA_MENU_BACK", react.iaHandler(function()
        params.onClose()
    end))
    react.useInputAction("IA_NAV_LEFT", react.iaFocusTraversal(true, false))
    ```

- `useInputAction : function(string, IaForward)`: Binds an input action (e.g. `"IA_MENU_BACK"`) of the current node to a handler, a focus traversal or a forward.

    ```lua
    react.useInputAction("IA_MENU_BACK", react.iaHandler(function()
        params.onClose()
    end))
    react.useInputAction("IA_NAV_LEFT", react.iaFocusTraversal(true, false))
    ```

- `iaHandler : function(action : function(), isEnabled? : function() : (boolean | Gui.InputAction.InputActionState), buttonPromptTextOverride? : string, acceptRepeatedPresses? : boolean, isActive? : boolean) : IaHandler`: Creates an input action handler.
    - param `action`: Called when the action fires.
    - param `isEnabled`: Optional; returns a boolean or an input action state.
    - param `buttonPromptTextOverride`: Optional text for the button prompt.
    - param `acceptRepeatedPresses`: If true, the action repeats while the key is held.
    - param `isActive`: Optional active flag.
- `iaHandlerExtended : function(action : function(Gui.InputAction.InvokeData), isEnabled? : function() : (boolean | Gui.InputAction.InputActionState), buttonPromptTextOverride? : string, isActive? : boolean) : IaHandler`: Like `iaHandler`, but the action receives the invoke data for every event, including key release.
    - notes: The implementation tests an undefined variable before applying `buttonPromptTextOverride`, so the override has no effect.
- `iaForward : function(ReactRefWrap, ?string) : IaForward`: Forwards the input action to another node, optionally as a different action (e.g. `"ACTION_CLICK"`).
- `iaForward : function<T>(ReactRefWrapT<T>, ?string) : IaForward`: Forwards the input action to another node, optionally as a different action (e.g. `"ACTION_CLICK"`).
- `iaForward : function<T, Api>(ReactRefWrapApiT<T, Api>, ?string) : IaForward`: Forwards the input action to another node, optionally as a different action (e.g. `"ACTION_CLICK"`).
- `iaForward : function<Api>(ReactRefWrapApi0<Api>, ?string) : IaForward`: Forwards the input action to another node, optionally as a different action (e.g. `"ACTION_CLICK"`).
- `provideApi : function(any)`: Sets the API table of the current node; other recipes reach it with `nodeRef:get():getApi()`.

    ```lua
    -- from base/content/gui/gui/map_editor/map_editor.tl
    local wc = param.gameCtx.windowContainer:get():getApi()
    ```

Deprecated, will be replaced by ToolStack. Returns selfRef that can be passed in setActiveOwner()
useAction and setActiveOwner are for internal use only!
- `useAction : function(rcRef : ReactRefWrapT<Builtin.RendererComponentParam>, actionFn? : (function() : TreeNodeId), key2? : string) : ReactRefWrap`: Binds an action function to a renderer component; deprecated in favour of the tool stack.
    - returns: Self ref to pass to `setActiveOwner`.
- `useAction : function(rcRef : ReactRefWrapApiT<Builtin.RendererComponentParam, Builtin.RenderComponentAPI>, actionFn? : (function() : TreeNodeId), key2? : string) : ReactRefWrap`: Binds an action function to a renderer component; deprecated in favour of the tool stack.
    - returns: Self ref to pass to `setActiveOwner`.
- `setActiveOwner : function(actionUser : ReactRefWrap)`: Internal; makes the action of `useAction` active.
- `setDisableFocusable : function(boolean)`: Makes the current node non-focusable.
- `setForceFocusable : function(boolean)`: Makes the current node focusable.
- `setIndicateSelectionWithMouseHoverOnly : function(boolean)`: Shows the selection highlight of the current node only on mouse hover.
- `setRestrictMouseFocus : function(boolean)`: Sets the restrict-mouse-focus flag of the current node.
- `setMouseTransparent : function(boolean)`: Lets mouse events pass through the current node.
- `setLightweight : function(boolean)`: Marks the current node as lightweight.
    - notes: The implementation always sets the flag to true, whatever the argument.
- `setClipContent : function(boolean)`: use with care: special animation cases only, otherwise it might mess with the "natural" gui behaviour
- `setStyleClasses : function(string, string...)`: Sets the style classes of the current node (joined with commas).
- `setPreferredFocusChild : function<T, Api>(ReactRefWrapApiT<T, Api>)`: Sets the child that gets the focus when the current node receives it.
- `setPreferredFocusChild : function<T>(ReactRefWrapT<T>)`: Sets the child that gets the focus when the current node receives it.
- `setPreferredFocusChild : function<Api>(ReactRefWrapApi0<Api>)`: Sets the child that gets the focus when the current node receives it.
- `setPreferredFocusChild : function(ReactRefWrap)`: Sets the child that gets the focus when the current node receives it.
- `iaFocusTraversal : function(horizontal : boolean, forward : boolean, allowBubbleUp? : boolean) : IaHandler`: Creates an input action that moves the focus to the next or previous node.
    - param `horizontal`: Move horizontally (true) or vertically (false).
    - param `forward`: Move right/down (true) or left/up (false).
    - param `allowBubbleUp`: Let the parent handle the action at the end of the list.
- `fireEvent : function(source : ReactRefWrap, name : string, param? : any)`: Sends an event with a name and an optional parameter; recipes receive it with `React.onEvent`.
    - param `source`: Node the event starts from; the base game passes nil for GUI-wide events such as `"PushTool"`.

    ```lua
    -- from base/content/gui/gui/layers/layer_terrain.tl
    react.fireEvent(param.toolParam.gameCtx.rendererComponent, "selectEntity", {entity = entity, stack = true})
    ```

- `fireEvent : function<T>(source : ReactRefWrapT<T>, name : string, param? : any)`: Sends an event with a name and an optional parameter; recipes receive it with `React.onEvent`.
    - param `source`: Node the event starts from; the base game passes nil for GUI-wide events such as `"PushTool"`.

    ```lua
    -- from base/content/gui/gui/layers/layer_terrain.tl
    react.fireEvent(param.toolParam.gameCtx.rendererComponent, "selectEntity", {entity = entity, stack = true})
    ```

- `fireEvent : function<T, Api>(source : ReactRefWrapApiT<T, Api>, name : string, param? : any)`: Sends an event with a name and an optional parameter; recipes receive it with `React.onEvent`.
    - param `source`: Node the event starts from; the base game passes nil for GUI-wide events such as `"PushTool"`.

    ```lua
    -- from base/content/gui/gui/layers/layer_terrain.tl
    react.fireEvent(param.toolParam.gameCtx.rendererComponent, "selectEntity", {entity = entity, stack = true})
    ```

- `getCurrentRecipeName : function() : string`: Name of the recipe being rendered.
- `getCurrentRecipeId : function() : integer`: Id of the recipe being rendered.
- `enqueueDeferredStep : function(fn : function(), debugScope : string)`: call this function with restricted api, in parallel to rendering preparations
- `enqueueJoin : function(fn : function(), debugScope : string)`: call this function on main thread after all parallelized react things are complete
- `enqueueParallel : function<ParamT>(useFnName : string, useFnParams : ParamT, join : function(workItemResult : any, useFnParams : ParamT))`: computes result in another thread via useFn(useFnName)(...) and applies it via enqueueJoin(result, ...)

### React.IaHandler (record)

```teal
record IaHandler
```

Input action handler created with `React.iaHandler` or `React.iaHandlerExtended`.

- `handlerFn : function(Gui.InputAction.InvokeData)`: note: invokeData is nil if acceptRepeatedPresses == true
- `isEnabledFn : function() : boolean`: Returns whether the action is enabled; a boolean result is converted to `Enabled` or `Inactive`.
- `promptTextOverride : string`: Text shown in the button prompt instead of the action's default name.
- `mode : integer`: 0: default, 1: auto keyrepeat, 2: get called with InputAction.InvokeData for all events (including keyrelease)

### React.IaForward (record)

```teal
record IaForward
```

Forwarding of an input action to another node, created with `React.iaForward`.

## ReactPluginDesc (record)

```teal
global record ReactPluginDesc
```

`data` of a `"react-plugin <extension point id>"` generic resource.

- `filePath : ResName`: The plugin recipe as `"file.script@RecipeName"`.
- `condition : ResName`: Optional function reference (`"file.script@fn"`) that decides whether the plugin is shown.
- `order : number`: Sort key; plugins are created in ascending order (default 0).

## ReactPlugin0 (record)

```teal
global record ReactPlugin0
```

A loaded plugin of an extension point without parameters.

- `recipe : Recipe0`: The plugin recipe.
- `order : number`: Sort key.
- `condition : function() : boolean`: Optional; the plugin is created only if it returns true.

## ReactPlugin (record)

```teal
global record ReactPlugin<RecipeParam>
```

A loaded plugin of an extension point with parameters.

- `recipe : Recipe<RecipeParam>`: The plugin recipe.
- `order : number`: Sort key.
- `condition : function(RecipeParam) : boolean`: Optional; the plugin is created only if it returns true for the parameters.

## ReactReplacementApi (record)

```teal
global record ReactReplacementApi
```

API given to a replacement config's `doReplaceFn`.

- `ReplaceRecipe : function<RecipeType>(originalRecipe : RecipeType, replacement : RecipeType)`: delegate all calls to the original recipe to the replacement
Remember that the replacement must be fully compatible, e.g. if the original provides an Api, the replacement must provide the same one!

## ReactReplacementConfigDesc (record)

```teal
global record ReactReplacementConfigDesc
```

`data` of a `"react-replacement-config"` generic resource, which replaces GUI recipes before the GUI starts.

Example:
```lua
-- mods/release/urbangames_campaign_mission_05/content/mission/mission/perk_icons_replacement.res.lua
function data()
    return {
        type = "react-replacement-config",
        data = {
            filePath = "urbangames_campaign_mission_05::/mission/perk_icons_replacement.script",
            doReplaceFn = "doReplaceFn"
        }
    }
end
```

- `filePath : ResName`: Script file that contains `doReplaceFn`, without the `@function` part.
- `doReplaceFn : string`: name of the function in the above .script.tl
- `order : number`: Sort key; configs run in ascending order (default 0).

## ReactReplacementConfig (record)

```teal
global record ReactReplacementConfig
```

A loaded replacement config.

the function that actually triggers the replacement (takes a callback to tell react what to exactly replace)
- `doReplaceFn : function(replacementApi : ReactReplacementApi)`
- `order : number`: Sort key.
