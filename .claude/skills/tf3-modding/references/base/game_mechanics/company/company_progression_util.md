# game_mechanics.company.company_progression_util

Definition file: `base/tealdef/game_mechanics/company/company_progression_util.d.tl` (in the game folder).

Company experience and rank, read from the `::game_mechanics/company/company_progression.gs` game script.

## CompaniesGrowthState (record)

```teal
global record CompaniesGrowthState
```

State of the company progression game script.

- `companyState : {Engine.Entity : CompanyData}`: Player entity -> progression.

### CompaniesGrowthState.CompanyData (record)

```teal
record CompanyData
```

Progression of one company.

- `experience : integer`: Experience points; never lower than the highest world population reached.
- `level : integer`: Rank the player has applied (unlocks perks).
- `potentialLevel : integer`: Rank reached by experience; the player applies it with `applyLevel`.
- `ticketPriceMultiplier : number`: Ticket price multiplier of the reached rank, or `nil`.

## CompanyProgressionUtil (record)

```teal
global record CompanyProgressionUtil
```
Load: `local company_progression_util = ug_require "::/game_mechanics/company/company_progression_util.tl" as CompanyProgressionUtil`

Company experience and rank.

- `getXpNeededForRank : function(basePopulation : integer, rank : integer) : integer`  (call as `company_progression_util.getXpNeededForRank(…)`): Returns the experience needed for a rank.
    - param `basePopulation`: World population at game start.
    - param `rank`: Rank.
    - returns: Experience (0 for ranks below 1).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:88`: `rank.requiredExp = company_progression_util.getXpNeededForRank(basePopulation, i)`
- `getMaxRank : function() : integer`  (call as `company_progression_util.getMaxRank(…)`): Returns the highest rank.
    - returns: 15.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:285`: `local maxRank : integer = company_progression_util.getMaxRank()`
- `getTicketPriceMultiplierForLevel : function(level : integer) : number`  (call as `company_progression_util.getTicketPriceMultiplierForLevel(…)`): Returns the ticket price multiplier of a rank, linear from 1 at rank 1 to `ticketPriceMultiplierAtMaxLevel` at the highest rank.
    - param `level`: Rank.
    - returns: Multiplier, or `nil` at rank 1 or when the config multiplier is 1.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_growth.script.tl:48`: `scriptState.companyState[companyEntity].ticketPriceMultiplier = company_progression_util.getTicketPriceMultiplierForLevel(newLevel)`
- `getExtraPermits : function(companyData : CompaniesGrowthState.CompanyData) : {ResName : integer}`  (call as `company_progression_util.getExtraPermits(…)`): Returns the permits of every permit key that the company has gained by rank.
    - param `companyData`: Progression of the company.
    - returns: Permit key -> number of permits.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_util.tl:93`: `result.extraPermits = company_progression_util.getExtraPermits(progressionState)`
- `getNumberOfExtraPermits : function(companyData : CompaniesGrowthState.CompanyData, permitKey : ResName) : integer`  (call as `company_progression_util.getNumberOfExtraPermits(…)`): Returns the permits of one permit key that the company has gained by rank.
    - param `companyData`: Progression of the company.
    - param `permitKey`: Permit key.
    - returns: Number of permits.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:53`: `permits = (permits or 0) + company_progression_util.getNumberOfExtraPermits({level = rank.level}, actualPermitKey)`
- `getCompanyProgressionState : function(entity : Engine.Entity) : CompaniesGrowthState.CompanyData`  (call as `company_progression_util.getCompanyProgressionState(…)`): Reads the progression of a company; the rank is replaced by `enforcedLevel` from the growth config if set.
    - param `entity`: Player entity.
    - returns: Progression, or `nil`.
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:866`: `return company_progression_util.getCompanyProgressionState(api.engine.util.getPlayer())`
- `getLevelAndFraction : function(basePopulation : integer, exp : integer) : integer, number, integer, integer`  (call as `company_progression_util.getLevelAndFraction(…)`): return: current level, fraction/percentage reached of current level, experience needed for current level, experience needed for next level
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_growth.script.tl:39`: `local newLevel = company_progression_util.getLevelAndFraction(basePopulation, exp)`

## CompanyGrowthConfig (record)

```teal
global record CompanyGrowthConfig
```

Config of company growth (`game_script_config` resource `::/game_mechanics/company/company_growth_config.res`).

- `enforcedLevel : integer`: default/vanilla is nil
- `ticketPriceMultiplierAtMaxLevel : number`: Ticket price multiplier at the highest rank (0.5 in the base game).
