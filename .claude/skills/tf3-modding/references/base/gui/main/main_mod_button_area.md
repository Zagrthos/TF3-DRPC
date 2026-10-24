# gui.main.main_mod_button_area

Definition file: `base/tealdef/gui/main/main_mod_button_area.d.tl` (in the game folder).

Horizontal button bar for mods in the game UI. Mods add buttons through the
`MainModButtonAreaExtension` extension point; when at least one plugin exists, the radial
menu gets a "Mods" entry that focuses the bar.

## MainModButtonAreaParams (record)

```teal
global record MainModButtonAreaParams
```

Parameters of the `MainModButtonArea` recipe (none).

## MainModButtonArea (record)

```teal
global record MainModButtonArea
```
Load: `local main_mod_button_area = ug_require "::/gui/main/main_mod_button_area.tl" as MainModButtonArea`

Mod button bar of the game UI and its extension point.

- `MainModButtonAreaExtension : ExtensionPoint0`: Extension point for mod buttons; register a recipe with `react.RegisterPluginRecipe(main_mod_button_area.MainModButtonAreaExtension, name, fn)`.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:380`: `local mainModButtonAreaPlugins = react.getPlugins(main_mod_button_area.MainModButtonAreaExtension)`
- `MainModButtonArea : Recipe<MainModButtonAreaParams>`: Recipe that lays out all plugins of `MainModButtonAreaExtension` in a horizontal scroll area; it becomes focusable on the `focusMainModButtonArea` event and leaves focus on `IA_MENU_BACK`.
    - used 2x in the base game, e.g. `base/content/gui/gui/main/game.tl:252`: `local modButtonAreaRef = react.useNodeRef(main_mod_button_area.MainModButtonArea)`
