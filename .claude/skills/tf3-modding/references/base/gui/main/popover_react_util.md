# gui.main.popover_react_util

Definition file: `base/tealdef/gui/main/popover_react_util.d.tl` (in the game folder).

Popover windows: small windows that open next to a toggle button and close when the user
clicks outside of them or presses back.

## PopoverReactUtil (record)

```teal
global record PopoverReactUtil
```
Load: `local popover_react_util = ug_require "::/gui/main/popover_react_util.tl" as PopoverReactUtil`

Popover window recipes.

- `PopoverWindowContent : Recipe<PopoverWindowParam>`: Content of a popover window; takes focus on mount and calls `onClose` on a click outside or on `IA_MENU_BACK`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/cargofilter_window.tl:902`: `content = popover_react_util.PopoverWindowContent {`
- `PopoverWindow : Recipe<PopoverWindowParam>`: Non-movable window without close button (style class `popover`) that shows `PopoverWindowContent`.
    - used 16x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_manager_panel.tl:68`: `wc.removeAllWindows(popover_react_util.PopoverWindow)`
- `TogglePopoverButton : Recipe<TogglePopoverButtonParams>`: Icon toggle button that opens and closes a `PopoverWindow` in the game's window container; only one popover is open at a time.

### PopoverReactUtil.PopoverWindowParam (record)

```teal
record PopoverWindowParam
```

Parameters of `PopoverWindow` and `PopoverWindowContent`.

- `onClose : function`: Called when the popover should close (click outside, `IA_MENU_BACK`).
- `windowClass : string`
- `windowTitle : string`: Title of the window.
- `x : number`: Initial horizontal position of the window.
- `y : number`: Initial vertical position of the window.
- `recipe : IRecipe`: Recipe of the content.
- `params : any`: Parameter passed to `recipe`.
- `compact : boolean`: Passed to the window's `compact` option.
- `inputActionBubbleUpFilter : Builtin.Type.InputActionBubbleUpFilter`: Filter for input actions that bubble up from the window.

### PopoverReactUtil.TogglePopoverButtonParams (record)

```teal
record TogglePopoverButtonParams
```

Parameters of `TogglePopoverButton`.

- `gameCtx : GameContext`: Game context; the popover is added to its window container.
- `getPositionFn : function() : Vec2f`: Optional; returns the position of the popover. Defaults to the top right corner of the button.
- `tooltip : string`: Tooltip of the button, also used as title of the popover window.
- `icon : string`: Icon path of the button.
- `extraButtonClass : string`
- `recipe : IRecipe`: Recipe of the popover content.
- `params : any`: Parameter passed to `recipe`.
- `compact : boolean`: Passed to the popover window's `compact` option.
- `windowClass : string`: Additional style class of the popover window.
