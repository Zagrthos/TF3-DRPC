# gui.main.collapsible_checkbox_list

Definition file: `base/tealdef/gui/main/collapsible_checkbox_list.d.tl` (in the game folder).

Lists of checkbox entries under a header button: either collapsible in place
(`CollapsibleCheckboxList`) or opened as a callout (`CheckboxListCallout`). Both support
mouse and gamepad.

## CheckboxList (record)

```teal
global record CheckboxList
```
Load: `local checkboxList = ug_require "::/gui/main/collapsible_checkbox_list.tl" as CheckboxList`

Recipes for checkbox lists with a header.

- `CollapsibleCheckboxList : Recipe<CollapsibleCheckboxListParam>`: Header button with a checkbox list below it that expands and collapses in place.
    - notes: The list starts expanded with mouse/keyboard and collapsed with a gamepad; `IA_BACK` collapses it. The style class of the recipe is `expanded` or `collapsed`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1151`: `groupChildren[#groupChildren + 1] = checkboxList.CollapsibleCheckboxList{`
- `CheckboxListCallout : Recipe<CheckboxListCalloutParam>`: Button that opens a checkbox list in a callout (via `CalloutReactUtil.ToggleCalloutButton`); supports indeterminate (tri-state) entries.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:450`: `local configurationButton = checkboxList.CheckboxListCallout{`

### CheckboxList.CollapsibleCheckboxListParam (record)

```teal
record CollapsibleCheckboxListParam
```

Parameters of `CollapsibleCheckboxList`.

- `onToggle : function(index : integer)`: Called with the 1-based index of the entry the user toggled.
- `isToggled : function(index : integer) : boolean`: Returns whether the entry with the given 1-based index is checked.
- `header : string`: Text of the header button that expands and collapses the list.
- `makeEntries : function() : {{TreeNodeId, integer}}`: second param is for level (defaults to 1 if nil)
- `hasArrow : boolean`: defaults to false if nil
- `verticalScrollPolicy : Builtin.Type.ScrollBarPolicy`: defaults to AlwaysOff if nil

### CheckboxList.CheckboxListCalloutParam (record)

```teal
record CheckboxListCalloutParam
```

Parameters of `CheckboxListCallout`.

- `calloutContainerRef : ReactRefWrapApi0<CalloutApi>`: Reference to the callout container in which the list is opened (e.g. `gameCtx.calloutContainerRef`).
- `onToggle : function(index : integer)`: Called with the 1-based index of the entry the user toggled.
- `isToggledDependentSource : ReactStateT<any>`: React state from which the check states are derived.
- `transformDependentSourceFn : function({integer}, any) : {integer}`: Derives the check state of each entry from `isToggledDependentSource`, as used by `react.useDependentState`.
    - notes: The resulting list has one value per entry (`1` checked, `0` unchecked, `-1` indeterminate).
- `header : string`: Text of the button that opens the callout.
- `makeEntries : function() : {{TreeNodeId, integer}}`: second param is for level (defaults to 1 if nil)
- `hasArrow : boolean`: defaults to false if nil
- `verticalScrollPolicy : Builtin.Type.ScrollBarPolicy`: defaults to AlwaysOff if nil
- `indeterminateToggleToChecked : boolean`: if true, toggling an indifferent checkbox will set it to checked instead of unchecked
- `tooltip : string`: Tooltip of the button that opens the callout.
