# scripts.app_util

Definition file: `base/tealdef/scripts/app_util.d.tl` (in the game folder).

Types of `/scripts/app_util.tl` (`AppUtil`) for choosing between mouse and gamepad behaviour.

## AppUtil (record)

```teal
global record AppUtil
```
Load: `local app_util = ug_require "::/scripts/app_util.tl" as AppUtil`

Input-mode helpers returned by `ug_require "/scripts/app_util.tl"`.

- `isGamepadInputMode : function() : boolean`  (call as `app_util.isGamepadInputMode(…)`): True if the game is currently in gamepad input mode.
    - used 36x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:105`: `not app_util.isGamepadInputMode() and builtin.ActionTooltip{`
- `getMouseOrGamepad : function<MouseType, GamepadTypeT>(mouse : MouseType, gamepad : GamepadTypeT) : MouseType | GamepadTypeT`  (call as `app_util.getMouseOrGamepad(…)`): Returns `gamepad` in gamepad input mode, otherwise `mouse`.

    ```lua
    crosshair = app_util.getMouseOrGamepad(false, true),
    ```

    - used 31x in the base game, e.g. `base/content/gui/gui/context_helper/context_helper_react.tl:299`: `text = app_util.getMouseOrGamepad(_("HELP_CONSTRUCTION_GENERATOR_STREETS"), _("HELP_CONSTRUCTION_GENERATOR_STREETS_CONTROLLER")),`
