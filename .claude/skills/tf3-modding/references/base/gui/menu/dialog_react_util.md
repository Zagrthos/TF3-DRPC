# gui.menu.dialog_react_util

Definition file: `base/tealdef/gui/menu/dialog_react_util.d.tl` (in the game folder).

Generic modal dialog window (title, text, a row of choice buttons) used throughout the menus and the in-game GUI.

## DialogReactUtil (record)

```teal
global record DialogReactUtil
```
Load: `local dialog_react_util = ug_require "::/gui/menu/dialog_react_util.tl" as DialogReactUtil`

Util table of `dialog_react_util.tl`; holds the `DialogWindow` recipe.

- `DialogWindow : Recipe<DialogWindowParam>`: Window recipe of a modal, non-movable dialog with optional text and a list of choice buttons.
    - notes: Pass it to a window container's `addSingletonWindow` together with a `DialogWindowParam`. Clicking any choice first calls `onClose` and then the choice's `onClick`. Clicking outside the dialog (left button) also calls `onClose`. On the menu-back input (`IA_MENU_BACK`) the choice at `onBackChoiceIndex` is triggered (if set) and then `onClose` is called. Focus starts on the first primary (non-secondary) choice.
    - used 60x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:17`: `wc.addSingletonWindow(dialog_react_util.DialogWindow, {`

### DialogReactUtil.DialogWindowParam (record)

```teal
record DialogWindowParam
```

Parameters of `DialogReactUtil.DialogWindow`.

- `onClose : function()`: Called when the dialog should close: after any choice, on a click outside the dialog, on menu-back, or from the window's close button. It must remove the window, usually with `removeAllWindows(dialog_react_util.DialogWindow)`.
- `closable : boolean`: Shows the window's close button when true. Default false (no close button).
- `setBlockedForModal : function(blocked : boolean)`: Optional callback that the dialog calls with `true` when it mounts and with `false` when it unmounts or is clicked away, so the caller can block the UI behind it. Menus pass `CommonMenuParams.setBlockedForModal`.
- `windowTitle : string`: Window title.
- `displayText : string`: Body text, shown in a scroll area above the buttons. Optional.
- `x : number`: Initial horizontal position of the window (passed as `initialX`; the base game uses `0.5` to centre it, or a screen position for pop-ups next to a control).
- `y : number`: Initial vertical position of the window (passed as `initialY`; `0.5` centres it).
- `choices : {Choice}`: Buttons of the dialog, in display order.
- `onBackChoiceIndex : integer`: specifies which choice has to be choosen at on back, default does NOTHING (to avoid bad side effects)
- `mode : Mode`: Layout of the buttons; `"Flow"` or `"Box"` (default).
- `windowClass : string`: Extra style class added to the window after `popover, dialog`.
- `windowId : string`: Id of the window.

#### DialogReactUtil.DialogWindowParam.Mode (enum)

```teal
enum Mode
```

How the choice buttons are laid out.

- `"Flow"`: Buttons are placed in a flow layout that wraps onto several lines (used for the tag picker of the mod upload form).
- `"Box"`: Buttons are placed in one horizontal row inside a scroll area. Any value other than `"Flow"` (including nil) gives this layout.

#### DialogReactUtil.DialogWindowParam.Choice (record)

```teal
record Choice
```

One button of the dialog.

- `text : string`: Button label.
- `tooltip : string`: Tooltip of the button.
- `onClick : function()`: Called when the button is clicked, after the dialog's `onClose`. Optional; a choice without `onClick` only closes the dialog.
- `isSecondary : boolean`: nil/false by default
- `enabled : boolean`: nil/false by default
