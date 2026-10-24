# game_mechanics.notifications.gui.sound.notification_sfx

Definition file: `base/tealdef/game_mechanics/notifications/gui/sound/notification_sfx.d.tl` (in the game folder).

Sounds of notification pop-ups (`notification_sfx.gres`).

## NotificationSFX (record)

```teal
global record NotificationSFX
```

Sounds of notification pop-ups, defined in `::/game_mechanics/notifications/gui/sound/notification_sfx.gres`.

- `Initialize : {FilePath}`: Played when a pop-up first appears, unless its GUI data has `soundOnMount`. One file is picked at random.
- `Resolve : {FilePath}`: Played when a pop-up disappears.
