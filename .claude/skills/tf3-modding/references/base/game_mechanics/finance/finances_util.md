# game_mechanics.finance.finances_util

Definition file: `base/tealdef/game_mechanics/finance/finances_util.d.tl` (in the game folder).

Labels and ordering of journal entries in the finances window.

## FinancesUtil (record)

```teal
global record FinancesUtil
```
Load: `local finances_util = ug_require "::/game_mechanics/finance/finances_util.tl" as FinancesUtil`

Labels and ordering of journal entries.

- `getCarrierLabel : function(JournalEntry.Carrier) : string`  (call as `finances_util.getCarrierLabel(…)`): Returns the label of a journal carrier (Road, Rail, Tram, Air, Water, General).
  In game: "Road", "Rail", "Tram" (German: „Strasse“, „Schiene“, „Tram“)
    - param `#1`: Carrier.
    - returns: Label, or an empty string.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:329`: `text = finances_util.getCarrierLabel(carrierState[1]),`
- `getOtherLabel : function(JournalEntry.Other) : string`  (call as `finances_util.getOtherLabel(…)`): Returns the label of the journal category `Other`.
    - param `#1`: Category.
    - returns: Label, or an empty string.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:430`: `text = finances_util.getOtherLabel(key),`
- `getKeyLabel : function({JournalEntry.Type, JournalEntry.Maintenance, JournalEntry.Construction}) : string`  (call as `finances_util.getKeyLabel(…)`): Returns the row label of a journal key (Revenue, running costs, upkeep, construction, Vehicles, Subsidies).
  In game: "Revenue", "Running Costs Vehicles", "Upkeep Stations" (German: „Erträge“, „Betriebskosten Fahrzeuge“, „Unterhalt Stationen“)
    - param `#1`: `{type, maintenance, construction}`.
    - returns: Label, or an empty string.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:307`: `text = finances_util.getKeyLabel(tableState:old():unfoldKey(entry.key)),`
- `scaleWithInflation : function(number) : integer`  (call as `finances_util.scaleWithInflation(…)`): Scales an amount by a factor that grows linearly from 1 in 1900 to 4 in 2000 (the current game year) and rounds it up to 10 000.
    - param `#1`: Amount in game currency.
    - returns: Scaled amount.
    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo/deliver_cargo.script.tl:120`: `local upfrontPayment = finances_util.scaleWithInflation(0.75 * math.random(20, 50) * 100000 * riskFactor * api.res.getBaseConfig().advancedOptions.subventionFin`
- `getKeyGroupOrder : function({JournalEntry.Type, JournalEntry.Maintenance, JournalEntry.Construction}) : integer`  (call as `finances_util.getKeyGroupOrder(…)`): Returns the sort group of a journal key (1 vehicle running costs, 2 vehicle maintenance, 3 infrastructure, 4 income, 5 other).
    - param `#1`: `{type, maintenance, construction}`.
    - returns: Sort group.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:295`: `local aOrder = finances_util.getKeyGroupOrder(tableState:old():unfoldKey(a.key))`
