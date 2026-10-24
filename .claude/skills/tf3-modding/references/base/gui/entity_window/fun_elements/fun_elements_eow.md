# gui.entity_window.fun_elements.fun_elements_eow

Definition file: `base/tealdef/gui/entity_window/fun_elements/fun_elements_eow.d.tl` (in the game folder).

Extension point and plugins of the window of fun elements such as UFOs and blimps (`fun_elements_eow.script.tl`).

## FunElementsEow (record)

```teal
global record FunElementsEow
```
Load: `local fun_elements_eow = ug_require "::/gui/entity_window/fun_elements/fun_elements_eow.script.tl" as FunElementsEow`

Extension point and plugins of the window of fun elements (UFOs, blimps and similar).

- `FunElementsEowExtensionPoint : ExtensionPoint<FunElementsWidgetPluginParams>`: Extension point of the fun element window; register plugins with a resource of type `"react-plugin ::FunElementsEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/fun_elements/fun_elements.tl:43`: `local widgets = gui_react_util.usePlugins(fun_elements_eow.FunElementsEowExtensionPoint, nil, extensionParams)`
- `FunElementsStatePlugin : Recipe<FunElementsWidgetPluginParams>`: Card with a table of the fun element's state (e.g. "Seeking", "Abducting", "Circling") and speed; registered with order 0.
  In game: "Abducting", "Clueless", "Circling" (German: „Entführt gerade“, „Ratlos“, „Kreisend“)

### FunElementsEow.FunElementsWidgetPluginParams (record)

```teal
record FunElementsWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the fun element window.
