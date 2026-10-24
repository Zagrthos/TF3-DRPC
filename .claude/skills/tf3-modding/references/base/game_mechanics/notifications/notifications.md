# game_mechanics.notifications.notifications

Definition file: `base/tealdef/game_mechanics/notifications/notifications.d.tl` (in the game folder).

Records of the notification system, the state of its game script and the event params for adding, dismissing and ignoring notifications.

## Globals

- `GameContext` (type): Shared context of the in-game GUI. Holds references to the main renderer component, window container and tool stack, plus mission filters, disabled features and a few shared states.
    - notes: Created once by `GameUIRoot` in `gui/main/game.tl` and passed to tools, windows and HUD recipes as `gameCtx` (for example `toolParam.gameCtx`). Code that has no `gameCtx` can reach the most important parts through `GameReactGlobals` (`gui/main/game_react_globals.tl`).

    ```lua
    -- open a singleton window and push a tool through the context
    gameCtx.windowContainer:get():getApi().addSingletonWindow(ContextHelperWindow, { gameCtx = gameCtx, onClose = onClose })
    gameCtx.toolStack:get():getApi().push(FinanceTool, nil, toolParam)
    ```

## NotificationGuiData (record)

```teal
global record NotificationGuiData
```

What a notification type's `useDataState` returns; the pop-up, the log, entity windows and the HUD display it.

- `title : string`: Title of the notification.
- `description : string`: Body text of the notification.
- `icon : string`: Path of the icon image, e.g. `"::/game_mechanics/notifications/gui/icons/terminal_full.tga"`.
- `hudIcon : string`: optional, if specified, for persistent notifications, a hud icon will be displayed above the according component carrying the entity
- `lvmIcon : string`: optional, if specified, for persistent notifications, the issue will be displayed in the lvm for the according line/vehicle/...
- `iconExplainTooltip : string`: optional, used to explain when hovering potential hud- or lvm icon
- `previewImage : string`: optional
- `soundOnMount : {FilePath}`: optional, one of the provided will be played randomly
- `wouldClick : function() : boolean`: optional
- `onClick : function(stack : boolean, dryRun? : boolean) : boolean`: optional
- `status : Status`: optional
- `progress : Progress`: optional
- `progresses : {Progress}`: Several progress bars. A set `progress` takes precedence.

### NotificationGuiData.Type (enum)

```teal
enum Type
```

GUI category of a notification type, set with `guiType` in its `.res` file. It picks the color and the filter group in the notification log.

- `"Info"`: News, e.g. new vehicles or constructions, a founded industry, a new cargo demand.
- `"Caution"`: Warnings, e.g. an overcrowded station, a closing industry, a vehicle in bad condition.
- `"Problem"`: Problems, e.g. line, station, vehicle or town connection issues.
- `"Opportunity"`: Subsidies (available, in progress, active, failed).
- `"Achievement"`: Successes, e.g. company rank-up, town notifications, completed mission tasks, landmarks.
- `"Unknown"`: NOTE: fall-back value to avoid modding issues, should not be used on purpose

### NotificationGuiData.Status (enum)

```teal
enum Status
```

Optional state of an `Opportunity` notification that changes its icon and progress-bar style.

- `"Pending"`: The subsidy is proposed and not yet started.
- `"Failed"`: The subsidy failed.

### NotificationGuiData.Progress (record)

```teal
record Progress
```

One progress bar in the notification.

- `percentage : number`: Progress from 0 to 1.
- `text : string`: Label of the bar. Without it the GUI shows the remaining time from `remainingDurationMs`.
- `remainingDurationMs : number`: optional

## NotificationSimType (record)

```teal
global record NotificationSimType<T>
```

Sim update script of a notification type, referenced by `simUpdateScript` in the type's `.res` file (e.g. `"::/game_mechanics/notifications/types/overcrowding.sim.script@updateData"`).

Notes:

The notifications game script calls `updateData` when a notification is added and
afterwards for each notification about every 30th tick. Use it to copy data that the
GUI cannot read later, such as the name of an entity that may be removed.

Example:
```lua
-- base game, notifications/types/overcrowding.sim.script.tl
local data : NotificationSimType<OvercrowdingNotificationParams> = {}

data.updateData = function(notificationParams : OvercrowdingNotificationParams, old : StationBusyNotificationSimParams) : StationBusyNotificationSimParams
    if old.mapping == nil then
        old.mapping = {}
    end
    if not entity_util.entityChanged0(notificationParams.entity) then
        old.mapping[notificationParams.entity.entity] = entity_util.getEntityName(notificationParams.entity.entity)
    end
    return old
end

return data
```

- `updateData : function(T, any) : any`: Computes the notification's `simParams`.
    - param `#1`: The notification's `params`.
    - param `#2`: The previous `simParams` (an empty table on the first call).
    - returns: The new `simParams`.

## NotificationType (record)

```teal
global record NotificationType<T>
```

GUI script of a notification type; the script named by `Notification.type` returns it.

Example:
```lua
-- base game, notifications/types/overcrowding.script.tl (shortened)
local data : NotificationType<OvercrowdingNotificationParams> = {}

data.useDataState = function(notificationParams : OvercrowdingNotificationParams, simParams : StationBusyNotificationSimParams) : NotificationGuiData
    local icon = "::/game_mechanics/notifications/gui/icons/terminal_full.tga"
    local state = engine_react_util.useStepStateTimer(function() : string
        return simParams.mapping[notificationParams.entity.entity]
    end)
    return {
        title = _("Station Overcrowded"),
        description = lang_util.format(_("{stationName} is overcrowded, passengers will be leaving."), { stationName = state:old() }),
        icon = icon,
        hudIcon = icon,
        lvmIcon = icon,
        wouldClick = notification_util.makeDefaultWouldClick({notificationParams.entity}),
        onClick = notification_util.makeDefaultOnClick({notificationParams.entity}),
    }
end

return data
```

- `useDataState : function(T, ?any) : NotificationGuiData`: only use in recipe WITHOUT another DEPENDENT state (as *THIS provides already one or more!)
use as last state in recipe, as the number of states provided might differ
use localKey in parent recipe to ensure proper update

## Notification (record)

```teal
global record Notification
```

A notification as sent with the `Notifications`/`add` script event and stored in `NotificationsState`.

Notes:

`type` names a notification type script. The `.res` file with the same base name
(`type = "notification"`) holds its `guiType`, `label`, `simUpdateScript` and
`initiallyIgnoredType`; the script itself returns a `NotificationType` whose
`useDataState(params, simParams)` builds the GUI text and icons.

A notification expires (and is hidden from pop-ups) when `autoDismissDuration` has
passed, or when `params.entities` is a non-empty list of `EntityUtil.EntityAndRevision`
and every one of those entities has changed or is gone.

`add` events are dropped while the notifications state is paused, and notifications
of a fully ignored type are not stored at all (see `NotificationsState.Ignored`).

Example:
```lua
-- urbangames_deluxe_upgrade_pack, fun_elements/balloon.script.tl
api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Notifications", "add", {
    type = resolve("balloon_notification.script"),
    params = {
        entity = entity_util.makeEntityAndRevision0(balloon.entity),
        townEntity = entity_util.makeEntityAndRevision0(townEntity),
    },
    autoDismissDuration = notification_util.defaultAutoDismissDurationMs,
}))
```

- `type : string`: Name of the notification type script, e.g. `"::/game_mechanics/notifications/types/subvention.script"` (the resource path with `.script` instead of `.res`).
- `params : table`: Type-specific params, passed to the type's `useDataState` and to its sim update script. A list of entities in `params.entities` makes the notification expire once all of them have changed.
- `notificationId : integer`: Id of the notification. It is not stored in the state (the id is the key of `NotificationsState.notifications`); the function from `NotificationReactUtil.createNotificationCacheAccessFn` fills it in.
- `autoDismissDuration : integer`: NOTE: THIS IS NOT THE SAME AS "dismiss", this is basically a "hide" after a certain time
- `simParams : any`: Data computed on the simulation side by the type's `simUpdateScript` (for example entity names), passed to `useDataState` as the second argument. `addNotification` overwrites it.

## PersistentNotifications (record)

```teal
global record PersistentNotifications
```

Param of the `Notifications`/`updatePersistent` event. Sets the complete list of persistent notifications of one type.

Notes:

Existing persistent notifications of `type` whose entities and params are not in the
new list expire; entries that are new get added. A persistent notification stays in the
history beyond the 100-entry limit and is shown in entity windows, the line and vehicle
manager and as a HUD icon (if its GUI data has `hudIcon` / `lvmIcon`).

Example:
```lua
-- urbangames_campaign_mission_05, mission/tasks/drilling/drilling.tl
local persistentNotifications : PersistentNotifications = {
    type = "urbangames_campaign_mission_05::/mission/tasks/drilling/mission_drilling_notification.script",
    entitiesAndParam = {{
        entities = entity_util.makeEntityAndRevision0Array(getEntititesForNotification(entity)),
        param = notificationParams,
    }},
    passOnlyParam = true,
}
api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Notifications", "updatePersistent", persistentNotifications))
```

- `entitiesAndParam : {EntitiesAndParam}`: All persistent notifications of this type that should exist now.
- `type : string`: Notification type script, as in `Notification.type`.
- `passOnlyParam : boolean`: If true, `param` becomes the notification's `params`; otherwise `params` is `{ entities = ..., param = ... }`.

### PersistentNotifications.EntitiesAndParam (record)

```teal
record EntitiesAndParam
```

One persistent notification.

- `entities : {EntityUtil.EntityAndRevision}`: Entities the notification sticks to; it is matched by these and shown for them.
- `param : any`: Type-specific param of the notification.

## RemovePersistentNotifications (record)

```teal
global record RemovePersistentNotifications
```

Param of the `Notifications`/`removePersistent` event. Expires the persistent notifications of `type` whose entities equal `entities`.

Example:
```lua
-- urbangames_campaign_mission_05, mission/tasks/drilling/drilling.tl
local persistentNotifications : RemovePersistentNotifications = {
    type = "urbangames_campaign_mission_05::/mission/tasks/drilling/mission_drilling_notification.script",
    entities = entity_util.makeEntityAndRevision0Array(getEntititesForNotification(entity)),
}
api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Notifications", "removePersistent", persistentNotifications))
```

- `entities : {EntityUtil.EntityAndRevision}`: Entity list to match, compared deeply with the stored entities.
- `type : string`: Notification type script, as in `Notification.type`.

## DismissNotification (record)

```teal
global record DismissNotification
```

Param of the `Notifications`/`dismiss` event (hides a notification from the pop-ups) and of the `enlist` event (shows it again).

- `id : integer`: Id of the notification (key in `NotificationsState.notifications`).

## NotificationsState (record)

```teal
global record NotificationsState
```

State of the game script `::/game_mechanics/notifications/notifications.gs`. Read it with `NotificationUtil.externalGetNotificationsState()`.

Notes:

The script handles these events (id `"Notifications"`): `add` (`Notification`),
`updatePersistent` (`PersistentNotifications`), `removePersistent`
(`RemovePersistentNotifications`), `dismiss` and `enlist` (`DismissNotification`),
`updateIgnoredTypes` (`{ ignoredTypes = {[type] = true}, ignoredGuiTypes = {...}, ignoreFully = boolean }`),
`pause` (boolean) and `initialSound` (`{ notificationId = id }`).

Besides stored notifications it creates persistent notifications itself for line,
station, town, vehicle, industry, subsidy and company issues.

Example:
```lua
-- base game, mission/mission_sim.script.tl: hide some types completely during a mission
api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "Notifications", "updateIgnoredTypes", {
    ignoredTypes = newNotificationFilter,
    ignoreFully = true,
}))
```

- `notifications : {integer : Entry}`: Stored notifications by id.
- `history : {integer}`: Notification ids, oldest first. Adding trims it to 100 entries; persistent notifications are kept.
- `maxId : integer`: Last id handed out. Ids wrap around at 1000000.
- `version : integer`: Version of the state layout, compared with `NotificationLegacyUtil.CurrentVersion` when a save game is loaded.
- `ignored : Ignored`: Ignored notification types; initially the types with `initiallyIgnoredType = true` in their `.res` file, with `fully = false`.
- `paused : boolean`: While true, the script skips its update and drops `add` events.
    - notes: `initMission` sets it. The `pause` event with `false` clears it. In this version the `pause` event with `true` builds a fresh state but stores the old one, so it does not set the flag.
- `noRoadConnectionUpdateTimestamp : integer`: Game time (ms) of the last check for stations without road connection. The check runs again after `75 * api.util.getDefaultDayDuration()`.
- `vehicle2problem : {Engine.Entity : VehicleProblem}`: Player vehicles that are en route, not stopped by the user, and have speed 0. After 5 minutes of game time (300000 ms) they get a "Vehicle Stuck" notification.
- `line2problemTimestamp : {Engine.Entity : integer}`: Game time (ms) when a problem of each line was first seen; lines without a problem are removed.
- `stationGroup2overflowResolvedTimestamp : {Engine.Entity : integer}`: Game time (ms) when each station group was first seen without overflow. The cooldown that read it is commented out in the base game.
- `wastedVehicles : {Engine.Entity : boolean}`: Vehicles that have a "Vehicle Condition" notification. A vehicle is added at maintenance state 0.2 or less and stays until the state rises above 0.3.

### NotificationsState.Entry (record)

```teal
record Entry
```

One stored notification with its display state.

- `timestamp : integer`: Game time (ms) when the notification was added.
- `notification : Notification`: The notification.
- `persisting : {EntityUtil.EntityAndRevision}`: if #> 0, the notification will not be deleted with the history entry
- `dismissed : boolean`: user input: dismiss via right mouse click on pop-up or toggle in log to hide it in popups
- `expired : boolean`: not "active" anymore, (maximal, ie only iff tracked) visible in history log
- `tracked : boolean`: if not ignored for popups/log then it is tracked for log and also shown there
- `playedInitialSound : boolean`: The pop-up has played its sound; set by the `initialSound` event so the sound plays only once.

### NotificationsState.Ignored (record)

```teal
record Ignored
```

Which notification types are ignored and how. Set by the `updateIgnoredTypes` event and by the filter in the notification log.

- `fully : boolean`: Controls how the notification types in "types" are ignored (types not in the set are unaffected):
the filter in the notification log is disabled in this mode, as it controls the case below
while still showing up in EOWs, LVM and as HUD icons
the log's filter can be used to change types with this level of ignorance
Defaults to true when "updateIgnoredTypes" is sent without "ignoreFully".
Note that whenver Ignored changes, that fully is set for the whole current state. It is advised to usually go with either strategy per "game".
(I.e. default game is usually false, while missions might go with true.)
- `types : {string : boolean}`: set of notification type names to ignore (see "fully" for how)

### NotificationsState.VehicleProblem (record)

```teal
record VehicleProblem
```

Tracking data for a vehicle that stands still.

- `problemSince : integer`: Game time (ms) since which the vehicle stands still.

## NotificationButtonParam (record)

```teal
global record NotificationButtonParam
```

Params of the `NotificationButton` recipe (`notifications/gui/notification_button.tl`), the game-bar button that opens and closes the notification log.

- `gameCtx : GameContext`: Game GUI context; its tool stack holds the log window.

## NotificationLogToolParam (record)

```teal
global record NotificationLogToolParam is ToolReactUtil.IToolParam
```

Params of the notification log tool window, pushed by the notification button.

- `gameCtx : GameContext`: Game GUI context.
- `onClose : function()`: Closes the log window.
- `storeFilterState : function(FilterState)`: Receives the current filter settings so the next opening can restore them.
- `initialFilterState : FilterState`: Filter settings to start with; `nil` opens the history view with no category filter.

### NotificationLogToolParam.FilterState (record)

```teal
record FilterState
```

Filter settings of the log window. The button keeps them between openings.

In game: "History", "Active", "All" (German: „Historie“, „Aktiv“, „Alle“)

- `category : NotificationGuiData.Type`: GUI category shown in the log; `nil` shows all.
- `displayIndex : integer`: 1 = history, 2 = active
