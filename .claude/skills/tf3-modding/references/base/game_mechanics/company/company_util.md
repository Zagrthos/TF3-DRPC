# game_mechanics.company.company_util

Definition file: `base/tealdef/game_mechanics/company/company_util.d.tl` (in the game folder).

Permit counting, disable reasons and state access of the company.

## CompanyUtil (record)

```teal
global record CompanyUtil
```
Load: `local company_util = ug_require "::/game_mechanics/company/company_util.tl" as CompanyUtil`

Permit counting, disable reasons and state access of the company.

- `prospectionTimeMs : number`: Maximum length of a prospection, six game months in ms.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_notification_prospection.script.tl:19`: `local percent = (gameTime.gameTime - notificationParams.initiatedTimestamp) / company_util.prospectionTimeMs`
- `prospectionIntervalTimeMs : number`: Time between spawn attempts of a prospection, one game month in ms.
- `prospectionStartChance : number`: Chance of the first spawn attempt (0.2).
- `prospectionEndChance : number`: Chance of the last spawn attempt (0.6); the chance rises linearly in between.
- `getActualPermitKey : function(constructionRes : ResName, metadata : ConstructionDescMetadata) : ResName`  (call as `company_util.getActualPermitKey(…)`): Returns the permit key of a construction. It is the `permitKey`, or the construction file if the metadata has `rankAndPermits` or `companyRank`.
    - param `constructionRes`: Construction file.
    - param `metadata`: Metadata of the construction resource.
    - returns: Permit key, or `nil` if the construction needs no permit.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:14`: `local actualPermitKey = company_util.getActualPermitKey(constructionFile, perk.metadata)`
- `getMaximumNumberOfPermits : function(companyProgress : CompaniesGrowthState.CompanyData, actualPermitKey : ResName, metadata : ConstructionDescMetadata) : integer, boolean`  (call as `company_util.getMaximumNumberOfPermits(…)`): Returns how many permits of a key the company has, from the construction's `rankAndPermits` up to its rank plus the extra permits of the key.
    - param `companyProgress`: Progression of the company.
    - param `actualPermitKey`: Permit key.
    - param `metadata`: Metadata of the construction resource.
    - returns: Number of permits, and `true` if the metadata mixes `rankAndPermits` with `permitKey`.
- `getNumberOfPermitsForRank : function(companyProgress : CompaniesGrowthState.CompanyData, metadata : ConstructionDescMetadata) : integer, boolean`  (call as `company_util.getNumberOfPermitsForRank(…)`): Returns the permits a construction gains at exactly the company's rank.
    - param `companyProgress`: Progression (only `level` is read).
    - param `metadata`: Metadata of the construction resource.
    - returns: Number of permits, and the mixed-metadata warning flag.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:51`: `permits = company_util.getNumberOfPermitsForRank({level = rank.level}, perk.metadata)`
- `getConstructionDisableCacheData : function(allDefs : {{ResName, ConstructionDescMetadata, string}}) : ConstructionDisableCacheData`  (call as `company_util.getConstructionDisableCacheData(…)`): Computes rank, permits, consumed permits, cooldowns and build counts of the player for a list of constructions.
    - param `allDefs`: List of `{construction file, metadata, name}`.
    - returns: Cache data.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:1171`: `cache.company = company_util.getConstructionDisableCacheData(allDefs)`
- `getConstructionDisableReason : function(constructionRes : ResName, metadata : ConstructionDescMetadata, cache : ConstructionDisableCacheData) : ItemDisableReason`  (call as `company_util.getConstructionDisableReason(…)`): Tells why a construction cannot be built (rank, cooldown, no permits left).
  In game: "Unlocked at Rank: {rank}", "Cooldown: {remainingTime}", "Already Built" (German: „Freigeschaltet ab Karrierestufe: {rank}“, „Abklingzeit: {remainingTime}“, „Bereits gebaut“)
    - param `constructionRes`: Construction file.
    - param `metadata`: Metadata of the construction resource.
    - param `cache`: Result of `getConstructionDisableCacheData`.
    - returns: Reason with remaining permits, or `nil` if the construction has no company limits.
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:1176`: `local companyReason = company_util.getConstructionDisableReason(def.resName, def.metadata, cache.company)`
- `findStateIndex : function(CompaniesState, EntityUtil.EntityAndRevision) : integer`  (call as `company_util.findStateIndex(…)`): Finds the index of a company in the state.
    - param `#1`: Companies state.
    - param `#2`: Company entity and revision.
    - returns: Index, or `-1`.
- `getCompanyState : function(CompaniesState, Engine.Entity) : CompanyState`  (call as `company_util.getCompanyState(…)`): Returns the state of a company, or a new empty one for a player without state.
    - param `#1`: Companies state.
    - param `#2`: Player entity.
    - returns: Company state, or `nil` if the entity is not a player.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:326`: `local companyState : CompanyState = company_util.getCompanyState(company_util.externalGetCompaniesState(), companyEntity)`
- `setCompanyState : function(CompaniesState, Engine.Entity, CompanyState) : CompanyState`  (call as `company_util.setCompanyState(…)`): Stores the state of a company, adding it if missing.
    - param `#1`: Companies state.
    - param `#2`: Player entity.
    - param `#3`: Company state.
- `externalGetCompanyState : function(Engine.Entity) : CompanyState`  (call as `company_util.externalGetCompanyState(…)`): Reads the state of a company from the company game script.
    - param `#1`: Player entity.
    - returns: Company state.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:211`: `local companyState = company_util.externalGetCompanyState(companyEntity)`
- `externalGetCompaniesState : function() : CompaniesState`  (call as `company_util.externalGetCompaniesState(…)`): Reads the state of the company game script.
    - returns: Companies state.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:326`: `local companyState : CompanyState = company_util.getCompanyState(company_util.externalGetCompaniesState(), companyEntity)`
- `getNeededPermitsForProposal : function(Proposal, ProposalData) : {ResName : integer}, {ResName : ConstructionDescMetadata}`  (call as `company_util.getNeededPermitsForProposal(…)`): Counts the permits that the new constructions of a proposal need (replaced constructions excluded).
    - param `#1`: Proposal.
    - param `#2`: Proposal data (unused).
    - returns: Permit key -> count, and permit key -> metadata.
- `countUsedConstructionPermits : function(permitKeys : {ResName : any}) : {ResName : integer}`  (call as `company_util.countUsedConstructionPermits(…)`): Counts the built constructions per permit key.
    - param `permitKeys`: Unused.
    - returns: Permit key -> number built.
- `getCompanyMetadataFromConstructionResource : function(constructionEntity : Engine.Entity) : CompanyMetadata.ConstructionDesc`  (call as `company_util.getCompanyMetadataFromConstructionResource(…)`): Returns the company metadata of a built construction's resource.
    - param `constructionEntity`: Construction entity.
    - returns: Company metadata, or `nil`.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/pollution_cleanup_facility_hud.script.tl:39`: `local companyMetaFromRes = company_util.getCompanyMetadataFromConstructionResource(entity)`
- `getCurrentCompanyState : function() : CurrentCompanyState`  (call as `company_util.getCurrentCompanyState(…)`): Returns rank and experience of the player (zeros if unknown).
    - returns: Current company state.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:677`: `return company_util.getCurrentCompanyState()`
- `getBasePopulation : function() : integer`  (call as `company_util.getBasePopulation(…)`): note: This will never change during play
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_growth.script.tl:81`: `local basePopulation = company_util.getBasePopulation()`
- `computeWorldPopulation : function() : integer`  (call as `company_util.computeWorldPopulation(…)`): Sums the residential capacity of all towns.
    - returns: World population.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_growth.script.tl:86`: `local numResidents = company_util.computeWorldPopulation()`
- `isConstructionLandmark : function(constructionEntity : Engine.Entity) : boolean`  (call as `company_util.isConstructionLandmark(…)`): Tells whether a construction's company perk category is `LANDMARK`.
    - param `constructionEntity`: Construction entity.
    - returns: `true` for landmarks.
    - used 2x in the base game, e.g. `mods/release/urbangames_campaign_mission_06/content/subventions/subventions/deliver_visitors/deliver_visitors.script.tl:120`: `if company_util.isConstructionLandmark(entity) then`

### CompanyUtil.ItemDisableReason (record)

```teal
record ItemDisableReason
```

Why a construction cannot be built (or how many permits remain).

- `reason : string`: Text such as "Unlocked at Rank", "Cooldown", "Already Built" or "Already Used"; `nil` if it can be built.
- `elapsedCooldownFraction : number`: Elapsed part of the cooldown (0 to 1).
- `category : string`: `company-rank` when the rank is too low or too high.
- `numRemaining : integer`: Permits left.

### CompanyUtil.ConstructionDisableCacheData (record)

```teal
record ConstructionDisableCacheData
```

Precomputed permit data for `getConstructionDisableReason`.

- `companyRank : integer`: Applied company rank.
- `extraPermits : {ResName : integer}`: Permit key -> extra permits by rank.
- `maximumPermits : {ResName : integer}`: contains extra permits
- `consumedPermits : {ResName : integer}`: Permit key -> permits consumed and still in cooldown.
- `remainingPermitCooldown : {ResName : CooldownState}`: Permit key -> cooldown of the oldest consumed permit.
- `data : {ResName : ItemDisableCacheData}`: Construction file or permit key -> build count.

#### CompanyUtil.ConstructionDisableCacheData.CooldownState (record)

```teal
record CooldownState
```

Cooldown of a permit key.

- `remainingMs : number`: Remaining cooldown in game time (ms).
- `elapsedFraction : number`: Elapsed part of the cooldown (0 to 1).

#### CompanyUtil.ConstructionDisableCacheData.ItemDisableCacheData (record)

```teal
record ItemDisableCacheData
```

Build count of a permit key.

- `numBuilt : integer`: Constructions built with this file or permit key.

### CompanyUtil.CurrentCompanyState (record)

```teal
record CurrentCompanyState
```

Rank and experience of the player.

- `level : integer`: Applied rank.
- `potentialLevel : integer`: Rank reached by experience.
- `experience : integer`: Experience points.
