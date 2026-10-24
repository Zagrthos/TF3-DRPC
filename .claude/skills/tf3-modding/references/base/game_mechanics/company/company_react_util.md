# game_mechanics.company.company_react_util

Definition file: `base/tealdef/game_mechanics/company/company_react_util.d.tl` (in the game folder).

GUI helpers of the company window.

## CompanyReactUtil (record)

```teal
global record CompanyReactUtil
```
Load: `local company_react_util = ug_require "::/game_mechanics/company/company_react_util.tl" as CompanyReactUtil`

GUI helpers of the company window.

- `gatherSortedStaticCompanyData : function({ConstructionDefinition}) : {CompanyStaticUtil.RankDescription}`  (call as `company_react_util.gatherSortedStaticCompanyData(…)`): Sorts construction definitions with company metadata into the ranks at which they unlock or gain permits.
    - param `#1`: Construction definitions.
    - returns: Rank descriptions with their perks.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:861`: `return company_react_util.gatherSortedStaticCompanyData(param.toolParam.allCompanyDefinitionsCacheRef:get())`
- `getNumberOfAvailablePermits : function(allRelevant : {ConstructionDefinition}, permitKey : ResName) : integer`  (call as `company_react_util.getNumberOfAvailablePermits(…)`): Returns how many permits of a key the player can still use.
    - param `allRelevant`: Construction definitions with company metadata.
    - param `permitKey`: Permit key.
    - returns: Number of permits.
