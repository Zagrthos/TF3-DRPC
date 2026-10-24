# gui.menu.savegame_react_util

Definition file: `base/tealdef/gui/menu/savegame_react_util.d.tl` (in the game folder).

Savegame helpers and recipes of the load, save and new game pages, loaded with `ug_require "/gui/menu/savegame_react_util.tl"`.

## SavegameReactUtil (record)

```teal
global record SavegameReactUtil
```
Load: `local savegame_react_util = ug_require "::/gui/menu/savegame_react_util.tl" as SavegameReactUtil`

Savegame helpers and recipes for the main menu and pause menu pages.

- `makeModIdList : function({string}) : {Mod.ModId}`  (call as `savegame_react_util.makeModIdList(…)`): Converts mod names to mod ids.
    - returns: A `Mod.ModId` per name.
    - used 4x in the base game, e.g. `base/content/gui/gui/menu/new_game_or_map_settings_page.tl:603`: `local modIdList = savegame_react_util.makeModIdList(activeModsState:old())`
- `makeModStringList : function({Mod.ModId}) : {string}`  (call as `savegame_react_util.makeModStringList(…)`): Converts mod ids to mod names.
    - returns: The `name` of each mod id.
- `groupSaves : function(savegames : {SaveGameInfo}, filter : string, prefix : string, sortMode? : SavegameFiltersParam.SortMode, ascending? : boolean, saveTypeFilter? : SaveGameType) : {SaveGroup}`  (call as `savegame_react_util.groupSaves(…)`): Groups savegames by their group name and sorts the groups.
    - param `savegames`: Savegames to group, for example from `app.findAllSavegames(namespace)`.
    - param `filter`: If set, only saves whose name contains this text are included.
    - param `prefix`: Stored as `groupName.prefix` of each group; the base game uses `"!"` for normal and `"?"` for built-in saves.
    - param `sortMode`: `"Name"` sorts by group name, otherwise by the time of the most recent save.
    - param `ascending`: Order of the sort.
    - param `saveTypeFilter`: If set, only saves of this type are included.
    - returns: The groups. Within a group the saves are sorted newest first; `mostRecent` is the newest manual save, or the newest save if there is no manual one.
    - used 5x in the base game, e.g. `base/content/gui/gui/menu/load_game_page.tl:34`: `return savegame_react_util.groupSaves(app.findAllSavegames(saveGameNamespace), filterText, "!", sortMode, ascending)`
- `getSaveGroupAndType : function(saveName : string) : string, SaveGameType, string, string`  (call as `savegame_react_util.getSaveGroupAndType(…)`): group, type, ingame date, file timestamp

Parses a savegame name with `app.parseSavegameName`.
    - returns: Group name, save type, in-game date and file timestamp.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/main_page.tl:411`: `local continueTitle = continueState:old().lastSaveGameId and savegame_react_util.getSaveGroupAndType(continueState:old().lastSaveGameId.saveGameName) or ""`
- `checkForError : function(savegameData : SaveGameData) : {ErrorType}, string`  (call as `savegame_react_util.checkForError(…)`): Checks whether a savegame can be loaded.
  In game: "Required mods or DLCs are missing.", "Map size or map format is not supported.", "The savegame could not be loaded." (German: „Erforderliche Mods oder DLCs fehlen.“, „Die Kartengrösse bzw. das Kartenformat wird nicht unterstützt.“, „Der Spielstand konnte nicht geladen werden.“)
    - param `savegameData`: Savegame data from `app.getSavegameInfo`.
    - returns: The list of errors (empty if none) and a translated error text.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/main_page.tl:368`: `errorTypes, errorMsg = savegame_react_util.checkForError(dataContinueState:old())`
- `SavegameCard : Recipe<SavegameCardParams>`: Tile showing a savegame group with its screenshot, save date and info icons, and a load button unless `save` is set.
  In game: "Load Game", "Date Saved", "Required mods or DLCs are missing." (German: „Spiel laden“, „Speicherdatum“, „Erforderliche Mods oder DLCs fehlen.“)
    - notes: The savegame information is loaded asynchronously; a loading or error image is shown meanwhile.
    - used 4x in the base game, e.g. `base/content/gui/gui/menu/load_game_page.tl:143`: `table.insert(elements, savegame_react_util.SavegameCard(react.ref(focusedCardRef), params))`
- `SavegameFilters : Recipe<SavegameFiltersParam>`: Filter text field and sort mode combo box for savegame lists.
  In game: "Filter Savegames...", "Name", "Date" (German: „Spielstände filtern ...“, „Name“, „Datum“)
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/load_game_page.tl:161`: `table.insert(headerChildren, savegame_react_util.SavegameFilters{`
- `SavegameSelectCard : Recipe<SavegameCardSelectParams>`: Detail view of a savegame group on the load page, with the saves of the group, their details, load, share and delete buttons and tabs for gameplay settings and mods.
  In game: "Load Game", "Delete Savegame", "The savegame will be lost permanently." (German: „Spiel laden“, „Spielstand löschen“, „Der Spielstand geht dauerhaft verloren.“)
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/load_game_page.tl:95`: `savegame_react_util.SavegameSelectCard(react.ref(detailPageRef), {`
- `SavegameStorageBar : Recipe0`: Progress bar showing the used and total savegame storage space in MB.
  In game: "Game Storage" (German: „Spielspeicher“)
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/load_game_page.tl:209`: `table.insert(bodyChildren, savegame_react_util.SavegameStorageBar())`
- `LoadPreview : Recipe<LoadPreviewParam>`: Image of a savegame (from `app.getSavegameImage`, or the screenshot in the savegame data), or a loading or error image.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/new_game_or_map_settings_page.tl:861`: `childrenRight[#childrenRight + 1] = savegame_react_util.LoadPreview{`
- `ScrollableAchievementsList : Recipe<ScrollableAchievementsListParams>`: Scrollable list of all achievements with their unlocked state and progress.
  In game: "Done" (German: „Fertig“)
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:918`: `savegame_react_util.ScrollableAchievementsList{},`
- `getCampaignInfoSorted : function(campaign : ResName) : CampaignInfo`  (call as `savegame_react_util.getCampaignInfoSorted(…)`): Collects the missions of a campaign, sorted by order, with their completion-based availability.
    - param `campaign`: Resource name of the campaign.
    - returns: The campaign info, or `nil` if the campaign is not found.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/campaign_page.tl:171`: `local campaignInfo = savegame_react_util.getCampaignInfoSorted(campaign)`

### SavegameReactUtil.SavegameCardParams (record)

```teal
record SavegameCardParams
```

Parameters of `SavegameCard`.

- `saveGame : SaveGroup`: Savegame group shown by the card; its most recent save is loaded and shown.
- `saveGameNamespace : string`: Namespace of the savegame (for example `app.SaveGameNamespace.getSavemap()` for maps).
- `mapEditor : boolean`: Whether the save is loaded in the map editor (button text "Load Map").
- `mapNewGame : boolean`: Whether the card starts a new game on a map (button text "Play Map").
- `builtinSavegame : boolean`: Whether the savegame is built into the game; shows an info icon that it cannot be deleted.
- `modMap : boolean`: Whether the map comes from a mod; shows an info icon.
- `deluxeMap : boolean`: Whether to show the animated "Deluxe" badge.
- `onClickMain : function(SaveGameData, SaveGameId)`: optional
- `onClickDetails : function(SaveGameData, SaveGameId)`: optional
- `onDoubleClickMain : function(SaveGameData, SaveGameId)`: optional
- `save : boolean`: Whether the card is used on the save page; the card then has no load button.
- `extraClass : string`: Additional style classes of the card.
- `calloutContainerRef : ReactRefWrapApi0<CalloutApi>`: Callout container used by the card.
- `commonParams : CommonMenuParams`: Common parameters of the menu page.

### SavegameReactUtil.SavegameFiltersParam (record)

```teal
record SavegameFiltersParam
```

Parameters of `SavegameFilters`.

- `sortMode : SortMode`: Current sort mode.
- `setSortMode : function(mode : SortMode)`: Called when the player chooses another sort mode.
- `filterText : string`: Current filter text.
- `setFilterText : function(text : string)`: Called while the player types in the filter field.

#### SavegameReactUtil.SavegameFiltersParam.SortMode (enum)

```teal
enum SortMode
```

Sort order of savegame groups.

- `"Name"`: Sort by group name.
  In game: "Name" (German: „Name“)
- `"Date"`: Sort by the time of the most recent save.
  In game: "Date" (German: „Datum“)

### SavegameReactUtil.SelectedSavegame (record)

```teal
record SelectedSavegame
```

Savegame selected on the load page.

- `saveGroup : GroupId`: Group of the selected savegame.
- `savegameData : SaveGameData`: Data of the selected savegame, possibly with changed mods and settings; `nil` until loaded.
- `initialData : SaveGameData`: Data of the savegame as stored, used to detect changed mods and settings.
- `index : integer`: Index of the selected save within the group's saves.

### SavegameReactUtil.SavegameCardSelectParams (record)

```teal
record SavegameCardSelectParams
```

Parameters of `SavegameSelectCard`.

- `selectedSavegame : SelectedSavegame`: Initially selected savegame.
- `saveNameSpace : string`: Namespace of the savegames.
- `map : boolean`: Whether the save is loaded in the map editor.
- `inPauseMenu : boolean`: Whether the card is shown in the pause menu; passed on to the mod selector page.
- `commonParams : CommonMenuParams`: Common parameters of the menu page.
- `onDelete : function()`: Called when the selected group no longer exists, for example after its last save has been deleted.

### SavegameReactUtil.ErrorType (enum)

```teal
enum ErrorType
```

Reason why a savegame cannot be loaded as it is.

- `"ModMissing"`: Required mods or DLCs are missing.
- `"CannotLoad"`: The savegame information could not be read.
- `"UnsupportedMapDimension"`: The map size or format is not supported (checked on consoles only).

### SavegameReactUtil.LoadPreviewParam (record)

```teal
record LoadPreviewParam
```

Parameters of `LoadPreview`.

- `saveGameData : SaveGameData`: Savegame data; its screenshot is used if the savegame image cannot be loaded.
- `saveGameId : SaveGameId`: Id of the savegame whose image is shown.

### SavegameReactUtil.ScrollableAchievementsListParams (record)

```teal
record ScrollableAchievementsListParams
```

Parameters of `ScrollableAchievementsList`.

- `enableRightStickNavigation : boolean`: Whether the list scrolls with the `IA_SCROLL_UP` and `IA_SCROLL_DOWN` input actions.

### SavegameReactUtil.CampaignInfo (record)

```teal
record CampaignInfo
```

Missions of a campaign with their unlocked state, returned by `getCampaignInfoSorted`.

- `missionInfo : {MissionInfo}`: Missions of the campaign, sorted by their `order`.
- `desc : CampaignDesc`: Campaign description.
- `highestUnlocked : integer`: Position (in `missionInfo`) of the last completed mission; `0` if none is completed.

#### SavegameReactUtil.CampaignInfo.MissionInfo (record)

```teal
record MissionInfo
```

One mission of a campaign.

- `name : string`: Resource name of the mission.
- `desc : MissionDesc`: Mission description.
- `enabled : boolean`: Whether the mission can be started. With `unlockingProgression`, only missions up to one after the last completed one are enabled.
