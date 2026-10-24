# gui.entity_window.warehouse.warehouse_eow

Definition file: `base/tealdef/gui/entity_window/warehouse/warehouse_eow.d.tl` (in the game folder).

Plugin types of the warehouse window (`warehouse_eow.script.tl`).

## WarehouseEow (record)

```teal
global record WarehouseEow
```
Load: `local warehouse_eow = ug_require "::/gui/entity_window/warehouse/warehouse_eow.script.tl" as WarehouseEow`

Plugin types of the warehouse window.

- `WarehouseStocksPlugin : Recipe<WarehouseEow.WarehouseWidgetPluginParams>`: "Stocks" card with the stored cargo; when modifiable, a collapsible part with the stocks, discard buttons and cargo type pin buttons; registered with order 20.
  In game: "Stocks", "No Cargo Currently Stored" (German: „Bestände“, „Derzeit keine Güter gelagert“)
- `WarehouseDiagramPlugin : Recipe<WarehouseEow.WarehouseWidgetPluginParams>`: "Statistics" chart of incoming and outgoing cargo (`itemsUnloaded`, `itemsLoaded`); registered with order 30 and condition `isModifiable`.
  In game: "Statistics", "Incoming", "Outgoing" (German: „Statistiken“, „Eingehend“, „Ausgehend“)
- `isModifiable : function(WarehouseEow.WarehouseWidgetPluginParams) : boolean`  (call as `warehouse_eow.isModifiable(…)`): Plugin condition; true if the warehouse is owned by the player or by nobody.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/warehouse/warehouse.tl:66`: `if warehouse_eow.isModifiable(extensionParams) then`
- `WarehouseEowExtensionPoint : ExtensionPoint<WarehouseWidgetPluginParams>`: Extension point of the warehouse window; register plugins with a resource of type `"react-plugin ::WarehouseEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/warehouse/warehouse.tl:58`: `for __, widget in ipairs(gui_react_util.usePlugins(warehouse_eow.WarehouseEowExtensionPoint, {lastRef = lastInScrollAreaRef}, extensionParams)) do`

### WarehouseEow.WarehouseEowState (record)

```teal
record WarehouseEowState
```

State of the warehouse window passed to the plugins.

- `isModifiable : boolean`: True if the warehouse's construction is owned by the player or by nobody.

### WarehouseEow.WarehouseWidgetPluginParams (record)

```teal
record WarehouseWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the warehouse window.

- `state : WarehouseEowState`: Window state.
