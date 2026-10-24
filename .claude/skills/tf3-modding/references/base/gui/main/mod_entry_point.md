# gui.main.mod_entry_point

Definition file: `base/tealdef/gui/main/mod_entry_point.d.tl` (in the game folder).

Extension point through which mods add their own top-level React UI to the game view.

## ModEntryPoint (record)

```teal
global record ModEntryPoint
```
Load: `local mod_entry_point = ug_require "::/gui/main/mod_entry_point.tl" as ModEntryPoint`

Holds the extension point for mod UI in the game view.

- `ModEntryPointExtension : ExtensionPoint0`: Extension point for mod UI; each plugin recipe registered with `react.RegisterPluginRecipe(mod_entry_point.ModEntryPointExtension, name, fn)` is placed in the game UI root as a full-size floating layout child.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:83`: `local pluginInstances = gui_react_util.usePlugins(mod_entry_point.ModEntryPointExtension)`
