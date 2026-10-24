# gui.entity_window.vehicle.vehicle_eow

Definition file: `base/tealdef/gui/entity_window/vehicle/vehicle_eow.d.tl` (in the game folder).

Plugin types of the vehicle window (`vehicle_eow.script.tl`).

## VehicleEow (record)

```teal
global record VehicleEow
```
Load: `local vehicle_eow = ug_require "::/gui/entity_window/vehicle/vehicle_eow.script.tl" as VehicleEow`

Plugin types of the vehicle window.

- `VehicleEowExtensionPoint : ExtensionPoint<VehicleWidgetPluginParams>`: Extension point of the vehicle window; register plugins with a resource of type `"react-plugin ::VehicleEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:246`: `for __, widget in ipairs(gui_react_util.usePlugins(vehicle_eow.VehicleEowExtensionPoint, {lastRef = lastInScrollAreaRef}, extensionParams)) do`
- `NotificationPlugin : Recipe<VehicleWidgetPluginParams>`: Notification cards of the vehicle; registered with order 0.
- `VehicleBasicsPlugin : Recipe<VehicleWidgetPluginParams>`: "Basics" card with the line, a card with the cargo on board (passengers table for own passenger vehicles) and a noise and pollution card; registered with order 10.
  In game: "Basics", "Stop {current} of {total}", "Going to Depot" (German: „Grundlagen“, „Haltestelle {current} von {total}“, „Unterwegs ins Depot“)
- `VehicleConditionPlugin : Recipe<VehicleWidgetPluginParams>`: "Condition" card with the condition bar and maintenance history, plus a card with the vehicle parts and their details; registered with order 50.
  In game: "Condition", "Vehicle Condition: {amount}", "Top Speed" (German: „Zustand“, „Fahrzeugzustand: {amount}“, „Höchstgeschwindigkeit“)
- `VehicleBalancePlugin : Recipe<VehicleWidgetPluginParams>`: "Balance" card with an income and running costs chart; registered with order 70 and condition `isPlayerOwned`.
  In game: "Balance", "Income", "Running Costs" (German: „Ertrag“, „Einnahmen“, „Betriebskosten“)
- `isPlayerOwned : function(VehicleWidgetPluginParams) : boolean`  (call as `vehicle_eow.isPlayerOwned(…)`): Plugin condition; true if the vehicle is owned by the local player.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/vehicle/vehicle.tl:290`: `local hasMaintenanceBar = vehicle_eow.isPlayerOwned(extensionParams)`

### VehicleEow.StaticVehicleInfoState (record)

```teal
record StaticVehicleInfoState
```

Vehicle data of the vehicle window that rarely changes.

- `modelIds : {integer}`: Model ids of the vehicle parts (e.g. locomotive and wagons), in order.
- `carrier : Carrier`: Carrier of the first vehicle part, from its `transportVehicle` metadata.
- `modifiers : Engine.Component.TransportVehicle.Modifiers`: Modifiers of the transport vehicle (`TRANSPORT_VEHICLE.modifiers`).
- `canTransportPassengers : boolean`: True if the vehicle has passenger capacity.
- `vehicleSpecialization : {string}`: Cargo classes the compartments of all parts can load.

### VehicleEow.VehicleEowState (record)

```teal
record VehicleEowState
```

State of the vehicle window passed to the plugins.

- `staticVehicleInfo : StaticVehicleInfoState`: Vehicle data that rarely changes.

### VehicleEow.VehicleWidgetPluginParams (record)

```teal
record VehicleWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the vehicle window.

- `state : VehicleEowState`: Window state.
