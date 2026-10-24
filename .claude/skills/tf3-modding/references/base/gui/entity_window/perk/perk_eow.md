# gui.entity_window.perk.perk_eow

Definition file: `base/tealdef/gui/entity_window/perk/perk_eow.d.tl` (in the game folder).

Plugin types of the perk window (landmarks, headquarters and other company buildings) (`perk_eow.script.tl`).

## PerkEow (record)

```teal
global record PerkEow
```
Load: `local perk_eow = ug_require "::/gui/entity_window/perk/perk_eow.script.tl" as PerkEow`

Plugin types of the perk window (landmarks, headquarters).

- `PerkEowExtensionPoint : ExtensionPoint<PerkWidgetPluginParams>`: Extension point of the perk window; register plugins with a resource of type `"react-plugin ::PerkEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/perk/perk.tl:109`: `local pluginWidgets = gui_react_util.usePlugins(perk_eow.PerkEowExtensionPoint, {lastRef = lastInScrollAreaRef}, extensionParams)`
- `PerkBasicsPlugin : Recipe<PerkWidgetPluginParams>`: Cards with the construction description and numbers, noise and pollution (if any) and the production rules (if any); registered with order 10.
  In game: "Gather and assemble all the necessary resources to complete the project and bring this magnificent structure to life." (German: „Sammle und stelle alle notwendigen Ressourcen zusammen, um das Projekt abzuschliessen und dieses grossartige Bauwerk zum Leben zu erwecken.“)
- `PerkCombinedStocksPlugin : Recipe<PerkWidgetPluginParams>`: Simplified input and output stock cards of the stock list entity; registered with order 40 and condition `hasStockListEntity`.
- `PerkBoosterPlugin : Recipe<PerkWidgetPluginParams>`: Card with the workers boost row and the workers table; registered with order 50 and condition `hasPersonCapacityAndStockListEntity`.
- `PerkPersonCapacityPlugin : Recipe<PerkWidgetPluginParams>`: Card with the person capacity and the table of visitors or workers; registered with order 60 and condition `hasPersonCapacityButNotStockListEntity`.
- `PerkMaintenanceBarPlugin : Recipe<PerkWidgetPluginParams>`: Yearly upkeep bar of the construction; registered with order 70.
- `conditionPerkEmission : function(params : PerkEow.PerkWidgetPluginParams) : boolean`  (call as `perk_eow.conditionPerkEmission(…)`): True if the perk has noise or pollution; decides whether `PerkBasicsPlugin` shows the emission card.
- `conditionPerkProductionRule : function(params : PerkEow.PerkWidgetPluginParams) : boolean`  (call as `perk_eow.conditionPerkProductionRule(…)`): True if the perk has a stock list with production rules; decides whether `PerkBasicsPlugin` shows the production card.
- `hasStockListEntity : function(params : PerkEow.PerkWidgetPluginParams) : boolean`  (call as `perk_eow.hasStockListEntity(…)`): Plugin condition; true if the perk has a stock list entity.
- `hasPersonCapacityAndStockListEntity : function(params : PerkEow.PerkWidgetPluginParams) : boolean`  (call as `perk_eow.hasPersonCapacityAndStockListEntity(…)`): Plugin condition; true if the perk has both a person capacity and a stock list entity.
- `hasPersonCapacityButNotStockListEntity : function(params : PerkEow.PerkWidgetPluginParams) : boolean`  (call as `perk_eow.hasPersonCapacityButNotStockListEntity(…)`): Plugin condition; true if the perk has a person capacity entity but no stock list entity.
- `NotificationPlugin : Recipe<PerkWidgetPluginParams>`: Notification cards of the perk; registered with order 0.

### PerkEow.PerkEowState (record)

```teal
record PerkEowState
```

State of the perk window, recomputed every step and passed to the plugins.

- `personCapacityEntity : Engine.Entity`: First subconstruction, if it has a `PERSON_CAPACITY` component.
- `stockListEntity : Engine.Entity`: First subconstruction, if it has a `STOCK_LIST` component.
- `hasStockListWithRules : boolean`: True if the stock list has production rules.
- `boosted : boolean`: True if the stock list entity is boosted by its workers.
- `isHeadquarters : boolean`: True if the construction has slots and its company metadata marks it as headquarters.
- `cargoTypes : {CargoTypeId : CargoUtil.StockProperties}`: Cargo types of the stock list with input/output flags.
- `noise : integer`: Sum of the noise power of all subconstructions with an emission emitter, rounded.
- `pollution : integer`: Sum of the pollution power of all subconstructions with an emission emitter, rounded.

### PerkEow.PerkWidgetPluginParams (record)

```teal
record PerkWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the perk window.

- `state : PerkEowState`: Window state.
