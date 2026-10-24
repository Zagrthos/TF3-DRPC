# gui.menu.menu_util

Definition file: `base/tealdef/gui/menu/menu_util.d.tl` (in the game folder).

Small main-menu recipes: the user profile badge and the animated game-edition logo.

## MenuUtil (record)

```teal
global record MenuUtil
```
Load: `local menu_util = ug_require "::/gui/menu/menu_util.tl" as MenuUtil`

Util table of `menu_util.tl`.

- `UserProfile : Recipe0`: Recipe without parameters that shows a profile icon and the player's user name (`app.getUserProfile():getUserName()`).
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/license_page.tl:124`: `menu_util.UserProfile(),`
- `EditionLogo : Recipe<EditionLogoParams>`: Recipe of the animated game logo (shader quad with a shine effect that repeats every 37 seconds); shows the Deluxe logo when `gameEditionText.hasDeluxeDLC` is true.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/main_page.tl:732`: `menu_util.EditionLogo{`

### MenuUtil.EditionLogoParams (record)

```teal
record EditionLogoParams
```

Parameters of `MenuUtil.EditionLogo`.

- `gameEditionText : GameEditionTextParam`: Installed edition packs; only `hasDeluxeDLC` is used.
