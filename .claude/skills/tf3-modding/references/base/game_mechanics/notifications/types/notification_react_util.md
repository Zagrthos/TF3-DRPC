# game_mechanics.notifications.types.notification_react_util

Definition file: `base/tealdef/game_mechanics/notifications/types/notification_react_util.d.tl` (in the game folder).

GUI recipes for notification icons, HUD icons and progress bars.

## NotificationReactUtil (record)

```teal
global record NotificationReactUtil
```
Load: `local notification_react_util = ug_require "::/game_mechanics/notifications/types/notification_react_util.tl" as NotificationReactUtil`

GUI recipes for notification icons and progress bars, used by pop-ups, the log, entity windows, the HUD and the line and vehicle manager.

- `NotificationHudIcons : Recipe<HudIconsParam>`: Row of icons above an entity in the world, one per persistent notification whose GUI data has a `hudIcon`.
    - used 21x in the base game, e.g. `base/content/gui/gui/construction/constructionHudIconMaster.tl:177`: `table.insert(hudIcons, 1, notification_react_util.NotificationHudIcons({`
- `NotificationSimpleIcon : Recipe<NotificationSimpleIconParam>`: Notification icon styled by category and status.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:50`: `} or notification_react_util.NotificationSimpleIcon{`
- `NotificationProgressIcon : Recipe<NotificationProgressIconParam>`: Notification icon with a circular progress indicator.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:45`: `dataState.progress and notification_react_util.NotificationProgressIcon{`
- `createNotificationCacheAccessFn : function() : function(Engine.Entity) : {Notification}`  (call as `notification_react_util.createNotificationCacheAccessFn(…)`): Makes a lookup function for the persistent notifications of an entity. It uses React refs and refreshes them every step, so call it inside a recipe.
    - returns: Function that takes an entity and returns its persistent notifications, with `notificationId` filled in.

    ```lua
    -- base game, gui/main/game.tl
    local accessNotificationCacheFn = notification_react_util.createNotificationCacheAccessFn()
    ```

    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:127`: `local accessNotificationCacheFn = notification_react_util.createNotificationCacheAccessFn()`
- `NotificationProgressContent : Recipe<NotificationProgressData>`: Notification body with icon, description and progress bars, as used in pop-ups.
  In game: "{remainingTime} left" (German: „{remainingTime} verbleibend“)
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_popups.tl:27`: `notification_react_util.NotificationProgressContent{`

### NotificationReactUtil.HudIconsParam (record)

```teal
record HudIconsParam
```

Params of `NotificationHudIcons`.

- `accessNotificationCacheFn : function(Engine.Entity) : {Notification}`: Function from `createNotificationCacheAccessFn` that returns the persistent notifications of an entity.
- `entity : Engine.Entity`: Entity whose notification icons are shown.

### NotificationReactUtil.NotificationSimpleIconParam (record)

```teal
record NotificationSimpleIconParam
```

Params of `NotificationSimpleIcon`.

- `icon : string`: Path of the icon image.
- `status : NotificationGuiData.Status`: optional
- `type : NotificationGuiData.Type`: GUI category; sets the style class and with it the color.

### NotificationReactUtil.NotificationProgressIconParam (record)

```teal
record NotificationProgressIconParam
```

Params of `NotificationProgressIcon`.

- `icon : string`: Path of the icon image.
- `status : NotificationGuiData.Status`: optional
- `type : NotificationGuiData.Type`: GUI category; sets the style class and with it the color.
- `percentage : number`: Progress from 0 to 1, drawn as a circle around the icon.

### NotificationReactUtil.NotificationProgressData (record)

```teal
record NotificationProgressData
```

Params of `NotificationProgressContent`.

- `description : string`: Text shown above the progress bars.
- `icon : string`: Path of the icon image; no icon if `nil`.
- `status : NotificationGuiData.Status`: optional
- `type : NotificationGuiData.Type`: GUI category; sets the style of the icon and the bars.
- `progress : NotificationGuiData.Progress`: optional
- `progresses : {NotificationGuiData.Progress}`: Progress bars to show. A set `progress` takes precedence.
