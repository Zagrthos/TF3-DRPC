# gui.main.town_hud_react_util

Definition file: `base/tealdef/gui/main/town_hud_react_util.d.tl` (in the game folder).

The HUD icon of towns (name, population, growth progress and received cargo types).

## TownHudReactUtil (record)

```teal
global record TownHudReactUtil
```
Load: `local town_hud_react_util = ug_require "::/gui/main/town_hud_react_util.tl" as TownHudReactUtil`

Town HUD icon recipe.

- `TownHudIcon : Recipe3<Builtin.HudIconParam, boolean, boolean>`: first false: hide town growth info, second false: hide cargo needs

HUD icon of a town; the second parameter shows the growth information, the third the cargo needs.

    ```lua
    -- from hud_icon_toolbox.tl
    children[#children + 1] = town_hud_react_util.TownHudIcon(param, true, true)
    ```

    - used 13x in the base game, e.g. `base/content/gui/gui/construction/constructionHudIconMaster.tl:188`: `hudIcons[#hudIcons + 1] = town_hud_react_util.TownHudIcon(param, true, true)`
