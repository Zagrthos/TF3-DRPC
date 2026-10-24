# gui.main.button_react_util

Definition file: `base/tealdef/gui/main/button_react_util.d.tl` (in the game folder).

Shortcut functions that build `builtin.Button` and `builtin.ToggleButton` nodes with a text,
an icon or both as content. Load the module with
`ug_require "/gui/main/button_react_util.tl"`.

## ButtonReactUtil (record)

```teal
global record ButtonReactUtil
```
Load: `local button_react_util = ug_require "::/gui/main/button_react_util.tl" as ButtonReactUtil`

Functions that return ready-made button nodes (text, icon, icon plus text, icon toggle).

Notes:

Each function returns a `TreeNodeId` that you place in a layout like any other node.
Unless noted otherwise, the button is enabled, its content keeps focus traversal
(`disableFocusableRecursive = false`) and text uses the style class `font-scale-body`.

- `makeTextButton : function(className : string, text : string, onClick : function(), tooltip : string, disabled? : boolean, textClass? : string, disableFocusableRecursive? : boolean) : TreeNodeId`  (call as `button_react_util.makeTextButton(…)`): Builds a `builtin.Button` whose content is a single `builtin.TextView`.
    - param `className`: Style class of the button.
    - param `text`: Button text.
    - param `onClick`: Called when the button is clicked.
    - param `tooltip`: Tooltip of the button.
    - param `disabled`: If `true`, the button is disabled. Note that this parameter is inverted compared to the `enabled` parameter of the icon buttons.
    - param `textClass`: Style class of the text view (default `font-scale-body`).
    - param `disableFocusableRecursive`: Passed to the button's `meta.disableFocusableRecursive` (default `false`).
    - returns: The button node.
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/feedback_list_util.tl:69`: `acceptButton and button_react_util.makeTextButton(`
- `makeIconToggleButton : function(className : string, path : string, onClick : function(v : boolean), tooltip : string, enabled? : boolean, tag? : string, disableFocusableRecursive? : boolean, value? : boolean) : TreeNodeId`  (call as `button_react_util.makeIconToggleButton(…)`): Builds a `builtin.ToggleButton` with an icon as content and a boolean value callback.
    - param `className`: Style class of the toggle button.
    - param `path`: Image path of the icon.
    - param `onClick`: Called with the new state (`true` = pressed) when the user toggles the button.
    - param `tooltip`: Tooltip of the button.
    - param `enabled`: If `false`, the button is disabled (default `true`).
    - param `tag`: Value for the button's `meta.tag`.
    - param `disableFocusableRecursive`: Passed to the button's `meta.disableFocusableRecursive` (default `false`).
    - param `value`: Current state of the toggle button (`nil` counts as `false`).
    - returns: The toggle button node.
    - notes: The underlying toggle button works with integers (`0`/`1`). This helper converts them: `value` is passed as `1` when `true`, otherwise `0`, and `onClick` receives `true` when the button's new value is `1`.
    - used 3x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:3520`: `table.insert(actionListChildrenButtons, button_react_util.makeIconToggleButton(`
- `makeIconButton : function(className : string, path : string, onClick : function(), tooltip : string, enabled? : boolean, tag? : string, disableFocusableRecursive? : boolean) : TreeNodeId`  (call as `button_react_util.makeIconButton(…)`): Builds a `builtin.Button` whose content is a `builtin.ImageView` scaled with `AutoFit`.
    - param `className`: Style class of the button.
    - param `path`: Image path of the icon, e.g. `"::/gui/main/icons/..."`.
    - param `onClick`: Called when the button is clicked.
    - param `tooltip`: Tooltip of the button.
    - param `enabled`: If `false`, the button is disabled (default `true`).
    - param `tag`: Value for the button's `meta.tag`.
    - param `disableFocusableRecursive`: Passed to the button's `meta.disableFocusableRecursive` (default `false`).
    - returns: The button node.
    - used 13x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:1729`: `button_react_util.makeIconButton(`
- `makeIconTextButton : function(className : string, path : string, text : string, onClick : function(), tooltip : string, enabled? : boolean, tag? : string, textClass? : string, disableFocusableRecursive? : boolean) : TreeNodeId`  (call as `button_react_util.makeIconTextButton(…)`): Builds a `builtin.Button` with an icon followed by a text in a horizontal box.
    - param `className`: Style class of the button.
    - param `path`: Image path of the icon.
    - param `text`: Text shown to the right of the icon.
    - param `onClick`: Called when the button is clicked.
    - param `tooltip`: Tooltip of the button.
    - param `enabled`: If `false`, the button is disabled (default `true`).
    - param `tag`: Value for the button's `meta.tag`.
    - param `textClass`: Style class of the text view (default `font-scale-body`).
    - param `disableFocusableRecursive`: Passed to the button's `meta.disableFocusableRecursive` (default `false`).
    - returns: The button node.
