# gui.entity_window.animal.animal_eow

Definition file: `base/tealdef/gui/entity_window/animal/animal_eow.d.tl` (in the game folder).

Extension point and plugins of the animal window (`/gui/entity_window/animal/animal_eow.script.tl`).

## AnimalEow (record)

```teal
global record AnimalEow
```

Extension point and plugins of the animal window.

- `AnimalEowExtensionPoint : ExtensionPoint<AnimalWidgetPluginParams>`: Extension point of the animal window; register plugins with a resource of type `"react-plugin ::AnimalEowExtensionPoint"`.
- `AnimalStatePlugin : Recipe<AnimalWidgetPluginParams>`: Card with the description of the animal's current movement type (from the model's `animal` metadata); registered with order 0.
- `conditionAnimalStatePlugin : function(AnimalWidgetPluginParams) : boolean`

### AnimalEow.AnimalWidgetPluginParams (record)

```teal
record AnimalWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the animal window.
