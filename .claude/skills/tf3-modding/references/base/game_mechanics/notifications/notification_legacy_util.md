# game_mechanics.notifications.notification_legacy_util

Definition file: `base/tealdef/game_mechanics/notifications/notification_legacy_util.d.tl` (in the game folder).

Initial state and save-game migration of the notifications game-script state.

## NotificationLegacyUtil (record)

```teal
global record NotificationLegacyUtil
```
Load: `local notification_legacy_util = ug_require "::/game_mechanics/notifications/notification_legacy_util.tl" as NotificationLegacyUtil`

Initial state and save-game migration of `NotificationsState`.

- `CurrentVersion : integer`: Current version of the state layout (24 in this game version).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:712`: `if (notificationsState.version ~= notification_legacy_util.CurrentVersion) then`
- `makeInitialState : function() : NotificationsState`  (call as `notification_legacy_util.makeInitialState(…)`): Makes an empty notifications state.
    - returns: State with no notifications, `ignored.types` from `NotificationUtil.getInitiallyIgnoredTypes()`, `ignored.fully = false` and `version = CurrentVersion`.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:53`: `notificationsState = notification_legacy_util.makeInitialState()`
- `updateLegacyState : function(notificationsState : NotificationsState)`  (call as `notification_legacy_util.updateLegacyState(…)`): Migrates a state from an older save game to `CurrentVersion`, in place. States older than version 9 are replaced by an empty state.
    - param `notificationsState`: State to migrate.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:713`: `notification_legacy_util.updateLegacyState(notificationsState)`
