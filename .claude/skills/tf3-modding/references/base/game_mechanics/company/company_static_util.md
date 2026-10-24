# game_mechanics.company.company_static_util

Definition file: `base/tealdef/game_mechanics/company/company_static_util.d.tl` (in the game folder).

Rank names and permit-key definitions (`company_permit_key` resources).

## CompanyStaticUtil (record)

```teal
global record CompanyStaticUtil
```
Load: `local company_static_util = ug_require "::/game_mechanics/company/company_static_util.tl" as CompanyStaticUtil`

Rank names and permit-key definitions.

- `getRole : function(companyLevel : integer) : string`  (call as `company_static_util.getRole(…)`): Returns the name of a rank (1 "Junior" to 15 "Tycoon").
  In game: "Junior", "Tycoon", "Rank {rank}" (German: „Junior“, „Tycoon“, „Karrierestufe {rank}“)
    - param `companyLevel`: Rank.
    - returns: Rank name, or "Rank n" outside 1..15.
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:56`: `role = company_static_util.getRole(value)`
- `getPermitKeyName : function(permitKey : ResName) : string`  (call as `company_static_util.getPermitKeyName(…)`): get name of the permitKey (if nil is returned, use construction as key)

get permit key definition as stored in the resource
- `getPermitKeyDesc : function(permitKey : ResName) : PermitKeyDesc`  (call as `company_static_util.getPermitKeyDesc(…)`)
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:62`: `local keyDesc = company_static_util.getPermitKeyDesc(actualPermitKey)`
- `getExtraPermitsAggregatedPerRank : function() : {string : {{integer, integer}}}`  (call as `company_static_util.getExtraPermitsAggregatedPerRank(…)`)
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_progression_util.tl:57`: `return getNumberOfExtraPermits(company_static_util.getExtraPermitsAggregatedPerRank(), companyData, permitKey)`
- `getMinRankConsideringPermits : function(companyMeta : CompanyMetadata.ConstructionDesc) : integer`  (call as `company_static_util.getMinRankConsideringPermits(…)`): Returns the rank at which a construction unlocks, from `companyRank`, the first `rankAndPermits` entry or its permit key.
    - param `companyMeta`: Company metadata of the construction.
    - returns: Rank, or `nil`.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_util.tl:173`: `local minRank = company_static_util.getMinRankConsideringPermits(company)`
- `getAllRanksConsideringPermits : function(companyMeta : CompanyMetadata.ConstructionDesc) : {integer}`  (call as `company_static_util.getAllRanksConsideringPermits(…)`): Returns all ranks at which a construction unlocks or gains permits.
    - param `companyMeta`: Company metadata of the construction.
    - returns: Ranks, or `nil`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:18`: `local affectedRanks : {integer} = company_static_util.getAllRanksConsideringPermits(companyMeta)`

### CompanyStaticUtil.PerkDescription (record)

```teal
record PerkDescription
```

A perk as listed under a rank in the company window.

- `name : string`: Name of the perk.
- `description : string`: Description of the perk.
- `icon : IconWithAvailability`: Icon of the perk.
- `permitKey : ResName`: Permit key of the perk.
- `permits : integer`: how often can it be built? (nil means unlimited)
- `isNewUnlock : boolean`: The perk unlocks at this rank; `false` means it only gains permits here.
- `cooldownInSeconds : number`: Cooldown of a used permit, in game seconds.
- `recycle : boolean`: re-gain permits after bulldozing
- `category : CompanyMetadata.ConstructionDesc.PerkCategory`: Kind of perk.
- `numbers : ConstructionDescReactUtil.Numbers`: Figures shown in the description.

### CompanyStaticUtil.RankDescription (record)

```teal
record RankDescription
```

A rank in the company window.

- `level : integer`: Rank number.
- `role : string`: Rank name (e.g. "Junior", "CEO").
- `requiredExp : integer`: Experience needed.
- `perks : {PerkDescription}`: Perks unlocked or extended at this rank.

### CompanyStaticUtil.PermitKeyDesc (record)

```teal
record PermitKeyDesc
```

Data of a `company_permit_key` resource.

- `name : string`: Display name.
- `rankAndPermits : {{integer, integer}}`: List of `{rank, permits}` gained at each rank.
- `cooldownInSeconds : number`: Game seconds after which a consumed permit is available again.
