# gui.entity_window.empty_construction.empty_construction_eow

Definition file: `base/tealdef/gui/entity_window/empty_construction/empty_construction_eow.d.tl` (in the game folder).

Extension point and plugins of the window of constructions without a more specific window (`empty_construction_eow.script.tl`).

## EmptyConstructionEow (record)

```teal
global record EmptyConstructionEow
```
Load: `local empty_construction_eow = ug_require "::/gui/entity_window/empty_construction/empty_construction_eow.script.tl" as EmptyConstructionEow`

Extension point and plugins of the window of constructions without a more specific window ("Empty Construction").

- `EmptyConstructionEowExtensionPoint : ExtensionPoint<EmptyConstructionWidgetPluginParams>`: Extension point of the construction window; register plugins with a resource of type `"react-plugin ::EmptyConstructionEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/empty_construction/empty_construction.tl:37`: `local widgets = gui_react_util.usePlugins(empty_construction_eow.EmptyConstructionEowExtensionPoint, nil, extensionParams)`
- `NotificationPlugin : Recipe<EmptyConstructionWidgetPluginParams>`: Notification cards of the construction (`EntityWindowUtil.NotificationWidget`); registered with order 0.

### EmptyConstructionEow.EmptyConstructionWidgetPluginParams (record)

```teal
record EmptyConstructionWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the construction window.
