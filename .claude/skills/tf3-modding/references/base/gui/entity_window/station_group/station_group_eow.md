# gui.entity_window.station_group.station_group_eow

Definition file: `base/tealdef/gui/entity_window/station_group/station_group_eow.d.tl` (in the game folder).

Extension point of the station window (`station_group_eow.script.tl`); the base game registers no plugins for it.

## StationGroupEow (record)

```teal
global record StationGroupEow
```
Load: `local station_group_eow = ug_require "::/gui/entity_window/station_group/station_group_eow.script.tl" as StationGroupEow`

Extension point of the station window.

- `StationGroupEowExtensionPoint : ExtensionPoint<StationGroupWidgetPluginParams>`: Extension point of the station window; register plugins with a resource of type `"react-plugin ::StationGroupEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/station_group/station_group.tl:864`: `local pluginWidgets = gui_react_util.usePlugins(station_group_eow.StationGroupEowExtensionPoint, nil, extensionParams)`

### StationGroupEow.StationGroupWidgetPluginParams (record)

```teal
record StationGroupWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the station window.
