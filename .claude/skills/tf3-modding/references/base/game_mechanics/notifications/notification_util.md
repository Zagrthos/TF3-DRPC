# game_mechanics.notifications.notification_util

Definition file: `base/tealdef/game_mechanics/notifications/notification_util.d.tl` (in the game folder).

Helpers that read notification type resources and change or read the notifications game-script state.

## NotificationUtil (record)

```teal
global record NotificationUtil
```
Load: `local notification_util = ug_require "::/game_mechanics/notifications/notification_util.tl" as NotificationUtil`

Helpers for notification type resources and the notifications game-script state.

Notes:

Functions that take a `NotificationsState` change that table; the game script uses
them inside its own update and event handler. Scripts outside it should send the
`Notifications` events instead (see `Notification`).

- `defaultAutoDismissDurationMs : integer`: Default `autoDismissDuration` of a notification, 60000 ms of game time.
    - used 19x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:116`: `autoDismissDuration = notification_util.defaultAutoDismissDurationMs,`
- `getGuiTypeFromNotificationType : function(name : string) : NotificationGuiData.Type`  (call as `notification_util.getGuiTypeFromNotificationType(…)`): Reads the GUI category of a notification type from its `.res` file.
    - param `name`: Notification type script, as in `Notification.type`.
    - returns: The `guiType` of the resource, or `"Unknown"` if there is no such notification resource or it has no `guiType`.
    - used 11x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:33`: `local guiType = notification_util.getGuiTypeFromNotificationType(param.entry.notification.type)`
- `getInitiallyIgnoredTypes : function() : {string : boolean}`  (call as `notification_util.getInitiallyIgnoredTypes(…)`): Reads `initiallyIgnoredType` of all notification resources.
    - returns: Map from notification type script name to whether it is ignored in a new game.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notification_legacy_util.tl:15`: `types = notification_util.getInitiallyIgnoredTypes(),`
- `getNotificationType2Label : function() : {string : string}`  (call as `notification_util.getNotificationType2Label(…)`): Reads the labels of all notification resources, as shown in the log filter.
    - returns: Map from notification type script name to its label; types without a label are left out.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:227`: `return notification_util.getNotificationType2Label()`
- `getAllNotificationGuiTypes : function() : {NotificationGuiData.Type}`  (call as `notification_util.getAllNotificationGuiTypes(…)`): Collects the GUI categories used by all notification resources.
    - returns: Distinct categories in no fixed order; `"Unknown"` stands for resources without `guiType`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:207`: `local allTypes = notification_util.getAllNotificationGuiTypes()`
- `getAllNotificationTypesForGuiType : function(guiType : NotificationGuiData.Type) : {string}`  (call as `notification_util.getAllNotificationTypesForGuiType(…)`): Lists all notification types of one GUI category.
    - param `guiType`: The category.
    - returns: Notification type script names.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:217`: `local names =  notification_util.getAllNotificationTypesForGuiType(ignorableType)`
- `getNotificationSimUpdateScript : function(type : string) : string`  (call as `notification_util.getNotificationSimUpdateScript(…)`): Reads the `simUpdateScript` of a notification type.
    - param `type`: Notification type script, as in `Notification.type`.
    - returns: The function path, e.g. `"::/game_mechanics/notifications/types/overcrowding.sim.script@updateData"`, or `nil`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:74`: `local updateScript = notification_util.getNotificationSimUpdateScript(notificationEntry.notification.type)`
- `removeNotification : function(NotificationsState, integer)`  (call as `notification_util.removeNotification(…)`): Deletes a notification from the state and the history.
    - param `#1`: Notifications state to change.
    - param `#2`: Id of the notification. Unknown ids are ignored.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notification_legacy_util.tl:404`: `notification_util.removeNotification(notificationsState, id)`
- `addNotification : function(NotificationsState, Notification) : integer`  (call as `notification_util.addNotification(…)`): Stores a new notification, runs its sim update script and trims the history to 100 entries.
    - param `#1`: Notifications state to change.
    - param `#2`: The notification.
    - returns: Id of the new notification; `nil` if its type is fully ignored, -1 if no free id was found.
    - notes: It does not check `paused`; the `add` event handler does.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:727`: `notification_util.addNotification(notificationsState, notification)`
- `enlistNotification : function(state : NotificationsState, notificationId : integer)`  (call as `notification_util.enlistNotification(…)`): Clears the `dismissed` flag so the notification shows in the pop-ups again.
    - param `state`: Notifications state to change.
    - param `notificationId`: Id of the notification.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:745`: `notification_util.enlistNotification(notificationsState, (param as DismissNotification).id)`
- `dismissNotification : function(state : NotificationsState, notificationId : integer)`  (call as `notification_util.dismissNotification(…)`): Sets the `dismissed` flag, which hides the notification from the pop-ups.
    - param `state`: Notifications state to change.
    - param `notificationId`: Id of the notification.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:749`: `notification_util.dismissNotification(notificationsState, (param as DismissNotification).id)`
- `updatePersistentNotifications : function(NotificationsState, {string : {integer}}, PersistentNotifications)`  (call as `notification_util.updatePersistentNotifications(…)`): Applies a `PersistentNotifications` update; the `updatePersistent` event calls it.
    - param `#1`: Notifications state to change.
    - param `#2`: Persistent notification ids by type, from `getPersistentHistoryByType`.
    - param `#3`: The update.
    - used 19x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:175`: `notification_util.updatePersistentNotifications(notificationsState, historyByType, persistentNotifications)`
- `removePersistentNotifications : function(NotificationsState, {string : {integer}}, RemovePersistentNotifications)`  (call as `notification_util.removePersistentNotifications(…)`): Expires the persistent notifications that match a `RemovePersistentNotifications`; the `removePersistent` event calls it.
    - param `#1`: Notifications state to change.
    - param `#2`: Persistent notification ids by type, from `getPersistentHistoryByType`.
    - param `#3`: Type and entities to match.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:741`: `notification_util.removePersistentNotifications(notificationsState, historyByType, persistentNotifications)`
- `externalGetNotificationsState : function() : NotificationsState`  (call as `notification_util.externalGetNotificationsState(…)`): Reads the state of the notifications game script from its `GAME_SCRIPT` component.
    - returns: The state as a Lua table.
- `getPersistentHistoryByType : function(state : NotificationsState) : {string : {integer}}`  (call as `notification_util.getPersistentHistoryByType(…)`): Groups the ids of persistent notifications in the history by type.
    - param `state`: Notifications state.
    - returns: Map from notification type to ids, in history order.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:117`: `local historyByType = notification_util.getPersistentHistoryByType(notificationsState)`
- `removeUnavailableAvailabilityNotifications : function(state : NotificationsState)`  (call as `notification_util.removeUnavailableAvailabilityNotifications(…)`): Deletes "new vehicle" notifications whose models or multiple units no longer exist, e.g. after a mod was removed. Runs when a save game is loaded.
    - param `state`: Notifications state to change.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:717`: `notification_util.removeUnavailableAvailabilityNotifications(notificationsState)`
- `externalGetNotificationsStateNative : function() : NativeLuaTable`  (call as `notification_util.externalGetNotificationsStateNative(…)`): Reads the notifications game-script state without converting it to a Lua table (`state_native` of the `GAME_SCRIPT` component).
    - returns: The native state; read it with the `...FromNative` functions.
    - used 19x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:333`: `local notificationsState = notification_util.externalGetNotificationsStateNative()`
- `getPersistingEntity2NotificationFromNative : function(nativeState : NativeLuaTable) : {Engine.Entity : {integer}}`  (call as `notification_util.getPersistingEntity2NotificationFromNative(…)`): Maps each entity to the persistent notifications that stick to it.
    - param `nativeState`: State from `externalGetNotificationsStateNative`.
    - returns: Map from entity to notification ids.
    - used 12x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/notification_react_util.tl:92`: `local persistingEntity2Notification = notification_util.getPersistingEntity2NotificationFromNative(notificationRefNative)`
- `getHistoryFromNative : function(nativeState : NativeLuaTable) : {integer}`  (call as `notification_util.getHistoryFromNative(…)`): Copies the history from the native state.
    - param `nativeState`: State from `externalGetNotificationsStateNative`.
    - returns: Notification ids, oldest first.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:335`: `local history = notification_util.getHistoryFromNative(notificationsState)`
- `getNotificationFromNative : function(nativeState : NativeLuaTable, notificationId : integer) : Notification`  (call as `notification_util.getNotificationFromNative(…)`): Copies one notification from the native state.
    - param `nativeState`: State from `externalGetNotificationsStateNative`.
    - param `notificationId`: Id of the notification.
    - returns: The notification as a Lua table.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/types/notification_react_util.tl:107`: `local notification = notification_util.getNotificationFromNative(notificationStateRef:get(), notificationId)`
- `getNotificationEntryFromNative : function(nativeState : NativeLuaTable, notificationId : integer) : NotificationsState.Entry`  (call as `notification_util.getNotificationEntryFromNative(…)`): Copies one state entry (notification with display flags) from the native state.
    - param `nativeState`: State from `externalGetNotificationsStateNative`.
    - param `notificationId`: Id of the notification.
    - returns: The entry as a Lua table.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_popups.tl:95`: `local notification = notification_util.getNotificationEntryFromNative(notificationStateNative, params.notificationId)`
- `makeDefaultWouldClick : function(entityAndRevision : {EntityUtil.EntityAndRevision}) : function() : boolean`  (call as `notification_util.makeDefaultWouldClick(…)`): Makes a `wouldClick` function for `NotificationGuiData`.
    - param `entityAndRevision`: Entities of the notification.
    - returns: Function that returns true if one of the entities still exists unchanged.
    - used 20x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_notification_greenify.script.tl:34`: `wouldClick = notification_util.makeDefaultWouldClick({notificationParams.constructionEntity}),`
- `makeDefaultOnClick : function(entityAndRevision : {EntityUtil.EntityAndRevision}) : function(stack : boolean) : boolean`  (call as `notification_util.makeDefaultOnClick(…)`): Makes an `onClick` function for `NotificationGuiData` that moves the camera to the first entity that still exists unchanged and selects it.
    - param `entityAndRevision`: Entities of the notification.
    - returns: Function that takes `stack` (passed with the `selectEntity` GUI event) and returns true if it found an entity.
    - used 20x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_notification_greenify.script.tl:35`: `onClick = notification_util.makeDefaultOnClick({notificationParams.constructionEntity}),`
