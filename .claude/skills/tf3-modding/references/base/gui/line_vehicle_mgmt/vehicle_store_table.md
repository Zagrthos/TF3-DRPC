# gui.line_vehicle_mgmt.vehicle_store_table

Definition file: `base/tealdef/gui/line_vehicle_mgmt/vehicle_store_table.d.tl` (in the game folder).

Parameters of the table layout of the vehicle store (`/gui/line_vehicle_mgmt/vehicle_store_table.tl`, which returns the recipe directly).

## VehicleStoreTableRecipeParams (record)

```teal
global record VehicleStoreTableRecipeParams
```
Load: `local VehicleStoreTable = ug_require "::/gui/line_vehicle_mgmt/vehicle_store_table.tl" as VehicleStoreTableRecipeParams`

Parameters of the table layout of the vehicle store.

- `filteredVectorState : ReactStateT<{VehicleStoreUtil.Element}>`: variants are mixed in when in table layout
- `filter : VehicleStore.VehicleBrowserFilter`: Current browser filter; also chooses the column set (engines only, cargo or passenger).
- `vehicleStoreMode : VehicleStore.VehicleStoreMode`: Store mode; decides what the buy button shows.
- `setFilterSort : function(newFilter : VehicleStore.VehicleBrowserFilter, newSort : VehicleStore.VehicleBrowserSort, forceUpdate : boolean)`  (call as `VehicleStoreTable.setFilterSort(…)`): Updates filter and sort; the table calls it once on mount with `(nil, nil, true)` to force a refresh.
- `cartRef : ReactRefWrapApi0<VehicleStore.VehicleCartApi>`: Ref to the cart, used for price checks and adding vehicles.
