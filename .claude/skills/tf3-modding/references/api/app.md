# app

Definition file: `api/tealdef/app.d.tl` (in the game folder).

Module controlling high level functions for the application, can be used to manipulate save games.

## App (record)

```teal
global record App
```
Access: `app`

Type of the global `app` object, which controls the application (save games, load progress, user profile, mods).

- `res : Res`: The application level resource repositories.
- `quit : function(instant : boolean)`  (call as `app.quit(…)`): Quit the application.
    - param `instant`: immediately quit one exit
- `restart : function()`  (call as `app.restart(…)`): Restart the application
- `printMemoryStats : function()`  (call as `app.printMemoryStats(…)`): Prints memory statistics to the log output.
- `startGame : function(startGameParams? : StartGameParams)`  (call as `app.startGame(…)`): Starts a new test game by specifying some basic parameters
    - param `startGameParams`: start game parameters (optional) - if not set, the game will start with default values
- `startGame2 : function(map : GameMap, seed : string, startDate : Date, startTimeOfDaySeconds : integer, startCloudCoverage : number, isMapEditor : boolean, mods : {Mod.ModId}, configDict : {{string, string}}, modParams : {string : {string : integer}})`  (call as `app.startGame2(…)`): Starts a new game
    - param `map`: the map to use
    - param `seed`: the seed use for the map generation
    - param `startDate`: the start date
    - param `startTimeOfDaySeconds`: is the start time of the day in seconds
    - param `startCloudCoverage`: is the start cloud coverage
    - param `isMapEditor`: starting from map editor
    - param `mods`: list of mods to start the game with
    - param `configDict`: list of game settings
    - param `modParams`: list of mod params
- `stopGame : function()`  (call as `app.stopGame(…)`): Stops the current game.
- `findAllSavegames : function(namespace : string) : {SaveGameInfo}`  (call as `app.findAllSavegames(…)`): Obtain list of all savegames.
    - param `namespace`: from which to obtain information (see app.SaveGameNamespace.getSavemap() or app.SaveGameNamespace.getSavegame() for the namespaces)
    - returns: a list of all savegame
- `loadGame : function(saveId : SaveGameId, isMapEditor : boolean, info? : SaveGameData.SaveGameDetails, isTutorialInit? : boolean)`  (call as `app.loadGame(…)`): Loads a savegame
    - param `saveId`: the savegame id
    - param `isMapEditor`: if the savegame is loaded in the map editor
    - param `info`: savegame info (optional)
    - param `isTutorialInit`: if the savegame is loaded as a tutorial init (optional)
- `loadMission : function(saveId : SaveGameId, campaign : string, mission : string)`  (call as `app.loadMission(…)`): Loads a mission
    - param `saveId`: the savegame id
    - param `campaign`: campaign name
    - param `mission`: mission name
- `saveGame : function(name : string, callBack : function(), isMapEditor : boolean, skipSetName? : boolean)`  (call as `app.saveGame(…)`): Saves the current game.
    - param `name`: the name of the savegame
    - param `callBack`: callback when the savegame is finished
    - param `isMapEditor`: is saving from the map editor
    - param `skipSetName`: skips setting the savegame name as the current default savegame name (optional)
- `parseSavegameName : function(savegameName : string) : ParsedSavegameName`  (call as `app.parseSavegameName(…)`): Parse savegame info from its file name
    - param `savegameName`: the name of the savegame
    - returns: The info (most notably, savegame type and group name)
- `getSavegameComplexity : function(saveGameDetails : SaveGameData.SaveGameDetails) : integer`  (call as `app.getSavegameComplexity(…)`): Get an estimated value of the savegame complexity
    - param `saveGameDetails`: the savegame data of the savegame
    - returns: a value that represents the estimate size of the savegame
- `getSavegameComplexityLimit : function() : integer`  (call as `app.getSavegameComplexityLimit(…)`): Get an limit for the savegame complexity for the current platform
    - returns: the complexity limit for the current platform
- `getSavegameInfo : function(saveId : SaveGameId) : Async<SaveGameData>`  (call as `app.getSavegameInfo(…)`): Loads savegame metadata asynchronously.
    - param `saveId`: the savegame id
    - returns: an async object with the savegame data
- `getSavegameImage : function(saveId : SaveGameId) : GameScreenshot | nil`  (call as `app.getSavegameImage(…)`): Obtains the savegame preview image
Note: This is - usually, if not explicitly changed in userdata - the same image as GameMetadata.screenshot but with the full available resolution
    - param `saveId`: the savegame id
    - returns: the image, or nil if no preview image is available
- `deleteSavegame : function(saveId : SaveGameId) : boolean`  (call as `app.deleteSavegame(…)`): Deletes a savegame
    - param `saveId`: the savegame to delete
    - returns: if the savegame was successfully deleted
- `getUserProfile : function() : UserProfile`  (call as `app.getUserProfile(…)`): Obtain the user profile.
    - returns: the user profile
- `findBuiltinSavegames : function(isMapSave : boolean) : {SaveGameInfo}`  (call as `app.findBuiltinSavegames(…)`): Obtains a list of built-in savegames.
    - param `isMapSave`: true to list built-in maps, false to list built-in savegames
    - returns: list of savegames
- `getSaveGameStorageInfo : function() : SaveGameStorageInfo`  (call as `app.getSaveGameStorageInfo(…)`): Returns the savegame storage infos
    - returns: savegame storage info
- `getSupportedMSAASampleCounts : function() : {integer}`  (call as `app.getSupportedMSAASampleCounts(…)`): Returns the supported MSAA sample counts
    - returns: list of supported sample counts
- `loadUserdata : function(directory : string, fileName : string) : table`  (call as `app.loadUserdata(…)`): Load a custom userdata file
    - param `directory`: a folder in the userdata directory
    - param `fileName`: a lua filename (without .lua extension)
    - returns: the deserialized lua data
- `saveUserdata : function(directory : string, fileName : string, table : table)`  (call as `app.saveUserdata(…)`): Saves a custom table as lua file.
    - param `directory`: a folder in the userdata directory
    - param `fileName`: the filename to save to (without .lua extension)
    - param `table`: the table to serialize
- `removeUserdata : function(directory : string, fileName : string)`  (call as `app.removeUserdata(…)`): Removes a userdata.
    - param `directory`: a folder in the userdata directory
    - param `fileName`: the filename to remove (without .lua extension)
- `getAllUserdata : function(directory : string) : {string}`  (call as `app.getAllUserdata(…)`): Returns a list of all saved mod preset names.
    - param `directory`: a folder in the userdata directory
    - returns: a list of mod preset names
- `playVoiceOverOrStop : function(resName : string)`  (call as `app.playVoiceOverOrStop(…)`): Plays a voice over if not currently playing, otherwise stops it.
    - param `resName`: the file to play
- `makeEmptyMap : function(climate : string, numTiles : Vec2i, waterLevel : number) : GameMap`  (call as `app.makeEmptyMap(…)`): Creates an empty map (for tooling / map generation workflows).
    - param `climate`: climate resource name
    - param `numTiles`: map dimension in tiles
    - param `waterLevel`: water level height
    - returns: the map
- `getInputActionRep : function() : InputActionRep`  (call as `app.getInputActionRep(…)`): Returns the global input action repository.
    - returns: input action repository
- `refreshMods : function()`  (call as `app.refreshMods(…)`): Refreshes the installed mods
- `setWaitForStartReadyGame : function()`  (call as `app.setWaitForStartReadyGame(…)`): Wait for the game to be ready to start. This is called when loading a game to prevent the game from starting automatically
- `isWaitForStartReadyGame : function() : boolean`  (call as `app.isWaitForStartReadyGame(…)`): Return if the `setWaitForStartReadyGame` had been called for this game.
- `startReadyGame : function()`  (call as `app.startReadyGame(…)`): Start Game that is fully loaded and ready to play. This is called after the progress page has finished loading the game.
- `getRandomTips : function() : {string}`  (call as `app.getRandomTips(…)`): Gets the list of random tips
    - returns: a list of tips as string
- `getModTagTranslation : function(tag : string) : string`  (call as `app.getModTagTranslation(…)`): Returns a translated name for a mod tag.
    - param `tag`: tag to translate
    - returns: the translated tag
- `getProgressMonitor : function() : ProgressMonitor`  (call as `app.getProgressMonitor(…)`): Gets the load progress monitor
- `openUserDataFolder : function()`  (call as `app.openUserDataFolder(…)`): Opens the user data folder in the OS file manager
- `openUserDataSubFolder : function(directory : string)`  (call as `app.openUserDataSubFolder(…)`): Opens the heightmaps folder in the OS file manager
    - param `directory`: subfolder
- `getUserDataFolder : function() : string`  (call as `app.getUserDataFolder(…)`): Gets the directory containing userdata
- `getAllHeightmaps : function() : {string}`  (call as `app.getAllHeightmaps(…)`): Gets a list of all heightmaps in the userdata directory
    - returns: list of heightmaps files
- `getAllBiomes : function() : {string}`  (call as `app.getAllBiomes(…)`): Gets a list of all biomes in the userdata directory
    - returns: list of biomes files
- `openBiomesFolder : function()`  (call as `app.openBiomesFolder(…)`): Opens the biomes folder
- `getBiomesFolder : function() : string`  (call as `app.getBiomesFolder(…)`): Gets the path to the biome folder
    - returns: path to biome folder
- `getGPUCapabilities : function() : GPUCapabilities`  (call as `app.getGPUCapabilities(…)`): Get information about the GPU
- `writeHeightmap : function(map : GameMap, path : string, writeStl : boolean, sixteenBits : boolean, biomesData : boolean) : boolean`  (call as `app.writeHeightmap(…)`): Writes a heightmap image (and optionally an STL) for a map.
    - param `map`: the map to write
    - param `path`: the relative path (extension will be added if missing)
    - param `writeStl`: write as an STL file as well
    - param `sixteenBits`: use 16 bits depth for the heightmap (default 8 bits depth)
    - param `biomesData`: if biomes data should as well be exported (will be written in the biomes folder)
    - returns: if the heightmap was successfully written
- `showNodeEditor : function()`  (call as `app.showNodeEditor(…)`): Shows the node editor.
- `debug : Debug`: The `App.Debug` helpers (`app.debug`).

### App.ProgressMonitor (record)

```teal
record ProgressMonitor
```

Progress of the current loading task, returned by `app.getProgressMonitor()`.

Example:
```lua
-- from gui/menu/progress_page.tl
local monitor = app.getProgressMonitor()
return {monitor:getProgress(), monitor:getTask()}
```

- `getProgress : function(ProgressMonitor) : number`: Returns the current progress as a value from 0 to 1
- `getTask : function(ProgressMonitor) : string`: Returns the current task being executed

### App.Res (record)

```teal
record Res
```

Application level resource repositories.
Note: climateRep, economyRep and terrainGeneratorRep are only populated in the main menu,
in the game UI use the corresponding repositories in `api.res`.

- `climateRep : ResTypeRep<integer, ClimateDesc>`: A resource repository containing data for the climates.
- `economyRep : ResTypeRep<integer, EconomyDesc>`: A resource repository containing data from the ".eco" economy resources.
- `terrainGeneratorRep : ResTypeRep<integer, TerrainGeneratorDesc>`: A resource repository containing data for the terrain generation.
- `playlistRep : ResTypeRep<integer, PlaylistDesc>`: A repository for playlists.
- `nameRep : ResTypeRep<integer, NameDesc>`: A resource repository for names.
- `campaignRep : ResTypeRep<integer, CampaignDesc>`: A resource repository for campaign chapters.
- `missionRep : ResTypeRep<integer, MissionDesc>`: A resource repository for campaign missions.

### App.SaveGameNamespace (record)

```teal
record SaveGameNamespace
```

Savegame namespace constants.

- `getUserdata : function() : string`: Returns the userdata savegame namespace.
- `getSavegame : function() : string`: Returns the savegame namespace.
- `getSavemap : function() : string`: Returns the savemap namespace.

### App.Debug (record)

```teal
record Debug
```

Debug helpers for gamepad input (forced scroll repeat speed, injected button events).

- `setForcedGamepadDynamicScrollRepeatSpeed : function(speed? : integer)`: Overrides gamepad dynamic scroll repeat speed.
    - param `speed`: the scroll speed (0: 1/sec, 3: max speed), or nil to clear the override
- `pushGamepadButtonKeyInputEvent : function(gamepadButtonName : string, press : boolean)`: Pushes a gamepad button event into the input queue.
    - param `gamepadButtonName`: the button to push
    - param `press`: if pressed
