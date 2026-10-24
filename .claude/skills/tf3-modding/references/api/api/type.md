# api.type

Definition file: `api/tealdef/api/type.d.tl` (in the game folder).

Contains game command objects and related utilities.

## Globals

- `Engine` (type)
- `JournalEntry` (type): One finance journal booking (`api.type.JournalEntry`). Create it with `api.type.JournalEntry.new()`,
set `amount`, `time` and `category`, and book it with `api.cmd.makeJournalBookAssetCmd`.
- `CargoType` (type)
- `LayerConfig` (type)
- `Transformator` (type)
- `Mod` (type): Forward declaration of the `Mod` record, which is defined in `api/type/mod.d.tl`.
- `ModId` (type): Global alias of `integer`. It is the type of `SaveGameInfo.modId`; mod identifiers used by the mod repository are `Mod.ModId` records instead.
- `Modhub` (type)
- `TrackType` (type): Opaque type without declared members; it is exposed as `api.type.TrackType`.
- `ResName : string`: Resource name of a game resource file, such as `"::/climates/dry/dry.clima"` or `"industry/coal_mine.con"`.
- `FilePath : string`: Path to a file (for example an icon `.tga` or a sound file), as a string.
- `Index : integer`: Integer index, for example the default value index of a script parameter (`ScriptParam.defaultIndex`).
- `CargoTypeId : integer`: Integer id of a cargo type in `api.res.cargoTypeRep`.
- `CargoClassId : integer`: Integer id of a cargo class, as returned by `api.res.cargoClassRep.getCargoClassId`.
- `ConstructionId : integer`: Integer id of a construction resource in `api.res.constructionRep`.
- `StockId : integer`: Integer id of one stock within a stock holding entity (for example an industry); used together with that entity, e.g. in `api.engine.system.simEntityAtStockSystem.getStockCount`.
- `App` (type): Type of the global `app` object, which controls the application (save games, load progress, user profile, mods).
- `NativeLuaValue : string | number | boolean | nil | NativeLuaTable`: Value stored in a `NativeLuaTable`: a string, number, boolean, `nil` or a nested `NativeLuaTable`.
- `pairs_native : function<K, V>(Map<K, V>) : (function() : K, V)`: Global iterator like `pairs` for a native `Map`: walks the keys with `Map.next` and returns each value with `Map.find`.
    - param `#1`: Native map to iterate.
    - returns: Iterator yielding each key and value.

    ```lua
    for k, v in pairs_native(map) do
    	print(k, v)
    end
    ```

- `pairs_native_lua_table : function(NativeLuaTable) : (function() : NativeLuaValue, NativeLuaValue)`: Global iterator like `pairs` for a `NativeLuaTable`: walks the keys with `NativeLuaTable.next` and returns each value with `NativeLuaTable.find`.
    - param `#1`: Native Lua table to iterate.
    - returns: Iterator yielding each key and value as `NativeLuaValue`.

    ```lua
    -- subvention_util.tl: find a subsidy by uid in the game script's native state
    for __, subventionValue in pairs_native_lua_table(subventionArray as NativeLuaTable) do
        local subventionTable = subventionValue as NativeLuaTable
        if subventionTable:find("uid") == subventionUID then
            return subventionTable:asTable() as ISubvention, subventionStatus
        end
    end
    ```

- `GlobalType : Type`: Global alias of `Type`, the type namespace reached as `api.type`.

## orderedPairs (record)

```teal
global record orderedPairs<T, V>
```

Global iterator like `pairs`, but returning the keys of a Lua table in sorted order (implemented in `base/init.lua`).

Example:
```lua
for key, value in orderedPairs(t) do
	print(key, value)
end
```

- `__call : (function({T : V}, any) : function() : T, V)`  (call as `orderedPairs(…)`): Call form `orderedPairs(t)`: returns an iterator over the table `t` that visits its keys in `table.sort` order.
    - notes: While iterating, a temporary `__orderedIndex` field is stored in the table and removed again when the iteration ends.

## Vector (record)

```teal
global record Vector<T>
```

Native (C++) vector exposed to Lua, used for the `*_native` fields of game objects. Index it with `at` (1-based) or iterate it with `ipairs_native`; `asTable` converts it to a Lua array.

- `new : function() : Vector<T>`: Creates an empty vector.
- `at : function(v : Vector<T>, i : integer) : T`  (call as `vector:at(…) (method)`): 1-base index into a standard C++ vector
    - used 15x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1298`: `local baseEdge = getBaseEdgeFromProposal(rangeGroup:at(1).edge)`
- `pushBack : function(v : Vector<T>, t : T)`: Appends `t` to the end of the vector.
- `size : function(v : Vector<T>) : integer`  (call as `vector:size(…) (method)`): Returns the number of elements.
    - used 11x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1308`: `if rangeGroup:size() == 1 then`
- `asTable : function(v : Vector<T>) : {T}`: Returns a copy of the elements as a Lua array.
- `clone : function(v : Vector<T>) : Vector<T>`: Returns a copy of the vector.
    - notes: The base game uses it to keep native image data, e.g. `savegameData.info.metadata.screenshot.image_native:clone()`.

## ipairs_native (record)

```teal
global record ipairs_native<T>
```

Global iterator like `ipairs` for a native `Vector`: iterates the elements in order via `Vector.at` until it returns `nil`.

Example:
```lua
for i, v in ipairs_native(vec) do
	print(i, v)
end
```

- `__call : function(Vector<T>) : function() : integer, T`  (call as `ipairs_native(…)`): Call form `ipairs_native(vec)`: returns an iterator yielding the 1-based index and the element of each entry of `vec`.

## Set (record)

```teal
global record Set<T>
```

Native (C++) set exposed to Lua.

- `contains : function(s : Set<T>, t : T) : boolean`: Returns whether `t` is in the set.
- `insert : function(s : Set<T>, t : T) : boolean`: Adds `t` to the set; returns a boolean result.
- `size : function(s : Set<T>) : integer`: Returns the number of elements.
- `asTable : function(s : Set<T>) : {T : boolean}`: Returns the set as a Lua table mapping each element to `true`.

## Map (record)

```teal
global record Map<K, V>
```

Native (C++) map exposed to Lua. Look values up with `find`, or iterate it with `pairs_native`; `asTable` converts it to a Lua table.

- `find : function(m : Map<K, V>, k : K) : V`  (call as `map:find(…) (method)`): Returns the value stored for key `k`.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notification_util.tl:306`: `local notificationId = history:find(i)`
- `insert : function(m : Map<K, V>, k : K, v : V)`: Stores value `v` under key `k`.
- `size : function(m : Map<K, V>) : integer`  (call as `map:size(…) (method)`): Returns the number of entries.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notification_util.tl:305`: `for i = 1, history:size() do`
- `asTable : function(m : Map<K, V>) : {K : V}`: Returns the entries as a Lua table.
- `next : function(k : K) : K`: Returns the key that follows `k`, or `nil` after the last key; `nil` as `k` returns the first key.
    - notes: `pairs_native` calls it as a method (`map:next(k)`), although the declared signature lists only the key.

## NativeLuaTable (record)

```teal
global record NativeLuaTable
```

Lua table data held natively by the engine, for example the state of a game script (`state_native` of a `GAME_SCRIPT` component). Read single entries with `find`/`findPath` or convert it with `asTable`.

Example:
```lua
local data = scriptComp.state_native:findPath({"townState", entity})
if data then
	local townData = data:asTable()
end
```

- `find : function(m : NativeLuaTable, k : NativeLuaValue) : NativeLuaValue`  (call as `nativeLuaTable:find(…) (method)`): Returns the value stored under key `k`.
    - used 21x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:334`: `local notifications = (notificationsState:find("notifications") as Map<integer, NotificationsState.Entry>):asTable()`
- `findPath : function(m : NativeLuaTable, k : {NativeLuaValue}) : NativeLuaValue`  (call as `nativeLuaTable:findPath(…) (method)`): Follows a list of keys through nested tables and returns the value at the end of the path.
    - notes: Example from the base game: `townStateNative:findPath({"townState", entity})` returns the entry `state.townState[entity]` without converting the whole state.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_cargo_util.tl:69`: `local townDataNative = townStateNative:findPath({"townState", entity}) as NativeLuaTable`
- `insert : function(m : NativeLuaTable, k : NativeLuaValue, v : any)`: Stores the Lua value `v` under key `k`.
- `insert_native : function(m : NativeLuaTable, k : NativeLuaValue, v : NativeLuaValue)`: Stores the native value `v` under key `k`.
- `remove : function(m : NativeLuaTable, k : NativeLuaValue)`: Removes the entry with key `k`.
- `size : function(m : NativeLuaTable) : integer`  (call as `nativeLuaTable:size(…) (method)`): Returns the number of entries.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notification_util.tl:310`: `if persisting ~= nil and persisting:size() > 0 then`
- `asTable : function(m : NativeLuaTable) : table`  (call as `nativeLuaTable:asTable(…) (method)`): Converts the native table to a plain Lua table.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:478`: `local activeSubvention = subventionTable:asTable() as Subvention<any>`
- `clone : function(m : NativeLuaTable) : NativeLuaTable`: Returns a copy of the native table.
- `next : function(m : NativeLuaTable, k? : NativeLuaValue) : NativeLuaValue`  (call as `nativeLuaTable:next(…) (method)`): Returns the key that follows `k` (the first key when `k` is omitted), for iterating the table.
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:4160`: `local key = modulesTable:next()`

## SaveGameId (record)

```teal
global record SaveGameId
```
Access: `api.type.SavegameId`

Savegame identifier with path and namespace

- `new : function() : SaveGameId`  (call as `api.type.SavegameId.new(…)`): Creates a new, empty `SaveGameId`.
    - used 9x in the base game, e.g. `base/content/gui/gui/menu/main_page.tl:649`: `local saveId = api.type.SavegameId.new()`
- `path : string`: Folder of the savegame when it is not one of the player's own saves (e.g. a savegame shipped with a mod); empty otherwise. The load menu treats a non-empty `path` as a mod savegame.
- `saveGameName : string`: Name of the savegame (`SaveGameInfo.saveName`).
- `saveGameNamespace : string`: Namespace the savegame belongs to, e.g. `app.SaveGameNamespace.getSavegame()` or `app.SaveGameNamespace.getSavemap()`.

## PlatformDeviceSpec (record)

```teal
global record PlatformDeviceSpec
```

Platform device specifications

- `gen : ConsoleGen`: Console generation of the device; `PC` on computers.
- `resolution : Vec2i`: Screen resolution of the device, in pixels.

## GPUCapabilities (record)

```teal
global record GPUCapabilities
```

Information about the graphics card, returned by `app.getGPUCapabilities()`.

- `hasDiscreteGPU : boolean`: Whether a discrete (dedicated) graphics card is available.
- `hasUnsupportedGPU : boolean`: Whether the graphics card is unsupported; the main menu then shows an unsupported-hardware warning (when `showIntelHDwarning` is enabled in the app config).

## GraphicsPresetParam (record)

```teal
global record GraphicsPresetParam
```

Hardware data returned by `api.util.getGraphicsPresetParam()`, from which `GraphicsPresetsUtil.getInitialGraphicsPresetDesktop` picks the initial desktop graphics preset.

- `vram : integer`: Video memory of the graphics card. The preset choice compares it with 7000 (medium), 11000 (high) and 15000 (very high). The game files don't name the unit; megabytes would match cards with 8, 12 and 16 GB.
- `hasDiscreteGPU : boolean`: Whether a dedicated graphics card is present. Without one the initial preset is low, whatever `vram` says.
- `screenExtent : {integer, integer}`: Screen width and height in pixels. The height sets the initial `resolutionScale` (0.75 from 2160, down to 1.0 below 1440).

## GameScreenshot (record)

```teal
global record GameScreenshot
```

Game screenshot data

- `width : integer`: Screenshot width in pixels
- `height : integer`: Screenshot height in pixels
- `image : {integer}`: Raw image data
- `image_native : Vector<integer>`: Native vector of image data

## CompanyValue (record)

```teal
global record CompanyValue
```

A structure that holds information about the company's value

- `totalScore : integer`: Total score of the company.
- `railVehicles : integer`: Number of rail vehicles (trains) the company owns.
- `trams : integer`: Number of trams the company owns.
- `roadVehicles : integer`: Number of road vehicles the company owns.
- `aircrafts : integer`: Number of aircraft the company owns.
- `ships : integer`: Number of ships the company owns.
- `trackTotalLength : integer`: Total length of the company's tracks, in metres.
- `trackElectricLength : integer`: Total length of electrified track, in metres.
- `bridgeTotalLength : integer`: Total length of bridges, in metres.
- `tunnelTotalLength : integer`: Total length of tunnels, in metres.
- `roadTotalLength : integer`: Total length of roads, in metres.
- `suppliedTowns : integer`: Number of supplied towns.
- `connectedIndustries : integer`: Number of connected industries.
- `numberOfLines : integer`: Number of lines.
- `totalStations : integer`: Total number of stations.
- `railStations : integer`: Number of train stations.
- `tramStations : integer`: Number of tram stations.
- `roadStations : integer`: Number of road stations.
- `aircraftStations : integer`: Number of airports and heliports.
- `shipStations : integer`: Number of ports.
- `topSpeed : number`: Highest vehicle speed ("Top Speed" in the finance window), in metres per second.
- `topLength : number`: Length of the longest train, in metres.
- `oldestTransportVehicle : integer`: Game time stamp used to compute the age of the oldest vehicle (passed to `api.engine.util.formatAge` with the current game time); `-1` if there is none.
- `balance : integer`: Current account balance, in game currency.
- `totalAssets : integer`: Value of the company's assets, in game currency; the finance chart computes the total company value as `totalAssets + balance - debt`.
- `debt : integer`: Outstanding debt, in game currency.

## TransportationStats (record)

```teal
global record TransportationStats
```

A structure that holds information about the transportation data statistics.

- `passengersTransported : integer`: Number of passengers transported.
- `cargoTransported : integer`: Amount of cargo transported.

## SavegameScriptMetadata (record)

```teal
global record SavegameScriptMetadata
```

Marker for type-safe savegame metadata table

Opaque type of the metadata a game script stores in the savegame (returned from the `storeSavegameMetadata` event).

## GameMetadata (record)

```teal
global record GameMetadata
```

Game metadata from a savegame

- `date : integer`: In-game date of the savegame as an integer; the menu converts it with `api.type.Date.new(metadata.date)` for display.
- `balance : integer`: Player's account balance
- `population : integer`: Current world population
- `startYear : integer`: List of game mods used
Year the game was started
- `numVehicleModels : integer`: The number of vehicle models present in the savegame
- `terrainDimensions : Vec2i`: Map dimensions in meters
- `companyValue : CompanyValue`: Company statistics (`CompanyValue`) at the time of saving.
- `transportationStats : TransportationStats`: Transported passengers and cargo at the time of saving.
- `scriptMetadata : SavegameScriptMetadata`: Metadata written by game scripts into the savegame. Read it through a metadata accessor, e.g. `company_metadata.savegameScript.get(metadata.scriptMetadata)`.
- `gameModDescs : {Mod.GameModDesc}`: Descriptions of the mods used in the savegame.
- `achievementsEarnable : boolean`: Whether achievements can be earned
- `screenshot : GameScreenshot`: Screenshot of the game

## SaveGameStorageInfo (record)

```teal
global record SaveGameStorageInfo
```

Information about savegame storage capacity

- `totalSpace : number`: Total storage space available, in bytes
- `usedSpace : number`: Storage space currently used, in bytes

## SaveGameData (record)

```teal
global record SaveGameData
```
Access: `api.type.SaveGameData`

Savegame data with info and error message

- `new : function(SaveGameData) : SaveGameData`  (call as `api.type.SaveGameData.new(…)`): Creates a copy of the given savegame data.
    - used 8x in the base game, e.g. `base/content/gui/gui/menu/advanced_settings.tl:40`: `local copy = api.type.SaveGameData.new(gameplaySettingsPageParams.params.savegameNameState:old().savegameData)`
- `errorMsg : string`: Error message if loading failed (empty string otherwise)
- `info : SaveGameDetails`: Detailed information about the savegame; may be missing when loading failed (see `errorMsg`).

### SaveGameData.SaveGameDetails (record)

```teal
record SaveGameDetails
```
Access: `api.type.SaveGameDetails`

Detailed information about a savegame

- `new : function(SaveGameDetails) : SaveGameDetails`  (call as `api.type.SaveGameDetails.new(…)`): Creates a copy of the given details.
    - used 8x in the base game, e.g. `base/content/gui/gui/menu/advanced_settings.tl:41`: `local infoCopy = api.type.SaveGameDetails.new(gameplaySettingsPageParams.params.savegameNameState:old().savegameData.info)`
- `metadata : GameMetadata`: Metadata of the savegame (date, statistics, screenshot).
- `mods : {Mod.ModId}`: Ids of the mods active in the savegame.
- `configDict : {{string, string}}`: Game configuration as a list of `{key, value}` string pairs (for example `"climate"`, `"economy"`, `"nameList"`).
- `modParams : {string : {string : integer}}`: Parameter values per mod: mod name to parameter key to value. The base game's own settings are under the key `""`.
- `campaign : string`: Resource name of the campaign the savegame belongs to.
- `mission : string`: Resource name of the mission the savegame belongs to; empty for free games.

## SaveGameType (record)

```teal
global record SaveGameType
```

Kind of savegame, as derived from its file name by `app.parseSavegameName`.

- `Manual : SaveGameType`
- `Autosave : SaveGameType`
- `Crash : SaveGameType`
- `Exit : SaveGameType`

## ParsedSavegameName (record)

```teal
global record ParsedSavegameName
```

Savegame info extracted solely from its filename

- `type : SaveGameType`: Kind of savegame (manual, autosave, crash or exit save).
- `groupName : string`: Name of the group the savegame is listed under in the load menu.
- `ingameTimestamp : string`: only for type == "autosave"
- `fileTimestamp : string`: only for type == "crash"

## GameMap (record)

```teal
global record GameMap
```
Access: `api.type.Map`

Information about the map, for map generation purposes.

- `new : function() : GameMap`  (call as `api.type.Map.new(…)`): Creates a new, empty `GameMap`.
    - used 6x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:752`: `local mapCopy = api.type.Map.new(fullState:old().readyMap)`
- `new : function(?GameMap) : GameMap`  (call as `api.type.Map.new(…)`): Copy constructor

Creates a new, empty `GameMap`.
    - used 6x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:752`: `local mapCopy = api.type.Map.new(fullState:old().readyMap)`
- `setHeightMap : function(GameMap, integer, number)`  (call as `gameMap:setHeightMap(…) (method)`): Allocates the height map and fills it: the base game passes the number of height samples and an initial height, e.g. `map:setHeightMap(fullWidth * fullHeight, 5.0)`.
    - notes: In the base game `fullWidth = numTilesX * 2^levels + 1` and `fullHeight = numTilesY * 2^levels + 1`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/new_game_or_map_settings_page.tl:99`: `map:setHeightMap(fullWidth * fullHeight, 5.0)`
- `setBiomeMap : function(GameMap, integer, integer, integer)`  (call as `gameMap:setBiomeMap(…) (method)`): Allocates the biome map: the base game passes its width, its height and an initial value, e.g. `map:setBiomeMap(numTilesX * 2^levels, numTilesY * 2^levels, 0)`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/new_game_or_map_settings_page.tl:100`: `map:setBiomeMap(numTiles.x * dim, numTiles.y * dim, 0)`
- `heightmap : {number}`: Terrain height samples of the map.
- `numTilesX : integer`: numTilesX number of tiles (256 m x 256 blocks) in X direction
- `numTilesY : integer`: numTilesY number of tiles (256 m x 256 blocks) in Y direction
- `levels : integer`: Subdivision level of a tile: each tile has `2^levels` height samples per side (the base game uses `6`).
- `offsetZ : number`: Vertical offset of the terrain (the base game uses `-100`).
- `waterLevel : number`: Water level height of the map (the base game uses `0`).
- `climateId : string`: Resource name of the climate, e.g. `"::/climates/dry/dry.clima"`.
- `resolution : Vec3f`: Grid resolution as `(x, y, z)`; the base game uses `(4, 4, 0.05)`, i.e. 4 m between height samples, so 64 samples cover one 256 m tile.
- `towns : {GameMap.Town}`: Towns to place on the map.
- `industries : {GameMap.Industry}`: Industries to place on the map.
- `townConnections : {{integer, integer}}`: Pairs of towns to connect by road, each `{a, b}` with indices into `towns`.
- `trees : GameMap.MapTree`: Trees to place on the map.
- `assets : GameMap.MapAsset`: Assets to place on the map.

### GameMap.Town (record)

```teal
record Town
```
Access: `api.type.Map.Town`

Information about towns on a map

- `new : function() : Town`  (call as `api.type.Map.Town.new(…)`): Creates a new, empty `Town`.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:1745`: `local mapTown = api.type.Map.Town.new()`
- `pos : Vec2f`: Town placement position
- `name : string`: Town name
- `sizeFactors : {number}`: The size factor for each district
- `landUse2CargoNeeds : {LandUseType : {{string, number}}}`: Cargo categories per land use type (note: these are *not* CargoTypeId's)
- `existing : Engine.Entity`: The town entity if it already exists

### GameMap.Industry (record)

```teal
record Industry
```
Access: `api.type.Map.Industry`

Information about industries on a map

- `new : function() : Industry`  (call as `api.type.Map.Industry.new(…)`): Creates a new, empty `Industry`.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:1773`: `local mapIndustry = api.type.Map.Industry.new()`
- `pos : Vec2f`: Placement position of the industry.
- `angle : number`: Rotation of the industry (`0` if not specified).
- `onWater : boolean`: Whether the industry is placed on water.
- `fileName : string`: File name of the industry's construction.
- `tag : string`: Industry type tag, the `placementTag` of its construction (e.g. `"coal_mine"`, `"oil_platform"`).
- `existing : Engine.Entity`: The industry entity if it already exists

### GameMap.MapTree (record)

```teal
record MapTree
```
Access: `api.type.Map.MapTree`

Container for tree data to be placed on the map

- `add : function(GameMap.MapTree, string, Vec3f)`  (call as `api.type.Map.MapTree.add(…)`): Adds a tree to the map
    - param `category`: The tree category
    - param `position`: The world position

### GameMap.MapAsset (record)

```teal
record MapAsset
```
Access: `api.type.Map.MapAsset`

Container for asset data to be placed on the map

- `add : function(GameMap.MapAsset, string, Mat4f)`  (call as `api.type.Map.MapAsset.add(…)`): Adds an asset to the map
    - param `category`: The asset category
    - param `position`: The world transform

### GameMap.Tree (record)

```teal
record Tree
```
Access: `api.type.Map.Tree`

A single tree on the map

- `id : integer`: Tree model id
- `pos : Vec3f`: Tree position

### GameMap.Asset (record)

```teal
record Asset
```
Access: `api.type.Map.Asset`

A single asset on the map

- `id : integer`: Asset model id
- `transf : Mat4f`: Asset transform

## Async (record)

```teal
global record Async<T>
```

Asynchronous operation wrapper for background tasks

- `isCompleted : function(Async<T>) : boolean`: Returns whether the background task has finished, so that `get` will not block.
- `get : function(Async<T>) : T`: Block until the task is complete and then return its result
- `abort : function(Async<T>)`: Cancels the background task.
- `getProgressMonitor : function(Async<T>) : App.ProgressMonitor`: Returns a progress monitor for the task (`getProgress` from 0 to 1, `getTask`).

## SaveGameInfo (record)

```teal
global record SaveGameInfo
```
Access: `api.type.SaveGameInfo`

Information about a savegame

- `new : function() : SaveGameInfo`  (call as `api.type.SaveGameInfo.new(…)`): Creates a new SaveGameInfo.
    - returns: New SaveGameInfo
- `equals : function(SaveGameInfo, SaveGameInfo) : boolean`  (call as `api.type.SaveGameInfo.equals(…)`): Returns `true` when both `SaveGameInfo` values are equal.
- `size : integer`: File size in bytes
- `timestamp : integer`: Time the savegame was written, as a POSIX timestamp; the menus sort by it and format it with `api.util.formatPosixTimestamp`.
- `path : string`: Folder of the savegame when it is not one of the player's own saves; empty otherwise (copied to `SaveGameId.path`).
- `modId : ModId`: ModId of the mod providing this save
- `saveName : string`: Name of the savegame; `app.parseSavegameName` derives type and group from it.

## StartGameParams (record)

```teal
global record StartGameParams
```
Access: `api.type.StartGameParams`

Parameters for starting a new game

- `new : function() : StartGameParams`  (call as `api.type.StartGameParams.new(…)`): Constructor
- `numTiles : Vec2i`: Map size in tiles
- `climateGenerator : string`: Climate generator script resource name
- `terrainGenerator : string`: Terrain generator script resource name
- `economy : string`: Economy difficulty script resource name
- `mods : {string}`: List of mod identifiers to load
- `modParams : {string : {string : integer}}`: List of mod parameters
- `seed : string`: Random seed for map generation
- `startDate : Date`: Starting date for the game
- `startTimeOfDaySeconds : integer`: Starting time in seconds of the time
- `startCloudCoverage : number`: Starting cloud coverage
- `isMapEditor : boolean`: Whether to start the game in map editor mode
- `generateTowns : boolean`: Whether to generate towns
- `generateIndustries : boolean`: Whether to generate industries
- `generateAssets : boolean`: Whether to generate assets
- `locationsConfig : BaseConfig.Locations`: Configuration for the placement of industry and towns on max generation

## LogLevel (record)

```teal
global record LogLevel
```

LogLevel for logging

- `Critical : LogLevel`
- `Error : LogLevel`
- `Warning : LogLevel`
- `Message : LogLevel`
- `Verbose : LogLevel`

## DateFormat (record)

```teal
global record DateFormat
```

DateFormat used for formatting calendar entries

- `LONG : DateFormat`
    - used 1x in the base game, e.g. `base/content/scripts/scripts/util.tl:37`: `return api.util.formatDate(api.engine.util.getCalendarDate(timestamp), api.type.enum.DateFormat.LONG)`
- `SHORT : DateFormat`
- `MEDIUM : DateFormat`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:17`: `return unixTime and api.util.formatPosixTimestamp(unixTime, api.type.enum.DateFormat.MEDIUM) or _("N/A")`
- `FULL : DateFormat`
    - used 1x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:240`: `local time = api.util.formatDate(api.type.Date.now(), api.type.enum.DateFormat.FULL) .. " " .. api.util.formatTime(api.util.getLocalTime())`
- `FILENAME : DateFormat`
- `YEAR : DateFormat`: Only output year

## Carrier (record)

```teal
global record Carrier
```

Carrier types for journal entries

- `__index : function(Carrier, string) : Carrier`: Looks up a carrier by name, e.g. `api.type.enum.Carrier["RAIL"]`; returns `nil` for unknown names.
- `ROAD : Carrier`: Road vehicles (trucks, buses, cars)
- `RAIL : Carrier`: Rail vehicles (trains)
- `TRAM : Carrier`: Tram vehicles
- `OTHER : Carrier`: Other types of carriers
- `AIR : Carrier`: Aircraft (planes, helicopters)
- `WATER : Carrier`: Water vehicles (ships, boats)

## EdgeObjectType (record)

```teal
global record EdgeObjectType
```

Types of objects that can be placed on a BaseEdge

- `STOP_LEFT : EdgeObjectType`: Stop on the left side of the edge
- `STOP_RIGHT : EdgeObjectType`: Stop on the right side of the edge
- `SIGNAL : EdgeObjectType`: Signal controlling rail reservation

## VehicleState (record)

```teal
global record VehicleState
```

Current state of a vehicle's movement

- `RUNNING : VehicleState`: Vehicle is actively moving
- `STOPPING : VehicleState`: Vehicle is in the process of stopping (due to player / script interaction)
- `STOPPED : VehicleState`: Vehicle is completely stopped and must be un-stopped by player / script
- `ARRIVED : VehicleState`: Vehicle has arrived at its destination

## TransportVehicleState (record)

```teal
global record TransportVehicleState
```

Current state of TransportVehicle

- `IN_DEPOT : TransportVehicleState`: Vehicle is in a depot
- `EN_ROUTE : TransportVehicleState`: Vehicle is traveling between terminals
    - used 2x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_list_react_util.tl:541`: `if (tv.state == api.type.enum.TransportVehicleState.EN_ROUTE) or (tv.state == api.type.enum.TransportVehicleState.AT_TERMINAL) then`
- `AT_TERMINAL : TransportVehicleState`: Vehicle is at a terminal (station/stop)
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_list_react_util.tl:541`: `if (tv.state == api.type.enum.TransportVehicleState.EN_ROUTE) or (tv.state == api.type.enum.TransportVehicleState.AT_TERMINAL) then`
- `GOING_TO_DEPOT : TransportVehicleState`: Vehicle is traveling to a depot

## TrafficLightPreference (record)

```teal
global record TrafficLightPreference
```

TrafficLightPreference: controls whether a node/edge has a traffic light.

- `YES : TrafficLightPreference`: Always place a traffic light.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:520`: `newBaseNodeState.trafficLightPreference = api.type.enum.TrafficLightPreference.YES`
- `NO : TrafficLightPreference`: Never place a traffic light.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:489`: `newBaseNodeState.trafficLightPreference = api.type.enum.TrafficLightPreference.NO`
- `AUTO : TrafficLightPreference`: Let the game decide whether to place a traffic light.

## BaseEdgeType (record)

```teal
global record BaseEdgeType
```

Base edge type

- `NORMAL : BaseEdgeType`: Regular edge on the ground.
- `BRIDGE : BaseEdgeType`: Edge on a bridge.
- `TUNNEL : BaseEdgeType`: Edge in a tunnel.

## Vec2i (record)

```teal
global record Vec2i
```
Access: `api.type.Vec2i`

A 2d vector of integers
Supports array subscript with `vec[i]` access. Supports addition `+`, subtraction `-`, multiplication `*` and division `/`.

- `new : function() : Vec2i`  (call as `api.type.Vec2i.new(…)`): Creates a new vector.
    - returns: Vec2i(0,0),
    - used 81x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/notification_react_util.tl:200`: `coord = api.type.Vec2i.new(6, 6),`
- `new : function(v : integer) : Vec2i`  (call as `api.type.Vec2i.new(…)`): Creates a new vector.
    - param `v`: Single value to set both x and y (optional)
    - returns: Vec2i(v,v)
    - used 81x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/notification_react_util.tl:200`: `coord = api.type.Vec2i.new(6, 6),`
- `new : function(x : integer, y : integer) : Vec2i`  (call as `api.type.Vec2i.new(…)`): Creates a new vector.
    - param `x`: X coordinate
    - param `y`: Y coordinate
    - returns: Vec2i(x,y)
    - used 81x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/notification_react_util.tl:200`: `coord = api.type.Vec2i.new(6, 6),`
- `equals : function(a : Vec2i, b : Vec2i) : boolean`  (call as `api.type.Vec2i.equals(…)`): check for value-based equality
- `x : integer`: x coordinate
- `y : integer`: y coordinate
- `__add : function(Vec2i, Vec2i) : Vec2i`: Operator `a + b`: component-wise sum of two vectors.
- `__sub : function(Vec2i, Vec2i) : Vec2i`: Operator `a - b`: component-wise difference of two vectors.
- `__mul : function(Vec2i, Vec2i) : Vec2i`: Operator `a * b` with two `Vec2i` values.
- `__div : function(Vec2i, Vec2i) : Vec2i`: Operator `a / b` with two `Vec2i` values.

## Vec2f (record)

```teal
global record Vec2f
```
Access: `api.type.Vec2f`

A 2d vector of floats
Supports array subscript with `vec[i]` access. Supports addition `+`, subtraction `-`, multiplication `*` and division `/`.

- `new : function() : Vec2f`  (call as `api.type.Vec2f.new(…)`): Creates a new vector.
    - returns: Vec2f(0,0)
    - used 243x in the base game, e.g. `base/content/base/base/mod.script.tl:148`: `fromTable(api.type.BaseConfig.Settings.GeometryQualityOption.new(), { viewNearFar = api.type.Vec2f.new(4.0, 7200.0), fogStartEndFarPerc = api.type.Vec2f.new(0.4`
- `new : function(v : number) : Vec2f`  (call as `api.type.Vec2f.new(…)`): Creates a new vector.
    - param `v`: Single value to set both x and y (optional)
    - returns: Vec2f(v,v)
    - used 243x in the base game, e.g. `base/content/base/base/mod.script.tl:148`: `fromTable(api.type.BaseConfig.Settings.GeometryQualityOption.new(), { viewNearFar = api.type.Vec2f.new(4.0, 7200.0), fogStartEndFarPerc = api.type.Vec2f.new(0.4`
- `new : function(x : number, y : number) : Vec2f`  (call as `api.type.Vec2f.new(…)`): Creates a new vector.
    - param `x`: X coordinate (optional)
    - param `y`: Y coordinate (optional)
    - returns: Vec2f(x,y)
    - used 243x in the base game, e.g. `base/content/base/base/mod.script.tl:148`: `fromTable(api.type.BaseConfig.Settings.GeometryQualityOption.new(), { viewNearFar = api.type.Vec2f.new(4.0, 7200.0), fogStartEndFarPerc = api.type.Vec2f.new(0.4`
- `equals : function(a : Vec2f, b : Vec2f) : boolean`  (call as `api.type.Vec2f.equals(…)`): check for value-based equality
- `clone : function(Vec2f) : Vec2f`  (call as `vec2f:clone(…) (method)`): creates a copy of a vec2f
    - used 1x in the base game, e.g. `dlcs/urbangames_deluxe_upgrade_pack/content/fun_elements/fun_elements/balloon.script.tl:74`: `wind = wind:clone() / l`
- `x : number`: x coordinate
- `y : number`: y coordinate
- `__add : function(Vec2f, Vec2f) : Vec2f`: Operator `a + b`: component-wise sum of two vectors.
- `__sub : function(Vec2f, Vec2f) : Vec2f`: Operator `a - b`: component-wise difference of two vectors.
- `__mul : function(Vec2f, number) : Vec2f`: Operator `v * s`: multiplies every component by the number `s`.
- `__div : function(Vec2f, number) : Vec2f`: Operator `v / s`: divides every component by the number `s`.
- `scale : function(v : Vec2f, s : number) : Vec2f`  (call as `api.type.Vec2f.scale(…)`): Scale a vector by an amount
    - param `s`: Scale amount
    - returns: Scaled vector
    - used 3x in the base game, e.g. `base/content/mission/mission/tutorial/tutorial_story_util.tl:418`: `local testPos = pos + api.type.Vec2f.scale(dir, dist)`
- `normalize : function(v : Vec2f) : Vec2f`  (call as `api.type.Vec2f.normalize(…)`): Normalize vector to unit length
    - returns: Normalized vector
    - used 4x in the base game, e.g. `base/content/gui/gui/node_editor/layer_nodes.script.tl:114`: `point = api.type.Vec2f.normalize(point)`
- `length : function(v : Vec2f) : number`  (call as `api.type.Vec2f.length(…)`): Calculate length of a vector
    - returns: Length
    - used 5x in the base game, e.g. `base/content/mission/mission/builder_util.tl:10`: `local dist = api.type.Vec2f.length(pos - goal)`
- `distance : function(a : Vec2f, b : Vec2f) : number`  (call as `api.type.Vec2f.distance(…)`): Calculate distance between two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - returns: Distance
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:147`: `if api.type.Vec2f.distance(target, current) < 0.01 then`
- `distance2 : function(a : Vec2f, b : Vec2f) : number`  (call as `api.type.Vec2f.distance2(…)`): Calculate squared distance between two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - returns: Squared Distance
- `lerp : function(a : Vec2f, b : Vec2f, t : number) : Vec2f`  (call as `api.type.Vec2f.lerp(…)`): Linear interpolation between two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - param `t`: Interpolation parameter (0 to 1)
    - returns: Interpolated vector
- `angle : function(Vec2f, Vec2f) : number`  (call as `api.type.Vec2f.angle(…)`): Calculate angle between two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - returns: angle (in radians)
    - used 1x in the base game, e.g. `base/content/scripts/scripts/polygon_util.tl:54`: `local diff = api.type.Vec2f.normalize(dir0 + dir1) * offset / math.cos(api.type.Vec2f.angle(dir0, dir1) * 0.5)`
- `rotate : function(v : Vec2f, angle : number) : Vec2f`  (call as `api.type.Vec2f.rotate(…)`): Rotate vector by angle
    - param `angle`: Rotation angle in radians
    - returns: Rotated vector
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:143`: `return api.type.Vec2f.rotate(api.type.Vec2f.new(1, 0), angle)`

## Vec3f (record)

```teal
global record Vec3f
```
Access: `api.type.Vec3f`

A 3d vector of floats
Supports array subscript with `vec[i]` access. Supports addition `+`, subtraction `-`, multiplication `*` and division `/`.

- `new : function() : Vec3f`  (call as `api.type.Vec3f.new(…)`): Creates a new vector.
    - returns: Vec3f(0,0,0)
    - used 138x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:359`: `specularColor = api.type.Vec3f.new(0.56, 0.65, 0.77),`
- `new : function(v : number) : Vec3f`  (call as `api.type.Vec3f.new(…)`): Creates a new vector.
    - param `v`: Single value to set all components
    - returns: Vec3f(v,v,v)
    - used 138x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:359`: `specularColor = api.type.Vec3f.new(0.56, 0.65, 0.77),`
- `new : function(x : number, y : number, z : number) : Vec3f`  (call as `api.type.Vec3f.new(…)`): Creates a new vector.
    - param `x`: X coordinate
    - param `y`: Y coordinate
    - param `z`: Z coordinate
    - returns: Vec3f(x,y,z)
    - used 138x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:359`: `specularColor = api.type.Vec3f.new(0.56, 0.65, 0.77),`
- `equals : function(a : Vec3f, b : Vec3f) : boolean`  (call as `api.type.Vec3f.equals(…)`): check for value-based equality
- `clone : function(self : Vec3f) : Vec3f`  (call as `api.type.Vec3f.clone(…)`): Create a copy of the vector
    - returns: Cloned vector
- `x : number`: x coordinate
- `y : number`: y coordinate
- `z : number`: z coordinate
- `__add : function(Vec3f, Vec3f) : Vec3f`: Operator `a + b`: component-wise sum of two vectors.
- `__sub : function(Vec3f, Vec3f) : Vec3f`: Operator `a - b`: component-wise difference of two vectors.
- `__mul : function(Vec3f, number) : Vec3f`: Operator `v * s`: multiplies every component by the number `s`.
- `__div : function(Vec3f, number) : Vec3f`: Operator `v / s`: divides every component by the number `s`.
- `scale : function(v : Vec3f, s : number) : Vec3f`  (call as `api.type.Vec3f.scale(…)`): Scale vector by a factor
    - param `s`: Scale factor
    - returns: Scaled vector
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1037`: `local offset = api.type.Vec3f.scale((api.type.Vec3f.normalize(townPos - pos)), radius)`
- `normalize : function(v : Vec3f) : Vec3f`  (call as `api.type.Vec3f.normalize(…)`): Normalize vector to unit length
    - returns: Normalized vector
    - used 13x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane.script.tl:124`: `local currentDir = api.type.Vec3f.normalize(pathPosData.pPrime)`
- `length : function(v : Vec3f) : number`  (call as `api.type.Vec3f.length(…)`): Calculate length of the vector
    - returns: Length
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/blimp_util.tl:86`: `data.velocityNormalized = api.type.Vec3f.length(velocityNew)`
- `distance : function(a : Vec3f, b : Vec3f) : number`  (call as `api.type.Vec3f.distance(…)`): Calculate distance between two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - returns: Distance
    - used 25x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:194`: `local distanceToTarget = api.type.Vec3f.distance(targetPos, position)`
- `distance2 : function(a : Vec3f, b : Vec3f) : number`  (call as `api.type.Vec3f.distance2(…)`): Calculate squared distance between two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - returns: Squared distance
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_workers/deliver_workers.script.tl:305`: `return api.type.Vec3f.distance2(a, targetPos) < api.type.Vec3f.distance2(b, targetPos)`
- `distance2_2 : function(Vec3f, Vec3f) : number`  (call as `api.type.Vec3f.distance2_2(…)`): Calculate squared distance between the xy components of two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - returns: Squared distance
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:1340`: `if api.type.Vec3f.distance2_2(waypoint.pos, position) < tooCloseSq then`
- `distance_2 : function(a : Vec3f, b : Vec3f) : number`  (call as `api.type.Vec3f.distance_2(…)`): Calculate distance between xy components of two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - returns: Squared distance
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/ufo_util.tl:170`: `local currentDistance = api.type.Vec3f.distance_2(targetPos, currentPos)`
- `lerp : function(a : Vec3f, b : Vec3f, t : number) : Vec3f`  (call as `api.type.Vec3f.lerp(…)`): Linear interpolation between two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - param `t`: Interpolation parameter (0 to 1)
    - returns: Interpolated vector
    - used 1x in the base game, e.g. `mods/release/urbangames_campaign_mission_05/content/mission/mission/tasks/drilling/patrol.tl:62`: `local interpolatedPos = api.type.Vec3f.lerp(pos0, pos, frameTime01)`
- `angle : function(a : Vec3f, b : Vec3f) : number`  (call as `api.type.Vec3f.angle(…)`): Calculate angle between two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - returns: angle (in radians)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/view_manager_util.tl:21`: `local angle = api.type.Vec3f.angle(tangents[i], api.type.Vec3f.new(0.0) - tangents[j])`
- `dot : function(a : Vec3f, b : Vec3f) : number`  (call as `api.type.Vec3f.dot(…)`): Calculate dot product of two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - returns: dot product

## Vec4f (record)

```teal
global record Vec4f
```
Access: `api.type.Vec4f`

A 4d vector of floats
Supports array subscript with `vec[i]` access. Supports addition `+`, subtraction `-`, multiplication `*` and division `/`.

- `new : function() : Vec4f`  (call as `api.type.Vec4f.new(…)`): Creates a new vector.
    - returns: Vec4f(0,0,0,0)
    - used 44x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:14`: `local col0 = api.type.Vec4f.new(tbl[1][1], tbl[2][1], tbl[3][1], tbl[4][1])`
- `new : function(v : number) : Vec4f`  (call as `api.type.Vec4f.new(…)`): Creates a new vector.
    - param `v`: Single value to set all components
    - returns: Vec4f(v,v,v,v)
    - used 44x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:14`: `local col0 = api.type.Vec4f.new(tbl[1][1], tbl[2][1], tbl[3][1], tbl[4][1])`
- `new : function(x : number, y : number, z : number, w : number) : Vec4f`  (call as `api.type.Vec4f.new(…)`): Creates a new vector.
    - param `x`: X coordinate
    - param `y`: Y coordinate
    - param `z`: Z coordinate
    - param `w`: W coordinate
    - returns: Vec4f(x, y, z, w)
    - used 44x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:14`: `local col0 = api.type.Vec4f.new(tbl[1][1], tbl[2][1], tbl[3][1], tbl[4][1])`
- `new : function(v : Vec3f, w : number) : Vec4f`  (call as `api.type.Vec4f.new(…)`): Creates a new vector from Vec3f and w component.
    - param `v`: Vec3f for x, y, z components
    - param `w`: W coordinate
    - returns: New Vec4f
    - used 44x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:14`: `local col0 = api.type.Vec4f.new(tbl[1][1], tbl[2][1], tbl[3][1], tbl[4][1])`
- `equals : function(a : Vec4f, b : Vec4f) : boolean`  (call as `api.type.Vec4f.equals(…)`): check for value-based equality
- `x : number`: x coordinate
- `y : number`: y coordinate
- `z : number`: z coordinate
- `w : number`: w coordinate
- `__add : function(Vec4f, Vec4f) : Vec4f`: Operator `a + b`: component-wise sum of two vectors.
- `__sub : function(Vec4f, Vec4f) : Vec4f`: Operator `a - b`: component-wise difference of two vectors.
- `__mul : function(Vec4f, number) : Vec4f`: Operator `v * s`: multiplies every component by the number `s`.
- `__div : function(Vec4f, number) : Vec4f`: Operator `v / s`: divides every component by the number `s`.
- `lerp : function(a : Vec4f, b : Vec4f, t : number) : Vec4f`  (call as `api.type.Vec4f.lerp(…)`): Linear interpolation between two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - param `t`: Interpolation parameter (0 to 1)
    - returns: Interpolated vector
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_react_util.tl:20`: `local interColor = api.type.Vec4f.lerp(params.colorA, params.colorB, s)`

## Vec5f (record)

```teal
global record Vec5f
```
Access: `api.type.Vec5f`

A 5d vector of floats
Supports array subscript with `vec[i]` access. Supports addition `+`, subtraction `-`, multiplication `*` and division `/`.

- `new : function() : Vec5f`  (call as `api.type.Vec5f.new(…)`): Creates a new vector.
    - returns: Vec5f(0, 0, 0, 0, 0)
    - used 4x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/tasks/everything/everything.tl:84`: `local camera = api.type.Vec5f.new(-1087.1555175781, -1253.4951171875, 223.9351348877, 0.66147267818451, 0)`
- `new : function(v : number) : Vec5f`  (call as `api.type.Vec5f.new(…)`): Creates a new vector.
    - param `v`: Single value to set all components
    - returns: Vec5f(v, v, v, v, v)
    - used 4x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/tasks/everything/everything.tl:84`: `local camera = api.type.Vec5f.new(-1087.1555175781, -1253.4951171875, 223.9351348877, 0.66147267818451, 0)`
- `new : function(x : number, y : number, z : number, w : number, q : number) : Vec5f`  (call as `api.type.Vec5f.new(…)`): Creates a new vector.
    - param `x`: X coordinate
    - param `y`: Y coordinate
    - param `z`: Z coordinate
    - param `w`: W coordinate
    - param `q`: Q coordinate
    - returns: Vec5f(x, y, z, w, q)
    - used 4x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/tasks/everything/everything.tl:84`: `local camera = api.type.Vec5f.new(-1087.1555175781, -1253.4951171875, 223.9351348877, 0.66147267818451, 0)`
- `equals : function(a : Vec5f, b : Vec5f) : boolean`  (call as `api.type.Vec5f.equals(…)`): check for value-based equality
- `x : number`: x coordinate
- `y : number`: y coordinate
- `z : number`: z coordinate
- `w : number`: w coordinate
- `q : number`: q coordinate
- `__add : function(Vec5f, Vec5f) : Vec5f`: Operator `a + b`: component-wise sum of two vectors.
- `__sub : function(Vec5f, Vec5f) : Vec5f`: Operator `a - b`: component-wise difference of two vectors.
- `__mul : function(Vec5f, number) : Vec5f`: Operator `v * s`: multiplies every component by the number `s`.
- `__div : function(Vec5f, number) : Vec5f`: Operator `v / s`: divides every component by the number `s`.
- `lerp : function(a : Vec5f, b : Vec5f, t : number) : Vec5f`  (call as `api.type.Vec5f.lerp(…)`): Linear interpolation between two vectors
    - param `a`: First vector
    - param `b`: Second vector
    - param `t`: Interpolation parameter (0 to 1)
    - returns: Interpolated vector

## Mat4f (record)

```teal
global record Mat4f
```
Access: `api.type.Mat4f`

A 4x4 transformation matrix
Supports addition `+`, subtraction `-` and multiplication `*`. Supports array subscript with `mat[i]` access (16 elements in column-major order).

- `new : function() : Mat4f`  (call as `api.type.Mat4f.new(…)`): Creates a new matrix.
    - returns: Identity matrix
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:18`: `return api.type.Mat4f.new(col0, col1, col2, col3)`
- `new : function(Vec4f) : Mat4f`  (call as `api.type.Mat4f.new(…)`): Creates a new matrix from Vec4f.
    - param `v`: Vec4f to initialize all columns
    - returns: New Mat4f
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:18`: `return api.type.Mat4f.new(col0, col1, col2, col3)`
- `new : function(col1 : Vec4f, col2 : Vec4f, col3 : Vec4f, col4 : Vec4f) : Mat4f`  (call as `api.type.Mat4f.new(…)`): Creates a new matrix from four column vectors.
    - param `col1`: First column
    - param `col2`: Second column
    - param `col3`: Third column
    - param `col4`: Fourth column
    - returns: New Mat4f
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:18`: `return api.type.Mat4f.new(col0, col1, col2, col3)`
- `clone : function(matrix : Mat4f) : Mat4f`  (call as `api.type.Mat4f.clone(…)`): Create a copy of the matrix
    - returns: Cloned matrix
- `__index : function(Mat4f, integer) : number`: Subscript `mat[i]`: returns element `i` of the 16 matrix elements in column-major order (1-based, so `mat[13]`, `mat[14]`, `mat[15]` are the translation).
- `__add : function(Mat4f, Mat4f) : Mat4f`: Operator `a + b`: element-wise sum of two matrices.
- `__sub : function(Mat4f, Mat4f) : Mat4f`: Operator `a - b`: element-wise difference of two matrices.
- `__mul : function(Mat4f, Mat4f) : Mat4f`: Operator `a * b`: matrix product.
- `cols : function(matrix : Mat4f, colIndex : integer) : Vec4f`  (call as `mat4f:cols(…) (method)`): Get column vector at index
    - param `matrix`: the matrix
    - param `colIndex`: Column index (1-4)
    - returns: Column vector
    - used 17x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:6`: `{ mat:cols(0).x, mat:cols(1).x, mat:cols(2).x, mat:cols(3).x },`
- `getTransl : function(matrix : Mat4f) : Vec3f`  (call as `api.type.Mat4f.getTransl(…)`): Get translation component of the matrix
    - returns: Translation vector
- `setTransl : function(matrix : Mat4f, v : Vec3f) : Mat4f`  (call as `mat4f:setTransl(…) (method)`): Set translation component of the matrix
    - param `v`: Translation vector
    - returns: Modified matrix
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/custom_entity_util.tl:67`: `transf:setTransl(pos)`
- `transformPosition : function(matrix : Mat4f, v : Vec3f) : Vec3f`  (call as `mat4f:transformPosition(…) (method)`): Transform a position by the matrix
    - param `v`: Position to transform
    - returns: Transformed position
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/rocket_util.tl:96`: `local accel = (pitchRotation:transformPosition(api.type.Vec3f.new(0, 0, 1))) * (rocketParams.thrust / rocketParams.mass) + rocketParams.gravity`
- `transformDirection : function(matrix : Mat4f, v : Vec3f) : Vec3f`  (call as `api.type.Mat4f.transformDirection(…)`): Transform a direction by the matrix
    - param `v`: Direction to transform
    - returns: Transformed direction
- `rotZ : function(angle : number) : Mat4f`  (call as `api.type.Mat4f.rotZ(…)`): Creates a new Z-axis rotation
    - param `angle`: Rotation angle in radians
    - returns: Rotation matrix
    - used 4x in the base game, e.g. `base/content/mission/mission/tasks/click_and_collect_custom_entity/spawn_entity.tl:28`: `newTransf = newTransf * api.type.Mat4f.rotZ(params.rotateZ)`
- `rotY : function(angle : number) : Mat4f`  (call as `api.type.Mat4f.rotY(…)`): Creates a new Y-axis rotation
    - param `angle`: Rotation angle in radians
    - returns: Rotation matrix
    - used 4x in the base game, e.g. `base/content/scripts/scripts/transformator_util.tl:186`: `local transfY = api.type.Mat4f.rotY(modelInfo.wheelSigns[i].y * totalDist / wheelInfo.radius)`
- `rotX : function(angle : number) : Mat4f`  (call as `api.type.Mat4f.rotX(…)`): Creates a new X-axis rotation
    - param `angle`: Rotation angle in radians
    - returns: Rotation matrix
- `rotZTransl : function(angle : number, transl : Vec3f) : Mat4f`  (call as `api.type.Mat4f.rotZTransl(…)`): Creates a new Z-axis rotation matrix plus translation
    - param `angle`: Rotation angle in radians
    - param `transl`: Translation vector
    - returns: Transformation matrix
    - used 10x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/blimp_util.tl:144`: `local transf = api.type.Mat4f.rotZTransl(angle, spawnPos)`
- `rotZYTransl : function(angles : Vec2f, transl : Vec3f) : Mat4f`  (call as `api.type.Mat4f.rotZYTransl(…)`): Creates a new Z-axis and Y-axis rotation matrix plus translation
    - param `angles`: Rotation angles (x: Z angle, y: Y angle)
    - param `transl`: Translation vector
    - returns: Transformation matrix
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/rocket_util.tl:94`: `local pitchRotation = api.type.Mat4f.rotZYTransl(api.type.Vec2f.new(rocketData.rotZ or 0, targetPitchAngle), api.type.Vec3f.new(0, 0, 0))`
- `scale : function(scale : Vec3f) : Mat4f`  (call as `api.type.Mat4f.scale(…)`): Create scale matrix
    - param `scale`: Scale factors
    - returns: Scale matrix
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/firework/firework.script.tl:18`: `transfsOutput:setUserTransf(0, api.type.Mat4f.scale(api.type.Vec3f.new(0.01, 0.01, 0.01)))`
- `scaleRotZTransl : function(scale : Vec3f, angle : number, transl : Vec3f) : Mat4f`  (call as `api.type.Mat4f.scaleRotZTransl(…)`): Creates a new Z-axis rotation matrix plus translation and scale
    - param `scale`: Scale factors
    - param `angle`: Rotation angle in radians
    - param `transl`: Translation vector
    - returns: Transformation matrix
- `scaleTransl : function(scale : Vec3f, transl : Vec3f) : Mat4f`  (call as `api.type.Mat4f.scaleTransl(…)`): Creates a new translation and scaling matrix
    - param `scale`: Scale factors
    - param `transl`: Translation vector
    - returns: Transformation matrix
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/blimp_util.tl:222`: `local newTransf = api.type.Mat4f.scaleTransl(api.type.Vec3f.new(0.1, 0.1, 0.1), currentPos)`
- `transl : function(transl : Vec3f) : Mat4f`  (call as `api.type.Mat4f.transl(…)`): Creates a new translation matrix
    - param `transl`: Translation vector
    - returns: Translation matrix
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/fun_elements/fireworks.script.tl:127`: `api.type.Mat4f.transl(townPos + random)`

## Box3f (record)

```teal
global record Box3f
```
Access: `api.type.Box3`

A 3d bounding box

- `new : function() : Box3f`  (call as `api.type.Box3.new(…)`): Creates a new Box3f
- `new : function(min : Vec3f, max : Vec3f) : Box3f`  (call as `api.type.Box3.new(…)`): Creates a new Box3f
- `min : Vec3f`: Minimum corner of the box
- `max : Vec3f`: Maximum corner of the box

## Box2f (record)

```teal
global record Box2f
```
Access: `api.type.Box2`

A 2d bounding box

- `new : function() : Box2f`  (call as `api.type.Box2.new(…)`): Creates a new Box2f
- `new : function(min : Vec2f, max : Vec2f) : Box2f`  (call as `api.type.Box2.new(…)`): Creates a new Box2f
- `min : Vec3f`: Minimum corner of the box
- `max : Vec3f`: Maximum corner of the box

## Gridf (record)

```teal
global record Gridf
```

A 2D grid of float values

- `new : function() : Gridf`: Creates a new empty grid.
    - returns: New empty Gridf
- `new : function(x0 : integer, y0 : integer, width : integer, height : integer) : Gridf`: Creates a new grid with specified dimensions.
    - param `x0`: X offset of the grid
    - param `y0`: Y offset of the grid
    - param `width`: Grid width
    - param `height`: Grid height
    - returns: New Gridf
- `new : function(x0 : integer, y0 : integer, width : integer, height : integer, value : number) : Gridf`: Creates a new grid with specified dimensions and initial value.
    - param `x0`: X offset of the grid
    - param `y0`: Y offset of the grid
    - param `width`: Grid width
    - param `height`: Grid height
    - param `value`: Initial value for all cells
    - returns: New Gridf
- `width : integer`: Grid width
- `height : integer`: Grid height
- `at : function(pos : Vec2i) : number`: Get value at position. Returns 0 if position is out of bounds.
    - param `pos`: Grid position
    - returns: Value at position

## GridVec2f (record)

```teal
global record GridVec2f
```

A 2D grid of Vec2f values

- `new : function() : GridVec2f`: Creates a new empty grid.
    - returns: New empty GridVec2f
- `new : function(x0 : integer, y0 : integer, width : integer, height : integer) : GridVec2f`: Creates a new grid with specified dimensions.
    - param `x0`: X offset of the grid
    - param `y0`: Y offset of the grid
    - param `width`: Grid width
    - param `height`: Grid height
    - returns: New GridVec2f
- `new : function(x0 : integer, y0 : integer, width : integer, height : integer, value : number) : GridVec2f`: Creates a new grid with specified dimensions and initial value.
    - param `x0`: X offset of the grid
    - param `y0`: Y offset of the grid
    - param `width`: Grid width
    - param `height`: Grid height
    - param `value`: Initial value for all cells
    - returns: New GridVec2f
- `width : integer`: Grid width
- `height : integer`: Grid height
- `x0 : integer`: X offset of the grid
- `y0 : integer`: Y offset of the grid
- `at : function(grid : GridVec2f, pos : Vec2i) : Vec2f`: Get value at position
    - param `grid`: Grid to query
    - param `pos`: Grid position
    - returns: Vec2f value at position

## View (record)

```teal
global record View
```

Camera view information

- `getEye : function(view : View) : Vec3f`: Get the camera eye position
    - returns: Eye position
- `getCenter : function(view : View) : Vec3f`: Get the camera center position
    - returns: Center position
- `getUp : function(view : View) : Vec3f`: Get the camera up vector
    - returns: Up vector
- `isVisible : function(view : View, position : Vec3f, radius : number) : boolean`: Check if a sphere is visible in the current view
    - param `view`: The view
    - param `position`: Center of the sphere
    - param `radius`: Radius of the sphere
    - returns: True if visible

## Date (record)

```teal
global record Date
```
Access: `api.type.Date`

Date representation with year, month, and day

- `new : function(year : integer, month : integer, day : integer) : Date`  (call as `api.type.Date.new(…)`): Creates a new date from year, month and day.
    - param `year`: Year
    - param `month`: Month (1-12)
    - param `day`: Day (1-31)
    - returns: New Date
    - used 10x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_widgets.tl:270`: `local apiDate = api.type.Date.new(date.year, date.month, date.day)`
- `new : function(year : integer) : Date`  (call as `api.type.Date.new(…)`): Creates a new date from year
    - param `year`: Year
    - returns: New Date
    - used 10x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_widgets.tl:270`: `local apiDate = api.type.Date.new(date.year, date.month, date.day)`
- `now : function() : Date`  (call as `api.type.Date.now(…)`): Get date of current day
    - returns: Current date
    - used 2x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:240`: `local time = api.util.formatDate(api.type.Date.now(), api.type.enum.DateFormat.FULL) .. " " .. api.util.formatTime(api.util.getLocalTime())`
- `fromYear : function(year : integer) : Date`  (call as `api.type.Date.fromYear(…)`): Create date at the first day of the given year
    - param `integer`: year integer
    - returns: A new Date
- `fromString : function(str : string) : Date`  (call as `api.type.Date.fromString(…)`): Parse date from string
    - param `str`: Date string
    - returns: Parsed date
- `getEndOfMonth : function(current : Date) : Date`  (call as `api.type.Date.getEndOfMonth(…)`): Get the last day of the current month
    - param `current`: Current date
    - returns: Date representing end of month
- `getDayOfWeek : function(current : Date) : integer`  (call as `api.type.Date.getDayOfWeek(…)`): Get the day of the week of the current date
    - param `current`: Current date
    - returns: The day of the week
- `addOffsetDays : function(date : Date, numDays : integer) : Date`  (call as `date:addOffsetDays(…) (method)`): add offset in days
    - param `date`: the date to offset
    - param `numDays`: number of days to add
    - returns: New Date offset by the given number of days
    - used 1x in the base game, e.g. `base/content/mission/mission/calendar.lua:21`: `local newDate = startDate:addOffsetDays(daystoforward)`
- `year : integer`: Year component
- `month : integer`: Month component (1-12)
- `day : integer`: Day component (1-31)

## StationTerminal (record)

```teal
global record StationTerminal
```
Access: `api.type.StationTerminal`

Identifies a Terminal inside a StationGroup

- `new : function() : StationTerminal`  (call as `api.type.StationTerminal.new(…)`): Constructor: creates empty terminal reference
    - used 7x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:939`: `newContext.highlightTerminal = { station.stationGroup, api.type.StationTerminal.new(station.stationIndex1-1, station.terminalIndex1-1)}`
- `new : function(StationTerminal) : StationTerminal`  (call as `api.type.StationTerminal.new(…)`): Copy constructor
    - used 7x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:939`: `newContext.highlightTerminal = { station.stationGroup, api.type.StationTerminal.new(station.stationIndex1-1, station.terminalIndex1-1)}`
- `new : function(station : integer, terminal : integer) : StationTerminal`  (call as `api.type.StationTerminal.new(…)`): Constructor with station and terminal indices
    - param `station`: The zero-based station index in the StationGroup
    - param `terminal`: The zero-based terminal index in the Station
    - used 7x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:939`: `newContext.highlightTerminal = { station.stationGroup, api.type.StationTerminal.new(station.stationIndex1-1, station.terminalIndex1-1)}`
- `station : integer`: The zero-based station index in the StationGroup
- `terminal : integer`: The zero-based terminal index in the Station

## Node (record)

```teal
global record Node
```

A single node within a TRANSPORT_NETWORK component.

## EdgeGeometry (record)

```teal
global record EdgeGeometry
```
Access: `api.type.EdgeGeometry`

Defines the geometry (shape) of an edge

- `new : function() : EdgeGeometry`  (call as `api.type.EdgeGeometry.new(…)`): Creates a new, empty `EdgeGeometry`.
- `type : Type`: Type for the 2D (xy) shape
- `arc : Arc`: Arc parameters (if type is ARC)
- `straight : Straight`: Straight parameters (if type is STRAIGHT)
- `cubicSpline : CubicSpline`: Cubic spline parameters (if type is CUBIC_SPLINE)
- `cubicOffsetSpline : CubicOffsetSpline`: Cubic offset spline parameters (if type is CUBIC_OFFSET_SPLINE)
- `tangent : Vec2f`: Vertical tangents at start and end
- `height : Vec2f`: Heights at start and end
- `length : number`: Length of the edge
- `width : number`: Width of the edge
- `locate : function(geom : EdgeGeometry, pos : Vec3f, maxSteps : integer, tol : number) : {number, boolean}`  (call as `api.type.EdgeGeometry.locate(…)`): Finds the parameter on a spline for a given world position
    - param `geom`: the edge geometry to sample
    - param `pos`: a 3d world position to search for
    - param `maxSteps`: the maximum number of steps to perform
    - param `tol`: a maximal distance for the point to be found
    - returns: paramAndFound the parameter value and whether there was a point on the spline
- `calcPos : function(geom : EdgeGeometry, param : number) : {Vec3f, Vec3f, Vec3f}`  (call as `api.type.EdgeGeometry.calcPos(…)`): Calculate a position on an edgegeometry, given a parameter in [0, 1]
    - param `geom`: the edge geometry to sample
    - param `param`: a number in [0, 1]
    - returns: the position, tangent and second derivative at the specified param

### EdgeGeometry.Type (record)

```teal
record Type
```
Access: `api.type.EdgeGeometry.Type`

Type of the edge geometry

- `ARC : Type`
- `STRAIGHT : Type`
- `CUBIC_SPLINE : Type`
- `CUBIC_OFFSET_SPLINE : Type`

### EdgeGeometry.Arc (record)

```teal
record Arc
```
Access: `api.type.EdgeGeometry.Arc`

Arc geometry parameters

- `new : function() : Arc`  (call as `api.type.EdgeGeometry.Arc.new(…)`): Creates a new, empty `Arc`.
- `center : Vec2f`: Center point of the arc
- `radius : number`: Radius of the arc
- `startAngle : number`: Starting angle in radians
- `angle : number`: Arc angle in radians

### EdgeGeometry.Straight (record)

```teal
record Straight
```
Access: `api.type.EdgeGeometry.Straight`

Straight line geometry parameters

- `new : function() : Straight`  (call as `api.type.EdgeGeometry.Straight.new(…)`): Creates a new, empty `Straight`.
- `pos : {Vec2f}`: Start and end positions

### EdgeGeometry.CubicSpline (record)

```teal
record CubicSpline
```
Access: `api.type.EdgeGeometry.CubicSpline`

Cubic spline geometry parameters

- `new : function() : CubicSpline`  (call as `api.type.EdgeGeometry.CubicSpline.new(…)`): Creates a new, empty `CubicSpline`.
- `pos : {Vec2f}`: Start and end positions
- `tangent : {Vec2f}`: Tangent vectors at start and end

### EdgeGeometry.CubicOffsetSpline (record)

```teal
record CubicOffsetSpline
```
Access: `api.type.EdgeGeometry.CubicOffsetSpline`

Cubic offset spline geometry parameters

- `new : function() : CubicOffsetSpline`  (call as `api.type.EdgeGeometry.CubicOffsetSpline.new(…)`): Creates a new, empty `CubicOffsetSpline`.
- `pos : {Vec2f}`: Start and end positions
- `tangent : {Vec2f}`: Tangent vectors at start and end
- `offset : number`: Offset value

## Edge (record)

```teal
global record Edge
```

A single edge within a Transport Network.

- `conns : {NodeId}`: Start and end node
- `geometry : EdgeGeometry`: Edge geometry definition
- `transportModes : {TransportMode : boolean}`: Transport modes allowed on this edge
- `speedLimit : number`: Speed limit for the edge
- `curveSpeedLimit : number`: Speed limit due to curvature
- `curSpeed : number`: Current speed on the edge (for statistics)
- `linkable : integer`: Linkable flag of the edge; constructions set it through the `linkable` option of their lanes (see `laneutil.createLane`).
- `pedestrianWalkPenalty : number`: Penalty for pedestrian walking
- `precedence : boolean`
- `noiseModifier : number`: Noise modifier
- `pollutionModifier : number`: Pollution modifier
- `needsReservation : boolean`: Whether the edge needs reservation
- `forwardOnly : boolean`: Whether the edge only allowed to be traversed in forward direction

## NodeId (record)

```teal
global record NodeId
```
Access: `api.type.NodeId`

References a single transport network node within a transport network entity

- `new : function() : NodeId`  (call as `api.type.NodeId.new(…)`): Creates a new, empty `NodeId`.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:53`: `nodes[#nodes+1] = api.type.NodeId.new(entity, index - 1)`
- `new : function(entity : Engine.Entity, index : integer) : NodeId`  (call as `api.type.NodeId.new(…)`): Constructor

Creates a new, empty `NodeId`.
    - param `entity`: The TransportNetwork entity
    - param `index`: The zero-based index within the nodes field of the TransportNetwork component
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:53`: `nodes[#nodes+1] = api.type.NodeId.new(entity, index - 1)`
- `entity : Engine.Entity`: The TransportNetwork entity
- `index : integer`: The zero-based index within the nodes field of the TransportNetwork component

## EdgeId (record)

```teal
global record EdgeId
```
Access: `api.type.EdgeId`

References a single transport network edge within a transport network entity

- `new : function() : EdgeId`  (call as `api.type.EdgeId.new(…)`): Creates a new, empty `EdgeId`.
- `new : function(entity : integer, index : integer) : EdgeId`  (call as `api.type.EdgeId.new(…)`): Constructor with entity and index

Creates a new, empty `EdgeId`.
    - param `entity`: The TransportNetwork entity
    - param `index`: The zero-based index within the edges field of the TransportNetwork component
- `entity : Engine.Entity`: The TransportNetwork entity
- `index : integer`: The zero-basd index within the edges field of the TransportNetwork component

## EdgePos (record)

```teal
global record EdgePos
```
Access: `api.type.EdgePos`

References a position inside an EdgeId

- `new : function() : EdgePos`  (call as `api.type.EdgePos.new(…)`): Creates a new, empty `EdgePos`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:378`: `snap = api.type.EdgePos.new(apiDetails.snap)`
- `new : function(EdgePos) : EdgePos`  (call as `api.type.EdgePos.new(…)`): Copy constructor

Creates a new, empty `EdgePos`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:378`: `snap = api.type.EdgePos.new(apiDetails.snap)`
- `equals : function(a : EdgePos, b : EdgePos) : boolean`  (call as `api.type.EdgePos.equals(…)`): Returns `true` when both `EdgePos` values are equal.
- `edgeId : EdgeId`: The edge (entity and index within TransportNetwork)
- `param : number`: The position on the edge

## PrecedencePreference (record)

```teal
global record PrecedencePreference
```

Preference for the right of way (precedence) of a street at one of its nodes; see `BaseEdgeStreet.precedenceNode0` and `precedenceNode1`.

- `YES : PrecedencePreference`: The street has precedence at the node.
- `NO : PrecedencePreference`: The street has no precedence at the node.
- `AUTO : PrecedencePreference`: Let the game decide precedence at the node.

## LaneConnection (record)

```teal
global record LaneConnection
```

A custom configuration for lanes on crossings

- `segment0 : Engine.Entity`: Incoming segment entity
- `lane0 : integer`: Incoming lane index (zero-based, from left to right when looking from the Node to segment0)
- `segment1 : Engine.Entity`: Outgoing segment entity
- `lane1 : integer`: Outgoing lane index (zero-based, from left to right when looking from the Node to segment1)
- `withRoad : boolean`: Whether connection supports road
- `withTram : boolean`: Whether connection supports tram

## TrafficLightData (record)

```teal
global record TrafficLightData
```

Additional traffic light data for a base node

- `state2lockedLanes : {{integer}}`: Lanes that are blocked in each traffic light state.
- `state2redLightModelIndices : {{integer}}`: Indices of the models that show a red light in each traffic light state.
- `state2duration : {number}`: Duration of each traffic light state.
- `animatedModels : {integer}`: Indices of the animated traffic light models.

## SubShape (record)

```teal
global record SubShape
```

Contains a procedurally generated sub-shape.

- `faces : {{Vec3f}}`: Faces of the sub-shape, each a list of `Vec3f` points.
- `startRule : integer`
- `height0 : number`: Min height for collision purposes.
- `height1 : number`: Max height for collision purposes.

## Shape (record)

```teal
global record Shape
```

Contains a procedurally generated shape.

- `subShapes : {SubShape}`: List of subshapes for the shape.
- `generator : integer`: Procedural generator (index) used to generate the shape.

## Waypoint (record)

```teal
global record Waypoint
```
Access: `api.type.Waypoint`

References the position of a waypoint, which is either an EdgePos or a Vec3f

- `new : function() : Waypoint`  (call as `api.type.Waypoint.new(…)`): Creates a new, empty `Waypoint`.
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:695`: `local apiWaypoint = api.type.Waypoint.new()`
- `new : function(Waypoint) : Waypoint`  (call as `api.type.Waypoint.new(…)`): Creates a new, empty `Waypoint`.
    - used 4x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:695`: `local apiWaypoint = api.type.Waypoint.new()`
- `clone : function(Waypoint) : Waypoint`  (call as `api.type.Waypoint.clone(…)`): Returns a copy of the waypoint.
- `equals : function(a : Waypoint, b : Waypoint)`  (call as `api.type.Waypoint.equals(…)`): Compares two waypoints by value.
- `edgePos : EdgePos`: Edge position when using lanes
- `pos : Vec3f`: 3D position for ships and aircraft
- `tag : integer`: tag to uniquely identify a waypoint even across addition/removal

## SignalId (record)

```teal
global record SignalId
```
Access: `api.type.SignalId`

Identifies a street/track signal

- `new : function() : SignalId`  (call as `api.type.SignalId.new(…)`): Creates a new, empty `SignalId`.
- `new : function(SignalId) : SignalId`  (call as `api.type.SignalId.new(…)`): Creates a new, empty `SignalId`.
- `entity : Engine.Entity`: The entity of the SignalList
- `index : integer`: The zero-based index of the signal inside the signal list

## Terminal (record)

```teal
global record Terminal
```

Data for a Station terminal.

- `vehicleNodeId : NodeId`: The position (node) where the vehicle will attempt to stop
- `personNodes : {NodeId}`: List of all nodes for the person/cargo wait area
- `personEdges : {EdgeId}`: List of all edges for the person/cargo wait area
- `vehicleEdges : {EdgePos}`: list of all edges for the vehicle to stop
- `tag : integer`: Identifying tag
- `cargoLoad : boolean`: Whether this terminal can handle cargo
- `passengersLoad : boolean`: Whether this terminal can handle passengers
- `cargoUnload : boolean`: Whether this terminal can handle cargo
- `passengersUnload : boolean`: Whether this terminal can handle passengers
- `transportModes : {TransportMode : boolean}`: Transport modes the terminal serves.
- `cargoTransferSpeeds : {TerminalCargoTransferSpeed}`: List of cargo transfer speed configurations
- `comfortFactor : number`: Control happiness decrease speed for waiting passengers
- `capacity : integer`: How many people can fit the specific terminal
- `terminalAssignmentWeight : number`: Weighting for the automatic terminal assignment of lines, lower means less important

### Terminal.TerminalCargoTransferSpeed (record)

```teal
record TerminalCargoTransferSpeed
```

Information about terminal cargo transfer speeds

- `cargoTypeSet : CargoTypeSet`: Set of cargo types this setting applies to
- `loadSpeedModifier : number`: Speed modifier for loading cargo
- `length : number`: Length of the terminal section

## Station (record)

```teal
global record Station
```

Data for a Station

- `new : function() : Station`: Creates a new, empty `Station`.
- `tag : integer`: A tag for identifying station between upgrades
- `terminals : {Terminal}`: list of all terminals
- `catchmentAreas : {CatchmentArea}`: list of all catchment areas
- `pool : Pool`: pool

### Station.Pool (record)

```teal
record Pool
```

Defines a common pool in a station for overflowing terminal capacities

- `new : function() : Pool`: Creates a new, empty `Pool`.
- `edges : {EdgeId}`: the list of edges of the shared waiting area
- `capacity : integer`: the size of the shared pool

## ComponentCustomState (record)

```teal
global record ComponentCustomState
```

A custom component might contain any data in form of a lua table (without functions) and can used with custom
scripts to create new custom objects in game

- `new : function() : ComponentCustomState`: Creates a new, empty `ComponentCustomState`.
- `new : function(ComponentCustomState) : Engine.Component.Line`: Creates a new, empty `ComponentCustomState`.
- `state : table`: Any lua table, as long as it is serializable

## LoadConfig (record)

```teal
global record LoadConfig
```
Access: `api.type.LoadConfig`

Configuration for loading cargo into a vehicle compartment

- `new : function() : LoadConfig`  (call as `api.type.LoadConfig.new(…)`): Creates a new, empty `LoadConfig`.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_util.tl:17`: `local lc = api.type.LoadConfig.new()`
- `loadConfigIndex : integer`: Index of the load configuration
- `cargoTypeId : CargoTypeId`: Type of cargo to load

## GameScriptDesc (record)

```teal
global record GameScriptDesc
```
Access: `api.type.GameScriptDesc`

Contains all data describing a game script

- `new : function() : GameScriptDesc`  (call as `api.type.GameScriptDesc.new(…)`): Creates a new, empty `GameScriptDesc`.
- `new : function(GameScriptDesc) : GameScriptDesc`  (call as `api.type.GameScriptDesc.new(…)`): Creates a new, empty `GameScriptDesc`.
- `postUpdateScript : ScriptRef`: Reference to the script's `postUpdate(userParams, state, dt, updateResult)` function, which runs after `update` and receives its return value.
- `handleEventScript : ScriptRef`: Reference to the script's `handleEvent(userParams, state, src, id, name, param)` function, which handles events sent to game scripts.
- `guiUpdateScript : ScriptRef`: Reference to the script's `guiUpdate(userParams, state, guiState)` function, which runs in the GUI with read-only access to the script state.
- `guiHandleEventScript : ScriptRef`: Reference to the script's `guiHandleEvent(userParams, state, guiState, src, id, name, param)` function, which handles events in the GUI.
- `updateScript : ScriptRef`: Reference to the script's `update(userParams, state, dt)` function, which updates the game script state.
    - notes: In a `.gs.lua` file the references are given as `{ fileName = "<script>@<function>" }`, e.g.: ```lua updateScript = { fileName = "industries.script@update" }, postUpdateScript = { fileName = "industries.script@postUpdate" }, handleEventScript = { fileName = "industries.script@handleEvent" }, ``` The function signatures are declared in `GameScript` and `GameScriptWithGui` (`base/tealdef/scripts/gamescript.d.tl`).

## ScriptRef (record)

```teal
global record ScriptRef
```
Access: `api.type.ScriptRef`

References a function in a lua script file with ".script" extension.

- `new : function() : ScriptRef`  (call as `api.type.ScriptRef.new(…)`): Creates a new, empty `ScriptRef`.
    - used 2x in the base game, e.g. `base/content/base/base/mod.script.tl:633`: `local newScriptRef = api.type.ScriptRef.new()`
- `fileName : string`: The script path, toghether with the lua path inside the function.


For example, this is the reference for a Crane flock function:

"models/model/animal/animal.flock.crane"


- the tokens separated by '/' before '.' are taken as part of the the path: "models/model/animal/animal"

- the tokens after the second '.' represent the lua path (in this case "flock.crane")


- `params : table`: A lua table of closure parameters that will be added at the end of the parameter list when the script is called.

## ModuleDesc (record)

```teal
global record ModuleDesc
```
Access: `api.type.ModuleDesc`

Used to configure new modules

- `new : function() : ModuleDesc`  (call as `api.type.ModuleDesc.new(…)`): Creates a new, empty `ModuleDesc`.
    - used 1x in the base game, e.g. `base/content/base/base/mod.script.tl:404`: `local mod = api.type.ModuleDesc.new()`
- `fileName : ResName`: Resource name of the module file
- `type : string`: Type of the module, a module of a specific type can be built only on slots with the same type
- `autoRemovable : boolean`: Whether it is automatically removed when building something on it (e.g. farm fields)
- `availability : ModelMetadata.Availability`: Availability by year for module builder
- `description : ModelMetadata.Description`: Localized strings and icons paths for user interface
- `category : ModelMetadata.Category`: Unused
- `cost : ModelMetadata.Cost`: Cost per unit
- `menuCategory : ModelMetadata.MenuCategory`: Configure category, order and filter behaviour in the construction menu
- `builderAudioRes : ResName`: Sound resource (.builder_audio) for construction action
- `metadata : table`: Additional metadata as table
- `metadata_native : Map<string, number>`: Additional metadata as native map
- `relativePath : string`: Deprecated
- `updateScript : ScriptRef`: Script function that gets called once per build module after the updateScript of the construction has run
- `getModelsScript : ScriptRef`: Script function used to generate module preview when building with module builder

## Modifiers (record)

```teal
global record Modifiers
```

Environmental modifiers affecting construction or vehicle performance

- `new : function() : Modifiers`: Creates a new, empty `Modifiers`.
- `new : function(copy : Modifiers) : Modifiers`: Creates a new, empty `Modifiers`.
- `equals : function(a : Modifiers, b : Modifiers) : boolean`: Returns `true` when both `Modifiers` values are equal.
- `noiseModifier : number`: Noise level modifier as an additive factor
- `pollutionModifier : number`: Pollution level modifier as an additive factor
- `maxSpeedModifier : number`: Top speed modifier as an additive factor

## Slot (record)

```teal
global record Slot
```

Modular construction slot

- `autoFill : boolean`: Autofill slots are used by field construction to add satellites automatically when the construction spawns
- `id : integer`: Unique identifier for the slot, can also be used to reconstruct the coordinates of the slot when encoded as specific pattern
- `type : string`: Type of the slot: only modules of the same type can be built on this slot
- `transf : Mat4f`: Transformation relative to the construction origin
- `spacing : Vec4f`: Extent of the slot relative to it's origin, (x, y) are the distances to the left, resp. right, and (z, w) are the distances
to the south and north part (when seen from above)
- `height : number`: Height of the slot for collision purposes
- `shape : integer`: Shape of the module snap point visualization, 0 = square, 1 = triangle, 2 = rectangle
- `alignToTerrain : boolean`: Slots with this flag will be moved vertically to macht the height of the terrain (used e.g. for fields)
- `replaceable : boolean`: Modules in slots with this flag can be replaced by other modules of the same type without bulldozing first

## SlotConfig (record)

```teal
global record SlotConfig
```

Slot configuration can specify additional properties for slots of a given type

- `maxModules : integer`: Max number of modules of this type that might be built
- `message : string`: Message for when this limit is reached
- `skipCollisionCheck : boolean`: Slots with this flag will not collide with other objects, but only the slot itself, the final construction
still has to pass the usual collision checks

## Signal (record)

```teal
global record Signal
```

Contains information about a street/track signal

- `edgePr : {EdgeId, boolean}`: EdgeId and direction pair
- `type : Type`: The type of the signal
- `state : integer`: Current animation playing
- `stateTime : integer`: Timer for the animation

### Signal.Type (record)

```teal
record Type
```

Enum containing the type of the signal

- `SIGNAL : Type`: Normal signal
- `ONE_WAY_SIGNAL : Type`: One-way signal
- `WAYPOINT : Type`: Waypoint

## VehiclePart (record)

```teal
global record VehiclePart
```

Configuration for a single vehicle part/unit

- `new : function() : VehiclePart`: Creates a new, empty `VehiclePart`.
- `modelId : integer`: Id of the vehicle model in `api.res.modelRep`.
- `reversed : boolean`: Whether the vehicle part is reversed (facing backwards)
- `compartment2loadConfig : {LoadConfig}`: Cargo configuration for the model. One entry per compartment. -1 means automatic selection of load config.
- `color : Vec3f`: Custom color of the part as RGB; a negative `x` means no custom color is set.

## TransportVehiclePart (record)

```teal
global record TransportVehiclePart
```
Access: `api.type.TransportVehiclePart`

A subsection of a Transport Vehicle.

- `new : function() : TransportVehiclePart`  (call as `api.type.TransportVehiclePart.new(…)`): Creates a new TransportVehiclePart.
    - returns: New TransportVehiclePart
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_util.tl:9`: `local part = api.type.TransportVehiclePart.new()`
- `part : VehiclePart`: Base vehicle part configuration
- `purchaseTime : integer`: Game time when this part was purchased
- `maintenanceChange : number`: Current rate of change of `maintenanceState`; the vehicle window shows it as the trend next to the condition.
- `maintenanceState : number`: Condition of the part, from 0 to 1 (shown as a percentage in the vehicle window).
- `autoLoadConfig : {boolean}`: Automatic load configuration per compartment

## TransportVehicleConfig (record)

```teal
global record TransportVehicleConfig
```
Access: `api.type.TransportVehicleConfig`

Complete configuration for a TransportVehicle (train consist, bus, etc.)

- `new : function() : TransportVehicleConfig`  (call as `api.type.TransportVehicleConfig.new(…)`): Creates a new, empty `TransportVehicleConfig`.
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:8500`: `local newConfig = api.type.TransportVehicleConfig.new(tv.transportVehicleConfig)`
- `new : function(TransportVehicleConfig) : TransportVehicleConfig`  (call as `api.type.TransportVehicleConfig.new(…)`): Creates a new, empty `TransportVehicleConfig`.
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:8500`: `local newConfig = api.type.TransportVehicleConfig.new(tv.transportVehicleConfig)`
- `vehicles : {TransportVehiclePart}`: List of all vehicle parts in order
- `vehicleGroups : {integer}`: Vehicles can be composed into groups, each entry is the size of the group.
The sum of all entries must be the total of vehicles.

Example:
 for a train made of a multiple unit with three cars and 1 extra waggon would be `{3, 1}`.
- `muFileNames : {string}`: for each group, if it is a multiple unit, stores the res name

## LandVehicleMoveInfo (record)

```teal
global record LandVehicleMoveInfo
```

Movement information for land vehicles (trains, trams, road vehicles)

- `availPower : number`: Power currently available to the vehicle; `vehicleAvailPower` lists it per vehicle part.
- `traction : number`
- `tractiveEffort : number`: Current tractive effort of the vehicle.
- `vehicleAvailPower : {number}`: Available power per vehicle part

## LineVehicleInfo (record)

```teal
global record LineVehicleInfo
```

Configuration of vehicles that can use the line

- `transportModes : {TransportMode : boolean}`: Type of tranport modes that can use the line

## ShipInfo (record)

```teal
global record ShipInfo
```

Movement information for ships

- `topSpeed : number`: Maximum speed (in m/s)
- `maxTractiveEffort : number`: Maximum tractive effort of the ship.
- `availablePower : number`: Power currently available to the ship.

## AircraftInfo (record)

```teal
global record AircraftInfo
```

Movement information for aircraft

- `weight : number`: Aircraft weight (in kg)
- `topSpeed : number`: Maximum speed (in m/s)
- `landingDistance : number`: Distance required for landing (in meters)
- `maxThrust : number`: Maximum engine thrust (in kN)
- `idleThrust : number`: Idle engine thrust (in kN)
- `mainGear : Vec2f`: Position of main landing gear
- `bodyAngle : number`
- `forwardOffset : number`: How far forward the front of the airfract is (in meters)
- `backwardOffset : number`: How far backwards the front of the airfract is (in meters)
- `taxiHoldOffset : number`: How far from the hold line the aircraft is supposed to wait (meters)
- `hasFlaps : boolean`: Whether the aircraft has flaps
- `isHelicopter : boolean`: Whether this is a helicopter

## ConstructionTemplateData (record)

```teal
global record ConstructionTemplateData
```

Specify additional variants of the construction by providing a list of modules and the slot in which they belong

- `initialModules : {ResName : integer}`: List of modules that come with the construction
- `params : {ScriptParam}`: Additional scriptparams that are used for this specific template

## StaticConstructionTemplate (record)

```teal
global record StaticConstructionTemplate
```

Static construction template configuration

- `initialModules : {ResName : integer}`: Modules that come with the construction, mapped from module resource name to slot id.

## DynamicConstructionTemplate (record)

```teal
global record DynamicConstructionTemplate
```
Access: `api.type.DynamicConstructionTemplate`

Dynamic construction template configuration

- `params : {ScriptParam}`: Additional script parameters used for this template.

## ConstructionTemplate (record)

```teal
global record ConstructionTemplate
```

Construction template with availability and type information

- `availability : ModelMetadata.Availability`: Specific availability (date) for the template (optional)
- `desc : ModelMetadata.Description`: Localized strings and icons paths for user interface
- `data : ConstructionTemplateData`: Additional slot configuration and params
- `menuCategory : ModelMetadata.MenuCategory`: Configure category, order and filter behaviour in the construction menu
- `builderAudioRes : ResName`: Sound resource (.builder_audio) for construction action
- `configureHudIconsScript : ScriptRef`: Reference to a function that configures the HUD icons shown while building this template; overrides `ConstructionDesc.configureHudIconsScript` when set.

## TownBuildingParams (record)

```teal
global record TownBuildingParams
```

Town building parameters

- `landUseType : LandUseType`: One of residential, commercial or industrial
- `level : integer`: Level, which is directly related to capacity and size of the building
- `parcelSize : Vec2i`: (x, y) dimension in parcel cells for the building

## BuildingType (record)

```teal
global record BuildingType
```

Specification for town building types

- `constructionFile : string`: Resource name of the town building's construction.
- `params : TownBuildingParams`: Town building parameters: land use type, level and parcel size.

## ConstructionInstanceMetadata (record)

```teal
global record ConstructionInstanceMetadata
```

Opaque type of the metadata stored with a built construction.

## SubconstructionInstanceMetadata (record)

```teal
global record SubconstructionInstanceMetadata
```

Opaque type of the metadata stored with a subconstruction instance.

## ConstructionDescMetadata (record)

```teal
global record ConstructionDescMetadata
```

Type of the `metadata` table of a construction description. The API declares its `emissionConfig` entry (`noise`, `pollution`); other entries are read and written through the `MetadataUtil` accessors.

- `emissionConfig : EmissionConfig`: Noise and pollution emitted by the construction.

### ConstructionDescMetadata.EmissionConfig (record)

```teal
record EmissionConfig
```

Noise and pollution emission values of a construction.

- `noise : number`: Noise emission.
- `pollution : number`: Pollution emission.

## ConstructionDesc (record)

```teal
global record ConstructionDesc
```
Access: `api.type.ConstructionDesc`

Describes a construction

- `new : function(?ConstructionDesc) : ConstructionDesc`  (call as `api.type.ConstructionDesc.new(…)`): Creates a construction description, optionally as a copy of an existing one.
    - used 2x in the base game, e.g. `mods/release/urbangames_campaign_mission_03/content/mod.script.tl:10`: `local desc = api.type.ConstructionDesc.new(initialDesc)`
- `updateScript : ScriptRef`: Reference to the construction's `updateFn`, which builds the construction from its parameters, with optional `params` passed to it.

    ```lua
    constr.updateScript = {
    	fileName = "pollution_cleanup_facility.script@updateFn",
    	params = { stockListConfig = stockListConfig },
    }
    ```

- `configureLayerScript : ScriptRef`: Reference to a function `configureLayerFn(captureParams, layerConfig)` that returns the `LayerConfig` (3D overlay layer) to show while building the construction.
- `configureHudIconsScript : ScriptRef`: Reference to a function that returns the HUD icon configuration (`ConstructionActionHudIcons`) shown while building the construction.
- `hudIconScript : ScriptRef`: Reference to a function `hudIconFn(captureParams, params, userParam)` that returns a custom HUD icon for the built construction.
- `entityWindowScript : ScriptRef`: Reference to a function `entityWindowFn(...)` that creates custom content for the construction's entity window.
- `fileName : string`: A file name or unique identifier for the construction.
- `availability : ModelMetadata.Availability`: Date range in which the construction is available.
- `soundConfig : ModelMetadata.SoundConfig`: Defines the sound emitted by the model when in-game
- `description : ModelMetadata.Description`: Localized name, description and icons for the user interface.
- `categoryList : ModelMetadata.CategoryList`: Category tags of the construction, e.g. the climates (`"temperate.eco"`) it belongs to.
- `params : {ScriptParam}`: A list of parameters that can be used to configure the construction.
- `categories : {string}`
- `order : number`
- `menuCategory : ModelMetadata.MenuCategory`: Category and order of the construction in the construction menu.
- `constructionTemplates : {ConstructionTemplate}`: [DEPRECATED] Use metaconstructions instead
- `constructionTemplates_native : Vector<ConstructionTemplate>`: [DEPRECATED] Use metaconstructions instead
- `placementTag : string`: Tag identifying the industry type, e.g. `"coal_mine"` or `"oil_platform"`; map generation and the industries script use it (`GameMap.Industry.tag`).
- `metadata : ConstructionDescMetadata`: Additional metadata of the construction, read by game scripts through metadata accessors.
- `snapping : SnapConfig`: Snapping options of the construction.
- `undergroundView : boolean`: Whether the construction menu enables the underground view parameter when building.
- `heightAdjustable : boolean`: Whether the construction menu offers the height slider when building.
- `isIndustry : boolean`: Whether the construction is an industry.
- `edgeObject : EdgeObject`: Optional edge object configuration
- `townBuildingParams : TownBuildingParams`: Town building parameters (land use type, level, parcel size) for constructions that are town buildings.
- `costsYearProgression : boolean`: Whether the construction's costs depend on the current year; the construction menu then adjusts the shown costs and keeps the build year in the `year` parameter.

### ConstructionDesc.SnapConfig (record)

```teal
record SnapConfig
```
Access: `api.type.ConstructionDesc.SnapConfig`

Snapping options of a construction.

- `rail : boolean`: Whether the construction snaps to rail tracks.
- `road : boolean`: Whether the construction snaps to roads.
- `water : boolean`: Whether the construction snaps to water.

### ConstructionDesc.EdgeObject (record)

```teal
record EdgeObject
```
Access: `api.type.ConstructionDesc.EdgeObject`

Settings for constructions placed as objects on a street or track edge (such as the small street stops).

- `minDistToCrossing : number`: Minimum distance to crossings (in meters)
- `snapToStreet : boolean`: Whether edge object snaps to streets
- `snapToTrack : boolean`: Whether edge object snaps to tracks
- `catchmentCargo : boolean`: Whether edge object has cargo catchment area
- `catchmentPassengers : boolean`: Whether edge object has passenger catchment area
- `catchmentAreaRadius : number`: Catchment area radius (in meters)

## ConstructionScriptParam (record)

```teal
global record ConstructionScriptParam
```

Parameter of a meta-construction (`MetaConstructionDesc.params`), shown in the construction menu. Unlike `ScriptParam` it uses string enums for `uiType`, `location` and `displayMode`.

- `key : string`: Unique key that is passed to the construction functions
- `name : string`: Display name
- `group : string`: Display Group:
- If hideLabel is true, items will be grouped horizontally
- Else items will be separated by a divider
- `tooltip : string`: Tooltip to display over certain parameters
- `values : {string}`: All display values of the params
- `numbers : {number}`: Specified return values instead of default 1, 2, 3...
- `tooltips : {string}`: Tooltips for the individual values
- `postConstructionModifiable : boolean`: This param can be changed when the construction is already built
- `hideLabel : boolean`: Allow to hide the label
- `images : {string}`: Image representations of the values (for Compact Mode)
- `defaultIndex : Index`: Default index/state
- `uiType : Type`: Type of ui shown in user interface selection
- `location : Location`: construction menu has two possible locations for parameters: default or toolbar
- `displayMode : DisplayMode`: displayMode controls whether the parameter is displayed in default or compact mode
- `tags : {string}`: Tags associated with the script param for filtering
- `yearFrom : integer`: Availability from
- `yearTo : integer`: Availability to
- `checkEnabledScript : ScriptRef`: callback function to allow dynamic disabling or hiding of the paramter
function(params : {string : number}, scriptRefParams : table) : ConstructionMenuParam.CheckEnabledResult
- `stepValueScript : ScriptRef`: callback function to allow returning any value on input actions
function(value : number, direction : integer, precise : boolean) : number
- `formatValueScript : ScriptRef`: callback function to allow formatting the value display in the UI
function(value : number) : string

### ConstructionScriptParam.Type (enum)

```teal
enum Type
```

Kind of control used to show the parameter (`uiType`).

- `"Button"`: Push button
- `"Slider"`: Slider
- `"ComboBox"`: Drop-down combo box
- `"IconButton"`: Button with an icon
- `"CheckBox"`: Check box

### ConstructionScriptParam.Location (enum)

```teal
enum Location
```

Where in the construction menu the parameter is shown (`location`).

- `"Default"`: Default construction parameter panel
- `"Toolbar"`: Toolbar area of construction panel

### ConstructionScriptParam.DisplayMode (enum)

```teal
enum DisplayMode
```

How the parameter label and value are laid out (`displayMode`).

- `"Default"`: For BottomBar==on is Horizontal, for BottomBar==off is Sparse
- `"Compact"`: Value is collapsed, full parameter is shown in a callout
- `"Horizontal"`: Label is shown left of value
- `"Vertical"`: Label is shown above value
- `"Sparse"`: Label is shown above the value, and the value might grow to multiple lines

## MetaConstructionDesc (record)

```teal
global record MetaConstructionDesc
```

A Metaconstruction is a collection of constructions and modules that can be simultaneously be built
(replaces the deprecated construction templates)

- `new : function(?MetaConstructionDesc) : MetaConstructionDesc`: Creates a new MetaConstructionDesc object
- `availability : ModelMetadata.Availability`: Main menu availability range
- `description : ModelMetadata.Description`: Textual description and icons
- `soundConfig : ModelMetadata.SoundConfig`: Configuration for building sounds
- `categoryList : ModelMetadata.CategoryList`: Additional tags
- `params : {ConstructionScriptParam}`: Parameters passed to the template function
- `menuCategory : ModelMetadata.MenuCategory`: Menu category
- `undergroundView : boolean`: Enable underground parameters when building
- `heightAdjustable : boolean`: Enable height adjustment options when building
- `isIndustry : boolean`: Whether the meta-construction is an industry.
- `heightModView : boolean`: Enable heigh modification grid visualization when building
- `configureLayerScript : ScriptRef`: Custom layer function for display when building
- `configureHudIconsScript : ScriptRef`: Custom layer function for display when building
- `metadata : ConstructionDescMetadata`: Additional user defined metadata
- `relativePath : string`: Relative path to use withing template script
- `createTemplateScript : ScriptRef`: A definition of a script function returning a list of construction, modules and transforms
- `order : integer`

## Clock (record)

```teal
global record Clock
```
Access: `api.type.Clock`

A very precise clock.

- `new : function() : Clock`  (call as `api.type.Clock.new(…)`): Constructs a new intance of a clock and starts its timer
    - used 3x in the base game, e.g. `base/content/gui/gui/main/credits_react_util.tl:129`: `local clockState = react.useState(api.type.Clock.new()) as ReactStateT<Clock>`
- `getTime : function(Clock) : integer`  (call as `api.type.Clock.getTime(…)`): Get elapsed time since clock construction in microseconds

## LoaderHelper (record)

```teal
global record LoaderHelper
```
Access: `api.type.LoaderHelper`

Helper class that allows loading and calling files in other modules

- `useFn : function(LoaderHelper, string)`  (call as `api.type.LoaderHelper.useFn(…)`): Loads the script file named in a function reference of the form `"<file>@<function>"` so that its functions can be looked up.
    - notes: Use it through `util.useFn(ref)` from `/scripts/util.tl`, which calls `loaderHelper:useFn(ref)` and then returns the referenced function from the global `game` table.

## Connection (record)

```teal
global record Connection
```
Access: `api.type.Connection`

An object used to disconnect callbacks.

- `disconnect : function(Connection)`  (call as `api.type.Connection.disconnect(…)`): The callback will be no longer called

## LandUseType (record)

```teal
global record LandUseType
```

Enumeration for land use types

- `RESIDENTIAL : LandUseType`: Residential land use (`"res"` in map files).
- `COMMERCIAL : LandUseType`: Commercial land use (`"com"` in map files).
- `INDUSTRIAL : LandUseType`: Industrial land use (`"ind"` in map files).

## SignalType (record)

```teal
global record SignalType
```

Enum containing the type of the signal

- `PATH_SIGNAL : SignalType`
- `WAYPOINT : SignalType`
- `ONE_WAY_PATH_SIGNAL : SignalType`

## VehicleEngineType (record)

```teal
global record VehicleEngineType
```

Engine type of a vehicle (`ModelMetadata.VehicleEngine.type`); the vehicle store filters and labels vehicles by it.

- `HORSE : VehicleEngineType`
- `STEAM : VehicleEngineType`
- `ELECTRIC : VehicleEngineType`
- `DIESEL : VehicleEngineType`

## StockListType (record)

```teal
global record StockListType
```

Defines the stock list type

- `InputStock : StockListType`
    - used 5x in the base game, e.g. `base/content/gui/gui/entity_window/town_building/town_building.tl:87`: `if stockList.stocks[inputStocks[i]].type == api.type.enum.StockListType.InputStock then`
- `OutputStock : StockListType`
    - used 3x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:186`: `elseif config.stockType == api.type.enum.StockListType.OutputStock then`
- `StorageStock : StockListType`
    - used 1x in the base game, e.g. `base/content/gui/gui/main/cargo_util.tl:188`: `elseif config.stockType == api.type.enum.StockListType.StorageStock then`

## TramTrackType (record)

```teal
global record TramTrackType
```

Defines the type of a tram track.

- `NO : TramTrackType`
- `YES : TramTrackType`
- `ELECTRIC : TramTrackType`

## Platform (record)

```teal
global record Platform
```

Platform represents the platform type.

- `Windows : Platform`
- `Linux : Platform`
- `Mac : Platform`
- `XBoxSeries : Platform`
- `PS5 : Platform`
- `PS : Platform`
- `XBox : Platform`
- `Desktop : Platform`
- `Console : Platform`

## ConsoleGen (record)

```teal
global record ConsoleGen
```

- `Gen9pro : ConsoleGen`
- `Gen9standard : ConsoleGen`
- `Gen9base : ConsoleGen`
- `PC : ConsoleGen`

## UgcAccessLimitedReason (record)

```teal
global record UgcAccessLimitedReason
```

Represent the reason why the user doesn't have access to UGC content

- `None : UgcAccessLimitedReason`
- `NotSignedIn : UgcAccessLimitedReason`
- `Restricted : UgcAccessLimitedReason`
- `Error : UgcAccessLimitedReason`

## Vendor (record)

```teal
global record Vendor
```

Vendor represents the vendor (store) type.

- `Steam : Vendor`
- `Gog : Vendor`
- `Eos : Vendor`
- `MacAppstore : Vendor`
- `Microsoft : Vendor`
- `Sony : Vendor`
- `Dev : Vendor`

## RoadType (record)

```teal
global record RoadType
```

Road vs Track type.

- `STREET : RoadType`
- `TRACK : RoadType`

## ScriptParamType (record)

```teal
global record ScriptParamType
```

UI Type for displaying the parameter. Used to configure the parameters used for constructions.

- `Button : ScriptParamType`
- `Slider : ScriptParamType`
- `ComboBox : ScriptParamType`
- `IconButton : ScriptParamType`
- `CheckBox : ScriptParamType`

## ScriptParamLocation (record)

```teal
global record ScriptParamLocation
```

Controls bottom bar mode of the parameter

- `Default : ScriptParamLocation`
- `Toolbar : ScriptParamLocation`: special location for construction menu parameters, which will be displayed in the tool bar on the left side

## ScriptParamDisplayMode (record)

```teal
global record ScriptParamDisplayMode
```

Controls how the parameter is displayed in the UI

- `Default : ScriptParamDisplayMode`: For BottomBar==on is Horizontal, for BottomBar==off is Sparse
- `Compact : ScriptParamDisplayMode`: Value is collapsed, full parameter is shown in a callout
- `Horizontal : ScriptParamDisplayMode`: Value is shown left of value
- `Vertical : ScriptParamDisplayMode`: Value is shown above value
- `Sparse : ScriptParamDisplayMode`: Label is shown above the value, and the value might grow to multiple lines

## DistanceSpeedUnit (record)

```teal
global record DistanceSpeedUnit
```

DistanceSpeedUnit represents the unit of measurement used for distance and speed.

- `Metric : DistanceSpeedUnit`
- `Imperial : DistanceSpeedUnit`

## HudEffectMode (record)

```teal
global record HudEffectMode
```

HudEffectMode configures the HUD effects

- `Always : HudEffectMode`
- `CockpitOnly : HudEffectMode`
- `Never : HudEffectMode`

## FontScaleClass (record)

```teal
global record FontScaleClass
```

FontScaleClass represents the current UI font scaling factor.

- `Small : FontScaleClass`
- `Medium : FontScaleClass`: Medium text scaling (settings value `1`, "Medium" in the Text Scaling option).
- `Large : FontScaleClass`: Large text scaling (settings value `2`, "Large" in the Text Scaling option).

## MoneyMode (record)

```teal
global record MoneyMode
```

MoneyMode represents the money mode.

- `RAW : MoneyMode`: Full money format; all digits are written out ("Full" in the Money Number Format setting).
- `KMB : MoneyMode`: Short money format with K/M/B suffixes, for example `1K` for 1,000 ("Short" in the Money Number Format setting).

## PowerUnit (record)

```teal
global record PowerUnit
```

PowerUnit represents the unit of measurement used for power.

- `KW : PowerUnit`: Power shown in kilowatts (kW).
- `HP : PowerUnit`: Power shown in horsepower (hp).

## WeightUnit (record)

```teal
global record WeightUnit
```

WeightUnit represents the unit of measurement used for weight.

- `T : WeightUnit`: Weight shown in metric tonnes (t).
- `KG : WeightUnit`: Weight shown in kilograms (kg).
- `TN_SH : WeightUnit`: Weight shown in short tons (US tons).
- `LB : WeightUnit`: Weight shown in pounds (lb).

## ForceUnit (record)

```teal
global record ForceUnit
```

ForceUnit represents the unit of measurement used for force.

- `KN : ForceUnit`: Force shown in kilonewtons (kN).
- `KGF : ForceUnit`: Force shown in kilogram-force (kgf).
- `LBF : ForceUnit`: Force shown in pound-force (lbf).

## Gender (record)

```teal
global record Gender
```

Gender of a person model, used in `ModelMetadata.Person.gender`.

- `FEMALE : Gender`
- `MALE : Gender`

## FlightState (record)

```teal
global record FlightState
```

Flight State of an Aircraft

- `Taxi0 : FlightState`
- `TakeOff : FlightState`
- `Flight : FlightState`
- `Approach : FlightState`
- `Landing : FlightState`
- `Taxi1 : FlightState`

## Easing (record)

```teal
global record Easing
```

Easing functions for particle curve interpolation

- `Linear : integer`
- `EaseOutSqrt : integer`
- `EaseInSine : integer`
- `EaseOutSine : integer`
- `EaseInOutSine : integer`
- `EaseInQuad : integer`
- `EaseOutQuad : integer`
- `EaseInOutQuad : integer`
- `EaseInCubic : integer`
- `EaseOutCubic : integer`
- `EaseInOutCubic : integer`
- `EaseInQuart : integer`
- `EaseOutQuart : integer`
- `EaseInOutQuart : integer`
- `EaseInQuint : integer`
- `EaseOutQuint : integer`
- `EaseInOutQuint : integer`
- `EaseInExpo : integer`
- `EaseOutExpo : integer`
- `EaseInOutExpo : integer`
- `EaseInCirc : integer`
- `EaseOutCirc : integer`
- `EaseInOutCirc : integer`
- `EaseInBack : integer`
- `EaseOutBack : integer`
- `EaseInOutBack : integer`
- `EaseInElastic : integer`
- `EaseOutElastic : integer`
- `EaseInOutElastic : integer`
- `EaseInBounce : integer`
- `EaseOutBounce : integer`
- `EaseInOutBounce : integer`

## BackgroundContrastMode (record)

```teal
global record BackgroundContrastMode
```

Where the white background contrast layer replaces normal world rendering. Stored in
`AppConfig.backgroundContrastMode` (setting "Enable Background Contrast Layer").

- `Never : integer`: The contrast layer is never used; the normal world rendering is shown (settings value `0`).
- `LineVehicleManager : integer`: The contrast layer is used in the line manager only (settings value `1`).
- `ModuleBuilder : integer`: The contrast layer is used in the module builder only (settings value `2`).
- `Both : integer`: The contrast layer is used in both the line manager and the module builder (settings value `3`).

## Enum (record)

```teal
global record Enum
```
Access: `api.type.enum`

Collection of all enumerations of the API, reached as `api.type.enum`.

Notes:

`enum` is a reserved word in Teal, so Teal scripts write `api.type["enum"].X`; Lua scripts can
also use `api.type.enum.X`.

Example:
```lua
if appConfig.backgroundContrastMode == api.type["enum"].BackgroundContrastMode.Both then
  -- ...
end
```

- `LogLevel : LogLevel`: The `LogLevel` enumeration, reached as `api.type.enum.LogLevel` (in Teal `api.type["enum"].LogLevel`).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:4333`: `local makeLevelTextView = function(level : api.type.enum.LogLevel, consoleOnly? : boolean) : TreeNodeId`
- `SaveGameType : SaveGameType`: The `SaveGameType` enumeration, reached as `api.type.enum.SaveGameType` (in Teal `api.type["enum"].SaveGameType`).
- `DateFormat : DateFormat`: The `DateFormat` enumeration, reached as `api.type.enum.DateFormat` (in Teal `api.type["enum"].DateFormat`).
- `EdgeObjectType : EdgeObjectType`: The `EdgeObjectType` enumeration, reached as `api.type.enum.EdgeObjectType` (in Teal `api.type["enum"].EdgeObjectType`).
- `TransportVehicleState : TransportVehicleState`: The `TransportVehicleState` enumeration, reached as `api.type.enum.TransportVehicleState` (in Teal `api.type["enum"].TransportVehicleState`).
- `Carrier : Carrier`: The `Carrier` enumeration, reached as `api.type.enum.Carrier` (in Teal `api.type["enum"].Carrier`).
- `SignalType : SignalType`: The `SignalType` enumeration, reached as `api.type.enum.SignalType` (in Teal `api.type["enum"].SignalType`).
- `VehicleEngineType : VehicleEngineType`: The `VehicleEngineType` enumeration, reached as `api.type.enum.VehicleEngineType` (in Teal `api.type["enum"].VehicleEngineType`).
- `LineProblem : LineProblem`: The `LineProblem` enumeration, reached as `api.type.enum.LineProblem` (in Teal `api.type["enum"].LineProblem`).
- `LineStationProblem : LineStationProblem`: The `LineStationProblem` enumeration, reached as `api.type.enum.LineStationProblem` (in Teal `api.type["enum"].LineStationProblem`).
- `VehicleProblem : VehicleProblem`: The `VehicleProblem` enumeration, reached as `api.type.enum.VehicleProblem` (in Teal `api.type["enum"].VehicleProblem`).
- `VehicleIconType : VehicleIconType`: The `VehicleIconType` enumeration, reached as `api.type.enum.VehicleIconType` (in Teal `api.type["enum"].VehicleIconType`).
- `TownProblem : TownProblem`: The `TownProblem` enumeration, reached as `api.type.enum.TownProblem` (in Teal `api.type["enum"].TownProblem`).
- `StockListType : StockListType`: The `StockListType` enumeration, reached as `api.type.enum.StockListType` (in Teal `api.type["enum"].StockListType`).
- `ConstructionType : ConstructionType`: The `ConstructionType` enumeration, reached as `api.type.enum.ConstructionType` (in Teal `api.type["enum"].ConstructionType`).
- `TransportMode : TransportMode`: The `TransportMode` enumeration, reached as `api.type.enum.TransportMode` (in Teal `api.type["enum"].TransportMode`).
    - used 3x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:3`: `local getAllTransportModes = function() : {api.type.enum.TransportMode}`
- `TramTrackType : TramTrackType`: The `TramTrackType` enumeration, reached as `api.type.enum.TramTrackType` (in Teal `api.type["enum"].TramTrackType`).
- `LandUseType : LandUseType`: The `LandUseType` enumeration, reached as `api.type.enum.LandUseType` (in Teal `api.type["enum"].LandUseType`).
- `InputMode : InputMode`: The `InputMode` enumeration, reached as `api.type.enum.InputMode` (in Teal `api.type["enum"].InputMode`).
- `GamepadType : GamepadType`: The `GamepadType` enumeration, reached as `api.type.enum.GamepadType` (in Teal `api.type["enum"].GamepadType`).
- `VramBudgetStatus : VramBudgetStatus`: The `VramBudgetStatus` enumeration, reached as `api.type.enum.VramBudgetStatus` (in Teal `api.type["enum"].VramBudgetStatus`).
- `Platform : Platform`: The `Platform` enumeration, reached as `api.type.enum.Platform` (in Teal `api.type["enum"].Platform`).
- `Vendor : Vendor`: The `Vendor` enumeration, reached as `api.type.enum.Vendor` (in Teal `api.type["enum"].Vendor`).
- `ConsoleGen : ConsoleGen`: The `ConsoleGen` enumeration, reached as `api.type.enum.ConsoleGen` (in Teal `api.type["enum"].ConsoleGen`).
- `TrafficLightPreference : TrafficLightPreference`: The `TrafficLightPreference` enumeration, reached as `api.type.enum.TrafficLightPreference` (in Teal `api.type["enum"].TrafficLightPreference`).
- `BaseEdgeType : BaseEdgeType`: The `BaseEdgeType` enumeration, reached as `api.type.enum.BaseEdgeType` (in Teal `api.type["enum"].BaseEdgeType`).
- `MoveMode : MoveMode`: The `MoveMode` enumeration, reached as `api.type.enum.MoveMode` (in Teal `api.type["enum"].MoveMode`).
- `RoadType : RoadType`: The `RoadType` enumeration, reached as `api.type.enum.RoadType` (in Teal `api.type["enum"].RoadType`).
- `ScriptParamType : ScriptParamType`: The `ScriptParamType` enumeration, reached as `api.type.enum.ScriptParamType` (in Teal `api.type["enum"].ScriptParamType`).
- `ScriptParamLocation : ScriptParamLocation`: The `ScriptParamLocation` enumeration, reached as `api.type.enum.ScriptParamLocation` (in Teal `api.type["enum"].ScriptParamLocation`).
- `ScriptParamDisplayMode : ScriptParamDisplayMode`: The `ScriptParamDisplayMode` enumeration, reached as `api.type.enum.ScriptParamDisplayMode` (in Teal `api.type["enum"].ScriptParamDisplayMode`).
- `DistanceSpeedUnit : DistanceSpeedUnit`: The `DistanceSpeedUnit` enumeration, reached as `api.type.enum.DistanceSpeedUnit` (in Teal `api.type["enum"].DistanceSpeedUnit`).
- `HudEffectMode : HudEffectMode`: The `HudEffectMode` enumeration, reached as `api.type.enum.HudEffectMode` (in Teal `api.type["enum"].HudEffectMode`).
- `FontScaleClass : FontScaleClass`: The `FontScaleClass` enumeration, reached as `api.type.enum.FontScaleClass` (in Teal `api.type["enum"].FontScaleClass`).
- `MoneyMode : MoneyMode`: The `MoneyMode` enumeration, reached as `api.type.enum.MoneyMode` (in Teal `api.type["enum"].MoneyMode`).
- `PowerUnit : PowerUnit`: The `PowerUnit` enumeration, reached as `api.type.enum.PowerUnit` (in Teal `api.type["enum"].PowerUnit`).
- `WeightUnit : WeightUnit`: The `WeightUnit` enumeration, reached as `api.type.enum.WeightUnit` (in Teal `api.type["enum"].WeightUnit`).
- `ForceUnit : ForceUnit`: The `ForceUnit` enumeration, reached as `api.type.enum.ForceUnit` (in Teal `api.type["enum"].ForceUnit`).
- `Gender : Gender`: The `Gender` enumeration, reached as `api.type.enum.Gender` (in Teal `api.type["enum"].Gender`).
- `UgcAccessLimitedReason : UgcAccessLimitedReason`: The `UgcAccessLimitedReason` enumeration, reached as `api.type.enum.UgcAccessLimitedReason` (in Teal `api.type["enum"].UgcAccessLimitedReason`).
- `FlightState : FlightState`: The `FlightState` enumeration, reached as `api.type.enum.FlightState` (in Teal `api.type["enum"].FlightState`).
- `Easing : Easing`: The `Easing` enumeration, reached as `api.type.enum.Easing` (in Teal `api.type["enum"].Easing`).
- `PdxErrorCodes : PdxErrorCodes`: The `PdxErrorCodes` enumeration, reached as `api.type.enum.PdxErrorCodes` (in Teal `api.type["enum"].PdxErrorCodes`).
- `PdxThirdPartyLinkState : PdxThirdPartyLinkState`: The `PdxThirdPartyLinkState` enumeration, reached as `api.type.enum.PdxThirdPartyLinkState` (in Teal `api.type["enum"].PdxThirdPartyLinkState`).
- `PdxThirdPartyAutoLoginPromptState : PdxThirdPartyAutoLoginPromptState`: The `PdxThirdPartyAutoLoginPromptState` enumeration, reached as `api.type.enum.PdxThirdPartyAutoLoginPromptState` (in Teal `api.type["enum"].PdxThirdPartyAutoLoginPromptState`).
- `BackgroundContrastMode : BackgroundContrastMode`: The `BackgroundContrastMode` enumeration, reached as `api.type.enum.BackgroundContrastMode` (in Teal `api.type["enum"].BackgroundContrastMode`).

## LineProblem (record)

```teal
global record LineProblem
```

Information about the problem of a line.

- `NOTHING : integer`: The line has no problem.
- `ZERO_OR_ONE_STATION : integer`: The line contains too few stations.
- `DOUBLE_STATIONS : integer`: A station appears twice in a row on the line.
- `INCOMPATIBLE_STATIONS : integer`: Stations are incompatible because conflicting stops are connected.
- `NO_PATH : integer`: The stations of the line could not be connected.
- `BAD_ALTERNATIVE_TERMINAL : integer`: Not all alternative terminals of the line could be connected.

## LineStationProblem (record)

```teal
global record LineStationProblem
```

Information about the problem of station within a line.

- `StopUseless : integer`: The stop cannot be used with the current vehicles of the line.
- `PassengerStopUseless : integer`: The passenger stop cannot be used by the cargo vehicles of the line.
- `CargoStopUseless : integer`: The cargo stop cannot be used by the passenger vehicles of the line.
- `AllPassengerStopUseless : integer`: All passenger stops of the line cannot be used by its cargo vehicles.
- `CargoTypeUnloadUseless : integer`: A configured cargo type cannot be unloaded at this stop.
- `CargoTypeLoadUseless : integer`: A configured cargo type cannot be loaded at this stop.

## VehicleProblem (record)

```teal
global record VehicleProblem
```

Information about the problem of a vehicle.

- `NoPathElectric : integer`: There is no electrified path for the vehicle.
- `NoPathShip : integer`: There is no path for big ships.
- `NoPathAircraft : integer`: There is no path for big aircraft.
- `NoPathGeneric : integer`: There is no path at all.
- `Blocked : integer`: Trains are blocking each other.

## VehicleIconType (record)

```teal
global record VehicleIconType
```

Icon type used to represent a vehicle.

- `Bus : VehicleIconType`
- `Truck : VehicleIconType`
- `TrainSteam : VehicleIconType`
- `TrainElectric : VehicleIconType`
- `TrainDiesel : VehicleIconType`
- `Tram : VehicleIconType`
- `Aircraft : VehicleIconType`
- `Helicopter : VehicleIconType`
- `Ship : VehicleIconType`

## TownProblem (record)

```teal
global record TownProblem
```

Town connection problem.

- `Disconnected : TownProblem`: The road connection between two towns has been interrupted.
- `Overlength : TownProblem`: The travel time between two towns is too long.

## ConstructionType (record)

```teal
global record ConstructionType
```

Type of the construction for displaying in the correct UI category and controlling specific behaviour for the type.

- `STREET_STATION : ConstructionType`
- `RAIL_STATION : ConstructionType`
- `AIRPORT : ConstructionType`
- `HARBOR : ConstructionType`
- `STREET_STATION_CARGO : ConstructionType`
- `RAIL_STATION_CARGO : ConstructionType`
- `HARBOR_CARGO : ConstructionType`
- `STREET_DEPOT : ConstructionType`
- `RAIL_DEPOT : ConstructionType`
- `WATER_DEPOT : ConstructionType`
- `INDUSTRY : ConstructionType`
- `WAREHOUSE : ConstructionType`
- `ASSET_DEFAULT : ConstructionType`
- `ASSET_TRACK : ConstructionType`
- `TOWN_BUILDING : ConstructionType`
- `STREET_CONSTRUCTION : ConstructionType`
- `NONE : ConstructionType`
- `AIRPORT_CARGO : ConstructionType`
- `TRACK_CONSTRUCTION : ConstructionType`
- `WATER_WAYPOINT : ConstructionType`

## TransportMode (record)

```teal
global record TransportMode
```

Defines the transport mode (this is used to blacklist/whitelist vehicles onto transport network edges).
These values are mosly used in a bit field to influence the pathfinding for vehicles and people.

- `PERSON : TransportMode`
    - used 2x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:5`: `api.type.enum.TransportMode.PERSON,`
- `CARGO : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:6`: `api.type.enum.TransportMode.CARGO,`
- `CAR : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:7`: `api.type.enum.TransportMode.CAR,`
- `BUS : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:8`: `api.type.enum.TransportMode.BUS,`
- `TRUCK : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:9`: `api.type.enum.TransportMode.TRUCK,`
- `TRAM : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:10`: `api.type.enum.TransportMode.TRAM,`
- `ELECTRIC_TRAM : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:11`: `api.type.enum.TransportMode.ELECTRIC_TRAM,`
- `TRAIN : TransportMode`
    - used 4x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:12`: `api.type.enum.TransportMode.TRAIN,`
- `ELECTRIC_TRAIN : TransportMode`
    - used 2x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:13`: `api.type.enum.TransportMode.ELECTRIC_TRAIN,`
- `AIRCRAFT : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:14`: `api.type.enum.TransportMode.AIRCRAFT,`
- `SHIP : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:15`: `api.type.enum.TransportMode.SHIP,`
- `SMALL_AIRCRAFT : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:16`: `api.type.enum.TransportMode.SMALL_AIRCRAFT,`
- `SMALL_SHIP : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:17`: `api.type.enum.TransportMode.SMALL_SHIP,`
- `HELICOPTER : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:18`: `api.type.enum.TransportMode.HELICOPTER,`
- `TRAM_TRACK : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:19`: `api.type.enum.TransportMode.TRAM_TRACK,`
- `ELECTRIC_TRAM_TRACK : TransportMode`
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_pathfinding_util.tl:20`: `api.type.enum.TransportMode.ELECTRIC_TRAM_TRACK,`

## ModelInstance (record)

```teal
global record ModelInstance
```

A model in the world, with complete information

- `modelId : integer`: The model id, referenced in modelRep
- `transf0 : Mat4f`: The model transform at the previous frame
- `transf : Mat4f`: The model transform
- `transformator : integer`: Zero-basd index of the transformator for the model (-1 if none)

## ThinModelInstance (record)

```teal
global record ThinModelInstance
```

A model in the world, with very light information

- `modelId : integer`: The model id, referenced in modelRep
- `pos : Vec3f`: The position in the world
- `rot : number`: The rotation angle (in radians)
- `scale : number`: The scale in all directions

## LineOrEdges (record)

```teal
global record LineOrEdges
```

Start of a path search: either a section of a line (`line` with stop indices) or a list of
edge positions (`edges`). Used in `PathFactoryInput.fromLineOrEdges`.

- `line : Engine.Entity`: Line entity the path starts on.
- `lineStop0 : integer`
- `lineStop1 : integer`
- `lineTerminal1 : {Engine.Entity, integer}`
- `edges : {EdgePos}`: Edge positions the path can start from.

## Path (record)

```teal
global record Path
```
Access: `api.type.Path`

A path comprised of EdgeId and directions

- `new : function() : Path`  (call as `api.type.Path.new(…)`): Create a new instance of a Path object
- `edges : {{EdgeId, boolean}}`: List of edges and their directions (true = forward, false = reverse)
- `endOffset : integer`: The actual end of the path (i.e. where to stop) as offset from the end
- `terminalDecisionOffset : integer`: The position at which the alternative terminal choice will start, as an offset from the end
- `earlyArrivalIndices : integer`

## PathPos (record)

```teal
global record PathPos
```
Access: `api.type.PathPos`

The position on a Path

- `new : function() : PathPos`  (call as `api.type.PathPos.new(…)`): Create a new instance of a PathPos object
- `edgeIndex : integer`: The zero-based index within a path
- `pos : number`: The position on the spline, relative to the length of the spline
- `pos01 : number`: The position on the spline, normalized as parameter from 0 to 1

## PathPosData (record)

```teal
global record PathPosData
```

The position in the world

- `position : Vec3f`: Position in world coordinates
- `pPrime : Vec3f`: First derivative of position (velocity direction)
- `pPrimePrime : Vec3f`: Second derivative of position (acceleration direction)

## ExtPath (record)

```teal
global record ExtPath
```

A path comprised of EdgePathEdges that does not necessarily start/end on a Port

- `edges : {ExtPathEdge}`: Edges of the path, in order; each is either an edge range or a link between two edge positions.

### ExtPath.ExtPathEdge (record)

```teal
record ExtPathEdge
```

A path edge of an ExtPath.
It can be either an EdgeRange (a sub-interval of an edge) or a LinkEdge connecting two arbitrary points of different edges.

- `range : EdgeRange`: If the edge represents an EdgeRange, provides access to its data (nil otherwise)
- `link : LinkEdge`: If the edge represents a LinkEdge, provides access to its data (nil otherwise)

#### ExtPath.ExtPathEdge.EdgeRange (record)

```teal
record EdgeRange
```

An EdgeRange of an ExtPath.ExtPathEdge

- `edgeId : EdgeId`: The ID of the corresponding transport edge
- `params : {number, number}`: The start and end positions (range: 0.0 - 1.0)
- `dir : boolean`: Direction along the edge (true if along the direction of edgeId, false for reverse)

#### ExtPath.ExtPathEdge.LinkEdge (record)

```teal
record LinkEdge
```

A LinkEdge of an ExtPath.ExtPathEdge

- `points : {EdgePos, EdgePos}`: The two locations that are connected

## EdgeIsect (record)

```teal
global record EdgeIsect
```

Specifies the intersection of two edges, with waiting area for vehicles.

- `infos : {Info, Info}`: The two intersection info entries for each of the two edges

### EdgeIsect.Info (record)

```teal
record Info
```

Data for an edge intersection entry

- `index : integer`: The index of the other edge in the intersection
- `param : number`: The parameter position on the edge
- `pos : Vec3f`: The world position of the intersection

## PathFactoryInput (record)

```teal
global record PathFactoryInput
```

Input of a pathfinding request for a simulated entity, stored in
`SimEntityNeedsPath.input`; the result is written to `SimEntityNeedsPath.output`.

- `fromEdgesWalk : {EdgePos}`: Edge positions where a walking path can start.
- `fromEdgesDrive : {EdgePos}`: Edge positions where a driving path can start.
- `fromLineOrEdges : LineOrEdges`: Line section or edge positions where a path over lines can start.
- `moveMode2costFactor : {number, number, number}`: Cost factors per move mode, one value for each of the three `MoveMode` values (`WALK`, `DRIVE`, `LINES`).
- `toEntityWalk : Engine.Entity`: Target entity of a walking path.
- `toEdgesDrive : {EdgePos}`: Edge positions where a driving path can end.
- `fromEdgesWalkForDrive : {EdgePos}`
- `toEdgesWalkForDrive : {EdgePos}`
- `cargoType : CargoTypeId`: Cargo type the path is searched for.

## MoveMode (record)

```teal
global record MoveMode
```

How a sim person moves: walking, driving, or using transport lines. Used in
`SimEntityMoving.lastMoveMode`, `SimEntityMoving.moveModes` and `PathFactoryOutput.moveMode`.

- `WALK : MoveMode`
- `DRIVE : MoveMode`
- `LINES : MoveMode`: Travelling on the player's transport lines.

## PathFactoryOutput (record)

```teal
global record PathFactoryOutput
```

Output result from pathfinding

- `line0 : LineSection`
- `line1 : LineSection`
- `path : ExtPath`: The path that was found.
- `cost : number`: Cost of the found path as computed by the pathfinder.
- `moveMode : MoveMode`: Move mode of the found path.

### PathFactoryOutput.LineSection (record)

```teal
record LineSection
```

Part of a path found by pathfinding that runs on a transport line.

- `line : Engine.Entity`: Line entity of this section.
- `lineStop0 : integer`
- `lineStop1 : integer`

## RunwayType (record)

```teal
global record RunwayType
```

Runway type enumeration

- `LANDING : RunwayType`: Landing runway
- `TAKEOFF : RunwayType`: Takeoff runway

## Runway (record)

```teal
global record Runway
```

Contains runway data

- `type : RunwayType`: Type of runway (landing or takeoff)
- `nodeId : NodeId`: Node ID of the runway
- `edgeIds : {EdgeId}`: List of edge IDs that make up the runway

## HeightmapNew (record)

```teal
global record HeightmapNew
```

Abstraction of heightmap data, consisting of a cell with floating point values at each point

- `getSize : function() : Vec2i`: The total size of the heightmap (x, y)
- `at : function(integer, integer) : number`: Get the value of the heightmap at the given x, y coordinate
    - param `x`: x coordinate
    - param `y`: y coordinate

## LogLevel (record)

```teal
global record LogLevel
```

Log level enumeration

- `Raw : LogLevel`
- `Error : LogLevel`
- `Warning : LogLevel`
- `Message : LogLevel`
- `Verbose : LogLevel`

## TerrainAlignmentType (record)

```teal
global record TerrainAlignmentType
```

Terrain alignment type enumeration

- `EQUAL : TerrainAlignmentType`
- `LESS : TerrainAlignmentType`
- `GREATER : TerrainAlignmentType`

## TerrainAlignment (record)

```teal
global record TerrainAlignment
```

Terrain alignment configuration

- `triangles : {{Vec3f, Vec3f, Vec3f}}`: Triangles of the alignment surface, each given as three points.
- `weights : {{number, number, number}}`
- `type : TerrainAlignmentType`: How the terrain is aligned to the triangles.
- `bbox : Box2f`: 2D bounding box of the alignment.

## PortId (record)

```teal
global record PortId
```

Identifies a Port of a Node within a transport network

- `nodeId : NodeId`: The NodeId of the node this port belongs to
- `port : integer`: The zero-based port index on the node

## TpNetLinkProposal (record)

```teal
global record TpNetLinkProposal
```

Contains the processed result with all edge links.

- `toRemove : {Data}`: Links to remove
- `toAdd : {Data}`: Links to add

### TpNetLinkProposal.Data (record)

```teal
record Data
```

An individual link entry in a TpNetLinkProposal

- `link : {EdgePos, EdgePos}`: The EdgePos pair making up the link

## StopState (record)

```teal
global record StopState
```

Information about the problem state of a stop within a line.

- `empty : boolean`: Stop is empty / valid
- `noPath : boolean`: No path between stops
- `noPathFromAlternative : boolean`: No path from an alternative stop
- `noPathToAlternative : boolean`: No path to an alternative stop
- `duplicateStop : boolean`: Stop is a duplicate
- `incompatibleStop : boolean`: Stop is incompatible with the line's transport mode
- `noPathLocation : boolean`: No path for a location stop

## Context (record)

```teal
global record Context
```
Access: `api.type.Context`

Proposal context to configure how the proposal is to be applied

- `new : function() : Context`  (call as `api.type.Context.new(…)`): Creates a new, empty `Context`.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/bridge_and_tunnel.tl:187`: `local context = api.type.Context.new()`
- `checkTerrainAlignment : boolean`
- `cleanupStreetGraph : boolean`: If true, attempts to simplifyy the base edge and base node graph
- `gatherBuildings : boolean`
- `gatherFields : boolean`
- `player : Engine.Entity`: Player entity on whose behalf the proposal is applied; the base game sets it to `api.engine.util.getPlayer()`.
- `refundableEntities : {{Engine.Entity, integer}, {Engine.Revision, integer}}`: Entities whose removal is refunded when the proposal is applied, in the format returned by `api.gui.construction.getRefundableEntities()`.

    ```lua
    local context = api.type.Context.new()
    context.player = api.engine.util.getPlayer()
    local refundables = api.gui.construction.getRefundableEntities()
    if refundables ~= nil then
      context.refundableEntities = refundables
    end
    api.cmd.sendCommand(api.cmd.makeWorldBuildProposalCmd(proposal, context, false, true))
    ```

- `extendProposalRedoPillars : boolean`: Find colliding bridge pillars and try to move them off the side

## Proposal (record)

```teal
global record Proposal
```
Access: `api.type.Proposal`

A more complete version of the SimpleProposal.
Most data can be read from a ProposalData or in the result of api.cmd.makeWorldBuildProposalCmd.

- `new : function() : Proposal`  (call as `api.type.Proposal.new(…)`): Creates a new, empty `Proposal`.
- `clone : function(Proposal) : Proposal`  (call as `api.type.Proposal.clone(…)`): Creates a copy of a proposal.
    - param `#1`: The proposal to copy.
    - returns: The new proposal.
- `proposal : StreetProposal`: Street proposal
- `toRemove : {Engine.Entity}`: Entities to remove
- `toRemove_native : Vector<Engine.Entity>`: Entities to remove as native vector
- `old2new : {Engine.Entity : integer}`: Mapping from old to new entities
- `toAdd : {ConstructionEntity}`: Construction entities to add
- `toAdd_native : Vector<ConstructionEntity>`: Construction entities to add as native vector
- `terrain : Terrain`: Terrain modifications

### Proposal.EdgeObject (record)

```teal
record EdgeObject
```
Access: `api.type.Proposal.EdgeObject`

Edge object information

- `resultEntity : Engine.Entity`: Resulting entity
- `category : integer`: Category (0 = stop, 1 = waypoint, 2 = signal)
- `modelInstance : ModelInstance`: Model instance for the edge object
- `playerEntity : Engine.Entity`: Player entity that owns this edge object
- `left : boolean`: Whether on left side

### Proposal.NodeAndEntity (record)

```teal
record NodeAndEntity
```
Access: `api.type.NodeAndEntity`

An entity with a BaseNode.
Used to specify a new base node entity to be added.

- `new : function() : NodeAndEntity`  (call as `api.type.NodeAndEntity.new(…)`): Creates a new, empty `NodeAndEntity`.
    - used 2x in the base game, e.g. `base/content/mission/mission/tasks/auto_builder/track_builder.tl:10`: `local node0 = api.type.NodeAndEntity.new()`
- `entity : Engine.Entity`: Entity id of the node; new entities use decreasing negative ids starting at `-1`.
- `comp : Engine.Component.BaseNode`: `BaseNode` component of the node.

### Proposal.SegmentAndEntity (record)

```teal
record SegmentAndEntity
```
Access: `api.type.SegmentAndEntity`

An entity with a BaseEdge component and extra information.

- `new : function() : SegmentAndEntity`  (call as `api.type.SegmentAndEntity.new(…)`): Creates a new, empty `SegmentAndEntity`.
    - used 2x in the base game, e.g. `base/content/mission/mission/tasks/auto_builder/electrify.tl:13`: `local edge0 = api.type.SegmentAndEntity.new()`
- `entity : Engine.Entity`: Entity id of the edge; new entities use decreasing negative ids starting at `-1`.
- `comp : Engine.Component.BaseEdge`: `BaseEdge` component of the edge.
- `type : integer`: Type (0 = street, 1 = track)
0 = street, 1 = track
- `streetEdge : Engine.Component.BaseEdgeStreet`: Street edge component
- `emissionEmitter : Engine.Component.EmissionEmitter`: Emission emitter component (optional)
- `playerOwned : Engine.Component.PlayerOwned`: Player owned component (optional)

### Proposal.BaseNodeLaneConnectionAndEntity (record)

```teal
record BaseNodeLaneConnectionAndEntity
```
Access: `api.type.BaseNodeLaneConnectionAndEntity`

An entity with a BaseNodeLaneConnection component.
Used to specify a new base node configuration entity to be added.

- `new : function() : BaseNodeLaneConnectionAndEntity`  (call as `api.type.BaseNodeLaneConnectionAndEntity.new(…)`): Creates a new, empty `BaseNodeLaneConnectionAndEntity`.
- `comp : Engine.Component.BaseNodeConfig`: `BaseNodeConfig` component with the lane connections of the node.
- `entity : Engine.Entity`: Entity id of the node configuration.

### Proposal.StreetProposal (record)

```teal
record StreetProposal
```
Access: `api.type.StreetProposal`

A more complete version of the SimpleStreetProposal.
Most data can be read from a ProposalData or in the result of cmd.makeWorldBuildProposalCmd.

- `addedNodes : {NodeAndEntity}`: Added nodes
- `addedNodes_native : Vector<NodeAndEntity>`: Added nodes as native vector
- `addedSegments : {SegmentAndEntity}`: Added segments
- `addedSegments_native : Vector<SegmentAndEntity>`: Added segments as native vector
- `removedSegments : {SegmentAndEntity}`: Removed segments
- `removedNodes : {NodeAndEntity}`: Removed nodes
- `edgeObjectsToAdd : {EdgeObject}`: Edge objects to add
- `new2oldEdgeObjects : {Engine.Entity : {Engine.Entity}}`: Mapping of new to old edge objects
- `old2newEdgeObjects : {Engine.Entity : {Engine.Entity}}`: Mapping of old to new edge objects
- `nodeConfigsToAdd : {BaseNodeLaneConnectionAndEntity}`: Node configurations that will be added
- `nodeConfigsToRemove : {Engine.Entity}`: Node configurations that will be removed (node itself cannot be removed at the same time)

### Proposal.TransformedModel (record)

```teal
record TransformedModel
```
Access: `api.type.Proposal.TransformedModel`

Transformed model information

- `id : string`: Model resource name; resolve it with `api.res.modelRep.find(id)`.
- `tag : integer`: Tag of the model.
- `transf : Mat4f`: Transformation matrix of the model.
- `thin : boolean`

### Proposal.Subconstruction (record)

```teal
record Subconstruction
```
Access: `api.type.Proposal.Subconstruction`

One subconstruction of a construction in a proposal, with its models, station and colliders.

- `models : {TransformedModel}`: Models of the subconstruction.
- `models_native : Vector<TransformedModel>`: Models of the subconstruction, as native vector (iterate with `:size()` and `:at(i)`).
- `metadata : SubconstructionInstanceMetadata`: used by nils to fill C++ (updateFn)
- `station : Station`: Station of the subconstruction.
- `colliders : {Collider}`: Colliders of the subconstruction.

### Proposal.Construction (record)

```teal
record Construction
```
Access: `api.type.Proposal.Construction`

A construction: usually contains stations, depots, town buildings or industries.

- `subconstructions : {Subconstruction}`: all subconstructions generated by the construction
- `subconstructions_native : Vector<Subconstruction>`: all subconstructions generated by the construction, as native vector
- `fileName : string`: the resource name of the construction (or the id in case of dynamic constructions)
- `frozenNodes : {Engine.Entity}`: nodes that belong to the construction and cannot be moved
- `frozenEdges : {Engine.Entity}`: frozenEdges edges that belong to the construction and cannot be moved
- `depots : {Engine.Entity}`: depots that belong to the construction
- `stations : {Engine.Entity}`: stations that belong to the construction
- `industries : {Engine.Entity}`: industries that belong to the construction
- `townBuildings : {Engine.Entity}`: town buildings that belong to the construction
- `persistentMetadata : ConstructionInstanceMetadata`: Construction metadata persisted per instance
- `params : table`: construction params

### Proposal.StreetTerminal (record)

```teal
record StreetTerminal
```
Access: `api.type.Proposal.StreetTerminal`

Street terminal of a construction that is placed on a street, such as a small bus or
truck stop. Set as `result.streetTerminal` in the construction's `updateFn`.

- `cargo : boolean`: Whether the terminal handles cargo.
- `passengers : boolean`: Whether the terminal handles passengers.
- `capacity : integer`: Capacity of the terminal.
- `wantSymmetric : boolean`: Whether the stop is two-sided (the base game's small stops set it from their `twoSided` parameter).
- `cargoUnload : boolean`: Whether cargo can be unloaded at the terminal.
- `cargoLoad : boolean`: Whether cargo can be loaded at the terminal.

### Proposal.Signal (record)

```teal
record Signal
```
Access: `api.type.Proposal.Signal`

Signal data of an edge object in a proposal.

- `type : SignalType`: Type of the signal.

### Proposal.ConstructionEntity (record)

```teal
record ConstructionEntity
```
Access: `api.type.Proposal.ConstructionEntity`

Construction entity data for use in proposals
Most data can be read from a ProposalData or in the result of cmd.makeWorldBuildProposalCmd.

- `fileName : ResName`: Construction resource name
- `hasCargoPlatform : boolean`: Whether it has cargo platform
- `desc : ConstructionDesc`: Construction description
- `construction : Construction`: Construction data
- `transf : Mat4f`: Transformation matrix
- `playerEntity : Engine.Entity`: Owning player entity

### Proposal.Terrain (record)

```teal
record Terrain
```
Access: `api.type.Proposal.Terrain`

Contains proposal terrain modifications.

- `baseHeightMod : GridVec2f`: Grid of terrain height modifications of the proposal.

## ConstructionResult (record)

```teal
global record ConstructionResult
```

Contains the processed result of a construction

- `subconstructions : {Proposal.Subconstruction}`: Subconstructions generated by the construction.
- `metadata : ConstructionInstanceMetadata`: Metadata of the construction instance.
- `cost : number`: Construction cost, in game currency.
- `maintenanceCost : number`: Maintenance cost of the construction; the build menu multiplies it by `baseConfig.costs.infrastructureMaintenanceScale` for display.
- `maintenanceCarrier : string`: Carrier whose maintenance the cost is booked under, as the name of a `Carrier` value (for example `"OTHER"`).
- `streetTerminal : Proposal.StreetTerminal`: Street terminal of the construction, if it has one (`nil` otherwise).
- `params : table`: Construction parameters.

## ProposalData (record)

```teal
global record ProposalData
```
Access: `api.type.ProposalData`

Contains the processed result of a Proposal.

- `collisionInfo : CollisionInfo`: Collision information of the proposal.
- `old2new : {Engine.Entity : integer}`
- `toAddData : {ConstructionData}`: Processed data of each construction to add, in the same order as `Proposal.toAdd`.

    ```lua
    for i = 1, proposalData.toAddData_native:size() do
      local data = proposalData.toAddData_native:at(i)
      local construction = proposal.toAdd_native:at(i).construction
      -- data.boundingVolume, data.subconstructionDatas_native ...
    end
    ```

- `toAddData_native : Vector<ConstructionData>`: Processed data of each construction to add, as native vector.
- `parallelProposal : ParallelStripProposal`: Parallel strips added and removed by the proposal.
- `terrainPreviewHeightModLocations : {Vec3f}`
- `terrainPreviewHeightModCutFill : {Vec2f}`
- `errorState : ErrorState`: Errors, warnings and information messages of the proposal.
- `entity2tn : {Engine.Entity : Engine.Component.TransportNetwork}`
- `tpNetLinkProposal : TpNetLinkProposal`: Transport network links added and removed by the proposal.
- `costs : integer`: Total cost of the proposal, in game currency.

### ProposalData.SubconstructionData (record)

```teal
record SubconstructionData
```
Access: `api.type.ProposalData.SubconstructionData`

Processed data of one subconstruction of a construction in a proposal.

- `station : Engine.Component.Station`: `Station` component of the subconstruction, or `nil` if the subconstruction has no station.

### ProposalData.ConstructionData (record)

```teal
record ConstructionData
```
Access: `api.type.ProposalData.ConstructionData`

Processed data of one construction to add. The entries of `ProposalData.toAddData`
match the entries of `Proposal.toAdd` in the same order.

- `boundingVolume : Engine.Component.BoundingVolume`: Bounding volume of the construction.
- `subconstructionDatas : {SubconstructionData}`: Data of each subconstruction.
- `subconstructionDatas_native : Vector<SubconstructionData>`: Data of each subconstruction, as native vector (iterate with `:size()` and `:at(i)`).

### ProposalData.CollisionInfo (record)

```teal
record CollisionInfo
```
Access: `api.type.ProposalData.CollisionInfo`

Collision information of the Proposal.

- `collisionEntities : {EntityData}`: Entities the proposal collides with.
- `autoRemovalEntity2models : {Engine.Entity : {{integer, integer}}}`
- `buildingEntities : {Engine.Entity}`
- `removableModules : {Engine.Entity}`

#### ProposalData.CollisionInfo.EntityData (record)

```teal
record EntityData
```
Access: `api.type.ProposalData.CollisionInfo.EntityData`

More data about the collision.

- `entity : Engine.Entity`: The colliding entity.

### ProposalData.ErrorState (record)

```teal
record ErrorState
```
Access: `api.type.ProposalData.ErrorState`

Contains error information for a proposal.
Wherever a construction cannot be built for any reason, then the ErrorState will contain
one or more message signaling the error type.

- `critical : boolean`: Whether the proposal has a critical error and cannot be built.
- `messages : {string}`: Error messages; the proposal cannot be built while this list is not empty.
- `warnings : {string}`: Warning messages.
- `infos : {string}`: Information messages.

### ProposalData.ParallelStripProposal (record)

```teal
record ParallelStripProposal
```
Access: `api.type.ProposalData.ParallelStripProposal`

Information about a parallel strip proposal.

- `toAdd : {Engine.Component.BaseParallelStrip}`: The parallel strips to add
- `toAdd_native : Vector<Engine.Component.BaseParallelStrip>`: The parallel strips to add as native vector
- `toRemove : {Engine.Entity}`: The parallel strips to remove
- `toRemove_native : Vector<Engine.Entity>`: The parallel strips to remove as native vector

## SimpleStreetProposal (record)

```teal
global record SimpleStreetProposal
```
Access: `api.type.SimpleStreetProposal`

A structure holding information to add street and track entities together with a SimpleProposal. 

A street proposal can add, remove and modify street, tracks, nodes and edge objects. To modify a street, you
need to remove all appropriate entities and add them with the correct information.

- `new : function() : SimpleStreetProposal`  (call as `api.type.SimpleStreetProposal.new(…)`): Creates a new, empty `SimpleStreetProposal`.
- `nodesToAdd : {Proposal.NodeAndEntity}`: List of base nodes to add (entity id will be decreasing starting from -1 for all new entities)
- `edgesToAdd : {Proposal.SegmentAndEntity}`: List of base edges to add (includes edge object reference which must match the content of `edgeObjectsToRemove`)
- `nodesToRemove : {Engine.Entity}`: List of entities with component BaseNode to remove
- `edgesToRemove : {Engine.Entity}`: List of entities with component BaseEdge to remove
- `edgeObjectsToAdd : {EdgeObject}`: List of EdgeObject to add (must be compatible with `edgesToAdd`)
- `edgeObjectsToRemove : {Engine.Entity}`: List of entities to be removed
- `nodeConfigsToAdd : {Proposal.BaseNodeLaneConnectionAndEntity}`: List of node configurations to add
- `nodeConfigsToRemove : {Engine.Entity}`: List of node configuration entities to remove
- `node2rcType : {Engine.Entity : integer}`: A map from node to railroad crossing type index

### SimpleStreetProposal.EdgeObject (record)

```teal
record EdgeObject
```
Access: `api.type.SimpleStreetProposal.EdgeObject`

A structure that holds information about new edgeobjects added to the streets

- `new : function() : EdgeObject`  (call as `api.type.SimpleStreetProposal.EdgeObject.new(…)`): Creates a new EdgeObject object
- `edgeEntity : Engine.Entity`: Edge entity (must be a new edge in edgesToAdd, starting from -1 and decreasing)
- `param : number`: Position between 0.0 and 1.0 within said edge
- `oneWay : boolean`: Flag specifying if one way or not
- `left : boolean`: Flag specifying the side of the road
- `model : ResName`: Model id of the edge object
- `playerEntity : Engine.Entity`: Owner of the object
- `name : string`: Name to give the object

## ProposalEventData (record)

```teal
global record ProposalEventData
```

Event data provided in builder.proposalApply event

- `proposal : Proposal`: The proposal.
- `data : ProposalData`: Processed data of the proposal.
- `result : {Engine.Entity}`: Entities created by the proposal; only set for the `builder.proposalApply` event.

## SimpleProposal (record)

```teal
global record SimpleProposal
```
Access: `api.type.SimpleProposal`

A structure holding information to build/remove and update constructions and streets.

It offers much more functionality than a simple ".con" file. See also `build_street.lua` for an example.
The most important operations that can be performed with a Simple Proposal is adding/removing and modifying
the following objects: constructions (stations, depots, ), (base) edges and (base) nodes, and edgeobjects (signals,
street terminals, waypoints).


In general, there are a few rules concerning this proposal. First, old entities are reference with the usual non-negative
id, whilst new entities are assigned a decreasing negative number starting at -1.


The general rule is that the modification/upgrade of a construction entails removing it and adding it
anew. The same rule applies to streets and edgeobjects.


The proposal will take care of updating lines, stop, people and cargo.

- `new : function() : SimpleProposal`  (call as `api.type.SimpleProposal.new(…)`): Creates a new, empty `SimpleProposal`.
    - used 10x in the base game, e.g. `base/content/landmarks/landmarks.script.tl:64`: `local proposal = api.type.SimpleProposal.new()`
- `constructionsToAdd : {ConstructionEntity}`: List of constructions to add (the entity id for constructions in this vector are an decreasing starting from -1)
- `constructionsToRemove : {Engine.Entity}`: List of construction entities to remove
- `old2new : {Engine.Entity : integer}`: Mapping from old construction entities to new construction entities (must be an index in constructionsToAdd)
- `streetProposal : SimpleStreetProposal`: Block defining modification and addition of streets

### SimpleProposal.ConstructionEntity (record)

```teal
record ConstructionEntity
```
Access: `api.type.SimpleProposal.ConstructionEntity`

A structure holding information to add construction entities together with a `SimpleProposal`.
It uses the data contained in a ".con" file (or added via `api.res.constructionRep`) to specify a construction.

- `new : function() : ConstructionEntity`  (call as `api.type.SimpleProposal.ConstructionEntity.new(…)`): Creates a new, empty `ConstructionEntity`.
    - used 7x in the base game, e.g. `base/content/landmarks/landmarks.script.tl:66`: `local ce = api.type.SimpleProposal.ConstructionEntity.new()`
- `fileName : string`: A fileName (or identifier in the case of dynamic constructions)
- `params : table`: The parameters, as received by the updateFn
- `transf : Mat4f`: The local transformation in the world, used also as local transformation for models and modules
- `name : string`: A name to give to the final resulting entity
- `playerEntity : Engine.Entity`: The player entity who will own the construction
- `autoFillSlots : boolean`: If true, the suitable (auto-fillable) construction slots will be automatically filled with modules

## CatchmentArea (record)

```teal
global record CatchmentArea
```
Access: `api.type.CatchmentArea`

A catchment area is a cylinder around a position with a specific radius

- `position : Vec3f`: Centre of the catchment area, in world coordinates.
- `radius : number`: Radius of the catchment area.

## AudioEffectSettings (record)

```teal
global record AudioEffectSettings
```

Specification for audio settings

- `bulldozer : boolean`: Bulldozer sound effects enabled
- `construction : boolean`: Construction sound effects enabled

## AppConfig (record)

```teal
global record AppConfig
```
Access: `api.type.AppConfig`

Game settings

- `new : function(?AppConfig) : AppConfig`  (call as `api.type.AppConfig.new(…)`): Creates a new game settings object; pass an existing one to copy it.
    - param `#1`: Settings to copy (optional).
    - returns: The new settings object.

    ```lua
    local newConfig = api.type.AppConfig.new(api.util.getAppConfig())
    -- change fields of newConfig ...
    api.util.setAppConfig(newConfig, false)
    ```

    - used 16x in the base game, e.g. `base/content/gui/gui/menu/main_menu.tl:64`: `local appConfig = api.type.AppConfig.new(api.util.getAppConfig())`
- `equals : function(AppConfig, AppConfig) : boolean`  (call as `appConfig:equals(…) (method)`): Returns `true` when both `AppConfig` values are equal.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:1438`: `if not appConfig:equals(appConfigState:old()) then`
- `distanceSpeedUnit : DistanceSpeedUnit`: Unit of measurement for distance
- `mainMenuState : table`: Main menu state data (filled by the UI scripts to persist things like the list of active mods)
- `enableKeyboardMouse : boolean`: Whether keyboard and mouse input is enabled
- `keyCmdDefinitions : {string : {KeyComboDef}}`: Keybindings (stored `settings_keys_v*.lua`)
- `moneyPrefix : string`: Prefix for money display
- `cameraTool : boolean`: Whether camera tool is enabled
- `showReleaseNotes : boolean`: Whether to show release notes
- `hasShownInitialDeluxeThankYou : boolean`: Whether an initial thank you message for owning deluxe edition has been shown before
- `updateMessageID : integer`: Last update message ID shown
- `debugMode : boolean`: Whether debug mode is enabled
- `guideSystemActive : boolean`: Whether guide system is active
- `experimentalTerrainGeneratorEditor : boolean`: Whether the terrain node graph editor is enabled
- `experimentalMapFeatures : boolean`: Whether experimental map features are enabled
- `screenMode : integer`: Screen mode setting (WINDOWED = 0, BORDERLESS = 1, FULLSCREEN = 2)
- `enableSeamlessStreetBuilder : boolean`: Whether to enable the seamless construction mode for the street builder
- `showIntelHDwarning : boolean`: Display unsupported hardware info
- `musicEnabled : boolean`: Whether music is enabled
- `menuMusicEnabled : boolean`: Whether menu music is enabled
- `voiceOverEnabled : boolean`: Whether voice-over is enabled
- `masterVolume : number`: Master volume level
- `soundVolume : number`: Sound effects volume level
- `musicVolume : number`: Music volume level
- `voiceOverVolume : number`: Voice-over volume level
- `audioEffectSettings : AudioEffectSettings`: Audio effect settings
- `experimentalTunnelAndBridgeMinHeight : boolean`: Enable experimental tunnel/bridge height settings
- `resolutionScale : number`: Render extent scale for in-game viewport
- `cssEffectsVelocity : number`: How fast animations and transitions are played for GUI objects - default is 1, 0 means no effect at all
- `backgroundContrastMode : BackgroundContrastMode`: Selects the contrast mode in LVM and module builder

## ScriptParam (record)

```teal
global record ScriptParam
```
Access: `api.type.ScriptParam`

Used to configure the parameter-based UI

- `new : function() : ScriptParam`  (call as `api.type.ScriptParam.new(…)`): Creates a new, empty `ScriptParam`.
    - used 6x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:361`: `local scriptParam = api.type.ScriptParam.new()`
- `key : string`: Unique key that is passed to the construction functions
- `name : string`: Display name
- `group : string`: Display Group:
- If hideLabel is true, items will be grouped horizontally
- Else items will be separated by a divider
- `tooltip : string`: Tooltip to display over certain parameters
- `values : {string}`: All possible values of the params
- `numbers : {number}`: Allow numbers to be used as value
- `tooltips : {string}`: Allow tooltips to be used as value
- `postConstructionModifiable : boolean`: This param can be changed when the construction is already built
- `hideLabel : boolean`: Allow to hide the label
- `images : {string}`: Image representations of the values (for Compact Mode)
- `defaultIndex : Index`: Default index/state
- `uiType : ScriptParamType`: Type of ui shown in user interface selection
- `location : ScriptParamLocation`: construction menu has two possible locations for parameters: default or toolbar (see ScriptParamLocation)
- `displayMode : ScriptParamDisplayMode`: displayMode controls whether the parameter is displayed in default, compact, or sparse mode (see ScriptParamDisplayMode)
- `tags : {string}`: Tags associated with the script param for filtering
- `yearFrom : integer`: Availability from
- `yearTo : integer`: Availability to
- `checkEnabledScript : ScriptRef`: callback function to allow dynamic disabling or hiding of the paramter
function(params : {string : number}, scriptRefParams : table) : ConstructionMenuParam.CheckEnabledResult
- `stepValueScript : ScriptRef`: callback function to allow returning any value on input actions
function(value : number, direction : integer, precise : boolean) : number
- `formatValueScript : ScriptRef`: callback function to allow formatting the value display in the UI
function(value : number) : string

## ModRep (record)

```teal
global record ModRep
```

Contains all active and installed mods and their description

- `getInstalledMods : function(ModRep) : {Mod.ModId}`: Returns the ids of all installed mods.
    - returns: List of mod ids.
- `exists : function(ModRep, Mod.ModId) : boolean`: Checks whether a mod with the given id exists.
    - returns: `true` if the mod exists.
    - notes: The definition file declares this field twice with the same signature.
- `getModDesc : function(ModRep, Mod.ModId) : Mod.ModDesc`: Returns the description of a mod (its parameters, visibility, severity of adding and removing it, and more).
    - returns: The mod description.
- `getModSize : function(ModRep, Mod.ModId) : Mod.ModSize`: Returns size statistics of a mod (number of models, textures and meshes, estimated size).
    - returns: The mod size information.
- `getGameModDesc : function(ModRep, Mod.ModId) : Mod.GameModDesc`: Returns the in-game description of a mod (id, source, mod hub id, display name and removal severity).
    - returns: The game mod description.
- `getModSource : function(ModRep, Mod.ModId) : string`: Returns the origin of a mod (for example user mod, staging area, DLC, mod.io).
    - returns: The mod source; pass it to `api.modhub.getBackendIdForSource` to find the mod hub backend.
- `exists : function(ModRep, Mod.ModId) : boolean`: Checks whether a mod with the given id exists.
    - returns: `true` if the mod exists.
    - notes: The definition file declares this field twice with the same signature.
- `wouldForceActivateAchievements : function(ModRep, modIds : {Mod.ModId}) : boolean`: Checks whether the given mod list would force achievements to be enabled.
    - param `modIds`: Mod ids to check.
    - returns: `true` if achievements would be force-activated.
- `couldAchievementsBeEarned : function(ModRep, modIds : {Mod.ModId}) : boolean`: Checks whether achievements can be earned with the given mods active.
    - param `modIds`: Mod ids to check.
    - returns: `true` if achievements can be earned; the base game shows a "no achievements" warning otherwise.

## LangKey (record)

```teal
global record LangKey
```
Access: `api.type.LangKey`

Localization information

- `new : function() : LangKey`  (call as `api.type.LangKey.new(…)`): Creates a new, empty `LangKey`.
- `equals : function(LangKey, LangKey) : boolean`  (call as `api.type.LangKey.equals(…)`): Returns `true` when both `LangKey` values are equal.
- `code : string`: Language code as two characters code or two characters and a two character rvariant code separated by underscore
- `mod : Mod.ModId`: Mod from which the lanuage is originating from

## LanguageRep (record)

```teal
global record LanguageRep
```

Contains all available languages of the application, including languages added by mods

- `getAllLanguages : function(LanguageRep) : {{string, LangKey}}`: Returns all available languages.
    - returns: List of `{name, langKey}` pairs; the settings page shows the name and passes the `LangKey` to `UserProfile.changeLanguage`.

## AnyKey (record)

```teal
global record AnyKey
```

## AnyAxis (record)

```teal
global record AnyAxis
```

## InputMode (record)

```teal
global record InputMode
```

InputMode represents the current input mode.

- `Undefined : InputMode`: No input mode is set.
- `KeyboardMouse : InputMode`: Keyboard and mouse input.
- `Gamepad : InputMode`: Gamepad input.

## GamepadType (record)

```teal
global record GamepadType
```

GamepadType represents the current gamepad.

- `None : GamepadType`: No gamepad.
- `XBox : GamepadType`
    - used 1x in the base game, e.g. `base/content/mission/mission/tutorial/guide_system/guide_system_story.tl:6`: `elseif api.util.getGamepadType() == api.type.enum.GamepadType.XBox then`
- `PlayStation5 : GamepadType`
    - used 1x in the base game, e.g. `base/content/mission/mission/tutorial/guide_system/guide_system_story.tl:4`: `if api.util.getGamepadType() == api.type.enum.GamepadType.PlayStation4 or api.util.getGamepadType() == api.type.enum.GamepadType.PlayStation5 then`
- `PlayStation4 : GamepadType`
    - used 1x in the base game, e.g. `base/content/mission/mission/tutorial/guide_system/guide_system_story.tl:4`: `if api.util.getGamepadType() == api.type.enum.GamepadType.PlayStation4 or api.util.getGamepadType() == api.type.enum.GamepadType.PlayStation5 then`

## VramBudgetStatus (record)

```teal
global record VramBudgetStatus
```

How well the VRAM budgets fit into the available VRAM with the current graphics settings.

- `Unknown : VramBudgetStatus`: No status is available.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:1858`: `if status ~= api.type.enum.VramBudgetStatus.Unknown then`
- `Sufficient : VramBudgetStatus`: The video memory is enough for the selected quality settings.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:1860`: `status == api.type.enum.VramBudgetStatus.Sufficient and ", vram-status-sufficient" or`
- `Limited : VramBudgetStatus`: The video memory is below the recommended amount for the selected quality settings. Some objects may show less detail and performance may suffer.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:104`: `elseif status == api.type.enum.VramBudgetStatus.Limited then`
- `Insufficient : VramBudgetStatus`: The video memory is not enough for the selected quality settings. Performance and visual quality suffer.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:97`: `if status == api.type.enum.VramBudgetStatus.Insufficient then`

## KeyCombo (record)

```teal
global record KeyCombo
```
Access: `api.type.KeyCombo`

Key combination for input binding

- `new : function(KeyComboDef) : KeyCombo`  (call as `api.type.KeyCombo.new(…)`): Creates a key combination from its definition.
    - param `#1`: The key combination definition.
    - returns: The key combination.
    - used 5x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:158`: `local newCombo = api.type.KeyCombo.new(v)`
- `toString : function(KeyCombo) : string`  (call as `api.type.KeyCombo.toString(…)`): Returns a display text for the key combination.
    - returns: Display text, as shown in the key binding settings.
- `isModifierOnly : function(KeyCombo) : boolean`  (call as `api.type.KeyCombo.isModifierOnly(…)`): Checks whether the key combination consists of modifier keys only.
    - returns: `true` if it has modifier keys only.
- `relevantFor : function(KeyCombo, InputMode) : boolean`  (call as `keyCombo:relevantFor(…) (method)`): Checks whether the key combination applies to the given input mode.
    - param `#2`: The input mode, for example `api.type.enum.InputMode.KeyboardMouse`.
    - returns: `true` if the combination is used in that input mode.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:159`: `if newCombo:relevantFor(api.type["enum"].InputMode.KeyboardMouse) then`

## KeyComboDef (record)

```teal
global record KeyComboDef
```
Access: `api.type.KeyComboDef`

Key combination definition for input binding

- `new : function() : KeyComboDef`  (call as `api.type.KeyComboDef.new(…)`): Creates a new, empty `KeyComboDef`.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:284`: `comboOfInterest = api.type.KeyComboDef.new()`
- `new : function(KeyComboDef) : KeyComboDef`  (call as `api.type.KeyComboDef.new(…)`): Creates a new, empty `KeyComboDef`.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:284`: `comboOfInterest = api.type.KeyComboDef.new()`
- `setKeyScancode : function(combo : KeyComboDef, scancode : integer)`  (call as `api.type.KeyComboDef.setKeyScancode(…)`): Set the trigger key to this scancode (ignored if scancode is a modifier key)
- `addModifierKey : function(combo : KeyComboDef, scancode : integer)`  (call as `api.type.KeyComboDef.addModifierKey(…)`): Add a modifier scancode to this combination (ignored if scancode is not a modifier key)
- `condition : integer`: Trigger condition for the key combination

`0`: Initial trigger on keydown or when conflicting longpress is rejected

`1`: Initial triggers when keycombo is pressed for X time)

## InputActionDesc (record)

```teal
global record InputActionDesc
```

Description of an input action for key binding

- `uiName : string`: Display name in UI
- `type : integer`: Type index

`0`: Simple
`1`: AxisLinear
`2`: AxisRadial
- `defaultBindings : {KeyComboDef}`: Default key bindings for this action
- `modifierOnly : boolean`: Whether this action is modifier-only (i.e. has no trigger key and can be triggered even if the keys are already used elsewhere)
- `uiCategory : string`: UI category for organization in settings menu
- `uiOrder : integer`: Display order inside the category in settings menu
- `duplicateOf : string`: This is supposed to be an alias only duplicateOf
- `duplicatedKeybindings : boolean`: If keybindings are configured exactly as for the duplicateOf. UI should hide the entry in this case.

## InputActionRep (record)

```teal
global record InputActionRep
```

Contains all input actions that can be bound to keys. Get it with `app.getInputActionRep()`.

Example:
```lua
local iaRep = app.getInputActionRep()
for _, name in ipairs(iaRep:getAll()) do
  local desc = iaRep:get(name)
  print(name, desc.uiName, desc.uiCategory)
end
```

- `getAll : function(InputActionRep) : {string}`: Returns the names of all input actions.
    - returns: List of input action names.
- `get : function(InputActionRep, string) : InputActionDesc`: Returns the description of an input action.
    - param `#2`: Name of the input action, as returned by `getAll`.
    - returns: The input action description.

## HeighmapGenerationMode (record)

```teal
global record HeighmapGenerationMode
```
Access: `api.type.HeighmapGenerationMode`

Map generation mode for heightmap generation

- `new : function() : HeighmapGenerationMode`  (call as `api.type.HeighmapGenerationMode.new(…)`): Creates a new, empty `HeighmapGenerationMode`.
    - used 2x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:546`: `local mode = api.type.HeighmapGenerationMode.new()`
- `terrainGenerator : string`: Terrain generator script resource name
- `generateTowns : boolean`: Whether to generate towns
- `generateIndustries : boolean`: Whether to generate industries
- `generateAssets : boolean`: Whether to generate assets
- `keepHeightmap : boolean`: Whether to keep the heightmap
- `keepBiomes : boolean`: Whether to keep the biomes

## StreetGenerationMode (record)

```teal
global record StreetGenerationMode
```
Access: `api.type.StreetGenerationMode`

Map generation mode for street generation

- `new : function() : StreetGenerationMode`  (call as `api.type.StreetGenerationMode.new(…)`): Creates a new, empty `StreetGenerationMode`.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:2408`: `return api.type.StreetGenerationMode.new()`

## TownIndustryGenerationMode (record)

```teal
global record TownIndustryGenerationMode
```
Access: `api.type.TownIndustryGenerationMode`

Map generation mode for adding towns and industries

- `new : function() : TownIndustryGenerationMode`  (call as `api.type.TownIndustryGenerationMode.new(…)`): Creates a new, empty `TownIndustryGenerationMode`.
    - used 2x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:1665`: `local mode = api.type.TownIndustryGenerationMode.new()`
- `numTownsToAdd : integer`: Number of towns to add
- `addIndustries : boolean`: if industries should be added
- `showExistingTowns : boolean`: Whether to show existing towns
- `showExistingIndustries : boolean`: Whether to show existing industries
- `keepExistingTowns : boolean`: Whether to preserve existing towns when generating new towns
- `keepExistingIndustries : boolean`: Whether to preserve existing industries when generating new industries

## ImportExportHeightmapMode (record)

```teal
global record ImportExportHeightmapMode
```
Access: `api.type.ImportExportHeightmapMode`

Map generation mode for importing or exporting heightmaps

- `new : function() : ImportExportHeightmapMode`  (call as `api.type.ImportExportHeightmapMode.new(…)`): Creates a new, empty `ImportExportHeightmapMode`.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:860`: `local mode = api.type.ImportExportHeightmapMode.new()`
- `fileName : string`: File name for import/export
- `import : boolean`: Whether to import (true) or export (false)
- `generateAssets : boolean`: Whether to generate assets
- `minH : number`: Minimum height value
- `maxH : number`: Maximum height value

## ImportBiomesMode (record)

```teal
global record ImportBiomesMode
```
Access: `api.type.ImportBiomesMode`

Map generation mode for importing biomes

- `new : function() : ImportBiomesMode`  (call as `api.type.ImportBiomesMode.new(…)`): Creates a new, empty `ImportBiomesMode`.
    - used 1x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:1180`: `local mode = api.type.ImportBiomesMode.new()`
- `terrainGenerator : string`: Terrain generator script resource name
- `biomeMap : string`: Biome map filename for import
- `layerMaps : {string : string}`: Additional layer map filenames for import

## TownInfo (record)

```teal
global record TownInfo
```

Extended town information for map generation

- `name : string`: Name of the town
- `position : Vec2f`: World position of the town
- `initialLandUseCapacities : {integer}`: Initial size for residential, commercial, industrial zones
- `landUse2CargoNeedsCategories : {LandUseType : {string}}`: Cargo needs for residential, commercial, industrial zones
- `majorStreetAngle : number`: Major street angle
- `initialMajorStreetAngle : number`: Initial major street angle
- `capacityScalingFactor : number`: Capacity scaling factor

## TownResult (record)

```teal
global record TownResult
```

Results of town generation operations

- `toRemove : {Engine.Entity}`: Town entities to remove.
- `toAdd : {TownInfo}`: Towns to add.

## IndustryInfo (record)

```teal
global record IndustryInfo
```

Industry placement information

- `fileName : string`: Construction resource name of the industry.
- `bbox : Box2f`
- `streetPoint : Vec2i`
- `transf : Mat4f`: Transformation matrix of the industry.
- `onWater : boolean`: Whether the industry is placed on water.

## IndustryResult (record)

```teal
global record IndustryResult
```

Results of industry generation operations

- `toRemove : {Engine.Entity}`: Industry entities to remove.
- `toAdd : {IndustryInfo}`: Industries to add.

## PlayerStatDesc (record)

```teal
global record PlayerStatDesc
```

A player stat, used for start tracking

- `id : string`: The unique id for that stat

## PlayerStatProgress (record)

```teal
global record PlayerStatProgress
```

The value of a player stat

- `id : string`: the unique id for the stat
- `getValue : function(PlayerStatProgress) : number`: the value of the stats (either the current value or a target value depending on the use context)

## PlayerStatRep (record)

```teal
global record PlayerStatRep
```

Repository for all player stat types (static data)
Access the current state of a player stats, use UserProfile:getPlayerStatProgress

- `get : function(playerStatRep : PlayerStatRep, statId : string) : PlayerStatDesc`: Get a player stat description from its id
    - param `playerStatRep`: the player start rep
    - param `statId`: the player stat id
    - returns: the player stat desc

## AchievementDesc (record)

```teal
global record AchievementDesc
```

The description of an achievement

- `id : string`: The unique id as a string
- `name : string`: The display name of the achievement
- `description : string`: The description of the acheivement
- `icon : string`: The icon once completed of the achievement
- `lockedIcon : string`: The icon while not yet completed
- `grade : string`: The grade of the achievement for platform that supports it
- `hidden : boolean`: If the achievement is hidden until it's completed
- `campaign : string`: If it's a campaign achievement
- `alwaysEarnable : boolean`: If the achievement can always be earned regardless
- `score : integer`: The score (points) earned when completing the achievements for platforms that supports it
- `playerStatTarget : PlayerStatProgress`: The player start value to reach to unlock the achievement (optional)

## AchievementRep (record)

```teal
global record AchievementRep
```

Repository for all achievement types (static data)
To access the current state of a player achievements, use UserProfile:getUnlockedAchievementIds

- `getNumAchievements : function(achievementRep : AchievementRep) : integer`: Get the number of achievement types
    - param `achievementRep`: the achievement rep
    - returns: the number of achievements
- `get : function(achievementRep : AchievementRep, index : integer) : AchievementDesc`: Get an achievement data by its index
    - param `achievementRep`: the achievement rep
    - param `index`: the achievement index (use `getNumAchievements` to iterate over all achievement types)
    - returns: the achievement desc

## MissionCampaign (record)

```teal
global record MissionCampaign
```

Campaign and mission identifier

- `campaign : ResName`: Resource name of the campaign.
- `mission : ResName`: Resource name of the mission.

## ParticleParamFloat (record)

```teal
global record ParticleParamFloat
```
Access: `api.type.ParticleParamFloat`

Particle system parameter for float values

- `new : function() : ParticleParamFloat`  (call as `api.type.ParticleParamFloat.new(…)`): Creates a new particle parameter
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:121`: `return api.type.ParticleParamFloat.new(value as number)`
- `new : function(value : number) : ParticleParamFloat`  (call as `api.type.ParticleParamFloat.new(…)`): Creates a new particle parameter with a value
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:121`: `return api.type.ParticleParamFloat.new(value as number)`
- `new : function(range : RandomMinMax) : ParticleParamFloat`  (call as `api.type.ParticleParamFloat.new(…)`): Creates a new particle parameter with a range
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:121`: `return api.type.ParticleParamFloat.new(value as number)`
- `new : function(range : ComponentWiseMinMax) : ParticleParamFloat`  (call as `api.type.ParticleParamFloat.new(…)`): Creates a new particle parameter with a component-wise range
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:121`: `return api.type.ParticleParamFloat.new(value as number)`
- `value : any`: Value or range (variant of number or RandomMinMax)
variant number|RandomMinMax

### ParticleParamFloat.RandomMinMax (record)

```teal
record RandomMinMax
```
Access: `api.type.ParticleParamFloat.RandomMinMax`

Random min/max range for particle parameters

- `new : function() : RandomMinMax`  (call as `api.type.ParticleParamFloat.RandomMinMax.new(…)`): Creates a new, empty `RandomMinMax`.
    - used 7x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:13`: `lifeTime = api.type.ParticleParamFloat.RandomMinMax.new(1.0, 2.0),`
- `new : function(min : number, max : number) : RandomMinMax`  (call as `api.type.ParticleParamFloat.RandomMinMax.new(…)`): Creates a new, empty `RandomMinMax`.
    - used 7x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:13`: `lifeTime = api.type.ParticleParamFloat.RandomMinMax.new(1.0, 2.0),`
- `min : number`: Minimum value
- `max : number`: Maximum value

### ParticleParamFloat.ComponentWiseMinMax (record)

```teal
record ComponentWiseMinMax
```
Access: `api.type.ParticleParamFloat.ComponentWiseMinMax`

Random min/max range for particle parameters

- `new : function() : ComponentWiseMinMax`  (call as `api.type.ParticleParamFloat.ComponentWiseMinMax.new(…)`): Creates a new, empty `ComponentWiseMinMax`.
- `new : function(min : number, max : number) : ComponentWiseMinMax`  (call as `api.type.ParticleParamFloat.ComponentWiseMinMax.new(…)`): Creates a new, empty `ComponentWiseMinMax`.
- `min : number`: Minimum value
- `max : number`: Maximum value

## ParticleParamVec2f (record)

```teal
global record ParticleParamVec2f
```
Access: `api.type.ParticleParamVec2f`

Particle system parameter for vec2f values

- `new : function() : ParticleParamVec2f`  (call as `api.type.ParticleParamVec2f.new(…)`): Creates a new particle parameter
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:125`: `return api.type.ParticleParamVec2f.new(value)`
- `new : function(value : Vec2f) : ParticleParamVec2f`  (call as `api.type.ParticleParamVec2f.new(…)`): Creates a new particle parameter with a value
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:125`: `return api.type.ParticleParamVec2f.new(value)`
- `new : function(range : RandomMinMax) : ParticleParamVec2f`  (call as `api.type.ParticleParamVec2f.new(…)`): Creates a new particle parameter with a range
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:125`: `return api.type.ParticleParamVec2f.new(value)`
- `new : function(range : ComponentWiseMinMax) : ParticleParamVec2f`  (call as `api.type.ParticleParamVec2f.new(…)`): Creates a new particle parameter with a component-wise range
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:125`: `return api.type.ParticleParamVec2f.new(value)`
- `value : any`: Value or range (variant of Vec2f or RandomMinMax)
variant Vec2f|RandomMinMax

### ParticleParamVec2f.RandomMinMax (record)

```teal
record RandomMinMax
```
Access: `api.type.ParticleParamVec2f.RandomMinMax`

Random min/max range for particle parameters

- `new : function() : RandomMinMax`  (call as `api.type.ParticleParamVec2f.RandomMinMax.new(…)`): Creates a new, empty `RandomMinMax`.
    - used 4x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:14`: `velocity = api.type.ParticleParamVec2f.RandomMinMax.new(`
- `new : function(min : Vec2f, max : Vec2f) : RandomMinMax`  (call as `api.type.ParticleParamVec2f.RandomMinMax.new(…)`): Creates a new, empty `RandomMinMax`.
    - used 4x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:14`: `velocity = api.type.ParticleParamVec2f.RandomMinMax.new(`
- `min : Vec2f`: Minimum value
- `max : Vec2f`: Maximum value

### ParticleParamVec2f.ComponentWiseMinMax (record)

```teal
record ComponentWiseMinMax
```
Access: `api.type.ParticleParamVec2f.ComponentWiseMinMax`

Random min/max range for particle parameters

- `new : function() : ComponentWiseMinMax`  (call as `api.type.ParticleParamVec2f.ComponentWiseMinMax.new(…)`): Creates a new, empty `ComponentWiseMinMax`.
- `new : function(min : Vec2f, max : Vec2f) : ComponentWiseMinMax`  (call as `api.type.ParticleParamVec2f.ComponentWiseMinMax.new(…)`): Creates a new, empty `ComponentWiseMinMax`.
- `min : Vec2f`: Minimum value
- `max : Vec2f`: Maximum value

## ParticleParamCurveFloat (record)

```teal
global record ParticleParamCurveFloat
```
Access: `api.type.ParticleParamCurveFloat`

Particle system parameter curve for float values

- `new : function() : ParticleParamCurveFloat`  (call as `api.type.ParticleParamCurveFloat.new(…)`): Creates a new particle parameter curve
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:149`: `return api.type.ParticleParamCurveFloat.new(curvePoints)`
- `new : function(points : {CurvePoint}) : ParticleParamCurveFloat`  (call as `api.type.ParticleParamCurveFloat.new(…)`): Creates a new particle parameter curve with curve points
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:149`: `return api.type.ParticleParamCurveFloat.new(curvePoints)`
- `curve : {CurvePoint}`: Array of curve points

### ParticleParamCurveFloat.CurvePoint (record)

```teal
record CurvePoint
```
Access: `api.type.ParticleParamCurveFloat.CurvePoint`

Curve point for float curve

- `new : function() : CurvePoint`  (call as `api.type.ParticleParamCurveFloat.CurvePoint.new(…)`): Creates a new, empty `CurvePoint`.
    - used 8x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:40`: `api.type.ParticleParamCurveFloat.CurvePoint.new(0.0, 0.0),`
- `new : function(time : number, value : number, easing? : integer) : CurvePoint`  (call as `api.type.ParticleParamCurveFloat.CurvePoint.new(…)`): Creates a new, empty `CurvePoint`.
    - used 8x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:40`: `api.type.ParticleParamCurveFloat.CurvePoint.new(0.0, 0.0),`
- `time : number`: Time point (0.0 to 1.0)
- `value : number`: Value at this time point
- `easing : integer`: Easing function (enum.Easing)

## ParticleParamCurveVec2f (record)

```teal
global record ParticleParamCurveVec2f
```
Access: `api.type.ParticleParamCurveVec2f`

Particle system parameter curve for vec2f values

- `new : function() : ParticleParamCurveVec2f`  (call as `api.type.ParticleParamCurveVec2f.new(…)`): Creates a new particle parameter curve
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:133`: `return api.type.ParticleParamCurveVec2f.new(curvePoints)`
- `new : function(points : {CurvePoint}) : ParticleParamCurveVec2f`  (call as `api.type.ParticleParamCurveVec2f.new(…)`): Creates a new particle parameter curve with curve points
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:133`: `return api.type.ParticleParamCurveVec2f.new(curvePoints)`
- `curve : {CurvePoint}`: Array of curve points

### ParticleParamCurveVec2f.CurvePoint (record)

```teal
record CurvePoint
```
Access: `api.type.ParticleParamCurveVec2f.CurvePoint`

Curve point for vec2f curve

- `new : function() : CurvePoint`  (call as `api.type.ParticleParamCurveVec2f.CurvePoint.new(…)`): Creates a new, empty `CurvePoint`.
    - used 8x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:53`: `api.type.ParticleParamCurveVec2f.CurvePoint.new(0.0, api.type.Vec2f.new(1.0, 1.0)),`
- `new : function(time : number, value : Vec2f, easing? : integer) : CurvePoint`  (call as `api.type.ParticleParamCurveVec2f.CurvePoint.new(…)`): Creates a new, empty `CurvePoint`.
    - used 8x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:53`: `api.type.ParticleParamCurveVec2f.CurvePoint.new(0.0, api.type.Vec2f.new(1.0, 1.0)),`
- `time : number`: Time point (0.0 to 1.0)
- `value : Vec2f`: Value at this time point
- `easing : integer`: Easing function (enum.Easing)

## ParticleParamCurveVec3f (record)

```teal
global record ParticleParamCurveVec3f
```
Access: `api.type.ParticleParamCurveVec3f`

Particle system parameter curve for vec3f values

- `new : function() : ParticleParamCurveVec3f`  (call as `api.type.ParticleParamCurveVec3f.new(…)`): Creates a new particle parameter curve
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:141`: `return api.type.ParticleParamCurveVec3f.new(curvePoints)`
- `new : function(points : {CurvePoint}) : ParticleParamCurveVec3f`  (call as `api.type.ParticleParamCurveVec3f.new(…)`): Creates a new particle parameter curve with curve points
    - used 1x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:141`: `return api.type.ParticleParamCurveVec3f.new(curvePoints)`
- `curve : {CurvePoint}`: Array of curve points

### ParticleParamCurveVec3f.CurvePoint (record)

```teal
record CurvePoint
```
Access: `api.type.ParticleParamCurveVec3f.CurvePoint`

Curve point for vec3f curve

- `new : function() : CurvePoint`  (call as `api.type.ParticleParamCurveVec3f.CurvePoint.new(…)`): Creates a new, empty `CurvePoint`.
    - used 5x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:19`: `api.type.ParticleParamCurveVec3f.CurvePoint.new(0.0, color_util.toVec3(colors.LevelUpStars)),`
- `new : function(time : number, value : Vec3f, easing? : integer) : CurvePoint`  (call as `api.type.ParticleParamCurveVec3f.CurvePoint.new(…)`): Creates a new, empty `CurvePoint`.
    - used 5x in the base game, e.g. `base/content/gui/gui/main/particle_react_util.tl:19`: `api.type.ParticleParamCurveVec3f.CurvePoint.new(0.0, color_util.toVec3(colors.LevelUpStars)),`
- `time : number`: Time point (0.0 to 1.0)
- `value : Vec3f`: Value at this time point
- `easing : integer`: Easing function (enum.Easing)

## UserProfile (record)

```teal
global record UserProfile
```

UserProfile

- `getModRep : function(UserProfile) : ModRep`: Returns the repository of installed mods.
    - returns: The mod repository.

    ```lua
    local modRep = app.getUserProfile():getModRep()
    for _, modId in ipairs(modRep:getInstalledMods()) do
      print(modRep:getModDesc(modId).visible)
    end
    ```

- `getLanguageRep : function(UserProfile) : LanguageRep`: Returns the repository of available languages.
    - returns: The language repository.
- `getUserName : function(UserProfile) : string`: Returns the name of the user profile.
    - returns: The user name.
- `changeLanguage : function(UserProfile, LangKey)`: Changes the display language of the game.
    - param `#2`: The new language, as listed by `LanguageRep.getAllLanguages`.
- `getLanguage : function(UserProfile) : LangKey`: Returns the current display language.
    - returns: The current language; its `code` field holds the language code.
- `subscribeMod : function(UserProfile, string)`
- `unsubscribeMod : function(UserProfile, string)`
- `getUnlockedAchievementIds : function(UserProfile) : {string : boolean}`: Returns the achievements the user has unlocked.
    - returns: Map from achievement id to `true`.
- `getAchievementRep : function(UserProfile) : AchievementRep`: Returns the repository of all achievements.
    - returns: The achievement repository.
- `getPlayerStatProgress : function(UserProfile, string) : number`: Returns the progress of one player stat.
    - param `#2`: Id of the player stat.
    - returns: The current value of the stat.
- `getPlayerStatsProgress : function(UserProfile) : {string : number}`: Returns the progress of all player stats.
    - returns: Map from player stat id to its current value.
- `getLastMission : function(UserProfile, string) : {string, SaveGameId}`: campaign
- `getLastSavegameForMission : function(UserProfile, campaign : string, mission : string) : SaveGameId`: Returns the last savegame of a mission.
    - param `campaign`: Campaign name.
    - param `mission`: Mission name.
    - returns: Savegame id; its `saveGameName` is empty if there is no savegame.
- `getLastGame : function(UserProfile) : SaveGameId | MissionCampaign`: campaign
- `isMissionCompleted : function(UserProfile, string, string) : boolean`: campaign, mission
- `isCurrentMissionCompleted : function(UserProfile) : boolean`: automatically gets campaign/mission from running game
- `noteEditorsNoteShown : function(UserProfile)`: Records that the campaign editor's note has been shown.
- `isEditorsNoteShown : function(UserProfile)`: Checks whether the campaign editor's note has already been shown.
    - notes: The definition declares no return value, but the base game uses the result as a boolean (`if not app.getUserProfile():isEditorsNoteShown() then ... end`).
- `noteMedalState : function(UserProfile, medal : string, achieved : boolean)`: automatically gets campaign/mission from running game
- `getMedalsState : function(UserProfile, campaign : string, mission : string) : {string : boolean}`: Returns which medals of a mission have been achieved.
    - param `campaign`: Campaign name.
    - param `mission`: Mission name.
    - returns: Map from medal id to whether it has been achieved.
- `noteStarState : function(UserProfile, star : string, achieved : boolean)`: automatically gets campaign/mission from running game
- `getStarsState : function(UserProfile, campaign : string, mission : string) : {string : boolean}`: Returns which stars of a mission have been achieved.
    - param `campaign`: Campaign name.
    - param `mission`: Mission name.
    - returns: Map from star id to whether it has been achieved.
- `hasUgcAccess : function(UserProfile) : boolean`: Checks whether the user currently has access to user-generated content (the Mod Hub).
    - returns: `true` if access is available.
- `checkUgcRestrictions : function(UserProfile) : boolean`: Checks the user-generated content restrictions again and tries to gain access.
    - returns: `true` if access was gained; otherwise `getUgcAccessLimitedReason` gives the reason.
- `hasAgeGroupForUgc : function(UserProfile) : boolean`: Checks whether the user is old enough for user-generated content; the base game shows "You must be 13 or older to access the Mod Hub" otherwise.
    - returns: `true` if the age requirement is met.
- `getUgcAccessLimitedReason : function(UserProfile) : UgcAccessLimitedReason`: Returns why access to user-generated content is limited.
    - returns: The reason (`None`, `NotSignedIn`, `Restricted` or `Error`).

## CommandProgress (record)

```teal
global record CommandProgress
```
Access: `api.type.CommandProgress`

Command progress tracker

- `new : function() : CommandProgress`  (call as `api.type.CommandProgress.new(…)`): Creates a new, empty `CommandProgress`.
    - used 6x in the base game, e.g. `base/content/gui/gui/map_editor/map_editor.tl:1006`: `local a = api.type.CommandProgress.new()`
- `get : function(CommandProgress) : number`  (call as `api.type.CommandProgress.get(…)`): Returns the progress of the command.
    - returns: Progress from `0.0` to `1.0`; `1.0` when the command has finished.

    ```lua
    local progress = api.type.CommandProgress.new()
    api.cmd.sendCommand(cmd, callback, progress)
    -- later, for example every step:
    print(progress:get())
    ```

## BaseConfig (record)

```teal
global record BaseConfig
```
Access: `api.type.BaseConfig`

Base configuration of a single game

- `animal : Animal`: Defines the properties of an animal
- `locations : Locations`: Town and industry placement at map generation.
- `settings : Settings`: Graphics quality settings (view distances and LOD scaling per quality mode).
- `particles : Particles`: Particle textures.
- `water : Water`: Water textures.
- `emission : Emissions`: Noise and pollution bounds and town noise settings; read with `api.res.getBaseConfig().emission`.
- `scaffold : {string : {string : {string}}}`: Models for construction sites per era, as a map from era to a map from part (`scaffold`, `crane`) to a list of `.mdl` files.

    ```lua
    baseConfig.scaffold = {
        era_a = {
            scaffold = { "::/assets/construction/scaffolding/era_a_normal.mdl",
                "::/assets/construction/scaffolding/era_a_ladder.mdl" },
            crane = { "::/assets/construction/crane/era_b_undercarriage.mdl",
                "::/assets/construction/crane/era_b_mast.mdl" },
        },
    }
    ```

- `costs : {string : number}`: Named cost values and cost scale factors, as a map from key to number.
    - notes: Keys set by the base game: `terrainRaise` and `terrainLower` (per cubic metre), `railroadCatenary`, `roadBusLane`, `roadTramLane`, `roadElectricTramLane` (fractions of the road or track cost), `bulldozeRefundTimeOut`, `bulldozeRefundFactor`, `removeField`, `tree`, and the difficulty scales `infrastructureMaintenanceScale`, `infrastructureCostScale`, `vehicleMaintenanceScale` and `vehiclePurchaseCostScale`.
- `advancedOptions : AdvancedOptions`: Advanced game options (maintenance effects, subsidies, landmark resources, reforestation).
- `economy : Economy`: Cargo and passenger income scripts and town and industry development settings.
- `terrain : Terrain`: Legacy terrain mesh settings (deprecated).
- `environment : ResName`: Resource name of the environment (see `api.res.environmentRep`).
- `economyId : ResName`: Resource name of the selected economy (`.eco`); the base game sets it from the `economy` entry of the game settings.
- `nameId : ResName`: Resource name of the selected name list; the base game sets it from the `nameList` entry of the game settings.
- `climate : ResName`: Resource name of the selected climate (`.clima`), e.g. `::/climates/temperate/temperate.clima`; the base game sets it from the `climate` entry of the game settings.
- `trackCatenaryYearFrom : integer`: Year from which rail electrification (catenary) becomes available (base game 1921).
- `busLaneYearFrom : integer`: Year from which bus lanes become available (base game 1925).
- `terrainToolMinStrength : number`: Minimum strength of the terrain tool (base game 5).
- `terrainToolMaxStrength : number`: Maximum strength of the terrain tool (base game 100).
- `enforceMainConnections : boolean`: Whether the game checks if main connections are cut and warns the player, as the wiki describes it (base game `true`).
- `townDevelopInterval : number`: Interval between town development updates (base game 60).
- `chargeMaintenanceInterval : number`: Interval at which maintenance is charged (base game 60).
- `simulateCargoWeight : boolean`: Whether the weight of transported cargo is simulated (base game `true`).
- `simPersonDestinationRecomputationProbability : number`: Probability that a simulated person recomputes its destination (base game 1.0).
- `townMajorStreetAngleRange : number`: Angle range in degrees within which the directions of a town's major streets are randomized (base game 0).
- `townConnectionMaxBridgeLength : number`: Towns are connected by bridges only if the distance is below this value (base game 250).
- `maxVehicleCloneAmount : number`: Maximum number of vehicles that can be cloned at once; the wiki says only the console edition uses it (base game 50).
- `trainAccelerationFactor : number`: Multiplier for train acceleration (base game 1.0).
- `trainBrakeDeceleration : number`: Global brake deceleration of trains. The wiki gives 2.5 as the default and says lower values make trains brake more slowly.
    - notes: The base `preRunFn` (`base/content/base/base/mod.script.tl`) doesn't set it. Every base train and tram also sets its own `landVehicle.brakeDeceleration` (2.5) in its model; the game files don't say how the global value and the per-model value combine.
- `terrainToolMinSize : number`: Minimum brush size of the terrain tool.
- `terrainToolMaxSize : number`: Maximum brush size of the terrain tool in the game; one size unit covers 4 m² (base game 32).
- `terrainToolScale : number`: Factor applied to the terrain tool brush sizes when they are listed in the construction menu.
- `terrainToolMaxSizeEditor : number`: Maximum brush size of the terrain tool in the map editor; one size unit covers 4 m² (base game 256).
- `terrainPainterMinSize : number`: Minimum brush size of the terrain painter; the game GUI also uses it as the terrain tool minimum.
- `terrainPainterMaxSize : number`: Maximum brush size of the terrain painter in the game; one size unit covers 1 m² (base game 256).
- `terrainPainterMaxSizeEditor : number`: Maximum brush size of the terrain painter in the map editor; one size unit covers 1 m² (base game 256).
- `terrainPainterScale : number`: Factor applied to the terrain painter brush sizes.
- `assetBrushMinSize : number`: Minimum brush size of the asset tools (placing and erasing assets and vegetation).
- `assetBrushMaxSize : number`: Maximum brush size of the asset tools; one size unit covers 1 m² (base game 256).
- `assetBrushScale : number`: Factor applied to the asset brush sizes when they are listed in the construction menu.
- `treeCostYearMultipliers : {integer : number}`: Tree cost multipliers by year, as a map from year to factor; the GUI evaluates it with `api.engine.util.construction.getCostMultiplierForYear`.

    ```lua
    baseConfig.treeCostYearMultipliers = {
        [1900] = 1.0, [1920] = 1.5, [1940] = 2.2, [1960] = 3.2,
        [1980] = 4.0, [2000] = 5.0, [2020] = 6.0,
    }
    ```

- `townInitialMajorStreetAngleRange : number`: Like `townMajorStreetAngleRange` (degrees), but only applied when a town is first created (base game 0).
- `townCapacityScalingFactor : number`: Capacity scaling factor passed to the town builder (`townBuilder.capacityScalingFactor`) when a town is placed.
- `sandboxMode : boolean`: Whether sandbox mode is active; the `urbangames_sandbox` mod sets it to `true`.
- `noCosts : boolean`: Whether building and buying is free; the `urbangames_no_costs` mod sets it to `true`.
- `constructionScript : ScriptRef`: Script used to build constructions from their modules.
    - notes: The base game sets `fileName` to `::scripts/construction/construction.script@constructWithModules` and passes the infrastructure cost and maintenance scales in `params.costs`.

### BaseConfig.AdvancedOptions (record)

```teal
record AdvancedOptions
```
Access: `api.type.BaseConfig.AdvancedOptions`

Advanced gameplay option

- `trafficSpeedSensitivityScale : number`: Scale of how sensitive town traffic ratings are to traffic speed; passed to `api.engine.util.town.computeTownsTrafficSpeedMap`. The base game sets it to `1.0`.
- `vehicleMaintenanceEmissionsEffect : number`: Strength of the effect of vehicle maintenance on emissions; the base game sets it from the "vehicle maintenance effect" game setting (`0` to `2`).
- `vehicleMaintenancePerformanceEffect : number`: Strength of the effect of vehicle maintenance on performance; the base game sets it from the "vehicle maintenance effect" game setting (`0` to `2`).
- `vehicleMaintenanceRunningCostsEffect : number`: Strength of the effect of vehicle maintenance on running costs; the base game sets it from the "vehicle maintenance effect" game setting (`0` to `2`).
- `missionVehicleMaintenanceDisableRepair : boolean`: Disables vehicle repair; set by campaign missions.
- `missionDisableTunnelBuilding : boolean`: Disables tunnel building; set by campaign missions.
- `missionEmissionsScriptFactor : number`: Factor applied to the noise and pollution power that the emissions script sets on entities.
    - notes: When it is `0`, the emissions game script does nothing.
- `simPersonHappinessDecayAtVehicle : number`: Rate at which the happiness of sim persons decreases while in a vehicle; campaign missions scale it.
- `simPersonHappinessDecayAtTerminal : number`: Rate at which the happiness of sim persons decreases while waiting at a terminal; campaign missions scale it.
- `subventionMode : integer`: How often subsidy (subvention) offers appear; `0` disables them.
    - notes: The base game sets it to the "subvention mode" game setting minus one. The spawn interval is the default month duration (longer for higher company ranks) multiplied by `12` (mode `1`), `6` (mode `2`), `3` (mode `3`) or `1` (mode `4`).
- `subventionRisk : integer`: Risk level of subsidy (subvention) offers, from `0` to `3`.
    - notes: The base game sets it to the "subvention risk" game setting minus one. The subsidy scripts turn it into a factor of `2`, `1`, `0.75` or `0.625` that scales both the time allowed and the upfront payment, so higher values give less time and less money.
- `subventionFinancialRewardMultiplier : number`: Multiplier for the financial reward of subsidies (subventions)
- `landmarkResources : number`: Multiplier for the resource amounts landmarks require (`0`, `0.5`, `1`, `2` or `4` depending on the game setting).
- `isReforestationActive : boolean`: Whether reforestation is active; the base game turns it off in the map editor.

### BaseConfig.Animal (record)

```teal
record Animal
```
Access: `api.type.BaseConfig.Animal`

Configuration for animal spawning

- `new : function() : Animal`  (call as `api.type.BaseConfig.Animal.new(…)`): Creates a new object of type Animal
    - used 1x in the base game, e.g. `base/content/base/base/mod.script.tl:138`: `baseConfig.animal = fromTable(api.type.BaseConfig.Animal.new(), {`
- `populationDensityMultiplier : number`: Multiply the density (in square exemplars per square km) by this factor for all animals
- `useLocalSpawning : boolean`: Spawning only looks at the existing population within localTileSize squared boxes instead of the entire map (results in density
more similarly distributed to the actual scoring functions)
- `localTileSize : number`: Size used for controlling local spawning
- `notifyAnimalDespawn : boolean`: Add notification when a named animal despawns

### BaseConfig.Locations (record)

```teal
record Locations
```
Access: `api.type.BaseConfig.Locations`

Configuration for the placement of industry and towns on max generation

- `new : function() : Locations`  (call as `api.type.BaseConfig.Locations.new(…)`): Creates a new object of type Locations
    - used 3x in the base game, e.g. `base/content/base/base/mod.script.tl:122`: `baseConfig.locations = fromTable(api.type.BaseConfig.Locations.new(), {`
- `town : TownParamList`: Configuration for town spawning at map generation
- `industry : IndustryParamList`: Configuration for industry spawning at map generation
- `makeInitialStreets : boolean`: Add small streets at the edge of industries

#### BaseConfig.Locations.IndustryParamList (record)

```teal
record IndustryParamList
```
Access: `api.type.BaseConfig.Locations.IndustryParamList`

Configuration parameters for industries

- `new : function() : IndustryParamList`  (call as `api.type.BaseConfig.Locations.IndustryParamList.new(…)`): Creates a new object of type IndustryParamList
    - used 1x in the base game, e.g. `base/content/base/base/mod.script.tl:128`: `industry = fromTable(api.type.BaseConfig.Locations.IndustryParamList.new(),{`
- `absoluteMinimum : integer`: Spawning will ensure at least this number of industries is present on the map
- `maxNumberPerArea : number`: Spawning will ensure that the density per square km will not exceed this number
- `industryDensity : number`: Spawning will try to spawn this number of industries
- `randomSize : boolean`: Productivity will be randomized slightly to induce more variation
- `industryProductivity : number`: Scale the industry productivity by this factor

#### BaseConfig.Locations.TownParamList (record)

```teal
record TownParamList
```
Access: `api.type.BaseConfig.Locations.TownParamList`

Configuration parameters for towns

- `new : function() : TownParamList`  (call as `api.type.BaseConfig.Locations.TownParamList.new(…)`): Creates a new object of type TownParamList
    - used 1x in the base game, e.g. `base/content/base/base/mod.script.tl:123`: `town =  fromTable(api.type.BaseConfig.Locations.TownParamList.new(), {`
- `maxNumberPerArea : number`: Spawning will ensure at most this density per square km
- `allowInRoughTerrain : boolean`: Town will be able to spawn in uneven and slightly unsuitable terrain, the resulting town might
be smaller than it's target size
- `townFrequency : number`: Number of towns per sqm
- `populationDensity : number`: Population will be scaled by this factor

### BaseConfig.Terrain (record)

```teal
record Terrain
```
Access: `api.type.BaseConfig.Terrain`

Configuration for the terrain mesh (Deprecated)

- `lodErrorMultiplier : number`: (Deprecated) Only used with legacy terrain, determines the quality of the terrain mesh

### BaseConfig.Economy (record)

```teal
record Economy
```
Access: `api.type.BaseConfig.Economy`

Configures cargo relations and industry developement for a single game

- `new : function() : Economy`  (call as `api.type.BaseConfig.Economy.new(…)`): Creates a new empty economy
- `cargoIncomeScript : ScriptRef`: A script reference controlling the income of a given cargo given specific parameters
- `passengerIncomeScript : ScriptRef`: A script reference controlling the income of a given passengers given specific parameters
- `townDevelopment : TownDevelopment`: Controls the automatic development of towns
- `industryDevelopment : IndustryDevelopment`: Controls the automatic developement of industries

#### BaseConfig.Economy.TownDevelopment (record)

```teal
record TownDevelopment
```
Access: `api.type.BaseConfig.Economy.TownDevelopment`

Parameters for the town developement

- `cargoNeedsPerTown : integer`: Number of initial cargo needs when the town ist first built
- `townGrowthMinDistrictFraction : number`: The minimum a distric fraction (compared to other disctricts) can shrink

#### BaseConfig.Economy.IndustryDevelopment (record)

```teal
record IndustryDevelopment
```
Access: `api.type.BaseConfig.Economy.IndustryDevelopment`

Parameters for the industry developement

- `spawnTargetTimeSpan : number`: Unused
- `spawnProbabilityExponent : number`: Control the frequency at which enw industries appear
- `spawnIndustries : boolean`: Control whether new industry might spawn on the map
- `unusedTimeSpan : number`: Control the mean time after which an industry will begin closing after not being used
- `closureCountdownTimeSpan : number`: Controls the grace period the player has to prevent closure
- `closureProbability : number`: Controls how likely an unused industry has of being closed
- `extendTimeSpan : number`: Control how often the industry will build new satellites

### BaseConfig.Settings (record)

```teal
record Settings
```
Access: `api.type.BaseConfig.Settings`

Graphic settings for main quality modes, camera tool and cockpit

- `new : function() : Settings`  (call as `api.type.BaseConfig.Settings.new(…)`): Creates a new graphic settings parameters
    - used 1x in the base game, e.g. `base/content/base/base/mod.script.tl:146`: `baseConfig.settings =  fromTable(api.type.BaseConfig.Settings.new(), {`
- `geometryQualityOptions : {GeometryQualityOption}`: Quality settings for: {low, medium, high, cockpit, camera tool}

#### BaseConfig.Settings.GeometryQualityOption (record)

```teal
record GeometryQualityOption
```
Access: `api.type.BaseConfig.Settings.GeometryQualityOption`

Contains information about graphi quality of a single game

- `new : function() : GeometryQualityOption`  (call as `api.type.BaseConfig.Settings.GeometryQualityOption.new(…)`): Creates a new GeometryQualityOption
    - used 6x in the base game, e.g. `base/content/base/base/mod.script.tl:148`: `fromTable(api.type.BaseConfig.Settings.GeometryQualityOption.new(), { viewNearFar = api.type.Vec2f.new(4.0, 7200.0), fogStartEndFarPerc = api.type.Vec2f.new(0.4`
- `viewNearFar : Vec2f`: Distance im meters of near and far plane
- `fogStartEndFarPerc : Vec2f`: Unused
- `lodDistanceScaling : number`: A factor multiplying all lod distances

### BaseConfig.Water (record)

```teal
record Water
```
Access: `api.type.BaseConfig.Water`

Configures water look and feel

- `textures : Textures`: Configures water textures

#### BaseConfig.Water.Textures (record)

```teal
record Textures
```
Access: `api.type.BaseConfig.Water.Textures`

Configures water textures

- `foamNormal : string`: Normal texture for water foam
- `foamDetail : string`: Extra detail texture for water foam
- `foam : string`: Albedo texture for water foam

### BaseConfig.Particles (record)

```teal
record Particles
```
Access: `api.type.BaseConfig.Particles`

Particle rendering configuration; holds the textures shared by particle effects.

- `textures : Textures`: Shared particle textures.

#### BaseConfig.Particles.Textures (record)

```teal
record Textures
```
Access: `api.type.BaseConfig.Particles.Textures`

Shared textures used by particle effects.

- `noise0 : string`: First noise texture for particles (base game: `::/base/tex/particle_noise0.dds`).
- `noise1 : string`: Second noise texture for particles (base game: `::/base/tex/particle_noise1.dds`).
- `smoke : string`: Smoke texture for particles (base game: `::/base/tex/particle_smoke.dds`).
- `smokeNormal : string`: Normal map for the smoke particle texture (base game: `::/base/tex/particle_smoke_normal.dds`).

### BaseConfig.Emissions (record)

```teal
record Emissions
```
Access: `api.type.BaseConfig.Emissions`

Noise and pollution settings for the emission layers and for how noise affects towns.

Notes:

The base game copies the bounds from `/game_mechanics/emission/emission_config.tl`
(noise 63-85 dB, pollution 57-80) in its `preRunFn`. Scripts read it with
`api.res.getBaseConfig().emission`.

- `noiseLowerBoundDB : number`: Lower bound of the noise scale in decibels, used as the minimum when sampling and displaying noise (base game 63).
- `noiseUpperBoundDB : number`: Upper bound of the noise scale in decibels, used as the maximum when sampling and displaying noise (base game 85).
- `pollutionLowerBoundDB : number`: Lower bound of the pollution scale, used as the minimum when sampling and displaying pollution (base game 57).
- `pollutionUpperBoundDB : number`: Upper bound of the pollution scale, used as the maximum when sampling and displaying pollution (base game 80).
- `townNoiseResidentialAffected : boolean`: Whether noise affects residential town buildings; `town_util.getNoiseLandUseTypes` includes `LandUseType.RESIDENTIAL` when set.
- `townNoiseCommercialAffected : boolean`: Whether noise affects commercial town buildings; `town_util.getNoiseLandUseTypes` includes `LandUseType.COMMERCIAL` when set.
- `townNoiseIndustrialAffected : boolean`: Whether noise affects industrial town buildings; `town_util.getNoiseLandUseTypes` includes `LandUseType.INDUSTRIAL` when set.

## Type (record)

```teal
global record Type
```
Access: `api.type`

Types namespace (used in `api.type.`...)

- `ScriptParam : ScriptParam`: `ScriptParam` type, reached as `api.type.ScriptParam`; `api.type.ScriptParam.new(...)` creates a new value.
- `LangKey : LangKey`: `LangKey` type, reached as `api.type.LangKey`.
- `ModDesc : Mod.ModDesc`: `Mod.ModDesc` type, reached as `api.type.ModDesc`.
- `ModId : Mod.ModId`: `Mod.ModId` type, reached as `api.type.ModId`; `api.type.ModId.new(...)` creates a new value.
- `KeyCombo : KeyCombo`: `KeyCombo` type, reached as `api.type.KeyCombo`; `api.type.KeyCombo.new(...)` creates a new value.
- `KeyComboDef : KeyComboDef`: `KeyComboDef` type, reached as `api.type.KeyComboDef`; `api.type.KeyComboDef.new(...)` creates a new value.
- `AppConfig : AppConfig`: `AppConfig` type, reached as `api.type.AppConfig`; `api.type.AppConfig.new(...)` creates a new value.
- `enum : Enum`: Enumerations namespace, reached as `api.type.enum` (base game scripts write `api.type["enum"]`), e.g. `api.type.enum.TransportMode`.
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/tools/town_builder_tool.script.tl:145`: `displayMode = api.type.enum["ScriptParamDisplayMode"].Sparse,`
- `Vec5f : Vec5f`: `Vec5f` type, reached as `api.type.Vec5f`; `api.type.Vec5f.new(...)` creates a new value.
- `Vec4f : Vec4f`: `Vec4f` type, reached as `api.type.Vec4f`; `api.type.Vec4f.new(...)` creates a new value.
- `Vec3f : Vec3f`: `Vec3f` type, reached as `api.type.Vec3f`; `api.type.Vec3f.new(...)` creates a new value.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:1777`: `position : api.type.Vec3f`
- `Vec2f : Vec2f`: `Vec2f` type, reached as `api.type.Vec2f`; `api.type.Vec2f.new(...)` creates a new value.
    - used 15x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:36`: `updateListConstructionDefinition : function(categoryIndex : integer, definition : ConstructionDefinition, forInfoPanel : boolean, position? : api.type.Vec2f, fo`
- `Mat4f : Mat4f`: `Mat4f` type, reached as `api.type.Mat4f`; `api.type.Mat4f.new(...)` creates a new value.
- `Vec2i : Vec2i`: `Vec2i` type, reached as `api.type.Vec2i`; `api.type.Vec2i.new(...)` creates a new value.
- `Box3 : Box3f`: `Box3f` type, reached as `api.type.Box3`.
- `Box2 : Box2f`: `Box2f` type, reached as `api.type.Box2`.
- `Clock : Clock`: `Clock` type, reached as `api.type.Clock`; `api.type.Clock.new(...)` creates a new value.
- `CommandProgress : CommandProgress`: `CommandProgress` type, reached as `api.type.CommandProgress`; `api.type.CommandProgress.new(...)` creates a new value.
- `LoaderHelper : LoaderHelper`: `LoaderHelper` type, reached as `api.type.LoaderHelper`.
- `Connection : Connection`: `Connection` type, reached as `api.type.Connection`.
- `ComponentType : Engine.ComponentType`: Component type enum `Engine.ComponentType`, reached as `api.type.ComponentType`; pass its values (e.g. `api.type.ComponentType.TRANSPORT_VEHICLE`) to `api.engine.getComponent`.
    - used 1x in the base game, e.g. `base/content/gui/gui/debug_panel/make_entity_debug_panel.tl:729`: `for componentName, v in orderedPairs(api.type.ComponentType.__index) do`
- `Date : Date`: `Date` type, reached as `api.type.Date`; `api.type.Date.new(...)` creates a new value.
- `JournalEntry : JournalEntry`: `JournalEntry` type, reached as `api.type.JournalEntry`; `api.type.JournalEntry.new(...)` creates a new value.
- `EdgeId : EdgeId`: `EdgeId` type, reached as `api.type.EdgeId`.
- `EdgePos : EdgePos`: `EdgePos` type, reached as `api.type.EdgePos`; `api.type.EdgePos.new(...)` creates a new value.
- `NodeId : NodeId`: `NodeId` type, reached as `api.type.NodeId`; `api.type.NodeId.new(...)` creates a new value.
- `Waypoint : Waypoint`: `Waypoint` type, reached as `api.type.Waypoint`; `api.type.Waypoint.new(...)` creates a new value.
- `SignalId : SignalId`: `SignalId` type, reached as `api.type.SignalId`.
- `StationTerminal : StationTerminal`: `StationTerminal` type, reached as `api.type.StationTerminal`; `api.type.StationTerminal.new(...)` creates a new value.
- `ScriptRef : ScriptRef`: `ScriptRef` type, reached as `api.type.ScriptRef`; `api.type.ScriptRef.new(...)` creates a new value.
- `ModuleDesc : ModuleDesc`: `ModuleDesc` type, reached as `api.type.ModuleDesc`; `api.type.ModuleDesc.new(...)` creates a new value.
- `ConstructionDesc : ConstructionDesc`: `ConstructionDesc` type, reached as `api.type.ConstructionDesc`; `api.type.ConstructionDesc.new(...)` creates a new value.
- `GameScriptDesc : GameScriptDesc`: `GameScriptDesc` type, reached as `api.type.GameScriptDesc`.
- `DynamicConstructionTemplate : DynamicConstructionTemplate`: `DynamicConstructionTemplate` type, reached as `api.type.DynamicConstructionTemplate`.
- `Line : Engine.Component.Line`: `Engine.Component.Line` type, reached as `api.type.Line`; `api.type.Line.new(...)` creates a new value.
- `EmissionGrid : Engine.Component.EmissionGrid`: `Engine.Component.EmissionGrid` type, reached as `api.type.EmissionGrid`.
- `MovePathAircraft : Engine.Component.MovePathAircraft`: `Engine.Component.MovePathAircraft` type, reached as `api.type.MovePathAircraft`; `api.type.MovePathAircraft.new(...)` creates a new value.
- `TransportVehicle : Engine.Component.TransportVehicle`: `Engine.Component.TransportVehicle` type, reached as `api.type.TransportVehicle`.
- `LoadConfig : LoadConfig`: `LoadConfig` type, reached as `api.type.LoadConfig`; `api.type.LoadConfig.new(...)` creates a new value.
- `BaseConfig : BaseConfig`: Base configuration type `BaseConfig`, reached as `api.type.BaseConfig`; mod scripts create its nested records with e.g. `api.type.BaseConfig.Locations.new()`.
- `CargoTypeSet : CargoTypeSet`: `CargoTypeSet` type, reached as `api.type.CargoTypeSet`; `api.type.CargoTypeSet.new(...)` creates a new value.
- `TransportVehiclePart : TransportVehiclePart`: `TransportVehiclePart` type, reached as `api.type.TransportVehiclePart`; `api.type.TransportVehiclePart.new(...)` creates a new value.
- `TransportVehicleConfig : TransportVehicleConfig`: `TransportVehicleConfig` type, reached as `api.type.TransportVehicleConfig`; `api.type.TransportVehicleConfig.new(...)` creates a new value.
- `Context : Context`: `Context` type, reached as `api.type.Context`; `api.type.Context.new(...)` creates a new value.
- `Proposal : Proposal`: `Proposal` type, reached as `api.type.Proposal`.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:295`: `local p = param as {api.type.Proposal, api.type.ProposalData, {api.engine.Entity}, boolean}`
- `SimpleProposal : SimpleProposal`: `SimpleProposal` type, reached as `api.type.SimpleProposal`; `api.type.SimpleProposal.new(...)` creates a new value.
- `ProposalData : ProposalData`: `ProposalData` type, reached as `api.type.ProposalData`.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:295`: `local p = param as {api.type.Proposal, api.type.ProposalData, {api.engine.Entity}, boolean}`
- `StreetProposal : Proposal.StreetProposal`: `Proposal.StreetProposal` type, reached as `api.type.StreetProposal`.
- `SimpleStreetProposal : SimpleStreetProposal`: `SimpleStreetProposal` type, reached as `api.type.SimpleStreetProposal`.
- `CatchmentArea : CatchmentArea`: `CatchmentArea` type, reached as `api.type.CatchmentArea`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:1522`: `local function appendResult(catchments : {Type.CatchmentArea}, action : TownProposalActions.Action)`
- `LayerConfig : LayerConfig`: `LayerConfig` type, reached as `api.type.LayerConfig`; `api.type.LayerConfig.new(...)` creates a new value.
- `NodeViewerData : NodeViewerData`: `NodeViewerData` type, reached as `api.type.NodeViewerData`; `api.type.NodeViewerData.new(...)` creates a new value.
- `ParticleParamFloat : ParticleParamFloat`: `ParticleParamFloat` type, reached as `api.type.ParticleParamFloat`; `api.type.ParticleParamFloat.new(...)` creates a new value.
- `ParticleParamVec2f : ParticleParamVec2f`: `ParticleParamVec2f` type, reached as `api.type.ParticleParamVec2f`; `api.type.ParticleParamVec2f.new(...)` creates a new value.
- `ParticleParamCurveFloat : ParticleParamCurveFloat`: `ParticleParamCurveFloat` type, reached as `api.type.ParticleParamCurveFloat`; `api.type.ParticleParamCurveFloat.new(...)` creates a new value.
- `ParticleParamCurveVec2f : ParticleParamCurveVec2f`: `ParticleParamCurveVec2f` type, reached as `api.type.ParticleParamCurveVec2f`; `api.type.ParticleParamCurveVec2f.new(...)` creates a new value.
- `ParticleParamCurveVec3f : ParticleParamCurveVec3f`: `ParticleParamCurveVec3f` type, reached as `api.type.ParticleParamCurveVec3f`; `api.type.ParticleParamCurveVec3f.new(...)` creates a new value.
- `Path : Path`: `Path` type, reached as `api.type.Path`.
- `PathPos : PathPos`: `PathPos` type, reached as `api.type.PathPos`.
- `AnimationState : Transformator.AnimationState`: `Transformator.AnimationState` type, reached as `api.type.AnimationState`; `api.type.AnimationState.new(...)` creates a new value.
- `HeighmapGenerationMode : HeighmapGenerationMode`: `HeighmapGenerationMode` type, reached as `api.type.HeighmapGenerationMode`; `api.type.HeighmapGenerationMode.new(...)` creates a new value.
- `TownIndustryGenerationMode : TownIndustryGenerationMode`: `TownIndustryGenerationMode` type, reached as `api.type.TownIndustryGenerationMode`; `api.type.TownIndustryGenerationMode.new(...)` creates a new value.
- `ImportExportHeightmapMode : ImportExportHeightmapMode`: `ImportExportHeightmapMode` type, reached as `api.type.ImportExportHeightmapMode`; `api.type.ImportExportHeightmapMode.new(...)` creates a new value.
- `ImportBiomesMode : ImportBiomesMode`: `ImportBiomesMode` type, reached as `api.type.ImportBiomesMode`; `api.type.ImportBiomesMode.new(...)` creates a new value.
- `StreetGenerationMode : StreetGenerationMode`: `StreetGenerationMode` type, reached as `api.type.StreetGenerationMode`; `api.type.StreetGenerationMode.new(...)` creates a new value.
- `Map : GameMap`: `GameMap` type, reached as `api.type.Map`; `api.type.Map.new(...)` creates a new value.
- `StartGameParams : StartGameParams`: `StartGameParams` type, reached as `api.type.StartGameParams`.
- `SaveGameInfo : SaveGameInfo`: `SaveGameInfo` type, reached as `api.type.SaveGameInfo`.
- `SavegameId : SaveGameId`: `SaveGameId` type, reached as `api.type.SavegameId`; `api.type.SavegameId.new(...)` creates a new value.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mission_page_content.tl:382`: `local existingSavegameState : ReactStateT<Type.SavegameId> = react.useState()`
- `SaveGameDetails : SaveGameData.SaveGameDetails`: `SaveGameData.SaveGameDetails` type, reached as `api.type.SaveGameDetails`; `api.type.SaveGameDetails.new(...)` creates a new value.
- `SaveGameData : SaveGameData`: `SaveGameData` type, reached as `api.type.SaveGameData`; `api.type.SaveGameData.new(...)` creates a new value.
- `ChartConfig : ChartConfig`: `ChartConfig` type, reached as `api.type.ChartConfig`; `api.type.ChartConfig.new(...)` creates a new value.
- `FinanceData : FinanceData`: `FinanceData` type, reached as `api.type.FinanceData`.
- `Failure : Failure`: `Failure` type, reached as `api.type.Failure`.
- `NodeAndEntity : Proposal.NodeAndEntity`: `Proposal.NodeAndEntity` type, reached as `api.type.NodeAndEntity`; `api.type.NodeAndEntity.new(...)` creates a new value.
- `SegmentAndEntity : Proposal.SegmentAndEntity`: `Proposal.SegmentAndEntity` type, reached as `api.type.SegmentAndEntity`; `api.type.SegmentAndEntity.new(...)` creates a new value.
- `BaseNodeLaneConnectionAndEntity : Proposal.BaseNodeLaneConnectionAndEntity`: `Proposal.BaseNodeLaneConnectionAndEntity` type, reached as `api.type.BaseNodeLaneConnectionAndEntity`.
- `EdgeGeometry : EdgeGeometry`: `EdgeGeometry` type, reached as `api.type.EdgeGeometry`.
- `MetadataMap : ModelMetadata`: Model metadata type `ModelMetadata`, reached as `api.type.MetadataMap`; its nested records (e.g. `api.type.MetadataMap.MenuCategory`) describe the `metadata` keys of `.mdl` files.
- `Collider : Collider`: `Collider` type, reached as `api.type.Collider`.
- `mod : Mod`: Mod type namespace `Mod`, reached as `api.type.mod` (e.g. `api.type.mod.ModId`).
- `modhub : Modhub`: Mod hub type namespace `Modhub`, reached as `api.type.modhub` (e.g. `api.type.modhub.StagingModData`).
- `TrackType : TrackType`: `TrackType` type, reached as `api.type.TrackType`.
- `GenericGameRes : GenericGameRes`: `GenericGameRes` type, reached as `api.type.GenericGameRes`.
- `BridgeType : BridgeType`: `BridgeType` type, reached as `api.type.BridgeType`.
- `Industry : Engine.Component.Industry`: `Engine.Component.Industry` type, reached as `api.type.Industry`.
- `StockList : Engine.Component.StockList`: `Engine.Component.StockList` type, reached as `api.type.StockList`.
- `LineIssue : LineIssue`: `LineIssue` type, reached as `api.type.LineIssue`.
- `TrafficLightConfig : Engine.Component.TrafficLightConfig`: `Engine.Component.TrafficLightConfig` type, reached as `api.type.TrafficLightConfig`.
- `TrafficLightState : Engine.Component.TrafficLightState`: `Engine.Component.TrafficLightState` type, reached as `api.type.TrafficLightState`; `api.type.TrafficLightState.new(...)` creates a new value.

## CargoTypeSet (record)

```teal
global record CargoTypeSet
```
Access: `api.type.CargoTypeSet`

Specification for cargo type set

- `new : function() : CargoTypeSet`  (call as `api.type.CargoTypeSet.new(…)`): Creates a new, empty `CargoTypeSet`.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3794`: `local set = api.type.CargoTypeSet.new()`
- `cargoClassesIncluded : {string}`: cargo classes included
- `cargoClassesExcluded : {string}`: cargo classes excluded
- `cargoTypesIncluded : {ResName}`: cargo types included
- `cargoTypesExcluded : {ResName}`: cargo types excluded

## SoundSet (record)

```teal
global record SoundSet
```

Configure sound effects for entities

- `tracks : {SoundClip}`: List of looping sound tracks, each a clip name with its sound attributes.
- `events : {string : SoundClipList}`: Event sounds, as a map from event key (e.g. `horn`) to a list of clips played for that event.
- `updateScript : ScriptRef`: Script that updates the tracks and events at runtime; `soundsetutil.makeSoundSet()` sets it to `::/scripts/soundset_default.script@updateSoundSet`.
    - notes: `params.updateFunctions` holds one entry per track and event, describing how gain and pitch follow scripting values such as the vehicle speed.
- `attrs : SoundAttributes`: Default sound attributes (gain, reference distance, roll-off) of the set.

## ModelMetadata (record)

```teal
global record ModelMetadata
```
Access: `api.type.MetadataMap`

Contains all metadata of a model. A specific metadata might be nil.

### ModelMetadata.Animal (record)

```teal
record Animal
```
Access: `api.type.MetadataMap.Animal`

Animal metadata of a model (`metadata.animal` in `.mdl` files): spawning configuration, movement types and the id used for achievements.

- `config : Config`: Spawning configuration.
- `movementTypes : {MovementType}`: Movement types (animation states) of the animal.
- `idForAchievement : string`: Id under which the animal counts for the spotted-animals achievement; when empty, the model's resource name is used. Variants of one species share it (e.g. `animal/cr_fish`).

#### ModelMetadata.Animal.MovementType (record)

```teal
record MovementType
```
Access: `api.type.MetadataMap.Animal.MovementType`

One movement or animation state of an animal (e.g. idle, walk), as listed in `movementTypes`.

- `description : string`: Display name of the movement type, e.g. `_("Idle")`.

#### ModelMetadata.Animal.Config (record)

```teal
record Config
```
Access: `api.type.MetadataMap.Animal.Config`

Spawning configuration of an animal.

- `density : number`: Target population density of the animal per square kilometre; multiplied by `BaseConfig.Animal.populationDensityMultiplier`.
- `fish : boolean`: Whether the animal is a fish (lives in water).

### ModelMetadata.Description (record)

```teal
record Description
```
Access: `api.type.MetadataMap.Description`

Display information of a model or construction (`metadata.description`): name, description text, icons and attributes.

- `name : string`: Display name, usually a translation key passed through `_()`.
- `description : string`: Description text, usually a translation key passed through `_()`.
- `iconSmall : string`: Small icon; the vehicle GUI uses it for vehicle parts.
- `iconSmallCblend : string`: Colour-blend mask for `iconSmall`.
- `icon : string`: Main icon image.
- `icon20 : string`: Icon 20 pixels high, used in vehicle lists; generated vehicle metadata defaults it to `icons/<vehicle>_icon20.tga`.
- `icon3d : string`: Large 3D-rendered icon; used in the vehicle store and in availability notifications.
- `icon20cblend : string`: Colour-blend mask for `icon20`.
- `lockedIcon : string`: Icon shown while the item is still locked.
- `previewIcon : string`: Preview image, e.g. shown in the construction menu.
- `attributes : {string : {number, number}}`: Displayed attributes, as a map from name to a pair of numbers, e.g. `cost = { 30000, -1 }` and `maintenanceCost = { 5000, -1 }`.
    - notes: After loading, the base game scales positive `cost` and `maintenanceCost` values by the infrastructure cost and maintenance scales (`post_run_util.updateDesc`).
- `attributes_native : Map<string, {number, number}>`: Native map view of `attributes`, with `find` and `insert`; used by `post_run_util.updateDesc` to patch cost attributes.
- `cargoTypeSet : CargoTypeSet`: Cargo the item handles, e.g. a street stop with `cargoClassesIncluded = { "PASSENGERS", "UNIVERSAL" }`.

### ModelMetadata.Category (record)

```teal
record Category
```
Access: `api.type.MetadataMap.Category`

Category tags of a model (`metadata.category`), e.g. `{ "helicopter" }`; the construction GUI uses them as filter categories.

- `categories : {string}`: List of category names.

### ModelMetadata.CategoryList (record)

```teal
record CategoryList
```
Access: `api.type.MetadataMap.CategoryList`

Category list of a model, construction or cargo (`metadata.categoryList`): tags such as climates (`temperate.clima`) or economies that decide where the item is available.

Notes:

The base game hides industries and cargo types whose list does not match the selected
economy's category, and uses climate tags to filter models by climate. Entries of the form
`cargo#format` are converted into `cargoModel` metadata.

- `categories : {string}`: List of category tags.

### ModelMetadata.MenuCategory (record)

```teal
record MenuCategory
```
Access: `api.type.MetadataMap.MenuCategory`

Places a model or construction in the construction menu (`metadata.menuCategory`).

- `categories : {CategoryWithOrder}`: Menu entries of the item.

#### ModelMetadata.MenuCategory.CategoryWithOrder (record)

```teal
record CategoryWithOrder
```
Access: `api.type.MetadataMap.MenuCategory.CategoryWithOrder`

One menu entry, with the menu category, the sort order and optional filter categories.

- `new : function() : CategoryWithOrder`  (call as `api.type.MetadataMap.MenuCategory.CategoryWithOrder.new(…)`): Creates a new, empty `CategoryWithOrder`.
    - used 1x in the base game, e.g. `base/content/base/base/mod.script.tl:431`: `local categoryWithOrder = api.type.MetadataMap.MenuCategory.CategoryWithOrder.new()`
- `category : string`: Menu category id, e.g. `landscaping_vegetation`.
- `order : integer`: Sort order within the menu category; lower values come first.
- `filterCategories : {string}`: Filter categories within the menu, e.g. `{ "plant" }`.

### ModelMetadata.Availability (record)

```teal
record Availability
```
Access: `api.type.MetadataMap.Availability`

Time span in which a model or construction can be built or bought (`metadata.availability`).

- `yearFrom : integer`: First year in which the item is available; 0 means no limit.
- `yearTo : integer`: Year from which the item is no longer available; 0 means no limit.
- `notifyWhenAvailable : boolean`: Emit notification when becoming available (default = true)
- `notificationGroup : string`: Group key for the availability notification; items with the same group are combined into one notification (e.g. `airfield`).
- `notificationSortKey : integer`: Sort key of the item within its availability notification group.

### ModelMetadata.SoundConfig (record)

```teal
record SoundConfig
```
Access: `api.type.MetadataMap.SoundConfig`

Sound configuration of a model (`metadata.soundConfig`): sound set, overridden effects and construction sound.

- `soundSet : SoundSet`: Sound set of the model.
- `effects : {string : {string}}`: Sound effects, map from key to list of file paths
used to overwrite certain clips in SoundSet (if SoundSet is set and has those)
- `builderAudioRes : ResName`: Sound resource (.builder_audio) for construction action

#### ModelMetadata.SoundConfig.SoundSet (record)

```teal
record SoundSet
```
Access: `api.type.MetadataMap.SoundConfig.SoundSet`

Reference to a sound set file.

- `name : string`: Path of the sound set (`.snd`) file, e.g. `/vehicle/train/shared/sound/train_electric_modern.snd`.

### ModelMetadata.Maintenance (record)

```teal
record Maintenance
```
Access: `api.type.MetadataMap.Maintenance`

Lifespan and running costs of a vehicle (`metadata.maintenance`).

- `lifespan : integer`: Lifespan of the vehicle; the vehicle GUI multiplies it by 1000 to compare it with game times in milliseconds. Base game vehicles mostly use 36525.
- `runningCosts : number`: Running costs of the vehicle in game currency per year; -1 lets the base game compute it from capacity, speed and power.
- `runningCostScale : number`: Factor applied to the computed running costs when `runningCosts` is -1.
- `maintenanceScale : number`: Maintenance scale factor.
- `topSpeedMaintenanceFactorPenaltyPerLevel : number`: Top speed penalty factor per maintenance level; the vehicle GUI passes it to `api.engine.util.vehicle.getMaintenanceTopSpeedPenalty`.

### ModelMetadata.Emission (record)

```teal
record Emission
```
Access: `api.type.MetadataMap.Emission`

Noise or pollution emission values of a model.

- `idle : number`: Emission while idle.
- `speed : number`: Emission component that depends on speed.
- `power : number`: Emission component that depends on engine power.
- `score : number`: Overall emission score from 0 to 100.
    - notes: For vehicles, the base game maps the score to a carrier-specific range and splits it into `idle`, `speed` and `power` when those are missing or not positive (`model_metadata_util.addEmissionMetadata`).

### ModelMetadata.Emissions (record)

```teal
record Emissions
```
Access: `api.type.MetadataMap.Emissions`

Noise and pollution emissions of a model (`metadata.emissions`).

- `noise : Emission`: Noise emission.
- `pollution : Emission`: Pollution emission.

### ModelMetadata.VehicleEngine (record)

```teal
record VehicleEngine
```
Access: `api.type.MetadataMap.VehicleEngine`

One engine of a land vehicle (an entry of `landVehicle.engines`).

- `power : number`: Engine power in kW.
- `tractiveEffort : number`: Tractive effort in kN.
- `type : VehicleEngineType`: Engine type: `HORSE`, `STEAM`, `ELECTRIC` or `DIESEL`.

### ModelMetadata.TransportVehicle (record)

```teal
record TransportVehicle
```
Access: `api.type.MetadataMap.TransportVehicle`

Transport properties of a vehicle (`metadata.transportVehicle`): carrier, compartments and cargo capacity, loading and transport modes.

- `line : Engine.Entity`
- `carrier : Carrier`: Carrier the vehicle uses: `ROAD`, `RAIL`, `TRAM`, `AIR` or `WATER`.
- `compartments : {Compartment}`: Compartments of the vehicle. Each compartment holds alternative `loadConfigs`; only one of them is used at a time.
    - notes: The base game's statistics (`vehicle_metadata_util.lua`) count the largest `capacity` among a compartment's load configs as that compartment's capacity and add up the compartments.
- `loadSpeed : number`: Loading speed of the vehicle (`transportVehicle.loadSpeed` in the `.mdl`). With `-1` the base game estimates a value from the vehicle's other stats.
- `reversible : boolean`: Whether the vehicle can reverse its direction of travel (`transportVehicle.reversible` in the `.mdl`).
- `arrivalDelay : integer`: Milliseconds between arriving at a stop and the start of loading. The wiki asks for the length of the longest door-open animation.
- `departureDelay : integer`: Milliseconds between the end of loading and departure. The wiki asks for the length of the longest door-close animation.
- `groupFileName : ResName`: Variant group the vehicle belongs to. The vehicle store groups vehicles with the same value as variants; empty when the vehicle has no variants.
- `transportModes : {TransportMode}`: Transport modes the vehicle can travel on, e.g. `{ "TRAIN", "ELECTRIC_TRAIN" }`.
    - notes: If a model leaves this out, `model_metadata_util.addTransportVehicleTransportModes` derives it from `carrier`, the engine types and, for aircraft and ships, from `isHelicopter` and the `SMALL` type.
- `engineTransportModes : {TransportMode}`: Transport modes on which the vehicle's own engines can move it. A diesel train lists `TRAIN` and `ELECTRIC_TRAIN`; an electric-only tram lists only `ELECTRIC_TRAM`.
- `filterTags : {string}`: Filter tags of the vehicle. Defaults to `{ "default" }`, or to an empty list for parts marked `multipleUnitOnly`.
- `comfortFactor : number`: Comfort ("Happiness Factor" in the model editor) of the vehicle, from 0 to 1. With `-1` the base game fills in an estimated or a per-carrier default value.
- `priceFactor : number`: Ticket price factor of the vehicle ("Ticket Price Factor" in the model editor). Set together with `comfortFactor` when that is `-1`.
    - notes: The wiki says this is currently unused.
- `maintenanceFactor : number`: Factor for the vehicle's maintenance. Defaults to `1` when the model leaves it out.
    - notes: The wiki says this is currently unused.
- `noCblendMask : boolean`: `true` when the model has no colour blend mask. Such a vehicle cannot be recoloured, and the GUI disables its colouring options.

#### ModelMetadata.TransportVehicle.CargoEntry (record)

```teal
record CargoEntry
```
Access: `api.type.MetadataMap.TransportVehicle.CargoEntry`

Cargo a load configuration carries and how much of it.

- `cargoTypeSet : CargoTypeSet`: Cargo classes and cargo types this entry accepts.
- `capacity : integer`: Number of cargo units (or passengers) this entry holds.
- `seats : {integer}`: Indices of the seats (in `seatProvider.seats`, counted from 0) used by this entry's passengers.
- `loadIndicator : string`: Used in conjunction with transport vehicle / industry metadata to define cargo positioning of custom cargo models

#### ModelMetadata.TransportVehicle.LoadConfig (record)

```teal
record LoadConfig
```
Access: `api.type.MetadataMap.TransportVehicle.LoadConfig`

One alternative load configuration of a compartment.

- `new : function() : LoadConfig`  (call as `api.type.MetadataMap.TransportVehicle.LoadConfig.new(…)`): Creates a new, empty `LoadConfig`.
- `cargoEntry : CargoEntry`: Cargo carried in this configuration.
- `toHide : {string}`: Mesh groups hidden while this configuration is used, e.g. `{ "comp1_grp", "comp2_grp" }`.

#### ModelMetadata.TransportVehicle.Compartment (record)

```teal
record Compartment
```
Access: `api.type.MetadataMap.TransportVehicle.Compartment`

A cargo compartment of the vehicle with its alternative load configurations.

- `loadConfigs : {LoadConfig}`: Alternative load configurations of the compartment.

### ModelMetadata.Cost (record)

```teal
record Cost
```
Access: `api.type.MetadataMap.Cost`

Purchase cost of a model (`metadata.cost` in the `.mdl`).

- `priceScale : number`: Factor applied to the automatically computed price (when `price` is negative). Defaults to 1.
- `price : integer`: Purchase price in game currency.
    - notes: For vehicles a negative price (base-game models use `-1`) is computed automatically: the estimated yearly maintenance cost times 6 times `priceScale`.

### ModelMetadata.LandVehicle (record)

```teal
record LandVehicle
```
Access: `api.type.MetadataMap.LandVehicle`

Physical properties of a land vehicle (train, tram or road vehicle): `metadata.landVehicle` in the `.mdl`.

Notes:

The model files use two more fields that this record lacks: `brakeDeceleration` (168 base
rail and tram models, all with 2.5) and `curveSpeedScale` (44 base models, with 1 or 1.25). Teal
code that reads or writes them has to go through a plain table, for example
`(model.metadata.landVehicle as {string:any}).brakeDeceleration`. The runtime record
`VehicleData.brakeDeceleration` is documented in m/s², which suggests the same unit for the
model field; the model files don't state one.

- `topSpeed : number`: Top speed in m/s (the base game converts it to km/h with `* 3.6`).
- `weightEmpty : number`: Empty weight in kg.
- `weightMaxPayload : number`: Maximum payload weight in kg.
- `engines : {VehicleEngine}`: Engines of the vehicle, each with `type` (`HORSE`, `STEAM`, `ELECTRIC` or `DIESEL`), `power` and `tractiveEffort`. Unpowered cars and wagons have none.

    ```lua
    engines = {
    	{ power = 630, tractiveEffort = 170, type = "DIESEL" },
    },
    ```

- `friction : number`: Rolling friction coefficient of the vehicle (base-game trains use values around `0.02`).

### ModelMetadata.WaterVehicle (record)

```teal
record WaterVehicle
```
Access: `api.type.MetadataMap.WaterVehicle`

Physical properties of a ship: `metadata.waterVehicle` in the `.mdl`.

- `topSpeed : number`: Top speed in m/s.
- `weightEmpty : number`: Empty weight in kg.
- `weightMaxPayload : number`: Maximum payload weight in kg.
- `availPower : number`: Engine power in watts. The vehicle store divides it by 1000 to show it next to land-vehicle power in kW.
- `maxRpm : number`: Maximum rotations per minute ("Max. rotation per min" in the model editor).
- `area : number`: Largest underwater cross-section of the hull in m². Drag grows linearly with it (wiki).

### ModelMetadata.AirVehicle (record)

```teal
record AirVehicle
```
Access: `api.type.MetadataMap.AirVehicle`

Physical properties of an aircraft or helicopter: `metadata.airVehicle` in the `.mdl`.

- `topSpeed : number`: Top speed in m/s.
- `weightEmpty : number`: Empty weight in kg.
- `weightMaxPayload : number`: Maximum payload weight in kg.
- `maxThrust : number`: Maximum engine thrust. The vehicle store divides it by 1000 for display.
- `isHelicopter : boolean`: Whether the aircraft is a helicopter. Helicopters get the `HELICOPTER` transport mode instead of `AIRCRAFT` or `SMALL_AIRCRAFT`.

### ModelMetadata.Person (record)

```teal
record Person
```
Access: `api.type.MetadataMap.Person`

Properties of a person model: `metadata.person` in the `.mdl`.

Notes:

The definition declares only `gender`. The wiki and all 37 base characters also set `drivingLicenses`, a list of strings such as `HELICOPTER` in `era_c_driver_helicopter.mdl`.

- `gender : Gender`: Gender of the person model (`FEMALE` or `MALE`).

### ModelMetadata.Tree (record)

```teal
record Tree
```
Access: `api.type.MetadataMap.Tree`

Properties of a tree model: `metadata.tree` in the `.mdl`.

- `reputationFactor : number`: Reputation factor of the tree.
    - notes: If missing or `-1`, `model_metadata_util.addTreeReputationFactor` computes it from the volume of the tree's `BOX` or `CYLINDER` collider divided by 2750, for trees smaller than that volume.

## CampaignDesc (record)

```teal
global record CampaignDesc
```

Campaign description

- `new : function() : CampaignDesc`: Creates a new, empty `CampaignDesc`.
- `name : string`: Display name of the campaign (usually a translated string).
- `description : string`: Description text of the campaign.
- `icon : string`: Icon file of the campaign. The `@2x` version should be 675 × 1080 pixels (wiki).
- `map : string`
- `order : number`: Sort order of the campaign.
- `voiceOver : string`: Voice-over audio file of the campaign.
- `requiresCompleted : string`
- `unlockingProgression : boolean`: If `true`, missions unlock one at a time: a mission is enabled when the one before it is completed (the wiki says one star is enough). If `false`, all missions are open.

## MissionDesc (record)

```teal
global record MissionDesc
```

Campaign mission description

- `name : string`: Display name of the mission.
- `description : string`: Description of the mission.
- `campaign : string`: Resource name of the campaign the mission belongs to, e.g. `"urbangames_campaign::/info.campaign"`.
- `characters : {Character}`: Characters introduced on the mission page.
- `order : integer`: Position of the mission within its campaign.
- `image : string`: Main image of the mission; also the loading-screen fallback when `loadscreen` is empty.
- `imagePreview : string`: Preview image shown on the mission page.
- `imageSuccess : string`: Image shown in the mission end window when the mission was won.
- `imageFailure : string`: Image shown in the mission end window when the mission was lost.
- `loadscreen : string`: Loading-screen image of the mission.
- `loadscreenDescription : string`: Text shown on the loading screen.
- `loadscreenVoiceOver : string`: Voice-over audio file played with the loading screen text.
- `savegame : string`: Savegame the mission starts from.
- `languageScript : string`: Script function (`file@function`) that returns, per language code, a map from dialogue or cutscene script name to the keyframe file for that language. Falls back to `en`. Required for dialogue tasks.
- `playlistOverride : string`: Music playlist (`.plist`) played during the mission instead of the normal playlist.
- `medals : {Medal}`: Medals that can be earned in the mission.
- `starsDescription : string`: Text explaining the mission's stars.
- `stars : {Star}`: Stars (optional achievements) of the mission.
- `voiceOver : string`: Voice-over audio file of the mission description.
- `location : string`: Location name of the mission, shown together with `year`.
- `year : string`: Year in which the mission is set, shown together with `location`.
- `musicTracks : {{ResName, boolean}}`: Music tracks played on the mission page and in the end window, as `{ fileName, repeat }` pairs.

### MissionDesc.Medal (record)

```teal
record Medal
```

Campaign mission medal information

- `id : string`: Unique id of the medal, used to check whether it was achieved.
- `name : string`: Display name of the medal once achieved.
- `nameLocked : string`: Display name of the medal while it is not yet achieved.
- `iconLocked : string`: Icon of the medal while it is not yet achieved.
- `iconCompleted : string`: Icon of the medal once achieved.

### MissionDesc.Star (record)

```teal
record Star
```

Campaign mission star (achievement) information

- `id : string`: Unique id of the star.
- `name : string`: Display name of the star.
- `description : string`: Description of the star.
- `iconLocked : string`: Icon of the star while it is not yet achieved.
- `iconCompleted : string`: Icon of the star once achieved.

### MissionDesc.Character (record)

```teal
record Character
```

Campaign mission character information

- `name : string`: Name of the character.
- `info : string`: Short description of the character, shown on the mission page.
- `portrait : string`: Portrait image of the character.

## NameDesc (record)

```teal
global record NameDesc
```

Name list description for procedural name generation

- `name : string`: Display name of the name list, e.g. `"Polish"`.
- `personNamesScript : string`: Script that generates person names.
- `townNamesScript : string`: Script that generates town names.
- `streetNamesScript : string`: Script that generates street names.

## PlaylistDesc (record)

```teal
global record PlaylistDesc
```

Music playlist description

- `name : string`: Display name of the playlist.
- `tracks : {PlaylistTrack}`: Tracks of the playlist.
- `order : integer`: Sort order in the music player's playlist selection.
- `hidden : boolean`: If `true`, the playlist is not offered in the music player (e.g. the menu playlist).
- `allFilter : boolean`

## PlaylistTrack (record)

```teal
global record PlaylistTrack
```

Music playlist track information

- `name : string`: id
- `fileName : string`: file path to the music file
- `yearFrom : integer`: Year when track becomes available
- `yearTo : integer`: Last year before track becomes unavailable

## SoundAttributes (record)

```teal
global record SoundAttributes
```

SoundClip attributes

- `gain : number`: Volume factor of the sound.
- `refDist : number`: Reference distance of the sound source for distance attenuation.
- `rolloffFact : number`: Rolloff factor for distance attenuation.
- `hasVelocity : boolean`

## SoundClip (record)

```teal
global record SoundClip
```

SoundClip for SoundSet, BuilderAudioSet, ...

- `name : string`: Sound file of the clip.
- `attrs : SoundAttributes`: Playback attributes of the clip.

## SoundClipList (record)

```teal
global record SoundClipList
```

SoundClipList for SoundSet

- `names : {string}`: Sound files of the list, e.g. the variants of a sound-set event.
- `attrs : SoundAttributes`: Playback attributes shared by all files of the list.

## BuilderAudioSet (record)

```teal
global record BuilderAudioSet
```

Sound set of a construction tool (a `.builder_audio` resource): sounds for dragging, building, bulldozing and terraforming.

Notes:

In the `.builder_audio.lua` file, clips are written as plain file names, e.g. `drag = { "build/street/drag_1.wav", ... }`. A list with several clips holds variants.

- `attrs : SoundAttributes`: Playback attributes for the set.
- `dragStart : SoundClip`: Sound played when dragging starts.
- `drag : {SoundClip}`: Sounds played while dragging.
- `dragBridge : {SoundClip}`: Sounds played while dragging a bridge section.
- `dragTunnel : {SoundClip}`: Sounds played while dragging a tunnel section.
- `dragEnd : SoundClip`: Sound played when dragging ends.
- `dragOffset : number`
- `build : {SoundClip}`: Sounds played when something is built.
- `noMoney : {SoundClip}`: Sounds played when building fails for lack of money.
- `bulldoze : {SoundClip}`: Sounds played when bulldozing.
- `terraform : {SoundClip}`: Sounds played when terraforming.

## LineCargoInfo (record)

```teal
global record LineCargoInfo
```

Information about cargo on a line

- `realSectionTimes : {{number, integer}}`: Real times for each section
- `sectionTimes : {number}`: Scheduled times for each section
- `frequency : number`: Frequency of the cargo on the line
- `driveRealSectionTimes : {{number, integer}}`: Real drive times for each section
- `driveSectionTimes : {number}`: Drive times for each section
- `numVehicles : integer`: Number of vehicles
- `totalCapacity : integer`: Total capacity
- `availableCapacities : {integer}`: Available capacities
- `comfortFactor : number`: Comfort factor for this cargo on the line
- `priceFactor : number`: Price factor for this cargo on the line

## ParcelData (record)

```teal
global record ParcelData
```

Entities attached to a segment

- `leftEntities : {Engine.Entity}`: Parcels on the left of the segment
- `rightParcels : {Engine.Entity}`: Parcels on the right of the segment

## GridCollisionSystem (record)

```teal
global record GridCollisionSystem
```

A single instance of a grid collision system

- `getCollisionDataAt : function(GridCollisionSystem, Vec2f) : integer`  (call as `gridCollisionSystem:getCollisionDataAt(…) (method)`): Gets the collision data at the specified position
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo_town/deliver_cargo_town.script.tl:277`: `local baseWeight = (gics:getCollisionDataAt(api.type.Vec2f.new(position.x, position.y)) + 1) / #missingSupply`

## TargetDataEntry (record)

```teal
global record TargetDataEntry
```

Target data for land use types

- `entity : Engine.Entity`
- `freeCapacity : integer`: Capacity of the target that is still free.
- `capacity : integer`: Total capacity of the target.
- `edgeWalk : {integer, integer}`
- `edgeDrive : {integer, integer}`
- `destinationWeight : number`
- `isReachableDrive : boolean`: Whether the target can be reached by car.
- `isReachableWalk : boolean`: Whether the target can be reached on foot.
- `isReachableLines : boolean`: Whether the target can be reached with lines.

## TargetData (record)

```teal
global record TargetData
```

Information about target destinations

- `entries : {TargetDataEntry}`: Target entries.
- `edges : {EdgePos}`

## TargetsByLandUse (record)

```teal
global record TargetsByLandUse
```

Target destinations organized by land use type

- `targets : {TargetData, TargetData}`

## NodeViewerData (record)

```teal
global record NodeViewerData
```
Access: `api.type.NodeViewerData`

Layer Types
Struct containing information for rendering transport network edges of a BaseNode

- `new : function() : NodeViewerData`  (call as `api.type.NodeViewerData.new(…)`): Creates a new, empty `NodeViewerData`.
    - used 6x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:87`: `local lockedLane = api.type.NodeViewerData.new()`
- `clone : function(NodeViewerData) : NodeViewerData`  (call as `nodeViewerData:clone(…) (method)`): Returns a copy of the object.
    - returns: A new `NodeViewerData` with the same values.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:121`: `table.insert(nodeConfigForEntity, hoverLane:clone())`
- `edgeIdx : integer`: The index inside the TransportNework edge list
- `colors : {Vec4f}`: Colors that will be smoothly interpolated from the beginning to the end of the edge
- `order : integer`: Rendering order of the edge
- `width : number`: Width of the edge

## LayerConfig (record)

```teal
global record LayerConfig
```
Access: `api.type.LayerConfig`

Describes configuration for 3d layers that can be enabled with the react interface

- `new : function() : LayerConfig`  (call as `api.type.LayerConfig.new(…)`): Creates a new, empty `LayerConfig`.
    - used 29x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:77`: `local layerConfig = api.type.LayerConfig.new()`
- `new : function(LayerConfig) : LayerConfig`  (call as `api.type.LayerConfig.new(…)`): Creates a new, empty `LayerConfig`.
    - used 29x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:77`: `local layerConfig = api.type.LayerConfig.new()`
- `colorMap : any`: Optional color map config: nil, EmissionColorMap or TerrainColorMap
variant: nil|EmissionColorMap|TerrainColorMap
- `contours : TerrainContours`: Optional countour rendering config
variant: nil|TerrainContours
- `colorPassFn : ColorPassFn`: Optional colorPassFn config
variant: nil|ColorPassFn

Whether to use colorPassFn only for buildings (default: false)
- `colorPassFnForBuildingRenderableOnly : boolean`
- `buildingRenderableConfig : BuildingRenderableConfig`: Optional preview of town buildings and parcels (see `LayerConfig.BuildingRenderableConfig`).
- `catchmentAreaRenderableConfig : CatchmentAreaRenderableConfig`: Optional rendering of catchment areas (see `LayerConfig.CatchmentAreaRenderableConfig`).
- `edgeRenderableConfig : any`: Optional edge renderable config: nil, SpeedLimitsEdgesRenderables, CargoEdgesRenderables, SatisfactionEdgeRenderables or TrafficEdgesRenderables
variant: nil|SpeedLimitsEdgesRenderables|CargoEdgesRenderables|SatisfactionEdgeRenderables|TrafficEdgesRenderables
- `voronoiRenderableConfig : VoronoiRenderableConfig`: Optional rendering of town limits (see `LayerConfig.VoronoiRenderableConfig`).
- `windRenderableConfig : WindRenderableConfig`: Optional wind display (see `LayerConfig.WindRenderableConfig`).
- `plotsRenderableConfig : PlotsRenderableConfig`: Optional rendering of industry plots (see `LayerConfig.PlotsRenderableConfig`).
- `cargoFlowRenderableConfig : CargoFlowRenderableConfig`: Optional 3D cargo flow arrows (see `LayerConfig.CargoFlowRenderableConfig`).
- `undergroundMode : boolean`: Underground mode enables rendering and snapping of underground entities
- `showOneWay : boolean`: Display one way arrows on streets
- `colorMode : boolean | nil`: Enable color mode rendering (overrides the default if set)
- `ambientScale : number | nil`: Sets the ambient scale, or default of not set (2 for color mode and 1 for non color mode)

### LayerConfig.EmissionColorMap (record)

```teal
record EmissionColorMap
```
Access: `api.type.LayerConfig.EmissionColorMap`

Colors entities and terrain based on

- `new : function() : EmissionColorMap`  (call as `api.type.LayerConfig.EmissionColorMap.new(…)`): Creates a new, empty `EmissionColorMap`.
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:70`: `local colorMap = api.type.LayerConfig.EmissionColorMap.new()`
- `new : function(EmissionColorMap) : EmissionColorMap`  (call as `api.type.LayerConfig.EmissionColorMap.new(…)`): Creates a new, empty `EmissionColorMap`.
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:70`: `local colorMap = api.type.LayerConfig.EmissionColorMap.new()`
- `colors : Colors`: Configures colors as a gradient map from the given threshold
- `currentEmissionId : Engine.Component.EmissionGrid.Type`: Choose among visualization of noise or pollution
- `visualizationType : VisualizationType`: The type of emission visualization to dsplay: average or instant

#### LayerConfig.EmissionColorMap.Colors (record)

```teal
record Colors
```
Access: `api.type.LayerConfig.EmissionColorMap.Colors`

Thresholds and colors for the emission visualization gradient

- `new : function() : Colors`  (call as `api.type.LayerConfig.EmissionColorMap.Colors.new(…)`): Creates a new, empty `Colors`.
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:25`: `local averageMapColors = api.type.LayerConfig.EmissionColorMap.Colors.new()`
- `new : function(Colors) : Colors`  (call as `api.type.LayerConfig.EmissionColorMap.Colors.new(…)`): Creates a new, empty `Colors`.
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:25`: `local averageMapColors = api.type.LayerConfig.EmissionColorMap.Colors.new()`
- `numberOfLevels : integer`: Number of discrete colour steps of the gradient (the base-game noise and pollution layers use 12).
- `colors : {Vec4f}`: Colours (RGBA) of the gradient.
- `thresholds : {number}`: Emission values the gradient is mapped to. The base-game noise layer passes the lower and upper bound in dB, e.g. `{ 63, 85 }`.

#### LayerConfig.EmissionColorMap.VisualizationType (record)

```teal
record VisualizationType
```
Access: `api.type.LayerConfig.EmissionColorMap.VisualizationType`

Type of visualization for the emission color

- `DecibelCurrent : VisualizationType`
- `DecibelAverage : VisualizationType`: Shows the averaged emission level. Used by the base-game noise and pollution layers.
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:73`: `colorMap.visualizationType = api.type.LayerConfig.EmissionColorMap.VisualizationType.DecibelAverage`
- `Combined : VisualizationType`

### LayerConfig.TerrainColorMap (record)

```teal
record TerrainColorMap
```
Access: `api.type.LayerConfig.TerrainColorMap`

Configures the terrain to be visualize terrain heightmap features like elevation and presence of navigable waters

- `new : function() : TerrainColorMap`  (call as `api.type.LayerConfig.TerrainColorMap.new(…)`): Iniztialize a new TerrainColorMap object
    - used 4x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_layers.script.tl:41`: `local colorMap = api.type.LayerConfig.TerrainColorMap.new()`
- `new : function(TerrainColorMap) : TerrainColorMap`  (call as `api.type.LayerConfig.TerrainColorMap.new(…)`): Copy constructs a TerrainColorMap
    - used 4x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_layers.script.tl:41`: `local colorMap = api.type.LayerConfig.TerrainColorMap.new()`
- `colors : Colors`: Color configuration for the TerrainColorMap

#### LayerConfig.TerrainColorMap.Colors (record)

```teal
record Colors
```
Access: `api.type.LayerConfig.TerrainColorMap.Colors`

Actual color configuration for the terrain visualization

- `new : function() : Colors`  (call as `api.type.LayerConfig.TerrainColorMap.Colors.new(…)`): Creates a new, empty `Colors`.
    - used 4x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_layers.script.tl:34`: `local mapColors = api.type.LayerConfig.TerrainColorMap.Colors.new()`
- `new : function(Colors) : Colors`  (call as `api.type.LayerConfig.TerrainColorMap.Colors.new(…)`): Creates a new, empty `Colors`.
    - used 4x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_layers.script.tl:34`: `local mapColors = api.type.LayerConfig.TerrainColorMap.Colors.new()`
- `numberOfLevels : integer`: Number of discrete steps between neighbouring colours. The base-game terrain layer uses 3; its legend shows `numberOfLevels * #colors + 1` steps.
- `colors : {Vec4f}`: Set the colors corrsponding to the thresholds, length must be equal to numerOfLevels and less than 4
- `thresholds : {number}`: Elevation thresholds (in meters above water) for the matching colors
- `water : Vec4f`: Color (as RGBA value) of non-navigable waters
- `navigableWater : Vec4f`: Color (as RGBA value) of navigable waters
- `interpolateNavigableWater : boolean`: Smoothen edges of navigable water grid with linear interpolation for more pleasant (yet imprecise) look

### LayerConfig.TerrainContours (record)

```teal
record TerrainContours
```
Access: `api.type.LayerConfig.TerrainContours`

Configuration for contour lines

- `new : function() : TerrainContours`  (call as `api.type.LayerConfig.TerrainContours.new(…)`): Creates a new, empty `TerrainContours`.
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_layers.script.tl:25`: `local terrainContours = api.type.LayerConfig.TerrainContours.new()`
- `new : function(TerrainContours) : TerrainContours`  (call as `api.type.LayerConfig.TerrainContours.new(…)`): Creates a new, empty `TerrainContours`.
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_layers.script.tl:25`: `local terrainContours = api.type.LayerConfig.TerrainContours.new()`
- `contours : {Contour}`: List of contours for  the terrain (at most 10)

#### LayerConfig.TerrainContours.Contour (record)

```teal
record Contour
```
Access: `api.type.LayerConfig.TerrainContours.Contour`

Configuration of a single contour (level line)

- `new : function() : Contour`  (call as `api.type.LayerConfig.TerrainContours.Contour.new(…)`): Creates a new, empty `Contour`.
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_layers.script.tl:18`: `local contour = api.type.LayerConfig.TerrainContours.Contour.new()`
- `new : function(Contour) : Contour`  (call as `api.type.LayerConfig.TerrainContours.Contour.new(…)`): Creates a new, empty `Contour`.
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_layers.script.tl:18`: `local contour = api.type.LayerConfig.TerrainContours.Contour.new()`
- `color : Vec4f`: color RGBA value of the contour for this level
- `level : number`: height in meters of the contour for this level
- `thickness : number`: Thicknes of the contour
- `fadeDistance : number`: how far the countour lines will be visible for

### LayerConfig.ColorPassFn (record)

```teal
record ColorPassFn
```
Access: `api.type.LayerConfig.ColorPassFn`

Configures the coloring for entities: each entity type can be configured with its own specific coloring
among a selection of available modes

- `new : function() : ColorPassFn`  (call as `api.type.LayerConfig.ColorPassFn.new(…)`): Creates a new isntance of a color pass configuration
    - used 14x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:608`: `local colorPassFn = api.type.LayerConfig.ColorPassFn.new()`
- `new : function(ColorPassFn) : ColorPassFn`  (call as `api.type.LayerConfig.ColorPassFn.new(…)`): Copy the color pass function
    - used 14x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:608`: `local colorPassFn = api.type.LayerConfig.ColorPassFn.new()`
- `terrainPainter : any`: Optional terrain painter configuration: nil or BaseColor
variant nil|BaseColor
- `waterPainter : any`: Optional water painter configuration: nil or BaseColor
variant nil|BaseColor
- `townBuildingPainter : any`: Optional town building painter configuration: nil, BaseColor, CargoColor, DestinationColor, EmissionImpactColor or LandUseColor
variant nil|BaseColor|CargoColor|DestinationColor|EmissionImpactColor|LandUseColor
- `industryPainter : any`: Optional industry painter configuration: nil, IndustryColor or DestinationColor
variant nil|IndustryColor|DestinationColor
- `emissionEmitterPainter : any`: Optional emission emitter painter configuration: nil or EmissionEmitterColor
variant nil|EmissionEmitterColor
- `stationPainter : any`: Optional station painter configuration: nil, BaseColor, HappinessColor or StationOvercrowdedColor
variant nil|BaseColor|HappinessColor|StationOvercrowdedColor
- `warehousePainter : any`: Optional warehouse painter configuration: nil or HappinessColor
variant nil|HappinessColor
- `transportNetworkPainter : any`: Optional transport network painter configuration: nil, peedLimitsColor or TrafficColor
variant nil|SpeedLimitsColor|TrafficColor
- `transportVehiclePainter : any`: Optional transport vehicle painter configuration: nil, LineVehicleColor, TrafficColor or EmissionEmitterColor
variant nil|LineVehicleColor|TrafficColor|EmissionEmitterColor|MaintainedTransportVehicleColor
- `landVehiclePainter : any`: Optional land vehicle painter configuration: nil or TrafficColor
variant nil|TrafficColor
- `simAtTerminalPainter : any`: Optional sim at terminal painter configuration: nil or HappinessColor
variant nil|HappinessColor
- `fallbackColor : FallbackColor`: Fallback color (if none of the above matches)

#### LayerConfig.ColorPassFn.BaseColor (record)

```teal
record BaseColor
```
Access: `api.type.LayerConfig.ColorPassFn.BaseColor`

Generic static constant color for this type of entity, can be applied to any entity

- `new : function(?BaseColor) : BaseColor`  (call as `api.type.LayerConfig.ColorPassFn.BaseColor.new(…)`): Creates a new instance of a basic color with default values or as a copy of an existing color
- `fromColor : function(color : Vec4f, overrideColorMap? : boolean) : BaseColor`  (call as `api.type.LayerConfig.ColorPassFn.BaseColor.fromColor(…)`): Create an instance of base color from a color
    - used 21x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station.tl:137`: `maintainedColor.color = api.type.LayerConfig.ColorPassFn.BaseColor.fromColor(color_util.toVec4(gameplayColorsRef:get().Maintenance))`
- `value : Vec4f`: Color RGBA value
- `overrideColorMap : boolean`: If this value is set, the value of a configured color map (such as emission) will be overridden

#### LayerConfig.ColorPassFn.BaseMappedColor (record)

```teal
record BaseMappedColor
```
Access: `api.type.LayerConfig.ColorPassFn.BaseMappedColor`

A color that depends on the given input values and applies a different gradient depending on the input

- `new : function() : BaseMappedColor`  (call as `api.type.LayerConfig.ColorPassFn.BaseMappedColor.new(…)`): Creates a new, empty `BaseMappedColor`.
- `new : function(BaseMappedColor) : BaseMappedColor`  (call as `api.type.LayerConfig.ColorPassFn.BaseMappedColor.new(…)`): Creates a new, empty `BaseMappedColor`.
- `new : function({number}, {Vec4f}, boolean, boolean) : BaseRangeColor`  (call as `api.type.LayerConfig.ColorPassFn.BaseMappedColor.new(…)`): Creates a new, empty `BaseMappedColor`.
- `thresholds : {number}`: Specifies values that are assigned to the specific colors
- `colors : {Vec4f}`: List of colors for the matching thresholds. Must have the same length as thresholds
- `nearest : boolean`: Interpolation applied to color: if neares the gradient will only pick exact values of the color that are specified
- `overrideColorMap : boolean`: If this value is set, the value of a configured color map (such as emission) will be overridden

#### LayerConfig.ColorPassFn.BaseRangeColor (record)

```teal
record BaseRangeColor
```
Access: `api.type.LayerConfig.ColorPassFn.BaseRangeColor`

Specify two colors that will be mixed based on the input value

- `new : function() : BaseRangeColor`  (call as `api.type.LayerConfig.ColorPassFn.BaseRangeColor.new(…)`): Constructs a new empty BaseRangeColor
    - used 23x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:593`: `cargoColor.residential = api.type.LayerConfig.ColorPassFn.BaseRangeColor.new({`
- `new : function(BaseRangeColor) : BaseRangeColor`  (call as `api.type.LayerConfig.ColorPassFn.BaseRangeColor.new(…)`): Constructs a copy of the input color range
    - used 23x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:593`: `cargoColor.residential = api.type.LayerConfig.ColorPassFn.BaseRangeColor.new({`
- `new : function(minMax : {Vec4f}, overrideColorMap? : boolean) : BaseRangeColor`  (call as `api.type.LayerConfig.ColorPassFn.BaseRangeColor.new(…)`): Constructs a BaseRangeColor given a pair of colorsx
    - used 23x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:593`: `cargoColor.residential = api.type.LayerConfig.ColorPassFn.BaseRangeColor.new({`
- `minMax : {Vec4f}`: A pair of colors among which the final color will be mixed
- `overrideColorMap : boolean`: If this value is set, the value of a configured color map (such as emission) will be overridden

#### LayerConfig.ColorPassFn.CargoColor (record)

```teal
record CargoColor
```
Access: `api.type.LayerConfig.ColorPassFn.CargoColor`

Specifies colors for town buildings for each land use type based on production rating

- `new : function() : CargoColor`  (call as `api.type.LayerConfig.ColorPassFn.CargoColor.new(…)`): Constructs a new empty CargoColor
    - used 4x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:592`: `local cargoColor = api.type.LayerConfig.ColorPassFn.CargoColor.new()`
- `clone : function(CargoColor) : CargoColor`  (call as `cargoColor:clone(…) (method)`): Constructs a copy of the input cargo color
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo_flow.tl:183`: `local cargoColor2 = cargoColor:clone()`
- `residential : BaseRangeColor`: Color that applies to commercial land use type
- `industrial : BaseRangeColor`: Color that applies to industrial land use type
- `commercial : BaseRangeColor`: Color that applies to commercial land use type
- `numBuildingLevels : integer`: Number of discrete steps the color is interpolated into (e.g. a value of 3 will result in interpolation values of 0, 0.5 and 1)
- `includePassengers : boolean`: Whether to include passengers cargo for the coloring (rating also includes line usage for inhabitants)

#### LayerConfig.ColorPassFn.DestinationColor (record)

```teal
record DestinationColor
```
Access: `api.type.LayerConfig.ColorPassFn.DestinationColor`

Specifiers colors for PersonCapacity entities based on whether any SimPerson has a destination to it, based
on the specified filtering criteria.

- `new : function() : DestinationColor`  (call as `api.type.LayerConfig.ColorPassFn.DestinationColor.new(…)`): Constructs an empty DestinationColor
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_passenger_flow.tl:81`: `local destinationColorFn = api.type.LayerConfig.ColorPassFn.DestinationColor.new()`
- `new : function(DestinationColor) : DestinationColor`  (call as `api.type.LayerConfig.ColorPassFn.DestinationColor.new(…)`): Copies a given destination color
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_passenger_flow.tl:81`: `local destinationColorFn = api.type.LayerConfig.ColorPassFn.DestinationColor.new()`
- `town : Engine.Entity`: Only consider TownBuildings that belong to a given Town
- `showPlayerTransport : boolean`: Do consider SimPersons moving using Line movement types
- `showPrivateTransport : boolean`: Do consider SimPersons moving using Drive or Walk movement types
- `residential : BaseRangeColor`: Select colors for people moving to residential destinations, if any filtered person satifies the criteria,
the max color will be applied, otherwise the min color will be used
- `industrial : BaseRangeColor`: Select colors for people moving to industrial destinations, if any filtered person satifies the criteria,
the max color will be applied, otherwise the min color will be used
- `commercial : BaseRangeColor`: Select colors for people moving to commercial destinations, if any filtered person satifies the criteria,
the max color will be applied, otherwise the min color will be used
- `numBuildingLevels : integer`: Number of discrete steps the color is interpolated into (e.g. a value of 3 will result in interpolation values of 0, 0.5 and 1)
No effect in this case, always set it to two

#### LayerConfig.ColorPassFn.EmissionEmitterColor (record)

```teal
record EmissionEmitterColor
```
Access: `api.type.LayerConfig.ColorPassFn.EmissionEmitterColor`

Color EmissionEmitter entities based on emission type and value

- `new : function() : EmissionEmitterColor`  (call as `api.type.LayerConfig.ColorPassFn.EmissionEmitterColor.new(…)`): Creates a new empty instance of EmissionEmitterColor
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:30`: `local emitterColor = api.type.LayerConfig.ColorPassFn.EmissionEmitterColor.new()`
- `new : function(EmissionEmitterColor) : EmissionImpactColor`  (call as `api.type.LayerConfig.ColorPassFn.EmissionEmitterColor.new(…)`): Creates a new copy of an instance of EmissionEmitterColor
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:30`: `local emitterColor = api.type.LayerConfig.ColorPassFn.EmissionEmitterColor.new()`
- `color : BaseRangeColor`: A pair of colors that will be interpolated, based on the value of the emission
- `currentEmissionId : Engine.Component.EmissionGrid.Type`: Selects the type of emission toconsider between noise and pollution
- `lowerBound : number`: Lower bound for the range, at which  min color of color will be chosen
- `upperBound : number`: Upper bound for the range, at which max color of color will be chosen
- `goodColor : BaseRangeColor`: A pair of colors that will be interpolated, based on the value of the emission, when the emission is negative
- `goodColorLowerBound : number`: Lower bound for the negative range, at which  min color of color will be chosen
- `goodColorUpperBound : number`: Upper bound for the negative range, at which max color of color will be chosen

#### LayerConfig.ColorPassFn.EmissionImpactColor (record)

```teal
record EmissionImpactColor
```
Access: `api.type.LayerConfig.ColorPassFn.EmissionImpactColor`

Color the entity based on the current level of emission the entity is experiencing

- `new : function() : EmissionImpactColor`  (call as `api.type.LayerConfig.ColorPassFn.EmissionImpactColor.new(…)`): Creates a new, empty `EmissionImpactColor`.
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:77`: `local townBuildingColor = api.type.LayerConfig.ColorPassFn.EmissionImpactColor.new()`
- `new : function(EmissionImpactColor) : EmissionImpactColor`  (call as `api.type.LayerConfig.ColorPassFn.EmissionImpactColor.new(…)`): Creates a new, empty `EmissionImpactColor`.
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:77`: `local townBuildingColor = api.type.LayerConfig.ColorPassFn.EmissionImpactColor.new()`
- `color : BaseRangeColor`: A pair of colors that maps lowerBoundDB (the min experienced emission) to uppedBoundDB (the max experienced emission)
- `currentEmissionId : Engine.Component.EmissionGrid.Type`: Selects the type of emission toconsider between noise and pollution
- `useAverage : boolean`: Choose to apply smoothed (averaged) emission isntead of instantaneous one
- `lowerBoundDB : number`: This value maps to min color in color
- `upperBoundDB : number`: This value maps to max color in color

#### LayerConfig.ColorPassFn.FallbackColor (record)

```teal
record FallbackColor
```
Access: `api.type.LayerConfig.ColorPassFn.FallbackColor`

Color chosen if no other selected type matches the entity

- `new : function() : FallbackColor`  (call as `api.type.LayerConfig.ColorPassFn.FallbackColor.new(…)`): Creates a new empty isntance FallbackColor
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_react_util.tl:247`: `local fallbackColor = api.type.LayerConfig.ColorPassFn.FallbackColor.new();`
- `new : function(FallbackColor) : FallbackColor`  (call as `api.type.LayerConfig.ColorPassFn.FallbackColor.new(…)`): Creates a copy of FallbackColor
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_react_util.tl:247`: `local fallbackColor = api.type.LayerConfig.ColorPassFn.FallbackColor.new();`
- `terrain : BaseColor`: Color applied to the terrain and terrain-like entities
- `water : BaseColor`: Color applied to water meshes
- `edge : BaseColor`: Color applied to entities with BaseEdge components
- `node : BaseColor`: Color applied to entities with BaseNode components
- `parallelStrip : BaseColor`: Color applied to entities with ParallelStrip components
- `whatever : BaseColor`: Color applied to any other entity not matching a specific type

#### LayerConfig.ColorPassFn.IndustryColor (record)

```teal
record IndustryColor
```
Access: `api.type.LayerConfig.ColorPassFn.IndustryColor`

Apply a color to Industry entities

- `new : function() : IndustryColor`  (call as `api.type.LayerConfig.ColorPassFn.IndustryColor.new(…)`): Creates a new empty IndustryColor
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_civilization.tl:40`: `local industryColor = api.type.LayerConfig.ColorPassFn.IndustryColor.new()`
- `new : function(IndustryColor) : IndustryColor`  (call as `api.type.LayerConfig.ColorPassFn.IndustryColor.new(…)`): Creates a copy of an existing IndustryColor
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_civilization.tl:40`: `local industryColor = api.type.LayerConfig.ColorPassFn.IndustryColor.new()`
- `industry : BaseColor`: The color to apply
- `onlyWithPersonCapacity : boolean`: Only apply the color if the entity also has an industrial PersonCapacity color

#### LayerConfig.ColorPassFn.LandUseColor (record)

```teal
record LandUseColor
```
Access: `api.type.LayerConfig.ColorPassFn.LandUseColor`

Apply colors to an entity based on the level of the TownBuilding, based on the LandUseType

- `new : function() : LandUseColor`  (call as `api.type.LayerConfig.ColorPassFn.LandUseColor.new(…)`): Creates a new empty LandUseColor
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_civilization.tl:14`: `local landUseColor = api.type.LayerConfig.ColorPassFn.LandUseColor.new()`
- `new : function(LandUseColor) : LandUseColor`  (call as `api.type.LayerConfig.ColorPassFn.LandUseColor.new(…)`): Creates a copy of an existing LandUseColor
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_civilization.tl:14`: `local landUseColor = api.type.LayerConfig.ColorPassFn.LandUseColor.new()`
- `residential : BaseRangeColor`: Color to be applied to residential LandUseType
- `industrial : BaseRangeColor`: Color to be applied to industrial LandUseType
- `commercial : BaseRangeColor`: Color to be applied to commercial LandUseType

#### LayerConfig.ColorPassFn.LineVehicleColor (record)

```teal
record LineVehicleColor
```
Access: `api.type.LayerConfig.ColorPassFn.LineVehicleColor`

Apply colors to vehicles based on the line color

- `new : function() : LineVehicleColor`  (call as `api.type.LayerConfig.ColorPassFn.LineVehicleColor.new(…)`): Creates a new empty LineVehicleColor
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:559`: `local transportVehiclePainter = api.type.LayerConfig.ColorPassFn.LineVehicleColor.new()`
- `new : function(LineVehicleColor) : LandUseColor`  (call as `api.type.LayerConfig.ColorPassFn.LineVehicleColor.new(…)`): Creates a copy of an existing LineVehicleColor
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:559`: `local transportVehiclePainter = api.type.LayerConfig.ColorPassFn.LineVehicleColor.new()`
- `highlightedEntities : {Engine.Entity}`: Specify specific entites to apply an highlight effect to
- `carriers : {Carrier}`: Only apply the color to the selected entities

#### LayerConfig.ColorPassFn.MaintainedTransportVehicleColor (record)

```teal
record MaintainedTransportVehicleColor
```
Access: `api.type.LayerConfig.ColorPassFn.MaintainedTransportVehicleColor`

Apply colors based on the maintenance state of a specific vehicle

- `new : function() : MaintainedTransportVehicleColor`  (call as `api.type.LayerConfig.ColorPassFn.MaintainedTransportVehicleColor.new(…)`): Creates a new empty MaintainedTransportVehicleColor
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station.tl:136`: `local maintainedColor = api.type.LayerConfig.ColorPassFn.MaintainedTransportVehicleColor.new()`
- `new : function(MaintainedTransportVehicleColor) : MaintainedTransportVehicleColor`  (call as `api.type.LayerConfig.ColorPassFn.MaintainedTransportVehicleColor.new(…)`): Creates a copy of an existing MaintainedTransportVehicleColor
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/maintenance_station/maintenance_station.tl:136`: `local maintainedColor = api.type.LayerConfig.ColorPassFn.MaintainedTransportVehicleColor.new()`
- `color : BaseColor`: The color to apply if the entity is being maintaned
- `maintenanceStationEntity : Engine.Entity`: Filter the vehicle based on the specific maintenance station it is maintained with

#### LayerConfig.ColorPassFn.StationOvercrowdedColor (record)

```teal
record StationOvercrowdedColor
```
Access: `api.type.LayerConfig.ColorPassFn.StationOvercrowdedColor`

Color stations based on the overcrowdedness of the station

- `new : function() : StationOvercrowdedColor`  (call as `api.type.LayerConfig.ColorPassFn.StationOvercrowdedColor.new(…)`): Creates a new empty StationOvercrowdedColor
- `new : function(StationOvercrowdedColor) : StationOvercrowdedColor`  (call as `api.type.LayerConfig.ColorPassFn.StationOvercrowdedColor.new(…)`): Creates a copy of an existing StationOvercrowdedColor
- `color : BaseMappedColor`: The color is chosen based on the ratio between people being considered "overflowing" and the total capacity of the station

#### LayerConfig.ColorPassFn.SpeedLimitsColor (record)

```teal
record SpeedLimitsColor
```
Access: `api.type.LayerConfig.ColorPassFn.SpeedLimitsColor`

Color transport network extes based on the max speed of the transport network edge

- `new : function() : SpeedLimitsColor`  (call as `api.type.LayerConfig.ColorPassFn.SpeedLimitsColor.new(…)`): Creates a new empty SpeedLimitsColor
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_infrastructure.tl:24`: `local speedLimitsColor = api.type.LayerConfig.ColorPassFn.SpeedLimitsColor.new()`
- `new : function(SpeedLimitsColor) : SpeedLimitsColor`  (call as `api.type.LayerConfig.ColorPassFn.SpeedLimitsColor.new(…)`): Creates a copy of an existing SpeedLimitsColor
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_infrastructure.tl:24`: `local speedLimitsColor = api.type.LayerConfig.ColorPassFn.SpeedLimitsColor.new()`
- `slowToFast : {BaseColor}`: Interolates colors in this list between the min speed and max speed, based on all available street templates for the year
- `numberOfLevels : integer`: Discretize the number of levels based on this value
- `hideNonPlayerOwned : boolean`: Hide edges that are not owned by the player
- `hideNonLocked : boolean`: Hide edges that do not have the roadDevelopmentLocked flag

#### LayerConfig.ColorPassFn.TrafficColor (record)

```teal
record TrafficColor
```
Access: `api.type.LayerConfig.ColorPassFn.TrafficColor`

Color the base edge based on the amount of traffic present on it

- `new : function() : TrafficColor`  (call as `api.type.LayerConfig.ColorPassFn.TrafficColor.new(…)`): Creates a new empty TrafficColor
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_traffic.tl:18`: `local trafficColor = api.type.LayerConfig.ColorPassFn.TrafficColor.new()`
- `new : function(TrafficColor) : TrafficColor`  (call as `api.type.LayerConfig.ColorPassFn.TrafficColor.new(…)`): Creates a copy of an existing TrafficColor
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_traffic.tl:18`: `local trafficColor = api.type.LayerConfig.ColorPassFn.TrafficColor.new()`
- `poorToGood : {BaseColor}`: Interpolates colors in this list (must have three elements) based on the traffic rating of the edge
- `numberOfLevels : integer`: Discretize the number of levels based on this value
- `discrete : boolean`: Unused

#### LayerConfig.ColorPassFn.HappinessColor (record)

```teal
record HappinessColor
```
Access: `api.type.LayerConfig.ColorPassFn.HappinessColor`

Apply color to entities (station, terminal and warehouse) based on the average happines of their users

- `new : function() : HappinessColor`  (call as `api.type.LayerConfig.ColorPassFn.HappinessColor.new(…)`): Creates a new empty HappinessColor
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo.tl:169`: `local happinessColorFn = api.type.LayerConfig.ColorPassFn.HappinessColor.new()`
- `new : function(HappinessColor) : HappinessColor`  (call as `api.type.LayerConfig.ColorPassFn.HappinessColor.new(…)`): Creates a copy of an existing HappinessColor
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo.tl:169`: `local happinessColorFn = api.type.LayerConfig.ColorPassFn.HappinessColor.new()`
- `normalToPoor : {BaseColor}`: Interpolates colors in this list (must have three elements) based on the average happines of the entity

### LayerConfig.BuildingRenderableConfig (record)

```teal
record BuildingRenderableConfig
```
Access: `api.type.LayerConfig.BuildingRenderableConfig`

Configures a preview showing town buildings and parcels

- `new : function() : BuildingRenderableConfig`  (call as `api.type.LayerConfig.BuildingRenderableConfig.new(…)`): Creates a new empty BuildingRenerableConfig
    - used 10x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:618`: `local buildingRenderableConfig = api.type.LayerConfig.BuildingRenderableConfig.new()`
- `new : function(BuildingRenderableConfig) : BuildingRenderableConfig`  (call as `api.type.LayerConfig.BuildingRenderableConfig.new(…)`): Creates a copy of BuildingRenderableConfig object
    - used 10x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:618`: `local buildingRenderableConfig = api.type.LayerConfig.BuildingRenderableConfig.new()`
- `isVisible : boolean`: Actually render this preview
- `includeTownBuildings : boolean`: Apply the rendering to town building parcel faces
- `includeTownBuildingModels : boolean`: Apply the rendering to the actual model instances of the town building
- `includeTownBuildingMainModel : boolean`: Apply the rendering to the main model instance of the town building only, without set dressing
- `includeIndustries : boolean`: Apply the rendering to entities with Industry component
- `includeWarehouses : boolean`: Apply the rendering to entities with Warehouse component
- `includeStations : boolean`: Apply the rendering to entities with Station component
- `includeEdges : boolean`: Apply the rendering to entities with BaseEdge component

### LayerConfig.CatchmentAreaDisplaySettings (record)

```teal
record CatchmentAreaDisplaySettings
```
Access: `api.type.LayerConfig.CatchmentAreaDisplaySettings`

Configures the rendering of catchment area of entities

- `new : function() : CatchmentAreaDisplaySettings`  (call as `api.type.LayerConfig.CatchmentAreaDisplaySettings.new(…)`): Creates a new empty CatchmentAreaDisplaySettings
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_infrastructure.tl:47`: `local catchmentAreaDisplaySettings = api.type.LayerConfig.CatchmentAreaDisplaySettings.new()`
- `new : function(CatchmentAreaDisplaySettings) : CatchmentAreaDisplaySettings`  (call as `api.type.LayerConfig.CatchmentAreaDisplaySettings.new(…)`): Creates a copy of CatchmentAreaDisplaySettings object
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_infrastructure.tl:47`: `local catchmentAreaDisplaySettings = api.type.LayerConfig.CatchmentAreaDisplaySettings.new()`
- `personBaseColor : Vec3f`: Color for catchment areas that apply to pasengers
- `cargoBaseColor : Vec3f`: Color for catchment areas that apply to non-passenger cargo
- `inactiveColor : Vec3f`: Color for catchment areas that are not affected by current selections
- `unreachableColor : Vec3f`: Color for catchment areas that are not reachable
- `deleteColor : Vec3f`: Color for catchment areas that are being removed
- `maintenanceColor : Vec3f`: Color for catchment areas for maintenance stations repairs
- `noiseColor : Vec3f`: Color for  radii of noise emitters
- `pollutionColor : Vec3f`: Color for  radii of pollution emitters
- `borderAlpha : number`: Transparency of the catchment area border
- `borderAlphaPassive : number`: Transparency of the catchment area border when the catchment area is not involved
- `godrayAlpha : number`: Transparency of the middle positioning beam
- `godrayAlphaPassive : number`: Transparency of the middle positioning beam when the catchment area is not involved
- `innerAlpha : number`: Transparency of the inside part of the circle
- `buildingAlpha : number`: Transparency of the town building
- `borderWidth : number`: Width of the catchment are circle's perimeter

### LayerConfig.CatchmentAreaRenderableConfig (record)

```teal
record CatchmentAreaRenderableConfig
```
Access: `api.type.LayerConfig.CatchmentAreaRenderableConfig`

Configures which catchment areas to render

- `new : function() : CatchmentAreaRenderableConfig`  (call as `api.type.LayerConfig.CatchmentAreaRenderableConfig.new(…)`): Creates a new, empty `CatchmentAreaRenderableConfig`.
    - used 12x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:78`: `local catchmentAreaRenderableConfig = api.type.LayerConfig.CatchmentAreaRenderableConfig.new()`
- `new : function(CatchmentAreaRenderableConfig) : VoronoiRenderableConfig`  (call as `api.type.LayerConfig.CatchmentAreaRenderableConfig.new(…)`): Creates a new, empty `CatchmentAreaRenderableConfig`.
    - used 12x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:78`: `local catchmentAreaRenderableConfig = api.type.LayerConfig.CatchmentAreaRenderableConfig.new()`
- `isVisible : boolean`: Activates and deactivate rendering of catchment areas
- `displaySettings : CatchmentAreaDisplaySettings`: Configures the look and feel of the areas
- `cargo : boolean`: Do display cargo-compatible areas
- `person : boolean`: Do display passenger-compatible areas
- `maintenance : boolean`: Do display maintenance station radii
- `noise : boolean`: Do display emission emitter noise radii
- `pollution : boolean`: Do display emission emitter pollution radii
- `addGodrays : boolean`: Add positioning beams in the middle of the circle
- `entity : Engine.Entity`: Only add areas to the specific entity
- `carriers : {Carrier}`: Only add areas that are affected by specific carriers

### LayerConfig.PlotsRenderableConfig (record)

```teal
record PlotsRenderableConfig
```
Access: `api.type.LayerConfig.PlotsRenderableConfig`

Configure industry plots renderable

- `new : function() : PlotsRenderableConfig`  (call as `api.type.LayerConfig.PlotsRenderableConfig.new(…)`): Creates a new, empty `PlotsRenderableConfig`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:324`: `local plotsRenderableConfig = api.type.LayerConfig.PlotsRenderableConfig.new()`
- `new : function(PlotsRenderableConfig) : PlotsRenderableConfig`  (call as `api.type.LayerConfig.PlotsRenderableConfig.new(…)`): Creates a new, empty `PlotsRenderableConfig`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:324`: `local plotsRenderableConfig = api.type.LayerConfig.PlotsRenderableConfig.new()`
- `triangles : {Vec2f}`: Triangles to draw, as a flat list of corners (three `Vec2f` per triangle). The industry window uses it to show expansion plots.

### LayerConfig.SignConfig (record)

```teal
record SignConfig
```
Access: `api.type.LayerConfig.SignConfig`

Configures the texttual display on edges

- `new : function() : SignConfig`  (call as `api.type.LayerConfig.SignConfig.new(…)`): Creates an empty SignConfig object
    - used 5x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo.tl:189`: `local signConfig = api.type.LayerConfig.SignConfig.new()`
- `new : function(SignConfig) : SignConfig`  (call as `api.type.LayerConfig.SignConfig.new(…)`): Copies a given SignConfig object
    - used 5x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo.tl:189`: `local signConfig = api.type.LayerConfig.SignConfig.new()`
- `signRenderDistance : number`: Maximal render distance for spped limits

### LayerConfig.CargoEdgesRenderables (record)

```teal
record CargoEdgesRenderables
```
Access: `api.type.LayerConfig.CargoEdgesRenderables`

Add an extra layer on top of base edges showing cargo that flowed onto this edge

- `new : function() : CargoEdgesRenderables`  (call as `api.type.LayerConfig.CargoEdgesRenderables.new(…)`): Creates a new empty object of type CargoEdgesRenderables
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo_flow.tl:195`: `local cargoConfig = api.type.LayerConfig.CargoEdgesRenderables.new()`
- `new : function(CargoEdgesRenderables) : CargoEdgesRenderables`  (call as `api.type.LayerConfig.CargoEdgesRenderables.new(…)`): Creates a copy of an object of type CargoEdgesRenderables
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo_flow.tl:195`: `local cargoConfig = api.type.LayerConfig.CargoEdgesRenderables.new()`
- `town : Engine.Entity`: Filter only cargo types flowing towards this town entitsy
- `cargoTypes : {CargoTypeId}`: Filter only cargo transported of the given type
- `showPlayerTransport : boolean`: Consider cargo transported via lines
- `showPrivateTransport : boolean`: Consider cargo transported via private transport
- `playerTransportColor : {Vec4f, Vec4f}`: Color for low and hight occupancy of transported cargo on public transport (lines)
- `privateTransportColor : {Vec4f, Vec4f}`: Color for low and hight occupancy of transported cargo on private transport (cargo)
- `numTransportLevels : integer`: Discretization levels for the cargo
- `signConfig : SignConfig`: Configures look and feel of labels

### LayerConfig.SatisfactionEdgesRenderables (record)

```teal
record SatisfactionEdgesRenderables
```
Access: `api.type.LayerConfig.SatisfactionEdgesRenderables`

Configures the rendering of cargo satisfaction on the edge

- `new : function() : SatisfactionEdgesRenderables`  (call as `api.type.LayerConfig.SatisfactionEdgesRenderables.new(…)`): Creates a new empty object of type SatisfactionEdgesRenderables
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo.tl:184`: `local edgeRenderableConfig = api.type.LayerConfig.SatisfactionEdgesRenderables.new()`
- `new : function(SatisfactionEdgesRenderables) : SatisfactionEdgesRenderables`  (call as `api.type.LayerConfig.SatisfactionEdgesRenderables.new(…)`): Creates a copy of an object of type SatisfactionEdgesRenderables
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo.tl:184`: `local edgeRenderableConfig = api.type.LayerConfig.SatisfactionEdgesRenderables.new()`
- `town : Engine.Entity`: Town entity whose cargo satisfaction is shown.
- `cargoTypes : {CargoTypeId}`: Only consider this cargo types
- `colors : {Vec4f}`: Interpotate based on satisfaction percentage of cargo transited through the edge
- `thresholds : {number}`: Values for the gradient of colors (must have same length as colors)
- `signConfig : SignConfig`: Configures look and feel of text labels

### LayerConfig.SpeedLimitsEdgesRenderables (record)

```teal
record SpeedLimitsEdgesRenderables
```
Access: `api.type.LayerConfig.SpeedLimitsEdgesRenderables`

Configures the rendering of the speed limits on edges

- `new : function() : SpeedLimitsEdgesRenderables`  (call as `api.type.LayerConfig.SpeedLimitsEdgesRenderables.new(…)`): Creates a new empty object of type SpeedLimitsEdgesRenderables
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_infrastructure.tl:36`: `local edgeRenderableConfig = api.type.LayerConfig.SpeedLimitsEdgesRenderables.new()`
- `new : function(SpeedLimitsEdgesRenderables) : SpeedLimitsEdgesRenderables`  (call as `api.type.LayerConfig.SpeedLimitsEdgesRenderables.new(…)`): Creates a copy of an object of type SpeedLimitsEdgesRenderables
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_infrastructure.tl:36`: `local edgeRenderableConfig = api.type.LayerConfig.SpeedLimitsEdgesRenderables.new()`
- `speedColor : {Vec4f, Vec4f, Vec4f}`: Intepolate between these colors based on the speed of the edges
- `undergroundSpeedColor : {Vec4f, Vec4f, Vec4f}`: Intepolate between these colors based on the speed of the edges (applied to tunnels)
- `numberOfLevels : integer`: Number of discretizaton levels of speedColor
- `tunnelOutlineColor : Vec4f`: Color of the outline on tunnels
- `tunnelOutlineSize : number`: Width of the outline on tunnels
- `signConfig : SignConfig`: Configures look and feel of text labels
- `hideNonPlayerOwned : boolean`: Hide edges that are not owned by the player
- `hideNonLocked : boolean`: Hide edges that do not have the roadDevelopmentLocked flag

### LayerConfig.TrafficEdgesRenderables (record)

```teal
record TrafficEdgesRenderables
```
Access: `api.type.LayerConfig.TrafficEdgesRenderables`

Configures the rendering of traffic in edges

- `new : function() : TrafficEdgesRenderables`  (call as `api.type.LayerConfig.TrafficEdgesRenderables.new(…)`): Creates a new empty object of type TrafficEdgesRenderables
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_traffic.tl:27`: `local edgeRenderableConfig = api.type.LayerConfig.TrafficEdgesRenderables.new()`
- `new : function(TrafficEdgesRenderables) : TrafficEdgesRenderables`  (call as `api.type.LayerConfig.TrafficEdgesRenderables.new(…)`): Creates a copy of an object of type TrafficEdgesRenderables
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_traffic.tl:27`: `local edgeRenderableConfig = api.type.LayerConfig.TrafficEdgesRenderables.new()`
- `trafficColor : {Vec4f, Vec4f, Vec4f}`: Interpolate bewteen these colors based on the amount of traffic on the edges
- `undergroundTrafficColor : {Vec4f, Vec4f, Vec4f}`: Interpolate bewteen these colors based on the amount of traffic on the edges  (applied to tunnels)
- `numberOfLevels : integer`: Number of discretizaton levels of speedColor
- `discrete : boolean`: Unused
- `tunnelOutlineColor : Vec4f`: Color of the outline on tunnels
- `tunnelOutlineSize : number`: Width of the outline on tunnels

### LayerConfig.VoronoiRenderableConfig (record)

```teal
record VoronoiRenderableConfig
```
Access: `api.type.LayerConfig.VoronoiRenderableConfig`

Configure the rendering of town limits (based on closest distance to towns)

- `new : function() : VoronoiRenderableConfig`  (call as `api.type.LayerConfig.VoronoiRenderableConfig.new(…)`): Creates a new empty object of type VoronoiRenderableConfig
    - used 7x in the base game, e.g. `base/content/gui/gui/entity_window/perk/perk.tl:139`: `local voronoiRenderableConfig = api.type.LayerConfig.VoronoiRenderableConfig.new()`
- `new : function(VoronoiRenderableConfig) : VoronoiRenderableConfig`  (call as `api.type.LayerConfig.VoronoiRenderableConfig.new(…)`): Creates a copy of an object of type VoronoiRenderableConfig
    - used 7x in the base game, e.g. `base/content/gui/gui/entity_window/perk/perk.tl:139`: `local voronoiRenderableConfig = api.type.LayerConfig.VoronoiRenderableConfig.new()`
- `showTowns : boolean`: Only consider towns of interest
- `townsOfInterest : {Engine.Entity}`: Optional towns of interest: If set AND `showTowns` is true, only those townns are considered
- `showMapBorder : boolean`: Display also borders at the edge of the map
- `lineWidth : number`: Width of the line
- `colors : {Vec4f}`: Colors of the line (alternatings)
- `sectionsPerColor : integer`: Number of section for each color
- `fadingMinHeight : number`: Height at which rendering is no longer visible
- `fadingDeltaHeight : number`: Height at which rendering stops is fadingMinHeight+fadingDeltaHeight

### LayerConfig.WindRenderableConfig (record)

```teal
record WindRenderableConfig
```
Access: `api.type.LayerConfig.WindRenderableConfig`

Configure wind display and its impact on diverse emisson types

- `new : function() : WindRenderableConfig`  (call as `api.type.LayerConfig.WindRenderableConfig.new(…)`): Creates a new empty object of type WindRenderableConfig
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_pollution.tl:69`: `local windRenderableConfig = api.type.LayerConfig.WindRenderableConfig.new()`
- `new : function(WindRenderableConfig) : WindRenderableConfig`  (call as `api.type.LayerConfig.WindRenderableConfig.new(…)`): Creates a copy of an object of type WindRenderableConfig
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_pollution.tl:69`: `local windRenderableConfig = api.type.LayerConfig.WindRenderableConfig.new()`
- `isVisible : boolean`: Enables or disable rendering of the visualization
- `emissionType : Engine.Component.EmissionGrid.Type`: Grid to use to chose the wind direction (usually applies only to pollution)

### LayerConfig.CargoFlowRenderableConfig (record)

```teal
record CargoFlowRenderableConfig
```
Access: `api.type.LayerConfig.CargoFlowRenderableConfig`

Configure 3d cargo flow arrows

- `new : function() : CargoFlowRenderableConfig`  (call as `api.type.LayerConfig.CargoFlowRenderableConfig.new(…)`): Creates a new empty object of type CargoFlowRenderableConfig
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:441`: `local cargoFlowRenderableConfig = api.type.LayerConfig.CargoFlowRenderableConfig.new()`
- `new : function(CargoFlowRenderableConfig) : CargoFlowRenderableConfig`  (call as `api.type.LayerConfig.CargoFlowRenderableConfig.new(…)`): Creates a copy of an object of type CargoFlowRenderableConfig
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:441`: `local cargoFlowRenderableConfig = api.type.LayerConfig.CargoFlowRenderableConfig.new()`
- `flowData : {FlowData}`: Arrows to draw.

#### LayerConfig.CargoFlowRenderableConfig.FlowData (record)

```teal
record FlowData
```
Access: `api.type.LayerConfig.CargoFlowRenderableConfig.FlowData`

One 3D cargo flow arrow from `from` to `to`.

Example:
```lua
local fd = api.type.LayerConfig.CargoFlowRenderableConfig.FlowData.new()
fd.from = entity_util.getPosition(fromEntity)
fd.to = entity_util.getPosition(toEntity)
fd.speed = 40.0
fd.arrowColors = colors
fd.cargoIconAlpha = 0.6
fd.cargoTypes = { api.res.cargoTypeRep.getPassengerCargoTypeId() }
```

- `new : function() : FlowData`  (call as `api.type.LayerConfig.CargoFlowRenderableConfig.FlowData.new(…)`): Creates a new, empty `FlowData`.
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:431`: `local newData = api.type.LayerConfig.CargoFlowRenderableConfig.FlowData.new()`
- `new : function(FlowData) : FlowData`  (call as `api.type.LayerConfig.CargoFlowRenderableConfig.FlowData.new(…)`): Creates a new, empty `FlowData`.
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:431`: `local newData = api.type.LayerConfig.CargoFlowRenderableConfig.FlowData.new()`
- `from : Vec3f`: Start position of the arrow.
- `to : Vec3f`: End position of the arrow.
- `speed : number`: Animation speed of the arrow. The base game uses `40.0` for active flows and `0.0` for potential ones.
- `arrowColors : {Vec4f}`: Colours (RGBA) of the arrow.
- `cargoIconAlpha : number`: Opacity of the cargo icons on the arrow, from 0 to 1.
- `cargoTypes : {CargoTypeId}`: Cargo types whose icons are shown on the arrow.

## BoundingInfo (record)

```teal
global record BoundingInfo
```

Information about the axis aligned bounding box of the model

- `new : function() : BoundingInfo`: Creates a new empty bounding info
- `new : function(bbMin : Vec3f, bbMax : Vec3f) : BoundingInfo`: Creates a new bounding info with specified bounds
    - param `bbMin`: Corner with lowest coordinates
    - param `bbMax`: Corner with highest coordinates
- `bbMax : Vec3f`: Corner with lowest coordinates
- `bbMin : Vec3f`: Corner with highest coordinates

## Extent (record)

```teal
global record Extent
```

Information about the game-logic related bounds, usually for vehicles (bbMax.x being the vehicle's front)

- `new : function() : Extent`: Creates a new extent with zero bounds
- `new : function(bbMin : Vec3f, bbMax : Vec3f) : Extent`: Creates a new bounding info with specified bounds
    - param `bbMin`: Corner with lowest coordinates
    - param `bbMax`: Corner with highest coordinates
- `bbMax : Vec3f`: Corner with lowest coordinates
- `bbMin : Vec3f`: Corner with highest coordinates

## Collider (record)

```teal
global record Collider
```
Access: `api.type.Collider`

Information about the collider of the model

Forward declaration of the `Collider` type, which is defined in `api/type.d.tl`. Used by `Engine.Component.ColliderList`.

- `transf : Mat4f`: The transform of the collider, relative to which the other data is transformed
- `type : Type`: The type of collider
- `box : Box`: Only if type == 1, a collider with a cuboidal shape
- `cylinder : Cylinder`: Only if type == 2, a collider with a cylindrical shape
- `pointCloud : PointCloud`: Only if type == 3, a collider made up of a set of points

### Collider.Type (record)

```teal
record Type
```
Access: `api.type.Collider.Type`

Shape type of a model collider.

- `NONE : Type`: No collider.
- `BOX : Type`: Box collider defined by `halfExtents`.
- `CYLINDER : Type`: Cylinder collider.
- `POINT_CLOUD : Type`: Collider defined by a point cloud.

### Collider.Box (record)

```teal
record Box
```
Access: `api.type.Collider.Box`

A rectangular parallelepiped used for colliders

- `halfExtents : Vec3f`: Distance from transform position to define the cuboid (in the direction relative to the transform)

### Collider.Cylinder (record)

```teal
record Cylinder
```
Access: `api.type.Collider.Cylinder`

A cylinder used for colliders

- `halfExtents : Vec3f`: The dimensions from the center the cylinder extends to. Before being transformed.

### Collider.PointCloud (record)

```teal
record PointCloud
```
Access: `api.type.Collider.PointCloud`

A set of points used for collision

- `points : {Vec3f}`: A list of points specifying all collision points

## Model (record)

```teal
global record Model
```

Information about a single model file: metadata and bounding information

- `metadata : table`: Additional information about the model
- `boundingInfo : BoundingInfo`: Bounding box for the model
- `collider : table`: Collider information
- `modelPath : ResName`: Optional model path that is used when adding a new model from mod.script.tl

## AutoGroundTex (record)

```teal
global record AutoGroundTex
```

Specification for auto-generated ground textures

- `new : function() : AutoGroundTex`: Creates a new, empty `AutoGroundTex`.
- `category : string`: Purpose of the rule: `"auto.model.tree"` for trees, `"auto.model.rock"` for rocks. Models join it through `metadata.autoGroundTex.categories`.
- `gridSize : integer`: Size of the grid cells in which matching models are counted.
- `countFrom : integer`: Minimum number of matching models for the ground texture to be applied.
- `countTo : integer`: Maximum number of matching models for the ground texture to be applied; `-1` for no upper limit.
- `groundTex : string`: Ground texture (`.gtex`) to apply, e.g. `"forest_floor.gtex"`.
- `individual : boolean`
- `individualCategory : string`

## BridgeType (record)

```teal
global record BridgeType
```
Access: `api.type.BridgeType`

Specification for bridge types and also tunnels

- `description : ModelMetadata.Description`: Localized strings and icons paths for user interface
- `availability : ModelMetadata.Availability`: Era information for user interface
- `menuCategory : ModelMetadata.MenuCategory`: Configure category, order and filter behaviour in the construction menu
- `cost : number`: Cost per unit length
    - notes: The wiki says the cost is multiplied by the area under the bridge.
- `speedLimit : number`: Top speed over the section type, limits top speed of road and tracks
- `new : function() : BridgeType`  (call as `api.type.BridgeType.new(…)`): Creates a new empty bridge type
- `costFactors : Vec3f`: Cost for extensions to bridges: thresold, exponent, offset within a curve
- `maintenanceCost : integer`: Maintenance cost per unit length
- `carriers : {Carrier}`: Carriers allowed on the bridge/tunnel
- `padding : number`: Extra width added to the side of the bridge (in meters)
    - notes: For tunnels, the wiki describes this as the distance from the outer lanes to the wall.
- `height : number`: Height (tunnel only): results in offset applied to the terrain alignment above the bridge
- `sidewalkHeight : number`: Overrides the sidewalk height for the bridge
- `pillarWidth : number`: Width (in meters) of bridge pillars
- `pillarLen : number`: Length (in meters) of bridge pillars
- `pillarMinDist : number`: Minimal distance (in meters) between bridge pillars along the length of the bridge
- `pillarMaxDist : number`: Maximal allowed (in meters) distance between pillars along the length of the bridge
- `pillarTargetDist : number`: Targeted distance (in meters) between pillars, as allowed by the street segment
- `abutmentLen : number`: Length of abutments along bridges (in meters)
- `abutmentWidth : number`: Wdith of abutments along bridges (in meters)
- `materialsToReplace : table`: Override street materials by using the given configurations
- `assetsToReplace : table`: Overrides street assets by using the given list
- `pillarGroundTexture : string`: Ground textures applied to the base of bridge pillars
- `pillarGroundTextureOffset : number`: Extra padding applied to the ground texture shape below bridges
- `updateScript : ScriptRef`: A script precisely controlling placement of railing models, pillars and entrances
- `isAutoSelectable : boolean`: Allow to be used for auto bridge selection while building roads and tracks, nil defaults to true
- `portals : table`: Deprecated

## GenericGameRes (record)

```teal
global record GenericGameRes
```
Access: `api.type.GenericGameRes`

Describes a generic game resource

- `new : function() : GenericGameRes`  (call as `api.type.GenericGameRes.new(…)`): Creates a new empty resource
- `type : string`: Each generic resource has a type to differentiate its uses and enable querying by type
- `data : table`: Any serializable data might be stored in a generic resource
- `setData : function(res : GenericGameRes, data : table)`  (call as `api.type.GenericGameRes.setData(…)`): Utility function to quickly set a data table

## GenericGameResT (record)

```teal
global record GenericGameResT<T>
```

Describes a generic game resource

- `new : function() : GenericGameResT<T>`: Creates a new empty resource
- `type : string`: Each generic resource has a type to differentiate its uses and enable querying by type
- `data : T`: Any serializable data might be stored in a generic resource
- `setData : function(res : GenericGameResT<T>, data : T)`: Utility function to quickly set a data table

## CargoCategory (record)

```teal
global record CargoCategory
```

Represent a group of cargo types that are transported with similar packaging/containers: e.g. liquid, bulk, ...

- `tier : integer`: Tier level of the cargo category
- `landUses : {LandUseType}`: Land use types associated with this category
- `requiredIndustryTags : {string}`: Industry tags required for this cargo category
- `townConsumptionFactor : number`: Consumption factor of the category in towns (the base game uses `0.5` or `1`).

## ClimateDesc (record)

```teal
global record ClimateDesc
```

Contains all data relevant for the climate

- `new : function() : ClimateDesc`: Creates a new, empty climate description
- `id : string`: Unique identifier for te climate
- `desc : ModelMetadata.Description`: Localized strings and icons paths for user interface
- `category : string`: Category for content selection: only objects with matching categories in categoryList will be availalbe in a
game
- `order : integer`: Order for UI sorting
- `visible : boolean`: Configure visibility of clima zones in the selection screen
- `mapColoring : PreviewColoring`: Configures minimap preview in colors and look and feel
- `economy : string`: Default economy for the climate zone
- `environment : string`: Default environment for the cliamte zone
- `defaultNames : string`: Default name lists for the cliate zone
- `dataMaps : DataMap`: List of data map that describes biomes and layers for the climate

### ClimateDesc.TerrainData (record)

```teal
record TerrainData
```

The definition of terrain data for the climate

- `key : string`: Key of the biome or layer, e.g. `"biome0"`.
- `displayName : string`: Display name of the biome or layer.
- `color : Vec3f`: Colour (RGB) of the biome or layer.
- `id : integer`: Numeric id of the biome or layer.

### ClimateDesc.DataMap (record)

```teal
record DataMap
```

Describes data associated with this climate (eg biomes or rivers maps)

- `biomes : {TerrainData}`: Biomes definition, biomes are exclusive to eachother
- `layers : {TerrainData}`: Layers definition, layers can overlap with eachother and overlap biomes

## EconomyDesc (record)

```teal
global record EconomyDesc
```

Contains all data relevant for the economy.

- `new : function() : EconomyDesc`: Creates a new, empty economy description
- `desc : ModelMetadata.Description`: Localized strings and icons paths for user interface
- `order : integer`: User interface order for sorting
- `visible : boolean`: Selects the availability of the economy on the selection screen
- `category : string`: Used for selecting content based on the economy, depending on whether the object has matching tags in categoryList categories
- `industryWeightsSum2Population : number`
- `placementParams : {string : PlacementParams}`: Industry placement parameters, keyed by industry name (e.g. `brewery`).
- `cargoCategories : {string : CargoCategory}`: Cargo categories of the economy, keyed by category name (e.g. `beverages`).
- `townLevels2CargoInfos : {integer : {TownCargoLevelInfo}}`: Cargo demands a town gets as it grows, keyed by town level.
    - notes: For every level up to its current one, the town growth script makes sure the town has a cargo demand for each `TownCargoLevelInfo` entry, picking a cargo with a tier between `minTier` and `maxTier`.
- `cargoRequiredIndustries : table`: Per cargo type name, a table of industry names with a factor each: the industries in the production chain of that cargo.
- `companyRankRequiredIndustries : table`: Per company rank, a table of industry names with a factor each.
- `defaultIndustryDensity : number`
- `defaultPopulationDensity : number`
- `defaultIndustryProductivity : number`: Default industry productivity of the economy (`0.58` in the base-game temperate economy).

### EconomyDesc.TownCargoLevelInfo (record)

```teal
record TownCargoLevelInfo
```

Contains all data relevant for adding new cargo requirements to a town at a specific level

- `new : function() : TownCargoLevelInfo`: Creates a new, empty `TownCargoLevelInfo`.
- `new : function(copy : TownCargoLevelInfo) : TownCargoLevelInfo`: Creates a new, empty `TownCargoLevelInfo`.
- `minTier : integer`: Lowest cargo category tier the new cargo demand may have.
- `maxTier : integer`: Highest cargo category tier the new cargo demand may have.
- `landUseType : LandUseType`: Land use type (`COMMERCIAL` or `INDUSTRIAL` in the base game) that gets the new cargo demand.

## TerrainGeneratorDesc (record)

```teal
global record TerrainGeneratorDesc
```

Contains all data relevant for the heightmap generation

- `new : function() : TerrainGeneratorDesc`: Creates a new, empty `TerrainGeneratorDesc`.
- `desc : ModelMetadata.Description`: Localized strings and icons paths for user interface
- `order : integer`: Sort position in the terrain generator dropdown, which appears in the map editor and, unless `editorOnly` is set, in the main menu.
- `editorOnly : boolean`: Only selectable in editor
- `isImportGenerator : boolean`: Is a generator used when importing biomes/heightmap
- `visible : boolean`: Invisible (will never be a choice)
- `climate : string`: Climate resource name to which the generator belongs to
- `nodeTree : string`: Resource name for the tree generator associated to this terrain generator
- `params : {ScriptParam}`: Script parameters (passed as input to tree node generator)

## BuildingType (record)

```teal
global record BuildingType
```

Specification for town building types

- `constructionFile : string`: Resource name of the construction

Resource name of the town building's construction.
- `params : TownBuildingParams`: Town building parameters: land use type, level and parcel size.

## CargoClass (record)

```teal
global record CargoClass
```

Specification for cargo types

- `new : function() : CargoClass`: Creates a new, empty `CargoClass`.
- `new : function(copy : CargoClass) : CargoClass`: Creates a new, empty `CargoClass`.
- `tag : string`: unique tag
- `name : string`: display name
- `description : string`: detailed info
- `icon : string`: display icon path
- `color : Vec3f`: color of the cargo class
- `order : integer`: display ordering

## CargoType (record)

```teal
global record CargoType
```

Specification for cargo types

- `new : function() : CargoType`: Creates a new, empty `CargoType`.
- `new : function(copy : CargoType) : CargoType`: Creates a new, empty `CargoType`.
- `name : string`: Display name
- `icon : string`: Display icon path
- `color : Vec3f`: Color of the cargo
- `scriptRef : ScriptRef`: Reference to cargo income factor script
- `weight : number`: Weight of a unit of cargo
- `cargoClasses : {string}`: Cargo classes of the cargo type, e.g. `{ "UNIVERSAL", "GOODS" }`. Vehicles select cargo by class through `CargoTypeSet`.
- `order : integer`: Order for sorting in cargo type lists
- `category : string`: Category of the cargo
- `categoryList : ModelMetadata.CategoryList`: Categories in which the cargo type is available, e.g. economies like `"temperate.eco"`.
- `timeToDeliverInSeconds : integer`: Lifetime of cargo in seconds
- `loadSpeedFactor : number`: Factor on loading speed for this cargo type (`1` for passengers, `0.0625` for the base-game cargo types).

## EdgeDecoration (record)

```teal
global record EdgeDecoration
```

An addon to the sides (or the center) of BaseEdge like noise barrier and alleys

- `new : function() : EdgeDecoration`: Creates a new, empty `EdgeDecoration`.
- `new : function(copy : EdgeDecoration) : EdgeDecoration`: Creates a new, empty `EdgeDecoration`.
- `availability : ModelMetadata.Availability`: Era information for user interface
- `desc : ModelMetadata.Description`: Localized strings and icons paths for user interface
- `cost : ModelMetadata.Cost`: Cost per unit of length
- `maintenance : number`: Maintenance cost per unit of length
- `categoryList : ModelMetadata.CategoryList`: Filtering tags for incompatibility between edge decorations and clima zone selection
- `requiredTags : {string}`: An edge decoration will only be able to be built if the union of the base edge tags and all edge decoration category list are
containing all required tags. Base edges have special tags denoting whether they are "track" or "street" edges and if they are "bridge" and "tunnel edges"
- `incompatibleTags : {string}`: An edge decoration will only be able to be built if the union of the base edge tags and all edge decoration category list are
not containing any incompatible tags. Any edge with one or more decoration has the additional built-in tag "decorated"
- `repModel : string`: Model name that will be repeated along the decoration
- `length : number`: Length of the model
- `stretch : boolean`: Do stretch the model to perfectly fill in thegaps
- `verticalAlignment : integer`: Align models vertically or based on the slope of the edge: 0 = keep models vertical, 1 = keep model vertical but apply vertical shear to fill the
gaps and align with stree slop, 2 = rotate the model to be normal to the street
    - notes: Typed as an integer, but the base files write strings (`"Side"`, `"Shear"`).
- `snapMode : integer`: Snapping mode for the decoration: 0 = at the centerline of the transport network, 1 = at the side of the transport network
    - notes: Typed as an integer, but the base files write strings (`"Side"`, `"Shear"`).
- `offset : number`: Offset (orthogonal to the direction of the edge (in meters)) from the center line
- `skipSameInParallelStrip : boolean`: If a decoration is applied on the matching side of the same parallel strip, it will not be shown
- `order : integer`: Order for user interface menus
- `modifiers : Modifiers`: Modifers that are applied to vehicles travelling onto edges with such edge decoration
- `menuCategory : ModelMetadata.MenuCategory`: Configure category, order and filter behaviour in the construction menu
- `builderAudioRes : ResName`: Sound resource (.builder_audio) for construction action

## Environment (record)

```teal
global record Environment
```

Specification for environment

- `new : function() : Environment`: Creates a new, empty `Environment`.
- `atmosphere : AthmostphereParams`: Atmosphere parameters of the environment.
- `name : string`
- `isToolEnvironment : boolean`

### Environment.AthmostphereParams (record)

```teal
record AthmostphereParams
```

Specification for atmosphere parameters

- `new : function() : AthmostphereParams`: Creates a new, empty `AthmostphereParams`.
- `cloudCoverage : number`: Cloud coverage from 0.0 (clear sky) to 0.5. Rain starts at about 0.45.
- `cloudBase : number`
- `cloudTop : number`
- `cloudScattering : number`
- `dayOfYear : number`: Day of the year that sets the sun position, e.g. `171`.
- `sunRotation : number`: Rotation of the sun, in radians (base files use `math.rad(...)`).

## GroundTexture (record)

```teal
global record GroundTexture
```

Specification for ground textures

- `new : function() : GroundTexture`: Creates a new, empty `GroundTexture`.
- `new : function(copy : GroundTexture) : GroundTexture`: Creates a new, empty `GroundTexture`.
- `matIndexMap : table`: Mapping of material indices
- `indices : table`: Texture indices
- `priority : integer`: Priority for texture selection
- `texSize : number`: Size of the texture

### GroundTexture.Data (record)

```teal
record Data
```

Extra information for ground textures

- `data : table`: Raw texture data
- `size : integer`: Size of the texture data

## LaneConfig (record)

```teal
global record LaneConfig
```

Specification for the lanes of street

- `speed : number`: Speed limit for the lane
- `width : number`: Width of the lane
- `height : number`: Height of the lane
- `forward : boolean`: Whether the lane is in forward direction
- `transportModes : {TransportMode : boolean}`: Transport modes allowed on this lane
- `offset : number`: Offset from center

## Modifiers (record)

```teal
global record Modifiers
```

Configure additiive modifiers for specific entities

- `new : function() : Modifiers`: Create a new Modifiers object

Creates a new, empty `Modifiers`.
- `new : function(copy : Modifiers) : Modifiers`: Create a copy of a Modifier object

Creates a new, empty `Modifiers`.
- `equals : function(Modifiers, Modifiers) : boolean`: Compare Modifier for equality

Returns `true` when both `Modifiers` values are equal.
- `noiseModifier : number`: A value of X will mean that noise produced by the entity is equal to (baseValue * (1 + X))
- `pollutionModifier : number`: A value of X will mean that pollution produced by the entity is equal to (baseValue * (1 + X))
- `maxSpeedModifier : number`: A value of X will mean that the top speed of the entity entity is equal to (baseValue * (1 + X))

## MultipleUnit (record)

```teal
global record MultipleUnit
```

Specification for multiple units

- `new : function() : MultipleUnit`: Create a new MultipleUnit object
- `new : function(copy : MultipleUnit) : MultipleUnit`: Create a copy of a MultiplUnit object
- `vehicles : {MultipleUnit.Vehicle}`: List of models of which the multiple unit is compsed of
- `name : string`: Localized display name for the unit
- `desc : string`: Localized display description for the unit
- `filterTags : {string}`: Vehicle filter tags
- `availabilityNotificationGroup : string`: Group string to combine availability notifications of related vehicles
- `availabilityNotificationSortKey : integer`: sortkey for availability notification group
- `groupFileName : string`: Used for variant goupings: multiple units belonging to the same group will be shown as variants of each other

### MultipleUnit.Vehicle (record)

```teal
record Vehicle
```

Specification for multiple units individual vehicles

- `name : string`: Vehicle resource name
- `forward : boolean`: Whether the vehicle faces forward

## RailroadCrossingType (record)

```teal
global record RailroadCrossingType
```

Specification for railroad crossings

- `description : ModelMetadata.Description`: Localized strings and icons paths for user interface
- `availability : ModelMetadata.Availability`: Era information for user interface
- `menuCategory : ModelMetadata.MenuCategory`: Configure category, order and filter behaviour in the construction menu
- `cost : number`: Cost per unit length
- `speedLimit : number`: Top speed over the section type, limits top speed of road and tracks
- `new : function() : RailroadCrossingType`: Creates a new RailroadCrossingType object
- `soundFileName : string`: Sound effect to play when the crossing is closing
- `config : {RailroadCrossingType.ConfigEntry}`: Models of the crossing, one entry per street width.
- `trafficDelay : number`: Time in milliseconds from the start of the open animation until road traffic moves again (default 2500; every base crossing sets 2500).
- `crossingMaterial : string`
- `categoryList : ModelMetadata.CategoryList`: Categories in which the crossing is available, e.g. climates like `"temperate.clima"`.
- `isAutoSelectable : boolean`: Allow to be used for auto railroad crossing selection while building roads and tracks, nil defaults to true

### RailroadCrossingType.ConfigEntry (record)

```teal
record ConfigEntry
```

Specification for railroad crossings models

- `modelLeft : string`: Model placed on the left side of the street.
- `modelRight : string`: Model placed on the right side of the street.
- `streetWidth : number`: Street width this entry is used for (base game: 12, 18, 22 and 28).

## StreetStyle (record)

```teal
global record StreetStyle
```

Specifies the look and feel of streets and tracks

- `categoryList : ModelMetadata.CategoryList`: Tags the style adds to its edges. Edge add-ons match them with `requiredTags` and `incompatibleTags` (e.g. `withSidewalk` in `town_old.street.lua`).
- `trackStraightModel : {string, string, string, string}`: Models used for stright sections of tracks

## StreetTemplate (record)

```teal
global record StreetTemplate
```

Specification for street templates which define simulation relevant quantities like lanes, costs and transport modes

- `new : function(string) : StreetTemplate`: Creates a new object of StreetTemplate type
- `roadType : RoadType`: Localized display name
- `description : ModelMetadata.Description`: Localized strings and icons paths for user interface
- `availability : ModelMetadata.Availability`: Availability from/to year for UI and simulation
- `simBuildable : boolean`: if this template can be used by town developer / industry connections at all.
- `initiallyLocked : boolean`: Initial value for the locking tool when built by the player
- `country : boolean`: Whether to consider a country or town road, this affects simulation and UI behaviour in various ways
- `laneConfigs : {LaneConfig}`: List of lanes with configuration, from left to right in the build direction
- `transportModesStreet : {TransportMode}`: Transport network types applied to vehicle lanes
- `modifiers : Modifiers`: Modifiers applied to vehicles on the road
- `cost : integer`: Cost in unit-length
- `maintenanceCost : integer`: Maintenance cost in unit-length
- `fillGroundTex : ResName`: Ground texture applied to the interior of the road
- `sidewalkFillGroundTex : ResName`: Ground texture applied under sidewalks
- `borderGroundTex : ResName`: Ground texture applied at the edges of the road
- `streetStyle : ResName`: Default street style deciding look and feel of the road/track
- `defaultEdgeDecorations : {{ResName, boolean}}`: Default edge decorations when newly built
- `defaultWithCrosswalk : boolean`: Whether to create crosswalks when the street is built
- `embankmentSlopeLow : number`: Terrain alignment embankment from below
- `embankmentSlopeHigh : number`: Terrain alignment embankment from above
- `maxSlope : number`: Max slope of the road the user can select
- `maxSlopeShape : number`: Max slope for the resulting street
- `priority : integer`: Priority for applying the street style at crossings
- `busAndTramRight : boolean`: Whether bus and tram lanes are by default on the rightmost lane
- `pedestrianWalkPenalty : number`: Pathfinding walk penalty when pedestran walk on sidewalk edges
- `speedCoeffs : {number}`: Max speed based on three different curvatures of the street
- `minCurveRadius : number`: Minimal radius of the finial proposal
- `minCurveRadiusBuild : number`: Minimal radius buildable by the player
- `trackDistance : number`: Distance between track centers
- `maxSlopeBuild : number`: Max slope when built by the player
- `slopeBuildSteps : number`: How fine grained the selection of the slope will be
- `menuCategory : ModelMetadata.MenuCategory`: Configure category, order and filter behaviour in the construction menu
- `emissions : Emissions`: Static nosie and pollution emitter for each road segment
- `catenaryAdd : ResName`: The street template to use to replace this street template when adding catenary
- `catenaryRemove : ResName`: The street template to use to replace this street template when removing catenary
- `builderAudioRes : ResName`: Sound resource (.builder_audio) for construction action

### StreetTemplate.Emissions (record)

```teal
record Emissions
```

Static noise and pollution emission of each street segment built with the template.

- `noise : number`: Noise emitted by each segment.
- `pollution : number`: Pollution emitted by each segment.
- `radius : number`: Radius of the emission.

## TrafficLightType (record)

```teal
global record TrafficLightType
```

Specification for traffic lights

- `availability : ModelMetadata.Availability`: Configures year availability of the traffic light (for AI and player building it)
- `description : ModelMetadata.Description`: Localized strings and icons paths for user interface
- `cost : number`: Cost per unit when built
- `categoryList : ModelMetadata.CategoryList`: Tags for content selection and filtering
- `menuCategory : ModelMetadata.MenuCategory`: Configure category, order and filter behaviour in the construction menu
- `tryOpposite : boolean`: Whether to build North American style on the opposide side of the intersection if possible
- `modelScript : ScriptRef`: A script controlling the actual placement of models on the intersection
- `relativePath : string`: Deprecated

## TerrainMaterialType (record)

```teal
global record TerrainMaterialType
```

Specification for terrain material types

- `new : function() : TerrainMaterialType`: Creates a new, empty `TerrainMaterialType`.
- `new : function(copy : TerrainMaterialType) : TerrainMaterialType`: Creates a new, empty `TerrainMaterialType`.
- `name : string`: Display name of the terrain material.
- `desc : string`: Description of the terrain material, shown in the material brush.
- `icon : ResName`: Icon of the material.
- `previewIcon : ResName`: Preview image of the material.
- `categories : {string}`: Material categories, e.g. `{ "grass" }`.
- `detailColorTexture : string`: Albedo (colour) detail texture.
- `detailMetalGlossAoHTexture : string`: Detail texture holding metalness, gloss, ambient occlusion and height.
- `detailNormalTexture : string`: Normal map detail texture.
- `overlayTexture : string`: Overlay texture, e.g. `"/terrain/materials/shared/tex/overlay_0.dds"`.
- `detailSize : number`: Scale of the detail texture. Most base materials use `1 / 32`; others range from `1 / 7` to `1 / 64`.
- `overlaySize : number`: Size factor of the overlay texture.
- `overlayStrength : number`: Strength of the overlay texture.
- `hScale : number`
- `order : integer`: Sort order of the material in the menu.
- `menuCategory : ModelMetadata.MenuCategory`: Category, order and filter behaviour of the material in the menu.
- `priority : integer`
- `grass : ResName`: Grass resource (`.grass`) grown on the material.
- `categoryList : ModelMetadata.CategoryList`: Categories in which the material is available, e.g. climates like `"tropical.clima"`.

## PlacementParams (record)

```teal
global record PlacementParams
```

Rules for placing one industry type on the map: `placementParams` entries of an economy (`.eco.lua`).

Notes:

The base game builds these tables in `economy/placementparamsutil.lua`.

- `requiredIndustryTags : {string}`: Industries that must exist for this industry to be placed, e.g. `{ "glass_works", "farm" }`.
- `tags : {string}`: Tags describing the industry, e.g. `INPUT_GRAIN`, `OUTPUT_COAL` or `TYPE_RAW`. Other industries refer to them in `distanceWeights`.
- `distanceWeights : {string : number}`: Preferred distance to towns (`TOWN`) and to industries with a given tag: positive values pull the industry closer, negative values push it away.
- `allowedOnLand : boolean`: Whether the industry may be placed on land.
- `allowedOnWater : boolean`: Whether the industry may be placed on water.
- `buildOrder : integer`: Order in which industry types are placed.
- `initWeight : integer`: Initial weight of the industry type in placement (base values 10 to 40).
- `tier : integer`: Tier of the industry.
- `weights : {string : number}`: Weights per biome (`biome0` to `biome4`). In the base game, negative values favour a biome and positive values avoid it.

## PreviewColoring (record)

```teal
global record PreviewColoring
```

Minimap preview colouring of a climate (`ClimateDesc.mapColoring`). The API declares no fields; base-game climates set `ambientColor` and `levels` of `{ color, height }`.

Notes:

The base climates also set `sunColor`, `treeColor`, `waterColor0` (shallow water) and `waterColor1` (deep water).

## ChartConfig (record)

```teal
global record ChartConfig
```
Access: `api.type.ChartConfig`

A structure that holds config for a chart

- `new : function() : ChartConfig`  (call as `api.type.ChartConfig.new(…)`): Creates a new, empty `ChartConfig`.
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:117`: `local config = api.type.ChartConfig.new()`
- `interval : integer`: The time in which the data gets collected.
- `count : integer`: The desired number of intervals.
- `minCount : integer`: The minimum number of intervals to display.
- `yStepScale : number`: Modify y stepSize for fine-tuning.

## ChartResult (record)

```teal
global record ChartResult
```

A structure that holds data for a chart

- `equals : function(ChartResult, ChartResult) : boolean`  (call as `chartResult:equals(…) (method)`): Returns `true` when both `ChartResult` values are equal.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:272`: `return a:equals(b)`
- `xRange : {number}`: X-axis range
- `yRange : {number}`: Y-axis range
- `series : {{{number}}}`: Data series
- `xMarkers : {number}`: X-axis markers
- `yMarkers : {number}`: Y-axis markers
- `numPotentialGroups : integer`: Number of potential groups

## TpNetData (record)

```teal
global record TpNetData
```

Contains additional data about the transport network

- `getNodeData : function(TpNetData, NodeId) : NodeData`  (call as `tpNetData:getNodeData(…) (method)`): Get node data for a specific node
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/line_util.tl:1474`: `local nd = tpNetData:getNodeData(terminal.vehicleNodeId)`
- `getEdgeData : function(TpNetData, EdgeId) : EdgeData`: Get edge data for a specific edge

### TpNetData.NodeData (record)

```teal
record NodeData
```

Data attached to a transport network node.

- `ports : {Port}`: The list of ports on this node

#### TpNetData.NodeData.Port (record)

```teal
record Port
```

An individual port on a NodeData

- `edgeId : EdgeId`: The edge ID this port connects to
- `transitions : {integer}`: The indices of the ports this port can transition to

### TpNetData.EdgeData (record)

```teal
record EdgeData
```

Data attached to a transport network edge.

- `ports : {PortId, PortId}`: The ports at both ends of the edge
- `equidistParams : {number}`: The equidistant parameter samples along the edge

## Failure (record)

```teal
global record Failure
```
Access: `api.type.Failure`

Why a vehicle cannot use a line, as returned by `api.engine.util.line.getFailedPathReason`.

- `reason : Reason`: Reason of the failure.
- `pos : Vec3f`

### Failure.Reason (record)

```teal
record Reason
```
Access: `api.type.Failure.Reason`

Reason codes of `Failure`.

- `Unkown : Reason`: Unknown reason (the API spells it `Unkown`).
- `NoPath : Reason`: No path found.
- `NoElectric : Reason`: Electric tracks are missing.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:5410`: `if failureParam.reason == api.type.Failure.Reason.NoElectric then`
- `NoTram : Reason`: Tram tracks are missing.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:5412`: `elseif failureParam.reason == api.type.Failure.Reason.NoTram then`
- `NoTramButTramTrack : Reason`: Tracks are incompatible with the vehicle.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:5414`: `elseif failureParam.reason == api.type.Failure.Reason.NoTramButTramTrack then`
- `NoTramTrackButRail : Reason`: The tracks lack tram support (only possible with mods).
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:5416`: `elseif failureParam.reason == api.type.Failure.Reason.NoTramTrackButRail then`
- `NoRail : Reason`: Rail tracks are missing.
- `NoRoad : Reason`: Roads are missing.
- `NoSmallAircraft : Reason`: There is a path only for small aircraft, not for big airplanes.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:5418`: `elseif failureParam.reason == api.type.Failure.Reason.NoSmallAircraft then`
- `NoSmallShip : Reason`: There is a path only for small ships, not for big ships.
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:5420`: `elseif failureParam.reason == api.type.Failure.Reason.NoSmallShip then`

## FinanceData (record)

```teal
global record FinanceData
```
Access: `api.type.FinanceData`

Finance table of a player, as returned by `api.engine.util.finance.computeFinanceTable`. Each value list has one entry per column of `header`; amounts are in game currency.

Example:
```lua
local config = api.type.ChartConfig.new()
config.count = 4
local fd = api.engine.util.finance.computeFinanceTable(api.engine.util.getPlayer(), config)
fd:foreach_investment(function(key, values)
	print(key, values[1])
end)
```

- `equals : function(FinanceData, FinanceData) : boolean`  (call as `api.type.FinanceData.equals(…)`): Returns `true` when both `FinanceData` values are equal.
- `transport : {JournalEntry.Carrier : {integer : {integer}}}`: Transport rows per carrier, then per row key.
- `investment : {integer : {integer}}`: Investment rows per row key.
- `other : {JournalEntry.Other, {integer}}`: Other income and expenses per `JournalEntry.Other` type.
- `loan : {integer}`: Loan values per column.
- `interest : {integer}`: Interest per column.
- `loanBorrowing : {integer}`: Amount borrowed per column.
- `loanRepayment : {integer}`: Amount repaid per column.
- `total : {integer}`: Total per column.
- `balance : {integer}`: Balance per column.
- `header : {string}`: Column headers of the table.
- `unfoldKey : function(FinanceData, integer) : {JournalEntry.Type, JournalEntry.Maintenance, JournalEntry.Construction}`  (call as `api.type.FinanceData.unfoldKey(…)`): Decodes a row key of `transport` or `investment` into its journal entry parts.
    - returns: The `JournalEntry.Type`, `JournalEntry.Maintenance` and `JournalEntry.Construction` the key stands for.
- `foreach_carrier : function(FinanceData, function(JournalEntry.Carrier, {integer : {integer}}))`  (call as `api.type.FinanceData.foreach_carrier(…)`): Calls the function for each carrier in `transport`.
- `foreach_transport : function(FinanceData, function(integer, {integer}), JournalEntry.Carrier)`  (call as `api.type.FinanceData.foreach_transport(…)`): Calls the function with the row key and values of every transport row of the given carrier.
- `foreach_investment : function(FinanceData, function(integer, {integer}))`  (call as `api.type.FinanceData.foreach_investment(…)`): Calls the function with the row key and values of every investment row.
- `foreach_other : function(FinanceData, function(JournalEntry.Other, {integer}))`  (call as `api.type.FinanceData.foreach_other(…)`): Calls the function with the type and values of every row in `other`.

## LineIssue (record)

```teal
global record LineIssue
```
Access: `api.type.LineIssue`

Problem with the cargo configuration of a line, as returned by `api.engine.util.line.getLineIssues`.

- `type : Type`: Kind of issue.
- `stopIndex : integer`: Zero-based index of the stop with the issue.
- `cargoType : CargoTypeId`: Cargo type with the issue; negative when the issue has no cargo type.

### LineIssue.Type (record)

```teal
record Type
```
Access: `api.type.LineIssue.Type`

Kinds of line issues.

- `NowhereToLoad : Type`: A cargo type is configured on the line but cannot be loaded anywhere.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/line_warning.script.tl:15`: `if issue[1] == api.type.LineIssue.Type.NowhereToLoad then`
- `NowhereToUnload : Type`: A cargo type is loaded at a stop but cannot be unloaded anywhere.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/line_warning.script.tl:19`: `elseif issue[1] == api.type.LineIssue.Type.NowhereToUnload then`
- `NoVehicleToLoadCargo : Type`: No vehicle on the line can carry some of the cargo configured to be loaded.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/line_warning.script.tl:24`: `elseif issue[1] == api.type.LineIssue.Type.NoVehicleToLoadCargo then`
- `VehicleUseless : Type`: Some vehicles on the line cannot carry any of the configured cargo, or carry only cargo on a passenger line.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/line_warning.script.tl:29`: `elseif issue[1] == api.type.LineIssue.Type.VehicleUseless then`
- `LineCargoConfig : Type`: The line is not configured to load any cargo.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/line_warning.script.tl:35`: `elseif issue[1] == api.type.LineIssue.Type.LineCargoConfig then`
- `None : Type`: No issue.

## VehicleCargoInfo (record)

```teal
global record VehicleCargoInfo
```

Information about cargo of a single compartment of a transport vehicle

- `capacity : number`: Capacity of this vehicle
- `offset : number`: Sum of the cargo capacities up to this vehicle

## TransportVehicleInfo (record)

```teal
global record TransportVehicleInfo
```

Information about all cargo of a transport vehicle

- `cargoInfos : {{{VehicleCargoInfo}}}`: List of VehicleCargoInfo for vehicles, compartments and cargo types

## CarriageInfo (record)

```teal
global record CarriageInfo
```

Information about carriages

- `owningVehicle : Engine.Entity`: Vehicle entity (train or tram) the carriage belongs to; negative if none.
- `index : integer`
- `offset : number`
- `reversed : boolean`: Whether the carriage is reversed
- `crew : Engine.Entity`
- `end : number`

## VehicleExtent (record)

```teal
global record VehicleExtent
```

Information about travel-direction extent of an entire vehicle (consist of carriages)

- `back : number`: negative distance in meters from back to vehicle pin position
- `front : number`: positive distance in meters from vehicle pin position to front plus some random spacing ahead

## VehicleData (record)

```teal
global record VehicleData
```

Information about an entire vehicle (consist of carriages)

- `bounds : VehicleExtent`: space in x-axis/driving direction, values for in front and behind of center
- `length : number`: total length (without random spacing ahead)
- `topSpeed : number`: minimum of all top speeds
- `brakeDeceleration : number`: Acceleration when breaking (in m / s^2)
- `carriages : {CarriageData}`: List of carriaces for the vehicle (applies to trams and trains)

### VehicleData.CarriageData (record)

```teal
record CarriageData
```

Data of one carriage of the vehicle.

- `offset : number`
- `reversed : boolean`: Whether the carriage is reversed.
- `emissions : ModelMetadata.Emissions`: Modified amount of emission for the carrriage

## VehiclePartInfo (record)

```teal
global record VehiclePartInfo
```

Details about a vehicle part

- `seats : {Seat}`: List of seats in this vehicle part
- `count : number`: Number of occupied seats on this vehicles

### VehiclePartInfo.Seat (record)

```teal
record Seat
```

A single seat in a vehicle

- `occupied : Engine.Entity`: Entity occupying this seat
- `animation : string`: Animation playing for this seat
- `crew : boolean`

## MemoryStatistics (record)

```teal
global record MemoryStatistics
```

Collection of useful memory statistics

- `vidMemUsage : integer`: Video memory usage
- `trackedVidMemUsage : integer`: Tracked video memory usage
- `vidMemMax : integer`: Maximum video memory
- `peakVidMem : integer`: Peak video memory usage
- `ramUsage : integer`: RAM usage
- `trackedRamUsage : integer`: Tracked RAM usage
- `ramMax : integer`: Maximum RAM
- `peakRam : integer`: Peak RAM usage
- `peakTotalMem : integer`: Peak total memory usage
- `largestFreeBlock : integer`: Largest free memory block

## PdxErrorCodes (record)

```teal
global record PdxErrorCodes
```

Error codes of Paradox account operations (`PdxResult.errorCode`).

- `AccountAlreadyExists : PdxErrorCodes`: An account with this email address already exists.
- `BadEmailAddressFormat : PdxErrorCodes`: The email address is invalid.
- `BadPasswordLength : PdxErrorCodes`: The password is invalid.
- `BadDateFormat : PdxErrorCodes`: The date of birth is invalid.
- `UnderAgeAccount : PdxErrorCodes`: The user is too young to create a Paradox account (minimum age 16).
- `MissingAccountInfo : PdxErrorCodes`: Account information is missing. The GUI asks the user to check the network connection.
- `ProfileDoesNotExist : PdxErrorCodes`: No profile exists yet.
- `ProfileAlreadyTaken : PdxErrorCodes`: The profile name is already taken.
- `ProfileAlreadyExists : PdxErrorCodes`: The profile name already exists.
- `ProfileNameTooLong : PdxErrorCodes`: The profile name is too long.
- `ProfileNameTooShort : PdxErrorCodes`: The profile name is too short.
- `ProfileNameInvalid : PdxErrorCodes`: The profile name is invalid.

## PdxResult (interface)

```teal
global interface PdxResult
```

Result of an asynchronous Paradox account operation (via `PdxFuture`).

- `success : boolean`: Whether the operation succeeded.
- `errorCode : PdxErrorCodes`: Error code if the operation failed.

## PdxLoginResult (record)

```teal
global record PdxLoginResult is PdxResult
```

Result of `api.gui.backend.loginAccount` and `loginThirdPartyAccount`.

## PdxAccountInfo (record)

```teal
global record PdxAccountInfo is PdxResult
```

Result of `api.gui.backend.getAccountInfo`.

- `email : string`: Email address of the logged-in account.

## PdxProfileResult (record)

```teal
global record PdxProfileResult is PdxResult
```

Result of `api.gui.backend.getProfile`.

- `id : string`: Id of the profile.
- `name : string`: Name of the profile.

## PdxLegalDocument (record)

```teal
global record PdxLegalDocument is PdxResult
```

Legal document fetched with `api.gui.backend.getLegalDocument`.

- `title : string`: Title of the document.
- `text : string`: Text of the document.

## PdxCountry (record)

```teal
global record PdxCountry
```

Country entry from `api.gui.backend.getCountries`, used for account creation.

- `name : string`: Display name of the country.
- `code : integer`: Country code, passed as `countryCode` to `api.gui.backend.createAccount`.

## PdxThirdPartyLinkState (record)

```teal
global record PdxThirdPartyLinkState
```

Link state between the Paradox account and the platform account (Steam, PlayStation Network, Xbox network), from `api.gui.backend.getThirdPartyLinkState`.

- `NeitherAccountHasLink : PdxThirdPartyLinkState`: Neither account is linked yet.
- `ParadoxAccountHasLink : PdxThirdPartyLinkState`: The Paradox account is already linked to a different platform account.
- `ThirdPartyAccountHasLink : PdxThirdPartyLinkState`: The platform account is already linked to a different Paradox account.
- `BothAccountsHaveLinks : PdxThirdPartyLinkState`: Both accounts are linked to other accounts.
- `BothAccountsAreLinked : PdxThirdPartyLinkState`: The two accounts are linked to each other.

## PdxThirdPartyAutoLoginPromptState (record)

```teal
global record PdxThirdPartyAutoLoginPromptState
```

Answer to the prompt that offers automatic login to the linked Paradox account.

- `NotShown : PdxThirdPartyAutoLoginPromptState`: The prompt has not been shown.
- `Accepted : PdxThirdPartyAutoLoginPromptState`: The user accepted automatic login.
- `Rejected : PdxThirdPartyAutoLoginPromptState`: The user rejected automatic login.

## PdxFuture (record)

```teal
global record PdxFuture<T>
```

Handle to the result of an asynchronous Paradox account operation. Poll `getResult` (e.g. every step) until it returns a value.

Example:
```lua
react.onStep(function()
	local result = future:getResult()
	if result ~= nil then
		if not result.success then print(result.errorCode) end
	end
end)
```

- `getResult : function(PdxFuture<T>) : T`: return result if available (otherwise nil)
