# gui.main.industry_util

Definition file: `base/tealdef/gui/main/industry_util.d.tl` (in the game folder).

Helpers for the industry UI: production boosts (worker boost and booster rules), input and
output data per rule, production numbers, satellite slots, connection checks, removing
industries, and finding suppliers and consumers. Load it with
`ug_require "/gui/main/industry_util.tl"`.

## IndustryUtil (record)

```teal
global record IndustryUtil
```
Load: `local industry_util = ug_require "::/gui/main/industry_util.tl" as IndustryUtil`

Helpers that read industry stock lists and rules for the UI.

- `getPersonCapacityBoostProgress : function(entity : Engine.Entity) : number`  (call as `industry_util.getPersonCapacityBoostProgress(…)`): Returns how close an industry is to the worker boost, from `0` to `1`.
    - returns: Occupied workplaces divided by capacity, mapped from `0..RequiredWorkersFractionForBoost` to `0..1`; `0` without a `PERSON_CAPACITY` component.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_workers/deliver_workers.script.tl:245`: `value = not entity_util.entityChanged0(data.to) and industry_util.getPersonCapacityBoostProgress(data.to.entity) or 0.0,`
- `isPersonCapacityBoosted : function(entity : Engine.Entity) : boolean`  (call as `industry_util.isPersonCapacityBoosted(…)`): Tells whether an industry has enough workers for the worker boost (`industry_productivity_util.isIndustryWorkersBoosted`).
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_workers/deliver_workers.script.tl:125`: `if not industry_util.isPersonCapacityBoosted(entity) and industry_util.isIndustryConnected(entity) then`
- `getPersonCapacityBoost : function(entity : Engine.Entity) : number`  (call as `industry_util.getPersonCapacityBoost(…)`): Returns the productivity factor of an industry from its workers (`industry_productivity_util.getIndustryProductivity`).
- `isEntityBoosted : function(boosterIndex : integer, entity : Engine.Entity, excludePersons? : boolean) : boolean`  (call as `industry_util.isEntityBoosted(…)`): Tells whether the n-th booster rule of an industry is active.
    - param `boosterIndex`: 1-based index among the booster rules of the stock list.
    - param `entity`: Entity with a `STOCK_LIST` component.
    - param `excludePersons`: If `true`, returns `false` when there is no such booster rule; otherwise falls back to `isPersonCapacityBoosted`.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:558`: `local isBoosted = industry_util.isEntityBoosted(i, param.entity)`
- `getIndustryBoost : function(Engine.Entity, boolean, ?integer) : IndustryBoost`  (call as `industry_util.getIndustryBoost(…)`): Computes the production boost of an industry from its booster rules and optionally its workers.
    - param `#1`: Entity with a `STOCK_LIST` component.
    - param `#2`: If `true`, starts from the worker productivity factor instead of `1`.
    - param `#3`: If set, only rules up to this index are checked.
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/industry_react_util.tl:36`: `boost = industry_util.getIndustryBoost(entity, true, nil).boost`
- `getStockListInputOutputData : function(Engine.Entity, boolean, boolean, integer) : {StockListCargoData}`  (call as `industry_util.getStockListInputOutputData(…)`): Lists the input or output cargo types of a stock list with the rules that use them.
    - param `#1`: Entity with a `STOCK_LIST` component.
    - param `#2`: `true` for inputs, `false` for outputs.
    - param `#3`: `true` for booster rules only, `false` for normal rules only, `nil` for both.
    - param `#4`: Maximum number of cargo types to return; `0` for no limit.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/industry_react_util.tl:382`: `local inputData = industry_util.getStockListInputOutputData(params.entity, true, false, params.ruleNumber)`
- `getIndustryProductionData : function(entity : Engine.Entity, input : boolean, booster : boolean, index : integer, isIndustry : boolean) : ProductionData`  (call as `industry_util.getIndustryProductionData(…)`): Returns current and maximum consumption or production per year for one input or output cargo type.
    - param `entity`: Entity with a `STOCK_LIST` component.
    - param `input`: `true` for an input, `false` for an output.
    - param `booster`: Rule selection as in `getStockListInputOutputData`.
    - param `index`: 1-based index of the cargo type in the result of `getStockListInputOutputData`.
    - param `isIndustry`: If `true`, applies `api.res.getBaseConfig().locations.industry.industryProductivity`.
    - returns: The production data, or `nil` if there is no cargo type at `index`.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:65`: `productionData = industry_util.getIndustryProductionData(params.entity, params.isInput, true, inputOutputData.adjustedIndex, params.isIndustry)`
- `getSatellitesNumAndMax : function(Engine.Entity) : SatellitesNumAndMax`  (call as `industry_util.getSatellitesNumAndMax(…)`): Returns the filled and total auto-fill slots of the construction that contains the entity.
- `createInputOutputData : function(RuleInputOutputCargoParams) : RuleInputOutputData`  (call as `industry_util.createInputOutputData(…)`): Returns the content of one input or output slot of an industry rule for display.
    - notes: For normal rules of constructions with satellite slots, input slot `1` is the satellite count. For booster rules, output slot `1` is the boost percentage. The other slots are shifted by one in these cases. Only the first input set of a rule is used.
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:62`: `local inputOutputData = industry_util.createInputOutputData(params)`
- `getNumCargosOfRule : function(Engine.Entity, integer, boolean) : integer`  (call as `industry_util.getNumCargosOfRule(…)`): Returns the number of input or output slots of a rule, including the satellite input or boost output slot.
    - param `#1`: Entity with a `STOCK_LIST` component.
    - param `#2`: 1-based rule index.
    - param `#3`: `true` to count inputs, `false` to count outputs.
    - returns: Slot count, at least `1`.
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/industry_react_util.tl:216`: `return industry_util.getNumCargosOfRule(params.entity, params.ruleNumber, params.isInput)`
- `getSortedOutputCargoTypes : function(entity : Engine.Entity) : {CargoTypeId}`  (call as `industry_util.getSortedOutputCargoTypes(…)`): Returns the output cargo types of all rules of an industry, sorted by id.
    - param `entity`: Industry entity (with an `INDUSTRY` component).
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:392`: `return industry_util.getSortedOutputCargoTypes(otherEntity)[1] -- always one cargo type per entry`
- `calcAnnualShipment : function(entity : Engine.Entity) : integer`  (call as `industry_util.calcAnnualShipment(…)`): Returns the cargo shipped per year from an industry's stock list.
    - param `entity`: Industry entity.
    - returns: Items per year, or `nil` if the industry has no rules.
    - used 6x in the base game, e.g. `base/content/gui/gui/statistics/statistic_industries.tl:330`: `return industry_util.calcAnnualShipment(entity)`
- `isIndustryConnected : function(entity : Engine.Entity) : boolean`  (call as `industry_util.isIndustryConnected(…)`): Tells whether an industry ships cargo, based on `api.engine.util.stock.getCargoShippedPerYear` of its stock list.
    - param `entity`: Industry entity.
    - notes: Delivered cargo (`getCargoDeliveredPerYear`) is only checked for rules whose `output` is `nil`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_workers/deliver_workers.script.tl:125`: `if not industry_util.isPersonCapacityBoosted(entity) and industry_util.isIndustryConnected(entity) then`
- `removeIndustries : function(industriesToRemove : {Engine.Entity}, followupFn : function(success : boolean), doDust : boolean)`  (call as `industry_util.removeIndustries(…)`): Removes industries by sending build proposals that remove their constructions, then removes the street segments that belong to them.
    - param `industriesToRemove`: Industry entities.
    - param `followupFn`: Called with the success of the last command, or with `true` at once if there is nothing to remove.
    - param `doDust`: Passed to `api.cmd.makeWorldBuildProposalCmd` (show demolition dust).
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/industries/industries.script.tl:222`: `industry_util.removeIndustries(industriesToRemove, followupFn, true)`
- `forEachMatchingStocklist : function(stockList : Engine.Component.StockList, stockListType : StockListType, callback : function(Engine.Entity, CargoTypeId)) : {CargoTypeId : boolean}`  (call as `industry_util.forEachMatchingStocklist(…)`): return input/output cargo types (depending on argument stockListType)

Calls `callback` for every industry that has a stock of the counterpart type for one of the given stock list's cargo types.
    - param `stockList`: Stock list component of the industry to look at.
    - param `stockListType`: `InputStock` finds suppliers (industries with a matching output stock); `OutputStock` finds consumers (industries with a matching input stock).
    - param `callback`: Called with the found industry entity and the cargo type.
    - returns: Set of the input or output cargo types of `stockList`.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:418`: `industry_util.forEachMatchingStocklist(stockList,`
- `getFailedExpansionTriangles : function(proposal : Proposal, collidingSubconstructions : {integer}) : {{Vec2f, Vec2f, Vec2f}}`  (call as `industry_util.getFailedExpansionTriangles(…)`): Returns ground triangles (x/y) of the colliding subconstructions of a construction proposal, for drawing where an expansion failed.
    - param `proposal`: Proposal whose first added construction is checked.
    - param `collidingSubconstructions`: 0-based indices of the colliding subconstructions.
    - notes: Uses quad point-cloud and box colliders of the subconstruction; if none are found, the lot triangles of the subconstruction.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/industries/industries.script.tl:22`: `local triangles = industry_util.getFailedExpansionTriangles(proposal, collidingSubconstructions)`
- `forEachTownSupplier : function(townEntity : Engine.Entity, callback : function(Engine.Entity, CargoTypeId))`  (call as `industry_util.forEachTownSupplier(…)`): Calls `callback` for every industry output stock whose cargo type the town has a supply limit for.
    - param `townEntity`: Town entity.
    - param `callback`: Called with the industry entity and the cargo type.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo_town/deliver_cargo_town.script.tl:511`: `industry_util.forEachTownSupplier(data.townEntity.entity, function(industryEntity : Engine.Entity, cargoType : CargoTypeId)`

### IndustryUtil.IndustryBoost (record)

```teal
record IndustryBoost
```

Production boost of an industry, as returned by `getIndustryBoost`.

- `boost : number`: Total production factor (worker boost if requested, multiplied by `1 + boostFactor` of every active booster rule).
- `ruleBoosts : {integer : RuleBoost}`: Booster rule states by rule index (1-based). Non-booster rules have no entry.

#### IndustryUtil.IndustryBoost.RuleBoost (record)

```teal
record RuleBoost
```

State of one booster rule.

- `boosted : boolean`: `true` if the rule is running or its input stocks hold enough cargo to run.
- `boostFactor : number`: Boost factor of the rule (`rule.boostFactor`); an active rule multiplies production by `1 + boostFactor`.

### IndustryUtil.StockListCargoData (record)

```teal
record StockListCargoData
```

One input or output cargo type of a stock list with the rules that use it, as returned by `getStockListInputOutputData`.

- `cargoType : CargoTypeId`: Cargo type id.
- `rulesData : {RuleInputOutputIndex}`: Rules that use the cargo type.

#### IndustryUtil.StockListCargoData.RuleInputOutputIndex (record)

```teal
record RuleInputOutputIndex
```

Reference to a rule that uses the cargo type.

- `ruleIndex : integer`: 1-based index of the rule in `stockList.rules`.
- `inputOutputIndex : integer`: 1-based index of the input (stock) in the rule. Only set for inputs.
- `onlyOutputFraction : number`: For outputs of industries whose rules produce without inputs, the share of this rule in the total production (`0..1`).

### IndustryUtil.ProductionData (record)

```teal
record ProductionData
```

Production or consumption of one cargo type, as returned by `getIndustryProductionData`.

- `cargoTypeId : CargoTypeId`: Cargo type id.
- `actual : number`: Current consumption or production frequency per year. `nil` when the output is split between rules that produce without inputs.
- `total : number`: Maximum consumption or production per year, including boosts and the base config's industry productivity.

### IndustryUtil.SatellitesNumAndMax (record)

```teal
record SatellitesNumAndMax
```

Filled and total auto-fill slots (satellites) of an industry's construction.

- `numSatellites : integer`: Number of filled slots.
- `maxSatellites : integer`: Total number of slots.

### IndustryUtil.RuleInputOutputCargoParams (record)

```teal
record RuleInputOutputCargoParams
```

Parameters of `createInputOutputData`.

- `entity : Engine.Entity`: Entity with a `STOCK_LIST` component.
- `ruleNumber : integer`: 1-based index of the rule.
- `isInput : boolean`: `true` for an input slot of the rule, `false` for an output slot.
- `index : integer`: 1-based slot index, counted as in `getNumCargosOfRule`.
- `isIndustry : boolean`
- `onlyOneProductionLine : boolean`

### IndustryUtil.RuleInputOutputData (record)

```teal
record RuleInputOutputData
```

Content of one input or output slot of a rule, as returned by `createInputOutputData`.

- `plotsInput : boolean`: `true` if the slot is the satellite (plot) input; then `count` is the number of filled satellites.
- `cargoType : CargoTypeId`: Cargo type id of the slot.
- `count : integer`: Amount of the cargo type per rule run, or the number of filled satellites for the plot input.
- `boostFractionOutputText : string`: Set for the first output slot of a booster rule, the boost as text, e.g. `"+20%"`.
- `adjustedIndex : integer`: Slot index after skipping the plot input or boost slot.
- `fraction : number`: Progress (`0..1`) of the current rule run for inputs; `nil` if the rule is not running.
