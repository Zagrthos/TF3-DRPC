# gui.entity_window.town_building.town_building_eow

Definition file: `base/tealdef/gui/entity_window/town_building/town_building_eow.d.tl` (in the game folder).

Extension point of the town building window (`town_building_eow.script.tl`); the base game registers no plugins for it.

## TownBuildingEow (record)

```teal
global record TownBuildingEow
```
Load: `local town_building_eow = ug_require "::/gui/entity_window/town_building/town_building_eow.script.tl" as TownBuildingEow`

Extension point of the town building window.

- `TownBuildingEowExtensionPoint : ExtensionPoint<TownBuildingWidgetPluginParams>`: Extension point of the town building window; register plugins with a resource of type `"react-plugin ::TownBuildingEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town_building/town_building.tl:334`: `for __, widget in ipairs(gui_react_util.usePlugins(town_building_eow.TownBuildingEowExtensionPoint, nil, extensionParams)) do`

### TownBuildingEow.TownBuildingWidgetPluginParams (record)

```teal
record TownBuildingWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the town building window.
