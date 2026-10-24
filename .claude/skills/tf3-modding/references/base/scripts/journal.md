# scripts.journal

Definition file: `base/tealdef/scripts/journal.d.tl` (in the game folder).

Type of the engine's finance journal entry (`api.type.JournalEntry`): one booking of money with its
time, amount and category. Scripts create one with `api.type.JournalEntry.new()` and book it with
`api.cmd.makeJournalBookAssetCmd`.

## JournalEntry (record)

```teal
global record JournalEntry
```
Access: `api.type.JournalEntry`

One finance journal booking (`api.type.JournalEntry`). Create it with `api.type.JournalEntry.new()`,
set `amount`, `time` and `category`, and book it with `api.cmd.makeJournalBookAssetCmd`.

- `new : function() : JournalEntry`  (call as `api.type.JournalEntry.new(…)`): Creates a new, empty `JournalEntry`.
    - used 16x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/loan_util.tl:203`: `local entry = api.type.JournalEntry.new()`
- `time : number`: Booking time; the base game sets `-1` when booking a subsidy.
- `amount : number`: Booked amount in game currency; positive for income, negative for costs.
- `category : Category`: Classification of the entry.

### JournalEntry.Type (record)

```teal
record Type
```
Access: `api.type.JournalEntry.Type`

Principal categories of a journal entry (`api.type.JournalEntry.Type`).

- `LOAN : Type`: Loan taken or repaid.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/loan_util.tl:206`: `entry.category.type = api.type.JournalEntry.Type.LOAN`
- `INTEREST : Type`: Loan interest.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/loan_util.tl:214`: `entry.category.type = api.type.JournalEntry.Type.INTEREST`
- `CONSTRUCTION : Type`: Construction costs (subcategory in `category.construction`).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:78`: `elseif key[1] == api.type.JournalEntry.Type.CONSTRUCTION then`
- `ACQUISITION : Type`: Vehicle purchases (shown as "Vehicles" in the finance window).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:76`: `elseif key[1] == api.type.JournalEntry.Type.ACQUISITION then`
- `MAINTENANCE : Type`: Maintenance costs (subcategory in `category.maintenance`).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:67`: `elseif key[1] == api.type.JournalEntry.Type.MAINTENANCE then`
- `INCOME : Type`: Revenue.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:65`: `if key[1] == api.type.JournalEntry.Type.INCOME then`
- `OTHER : Type`: Other bookings (subcategory in `category.other`).
    - used 12x in the base game, e.g. `base/content/gui/gui/construction/tools/marketing_campaign_tool.script.tl:101`: `journalEntry.category.type = api.type.JournalEntry.Type.OTHER`
- `SUBSIDY : Type`: Subsidy (subvention) payments.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:80`: `elseif key[1] == api.type.JournalEntry.Type.SUBSIDY then`

### JournalEntry.Construction (record)

```teal
record Construction
```
Access: `api.type.JournalEntry.Construction`

Construction subcategories (`api.type.JournalEntry.Construction`).

- `STREET : Construction`: Streets.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:36`: `elseif construction == api.type.JournalEntry.Construction.STREET then`
- `TRACK : Construction`: Tracks.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:38`: `elseif construction == api.type.JournalEntry.Construction.TRACK then`
- `SIGNAL : Construction`: Signals.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:49`: `construction == api.type.JournalEntry.Construction.SIGNAL or`
- `STATION : Construction`: Stations.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:32`: `if construction == api.type.JournalEntry.Construction.STATION then`
- `DEPOT : Construction`: Depots.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:34`: `elseif construction == api.type.JournalEntry.Construction.DEPOT then`
- `BULLDOZER : Construction`: Bulldozing.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:56`: `elseif construction == api.type.JournalEntry.Construction.BULLDOZER then`
- `WAREHOUSE : Construction`: Warehouses.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:40`: `elseif construction == api.type.JournalEntry.Construction.WAREHOUSE then`
- `OTHER : Construction`: Other constructions.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:50`: `construction == api.type.JournalEntry.Construction.OTHER then`

### JournalEntry.Maintenance (record)

```teal
record Maintenance
```
Access: `api.type.JournalEntry.Maintenance`

Journal entry Maintenance Category

- `VEHICLE : Maintenance`: Vehicle running costs ("Running Costs Vehicles").
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:68`: `if key[2] == api.type.JournalEntry.Maintenance.VEHICLE then`
- `INFRASTRUCTURE : Maintenance`: Infrastructure maintenance.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:70`: `elseif key[2] == api.type.JournalEntry.Maintenance.INFRASTRUCTURE then`
- `OTHER : Maintenance`: Other maintenance.
- `VEHICLE_MAINTENANCE : Maintenance`: Vehicle maintenance ("Maintenance Vehicles").
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:72`: `elseif key[2] == api.type.JournalEntry.Maintenance.VEHICLE_MAINTENANCE then`

### JournalEntry.Other (record)

```teal
record Other
```
Access: `api.type.JournalEntry.Other`

Journal entry Other Category

- `OTHER : Other`: Other bookings.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_util.tl:24`: `if other == api.type.JournalEntry.Other.OTHER then`

### JournalEntry.Carrier (record)

```teal
record Carrier
```
Access: `api.type.JournalEntry.Carrier`

Carrier subcategories (`api.type.JournalEntry.Carrier`); the finance window shows one column per carrier.

- `ROAD : Carrier`: Road vehicles.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:583`: `local carriers = { api.type.JournalEntry.Carrier.ROAD,`
- `RAIL : Carrier`: Trains.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:584`: `api.type.JournalEntry.Carrier.RAIL,`
- `TRAM : Carrier`: Trams.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:585`: `api.type.JournalEntry.Carrier.TRAM,`
- `OTHER : Carrier`: Not carrier-specific (shown as "General").
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:588`: `api.type.JournalEntry.Carrier.OTHER }`
- `AIR : Carrier`: Aircraft.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:587`: `api.type.JournalEntry.Carrier.AIR,`
- `WATER : Carrier`: Ships.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:586`: `api.type.JournalEntry.Carrier.WATER,`

### JournalEntry.Category (record)

```teal
record Category
```
Access: `api.type.JournalEntry.Category`

Details about the classification of the journal entry

- `new : function() : Category`  (call as `api.type.JournalEntry.Category.new(…)`): Creates a new, empty `Category`.
- `type : Type`: The principal category
- `construction : Construction`: The construction subcategory, only if type == Construction
- `maintenance : Maintenance`: The maintenance subcategory, only if type == Maintenance
- `other : Other`: The other subcategory, only if type == Other
- `carrier : Carrier`: The carrier subcategory, only if type == Carrier
