# gui.main.radial_menu_extension

Definition file: `base/tealdef/gui/main/radial_menu_extension.d.tl` (in the game folder).

Extension point through which mods add entries to the main radial menu of the game bar.

## RadialMenuExtension (record)

```teal
global record RadialMenuExtension
```
Load: `local radial_menu_extension = ug_require "::/gui/main/radial_menu_extension.tl" as RadialMenuExtension`

Holds the extension point for the main radial menu.

- `RadialMenuExtension : WrapperExtensionPoint0<Builtin.RadialMenuChild>`: Wrapper extension point for `builtin.RadialMenuChild` entries; the plugins are appended after the base game's entries of the main radial menu.
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:54`: `local radialMenuPlugins = gui_react_util.usePlugins(radial_menu_extension.RadialMenuExtension)`
