# gui.main.script_param_util

Definition file: `base/tealdef/gui/main/script_param_util.d.tl` (in the game folder).

Widgets for script parameters (`ScriptParam`): the settings of the base game and of mods
that are shown as buttons, icon buttons, sliders, combo boxes or checkboxes, e.g. in the
new-game and mod settings and in the construction menu.

## ScriptParamUtil (record)

```teal
global record ScriptParamUtil
```
Load: `local script_param_util = ug_require "::/gui/main/script_param_util.tl" as ScriptParamUtil`

Builds UI widgets for script parameters and looks up the base game's parameters.

Notes:

A parameter's value is either an entry of its `numbers` list or, when it has no
`numbers`, the 1-based index of the chosen entry of `values`.

- `buildScriptParamCompSimple : function(param : CompSimpleParam) : TreeNodeId`  (call as `script_param_util.buildScriptParamCompSimple(…)`): Builds the widget (with label) for a parameter described by `ParamForUi`.

    ```lua
    -- from town_eow.script.tl
    local scriptParam = script_param_util.getScriptParam(optionName)
    script_param_util.buildScriptParamCompSimple({
        scriptParam = {
            uiType = scriptParam.uiType,
            name = scriptParam.name,
            values = scriptParam.values,
        },
        currentValue = sensitivityState:old(),
        onValueChange = function(index) commit(index, nil, nil) end,
        vertical = false,
    })
    ```

    - used 4x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:1161`: `scriptParamForUiOld and scriptParamUtil.buildScriptParamCompSimple({`
- `buildScriptParamComp : function(scriptParam : ScriptParam, initialIndex : integer, onValueChange : function(integer), addSpacer? : boolean, toggleButtonsFlowLayout? : boolean, onHover? : function(boolean), allowCoalesce? : boolean) : TreeNodeId`  (call as `script_param_util.buildScriptParamComp(…)`): filteredIndices are expected to be unique and sorted ascending --> these indices get filtered AWAY!

Builds the widget for a `ScriptParam`, starting at the choice `initialIndex`; `onValueChange` receives the new value.
    - used 10x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:368`: `return scriptparamutil.buildScriptParamComp(scriptParam, scriptParam.defaultIndex, function(newValue : integer)`

filteredIndices are expected to be unique and sorted ascending --> these indices get filtered AWAY!
- `searchAndBuildScriptParamComp : function(key : string, getModParam : function(field : string) : number, setModParamField : function(field : string, value : number), addSpacer? : boolean, onHover? : function(isSelected : boolean), filterTags? : {string}, allowCoalesce? : boolean) : MenuIconReactUtil.HintWidgetParam.Data, TreeNodeId`  (call as `script_param_util.searchAndBuildScriptParamComp(…)`): Looks up a base-game parameter by key (see `getScriptParam`) and builds its widget. The value is read with `getModParam` and written with `setModParamField`.
    - returns: The hint data (title and description of the parameter) and the widget, or `nil, nil` when no parameter matches.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/advanced_settings.tl:96`: `local data, object = script_param_util.searchAndBuildScriptParamComp(`
- `wrap : function(text : string, vertical : boolean, node : TreeNodeId, addSpacer? : boolean, onHover? : function(boolean), extraClass? : string) : TreeNodeId`  (call as `script_param_util.wrap(…)`): Puts a label text and a widget into one row (or column, when `vertical`), optionally with a spacer and a hover callback on the label.
    - used 8x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:220`: `return scriptparamutil.wrap(text, bool, builtin.Component{`
- `getScriptParam : function(key : string, filterTags? : {string}) : ScriptParam`  (call as `script_param_util.getScriptParam(…)`): Finds a parameter of the base game (the mod with the empty name) by key.
    - param `key`: Key of the parameter.
    - param `filterTags`: Tags the parameter must have (compared case-insensitively, all must match); parameters without tags always match.
    - returns: The parameter, or `nil`.
    - used 7x in the base game, e.g. `base/content/gui/gui/entity_window/town/town_eow.script.tl:907`: `local scriptParam = script_param_util.getScriptParam(optionName)`
- `clampScriptParamToDiscreteValue : function(value : number, scriptParam : ScriptParam) : number`  (call as `script_param_util.clampScriptParamToDiscreteValue(…)`): Returns the entry of the parameter's `numbers` closest to `value`, or, without `numbers`, `value` limited to the number of `values`.
- `clampScriptParamValueIndex : function(value : number, scriptParam : ScriptParam) : integer`  (call as `script_param_util.clampScriptParamValueIndex(…)`): `clampScriptParamToDiscreteValue`, rounded to an integer.
    - used 4x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:534`: `params[scriptParam.key] = scriptparamutil.clampScriptParamValueIndex(params[scriptParam.key], scriptParam)`
- `getScriptParamDefaultValue : function(numbers : {number}, defaultIndex : integer) : number`  (call as `script_param_util.getScriptParamDefaultValue(…)`): Returns `numbers[defaultIndex]` when `numbers` has more than `defaultIndex` entries, otherwise `defaultIndex` itself.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:1253`: `values[p.key] = scriptParamUtil.getScriptParamDefaultValue(`
- `getScriptParamDefaultValueIndex : function(numbers : {number}, defaultIndex : integer) : integer`  (call as `script_param_util.getScriptParamDefaultValueIndex(…)`): `getScriptParamDefaultValue`, rounded to an integer.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:536`: `params[scriptParam.key] = scriptparamutil.getScriptParamDefaultValueIndex(scriptParam.numbers, scriptParam.defaultIndex)`
- `getScriptParamLabel : function(key : string, value : number, filterTags? : {string}) : string`  (call as `script_param_util.getScriptParamLabel(…)`): Returns the display text of the choice of a base-game parameter that is closest to `value`.
- `getScriptParamLabelUnfiltered : function(key : string, value : number) : string`  (call as `script_param_util.getScriptParamLabelUnfiltered(…)`): get first match of the value (among all filterTags), or nil of no exact match
    - used 4x in the base game, e.g. `base/content/gui/gui/menu/new_game_or_map_settings_page.tl:216`: `local formatText = script_param_util.getScriptParamLabelUnfiltered("map.format", savegameData.info.modParams[""]["map.format"])`

### ScriptParamUtil.ParamForUi (record)

```teal
record ParamForUi
```

Description of a parameter for `buildScriptParamCompSimple`; usually copied from a `ScriptParam`.

- `uiType : ScriptParamType`: Kind of widget (button, icon button, slider, combo box, checkbox).
- `iconStyle : IconStyle`: Style of the icons of an icon-button parameter.
- `name : string`: Label of the parameter.
- `values : {string}`: Display texts of the choices (for icon buttons, the icon paths).
- `numbers : {number}`: Optional value of each choice; without it, the value is the 1-based choice index.
- `tooltips : {string}`: Optional tooltip per choice.
- `defaultIndex : integer`
- `hideLabel : boolean`: In compact mode, hides the label of an icon-button parameter.

allows to delay onValueChange (e.g. when dragging a slider, it will delay onValueChange to mouse release)
- `allowCoalesce : boolean`
- `makeCompactButtonFn : function(value : number) : TreeNodeId`: Optional; builds the button content for a value (in compact mode and for button choices) instead of the default icon or text.
- `formatValueFn : function(value : number) : string`: Optional; formats a value for the text next to a slider.
- `stepValueFn : function(value : number, direction : integer, precise : boolean) : number`

#### ScriptParamUtil.ParamForUi.IconStyle (enum)

```teal
enum IconStyle
```

Display style of icon buttons.

- `"Default"`: Icons as they are.
- `"Colored"`: Icons with the coloured style class `forever-fair`.

### ScriptParamUtil.CompSimpleParam (record)

```teal
record CompSimpleParam is IRecipeParamWithMeta
```

Parameters of `buildScriptParamCompSimple`.

- `scriptParam : ParamForUi`: The parameter to show.
- `currentValue : number`: Current value (see `ScriptParamUtil` for the meaning of values).
- `onValueChange : function(number)`: Called with the new value when the user changes it.
- `toggleButtonsFlowLayout : boolean`: For button parameters, lay the buttons out in a flow instead of uniform cells.
- `vertical : boolean`: Places the label above the widget instead of to its left; `nil` returns the widget without label.
- `compact : boolean`: Shows only a small button that opens the full widget in a callout.
- `addSpacer : boolean`: Adds a spacer on the right side of the row.
- `disableGamepadNavigation : boolean`: Passed to the widget (e.g. the slider) to disable its gamepad navigation.
- `onHover : function(boolean)`: Called with `true`/`false` when the label gains or loses attention.
- `gameCtx : GameContext`: Game context; needed in compact mode for the callout container.
- `updateCurrentValue : boolean`
