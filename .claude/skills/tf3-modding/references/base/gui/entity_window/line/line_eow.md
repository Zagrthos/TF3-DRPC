# gui.entity_window.line.line_eow

Definition file: `base/tealdef/gui/entity_window/line/line_eow.d.tl` (in the game folder).

Plugins and widgets of the line window (`line_eow.script.tl`).

## LineEow (record)

```teal
global record LineEow
```
Load: `local line_eow = ug_require "::/gui/entity_window/line/line_eow.script.tl" as LineEow`

Plugins and widgets of the line window.

- `LineEowExtensionPoint : ExtensionPoint<LineWidgetPluginParams>`: Extension point of the line window; register plugins with a resource of type `"react-plugin ::LineEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/line/line.tl:132`: `local pluginWidgets = gui_react_util.usePlugins(line_eow.LineEowExtensionPoint, {lastRef = lastInScrollAreaRef}, extensionParams)`
- `LineFrequency : Recipe<Engine.Entity>`: Stopwatch icon with the line frequency (`line_util.calculateFrequencySeconds`, "--" when 0); the parameter is the line entity.
  In game: "Frequency: {count}" (German: „Takt: {count}“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:980`: `lineEntity and line_eow.LineFrequency(lineEntity or -1) or nil,`
- `LineRate : Recipe<Engine.Entity>`: Cargo icon with the line rate (`api.engine.util.line.calcLineStationThroughput`); the parameter is the line entity.
  In game: "Rate: {count}" (German: „Rate: {count}“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle_eow.script.tl:979`: `lineEntity and line_eow.LineRate(lineEntity or -1) or nil,`
- `NotificationPlugin : Recipe<LineWidgetPluginParams>`: Notification cards of the line; registered with order 0.
- `LineColorPickerPlugin : Recipe<LineWidgetPluginParams>`: Colour picker of the line; registered with order 10.
- `LineBasicsPlugin : Recipe<LineWidgetPluginParams>`: "Basics" card with the line's carriage information and a card with the summed noise and pollution of all its vehicles; registered with order 20.
  In game: "Basics" (German: „Grundlagen“)
- `LineTransportedGraphPlugin : Recipe<LineWidgetPluginParams>`: "Transported" card with last year's transported amount and a collapsible chart of the `itemsTransported` log; registered with order 30 and condition `conditionPlayerOwned`.
  In game: "Transported" (German: „Transportiert“)
- `LineVehiclesPlugin : Recipe<LineWidgetPluginParams>`: "Vehicles" card with the vehicles of the line; registered with order 50.
  In game: "Vehicles", "Vehicle", "Age" (German: „Fahrzeuge“, „Fahrzeug“, „Alter“)
- `LineIncomeGraphPlugin : Recipe<LineWidgetPluginParams>`: "Balance" card with an income and running costs chart of the line's account; registered with order 60 and condition `conditionPlayerOwned`.
  In game: "Balance", "Income", "Running Costs" (German: „Ertrag“, „Einnahmen“, „Betriebskosten“)
- `conditionPlayerOwned : function(params : LineEow.LineWidgetPluginParams) : boolean`  (call as `line_eow.conditionPlayerOwned(…)`): Plugin condition; true unless the line is owned by another player (also true for lines without owner).

### LineEow.LineWidgetPluginParams (record)

```teal
record LineWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the line window.
