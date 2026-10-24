# gui.line_vehicle_mgmt.vehicle_list_react_util

Definition file: `base/tealdef/gui/line_vehicle_mgmt/vehicle_list_react_util.d.tl` (in the game folder).

The vehicle list table of the line and vehicle manager (vehicle selection checkbox, line or depot, name and icons per row).

## VehicleListReactUtil (record)

```teal
global record VehicleListReactUtil
```

Vehicle list table of the line and vehicle manager.

- `VehicleList : Recipe<VehicleListParams>`: Recipe of the vehicle list, a keyboard-navigable `DataTable` sorted by line by default.
  In game: "Select Vehicle", "Locate Vehicle", "Going to Depot" (German: „Fahrzeug auswählen“, „Fahrzeug anpeilen“, „Unterwegs ins Depot“)

### VehicleListReactUtil.VehicleListParams (record)

```teal
record VehicleListParams
```

Parameters of `VehicleList`.

- `vehicles : {Engine.Entity}`: Vehicle entities shown as rows.
- `managerRef : ReactRefWrapApiT<VehicleManagerParams, VehicleManagerApi>`: Ref to the vehicle manager; its API is used to read and change the vehicle selection.
- `managerActionButtonsRef : ReactRefWrap`: Passed by the manager window but not used by `VehicleList` itself.
- `commonParams : CommonActionParams`: Shared action parameters of the manager window (gamepad mode, more-options state, ...).
- `onClickSelectLineOrDepot : function(entity : Engine.Entity, isLine : boolean)`: Called when the line / depot button of a row is clicked; `isLine` tells which one the entity is.
- `onClickSelectVehicle : function(vehicleEntity : Engine.Entity)`: Called when a vehicle row is clicked (the manager toggles the vehicle's selection). Double-clicking a row makes the camera follow the vehicle.
