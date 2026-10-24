# gui.menu.main_menu

Definition file: `base/tealdef/gui/menu/main_menu.d.tl` (in the game folder).

Shared types of the main menu and the in-game pause menu: page names, the common parameters every menu page receives, save-game groups and release-note data.

## Page (enum)

```teal
global enum Page
```

Name of a menu page, passed to `CommonMenuParams.setPage`.

Notes:

The main menu handles `Main`, `NewGame`, `NewGameOrMapSettings`, `LoadGame`, `Tutorial`, `Campaign`, `ModManager`,
`Achievements`, `Settings`, `Credits`, `LicensePage` and `ProgressPage`; any other value shows the main page. The
in-game pause menu handles `Main` (its own start page), `SaveGame`, `LoadGame`, `Settings` and `ModManager`. No base
game script sets `MapEditor` or `ReleaseNotes` (release notes are opened with `CommonMenuParams.showReleaseNotes`).

- `"Main"`: Start page of the menu (in the pause menu, its main page).
- `"NewGame"`: Page for starting a new game.
  In game: "New Game", "Map Editor" (German: „Neues Spiel“, „Karten-Editor“)
- `"SaveGame"`: Save page of the in-game pause menu.
  In game: "Save Game", "Save Map", "Save and Play" (German: „Spiel speichern“, „Karte speichern“, „Karte speichern und spielen“)
- `"NewGameOrMapSettings"`: Settings page of a new game or map (`new_game_or_map_settings_page.tl`).
- `"ModManager"`: Mod Hub / mod manager page.
  In game: "Mod Hub" (German: „Mod-Hub“)
- `"LoadGame"`: Load-game page.
  In game: "Load Game", "Load Map" (German: „Spiel laden“, „Karte laden“)
- `"Campaign"`: Campaign page; with `CampaignParam.mission` set it shows that mission.
  In game: "Campaign" (German: „Kampagne“)
- `"Tutorial"`: Tutorial page.
  In game: "Tutorial" (German: „Tutorial“)
- `"MapEditor"`
- `"Achievements"`: Achievements page.
  In game: "Achievements", "Trophies" (German: „Errungenschaften“, „Trophäen“)
- `"Settings"`: Settings page.
  In game: "Settings" (German: „Einstellungen“)
- `"Credits"`: Credits page.
  In game: "Credits" (German: „Mitwirkende“)
- `"LicensePage"`: License-agreement page; the main menu shows it instead of the main page while `api.gui.backend.hasLegalDocuments()` is true.
- `"ReleaseNotes"`
- `"ProgressPage"`: Loading screen with progress bar; the main menu switches to it on its own while the progress monitor reports a task. Its parameters are a `ProgressPageMissionParam`.
  In game: "Start Game" (German: „Spiel starten“)

## NewOrLoadPage (enum)

```teal
global enum NewOrLoadPage
```

Tab of the new-game / map settings page and of the load-game settings.

- `"NewOrLoadMain"`: General settings tab.
- `"NewOrLoadAdvanced"`: Advanced settings tab.
  In game: "Gameplay Settings" (German: „Gameplay-Einstellungen“)
- `"NewOrLoadMods"`: Mods tab.

## CommonMenuParams (record)

```teal
global record CommonMenuParams
```

Services that the main menu or the in-game pause menu hands to each of its pages as `commonParams`.

Notes:

The pause menu fills only `setPage`, `windowContainer`, `calloutContainerRef`, `onClose`, `inGame` (true) and
`setBlockedForModal`. The main menu fills all other fields but not `onClose` and `inGame`.

- `setPage : function(Page, params? : any)`: Switches the menu to another page.
    - notes: The second argument is passed to the new page as its `params` (e.g. a `CampaignParam` or a `ProgressPageMissionParam`). In the main menu, leaving a page while mods are still downloading first asks the player in a dialog, and switching also sets the Mod Hub management mode for the new page.
- `showReleaseNotes : function(fadeIn? : boolean, closeCallback? : function(), pos? : Vec2f)`: Opens the release-notes window (main menu only).
    - notes: `fadeIn` adds the `fade-in` style class, `closeCallback` is called when the window closes and `pos` is the initial window position.
- `windowContainer : ReactRefWrapApiT<Builtin.WindowContainerDelegateParam, Builtin.WindowAPI>`: Window container of the menu; call `windowContainer:get():getApi()` to get the `Builtin.WindowAPI` used to add and remove dialog windows.
- `calloutContainerRef : ReactRefWrapApi0<CalloutApi>`: Ref to the menu's callout container.
- `setBlockedForModal : function(blocked : boolean)`: Call with true when a modal dialog opens and with false when it closes. Calls are counted, so each true needs a matching false.
- `onClose : function()`: Closes the pause menu (pause menu only).
- `inGame : boolean`: True when the page is shown in the in-game pause menu; not set (nil) in the main menu.
- `getDLCModState : function() : GameEditionTextParam`: Returns which edition packs are installed (main menu only).
    - returns: A `GameEditionTextParam`.
- `triggerBackgroundEvent : function(pos : Vec2f, forceR? : boolean, forceG? : boolean, forceB? : boolean, forceA? : boolean)`: Starts a highlight effect of the animated main-menu background at a position (main menu only). Ignored if the last background event was less than 0.6 seconds ago.
    - notes: The background has four effect channels (R, G, B, A) that are used in turn; `forceR` to `forceA` select a specific channel instead.
- `triggerBackgroundEventBig : function(pos? : Vec2f)`: Starts the large effect of the animated main-menu background (main menu only); `pos` defaults to the centre `(0.5, 0.5)`. Ignored if the last background event was less than 0.6 seconds ago.
- `fadeIn : FadeInMode`: Current fade-in state of the main page; nil once the fade-in is over.

### CommonMenuParams.FadeInMode (enum)

```teal
enum FadeInMode
```

Visibility state of the main page while the main menu fades in.

- `"Invisible"`: The main page is still hidden (first frames, to hide transition lag).
- `"FadeIn"`: The main page is fading in.

## GroupId (record)

```teal
global record GroupId
```

Identifies a save-game group, i.e. all saves of one game (manual saves, autosaves, ...).

- `name : string`: Group name parsed from the save names (`app.parseSavegameName(saveName).groupName`).
- `prefix : string`: Marks the source of the group; the load-game page uses `"!"` for the player's saves and `"?"` for built-in saves.

## SaveGroup (record)

```teal
global record SaveGroup
```

Saves of one game grouped together, as returned by `savegame_react_util.groupSaves`.

- `mostRecent : SaveGameInfo`: The newest manual save of the group, or the newest save of any type if there is no manual save.
- `mostRecentIndex : integer`: Index of `mostRecent` in `saves`.
- `saves : {SaveGameInfo}`: All saves of the group, newest first.
- `groupName : GroupId`: Id of the group.

## GameEditionTextParam (record)

```teal
global record GameEditionTextParam
```

Which edition packs are installed; used to choose the logo and the release-note texts.

- `hasDeluxeDLC : boolean`: True when the mod `urbangames_deluxe_upgrade_pack` is installed.
- `hasPreorderDLC : boolean`: True when the mod `urbangames_preorder_pack` is installed.

## ReleaseNoteData (record)

```teal
global record ReleaseNoteData
```

Content of the release-notes window, returned by `data(edition)` in `/gui/menu/configs/release_notes.lua`.

- `infoItemsPerPage : integer`: How many items are shown per tab page of the window.
- `updateMessageID : integer`: Version number of these release notes; increase it for each new update.
    - notes: The main menu opens the release notes on start when `AppConfig.showReleaseNotes` is true and `AppConfig.updateMessageID` is lower than this value. The "do not show again" button stores this value in `AppConfig.updateMessageID`.
- `title : string`: Window title; a single space is used when nil.
- `infoItems : {InfoItem}`: The items, in display order.

### ReleaseNoteData.InfoItem (record)

```teal
record InfoItem
```

One page (text and/or image) of the release notes.

- `title : string`: Heading of the item. Optional.
- `description : string`: Text of the item; supports inline hints. Optional.
- `image : string`: Path of an image shown next to (or, without description, instead of) the text. Optional.
- `preferShaderQuadOverImage : boolean`: Shows the animated game logo (`MenuUtil.EditionLogo`) instead of `image`.
- `leftImage : boolean`: Places the image left of the text instead of right.
- `filterOutVendor : {string}`: Vendor names (converted with `api.util.string2Vendor`); the item is hidden when the game runs on one of them.
- `filterOutPlatform : {string}`: Platform names (converted with `api.util.string2Platform`); the item is hidden on these platforms.
- `alignment : string`: Style class for the text when there is no image, e.g. `"left-align"`.
- `scrollable : boolean`: Puts the text into a scroll area.
- `background : boolean`: Adds the `with-background` style class to the item.

## ProgressData (record)

```teal
global record ProgressData
```

Mission and save game shown by the loading screen; also the payload of the `SetProgressData` GUI event.

- `mission : ResName`: Mission resource being loaded, used for the loading-screen image and text. Optional.
- `saveGameId : SaveGameId`: Save game being loaded; its group name is shown as the title. Optional.

## MissionPageContentParam (record)

```teal
global record MissionPageContentParam
```

Parameters of the mission page content recipe (`/gui/menu/mission_page_content.tl`) of the campaign page.

- `campaign : string`: Campaign resource name; `.campaign` is appended if missing.
- `mission : string`: Mission resource name; `.mission` is appended if missing.
- `onLoad : function(onMount : function())`: Called when the player starts or continues the mission.
    - notes: The argument is a function that actually loads the mission or save game. The campaign page switches to the `ProgressPage` and passes this function as `ProgressPageMissionParam.onMount`, so loading starts once the loading screen is shown.

## IMenuPageParam (interface)

```teal
global interface IMenuPageParam
```

Parameters every menu page recipe receives: the shared `commonParams` and page-specific `params`.

- `commonParams : CommonMenuParams`: Services of the hosting menu (main menu or pause menu).
- `params : ISpecificParam`: Page-specific parameters, i.e. the second argument of `CommonMenuParams.setPage`. May be nil.

### IMenuPageParam.ISpecificParam (interface)

```teal
interface ISpecificParam
```

Marker interface for the page-specific parameter types (such as `CampaignParam` or `MainPageParam`).

## CampaignParam (record)

```teal
global record CampaignParam
```

Page parameters of the `Campaign` page.

- `campaign : string`: Campaign to show; the first campaign is used when nil.
- `mission : string`: Mission to show; when set, the page shows that mission instead of the campaign overview.
- `showEditorsNote : boolean`: Shows the editor's note page. It is also shown when the player has not seen it yet.

## ProgressPageMissionParam (record)

```teal
global record ProgressPageMissionParam
```

Page parameters of the `ProgressPage` (loading screen).

- `mission : ResName`: Mission being loaded.
- `saveGameId : SaveGameId`: Save game being loaded.
- `onMount : function()`: Called once, on the first frame after the loading screen is shown; used to start the actual load.

## MainPageParam (record)

```teal
global record MainPageParam
```

Page parameters of the `Main` page.

- `startWithReleaseNotes : boolean`: Opens the release notes when the main page appears; the main menu sets it on start when there are unseen release notes.

## MenuPageParam (record)

```teal
global record MenuPageParam<P is IMenuPageParam.ISpecificParam> is IMenuPageParam
```

`IMenuPageParam` with a typed `params` field; page recipes declare their parameter as e.g. `MenuPageParam<CampaignParam>`.

- `params : P`: Page-specific parameters of type `P`. May be nil.
