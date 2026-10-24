# api.modhub

Definition file: `api/tealdef/api/modhub.d.tl` (in the game folder).

Contains all api definitions for the modhub interface.

## ModhubApi (record)

```teal
global record ModhubApi
```
Access: `api.modhub`

Main Modhub interface

Notes:

Every function except `getBackendIDs`, `getBackendIdForSource`, `getModHubModIdForModId`,
`stageSavegame`, `openPath` and `exportValidatorResult` takes the backend id first.
The `...Async` functions call their callback with a `Modhub.AsyncResult`
(`isSuccess()`, `getData()`, `getError()`) and return a `Modhub.UniquePendingRequestId`
whose `abort()` cancels the request. The base game uses this API only in the menus
(`gui/menu/mod_manager_react_util.tl`, `mod_selector_page.tl`, `main_menu.tl`,
`savegame_react_util.tl`, `main/pause_menu.tl`).

Example:
```lua
for _, backendId in ipairs(api.modhub.getBackendIDs()) do
  local caps = api.modhub.getCapabilities(backendId)
  local loggedIn = api.modhub.getUserInfo(backendId) ~= nil
  if not caps.isInfoOnly and loggedIn then
    api.modhub.subscribeModAsync(backendId, api.type.modhub.SubscribeModRequest.new(modId),
      function(result)
        if not result:isSuccess() then print(result:getError()) end
      end)
  end
end
```

- `getBackendIDs : function() : {Modhub.BackendId}`  (call as `api.modhub.getBackendIDs(…)`): Ids of all available mod hub backends.
    - used 9x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:281`: `for __, backendId in ipairs(api.modhub.getBackendIDs()) do`
- `getBackendIdForSource : function(string) : Modhub.BackendId`  (call as `api.modhub.getBackendIdForSource(…)`): Backend id for a mod source name (as returned by the mod repository's `getModSource`).
    - returns: The backend id; negative if no backend serves that source (the base game checks `>= 0`).
    - used 5x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:5694`: `local modhubBackendId = api.modhub.getBackendIdForSource(source)`
- `getUserInfo : function(Modhub.BackendId) : Modhub.UserInfo`  (call as `api.modhub.getUserInfo(…)`): Logged-in user of the backend.
    - returns: User name, display name and profile URL, or `nil` when no user is logged in (the base game uses `~= nil` as the logged-in test).
    - used 17x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:754`: `local loggedIn = api.modhub.getUserInfo(params.context.backendId) ~= nil`
- `getDescription : function(Modhub.BackendId) : Modhub.Description`  (call as `api.modhub.getDescription(…)`): Display information of the backend.
    - returns: Name, localized name, URL and icon of the backend.
    - used 5x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_page.tl:214`: `local desc = api.modhub.getDescription(backendId)`
- `getCapabilities : function(Modhub.BackendId) : Modhub.Capabilities`  (call as `api.modhub.getCapabilities(…)`): Features the backend supports (upload, report, force uninstall, restricted mode, manual refresh, ...).
    - notes: `isInfoOnly` marks a backend without mod management (the local mods list); the menus hide subscribe and management actions for it.
    - used 23x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:291`: `return api.modhub.getCapabilities(remoteModhub).allowRestrictedMode`
- `getSortAndFilterOptions : function(Modhub.BackendId) : Modhub.SortAndFilterOptions`  (call as `api.modhub.getSortAndFilterOptions(…)`): Sort types, sort orders and tag groups the backend offers for mod listings.
- `requestEmailAuthCodeAsync : function(Modhub.BackendId, Modhub.EmailAuthCodeRequest, function(Modhub.AsyncResult<Modhub.EmailAuthCodeData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.requestEmailAuthCodeAsync(…)`): Requests an authentication code by email (first step of the email login).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2855`: `mod_manager_react_util.setAsyncRef(asyncRef, api.modhub.requestEmailAuthCodeAsync(params.backendId, request, function(result : Modhub.AsyncResult<Modhub.EmailAu`
- `authenticateUserEmailAsync : function(Modhub.BackendId, Modhub.AuthenticateUserEmailRequest, function(Modhub.AsyncResult<Modhub.AuthenticateUserEmailData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.authenticateUserEmailAsync(…)`): Logs in with the code received by email (second step of the email login).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:2882`: `mod_manager_react_util.setAsyncRef(asyncRef, api.modhub.authenticateUserEmailAsync(params.backendId, request, function(result : Modhub.AsyncResult<Modhub.Authen`
- `getTermsOfUseAsync : function(Modhub.BackendId, Modhub.TermsOfUseRequest, function(Modhub.AsyncResult<Modhub.TermsOfUseData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.getTermsOfUseAsync(…)`): Fetches the terms of use that must be accepted before logging in with an external account type.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3097`: `mod_manager_react_util.setAsyncRef(asyncRef, api.modhub.getTermsOfUseAsync(params.backendId, request, function(result : Modhub.AsyncResult<Modhub.TermsOfUseData`
- `authenticateUserExternalAsync : function(Modhub.BackendId, Modhub.AuthenticateUserExternalRequest, function(Modhub.AsyncResult<Modhub.AuthenticateUserExternalData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.authenticateUserExternalAsync(…)`): Logs in with an external account (Steam, Steam offline, GOG, Epic, Xbox Live, PSN or Paradox; see `Modhub.AuthenticationType`).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3065`: `mod_manager_react_util.setAsyncRef(asyncRef, api.modhub.authenticateUserExternalAsync(params.backendId, request, function(result : Modhub.AsyncResult<Modhub.Aut`
- `logoutAsync : function(Modhub.BackendId, function(Modhub.AsyncResult<Modhub.LogoutData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.logoutAsync(…)`): Logs the current user out of the backend.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1262`: `mod_manager_react_util.setAsyncRef(asyncRef, api.modhub.logoutAsync(params.context.backendId, function(result : Modhub.AsyncResult<Modhub.LogoutData>)`
- `listModsAsync : function(Modhub.BackendId, Modhub.ListModsRequest, function(Modhub.AsyncResult<Modhub.ListModsData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.listModsAsync(…)`): Requests a list of mods from the backend.
- `getModDetailsAsync : function(Modhub.BackendId, Modhub.GetModDetailsRequest, function(Modhub.AsyncResult<Modhub.ModDetailsData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.getModDetailsAsync(…)`): Requests the details of a mod.
    - used 4x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:717`: `mod_manager_react_util.setAsyncRef(checkRef, api.modhub.getModDetailsAsync(params.context.backendId, request,`
- `getModMediaAsync : function(Modhub.BackendId, Modhub.GetModMediaRequest, function(Modhub.AsyncResult<Modhub.ModMediaData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.getModMediaAsync(…)`): Requests an image of a mod (logo, gallery image or author avatar, see `Modhub.ModMediaType`).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:160`: `local async = api.modhub.getModMediaAsync(params.context.backendId, request, function(result : Modhub.AsyncResult<Modhub.ModMediaData>)`
- `getModRatingAsync : function(Modhub.BackendId, Modhub.GetModRatingRequest, function(Modhub.AsyncResult<Modhub.ModRatingData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.getModRatingAsync(…)`): Requests the current user's rating of a mod.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1037`: `local async = api.modhub.getModRatingAsync(params.context.backendId, request, function(result : Modhub.AsyncResult<Modhub.ModRatingData>)`
- `subscribeModAsync : function(Modhub.BackendId, Modhub.SubscribeModRequest, function(Modhub.AsyncResult<Modhub.SubscriptionData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.subscribeModAsync(…)`): Subscribes the current user to a mod, which queues it for installation.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:898`: `async = api.modhub.subscribeModAsync(params.context.backendId, api.type.modhub.SubscribeModRequest.new(params.modId), function(result : Modhub.AsyncResult<Modhu`
- `unsubscribeModAsync : function(Modhub.BackendId, Modhub.UnsubscribeModRequest, function(Modhub.AsyncResult<Modhub.SubscriptionData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.unsubscribeModAsync(…)`): Unsubscribes the current user from a mod.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:837`: `async = api.modhub.unsubscribeModAsync(params.context.backendId, api.type.modhub.UnsubscribeModRequest.new(params.modId), function(result : Modhub.AsyncResult<M`
- `forceUninstallModAsync : function(Modhub.BackendId, Modhub.ForceUninstallModRequest, function(Modhub.AsyncResult<Modhub.ForceUninstallModData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.forceUninstallModAsync(…)`): Uninstalls a mod from the local disk (offered when the backend capability `allowForceUninstall` is set).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3606`: `mod_manager_react_util.setAsyncRef(asyncRef, api.modhub.forceUninstallModAsync(params.context.backendId, request,`
- `refreshAsync : function(Modhub.BackendId, function(Modhub.AsyncResult<Modhub.RefreshData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.refreshAsync(…)`): Refreshes the backend state (offered when the capability `manualRefresh` is set).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1318`: `mod_manager_react_util.setAsyncRef(asyncRef, api.modhub.refreshAsync(params.context.backendId, function(result : Modhub.AsyncResult<Modhub.RefreshData>)`
- `specialRefreshModAsync : function(Modhub.BackendId, Modhub.SpecialRefreshModRequest, function(Modhub.AsyncResult<Modhub.SpecialRefreshModData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.specialRefreshModAsync(…)`): Backend-specific refresh of one mod, used by the menus when an install runs out of space (capability `specialModRefresh`).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:827`: `async = api.modhub.specialRefreshModAsync(params.context.backendId, api.type.modhub.SpecialRefreshModRequest.new(params.modId, unsubscribe), function(result : M`
- `restartModBackendAsync : function(Modhub.BackendId, function(Modhub.AsyncResult<Modhub.RestartModBackendData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.restartModBackendAsync(…)`): Restarts the backend; the mod manager shows it as a "retry download" button when `shouldRetryInstall` is true.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:642`: `api.modhub.restartModBackendAsync(params.context.backendId,`
- `submitModRatingAsync : function(Modhub.BackendId, Modhub.SubmitModRatingRequest, function(Modhub.AsyncResult<Modhub.SubmitModRatingData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.submitModRatingAsync(…)`): Submits the current user's rating of a mod.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1083`: `local async = api.modhub.submitModRatingAsync(params.context.backendId, request, function(result : Modhub.AsyncResult<Modhub.SubmitModRatingData>)`
- `submitModReportAsync : function(Modhub.BackendId, Modhub.SubmitModReportRequest, function(Modhub.AsyncResult<Modhub.SubmitModReportData>)) : Modhub.UniquePendingRequestId`  (call as `api.modhub.submitModReportAsync(…)`): Reports a mod to the backend (capability `allowReport`).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3340`: `mod_manager_react_util.setAsyncRef(asyncRef, api.modhub.submitModReportAsync(params.context.backendId, request,`
- `setModManagementMode : function(Modhub.BackendId, Modhub.ModManagementMode)`  (call as `api.modhub.setModManagementMode(…)`): Sets how far the backend may install and remove mods right now.
    - notes: The main menu sets `Full` on pages such as Main, Mod Manager or Settings, `Restricted` while a game is being set up or loaded (New Game, Load Game, Campaign) and `Disabled` elsewhere; it applies the mode to every backend from `getBackendIDs()`.
    - used 2x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:282`: `api.modhub.setModManagementMode(backendId, mode)`
- `getModSubscriptionState : function(Modhub.BackendId, Modhub.ModId) : boolean`  (call as `api.modhub.getModSubscriptionState(…)`): Whether the current user is subscribed to the mod.
    - used 5x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:738`: `local subscribed = api.modhub.getModSubscriptionState(params.context.backendId, params.modId)`
- `getModInstallState : function(Modhub.BackendId, Modhub.ModId) : Modhub.InstallState`  (call as `api.modhub.getModInstallState(…)`): Install state of a mod (none, downloading, installed, pending, insufficient space, error, ...).
    - used 8x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:603`: `local installState = api.modhub.getModInstallState(params.context.backendId, modId)`
- `getInstalledMods : function(Modhub.BackendId) : {Modhub.ModId}`  (call as `api.modhub.getInstalledMods(…)`): Ids of the mods this backend has installed.
- `getModInstallInfo : function(Modhub.BackendId, Modhub.ModId) : Modhub.ModInstallInfo`  (call as `api.modhub.getModInstallInfo(…)`): Install path and runtime revision of an installed mod.
- `getModInfoForInstalledMod : function(Modhub.BackendId, Modhub.ModId) : Modhub.ModInfo`  (call as `api.modhub.getModInfoForInstalledMod(…)`): Mod information (title, summary, logo, ratings, release info, ...) of an installed mod.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:101`: `return api.modhub.getModInfoForInstalledMod(modhubBackendId, modHubModId)`
- `getModInfoForSubscribedMod : function(Modhub.BackendId, Modhub.ModId) : Modhub.ModInfo`  (call as `api.modhub.getModInfoForSubscribedMod(…)`): Mod information (title, summary, logo, ratings, release info, ...) of a subscribed mod.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:601`: `local modInfo = api.modhub.getModInfoForSubscribedMod(params.context.backendId, modId)`
- `getStorageInfo : function(Modhub.BackendId) : Modhub.StorageInfo`  (call as `api.modhub.getStorageInfo(…)`): Used and total storage space for mods (shown as a storage bar on consoles).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:5124`: `storageInfo = api.modhub.getStorageInfo(params.backendId)`
- `getModManagementState : function(Modhub.BackendId) : Modhub.ModManagementState`  (call as `api.modhub.getModManagementState(…)`): Current activity of the backend (busy, uploading, pending and finished mods, progress).
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/main_menu.tl:573`: `return remoteModhub ~= nil and api.modhub.getModManagementState(remoteModhub).busy`
- `isInitialized : function(Modhub.BackendId) : boolean`  (call as `api.modhub.isInitialized(…)`): Whether the backend has finished initialising.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:5177`: `if not api.modhub.isInitialized(backendId) then`
- `openExternalModPage : function(Modhub.BackendId, Modhub.ModId)`  (call as `api.modhub.openExternalModPage(…)`): Opens the mod's page on the mod hub website.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1452`: `api.modhub.openExternalModPage(params.context.backendId, data.publishResult:getData().modId)`
- `shouldRetryInstall : function(Modhub.BackendId) : boolean`  (call as `api.modhub.shouldRetryInstall(…)`): Whether the mod manager should offer to retry failed downloads (via `restartModBackendAsync`).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:582`: `shouldRetry = api.modhub.shouldRetryInstall(params.context.backendId),`
- `getModHubModIdForModId : function(Mod.ModId) : Modhub.ModId`  (call as `api.modhub.getModHubModIdForModId(…)`): Mod hub id of a locally installed mod.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:99`: `local modHubModId = api.modhub.getModHubModIdForModId(modId)`
- `getModhubModIdForSavegame : function(Modhub.BackendId, SaveGameId) : Modhub.ModId`  (call as `api.modhub.getModhubModIdForSavegame(…)`): Mod hub id under which a savegame was shared.
    - returns: The id; `isValid()` is false when the savegame has not been shared.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:3995`: `local modhubId = api.modhub.getModhubModIdForSavegame(modPublishBackendId, params.selectedSave)`
- `setModhubModIdForSavegame : function(Modhub.BackendId, SaveGameId, Modhub.ModId)`  (call as `api.modhub.setModhubModIdForSavegame(…)`): Records the mod hub id under which a savegame was published.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:4012`: `api.modhub.setModhubModIdForSavegame(modPublishBackendId, params.selectedSave, actualModhubId)`
- `stageSavegame : function(SaveGameId, boolean, Modhub.BackendId, Modhub.ModId) : {Modhub.ModId, {string : any}}`  (call as `api.modhub.stageSavegame(…)`): Prepares a savegame or saved map for publishing on the mod hub.
    - param `#1`: Id of the savegame.
    - param `#2`: True for a savegame, false for a saved map (savemap namespace).
    - param `#3`: Backend to publish to.
    - param `#4`: Existing mod hub id of the savegame, if it was shared before.
    - returns: the staged mod's local id, and any extra metadata computed while staging.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:4000`: `local res = api.modhub.stageSavegame(params.selectedSave, params.savegame, modPublishBackendId, modhubId)`
- `openTermsInBrowser : function(Modhub.BackendId)`  (call as `api.modhub.openTermsInBrowser(…)`): Opens the terms of use of the given mod backend in the browser. The mod publish form calls it from its "View" button next to the mod.io terms notice (not on consoles).
    - param `#1`: Backend, usually `mod_manager_react_util.getModPublishBackendId()`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1657`: `api.modhub.openTermsInBrowser(mod_manager_react_util.getModPublishBackendId())`
- `exportValidatorResult : function(result : Modhub.ModValidatorResult) : string`  (call as `api.modhub.exportValidatorResult(…)`): Writes a mod validation report to a file.
    - returns: Path of the exported file (shown to the user).
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:4523`: `local file = api.modhub.exportValidatorResult(params.result)`
