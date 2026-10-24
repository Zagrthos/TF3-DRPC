# gui.entity_window.sim_person.sim_person_eow

Definition file: `base/tealdef/gui/entity_window/sim_person/sim_person_eow.d.tl` (in the game folder).

Plugin types of the resident (sim person) window (`sim_person_eow.script.tl`).

## SimPersonEow (record)

```teal
global record SimPersonEow
```
Load: `local sim_person_eow = ug_require "::/gui/entity_window/sim_person/sim_person_eow.script.tl" as SimPersonEow`

Plugin types of the resident (sim person) window.

- `SimPersonEowExtensionPoint : ExtensionPoint<SimPersonWidgetPluginParams>`: Extension point of the resident window; register plugins with a resource of type `"react-plugin ::SimPersonEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/sim_person/sim_person.tl:135`: `local pluginWidgets = gui_react_util.usePlugins(sim_person_eow.SimPersonEowExtensionPoint, {firstRef = initialFocusRef}, extensionParams)`
- `SimPersonLocationsPlugin : Recipe<SimPersonWidgetPluginParams>`: "Locations" card with the home, shop and work destinations of the person; registered with order 20.
  In game: "Locations" (German: „Standorte“)
- `SimPersonHappinessPlugin : Recipe<SimPersonWidgetPluginParams>`: "Happiness" card with the happiness bar and a collapsible happiness history; registered with order 30.
  In game: "Happiness", "This resident is happy.", "Waiting at Station" (German: „Zufriedenheit“, „Dieser Einwohner ist zufrieden.“, „Wartet an Haltestelle“)
- `SimPersonRoutePlugin : Recipe<SimPersonWidgetPluginParams>`: "Current Route" card with origin, move mode, line and target; registered with order 40.
  In game: "Current Route" (German: „Aktuelle Route“)

### SimPersonEow.SimPersonEowState (record)

```teal
record SimPersonEowState
```

State of the resident window, recomputed every step and passed to the plugins.

- `happiness : number`: Happiness of the person (`SIM_PERSON.happiness`, up to 1).
- `changeTendency : number`: Shown trend of the happiness, either 0.05 (rising, e.g. while at a building), -0.05 (falling, e.g. when late or waiting more than two minutes at a terminal) or 0.
- `isInCar : boolean`: True if the person currently drives a car (has a `LAND_VEHICLE` component).
- `destinationTargetIndex : integer`: Index of the destination the person is heading to or is at (0 home, 1 shop, 2 work).
- `moveMode : MoveMode`: Move mode of the current trip (`SIM_PERSON.lastMoveMode`).
- `destinationOriginIndex : integer`: Destination index the trip started from; for trips home it is the last non-residential destination type, otherwise 0.
- `lineId : Engine.Entity`: Line of the vehicle the person is in or waiting for, or nil.
- `atBuilding : boolean`: True while the person is at a building.

### SimPersonEow.SimPersonWidgetPluginParams (record)

```teal
record SimPersonWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the resident window.

- `state : SimPersonEowState`: Window state.
