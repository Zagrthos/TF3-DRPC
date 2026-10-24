# gui.menu.difficulty_react_util

Definition file: `base/tealdef/gui/menu/difficulty_react_util.d.tl` (in the game folder).

Toggle-button row that applies one of the difficulty presets (Easy, Medium, Hard, Very Hard) to a set of game-mechanic parameters.

## DifficultyReactUtil (record)

```teal
global record DifficultyReactUtil
```
Load: `local difficulty_react_util = ug_require "::/gui/menu/difficulty_react_util.tl" as DifficultyReactUtil`

Util table of `difficulty_react_util.tl`; holds the `DifficultyPresetToggles` recipe.

- `DifficultyPresetToggles : Recipe<DifficultyPresetTogglesParam>`: Toggle-button group with one button per difficulty (Easy, Medium, Hard, Very Hard) that sets the game-mechanic parameters to that preset.
    - notes: The selected button is the difficulty that `difficulty_util.calculateDifficulty(modParams)` derives from the current values; when the values match no preset, no button is selected. Clicking a button copies `modParams`, overwrites the entries of the chosen preset (`difficulty_util.getModParamPreset`) and passes the result to `setModParams`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/advanced_settings.tl:206`: `local difficultyToggles = difficulty_react_util.DifficultyPresetToggles {`

### DifficultyReactUtil.DifficultyPresetTogglesParam (record)

```teal
record DifficultyPresetTogglesParam
```

Parameters of `DifficultyReactUtil.DifficultyPresetToggles`.

- `modParams : {string : integer}`: Current parameter values (parameter key to selected option index) from which the active difficulty is derived.
- `setModParams : function({string : integer})`: Receives the new parameter table after a difficulty button was clicked.
- `onHover : function(boolean)`: Attention callback of the button group (`onAttention`); called with true or false when the group gains or loses hover/attention.
