# base.difficulty_util

Definition file: `base/tealdef/base/difficulty_util.d.tl` (in the game folder).

Types of `/base/difficulty_util.tl` (`DifficultyUtil`): the difficulty presets of the new-game
settings and the value tables that map a settings level to a scale factor.

## GameDifficulty (enum)

```teal
global enum GameDifficulty
```

Difficulty preset of the new-game settings.

- `"Easy"`: Easy (label "Easy").
  In game: "Easy" (German: „Einfach“)
- `"Medium"`: Medium (label "Normal").
  In game: "Normal" (German: „Normal“)
- `"Hard"`: Hard (label "Hard").
  In game: "Hard" (German: „Schwierig“)
- `"VeryHard"`: Very hard (label "Very Hard").
  In game: "Very Hard" (German: „Sehr schwierig“)

## DifficultyUtil (record)

```teal
global record DifficultyUtil
```
Load: `local difficulty_util = ug_require "::/base/difficulty_util.tl" as DifficultyUtil`

Difficulty helpers returned by `ug_require "/base/difficulty_util.tl"`.

- `calculateDifficulty : function(modParams : {string : integer}) : GameDifficulty`  (call as `difficulty_util.calculateDifficulty(…)`): Finds the preset whose settings match the given mod parameters exactly.
    - param `modParams`: Mod parameters of the base game (`modParams[""]`), keyed like `"advancedOptions.cargoIncome"`.
    - returns: The matching difficulty, or nil if a difficulty setting is missing or the combination matches no preset (custom settings).

    ```lua
    -- from base/content/gui/gui/menu/new_game_or_map_settings_page.tl
    difficulty = difficulty_util.getLabel(difficulty_util.calculateDifficulty(saveGameData.info.modParams[""]))
    ```

    - used 3x in the base game, e.g. `base/content/gui/gui/menu/difficulty_react_util.tl:20`: `local oldDifficulty = difficulty_util.calculateDifficulty(param.modParams)`
- `getModParamPreset : function(difficulty : GameDifficulty) : {string : integer}`  (call as `difficulty_util.getModParamPreset(…)`): Returns the settings levels of a preset, keyed by mod parameter name.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/difficulty_react_util.tl:12`: `local preset = difficulty_util.getModParamPreset(difficulty)`
- `getLabel : function(difficulty : GameDifficulty) : string`  (call as `difficulty_util.getLabel(…)`): Translated name of the difficulty; "Custom" for nil.
    - used 6x in the base game, e.g. `base/content/gui/gui/menu/difficulty_react_util.tl:22`: `["Easy"] = {1, difficulty_util.getLabel("Easy")},`
- `getScale : function(modParam : string, level : integer) : number`  (call as `difficulty_util.getScale(…)`): Scale factor of a setting at a given level, for example the cost scale for the chosen cost setting.
    - param `modParam`: Setting name, e.g. `"advancedOptions.landmarkResources"` or `"locations.towns.frequency"`.
    - param `level`: 1-based index into the setting's value table.
    - returns: The factor; `1.0` (with a warning in the log) if the setting is unknown or the level is out of range.
    - used 21x in the base game, e.g. `base/content/base/base/mod.script.tl:195`: `baseConfig.locations.industry.industryDensity * difficulty_util.getScale("locations.industry.targetIndustryDensity", modParams["locations.industry.targetIndustr`
- `getAllValues : function(modParam : string) : {number}`  (call as `difficulty_util.getAllValues(…)`): All factors of a setting, indexed by level; `{1.0}` if the setting is unknown.
    - used 6x in the base game, e.g. `base/content/gui/gui/entity_window/town/town_eow.script.tl:934`: `values = difficulty_util.getAllValues("townConfig.sensitivityUrbanCare"),`
