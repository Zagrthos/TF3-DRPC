# scripts.util.town_growth_function

Definition file: `base/tealdef/scripts/util/town_growth_function.d.tl` (in the game folder).

Types of `/scripts/util/town_growth_function.tl` (`TownGrowthFunction`), the polynomials that scale town and company growth with level.

## TownGrowthFunction (record)

```teal
global record TownGrowthFunction
```
Load: `local townGrowthFunction = ug_require "::/scripts/util/town_growth_function.tl" as TownGrowthFunction`

Growth polynomials returned by `ug_require "/scripts/util/town_growth_function.tl"`.

- `worldGrowthFactor : function(integer) : number`  (call as `townGrowthFunction.worldGrowthFactor(…)`): Cubic growth factor by level with F(1) = 1 and F(15) = 12.5; the company progression uses it to compute the experience needed for a rank.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_progression_util.tl:13`: `return math.max(rank, math.floor(interpolatedBasePopulation * townGrowthFunction.worldGrowthFactor(rank)) - interpolatedBasePopulation + basePopulation)`
- `getGrowthFactor : function(level : number) : number`  (call as `townGrowthFunction.getGrowthFactor(…)`): Cubic growth factor of a town by level with F(1) = 1 and F(18) = 40; the town growth script uses it with the levels gained since the town's initial level.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_cargo_util.tl:109`: `return townGrowthFunction.getGrowthFactor(l)`
