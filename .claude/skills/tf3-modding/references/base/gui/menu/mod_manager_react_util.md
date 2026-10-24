# gui.menu.mod_manager_react_util

Definition file: `base/tealdef/gui/menu/mod_manager_react_util.d.tl` (in the game folder).

Building blocks of the Mod Hub (mod browser, mod details, upload form, login, report and dependency dialogs) and helpers around the `api.modhub` backends.

## ModManagerReactUtil (record)

```teal
global record ModManagerReactUtil
```
Load: `local mod_manager_react_util = ug_require "::/gui/menu/mod_manager_react_util.tl" as ModManagerReactUtil`

Util table of `mod_manager_react_util.tl` with the Mod Hub recipes, dialogs and backend helpers.

- `makeErrorMessage : function(err : Modhub.ModHubErrorCode) : string`  (call as `mod_manager_react_util.makeErrorMessage(…)`): Turns a Mod Hub error into a message for the player.
  In game: "Please check your network connection and try again later. (Error Code: {code})" (German: „Bitte überprüfe deine Netzwerkverbindung und versuche es später erneut. (Fehlercode: {code})“)
    - returns: The backend's `error_message` field if present; in debug mode the raw message with the error code; otherwise a
generic "check your network connection" text with the error code.

- `showMessageBox : function(wc : Builtin.WindowAPI, setBlockedForModal : function(boolean), title : string, message : string)`  (call as `mod_manager_react_util.showMessageBox(…)`): Opens a centred `DialogReactUtil.DialogWindow` with a title, a message and an OK button.
    - param `wc`: Window API to add the dialog to.
    - param `setBlockedForModal`: Passed to the dialog as `setBlockedForModal`.
    - param `title`: Window title.
    - param `message`: Message text.
- `createAsyncRef : function() : ReactRefT<Modhub.UniquePendingRequestId>`  (call as `mod_manager_react_util.createAsyncRef(…)`): Creates a ref for a pending `api.modhub` request that is aborted automatically when the component unmounts.
    - returns: A ref, initially holding nil.
    - notes: Uses `react.useRef` and `react.onUnmount`, so call it inside a recipe function.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:247`: `local subscribeRef = mod_manager_react_util.createAsyncRef()`
- `setAsyncRef : function(ref : ReactRefT<Modhub.UniquePendingRequestId>, request : Modhub.UniquePendingRequestId)`  (call as `mod_manager_react_util.setAsyncRef(…)`): Stores a new pending request in an async ref, aborting the request it held before. Pass nil to clear it after a request finished.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:311`: `mod_manager_react_util.setAsyncRef(subscribeRef, api.modhub.subscribeModAsync(`
- `showErrorBox : function(windowApi : Builtin.WindowAPI, setBlockedForModal : function(boolean), title : string, errorCode? : Modhub.ModHubErrorCode)`  (call as `mod_manager_react_util.showErrorBox(…)`): Shows an error dialog with `showMessageBox`.
  In game: "Error" (German: „Error“)
    - param `windowApi`: Window API to add the dialog to.
    - param `setBlockedForModal`: Passed to the dialog.
    - param `title`: Window title; without `errorCode` it is used as the message and the title becomes "Error".
    - param `errorCode`: Mod Hub error whose `makeErrorMessage` text is shown. Optional.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:1477`: `mod_manager_react_util.showErrorBox(wc, modSelectorPage.commonParams.setBlockedForModal, title, errorCode)`
- `showLoginDialog : function(context : ModManagerContext, onLogin? : function())`  (call as `mod_manager_react_util.showLoginDialog(…)`): Opens the Mod Hub login window for the context's backend.
    - param `onLogin`: Called when the login window closes, whether or not the login succeeded. Optional.
- `showReportDialog : function(context : ModManagerContext, modId : Modhub.ModId)`  (call as `mod_manager_react_util.showReportDialog(…)`): Opens the "Report Mod" window for a mod and blocks the UI behind it until it closes.
- `showUninstallDialog : function(context : ModManagerContext, modId : Modhub.ModId, modTitle : string)`  (call as `mod_manager_react_util.showUninstallDialog(…)`): Opens the "Force Uninstall Mod" confirmation window, which calls `api.modhub.forceUninstallModAsync` on confirmation.
    - param `modTitle`: Mod title shown in the question.
- `checkMissingDependencies : function(context : ModManagerContext, modIds : {Modhub.ModId}, asyncRef : ReactRefT<Modhub.UniquePendingRequestId>)`  (call as `mod_manager_react_util.checkMissingDependencies(…)`): Looks up the Mod Hub dependencies of the given mods with `fetchMissingDependencies` and, if any are not subscribed, opens the missing-dependencies dialog.
    - notes: Called by `ModSubscribeButton` after a successful subscription.
- `fetchMissingDependencies : function(modIds : {Modhub.ModId}, asyncRef : ReactRefT<Modhub.UniquePendingRequestId>, onDone : function({ModDependency}))`  (call as `mod_manager_react_util.fetchMissingDependencies(…)`): Collects the direct Mod Hub dependencies of the given mods that the player has not subscribed to.
    - param `modIds`: Mods to check. Subscribed mods whose info says they have no dependencies are skipped.
    - param `asyncRef`: Ref that holds the pending detail requests.
    - param `onDone`: Called with the unsubscribed dependencies (each listed once); called with an empty list when there is no publishing backend.
    - notes: The mod details are requested one after another from the backend returned by `getModPublishBackendId()`. Dependencies of dependencies are not followed.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:1174`: `mod_manager_react_util.fetchMissingDependencies(dependents, dependencyRef,`
- `showMissingDependenciesDialog : function(context : ModManagerContext, params : MissingDependenciesParams)`  (call as `mod_manager_react_util.showMissingDependenciesDialog(…)`): Opens the missing-dependencies window, which lists the dependencies with a subscribe button each.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:1186`: `mod_manager_react_util.showMissingDependenciesDialog({`
- `showShareSavegameDialog : function(context : ModManagerContext, saveId : SaveGameId, savegame : boolean)`  (call as `mod_manager_react_util.showShareSavegameDialog(…)`): Opens the "Share Savegame" window that stages a save game or saved map as a mod and uploads it to the publishing backend.
    - param `saveId`: Save to share.
    - param `savegame`: True for a save game, false for a saved map.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/savegame_react_util.tl:1114`: `mod_manager_react_util.showShareSavegameDialog(context, makeSaveId(selectedSaveOld), savegameNamespace ~= app.SaveGameNamespace.getSavemap())`
- `showValidationReportDialog : function(context : ModManagerContext, result : Modhub.ModValidatorResult)`  (call as `mod_manager_react_util.showValidationReportDialog(…)`): Opens the "Validation Report" window with the results of a mod validation.
- `doRequiredUserAction : function(context : ModManagerContext, action : function())`  (call as `mod_manager_react_util.doRequiredUserAction(…)`): Runs `action` right away when the player is logged in to the context's backend; otherwise opens the login dialog first and runs `action` only if the login succeeded.
- `getTotalModMemoryMB : function(modIds : {Mod.ModId}) : integer`  (call as `mod_manager_react_util.getTotalModMemoryMB(…)`): Sums the estimated size of the given installed mods.
    - returns: Total of `totalEstimatedSizeMB` from `modRep:getModSize`, in MB. Mods that are not installed are skipped.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:917`: `local totalMemoryMB = mod_manager_react_util.getTotalModMemoryMB(modIds)`
- `ModImage : Recipe<ModImageParams>`: Recipe that loads and shows a mod image from the Mod Hub, with a throbber while loading and the error message over a placeholder on failure.
    - notes: Its API (`react.provideApi`) is a `ModImageApi`. A new request is only sent when `request` changes.
- `ModTile : Recipe<ModTileParams>`: Tile of one mod in the mod browser (image, title, info icons and buttons).
  In game: "Subscribers", "Likes", "Not Public" (German: „Abonnenten“, „Likes“, „Nicht öffentlich“)
    - notes: Unpublished mods (`modInfo.visible` false) get the `unpublished` style class.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_page.tl:119`: `return mod_manager_react_util.ModTile {`
- `ModTilesNotAvailable : Recipe<ModTilesNotAvailableParams>`: Card shown instead of the mod tiles while loading, on errors or when no mods match the filters.
- `DisplayRows : integer`: Number of tile rows of the mod browser (15).
- `DisplayCols : integer`: Number of tile columns of the mod browser (4).
- `DisplayPerPage : integer`: Mods per page (`DisplayRows * DisplayCols`, 60); passed to `ModBrowserDataProvider.new`.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_page.tl:220`: `dataProviders[tabKey]:set(api.type.modhub.ModBrowserDataProvider.new(backendId, mod_manager_react_util.DisplayPerPage))`
- `ModListPager : Recipe<ModListPagerParams>`: Row of page buttons with previous/next arrows and "…" gaps. The base game's mod browser no longer uses it.
- `ModManagementStatus : Recipe<ModManagementStatusParams>`: Progress bar of the backend's pending downloads or uploads, with a "Retry Download" button when `api.modhub.shouldRetryInstall` is true.
  In game: "Downloading Mods", "No Downloads Pending", "Retry Download" (German: „Mods werden heruntergeladen“, „Keine Downloads ausstehend“, „Download erneut versuchen“)
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/main_page.tl:29`: `modManagementBusyState:old() and mod_manager_react_util.ModManagementStatus {`
- `ModListStatus : Recipe<ModListStatusParams>`: Text "Showing 1-n of total Mods". The base game's mod browser no longer uses it.
  In game: "Showing {from}-{to} of {total} Mods" (German: „Zeige {from}–{to} von {total} Mods“)
- `SortCombobox : Recipe<SortComboboxParams>`: Combo box of the sort orders, labelled with `getSortTypeInfo`.
- `ModSubscribeButton : Recipe<ModSubscribeButtonParams>`: Subscribe/unsubscribe toggle button of a mod, showing the install state icon.
  In game: "Subscribe", "Unsubscribe", "Failed To Subscribe" (German: „Abonnieren“, „Deabonnieren“, „Abonnieren fehlgeschlagen“)
    - notes: Logs the player in first if needed (`doRequiredUserAction`). For a subscribed mod without enough storage space it shows a dialog that offers to retry or unsubscribe (or only cancel, depending on the backend capabilities).
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_page.tl:97`: `table.insert(buttons, mod_manager_react_util.ModSubscribeButton {`
- `ModActivateButton : Recipe<ModActivateButtonParams>`: Activate/deactivate button used in the mod selector's details page.
  In game: "Activate", "Activated" (German: „Aktivieren“, „Aktiviert“)
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:180`: `buttons[#buttons+1] = mod_manager_react_util.ModActivateButton {`
- `ModVoteButtonBar : Recipe<ModVoteButtonBarParams>`: Like and dislike toggle buttons with vote counts. The player's own rating is requested when logged in; until it arrives the buttons are disabled.
  In game: "Failed To Get Rating" (German: „Abrufen der Bewertung fehlgeschlagen“)
- `ModSearchField : Recipe<ModSearchFieldParams>`: Search text field of the mod browser (at most 80 characters).
  In game: "Search..." (German: „Suche...“)
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:1539`: `table.insert(actionsLayout, mod_manager_react_util.ModSearchField {`
- `ModFilterMenu : Recipe<ModFilterMenuParams>`: List of collapsible checkbox groups, one per visible tag group, for filtering the mod browser.
- `UserActions : Recipe<UserActionsParams>`: Account buttons of the mod browser: "Refresh" (if the backend supports manual refresh), "Log In" when logged out, and the account name, which opens an account window with a log-out button.
  In game: "mod.io Refresh", "{num} Mods Refreshed", "Log In" (German: „mod.io aktualisieren“, „{num} Mods aktualisiert“, „Anmelden“)
- `ModUploadForm : Recipe<ModUploadFormParams>`: Upload form of a staging mod: validation status and report, publish options, upload progress and result.
  In game: "Upload", "Upload Successful", "Validation Issues Detected" (German: „Hochladen“, „Hochladen erfolgreich“, „Validierungsprobleme gefunden“)
- `ModUploadNotAvailable : Recipe<ModUploadFormParams>`: Placeholder shown instead of `ModUploadForm` when the staging mod was not found; ignores its parameters.
  In game: "Upload Mod: Unavailable", "Staging mod not found" (German: „Mod hochladen: nicht verfügbar“, „Staging-Mod nicht gefunden“)
- `ModDetails : Recipe<ModDetailsParams>`: Main content of the mod details window: editable title bar for staging mods, image gallery and description.
  In game: "Open Mod Page", "Open Folder", "Rename" (German: „Mod-Seite öffnen“, „Ordner öffnen“, „Umbenennen“)
    - notes: Its API (`react.provideApi`) is a `ModDetailsApi`.
- `ModAuthor : Recipe<ModAuthorParams>`: "Author" section of the mod details: avatar and name, optionally as a button that shows the author's other mods.
  In game: "Author" (German: „Autor“)
- `ModDependencies : Recipe<ModDependenciesParams>`: "Dependencies" section of the mod details: a table of dependency names with an action each.
  In game: "Dependencies", "No Dependencies" (German: „Abhängigkeiten“, „Keine Abhängigkeiten“)
- `ModTags : Recipe<ModTagsParams>`: "Tags" section of the mod details; for staging mods with remove buttons and an add button that opens a tag picker.
  In game: "Tags", "Choose a Tag to Add" (German: „Tags“, „Einen Tag zum Hinzufügen auswählen“)
- `ModStats : Recipe<ModStatsParams>`: "Stats" section of the mod details: downloads, rating, file size, date added and date updated.
  In game: "Stats", "Downloads", "{percentage} Positive" (German: „Statistiken“, „Downloads“, „{percentage} positiv“)
- `ImageGallery : Recipe<ImageGalleryParams>`: Image gallery of the mod details: a large image with thumbnails for navigation.
  In game: "Images", "Add Image", "Remove Image" (German: „Bilder“, „Bild hinzufügen“, „Bild entfernen“)
    - notes: Its API (`react.provideApi`) is a `ModGalleryApi`.
- `ModBrowserTab : Recipe<ModBrowserTabParams>`: Mod browser of one backend: search field, account buttons, sort box, tag filters, scrolling tile list and status bars.
  In game: "Loading Mods...", "No Mods Found for the Selected Filters", "Please log in to access the Mod Hub." (German: „Mods werden geladen…“, „Keine Mods für die ausgewählten Filter gefunden“, „Bitte anmelden, um auf den Mod Hub zuzugreifen.“)
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_page.tl:250`: `item = mod_manager_react_util.ModBrowserTab {`
- `ModDetailsWindow : Recipe<ModDetailsWindowParams>`: Window recipe of the mod details, opened with `addSingletonWindow(mod_manager_react_util.ModDetailsWindow, params)`.
    - notes: For `modManagerParams` the window replaces the contexts' `setBlockedForModal`, so that dialogs opened from the details disable the window content and hide its close button while they are open.
    - used 4x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_page.tl:145`: `wc.removeAllWindows(mod_manager_react_util.ModDetailsWindow)`
- `getSourceInfo : function(source : string) : SourceInfo`  (call as `mod_manager_react_util.getSourceInfo(…)`): Returns icon and title for a mod source.
  In game: "Built-In", "User Mod", "My Mods" (German: „Built-in Mods“, „Benutzer-Mod“, „Meine Mods“)
    - param `source`: `"UserMods"`, `"StagingArea"`, `"BuiltInMods"`, `"DLC"` or the name of a Mod Hub backend (e.g. `"mod.io"`); unknown values give "System".
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:589`: `local sourceInfo = mod_manager_react_util.getSourceInfo(source)`
- `getSortTypeInfo : function(sortType : Modhub.SortType) : SortTypeInfo`  (call as `mod_manager_react_util.getSortTypeInfo(…)`): Returns the display name of a Mod Hub sort type (`"MISSING"` for unknown types).
  In game: "Trending", "Rating", "Activation Order" (German: „Angesagt“, „Bewertung“, „Aktivierungsreihenfolge“)
- `getTagInfo : function(tag : string) : TagInfo`  (call as `mod_manager_react_util.getTagInfo(…)`): Returns the display name of a mod tag: its translation (`app.getModTagTranslation`), or, when there is none, the title from `getSourceInfo(tag)`.
- `StorageBar : Recipe<StorageBarParams>`: Bar showing the storage used by the backend's mods (in MB, and as a share of the total when known); shown on consoles.
- `ModMemoryBar : Recipe<ModMemoryBarParams>`: Bar showing the estimated size of the active mods against a memory budget, with an overflow style when the budget is exceeded; used in the mod selector.
  In game: "{total} / {budget} MB" (German: „{total} / {budget} MB“)
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:2019`: `memoryState:old().budgetMB >= 0 and mod_manager_react_util.ModMemoryBar{`
- `getModPublishBackendId : function() : Modhub.BackendId`  (call as `mod_manager_react_util.getModPublishBackendId(…)`): Returns the Mod Hub backend that can download and publish mods (the first backend that is not info-only), or nil if there is none.
    - notes: Logs an error when there is more than one such backend.
    - used 8x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:287`: `local remoteModhub = mod_manager_react_util.getModPublishBackendId()`
- `getLocalBackendId : function() : Modhub.BackendId`  (call as `mod_manager_react_util.getLocalBackendId(…)`)
- `checkModManagerAccess : function(windowApi : Builtin.WindowAPI) : boolean`  (call as `mod_manager_react_util.checkModManagerAccess(…)`): Checks whether the Mod Hub can be opened and shows a message box explaining why when it cannot.
  In game: "Mod Hub Unavailable", "The mod hub is currently unavailable, please check your internet connection or try again later.", "You must be 13 or older to access the Mod Hub." (German: „Mod Hub nicht verfügbar.“, „Der Mod Hub ist aktuell nicht verfügbar, bitte Internetverbindung überprüfen oder später erneut versuchen.“, „Ein Mindestalter von 13 Jahren ist für den Zugang zum Mod Hub erforderlich.“)
    - returns: False when the GUI backend or a Mod Hub backend is not initialised, when the player has no access to user-generated
content, or when the player is younger than 13; otherwise true.

    - used 2x in the base game, e.g. `base/content/gui/gui/menu/main_page.tl:615`: `if not mod_manager_react_util.checkModManagerAccess(mainPageParams.commonParams.windowContainer:get():getApi()) then`

### ModManagerReactUtil.ModAction (enum)

```teal
enum ModAction
```

Mod actions; not used by the base game scripts.

- `"ForceUninstall"`
- `"Report"`

### ModManagerReactUtil.GalleryState (enum)

```teal
enum GalleryState
```

Loading state of a mod's images, returned by `ModGalleryApi.getState` and `ModDetailsApi.getGalleryState`.

- `"Loading"`: Not all images have loaded yet.
- `"Error"`: At least one image failed to load (for mod details, also when the mod has no logo image).
- `"Ready"`: All images have loaded.

### ModManagerReactUtil.ModInfoIcon (record)

```teal
record ModInfoIcon
```

An icon with label shown on a mod tile (`ModTileParams.leftInfoIcons`, `ModTileParams.customSortInfoIcon`).

- `iconPath : string`: Path of the icon image.
- `title : string`: Label shown next to the icon.
- `tooltip : string`: Tooltip of the icon.
- `critical : boolean`: Marks the icon as critical (e.g. "Staging mod not found"); only used for left icons.

### ModManagerReactUtil.DialogButton (record)

```teal
record DialogButton
```

A button of the missing-dependencies dialog.

- `text : string`: Button label.
- `isSecondary : boolean`: Secondary style instead of primary.
- `onClick : function()`: Called after the dialog was closed. Optional.

### ModManagerReactUtil.MissingDependenciesParams (record)

```teal
record MissingDependenciesParams
```

Content of the dialog opened by `showMissingDependenciesDialog`.

- `title : string`: Window title; default "Missing Dependencies".
- `message : string`: Text at the top; default "The following required mods are not subscribed:".
- `names : {string}`: Names listed below the message (one per line). Optional.
- `dependenciesMessage : string`: Text shown above the list of subscribable dependencies. Optional.
- `dependencies : {ModDependency}`: Mods listed with a subscribe button each.
- `footer : string`: Text below the lists. Optional.
- `buttons : {DialogButton}`: Buttons at the bottom; a single "Close" button when nil or empty.

### ModManagerReactUtil.ModDependency (record)

```teal
record ModDependency
```

A Mod Hub mod that another mod depends on.

- `modId : Modhub.ModId`: Mod Hub id of the dependency.
- `name : string`: Display name of the dependency.

### ModManagerReactUtil.ModManagerContext (record)

```teal
record ModManagerContext
```

Backend and UI services passed to most Mod Hub recipes and dialogs.

Notes:

The mod manager page builds one context per backend tab from its `CommonMenuParams` (`windowContainer`,
`setBlockedForModal`, `calloutContainerRef`) and adds `createModTile`.

- `backendId : Modhub.BackendId`: Mod Hub backend that requests go to.
- `wc : ReactRefWrapApiT<Builtin.WindowContainerDelegateParam, Builtin.WindowAPI>`: Window container used for dialogs; call `wc:get():getApi()`.
- `setBlockedForModal : function(blocked : boolean)`: Called with true/false when a modal dialog opens or closes (see `CommonMenuParams.setBlockedForModal`).
- `calloutContainerRef : ReactRefWrapApi0<CalloutApi>`: Callout container passed on to tiles and dialogs.
- `onRefresh : function()`
- `createModTile : function(displayData : Modhub.ModBrowserDisplayData, index : integer, isCurated : boolean) : TreeNodeId`: Builds the tile of one entry of the mod browser; `ModBrowserTab` calls it for every entry it displays.
    - param `displayData`: Current display data of the browser.
    - param `index`: Index of the entry in `displayData.entries`.
    - param `isCurated`: True when the mod has the `Curated` tag.
    - returns: The tile, usually a `ModTile`.

### ModManagerReactUtil.ModImageParams (record)

```teal
record ModImageParams
```

Parameters of `ModImage`.

- `context : ModManagerContext`: Context of the backend the image is loaded from.
- `request : Modhub.GetModMediaRequest`: Image to load with `api.modhub.getModMediaAsync`; when nil, `imagePath` or a placeholder is shown.
- `result : Modhub.AsyncResult<Modhub.ModMediaData>`: Already loaded result; when set, it is shown instead of the result of `request`.
- `imagePath : string`: Local image shown when `request` is nil; defaults to a placeholder.
- `modMediaType : Modhub.ModMediaType`: Media type used by `ImageGallery` when it builds the request for this entry.

### ModManagerReactUtil.ModImageApi (record)

```teal
record ModImageApi
```

API of a `ModImage` component (`react.provideApi`); read it through a node ref with `ref:get():getApi()`.

- `hasData : function() : boolean`: Returns true once the image has loaded.
- `getError : function() : Modhub.ModHubErrorCode`: Returns the error of the last request, or nil.

### ModManagerReactUtil.ModTileParams (record)

```teal
record ModTileParams
```

Parameters of `ModTile`.

- `context : ModManagerContext`: Context of the tile's backend.
- `modPublishHelper : ReactRefT<Modhub.ModPublishHelper>`: Publish helper; when it has a staging mod for `modHubId`, the cover image can be added or removed on the tile.
- `modHubId : Modhub.ModId`: Mod Hub id used with `modPublishHelper` for the staging mod.
- `modInfo : Modhub.ModInfo`: Mod Hub info of the mod (title, summary, logo, statistics).
- `modId : Mod.ModId`
- `sortType : Modhub.SortType`: Current sort type; selects which statistic is shown on the right of the tile.
- `onClick : function`: Called when the tile or its details button is clicked.
- `leftInfoIcons : {ModInfoIcon}`
- `customSortInfoIcon : ModInfoIcon`: Icon on the right of the tile when sorting by `Custom` (activation order).
- `createButtons : function() : {TreeNodeId}`: Returns the buttons of the tile (e.g. a `ModSubscribeButton`).
- `onRefresh : function()`: Called after the cover image of a staging mod was changed.

### ModManagerReactUtil.ModTilesNotAvailableParams (record)

```teal
record ModTilesNotAvailableParams
```

Parameters of `ModTilesNotAvailable`.

- `layout : TreeNodeId`: Content of the card.

### ModManagerReactUtil.ModListPagerParams (record)

```teal
record ModListPagerParams
```

Parameters of `ModListPager`.

- `currentPage : integer`: Current page, counted from 0.
- `numPages : integer`: Number of pages.
- `totalResults : integer`: Total number of results; not used by the pager.
- `onPageClick : function(pageNum : integer)`: Called with the clicked page, counted from 0.
- `onNextClick : function()`: Called by the "next" arrow.
- `onPrevClick : function()`: Called by the "previous" arrow.

### ModManagerReactUtil.ModManagementStatusParams (record)

```teal
record ModManagementStatusParams
```

Parameters of `ModManagementStatus`.

- `context : ModManagerContext`: Context of the backend to watch.
- `upload : boolean`: Uses upload texts ("Uploading Mods") instead of download texts.

### ModManagerReactUtil.ModListStatusParams (record)

```teal
record ModListStatusParams
```

Parameters of `ModListStatus`.

- `currentPage : integer`: Current page, counted from 0.
- `displayPerPage : integer`: Mods per page.
- `totalPages : integer`: Number of pages; not used.
- `totalResults : integer`: Total number of mods.

### ModManagerReactUtil.SortComboboxParams (record)

```teal
record SortComboboxParams
```

Parameters of `SortCombobox`.

- `sortOrders : {Modhub.SortOrder}`: Available sort orders, from `getSortAndFilterOptions().sortOrders`.
- `sortType : Modhub.SortType`: Selected sort type.
- `onSortChange : function(Modhub.SortType)`: Called with the newly selected sort type.

### ModManagerReactUtil.ModSubscribeButtonParams (record)

```teal
record ModSubscribeButtonParams
```

Parameters of `ModSubscribeButton`.

- `context : ModManagerContext`: Context of the backend to subscribe through.
- `modId : Modhub.ModId`: Mod Hub id of the mod.
- `checkExists : boolean`: Requests the mod details first and disables subscribing when the backend does not find the mod.
- `checkDependencies : boolean`: Set to false to skip `checkMissingDependencies` after subscribing; nil or true checks them.

### ModManagerReactUtil.ModActivateButtonParams (record)

```teal
record ModActivateButtonParams
```

Parameters of `ModActivateButton`.

- `active : boolean`: Whether the mod is active; shows "Activated" instead of "Activate".
- `missing : boolean`: Shows an issue icon on the button.
- `onValueChange : function(value : boolean)`: Called with the toggled state (`not active`) when clicked.

### ModManagerReactUtil.ModVoteButtonBarParams (record)

```teal
record ModVoteButtonBarParams
```

Parameters of `ModVoteButtonBar`.

- `context : ModManagerContext`: Context of the backend to vote through.
- `modInfo : Modhub.ModInfo`: Mod to rate; its `likes` and `dislikes` give the counts.

### ModManagerReactUtil.ModSearchFieldParams (record)

```teal
record ModSearchFieldParams
```

Parameters of `ModSearchField`.

- `value : string`: Current search text.
- `onChange : function(string)`: Called with the new text while typing, and with an empty string on cancel.

### ModManagerReactUtil.ModFilterMenuParams (record)

```teal
record ModFilterMenuParams
```

Parameters of `ModFilterMenu`.

- `sortAndFilterOptions : Modhub.SortAndFilterOptions`: Tag groups to offer; only groups marked `visible` are shown.
- `getTagToggled : function(string) : boolean`: Returns whether a tag filter is active.
- `onToggleTag : function(string)`: Called with the tag whose checkbox was toggled.
- `topItems : {TreeNodeId}`: Items placed above the tag groups (required; may be empty).
- `bottomItems : {TreeNodeId}`: Items placed below the tag groups (required; may be empty).

### ModManagerReactUtil.UserActionsParams (record)

```teal
record UserActionsParams
```

Parameters of `UserActions`.

- `context : ModManagerContext`: Context of the backend.
- `doRefresh : function()`: Called after a successful manual refresh or login.
- `doLogIn : function()`

### ModManagerReactUtil.ModUploadFormParams (record)

```teal
record ModUploadFormParams
```

Parameters of `ModUploadForm` and `ModUploadNotAvailable`.

- `context : ModManagerContext`: Context of the publishing backend.
- `modPublishHelper : ReactRefT<Modhub.ModPublishHelper>`: Publish helper that holds the staging mod.
- `modId : Modhub.ModId`: Id of the staging mod.
- `errorMessage : string`: When set, shown as status text and the upload button is disabled.
- `loadingMessage : string`: When set, shown as status text instead of the default.
- `onRefresh : function()`: Called after the staging mod data was changed.
- `publishSettings : Modhub.PublishSettings`: Fixed publish settings; when set, the visibility, metadata and changelog inputs are hidden.
- `onClose : function()`: Called by the "Ok" button after a successful upload.
- `disableValidator : boolean`: Skips mod validation and publishes as a save game (`PublishSettings.isSavegame`); set by the save-game sharing dialog.

### ModManagerReactUtil.ModDetailsParams (record)

```teal
record ModDetailsParams
```

Parameters of `ModDetails`.

- `context : ModManagerContext`: Context of the backend the images are loaded from.
- `uploadContext : ModManagerContext`: Context of the publishing backend; used for the "Open Mod Page" button of a staging mod.
- `modPublishHelper : ReactRefT<Modhub.ModPublishHelper>`: Publish helper; when it has a staging mod for `modId`, title and images can be edited.
- `onRefresh : function()`: Called after the staging mod data was changed.
- `modId : Modhub.ModId`: Mod Hub id of the mod.
- `title : string`: Mod title.
- `logoImage : string`: Logo image of the mod; when empty, the gallery state is reported as `Error`.
- `url : string`
- `description : string`: Description, rendered as HTML.
- `imageParams : {ModImageParams}`: Images of the gallery.
- `hasResult : boolean`: False while the mod details are still loading (gallery state `Loading`).
- `isCurated : boolean`: Shows the animated "Curated" badge.

### ModManagerReactUtil.ModAuthorParams (record)

```teal
record ModAuthorParams
```

Parameters of `ModAuthor`.

- `context : ModManagerContext`: Context of the backend the avatar is loaded from.
- `uploadContext : ModManagerContext`
- `modPublishHelper : ReactRefT<Modhub.ModPublishHelper>`
- `onRefresh : function()`
- `onClose : function()`: Called after `onShowAuthorMods` when the author button is clicked.
- `modId : Modhub.ModId`: Mod whose author avatar is loaded; no avatar is shown when its value is empty.
- `authorName : string`: Author name.
- `userPortal : Modhub.UserPortal`: Portal of the author; PlayStation Network and Xbox Live show a fixed image instead of the avatar.
- `onShowAuthorMods : function()`: Called when the author button is clicked; the mod manager page then lists the author's mods.
- `isUserNameSearchEnabled : boolean`: Makes the author a button that calls `onShowAuthorMods`.

### ModManagerReactUtil.ModDependencyEntry (record)

```teal
record ModDependencyEntry
```

One row of `ModDependencies`.

- `name : string`: Name of the dependency.
- `action : TreeNodeId`: Component shown next to the name, e.g. a subscribe button.

### ModManagerReactUtil.ModDependenciesParams (record)

```teal
record ModDependenciesParams
```

Parameters of `ModDependencies`.

- `entries : {ModDependencyEntry}`: The rows; "No Dependencies" is shown when empty.

### ModManagerReactUtil.ModTagsParams (record)

```teal
record ModTagsParams
```

Parameters of `ModTags`.

- `context : ModManagerContext`: Context used for the tag-picker dialog.
- `tags : {string}`: Tags of the mod.
- `uploadContext : ModManagerContext`
- `modPublishHelper : ReactRefT<Modhub.ModPublishHelper>`: Publish helper; when it has a staging mod for `modId`, tags can be added and removed.
- `onRefresh : function()`: Called after the tags of the staging mod were changed.
- `modId : Modhub.ModId`: Id of the mod.

### ModManagerReactUtil.ModStatsParams (record)

```teal
record ModStatsParams
```

Parameters of `ModStats`.

- `context : ModManagerContext`
- `modInfo : Modhub.ModInfo`: Mod whose statistics are shown; nothing is listed when nil.

### ModManagerReactUtil.ImageGalleryParams (record)

```teal
record ImageGalleryParams
```

Parameters of `ImageGallery`.

- `context : ModManagerContext`: Context of the backend the images are loaded from.
- `modPublishHelper : ReactRefT<Modhub.ModPublishHelper>`: Publish helper; when it has a staging mod for `modId`, images can be added, moved and removed.
- `modId : Modhub.ModId`: Id of the mod.
- `imageParams : {ModImageParams}`: The images, in order.
- `showSelector : boolean`: Not used by the implementation.
- `onRefresh : function()`: Called after the images of the staging mod were changed.

### ModManagerReactUtil.ModGalleryApi (record)

```teal
record ModGalleryApi
```

API of a `ImageGallery` component (`react.provideApi`); read it through a node ref with `ref:get():getApi()`.

- `getState : function() : GalleryState`: Returns the combined loading state of all gallery images.

### ModManagerReactUtil.ModDetailsApi (record)

```teal
record ModDetailsApi
```

API of a `ModDetails` component (`react.provideApi`); read it through a node ref with `ref:get():getApi()`.

- `getGalleryState : function() : GalleryState`: Returns the loading state of the mod's images.

### ModManagerReactUtil.ModBrowserTabParams (record)

```teal
record ModBrowserTabParams
```

Parameters of `ModBrowserTab`.

- `context : ModManagerContext`: Context of the backend; its `createModTile` builds the tiles.
- `dataProvider : ReactRefT<Modhub.ModBrowserDataProvider>`: Data provider of the listed mods (search, filters, sorting, paging), e.g. `api.type.modhub.ModBrowserDataProvider.new(backendId, DisplayPerPage)`.
- `additionalTopBarItems : {TreeNodeId}`: Extra items for the top bar, placed after the search field and account buttons.
- `additionalBottomBarItems : {TreeNodeId}`: Extra items for the bottom bar (e.g. a `ModMemoryBar`).
- `aboveMainContentItem : TreeNodeId`: Item placed above the mod tiles.
- `showComboBox : boolean`: Shows the status filter combo box (from the tag group at `statusTagGroupIndex`) above the filter menu.

### ModManagerReactUtil.ModManagerDetailsPageParams (record)

```teal
record ModManagerDetailsPageParams
```

Mod details of the mod manager, shown in `ModDetailsWindow` through `modManagerParams`.

- `context : ModManagerContext`: Context of the backend the mod comes from.
- `uploadContext : ModManagerContext`: Context of the publishing backend, when there are staging mods.
- `modId : Modhub.ModId`: Mod Hub id of the mod.
- `modPublishHelperRef : ReactRefT<Modhub.ModPublishHelper>`: Publish helper for staging mods.
- `onClose : function()`: Closes the details window.
- `detailsBack : function(userSearchId? : Modhub.UserId, authorName? : string)`: Returns from the details to the browser; with a user id it filters the browser by that author (used by the author button).
- `isUserNameSearchEnabled : boolean`: Makes the author clickable (see `ModAuthorParams.isUserNameSearchEnabled`).
- `isCurated : boolean`: Shows the "Curated" badge.
- `inPauseMenu : boolean`: When true, a left click outside the window closes it.

### ModManagerReactUtil.ModSelectorDetailsPageParams (record)

```teal
record ModSelectorDetailsPageParams
```

Mod details of the mod selector (new-game mod settings), shown in `ModDetailsWindow` through `modSelectorParams`.

- `mod : string`: Name (folder id) of the installed mod.
- `modInfo : Modhub.ModInfo`: Mod Hub info of the mod; its `modSource` selects the backend for the details.
- `modParams : {string : integer}`: Current values of the mod's settings (parameter key to option index).
- `isModActive : function(modName : string)`: Returns whether a mod (by name) is active.
- `toggleMod : function(modName : string)`: Activates or deactivates a mod (by name).
- `onParamChanged : function(key : string, index : integer)`: Called with parameter key and new option index when a mod setting is changed.
- `onClose : function()`: Closes the details window.
- `isCurated : boolean`: Shows the "Curated" badge.
- `inPauseMenu : boolean`: When true, a left click outside the window closes it.

### ModManagerReactUtil.ModDetailsWindowParams (record)

```teal
record ModDetailsWindowParams
```

Parameters of `ModDetailsWindow`; set either `modManagerParams` or `modSelectorParams`.

- `title : string`: Window title; default "Mod Details".
- `modManagerParams : ModManagerDetailsPageParams`: Shows the mod manager's details page; takes precedence over `modSelectorParams`.
- `modSelectorParams : ModSelectorDetailsPageParams`: Shows the mod selector's details page.
- `onClose : function()`: Called by the close button, unless a dialog opened from the window is still blocking it.

### ModManagerReactUtil.SourceInfo (record)

```teal
record SourceInfo
```

Icon and title of a mod source, returned by `getSourceInfo`.

- `iconPath : string`: Path of the source icon.
- `title : string`: Translated source name.

### ModManagerReactUtil.SortTypeInfo (record)

```teal
record SortTypeInfo
```

Display data of a sort type, returned by `getSortTypeInfo`.

- `title : string`: Translated name of the sort type; `Custom` is shown as "Activation Order".

### ModManagerReactUtil.TagInfo (record)

```teal
record TagInfo
```

Display data of a mod tag, returned by `getTagInfo`.

- `name : string`: Translated tag name.

### ModManagerReactUtil.StorageBarParams (record)

```teal
record StorageBarParams
```

Parameters of `StorageBar`.

- `backendId : Modhub.BackendId`: Backend whose storage use is shown.

### ModManagerReactUtil.ModMemoryBarParams (record)

```teal
record ModMemoryBarParams
```

Parameters of `ModMemoryBar`; all values in MB.

- `budgetMB : integer`: Memory budget for mods.
- `usageMB : integer`: Estimated size of the active mods.
- `deltaMB : integer`: Size that the focused mod adds; shown as a second bar on top of `usageMB`. Negative values are subtracted.
