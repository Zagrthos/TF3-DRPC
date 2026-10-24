# api.type.modhub

Definition file: `api/tealdef/api/type/modhub.d.tl` (in the game folder).

Contains all type definitions for the modhub.

## Globals

- `Mod` (type): Forward declaration of the `Mod` record, which is defined in `api/type/mod.d.tl`.

## Modhub (record)

```teal
global record Modhub
```
Access: `api.type.modhub`

modhub types

- `BackendId : integer`: Integer handle of a modhub backend (e.g. mod.io or the local staging area). List them with `api.modhub.getBackendIDs()`.
    - used 6x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_page.tl:16`: `browsingBackend : Modhub.BackendId`

### Modhub.ModValidatorMessage (record)

```teal
record ModValidatorMessage
```
Access: `api.type.modhub.ModValidatorMessage`

Message from mod validator

- `context : string`: The context of the message, e.g. which check produced it
- `file : string`: The file path where the issue was found
- `level : LogLevel`: The severity level of the message
- `message : string`: The validation message
- `consoleOnly : boolean`: Whether this issue only affects console platforms

### Modhub.ModValidatorResult (record)

```teal
record ModValidatorResult
```
Access: `api.type.modhub.ModValidatorResult`

Result from mod validation

- `messages : {ModValidatorMessage}`: List of validation messages
- `criticalPc : boolean`: Whether the validation found critical issues on PC
- `criticalConsole : boolean`: Whether the validation found critical issues on console
- `cookedPcPath : string`: Cooked PC Path
- `cookedConsolePath : string`: Cooked Console Path
- `levelPc : LogLevel`: Minimum log level found on PC
- `levelConsole : LogLevel`: Minimum log level found on console

### Modhub.ModHubErrorCode (record)

```teal
record ModHubErrorCode
```
Access: `api.type.modhub.ModHubErrorCode`

Error returned by a failed modhub request, e.g. from `Modhub.AsyncResult.getError`.

Notes:

The base mod manager shows `fields["error_message"]` to the user when it is set; otherwise it shows `message` and `code` only in debug mode and a generic network error with the code.

- `code : integer`: Error Code Number
- `message : string`: Error Message (not localized, for debug purposes only)
- `fields : {string : string}`: Additional fields with more information about the error

### Modhub.SortType (record)

```teal
record SortType
```
Access: `api.type.modhub.SortType`

Sort orders for mod lists in the mod browser. Values are accessed as `api.type.modhub.SortType.<Name>`.

- `Trending : SortType`: Sort by trend; the mod tiles then show the subscriber count.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:400`: `if sortType == api.type.modhub.SortType.Trending or sortType == api.type.modhub.SortType.SubscriberCount then`
- `Rating : SortType`: Sort by rating; the mod tiles then show the rating as a percentage.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:391`: `if sortType == api.type.modhub.SortType.Rating then`
- `SubscriberCount : SortType`: Sort by number of subscribers.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:400`: `if sortType == api.type.modhub.SortType.Trending or sortType == api.type.modhub.SortType.SubscriberCount then`
- `DateAdded : SortType`: Sort by publication date (`ModInfo.publishedUnixTimestamp`).
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:409`: `if sortType == api.type.modhub.SortType.DateAdded then`
- `DateUpdated : SortType`: Sort by last update (`ModInfo.updatedUnixTimestamp`).
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:418`: `if sortType == api.type.modhub.SortType.DateUpdated then`
- `Name : SortType`: Sort by mod name.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:5017`: `elseif sortType == api.type.modhub.SortType.Name then`
- `Source : SortType`: Sort by mod source (`ModInfo.modSource`).
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:427`: `if sortType == api.type.modhub.SortType.Source then`
- `Custom : SortType`: Custom order supplied by the caller; the mod selector uses it for the activation order of the active mods (shown as "Activation Order").
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:437`: `if sortType == api.type.modhub.SortType.Custom then`

### Modhub.SortOrder (record)

```teal
record SortOrder
```
Access: `api.type.modhub.SortOrder`

One sort option offered by a backend, part of `Modhub.SortAndFilterOptions.sortOrders`.

- `sortType : SortType`: The sort type of this option.
- `allowAscDesc : boolean`: Whether this sort option can be used in both ascending and descending order.

### Modhub.TagGroup (record)

```teal
record TagGroup
```
Access: `api.type.modhub.TagGroup`

A group of filter tags offered by a backend (shown as one collapsible checkbox list in the mod filter menu).

- `name : string`: Name of the tag group; the GUI translates it with `app.getModTagTranslation`.
- `values : {string}`: Tags in this group; each one can be toggled as a filter.
- `allowMultipleSelection : boolean`: Whether more than one tag of this group can be selected at once.
- `noSortTag : boolean`
- `visible : boolean`: Whether the group is shown in the filter menu.

### Modhub.SortAndFilterOptions (record)

```teal
record SortAndFilterOptions
```
Access: `api.type.modhub.SortAndFilterOptions`

Sort and filter options of a backend or data provider. Get them with `api.modhub.getSortAndFilterOptions(backendId)` or `provider:getSortAndFilterOptions()`.

- `tagGroups : {TagGroup}`: Filter tag groups.
- `sortOrders : {SortOrder}`: Available sort options, used to fill the sort combo box.
- `statusTagGroupIndex : integer`: 1-based index into `tagGroups` of the group shown as the status combo box; the GUI ignores it when it is `nil` or larger than the number of groups.

### Modhub.UserInfo (record)

```teal
record UserInfo
```
Access: `api.type.modhub.UserInfo`

The user logged in to a backend; `api.modhub.getUserInfo(backendId)` returns `nil` when nobody is logged in.

- `userName : string`: User name of the account.
- `profileUrl : string`: URL of the user's profile page.
- `displayName : string`: Display name of the user, shown in the account menu.

### Modhub.ModId (record)

```teal
record ModId
```
Access: `api.type.modhub.ModId`

Identifier of a mod in a modhub backend (e.g. a mod.io mod or a staging-area mod). Not the same as `Mod.ModId`; convert with `api.modhub.getModHubModIdForModId`.

- `new : function() : ModId`  (call as `api.type.modhub.ModId.new(…)`): Creates a new, empty `ModId`.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_page.tl:131`: `local modId = api.type.modhub.ModId.new()`
- `clone : function(ModId) : ModId`  (call as `api.type.modhub.ModId.clone(…)`): Returns a copy of the id.
    - param `#1`: The id to copy.
- `isValid : function(ModId) : boolean`  (call as `modId:isValid(…) (method)`): Returns whether the id refers to a mod (e.g. a staging mod that was already published has a valid `StagingModData.modhubId`).
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3996`: `if modhubId:isValid() then`
- `value : string`: The id as a string.

### Modhub.UserId (record)

```teal
record UserId
```
Access: `api.type.modhub.UserId`

Identifier of a user in a modhub backend, e.g. the author of a mod (`ModDetailsData.userId`).

- `new : function(value? : integer) : UserId`  (call as `api.type.modhub.UserId.new(…)`): Creates a user id. Without a value the id is invalid, which the mod manager uses to clear an author search (`provider:setSearchUserId(api.type.modhub.UserId.new())`).
    - param `value`: Numeric user id.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_page.tl:42`: `dataProvider:get():setSearchUserId(api.type.modhub.UserId.new())`
- `equals : function(UserId, UserId) : boolean`  (call as `api.type.modhub.UserId.equals(…)`): Returns `true` when both `UserId` values are equal.
- `isValid : function(UserId) : boolean`  (call as `api.type.modhub.UserId.isValid(…)`): Returns whether the id refers to a user.
- `value : integer`: Numeric user id.

### Modhub.ModManagementMode (record)

```teal
record ModManagementMode
```
Access: `api.type.modhub.ModManagementMode`

How much mod management (downloads, installs) a backend may do; set with `api.modhub.setModManagementMode(backendId, mode)`.

Notes:

The main menu sets `Full` on the main, mod manager, settings and similar pages, `Restricted` on the new game, load game and campaign pages (or `Disabled` if the backend's `Capabilities.allowRestrictedMode` is false), and `Disabled` when the menu closes. Leaving `Full` while downloads are running warns that unfinished downloads will be cancelled.

- `Disabled : ModManagementMode`: No mod management; unfinished downloads are cancelled.
    - used 8x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:296`: `setModManagementMode(isRestrictedModeAllowed() and api.type.modhub.ModManagementMode.Restricted or api.type.modhub.ModManagementMode.Disabled)`
- `Restricted : ModManagementMode`: Limited mod management while setting up or loading a game; mods that are still downloading will not be available.
    - used 8x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:296`: `setModManagementMode(isRestrictedModeAllowed() and api.type.modhub.ModManagementMode.Restricted or api.type.modhub.ModManagementMode.Disabled)`
- `Full : ModManagementMode`: Full mod management, used outside a game.
    - used 8x in the base game, e.g. `base/content/gui/gui/menu/main_menu.tl:547`: `return api.type.modhub.ModManagementMode.Full`

### Modhub.InstallState (record)

```teal
record InstallState
```
Access: `api.type.modhub.InstallState`

Install state of a subscribed mod, from `api.modhub.getModInstallState(backendId, modId)`.

- `None : InstallState`: No install state.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:5382`: `and api.modhub.getModInstallState(params.context.backendId, state:old().modDetails.modInfo.modId) ~= api.type.modhub.InstallState.None`
- `Downloading : InstallState`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:48`: `elseif installState == api.type.modhub.InstallState.Downloading then`
- `UpdatePending : InstallState`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:50`: `elseif installState == api.type.modhub.InstallState.UpdatePending then`
- `Installed : InstallState`
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:52`: `elseif installState == api.type.modhub.InstallState.Installed then`
- `DownloadPending : InstallState`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:54`: `elseif installState == api.type.modhub.InstallState.DownloadPending then`
- `InstallationPending : InstallState`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:56`: `elseif installState == api.type.modhub.InstallState.InstallationPending then`
- `UninstallPending : InstallState`
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:58`: `elseif installState == api.type.modhub.InstallState.UninstallPending then`
- `Extracting : InstallState`
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:60`: `elseif installState == api.type.modhub.InstallState.Extracting then`
- `InsufficientSpace : InstallState`: Installation failed because there is not enough storage space.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:44`: `if installState == api.type.modhub.InstallState.InsufficientSpace then`
- `MiscError : InstallState`: Installation failed with a file system error.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:46`: `elseif installState == api.type.modhub.InstallState.MiscError then`

### Modhub.ModInstallInfo (record)

```teal
record ModInstallInfo
```
Access: `api.type.modhub.ModInstallInfo`

Where and in which revision an installed mod lies, from `api.modhub.getModInstallInfo(backendId, modId)`.

- `installPath : string`: Folder the mod is installed in.
- `runtimeRevision : integer`

### Modhub.ModReleaseInfo (record)

```teal
record ModReleaseInfo
```
Access: `api.type.modhub.ModReleaseInfo`

Information about the current release of a mod (`ModInfo.releaseInfo`).

- `version : string`: Version string of the release.
- `releaseNotes : string`: Release notes (change log) of the release.
- `releaseUnixTimestamp : integer`: Release date as a Unix timestamp.

### Modhub.ModInfo (record)

```teal
record ModInfo
```
Access: `api.type.modhub.ModInfo`

Summary of a mod as listed by a backend: title, texts, statistics, tags. Used for mod tiles and the mod detail page.

- `clone : function(ModInfo) : ModInfo`  (call as `api.type.modhub.ModInfo.clone(…)`): Returns a copy of the mod info.
    - param `#1`: The mod info to copy.
- `modId : ModId`: Id of the mod in the backend.
- `modSource : string`: Origin of the mod, e.g. `"StagingArea"`, `"BuiltInMods"`, `"DLC"` or the name of a modhub backend such as `"mod.io"` (see `Modhub.Description.name`).
- `visible : boolean`: Whether the mod is public in the backend; publishing uses it as the default for `PublishSettings.setPublic`.
- `numDownloads : integer`: Number of downloads.
- `numSubscribers : integer`: Number of subscribers.
- `title : string`: Title of the mod.
- `url : string`: URL of the mod's web page.
- `summary : string`: Short summary text of the mod.
- `description : string`: Long description as plain text; the GUI prefers `descriptionHtml` when set.
- `descriptionHtml : string`: Long description as HTML.
- `logoImage : string`: Logo image of the mod; an empty string means the mod has no logo.
- `installSize : integer`: Size of the installed mod in bytes (the GUI formats it with `api.util.formatMemory`).
- `likes : integer`: Number of positive ratings.
- `dislikes : integer`: Number of negative ratings.
- `rating : number`: Rating as a fraction, shown with `api.util.toStringPercent`.
- `tags : {string}`: Tags of the mod (e.g. `"Curated"`), matched against the filter tags.
- `hasDependencies : boolean`: Whether the mod depends on other mods; the mod manager then fetches `ModDetailsData.dependencies` to find missing ones.
- `publishedUnixTimestamp : integer`: Publication date as a Unix timestamp.
- `updatedUnixTimestamp : integer`: Date of the last update as a Unix timestamp.
- `releaseInfo : ModReleaseInfo`: Information about the current release.

### Modhub.ModMediaType (record)

```teal
record ModMediaType
```
Access: `api.type.modhub.ModMediaType`

Kind of image requested with `Modhub.GetModMediaRequest`.

- `Logo : ModMediaType`
    - used 6x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:306`: `request = api.type.modhub.GetModMediaRequest.new(modInfo.modId, api.type.modhub.ModMediaType.Logo, -1)`
- `GalleryThumb180p : ModMediaType`: Gallery image as a small (180p) thumbnail.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2496`: `local smallParams = adjustImageParams(index, api.type.modhub.ModMediaType.GalleryThumb180p)`
- `GalleryThumb720p : ModMediaType`: Gallery image as a 720p thumbnail.
- `GalleryOriginal : ModMediaType`: Gallery image in original size.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2474`: `local bigParams = adjustImageParams(state:old().currentIndex, api.type.modhub.ModMediaType.GalleryOriginal)`
- `AuthorAvatar : ModMediaType`: Avatar image of the mod's author.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2170`: `request = api.type.modhub.GetModMediaRequest.new(params.modId, api.type.modhub.ModMediaType.AuthorAvatar, -1)`

### Modhub.AuthenticationType (record)

```teal
record AuthenticationType
```
Access: `api.type.modhub.AuthenticationType`

Login methods of a modhub backend, listed in `Capabilities.authenticationTypes`.

- `Email : AuthenticationType`: Login with an email address and an auth code sent by mail; the GUI labels it "Mod.io". It does not use the terms-of-use step.
    - used 5x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2761`: `elseif authType == api.type.modhub.AuthenticationType.Email then`
- `Steam : AuthenticationType`
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2763`: `elseif authType == api.type.modhub.AuthenticationType.Steam then`
- `SteamOffline : AuthenticationType`: Steam offline login; the base login dialog shows it disabled.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2765`: `elseif authType == api.type.modhub.AuthenticationType.SteamOffline then`
- `Gog : AuthenticationType`
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2767`: `elseif authType == api.type.modhub.AuthenticationType.Gog then`
- `Epic : AuthenticationType`
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2769`: `elseif authType == api.type.modhub.AuthenticationType.Epic then`
- `XboxLive : AuthenticationType`
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2771`: `elseif authType == api.type.modhub.AuthenticationType.XboxLive then`
- `PSN : AuthenticationType`
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2773`: `elseif authType == api.type.modhub.AuthenticationType.PSN then`
- `Paradox : AuthenticationType`: Paradox account login, shown as a separate login panel.
    - used 5x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2759`: `if authType == api.type.modhub.AuthenticationType.Paradox then`

### Modhub.RatingType (record)

```teal
record RatingType
```
Access: `api.type.modhub.RatingType`

A user's rating of a mod.

- `Neutral : RatingType`: No rating (a like or dislike was withdrawn).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1075`: `local newRating = value == 1 and ratingType or api.type.modhub.RatingType.Neutral`
- `Positive : RatingType`: Like.
    - used 5x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1011`: `local voteCount = buttonRating == api.type.modhub.RatingType.Positive`
- `Negative : RatingType`: Dislike.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1028`: `updateToggleButtonState(api.type.modhub.RatingType.Negative, newState)`

### Modhub.ReportType (record)

```teal
record ReportType
```
Access: `api.type.modhub.ReportType`

Reason for reporting a mod with `api.modhub.submitModReportAsync`.

- `Generic : ReportType`
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3315`: `reportType = api.type.modhub.ReportType.Generic,`
- `DMCA : ReportType`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3372`: `{ value = api.type.modhub.ReportType.DMCA, text = _("DMCA") },`
- `NotWorking : ReportType`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3373`: `{ value = api.type.modhub.ReportType.NotWorking, text = _("Not Working") },`
- `RudeContent : ReportType`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3374`: `{ value = api.type.modhub.ReportType.RudeContent, text = _("Rude Content") },`
- `IllegalContent : ReportType`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3375`: `{ value = api.type.modhub.ReportType.IllegalContent, text = _("Illegal Content") },`
- `StolenContent : ReportType`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3376`: `{ value = api.type.modhub.ReportType.StolenContent, text = _("Stolen Content") },`
- `FalseInformation : ReportType`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3377`: `{ value = api.type.modhub.ReportType.FalseInformation, text = _("False Information") },`
- `Other : ReportType`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3378`: `{ value = api.type.modhub.ReportType.Other, text = _("Other") }`

### Modhub.StorageInfo (record)

```teal
record StorageInfo
```
Access: `api.type.modhub.StorageInfo`

Storage used by installed mods, from `api.modhub.getStorageInfo(backendId)`.

- `usedSpace : integer`: Used storage in bytes.
- `totalSpace : integer`: Available storage in bytes; may be `nil` when there is no limit (the GUI then shows no fill level).

### Modhub.ModManagementState (record)

```teal
record ModManagementState
```
Access: `api.type.modhub.ModManagementState`

Progress of a backend's downloads or uploads, from `api.modhub.getModManagementState(backendId)`.

- `uploading : boolean`: Whether the running operations are uploads (otherwise downloads).
- `busy : boolean`: Whether downloads or uploads are running.
- `totalPending : integer`: Total number of mods in the current batch.
- `pending : {ModId}`: Mods still waiting to be processed.
- `finished : {ModId}`: Mods already processed in the current batch.
- `progress : number`: Progress of the current operation, shown in a progress bar.
- `currentModId : ModId`: Mod being processed right now, or `nil`.

### Modhub.Capabilities (record)

```teal
record Capabilities
```
Access: `api.type.modhub.Capabilities`

What a backend supports, from `api.modhub.getCapabilities(backendId)`. The GUI shows or hides features based on these flags.

- `allowForceUninstall : boolean`: Whether the force-uninstall button is shown (`api.modhub.forceUninstallModAsync`).
- `authenticationTypes : {AuthenticationType}`: Login methods offered; the GUI only shows a login button when the list is not empty.
- `manualRefresh : boolean`: Whether a manual Refresh button is offered (`api.modhub.refreshAsync`).
- `specialModRefresh : boolean`: Whether `api.modhub.specialRefreshModAsync` is used to retry a mod that failed for lack of space.
- `allowReport : boolean`: Whether mods can be reported.
- `isInfoOnly : boolean`: True for a local, information-only backend (such as the staging area) that cannot subscribe or install mods.
- `allowUnsubscribeInstalledMods : boolean`: Whether installed mods may be unsubscribed.
- `allowRestrictedMode : boolean`: Whether `ModManagementMode.Restricted` is allowed; otherwise the main menu uses `Disabled` instead.
- `allowModUpload : boolean`: Whether mods (and shared savegames) can be uploaded.
- `visibleInModHub : boolean`: Whether the backend gets a tab in the mod manager.

### Modhub.Description (record)

```teal
record Description
```
Access: `api.type.modhub.Description`

Name and icon of a backend, from `api.modhub.getDescription(backendId)`.

- `name : string`: Internal name of the backend, e.g. `"mod.io"`; matches `ModInfo.modSource`.
- `nameLocalized : string`: Translated name, used as the tab title.
- `url : string`: URL of the backend's web site.
- `icon : string`: Icon image path of the backend.

### Modhub.EmailAuthCodeRequest (record)

```teal
record EmailAuthCodeRequest
```
Access: `api.type.modhub.EmailAuthCodeRequest`

Request for `api.modhub.requestEmailAuthCodeAsync`: step 1 of the email login, which asks the backend to mail an auth code.

- `new : function(string) : EmailAuthCodeRequest`  (call as `api.type.modhub.EmailAuthCodeRequest.new(…)`): Creates the request.
    - param `#1`: Email address of the user.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2854`: `local request = api.type.modhub.EmailAuthCodeRequest.new(localEmail)`
- `email : string`: Email address of the user.

### Modhub.EmailAuthCodeData (record)

```teal
record EmailAuthCodeData
```
Access: `api.type.modhub.EmailAuthCodeData`

Result data of `api.modhub.requestEmailAuthCodeAsync`.

- `response : Response`: Outcome of the request.

#### Modhub.EmailAuthCodeData.Response (record)

```teal
record Response
```
Access: `api.type.modhub.EmailAuthCodeData.Response`

Outcome of an auth code request.

- `ValidationError : Response`: The email address was rejected.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2860`: `elseif result:getData().response == api.type.modhub.EmailAuthCodeData.Response.ValidationError then`
- `NeedAuthCode : Response`: A code was sent; continue with `api.modhub.authenticateUserEmailAsync`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2864`: `elseif result:getData().response == api.type.modhub.EmailAuthCodeData.Response.NeedAuthCode then`
- `Authenticated : Response`: The user is logged in.

### Modhub.AuthenticateUserEmailRequest (record)

```teal
record AuthenticateUserEmailRequest
```
Access: `api.type.modhub.AuthenticateUserEmailRequest`

Request for `api.modhub.authenticateUserEmailAsync`: step 2 of the email login, which sends the auth code from the mail.

- `new : function(string) : AuthenticateUserEmailRequest`  (call as `api.type.modhub.AuthenticateUserEmailRequest.new(…)`): Creates the request.
    - param `#1`: The auth code.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2881`: `local request = api.type.modhub.AuthenticateUserEmailRequest.new(localAuthCode)`
- `authCode : string`: The auth code from the mail.

### Modhub.AuthenticateUserEmailData (record)

```teal
record AuthenticateUserEmailData
```
Access: `api.type.modhub.AuthenticateUserEmailData`

Result data of `api.modhub.authenticateUserEmailAsync`.

- `response : Response`: Outcome of the authentication.

#### Modhub.AuthenticateUserEmailData.Response (record)

```teal
record Response
```
Access: `api.type.modhub.AuthenticateUserEmailData.Response`

Outcome of an email authentication.

- `ValidationError : Response`: The auth code was rejected.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2886`: `elseif result:getData().response == api.type.modhub.AuthenticateUserEmailData.Response.ValidationError then`
- `ExpiredAuthCode : Response`: The auth code has expired; a new one must be requested.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2891`: `elseif result:getData().response == api.type.modhub.AuthenticateUserEmailData.Response.ExpiredAuthCode then`
- `Authenticated : Response`: The user is logged in.

### Modhub.TermsOfUseRequest (record)

```teal
record TermsOfUseRequest
```
Access: `api.type.modhub.TermsOfUseRequest`

Request for `api.modhub.getTermsOfUseAsync`, sent before an external (non-email) login.

- `new : function(AuthenticationType) : TermsOfUseRequest`  (call as `api.type.modhub.TermsOfUseRequest.new(…)`): Creates the request.
    - param `#1`: Login method the terms are needed for.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3096`: `local request = api.type.modhub.TermsOfUseRequest.new(authType)`
- `type : AuthenticationType`: Login method the terms are needed for.

### Modhub.TermsOfUseData (record)

```teal
record TermsOfUseData
```
Access: `api.type.modhub.TermsOfUseData`

Terms of use the user must accept before an external login.

- `terms : string`: Text of the terms; when `nil` the GUI logs in without asking.
- `agreeLabel : string`: Label of the accept button.
- `disagreeLabel : string`: Label of the decline button.

### Modhub.AuthenticateUserExternalRequest (record)

```teal
record AuthenticateUserExternalRequest
```
Access: `api.type.modhub.AuthenticateUserExternalRequest`

Request for `api.modhub.authenticateUserExternalAsync`: login through a platform account (Steam, GOG, Epic, console, Paradox).

- `new : function(AuthenticationType) : AuthenticateUserExternalRequest`  (call as `api.type.modhub.AuthenticateUserExternalRequest.new(…)`): Creates the request.
    - param `#1`: Login method.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3064`: `local request = api.type.modhub.AuthenticateUserExternalRequest.new(authType)`
- `type : AuthenticationType`: Login method.
- `email : string`: optional

### Modhub.AuthenticateUserExternalData (record)

```teal
record AuthenticateUserExternalData
```
Access: `api.type.modhub.AuthenticateUserExternalData`

Result data of `api.modhub.authenticateUserExternalAsync`.

- `response : Response`: Outcome of the login.

#### Modhub.AuthenticateUserExternalData.Response (record)

```teal
record Response
```
Access: `api.type.modhub.AuthenticateUserExternalData.Response`

Outcome of an external login.

- `UnkownError : Response`
- `PortalSignInError : Response`: Login through the platform failed; the GUI asks the user to check that they are logged in to the platform.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3069`: `elseif result:getData().response == api.type.modhub.AuthenticateUserExternalData.Response.PortalSignInError then`
- `ValidationError : Response`: The login was rejected.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3085`: `elseif result:getData().response == api.type.modhub.AuthenticateUserExternalData.Response.ValidationError then`
- `Authenticated : Response`: The user is logged in.

### Modhub.LogoutData (record)

```teal
record LogoutData
```
Access: `api.type.modhub.LogoutData`

Result data of `api.modhub.logoutAsync` (no fields).

### Modhub.ListModsRequest (record)

```teal
record ListModsRequest
```
Access: `api.type.modhub.ListModsRequest`

Request for `api.modhub.listModsAsync`: one page of a mod search.

- `new : function() : ListModsRequest`  (call as `api.type.modhub.ListModsRequest.new(…)`): Creates a new, empty `ListModsRequest`.
- `page : integer`: Page to fetch.
- `preferedPageSize : integer`: Preferred number of mods per page.
- `searchString : string`: Search text.
- `sortType : SortType`: Sort order.
- `sortAscending : boolean`: Whether to sort ascending.
- `tags : {integer}`
- `fetchLongDescription : boolean`: Whether to include the long description in the results.

### Modhub.ListModsData (record)

```teal
record ListModsData
```
Access: `api.type.modhub.ListModsData`

Result data of `api.modhub.listModsAsync`.

- `modInfos : {ModInfo}`: Mods on the requested page.
- `modsPerPage : integer`: Number of mods per page.
- `totalResultCount : integer`: Total number of mods matching the search.

### Modhub.GetModDetailsRequest (record)

```teal
record GetModDetailsRequest
```
Access: `api.type.modhub.GetModDetailsRequest`

Request for `api.modhub.getModDetailsAsync`.

- `new : function(ModId) : GetModDetailsRequest`  (call as `api.type.modhub.GetModDetailsRequest.new(…)`): Creates the request.
    - param `#1`: Id of the mod.
    - used 4x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:715`: `local request = api.type.modhub.GetModDetailsRequest.new(params.modId)`
- `modId : ModId`: Id of the mod.

### Modhub.UserPortal (record)

```teal
record UserPortal
```
Access: `api.type.modhub.UserPortal`

Platform account type of a mod author (`ModDetailsData.userPortal`).

- `XboxLive : UserPortal`
- `PlaystationNetwork : UserPortal`
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2168`: `imagePath = params.userPortal == api.type.modhub.UserPortal.PlaystationNetwork and "::/gui/menu/images/user_psn.tga" or "::/gui/menu/images/user_xboxlive.tga"`
- `Misc : UserPortal`: Any other account; the GUI then loads the author avatar from the backend instead of a platform image.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2167`: `if params.userPortal ~= nil and params.userPortal ~= api.type.modhub.UserPortal.Misc then`

### Modhub.ModDetailsData (record)

```teal
record ModDetailsData
```
Access: `api.type.modhub.ModDetailsData`

Full details of a mod, the result data of `api.modhub.getModDetailsAsync`.

- `new : function() : ModDetailsData`  (call as `api.type.modhub.ModDetailsData.new(…)`): Creates a new, empty `ModDetailsData`.
- `modInfo : ModInfo`: Summary information of the mod.
- `author : string`: Name of the author.
- `userId : UserId`: User id of the author; the mod manager uses it to list the author's other mods.
- `userPortal : UserPortal`: Platform account type of the author.
- `numGalleryImages : integer`: Number of gallery images; fetch them with `GetModMediaRequest` and indices `0` to `numGalleryImages - 1`.
- `logoImage : string`
- `galleryImages : {string}`
- `dependencies : {{ModId, string}}`: Dependencies of the mod as `{modId, name}` pairs.
- `found : boolean`: Whether the mod exists in the backend.

### Modhub.GetModMediaRequest (record)

```teal
record GetModMediaRequest
```
Access: `api.type.modhub.GetModMediaRequest`

Request for `api.modhub.getModMediaAsync`: loads one image of a mod.

- `new : function(ModId, ModMediaType, integer) : GetModMediaRequest`  (call as `api.type.modhub.GetModMediaRequest.new(…)`): Creates the request.
    - param `#1`: Id of the mod.
    - param `#2`: Kind of image.
    - param `#3`: Gallery index (0-based) for gallery images.

    ```lua
    local request = api.type.modhub.GetModMediaRequest.new(modId, api.type.modhub.ModMediaType.GalleryOriginal, 0)
    api.modhub.getModMediaAsync(backendId, request, function(result)
      if result:isSuccess() then
        local data = result:getData()
        print(data.width, data.height)
      end
    end)
    ```

    - used 7x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:306`: `request = api.type.modhub.GetModMediaRequest.new(modInfo.modId, api.type.modhub.ModMediaType.Logo, -1)`
- `modId : ModId`: Id of the mod.
- `type : ModMediaType`: Kind of image.
- `galleryIndex : integer`: Gallery index (0-based) for gallery images.

### Modhub.ModMediaData (record)

```teal
record ModMediaData
```
Access: `api.type.modhub.ModMediaData`

Image data, the result data of `api.modhub.getModMediaAsync`.

Notes:

The GUI shows it with `builtin.ImageView{ data_native = data.data_native, size = api.type.Vec2i.new(data.width, data.height) }`.

- `new : function() : ModMediaData`  (call as `api.type.modhub.ModMediaData.new(…)`): Creates a new, empty `ModMediaData`.
- `width : integer`: Image width in pixels.
- `height : integer`: Image height in pixels.
- `channels : integer`: Number of color channels.
- `data : {integer}`: Pixel data as a Lua table.
- `data_native : Vector<integer>`: Pixel data as a native vector, as passed to `ImageView.data_native`.

### Modhub.GetModRatingRequest (record)

```teal
record GetModRatingRequest
```
Access: `api.type.modhub.GetModRatingRequest`

Request for `api.modhub.getModRatingAsync`: the current user's rating of a mod.

- `new : function(ModId) : GetModRatingRequest`  (call as `api.type.modhub.GetModRatingRequest.new(…)`): Creates the request.
    - param `#1`: Id of the mod.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1036`: `local request = api.type.modhub.GetModRatingRequest.new(params.modInfo.modId)`
- `modId : ModId`: Id of the mod.

### Modhub.ModRatingData (record)

```teal
record ModRatingData
```
Access: `api.type.modhub.ModRatingData`

Result data of `api.modhub.getModRatingAsync`.

- `new : function() : ModRatingData`  (call as `api.type.modhub.ModRatingData.new(…)`): Creates a new, empty `ModRatingData`.
- `rating : RatingType`: The current user's rating of the mod.

### Modhub.SubscribeModRequest (record)

```teal
record SubscribeModRequest
```
Access: `api.type.modhub.SubscribeModRequest`

Request for `api.modhub.subscribeModAsync`.

- `new : function(ModId) : SubscribeModRequest`  (call as `api.type.modhub.SubscribeModRequest.new(…)`): Creates the request.
    - param `#1`: Id of the mod.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:898`: `async = api.modhub.subscribeModAsync(params.context.backendId, api.type.modhub.SubscribeModRequest.new(params.modId), function(result : Modhub.AsyncResult<Modhu`
- `modId : ModId`: Id of the mod to subscribe to.

### Modhub.UnsubscribeModRequest (record)

```teal
record UnsubscribeModRequest
```
Access: `api.type.modhub.UnsubscribeModRequest`

Request for `api.modhub.unsubscribeModAsync`.

- `new : function(ModId) : UnsubscribeModRequest`  (call as `api.type.modhub.UnsubscribeModRequest.new(…)`): Creates the request.
    - param `#1`: Id of the mod.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:837`: `async = api.modhub.unsubscribeModAsync(params.context.backendId, api.type.modhub.UnsubscribeModRequest.new(params.modId), function(result : Modhub.AsyncResult<M`
- `modId : ModId`: Id of the mod to unsubscribe from.

### Modhub.SubscriptionData (record)

```teal
record SubscriptionData
```
Access: `api.type.modhub.SubscriptionData`

Result data of `api.modhub.subscribeModAsync` and `api.modhub.unsubscribeModAsync` (no fields).

### Modhub.RefreshData (record)

```teal
record RefreshData
```
Access: `api.type.modhub.RefreshData`

Result data of `api.modhub.refreshAsync` and `Modhub.ModPublishHelper.refresh`.

- `numRefreshed : integer`: Number of mods that were refreshed.

### Modhub.ForceUninstallModRequest (record)

```teal
record ForceUninstallModRequest
```
Access: `api.type.modhub.ForceUninstallModRequest`

Request for `api.modhub.forceUninstallModAsync`.

- `new : function(ModId) : ForceUninstallModRequest`  (call as `api.type.modhub.ForceUninstallModRequest.new(…)`): Creates the request.
    - param `#1`: Id of the mod.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3604`: `local request = api.type.modhub.ForceUninstallModRequest.new(params.modId)`
- `modId : ModId`: Id of the mod to uninstall.

### Modhub.ForceUninstallModData (record)

```teal
record ForceUninstallModData
```
Access: `api.type.modhub.ForceUninstallModData`

Result data of `api.modhub.forceUninstallModAsync` (no fields).

### Modhub.SpecialRefreshModRequest (record)

```teal
record SpecialRefreshModRequest
```
Access: `api.type.modhub.SpecialRefreshModRequest`

Request for `api.modhub.specialRefreshModAsync`, which the mod manager uses to retry (or drop) a mod that could not be installed for lack of space.

- `new : function(ModId, boolean) : SpecialRefreshModRequest`  (call as `api.type.modhub.SpecialRefreshModRequest.new(…)`): Creates the request.
    - param `#1`: Id of the mod.
    - param `#2`: Whether to unsubscribe from the mod.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:827`: `async = api.modhub.specialRefreshModAsync(params.context.backendId, api.type.modhub.SpecialRefreshModRequest.new(params.modId, unsubscribe), function(result : M`
- `modId : ModId`: Id of the mod.
- `unsubscribe : boolean`: Whether to unsubscribe from the mod instead of retrying.

### Modhub.SpecialRefreshModData (record)

```teal
record SpecialRefreshModData
```
Access: `api.type.modhub.SpecialRefreshModData`

Result data of `api.modhub.specialRefreshModAsync` (no fields).

### Modhub.RestartModBackendData (record)

```teal
record RestartModBackendData
```
Access: `api.type.modhub.RestartModBackendData`

Result data of `api.modhub.restartModBackendAsync` (no fields); the mod manager calls it from the Retry Download button.

### Modhub.SubmitModRatingRequest (record)

```teal
record SubmitModRatingRequest
```
Access: `api.type.modhub.SubmitModRatingRequest`

Request for `api.modhub.submitModRatingAsync`.

- `new : function(ModId, RatingType) : SubmitModRatingRequest`  (call as `api.type.modhub.SubmitModRatingRequest.new(…)`): Creates the request.
    - param `#1`: Id of the mod.
    - param `#2`: The new rating; `Neutral` withdraws a like or dislike.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1082`: `local request = api.type.modhub.SubmitModRatingRequest.new(params.modInfo.modId, newRating)`
- `modId : ModId`: Id of the mod.
- `rating : RatingType`: The new rating.

### Modhub.SubmitModRatingData (record)

```teal
record SubmitModRatingData
```
Access: `api.type.modhub.SubmitModRatingData`

Result data of `api.modhub.submitModRatingAsync`.

- `rating : RatingType`: The rating stored by the backend.

### Modhub.SubmitModReportRequest (record)

```teal
record SubmitModReportRequest
```
Access: `api.type.modhub.SubmitModReportRequest`

Request for `api.modhub.submitModReportAsync`: reports a mod to the backend.

- `new : function(ModId, ReportType, string, ?string, ?string) : SubmitModReportRequest`  (call as `api.type.modhub.SubmitModReportRequest.new(…)`): Creates the request.
    - param `#1`: Id of the mod.
    - param `#2`: Reason of the report.
    - param `#3`: Report text (required by the base dialog).
    - param `#4`: Optional name of the reporter.
    - param `#5`: Optional contact (email) of the reporter.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3338`: `local request = api.type.modhub.SubmitModReportRequest.new(params.modId, state.reportType, state.comment,  state.name, state.email)`
- `modId : ModId`: Id of the reported mod.
- `type : ReportType`: Reason of the report.
- `reportDescription : string`: Report text.
- `reporterName : string`: Name of the reporter.
- `reporterContact : string`: Contact (email) of the reporter.

### Modhub.SubmitModReportData (record)

```teal
record SubmitModReportData
```
Access: `api.type.modhub.SubmitModReportData`

Result data of `api.modhub.submitModReportAsync`.

- `response : Response`: Outcome of the report.

#### Modhub.SubmitModReportData.Response (record)

```teal
record Response
```
Access: `api.type.modhub.SubmitModReportData.Response`

Outcome of a report.

- `ValidationError : Response`: The report was rejected; the input should be checked.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3352`: `elseif (result:getData().response == api.type.modhub.SubmitModReportData.Response.ValidationError) then`
- `Submitted : Response`: The report was submitted.

### Modhub.PartialUploadError (record)

```teal
record PartialUploadError
```
Access: `api.type.modhub.PartialUploadError`

Error of one step of an otherwise successful upload (`UploadModData.partialErrors`).

- `step : string`: Name of the upload step that failed.
- `error : ModHubErrorCode`: The error of that step.

### Modhub.UploadModData (record)

```teal
record UploadModData
```
Access: `api.type.modhub.UploadModData`

Result data of `Modhub.ModPublishHelper.publish`.

- `modId : ModId`: Remote id of the published mod (e.g. its mod.io id).
- `partialErrors : {PartialUploadError}`: Steps that failed although the upload as a whole succeeded; the GUI lists them to the user.

### Modhub.UniquePendingRequestId (record)

```teal
record UniquePendingRequestId
```
Access: `api.type.modhub.UniquePendingRequestId`

Handle of a running asynchronous modhub request, returned by the `api.modhub.*Async` functions.

- `abort : function(UniquePendingRequestId)`  (call as `api.type.modhub.UniquePendingRequestId.abort(…)`): Cancels the request; its callback is then not needed any more.
    - notes: The mod manager keeps the handle in a React ref and aborts it when the component unmounts or when a newer request replaces it (`mod_manager_react_util.setAsyncRef`).

### Modhub.AsyncResult (record)

```teal
record AsyncResult<T>
```
Access: `api.type.modhub.AsyncResult`

Result passed to the callback of an asynchronous modhub request. `T` is the request's data type.

Example:
```lua
api.modhub.refreshAsync(backendId, function(result)
  if not result:isSuccess() then
    print(result:getError().code)
  else
    print(result:getData().numRefreshed)
  end
end)
```

- `isSuccess : function(AsyncResult<T>) : boolean`  (call as `asyncResult:isSuccess(…) (method)`): Returns whether the request succeeded.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:315`: `if not result:isSuccess() then`
- `getData : function(AsyncResult<T>) : T`  (call as `api.type.modhub.AsyncResult.getData(…)`): Returns the result data; use it only when `isSuccess()` is true.
- `getError : function(AsyncResult<T>) : ModHubErrorCode`  (call as `asyncResult:getError(…) (method)`): Returns the error of a failed request.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:316`: `params.selectorApi.showError(_("Failed To Subscribe"), result:getError())`

### Modhub.ModBrowserDisplayData (record)

```teal
record ModBrowserDisplayData
```
Access: `api.type.modhub.ModBrowserDisplayData`

Current page of a mod browser, from `Modhub.ModBrowserDataProvider.getDisplayData`.

- `entries : {Entry}`: nil: is currently loading, empty: no results
- `error : Modhub.ModHubErrorCode`: nil: no error, otherwise error should be shown to user
- `currentPage : integer`: Current page (0-based); the GUI shows a Show More button while `currentPage < numPages - 1`.
- `numPages : integer`: Number of pages.
- `displayPerPage : integer`: Number of mods per page.
- `totalResults : integer`: Total number of matching mods.
- `appendResults : boolean`
- `searchString : string`: Current search text.
- `sortType : Modhub.SortType`: Current sort order.
- `hasNoSortTag : boolean`
- `filterTags : {string}`: Tags currently toggled as filters.
- `excludeTags : {string}`: Tags whose mods are excluded; the mod selector hides installed mods carrying any of them.

#### Modhub.ModBrowserDisplayData.Entry (record)

```teal
record Entry
```
Access: `api.type.modhub.ModBrowserDisplayData.Entry`

One mod in the browser list.

- `backendId : BackendId`: Backend the mod comes from.
- `modInfo : ModInfo`: Summary information of the mod.
- `modId : Mod.ModId`: Id of the mod in the local mod repository.

### Modhub.ModBrowserDataProvider (record)

```teal
record ModBrowserDataProvider
```
Access: `api.type.modhub.ModBrowserDataProvider`

Paged, sortable and filterable mod list behind a mod browser tab or the mod selector. Poll `getDisplayData()` every step to render it.

Example:
```lua
local provider = api.type.modhub.ModBrowserDataProvider.new(backendId, 12)
provider:setSearchString("station")
-- later, e.g. in react.onStep:
local displayData = provider:getDisplayData()
if displayData.entries ~= nil then print(#displayData.entries) end
```

- `new : function(BackendId, integer) : ModBrowserDataProvider`  (call as `api.type.modhub.ModBrowserDataProvider.new(…)`): Creates a provider that lists the mods of a modhub backend.
    - param `#1`: Backend to list.
    - param `#2`: Number of mods per page.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_page.tl:220`: `dataProviders[tabKey]:set(api.type.modhub.ModBrowserDataProvider.new(backendId, mod_manager_react_util.DisplayPerPage))`
- `newForSelector : function(integer, function(mod1 : Mod.ModId, mod2 : Mod.ModId), gameModDescs : {Mod.GameModDesc}) : ModBrowserDataProvider`  (call as `api.type.modhub.ModBrowserDataProvider.newForSelector(…)`): Creates a provider for the mod selector, listing local mods.
    - param `#1`: Number of mods per page.
    - param `#2`: Custom sort function for `SortType.Custom`; the base mod selector returns `true` if `mod1` comes before `mod2` and `nil` when it has no preference.
    - param `gameModDescs`: Mods of the savegame, so that missing ones can be listed.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:754`: `modSelectorProvider:set(api.type.modhub.ModBrowserDataProvider.newForSelector(mod_manager_react_util.DisplayPerPage, customSortFn,`
- `suspend : function(ModBrowserDataProvider)`  (call as `api.type.modhub.ModBrowserDataProvider.suspend(…)`): Pauses the provider, e.g. when its tab is hidden or the page unmounts.
- `resume : function(ModBrowserDataProvider)`  (call as `api.type.modhub.ModBrowserDataProvider.resume(…)`): Resumes a suspended provider.
- `getDisplayData : function(ModBrowserDataProvider) : ModBrowserDisplayData`  (call as `api.type.modhub.ModBrowserDataProvider.getDisplayData(…)`): Returns the current page and state of the list.
- `setLoadPage : function(ModBrowserDataProvider, integer)`  (call as `api.type.modhub.ModBrowserDataProvider.setLoadPage(…)`): Requests that the given page be loaded; the mod browser passes the next page when the list is scrolled to the end, else 0.
    - param `#2`: Page to load (0-based).
- `setNextContinuousPage : function(ModBrowserDataProvider)`  (call as `api.type.modhub.ModBrowserDataProvider.setNextContinuousPage(…)`): Appends the next page to the list (Show More button).
- `setCurrentPage : function(ModBrowserDataProvider, integer)`  (call as `api.type.modhub.ModBrowserDataProvider.setCurrentPage(…)`): Jumps to a page.
    - param `#2`: Page number (0-based).
- `setSearchString : function(ModBrowserDataProvider, string)`  (call as `api.type.modhub.ModBrowserDataProvider.setSearchString(…)`): Sets the search text.
    - param `#2`: Search text.
- `setSearchUserId : function(ModBrowserDataProvider, UserId)`  (call as `api.type.modhub.ModBrowserDataProvider.setSearchUserId(…)`): Lists only the mods of one author; an invalid id (`api.type.modhub.UserId.new()`) clears the author filter.
    - param `#2`: Author's user id.
- `setNextPage : function(ModBrowserDataProvider)`  (call as `api.type.modhub.ModBrowserDataProvider.setNextPage(…)`): Goes to the next page.
- `setPreviousPage : function(ModBrowserDataProvider)`  (call as `api.type.modhub.ModBrowserDataProvider.setPreviousPage(…)`): Goes to the previous page.
- `refreshCurrentPage : function(ModBrowserDataProvider, boolean)`  (call as `api.type.modhub.ModBrowserDataProvider.refreshCurrentPage(…)`): Reloads the current page, e.g. after an error or a change of the custom sort order.
- `getSortType : function(ModBrowserDataProvider) : Modhub.SortType`  (call as `api.type.modhub.ModBrowserDataProvider.getSortType(…)`): Returns the current sort order.
- `getTagToggled : function(ModBrowserDataProvider, string) : boolean`  (call as `api.type.modhub.ModBrowserDataProvider.getTagToggled(…)`): Returns whether a filter tag is toggled on.
    - param `#2`: The tag.
- `setSortType : function(ModBrowserDataProvider, SortType)`  (call as `api.type.modhub.ModBrowserDataProvider.setSortType(…)`): Sets the sort order.
    - param `#2`: The sort order.
- `setNextSortType : function(ModBrowserDataProvider)`  (call as `api.type.modhub.ModBrowserDataProvider.setNextSortType(…)`): Switches to the next available sort order.
- `toggleTag : function(ModBrowserDataProvider, string)`  (call as `api.type.modhub.ModBrowserDataProvider.toggleTag(…)`): Toggles a filter tag on or off.
    - param `#2`: The tag.
- `resetFilters : function(ModBrowserDataProvider)`  (call as `api.type.modhub.ModBrowserDataProvider.resetFilters(…)`): Clears all filter tags.
- `getSortAndFilterOptions : function(ModBrowserDataProvider) : Modhub.SortAndFilterOptions`  (call as `api.type.modhub.ModBrowserDataProvider.getSortAndFilterOptions(…)`): Returns the sort options and filter tag groups available for this list.

### Modhub.StagingModData (record)

```teal
record StagingModData
```
Access: `api.type.modhub.StagingModData`

Publishing state of a mod in the staging area, from `Modhub.ModPublishHelper.getStagingModData`.

- `equals : function(StagingModData, StagingModData) : boolean`  (call as `api.type.modhub.StagingModData.equals(…)`): Returns `true` when both `StagingModData` values are equal.
- `localModhubDetails : Modhub.ModDetailsData`: Locally edited details (title, tags, ...) of the mod; edit a copy and pass it to `ModPublishHelper.setModDetails`.
- `state : State`: Current publishing state.
- `modhubId : Modhub.ModId`: Remote id of the mod; invalid if it was never published.
- `modhubDetails : Modhub.ModDetailsData`: Details of the published mod in the backend, or `nil` if it does not exist (any more).
- `publishResult : AsyncResult<UploadModData>`: Result of the last upload, set in state `HasResult`.

#### Modhub.StagingModData.State (record)

```teal
record State
```
Access: `api.type.modhub.StagingModData.State`

Publishing state of a staging mod.

- `Sync : State`: Still synchronising with the backend; publishing is not possible yet.
    - used 8x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1416`: `state = api.type.modhub.StagingModData.State.Sync,`
- `Ready : State`: Ready to be published or edited.
    - used 20x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:273`: `if data.state == api.type.modhub.StagingModData.State.Ready and files and #files > 0 then`
- `Publishing : State`: An upload is running.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1569`: `elseif state:old().state == api.type.modhub.StagingModData.State.Publishing then`
- `HasResult : State`: The upload finished; see `publishResult`.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1426`: `if old ~= nil and old.state ~= data.state and data.state == api.type.modhub.StagingModData.State.HasResult then`

### Modhub.ValidationData (record)

```teal
record ValidationData
```
Access: `api.type.modhub.ValidationData`

State of the validation of a single staging mod, see ModPublishHelper.validate

- `equals : function(ValidationData, ValidationData) : boolean`  (call as `api.type.modhub.ValidationData.equals(…)`): Returns `true` when both `ValidationData` values are equal.
- `validating : boolean`: Whether a validation started with ModPublishHelper.validate is still running
- `progress : number`: Progress of the running validation, in [0, 1]
- `task : string`: Name of the task the running validation is currently busy with
- `result : Modhub.ModValidatorResult`: Result of the last completed validation, nil if the mod wasn't validated yet

### Modhub.PublishSettings (record)

```teal
record PublishSettings
```
Access: `api.type.modhub.PublishSettings`

Options for `Modhub.ModPublishHelper.publish`. Create one with `new()` or copy one with `new(settings)`.

- `new : function() : PublishSettings`  (call as `api.type.modhub.PublishSettings.new(…)`): Creates a new, empty `PublishSettings`.
    - used 7x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1405`: `local publishSettings : ReactStateT<Modhub.PublishSettings> = react.useState(api.type.modhub.PublishSettings.new())`
- `new : function(PublishSettings) : PublishSettings`  (call as `api.type.modhub.PublishSettings.new(…)`): Creates a new, empty `PublishSettings`.
    - used 7x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1405`: `local publishSettings : ReactStateT<Modhub.PublishSettings> = react.useState(api.type.modhub.PublishSettings.new())`
- `changeLog : string`: Change log text of this upload.
- `updateMetadata : boolean`: Whether to update the mod's metadata (title, description, images, tags) in the backend.
- `setPublic : boolean`: Whether to make the mod public.
- `isSavegame : boolean`: Shared savegames are uploaded as staged, they are not validated or cooked
- `issuesConfirmed : boolean`: The user confirmed the issues the last validation reported, so publishing only aborts if the
validation it runs comes back worse than those
- `additionalMetadata : {string : any}`: Free-form extra metadata merged into the upload's metadata (eg. mapSize for savegames)

### Modhub.ModPublishHelper (record)

```teal
record ModPublishHelper
```
Access: `api.type.modhub.ModPublishHelper`

Manages mods in the local staging area and publishes them to a backend (validation, metadata, images, upload).

- `new : function(BackendId) : ModPublishHelper`  (call as `api.type.modhub.ModPublishHelper.new(…)`): Creates a publish helper for a backend.
    - param `#1`: Backend to publish to.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_page.tl:182`: `modPublishHelperRef:set(api.type.modhub.ModPublishHelper.new(publishBackendId))`
- `hasAnyStagingMods : function(publishHelper : ModPublishHelper) : boolean`  (call as `api.type.modhub.ModPublishHelper.hasAnyStagingMods(…)`): Checks if there are any mods in the staging area
    - param `publishHelper`: the publish helper
- `canRefresh : function(publishHelper : ModPublishHelper) : boolean`  (call as `api.type.modhub.ModPublishHelper.canRefresh(…)`): Checks if the publish helper can be refreshed
    - param `publishHelper`: the publish helper
- `refresh : function(publishHelper : ModPublishHelper) : AsyncResult<RefreshData>`  (call as `api.type.modhub.ModPublishHelper.refresh(…)`): Refreshes the mod helper, this is an async operation
    - param `publishHelper`: the publish helper
    - returns: the result of the async operation, including the number of mods that were refreshed
- `update : function(publishHelper : ModPublishHelper)`  (call as `api.type.modhub.ModPublishHelper.update(…)`): Polls the pending validations, has to be called regularly while a validation is running
    - param `publishHelper`: the publish helper
- `validate : function(publishHelper : ModPublishHelper, modId : Modhub.ModId)`  (call as `api.type.modhub.ModPublishHelper.validate(…)`): Validates a staging mod, this is an async operation. Aborts a validation that is already
running for that mod. The result ends up in ValidationData.result once
ValidationData.validating turned false again. Does not cook, only publish does.
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
- `abortValidation : function(publishHelper : ModPublishHelper, modId : Modhub.ModId)`  (call as `api.type.modhub.ModPublishHelper.abortValidation(…)`): Aborts a running validation
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
- `getValidationData : function(publishHelper : ModPublishHelper, modId : Modhub.ModId) : ValidationData`  (call as `api.type.modhub.ModPublishHelper.getValidationData(…)`): Returns the validation state of a staging mod
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
- `hasStagingMod : function(publishHelper : ModPublishHelper, modId : Modhub.ModId) : boolean`  (call as `modPublishHelper:hasStagingMod(…) (method)`): Checks if a specific mod is in the staging area
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:5573`: `if (modPublishHelper:hasStagingMod(params.modId) == false) then`
- `getStagingModData : function(publishHelper : ModPublishHelper, modId : Modhub.ModId) : StagingModData`  (call as `modPublishHelper:getStagingModData(…) (method)`): Returns the staging mod data for a specific mod
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:5579`: `local data = modPublishHelper:getStagingModData(params.modId)`
- `setModDetails : function(publishHelper : ModPublishHelper, modId : Modhub.ModId, modDetailsData : Modhub.ModDetailsData)`  (call as `api.type.modhub.ModPublishHelper.setModDetails(…)`): Sets the mod details data
    - param `publishHelper`: the publish helper
    - param `modId`: the local mod id of the mod to update
    - param `modDetailsData`: the mod details data
- `setRemoteModId : function(publishHelper : ModPublishHelper, localModId : Modhub.ModId, remoteModId : Modhub.ModId)`  (call as `api.type.modhub.ModPublishHelper.setRemoteModId(…)`): Update the local modId of a mod to the remote (eg. mod.io) mod.io of the mod when publishing
    - param `publishHelper`: the publish helper
    - param `localModId`: the local mod id of the mod to update
    - param `remoteModId`: the remote mod id to set
- `publish : function(publishHelper : ModPublishHelper, modId : Modhub.ModId, publishSettings : PublishSettings) : AsyncResult<UploadModData>`  (call as `api.type.modhub.ModPublishHelper.publish(…)`): Publishes a mod (eg. on mod.io), this is an async call.
Validates and cooks the mod once more first and uploads the cooked folders, unless the settings
mark it as a savegame; see PublishSettings.isSavegame and PublishSettings.issuesConfirmed
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
    - param `publishSettings`: the publish settings
    - returns: the result of the async call, also contains the remote mod id
- `abortPublish : function(publishHelper : ModPublishHelper, modId : Modhub.ModId) : UniquePendingRequestId`  (call as `api.type.modhub.ModPublishHelper.abortPublish(…)`): Aborts the publication of a mod
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
- `addLogoImage : function(publishHelper : ModPublishHelper, modId : Modhub.ModId, image : string)`  (call as `api.type.modhub.ModPublishHelper.addLogoImage(…)`): Adds a list of image to the mod gallery
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
    - param `image`: image file to add to the gallery
- `removeLogoImage : function(publishHelper : ModPublishHelper, modId : Modhub.ModId)`  (call as `api.type.modhub.ModPublishHelper.removeLogoImage(…)`): Removes the logo image
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
    - param `index`: the index of the image in the gallery to remove
- `addGalleryImages : function(publishHelper : ModPublishHelper, modId : Modhub.ModId, images : {string})`  (call as `api.type.modhub.ModPublishHelper.addGalleryImages(…)`): Adds a list of image to the mod gallery
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
    - param `images`: the list of image file to add to the gallery
- `removeGalleryImage : function(publishHelper : ModPublishHelper, modId : Modhub.ModId, index : integer)`  (call as `api.type.modhub.ModPublishHelper.removeGalleryImage(…)`): Removes an image from the mod gallery
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
    - param `index`: the index of the image in the gallery to remove
- `moveGalleryImage : function(publishHelper : ModPublishHelper, modId : Modhub.ModId, fromIndex : integer, toIndex : integer)`  (call as `api.type.modhub.ModPublishHelper.moveGalleryImage(…)`): Moves an image position in the mod gallery
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
    - param `fromIndex`: the index in the mod gallery of the image to move
    - param `toIndex`: the new position of the image in the mod gallery
- `setPostPublishFn : function(publishHelper : ModPublishHelper, modId : Modhub.ModId, publishFn : function(Modhub.ModId))`  (call as `modPublishHelper:setPostPublishFn(…) (method)`): Set a function called after a mod has been successfuly published
    - param `publishHelper`: the publish helper
    - param `modId`: the mod id
    - param `publishFn`: a function called after the mod has been published, the function gives the remote mod id as parameter
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:4011`: `modPublishHelper:setPostPublishFn(localModId, function(actualModhubId : Modhub.ModId)`
- `openFolder : function(publishHelper : ModPublishHelper, modId : Modhub.ModId)`  (call as `api.type.modhub.ModPublishHelper.openFolder(…)`): Opens the mod folder
    - param `publishHelper`: the publish helper
    - param `modId`: the modId
- `getSortAndFilterOptions : function(publishHelper : ModPublishHelper) : Modhub.SortAndFilterOptions`  (call as `api.type.modhub.ModPublishHelper.getSortAndFilterOptions(…)`): Gets the list of sorting and filtering options (eg. tags)
    - param `publishHelper`: the publish helper
    - returns: an object containing the list of sort and filter options
