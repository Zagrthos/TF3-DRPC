# gui.main.hudIconMaster

Definition file: `base/tealdef/gui/main/hudIconMaster.d.tl` (in the game folder).

Older declaration of the HUD icon parameter types. The base game uses the identical
types from `HudIconToolbox` (`gui/main/hud_icon_toolbox`).

## HudIconMaster (record)

```teal
global record HudIconMaster
```

Types for HUD icons; same as the ones in `HudIconToolbox`, which the base game uses.

### HudIconMaster.Happiness (record)

```teal
record Happiness
```

Waiting passengers at a station or terminal, used by the station HUD icon.

- `totalWaiting : integer`: Number of passengers waiting.
- `unhappy : integer`: Number of waiting passengers with bad quality (unhappy).

### HudIconMaster.HudIconMasterUserParam (interface)

```teal
interface HudIconMasterUserParam
```

User parameter passed to HUD icon recipes.

- `accessNotificationCacheFn : function(Engine.Entity) : {Notification}`: Returns the notifications of an entity, shown as notification icons next to the HUD icon.
- `selectedEntity : Engine.Entity`: Currently selected entity.
