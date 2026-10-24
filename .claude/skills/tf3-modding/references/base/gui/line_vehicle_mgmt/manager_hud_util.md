# gui.line_vehicle_mgmt.manager_hud_util

Definition file: `base/tealdef/gui/line_vehicle_mgmt/manager_hud_util.d.tl` (in the game folder).

HUD icon recipe used by the Line & Vehicle Manager for stations, depots, vehicles, towns and industries while the manager tool is active.

## ManagerHudUtil (record)

```teal
global record ManagerHudUtil
```
Load: `local manager_hud_util = ug_require "::/gui/line_vehicle_mgmt/manager_hud_util.tl" as ManagerHudUtil`

HUD icons of the Line & Vehicle Manager.

- `LineVehicleManagerHudIconMaster : Recipe2<Builtin.HudIconParam, CommonActionParams>`: HUD icon master recipe the manager tool sets with `ContextBuilder.setHudMaster`; its user parameter is the manager's `CommonActionParams`.
  In game: "Configure Stop", "Move Stop", "Remove Stop" (German: „Haltestelle konfigurieren“, „Haltestelle verschieben“, „Haltestelle entfernen“)
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:270`: `ctx.setHudMaster(manager_hud_util.LineVehicleManagerHudIconMaster)`
