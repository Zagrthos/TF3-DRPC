# gui.menu.new_game_react_util

Definition file: `base/tealdef/gui/menu/new_game_react_util.d.tl` (in the game folder).

Helpers of the "new game" and "new map" settings pages, loaded with `ug_require "/gui/menu/new_game_react_util.tl"`.

## NewGameReactUtil (record)

```teal
global record NewGameReactUtil
```
Load: `local new_game_react_util = ug_require "::/gui/menu/new_game_react_util.tl" as NewGameReactUtil`

Helpers for building the new game / new map settings pages.

- `makeDefaultBaseModParams : function() : {string : Index}`  (call as `new_game_react_util.makeDefaultBaseModParams(…)`): Returns the default index of every script parameter of the base game (mod `""`).
    - returns: Parameter keys mapped to their default value index.
- `makeDefaultConfigDictForClimate : function(climate : string) : {{string, string}}`  (call as `new_game_react_util.makeDefaultConfigDictForClimate(…)`): Builds the default config dictionary for a climate.
    - param `climate`: Name of the climate resource.
    - returns: List of `{key, value}` pairs: `climate`, `economy` and `nameList` (the climate's default economy and name list). Empty, with an error logged, if the climate does not exist.
- `gatherMapConfig : function(configDict : {{string, string}}) : MapConfig`  (call as `new_game_react_util.gatherMapConfig(…)`): Finds the climate of a config dictionary and the terrain generators that can be used with it.
    - param `configDict`: Config dictionary (`{key, value}` pairs) with a `climate` entry.
    - returns: The climate description and the visible terrain generators of that climate that are neither editor-only nor import generators, sorted by `order`.
- `autoActivateConfigDictMods : function(activeMods : {string}, initialConfigDict : {{string, string}}) : {string}`  (call as `new_game_react_util.autoActivateConfigDictMods(…)`): Adds the mods that config dictionary values refer to to a list of active mods.
    - param `activeMods`: Names of the active mods.
    - param `initialConfigDict`: Config dictionary; a value of the form `<mod>::...` requires the mod `<mod>`.
    - returns: A copy of `activeMods` with the required mods added.
- `prepareState : function(initialConfigDict : {{string, string}}, possibleGenerators : {NewGameReactUtil.Generator}, initial : integer, autoActivateMods : boolean) : StatesBundle`  (call as `new_game_react_util.prepareState(…)`): Reads the last used new game settings from the app config (`mainMenuState`) and completes them.
    - param `initialConfigDict`: Config dictionary of the map; used to auto-activate the mods it requires.
    - param `possibleGenerators`: Available terrain generators; if the stored generator is not among them, the last one is used.
    - param `initial`: If set, a new random seed is generated.
    - param `autoActivateMods`: If set (or `initial` is set), active mods that are no longer installed or are invisible are removed, and mods required by the config dictionary or marked `autoActivate` are added.
    - returns: The initial states. Missing base-game parameters get their default index.
- `searchBuildAndAddScriptParamComp : function(settings : {NewGameReactUtil.SettingsEntry}, activeModsParamsState : ReactStateT<{string : {string : integer}}>, key : string, addSpacer? : boolean, filterTags? : {string}, otherKeysToSet? : {string}) : TreeNodeId`  (call as `new_game_react_util.searchBuildAndAddScriptParamComp(…)`): Builds the control for a base-game script parameter and appends it to `settings`.
    - param `settings`: List the entry is appended to.
    - param `activeModsParamsState`: Parameter state; the base-game values (`[""]`) are read and written.
    - param `key`: Key of the script parameter, for example `"map.size"`.
    - param `addSpacer`: Whether to add a spacer to the control.
    - param `filterTags`: Tags used to select the platform-specific variant of the parameter (see `getFilterTags`).
    - param `otherKeysToSet`: Further parameter keys that are set to the same value index.
    - notes: Nothing is added if the parameter is not found.
- `addNamesSettingsEntry : function(settings : {NewGameReactUtil.SettingsEntry}, configDictState : ReactStateT<{{string, string}}>)`  (call as `new_game_react_util.addNamesSettingsEntry(…)`): Adds the "Names" setting, a combo box of all name lists, which sets the `nameList` entry of the config dictionary.
  In game: "Names", "Change the region from which the names of towns, roads and residents should originate." (German: „Namen“, „Ändere die Region, aus der die Namen von Städten, Strassen und Einwohnern stammen sollen.“)
- `addStartYearSettingsEntry : function(settings : {NewGameReactUtil.SettingsEntry}, mapState : ReactStateT<GameMap>, mapParamsState : ReactStateT<NewGameReactUtil.NewGameState>)`  (call as `new_game_react_util.addStartYearSettingsEntry(…)`): Adds the "Start Year" slider, which sets `startYearIndex` of the map parameters.
  In game: "Start Year", "The start year affects the appearance of towns and which vehicles are available." (German: „Startjahr“, „Das Startjahr beeinflusst das Erscheinungsbild der Städte und die verfügbaren Fahrzeuge.“)
    - notes: Changing the start year does not reset the map preview.
- `addTutorialSettingsEntry : function(settings : {NewGameReactUtil.SettingsEntry}, activeModsParamsState : ReactStateT<{string : {string : integer}}>, filterTags : {string})`  (call as `new_game_react_util.addTutorialSettingsEntry(…)`): Adds the control for the script parameter `guideSystemConfig.tutorial`.
- `addSeedSettingsEntry : function(settings : {NewGameReactUtil.SettingsEntry}, mapState : ReactStateT<GameMap>, mapParamsState : ReactStateT<NewGameReactUtil.NewGameState>)`  (call as `new_game_react_util.addSeedSettingsEntry(…)`): Adds the "Seed" text field with a button that generates a random seed.
  In game: "Seed", "Change the random seed to generate completely different maps." (German: „Zufallswert“, „Ändere den Startwert, um völlig unterschiedliche Karten zu generieren.“)
    - notes: Changing the seed sets `mapState` to `nil`, so the map preview is generated again.
- `addMapSizeSettingsEntry : function(settings : {NewGameReactUtil.SettingsEntry}, activeModsParamsState : ReactStateT<{string : {string : integer}}>, filterTags? : {string})`  (call as `new_game_react_util.addMapSizeSettingsEntry(…)`): Adds the control for the script parameter `map.size`.
- `addMapFormatSettingsEntry : function(settings : {NewGameReactUtil.SettingsEntry}, activeModsParamsState : ReactStateT<{string : {string : integer}}>, filterTags? : {string})`  (call as `new_game_react_util.addMapFormatSettingsEntry(…)`): Adds the control for the script parameter `map.format`.
- `addInitialWeatherSettingsEntry : function(settings : {NewGameReactUtil.SettingsEntry}, activeModsParamsState : ReactStateT<{string : {string : integer}}>, filterTags? : {string})`  (call as `new_game_react_util.addInitialWeatherSettingsEntry(…)`): Adds the control for the script parameter `weatherConfig.dynamicWeather`.
- `addInitialTimeOfDaySettingsEntry : function(settings : {NewGameReactUtil.SettingsEntry}, activeModsParamsState : ReactStateT<{string : {string : integer}}>, filterTags? : {string})`  (call as `new_game_react_util.addInitialTimeOfDaySettingsEntry(…)`): Adds the control for the script parameter `gameTimeConfig.timeOfDayMode`.
- `addGeneratorSettingsEntry : function(settings : {NewGameReactUtil.SettingsEntry}, possibleGenerators : {NewGameReactUtil.Generator}, mapState : ReactStateT<GameMap>, mapParamsState : ReactStateT<NewGameReactUtil.NewGameState>)`  (call as `new_game_react_util.addGeneratorSettingsEntry(…)`): Adds the "Generator" combo box for choosing the terrain generator. Nothing is added if there is only one generator.
  In game: "Generator", "This setting controls which generator is used to generate the terrain." (German: „Generator“, „Wähle den Generator zur Erstellung des Geländes.“)
    - notes: Changing the generator sets `mapState` to `nil`, so the map preview is generated again.
- `addTerrainParameterSettingsEntry : function(settings : {NewGameReactUtil.SettingsEntry}, mapState : ReactStateT<GameMap>, mapParamsState : ReactStateT<NewGameReactUtil.NewGameState>, mapGenParamsState : ReactStateT<{string : integer}>)`  (call as `new_game_react_util.addTerrainParameterSettingsEntry(…)`): Adds one control per parameter of the selected terrain generator; values are stored in `mapGenParamsState`.
    - notes: Changing a value sets `mapState` to `nil`, so the map preview is generated again.
- `getYearValues : function() : {integer}`  (call as `new_game_react_util.getYearValues(…)`): Returns the selectable start years.
    - returns: The years 1900 to 2020 in steps of 10.
- `getYearScriptParam : function() : ScriptParam`  (call as `new_game_react_util.getYearScriptParam(…)`): Returns the script parameter for the start year slider.
  In game: "Start Year" (German: „Startjahr“)
    - returns: A slider parameter with key `startYear`, values `"1900"` to `"2020"` in steps of 10 and default index 1.
- `getInitialTimeOfDayValues : function() : {integer}`  (call as `new_game_react_util.getInitialTimeOfDayValues(…)`): Start time of day for each value of the `gameTimeConfig.timeOfDayMode` parameter.
    - returns: Seconds since midnight, indexed like the parameter values.
- `getInitialCloudCoverageValues : function() : {number}`  (call as `new_game_react_util.getInitialCloudCoverageValues(…)`): Initial cloud coverage for each value of the `weatherConfig.dynamicWeather` parameter.
    - returns: Cloud coverage from 0 to 1, indexed like the parameter values.
- `calculateSeedString : function(maxLen : integer) : string`  (call as `new_game_react_util.calculateSeedString(…)`): Generates a random seed of letters and digits.
    - param `maxLen`: Maximum length; the base game passes 10, which gives a 10-character seed.
    - used 3x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:254`: `setSeed(new_game_react_util.calculateSeedString(10))`
- `getFilterTags : function() : {string}`  (call as `new_game_react_util.getFilterTags(…)`): Returns the tags used to select script parameter variants for the current platform.
    - returns: The tags of `getPlatformFilterTags`, plus `"experimentalMapFeatures"` if that app config option is enabled.
    - used 2x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:2112`: `local filterTags = new_game_react_util.getFilterTags()`
- `getPlatformFilterTags : function() : {string}`  (call as `new_game_react_util.getPlatformFilterTags(…)`): Returns the platform tag for script parameter variants.
    - returns: `{"console"}` on consoles, `{"desktop"}` on desktop.
- `findConfigDictIndex : function<K, V>(dict : {{K, V}}, key : K) : integer`  (call as `new_game_react_util.findConfigDictIndex(…)`): Finds the position of a key in a config dictionary (a list of `{key, value}` pairs).
    - returns: The index of the pair, or `0` if the key is not present.

### NewGameReactUtil.Generator (record)

```teal
record Generator
```

A terrain generator offered for a new map.

- `name : string`: Resource name of the terrain generator.
- `order : integer`: Sort order from the generator definition.

### NewGameReactUtil.MapConfig (record)

```teal
record MapConfig
```

Result of `gatherMapConfig`.

- `possibleGenerators : {Generator}`: Terrain generators available for the climate, sorted by `order`.
- `climateDesc : ClimateDesc`: Description of the selected climate.

### NewGameReactUtil.StatesBundle (record)

```teal
record StatesBundle
```

Initial values of the new game page states, returned by `prepareState`.

- `mapParamsState : NewGameReactUtil.NewGameState`: Start year, terrain generator and seed.
- `mapGenParamsState : {string : integer}`: Selected value index per terrain generator parameter key.
- `activeModsState : {string}`: Names of the active mods.
- `activeModsParamsState : {string : {string : integer}}`: Selected value index per script parameter key, per mod name (`""` is the base game).

### NewGameReactUtil.NewGameState (record)

```teal
record NewGameState
```

Basic settings of a new map; stored in the app config as `mainMenuState.newGameState`.

- `startYearIndex : integer`: Index into `getYearValues()`.
- `terrainGenerator : string`: Resource name of the selected terrain generator.
- `seed : string`: Seed string for the map generator.

### NewGameReactUtil.SettingsEntry (record)

```teal
record SettingsEntry
```

One row of the settings list of the new game page.

- `title : string`: Title of the setting.
- `description : string`: Description shown in the hint panel when the row is hovered or focused.
- `hintIdKey : string`: Key that links the row to its hint; the helpers use `"hintIdKey" .. title`.
- `element : TreeNodeId`: The control of the setting.
