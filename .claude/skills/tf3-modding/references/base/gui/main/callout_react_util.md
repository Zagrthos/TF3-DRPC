# gui.main.callout_react_util

Definition file: `base/tealdef/gui/main/callout_react_util.d.tl` (in the game folder).

Recipes for callouts (small popups anchored to a node): a toggle button that opens a callout
while it is pressed, and a default wrapper that draws callout content with an arrow.
Callouts are shown by the callout container (`CalloutApi`), usually the one in
`GameContext.calloutContainerRef`.

## CalloutReactUtil (record)

```teal
global record CalloutReactUtil
```
Load: `local callout_react_util = ug_require "::/gui/main/callout_react_util.tl" as CalloutReactUtil`

Recipes for toggle buttons that open callouts, and a default callout frame.

- `ToggleCalloutButton : Recipe<ToggleCalloutButtonParamsT<IRecipeParamWithMeta>>`: Toggle button that opens a callout with the given recipe while it is pressed.
    - notes: The callout closes when the user clicks the button again, presses the menu-back input, clicks outside the callout, moves focus out of the callout, or when the button is no longer drawn (checked on every `heartbeat` event). The callout also closes when the button unmounts. Inside the callout, the navigation input actions move focus, and the callout takes focus when it opens. The callout content is compared with `table_util.deepEquals` (or `recipeParamCompareFn`) to decide whether it has to be rebuilt.

    ```lua
    -- from collapsible_checkbox_list.tl (shortened)
    local button = callout_react_util.ToggleCalloutButton{
        calloutContainerRef = param.calloutContainerRef,
        button = param.header,
        onOpenedChanged = function(isOpen : boolean)
            isOpenState:set(isOpen)
        end,
        recipe = CheckBoxEntriesList,
        recipeParam = { makeEntries = param.makeEntries },
    }
    ```

    - used 8x in the base game, e.g. `base/content/gui/gui/main/collapsible_checkbox_list.tl:198`: `local button = callout_react_util.ToggleCalloutButton{`
- `DefaultCalloutWrapper : Recipe<DefaultCalloutWrapperParamsT<IRecipeParamWithMeta>>`: Draws the content of `recipe` in a box with style class `callout-content` and a downward arrow below it.
    - notes: Use it as the `recipe` of `CalloutApi.open` when the callout sits above its node. The arrow is the image `::/gui/builtin/window/icons/callout_down.tga` with style class `callout-arrow`.

    ```lua
    -- from tile_list_react_util.tl
    getCalloutApi().open({
        recipe = callout_react_util.DefaultCalloutWrapper,
        recipeParam = {
            meta = { class = "critical" },
            recipe = builtin.TextView,
            recipeParam = { text = criticalInfo, meta = { class = "font-scale-body" } },
        },
        anchor = api.type.Vec2f.new(0.5, 1.0),
        gravity = api.type.Vec2f.new(0.4, 0.0),
        parent = criticalIconRef,
    } as CalloutApi.CalloutParamsT<IRecipeParamWithMeta>)
    ```

    - used 3x in the base game, e.g. `base/content/gui/gui/main/tile_list_react_util.tl:59`: `recipe = callout_react_util.DefaultCalloutWrapper,`

### CalloutReactUtil.ToggleCalloutButtonParamsT (record)

```teal
record ToggleCalloutButtonParamsT<R is IRecipeParamWithMeta>
```

Parameters of the `ToggleCalloutButton` recipe.

Notes:

`R` is the parameter type of the recipe shown inside the callout.

- `gameCtx : GameContext`: Game context. Its `calloutContainerRef` is used when `calloutContainerRef` is not set.
- `button : TreeNodeId`: Node shown as the content of the toggle button.
- `toggleButtonClass : string`: Style class of the toggle button.
- `calloutContainerRef : ReactRefWrapApi0<CalloutApi>`: Callout container to open the callout in. If `nil`, `gameCtx.calloutContainerRef` is used.
- `recipe : Recipe<R>`: Recipe that builds the callout content.
- `recipeParam : R`: Parameters passed to `recipe`.
- `anchor : Vec2f`: Anchor point of the callout content, relative to its own size (`0..1` per axis).
    - notes: The callout container applies it as the `anchorPoint` style of the content. The base game uses `(0.5, 1.0)` (bottom centre of the callout) together with a `gravity` of `(0.5, 0.0)` to place a callout above its node.
- `gravity : Vec2f`: Point on the toggle button, relative to its size (`0..1` per axis), where the callout is placed.
    - notes: The callout container passes it to `getPosition(gravity.x, gravity.y)` of the parent node. Ignored when `getPositionFn` is set.
- `getPositionFn : function() : Vec2f`: overrides automatic placement
- `recipeParamCompareFn : function(R, R) : boolean`: overrides default compare
- `onOpenedChanged : function(boolean)`: Called with `true` when the callout opens and with `false` when it closes.
- `onValueChange : function(integer)`: If set, called with the toggle button's new value (`0` or `1`) instead of the default open/close handling.
    - notes: Without this callback, pressing the button opens the callout and releasing it closes it. With it, the recipe does not open or close the callout on clicks; the callback replaces that logic.
- `tooltip : string`: Tooltip of the toggle button.

### CalloutReactUtil.ToggleCalloutButtonApi (record)

```teal
record ToggleCalloutButtonApi
```

API that `ToggleCalloutButton` provides through `react.provideApi`.

- `close : function()`: Closes the callout and resets the toggle button if the callout is open.

### CalloutReactUtil.DefaultCalloutWrapperParamsT (record)

```teal
record DefaultCalloutWrapperParamsT<R is IRecipeParamWithMeta> is IRecipeParamWithMeta
```

Parameters of the `DefaultCalloutWrapper` recipe.

- `recipe : Recipe<R>`: Recipe that builds the content inside the callout frame.
- `recipeParam : R`: Parameters passed to `recipe`.
