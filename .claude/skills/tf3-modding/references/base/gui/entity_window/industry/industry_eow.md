# gui.entity_window.industry.industry_eow

Definition file: `base/tealdef/gui/entity_window/industry/industry_eow.d.tl` (in the game folder).

Plugin types of the industry window (`industry_eow.script.tl`).

## IndustryEow (record)

```teal
global record IndustryEow
```
Load: `local industry_eow = ug_require "::/gui/entity_window/industry/industry_eow.script.tl" as IndustryEow`

Plugin types of the industry window.

- `IndustryEowExtensionPoint : ExtensionPoint<IndustryWidgetPluginParams>`: Extension point of the industry window; register plugins with a resource of type `"react-plugin ::IndustryEowExtensionPoint"`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:300`: `widgets = table_util.concatenate(widgets, gui_react_util.usePlugins(industry_eow.IndustryEowExtensionPoint, { firstRef = initialFocusRef, }, extensionParams))`
- `NotificationPlugin : Recipe<IndustryWidgetPluginParams>`: Notification cards of the industry; registered with order 0.
- `IndustryProductionRulesPlugin : Recipe<IndustryWidgetPluginParams>`: "Production" card with the production rules (`IndustryReactUtil.ProductionRulesWidget`); registered with order 10 and condition `conditionProductionRules`.
  In game: "Production" (German: „Produktion“)
- `IndustryBoostersPlugin : Recipe<IndustryWidgetPluginParams>`: "Booster(s)" card with cargo booster rules and the workers boost, with the workers table as collapsible part; registered with order 20 and condition `conditionBoosters`.
  In game: "Booster / Boosters" (German: „Booster / Booster“)
- `IndustryCombinedStockGroupedCountsPlugin : Recipe<IndustryWidgetPluginParams>`: Input and output stock cards with suppliers and consumers tables as collapsible parts; registered with order 30 and condition `conditionPlayerOwnedOrNotOwned`.
  In game: "Supplier", "Consumer", "Connect this location with the transport system to allow for cargo transport to this location." (German: „Lieferant“, „Abnehmer“, „Verbinde diesen Standort mit dem Transportsystem, um den Gütertransport zu diesem Standort zu ermöglichen.“)
- `IndustryBalanceChartPlugin : Recipe<IndustryWidgetPluginParams>`: "Cargo Flow" chart of consumed, produced and destroyed cargo; registered with order 40 and condition `conditionPlayerOwnedOrNotOwned`.
  In game: "Cargo Flow", "Produced", "Consumed" (German: „Güterfluss“, „Produziert“, „Verbraucht“)
- `conditionProductionRules : function(params : IndustryEow.IndustryWidgetPluginParams) : boolean`  (call as `industry_eow.conditionProductionRules(…)`): Plugin condition; true if the industry has non-booster production rules.
    - returns: `params.state.addProductionRules`.
- `conditionBoosters : function(params : IndustryEow.IndustryWidgetPluginParams) : boolean`  (call as `industry_eow.conditionBoosters(…)`): Plugin condition; true if the industry is not owned by another player and has production rules.
- `conditionPlayerOwnedOrNotOwned : function(params : IndustryEow.IndustryWidgetPluginParams) : boolean`  (call as `industry_eow.conditionPlayerOwnedOrNotOwned(…)`): Plugin condition; true unless the industry's construction is owned by another player.

### IndustryEow.IndustryEowState (record)

```teal
record IndustryEowState
```

State of the industry window, recomputed every step and passed to the plugins.

- `addProductionRules : boolean`: True if the stock list has at least one production rule that is not a booster.
- `addBoosters : boolean`: True if there are booster rules or workplaces.
- `numBoosters : integer`: Number of booster rules, plus one if the industry has workplaces.
- `addWorkplaces : boolean`: True if the industry has workplaces (`PERSON_CAPACITY` with capacity above 0).
- `editConstructionEntity : Engine.Entity`: Construction to configure, set when it is owned by the player and modifiable.
- `cargoTypes : {CargoTypeId : CargoUtil.StockProperties}`: Cargo types of the industry's stock list with input/output flags.
- `modifiers : {string : integer}`: Modifiers of the industry; currently only `ecoLevel` from the emissions game script, or nil.
- `hadConsumed : boolean`: True if a production rule consumes input or the logbook has an `itemsConsumed` log; adds "Consumed" to the cargo flow chart.
- `maxLevel : boolean`: True if the industry has reached its maximum level.
- `failedExpansion : boolean`: True if the last expansion attempt was blocked (there are colliding areas).
- `cargoFlowData : CargoFlowData`: Data for the cargo flow arrows.

#### IndustryEow.IndustryEowState.CargoFlowData (record)

```teal
record CargoFlowData
```

Data for the cargo flow arrows drawn while the window is open.

- `this : Vec3f`: Position of the industry (top centre of its bounding box).
- `suppliers : {CargoUtil.CargoFlowData}`: Industries supplying this industry, with position, cargo type and amount.
- `consumers : {CargoUtil.CargoFlowData}`: Industries and towns consuming this industry's output, with position, cargo type and amount.

### IndustryEow.IndustryWidgetPluginParams (record)

```teal
record IndustryWidgetPluginParams is EowExtensionUtil.IEowWidgetsExtensionParams
```

Parameters passed to the plugins of the industry window.

- `state : IndustryEowState`: Window state.
