# game_mechanics.notifications.gui.notification_colors

Definition file: `base/tealdef/game_mechanics/notifications/gui/notification_colors.d.tl` (in the game folder).

GUI colors of the notification categories (`notification_colors.gres`).

## NotificationColors (record)

```teal
global record NotificationColors
```

Colors of the notification categories, defined in `::/game_mechanics/notifications/gui/notification_colors.gres`. Style sheets of notifications, statistics and the line and vehicle manager use them.

Example:
```lua
-- base game, notifications/gui/notifications.css.lua
local colorNotification = api.gui.genericRep.get(api.gui.genericRep.find("::/game_mechanics/notifications/gui/notification_colors.gres")).data
local notificationOpportunitySolid = colorNotification.Opportunity.Active
```

- `Caution : GResColor`: Color of `Caution` notifications (school-bus yellow).
- `Problem : GResColor`: Color of `Problem` notifications (fire-engine red).
- `Info : GResColor`: Color of `Info` notifications (true blue).
- `Achievement : GResColor`: Color of `Achievement` notifications (Kelly green).
- `Unkown : GResColor`: Misspelled in the definition. The `.gres` file sets `Unknown` (gray), so this field is `nil`.
- `Opportunity : OpportunityColor`: Colors of `Opportunity` notifications.

### NotificationColors.OpportunityColor (record)

```teal
record OpportunityColor
```

Colors of `Opportunity` (subsidy) notifications by subsidy state.

- `Active : GResColor`: Subsidy in progress (grape).
- `Complete : GResColor`: Subsidy completed (grape).
- `Missed : GResColor`: Subsidy missed (grape).
- `Proposed : GResColor`: Subsidy proposed, RGB 170, 80, 170.
- `Failed : GResColor`: Subsidy failed, RGB 70, 20, 70.
