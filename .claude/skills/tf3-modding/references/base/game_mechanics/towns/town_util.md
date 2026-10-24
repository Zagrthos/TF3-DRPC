# game_mechanics.towns.town_util

Definition file: `base/tealdef/game_mechanics/towns/town_util.d.tl` (in the game folder).

Town ratings, reputation bookkeeping and helper functions (`game_mechanics/towns/town_util.tl`).

## TownRatingKey (enum)

```teal
global enum TownRatingKey
```

Keys of the six town ratings (`TownUtil.Rating.key`).

- `"urban_care"`: Reputation rating.
  In game: "Reputation" (German: „Ruf“)
- `"traffic_congestion"`: Traffic rating.
  In game: "Traffic" (German: „Verkehr“)
- `"people_happiness"`: Happiness rating.
  In game: "Happiness" (German: „Zufriedenheit“)
- `"cargo_delivery"`: Delivery time rating.
  In game: "Delivery" (German: „Lieferung“)
- `"noise"`: Noise rating.
  In game: "Noise" (German: „Lärm“)
- `"pollution"`: Pollution rating.
  In game: "Pollution" (German: „Verschmutzung“)

## TownUtil (record)

```teal
global record TownUtil
```
Load: `local town_util = ug_require "::/game_mechanics/towns/town_util.tl" as TownUtil`

Town ratings, reputation and helper functions (`game_mechanics/towns/town_util.tl`).

Example:
```lua
local town_util = ug_require "/game_mechanics/towns/town_util.tl" as TownUtil
local townState = town_util.externalGetTownState(townEntity)
local level = town_util.getFactorRatingLevel(townState.authorityScore)
```

- `GetRatings : function() : {Rating}`  (call as `town_util.GetRatings(…)`): The six town ratings in the order urban care, traffic, happiness, delivery, noise, pollution.
    - returns: List of ratings.
    - used 10x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:224`: `local ratings = town_util.GetRatings()`
- `GetRating : function(key : string) : Rating`  (call as `town_util.GetRating(…)`): Rating description for a rating key.
  In game: "Reputation", "Traffic", "Happiness" (German: „Ruf“, „Verkehr“, „Zufriedenheit“)
    - param `key`: A `TownRatingKey`.
    - returns: The rating, or `nil` for an unknown key.
    - used 3x in the base game, e.g. `base/content/gui/gui/statistics/statistic_towns.tl:277`: `local staticRating = town_util.GetRating(key)`
- `GetRatingsToLog : function() : {Rating}`  (call as `town_util.GetRatingsToLog(…)`): Ratings written to the town journal; `GetRatings` plus the supply ratings `supplies_cargo_0` to `supplies_cargo_2` and `supplies_passenger`.
    - returns: List of ratings.
- `SeveredConnectionPenalty : number`: Reputation penalty per severed main connection (`1/3`).
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:89`: `local targetPenalty = numSeveredConnections * town_util.SeveredConnectionPenalty`
- `NumLinesForHappinessStats : integer`: Number of lines listed in the happiness statistics (`5`).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util_parallel.script.tl:44`: `local stats = api.engine.util.town.getTownHappinessStats(townEntity, town_util.NumLinesForHappinessStats)`
- `GrowthLevelCacheKey : string`: Key (`"growth_level"`) in `TownState.cachedRatings` that holds the town's growth level. `calcAuthorityScore` stores it, `getGrowthLevelMult` reads it instead of recalculating, and starting a new game from a map (`initNewGameFromMap`) clears it.
- `LineUsageRatingMultiplier : number`: Factor on the town's line usage when computing the passenger supply (`2.0`).
- `getMultiplierToRankClass : function(multiplier : number) : string`  (call as `town_util.getMultiplierToRankClass(…)`): CSS class for a growth multiplier, from `town-growth-rank-1` (below 0.15) to `town-growth-rank-6` (0.8 or more).
    - param `multiplier`: Multiplier between 0 and 1.
    - returns: CSS class name.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:842`: `class = "font-scale-body,last-year-multiplier-icon," .. town_util.getMultiplierToRankClass(multiplier),`
- `getRatingSensitivityOptionName : function(ratingName : TownRatingKey) : string`  (call as `town_util.getRatingSensitivityOptionName(…)`): Name of the script option that holds the sensitivity of a rating, for example `townConfig.sensitivityNoise`.
    - param `ratingName`: Rating key.
    - returns: Option name, or an empty string for an unknown key.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town_eow.script.tl:906`: `local optionName = town_util.getRatingSensitivityOptionName(params.rating)`
- `getRatingLabel : function(RatingLevel) : string`  (call as `town_util.getRatingLabel(…)`): Localized label of a rating level, such as "Very Poor".
  In game: "Very Poor", "Mediocre", "Excellent" (German: „Sehr schlecht“, „Mittelmässig“, „Ausgezeichnet“)
    - param `#1`: Rating level.
    - returns: Localized label.
    - used 11x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:75`: `text = town_util.getRatingLabel(ratingLevel),`
- `getRatingNameFromKey : function(name : TownRatingKey) : string`  (call as `town_util.getRatingNameFromKey(…)`): Localized display name of a rating.
    - param `name`: Rating key.
    - returns: Display name, or `nil` for an unknown key.
    - used 7x in the base game, e.g. `base/content/gui/gui/statistics/statistic_towns.tl:507`: `{ path = getRatingIconFromKey("urban_care"), tooltip = town_util.getRatingNameFromKey("urban_care"), recipe = TownUrbanCareCell, getCompareValue = getUrbanCareC`
- `getRatingUrbanCare : function(entity : Engine.Entity, townState : TownState) : number`  (call as `town_util.getRatingUrbanCare(…)`): Reputation rating of a town. Each severed main connection counts as a penalty of `SeveredConnectionPenalty`.
    - param `entity`: Town entity.
    - param `townState`: State of the town.
    - returns: Reputation between 0 and 1; `1` when the sensitivity is 0.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:81`: `local ratingUrbanCare = town_util.getRatingUrbanCare(townEntity, townState)`
- `getRatingNoise : function(entity : Engine.Entity, townState : TownState) : number`  (call as `town_util.getRatingNoise(…)`): Noise rating of a town. The town's noise in dB, plus a per-town random offset and `10 * log10(sensitivity)`, is mapped linearly between the noise bounds of `emission_config`.
    - param `entity`: Town entity.
    - param `townState`: State of the town.
    - returns: Rating between 0 and 1 (1 is quiet); `1` when the sensitivity is 0.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1790`: `local noiseRating = town_util.getRatingNoise(params.entity, townState)`
- `getRatingPollution : function(entity : Engine.Entity, townState : TownState) : number`  (call as `town_util.getRatingPollution(…)`): Pollution rating of a town. The average pollution per 16 m by 16 m cell of the settlement area is converted to dB, adjusted for sensitivity and mapped between the pollution bounds of `emission_config`.
    - param `entity`: Town entity.
    - param `townState`: State of the town.
    - returns: Rating between 0 and 1 (1 is clean); `1` when the sensitivity is 0.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util_parallel.script.tl:80`: `local emissionRating = params.pollution and town_util.getRatingPollution(params.townEntity, townState) or town_util.getRatingNoise(params.townEntity, townState)`
- `getRatingTraffic : function(entity : Engine.Entity, townState : TownState) : number`  (call as `town_util.getRatingTraffic(…)`): Traffic rating of a town from the engine's traffic speed rating, raised to the power of the sensitivity, plus `constructionBoni.trafficRatingIncrease`.
    - param `entity`: Town entity.
    - param `townState`: State of the town.
    - returns: Rating between 0 and 1; `1` when the sensitivity is 0.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util_parallel.script.tl:35`: `local ratingTraffic = town_util.getRatingTraffic(townEntity, townState)`
- `getRatingTrafficAllTowns : function(townStates : {TownState}) : {Engine.Entity : number}`  (call as `town_util.getRatingTrafficAllTowns(…)`): Traffic ratings of several towns from one engine call. Unlike `getRatingTraffic` it ignores the per-town sensitivity.
    - param `townStates`: States of the towns.
    - returns: Rating between 0 and 1 per town entity.
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistic_towns.tl:479`: `local allTrafficRatings = town_util.getRatingTrafficAllTowns(townsState.townStates or {})`
- `getRatingHappiness : function(entity : Engine.Entity, townState : TownState) : number`  (call as `town_util.getRatingHappiness(…)`): Happiness rating of a town from `api.engine.util.town.getTownHappinessStats`; see `getRatingHappinessFromStats`.
    - param `entity`: Town entity.
    - param `townState`: State of the town.
    - returns: Rating between 0 and 1.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util_parallel.script.tl:243`: `return town_util.getFactorRatingLevel(town_util.getRatingHappiness(param.townEntity, townState))`
- `getRatingCargoDelivery : function(entity : Engine.Entity, townState : TownState) : number`  (call as `town_util.getRatingCargoDelivery(…)`): Delivery time rating of a town. The share of cargo delivered on time in the last half game year (at least 10 items assumed) is mapped between bounds that depend on the sensitivity.
    - param `entity`: Town entity.
    - param `townState`: State of the town.
    - returns: Rating between 0 and 1; `1` when the sensitivity is 0.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util_parallel.script.tl:139`: `local cargoDeliveryRating = town_util.getRatingCargoDelivery(townEntity, townState)`
- `getRatingDeliveriesLegacy : function(entity : Engine.Entity) : number`  (call as `town_util.getRatingDeliveriesLegacy(…)`): Old supplies rating, the average of the cargo and passenger supply ratings. No base-game script calls it.
    - param `entity`: Town entity.
    - returns: Rating between 0 and 1; `0` when the town has no demand.
- `getRatingDeliveries : function(entity : Engine.Entity) : TownUtil.RatingDeliveries`  (call as `town_util.getRatingDeliveries(…)`): Supplies rating level of a town. The level numbers of the cargo and passenger supply ratings are averaged and rounded up.
    - param `entity`: Town entity.
    - returns: Level number and a flag for "no deliveries". A town without any demand gets level 5.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:872`: `local ratingDeliveries = town_util.getRatingDeliveries(params.entity)`
- `getGrowthLevelMult : function(entity : Engine.Entity, townState : TownState) : RatingLevel`  (call as `town_util.getGrowthLevelMult(…)`): Growth level of a town from the product of its rating level (`authorityScore`) and its supplies level. Higher levels need a larger product. Uses the value cached under `GrowthLevelCacheKey` when there is one.
    - param `entity`: Town entity.
    - param `townState`: State of the town.
    - returns: Growth level; `TownCargoUtil.authority2xpFactor` maps it to the experience factor.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util_parallel.script.tl:282`: `local growthLevel = town_util.getGrowthLevelMult(townEntity, townState)`
- `getRatingBonuses : function(entity : Engine.Entity, townState : TownState) : number`  (call as `town_util.getRatingBonuses(…)`): Bonus rating shown in the town window.
    - param `entity`: Town entity (not used).
    - param `townState`: State of the town.
    - returns: `1.0` when the town has an experience bonus from constructions, otherwise `0.5`.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1186`: `local ratingBonuses = town_util.getRatingBonuses(params.entity, townState)`
- `getRatingDeliveriesCargo : function(entity : Engine.Entity) : number`  (call as `town_util.getRatingDeliveriesCargo(…)`): Cargo supply rating of a town, the best of the three districts.
    - param `entity`: Town entity.
    - returns: Supply divided by demand (0 to 1), or `nil` when no district demands cargo.
- `getRatingDeliveriesCargoLandUse : function(entity : Engine.Entity, landUse : LandUseType) : number`  (call as `town_util.getRatingDeliveriesCargoLandUse(…)`): Cargo supply rating of one district, total supply divided by total demand.
    - param `entity`: Town entity.
    - param `landUse`: District type.
    - returns: Rating between 0 and 1, or `nil` when the district demands no cargo.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util_parallel.script.tl:17`: `local cargoRatingResidential = town_util.getRatingDeliveriesCargoLandUse(townEntity, api.type["enum"].LandUseType.RESIDENTIAL)`
- `getRatingDeliveriesPassenger : function(entity : Engine.Entity) : number`  (call as `town_util.getRatingDeliveriesPassenger(…)`): Passenger supply rating of a town, twice the town's line usage.
    - param `entity`: Town entity.
    - returns: Rating between 0 and 1, or `nil` when the town has no residential capacity.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util_parallel.script.tl:14`: `local passengerRating = town_util.getRatingDeliveriesPassenger(townEntity)`
- `getCargoSupplyDemandsLandUse : function(Engine.Entity, LandUseType) : {CargoTypeSupplyDemand}`  (call as `town_util.getCargoSupplyDemandsLandUse(…)`): Supply and demand per cargo type of one district, from `api.engine.system.townBuildingSystem.getCargoSupplyAndLimit`.
    - param `#1`: Town entity.
    - param `#2`: District type.
    - returns: List sorted by demand (highest first), then by cargo type id.
    - used 1x in the base game, e.g. `base/content/gui/gui/layers/layer_cargo_flow.tl:75`: `districtSupplyDemand = town_util.getCargoSupplyDemandsLandUse(param.entity, townSelectionDetails.districtType)`
- `getCargoSupplyDemandsLandUseWithResidentialLineUsage : function(townEntity : Engine.Entity, landUse : LandUseType) : {CargoTypeSupplyDemand}`  (call as `town_util.getCargoSupplyDemandsLandUseWithResidentialLineUsage(…)`): Like `getCargoSupplyDemandsLandUse`. For the residential district it puts a passenger entry first, with the residential person capacity as demand and the capacity times `lineUsage * LineUsageRatingMultiplier` (at most 1) as supply.
    - param `townEntity`: Town entity.
    - param `landUse`: District type.
    - returns: List of supply and demand entries.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util_parallel.script.tl:8`: `local result = town_util.getCargoSupplyDemandsLandUseWithResidentialLineUsage(param.townEntity, param.landUse)`
- `getCargoSupplyDemands : function(Engine.Entity) : CargoTypeSupplyDemands`  (call as `town_util.getCargoSupplyDemands(…)`): Cargo supply and demand of all three districts of a town.
    - param `#1`: Town entity.
    - returns: Supply and demand per district.
    - used 12x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_cargo_town/deliver_cargo_town.script.tl:138`: `local cargoSupplyDemands : CargoTypeSupplyDemands = town_util.getCargoSupplyDemands(townEntity)`
- `getLandUseCapacities : function(Engine.Entity) : TownCapacities`  (call as `town_util.getLandUseCapacities(…)`): Person capacity of each district of a town.
    - param `#1`: Town entity.
    - returns: Capacity per district.
- `getTownCapacityUsages : function(Engine.Entity) : TownCapacityUsages`  (call as `town_util.getTownCapacityUsages(…)`): Used and total capacity of each district of a town, from `api.engine.util.town.getTownCapacityUsage`.
    - param `#1`: Town entity.
    - returns: Capacity usage per district.
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:869`: `local townCapacityUsages : TownCapacityUsages = town_util.getTownCapacityUsages(params.entity)`
- `getReachability : function(Engine.Entity) : UtilTown.TownReachability`  (call as `town_util.getReachability(…)`): Reachability data of a town; calls `api.engine.util.town.getTownReachability`.
    - param `#1`: Town entity.
    - returns: Reachability of the town.
    - used 4x in the base game, e.g. `base/content/gui/gui/statistics/statistic_towns.tl:84`: `local reachability : UtilTown.TownReachability = town_util.getReachability(entity)`
- `getTownEmissionDB : function(Engine.Entity) : UtilTown.TownEmission`  (call as `town_util.getTownEmissionDB(…)`): Noise and pollution of a town in dB, including the per-town random offset of `getTownEmissionRandomOffsetDB`.
    - param `#1`: Town entity.
    - returns: Emission of the town.
- `getTownEmissionRandomOffsetDB : function(Engine.Entity) : UtilTown.TownEmission`  (call as `town_util.getTownEmissionRandomOffsetDB(…)`): Fixed random offset in dB that each town adds to its noise and pollution. The random generator is seeded with the town entity id, so the offset stays the same.
    - param `#1`: Town entity.
    - returns: Noise and pollution offsets, within `emission_config.noiseRatingRandomOffsetMinMax` and `pollutionRatingRandomOffsetMinMax`.
    - notes: Calls `math.randomseed` with the town entity id.
- `roundRatingPrecision : function(number) : integer`  (call as `town_util.roundRatingPrecision(…)`): Rounds a rating value down to an integer.
    - param `#1`: Value.
    - returns: `math.floor` of the value.
- `getReachabilityRating : function(integer, number) : integer`  (call as `town_util.getReachabilityRating(…)`): Rating from a number of reachable destinations, `floor(destinations^0.65 * 0.55 * scale)`. No base-game script calls it.
    - param `#1`: Number of destinations.
    - param `#2`: Scale.
    - returns: Rating as an integer.
- `getTownCapacityUsageRating : function(UtilTown.TownCapacityUsage, number) : integer`  (call as `town_util.getTownCapacityUsageRating(…)`): Capacity usage as a percentage, `used * scale * 100 / capacity` rounded down. No base-game script calls it.
    - param `#1`: Capacity usage.
    - param `#2`: Scale.
    - returns: Percentage; `0` when the capacity is 0.
- `getCargoTypeSupplyDemandRating : function(CargoTypeSupplyDemand, number) : integer`  (call as `town_util.getCargoTypeSupplyDemandRating(…)`): Supply as a percentage of demand, `supply * 100 * scale / demand` rounded down. No base-game script calls it.
    - param `#1`: Supply and demand of a cargo type.
    - param `#2`: Scale.
    - returns: Percentage; `0` when the demand is 0.
- `findStateIndex : function(TownsState, EntityUtil.EntityAndRevision) : integer`  (call as `town_util.findStateIndex(…)`): Index of a town's state in `townStates`.
    - param `#1`: State of the town script.
    - param `#2`: Town entity with revision.
    - returns: Index, or `-1` when the town has no stored state.
- `getTownState : function(TownsState, Engine.Entity) : TownState`  (call as `town_util.getTownState(…)`): State of a town. Missing fields of a stored state are filled with defaults. A town without stored state gets a new default state, which is not added to `townsState`.
    - param `#1`: State of the town script.
    - param `#2`: Town entity.
    - returns: State of the town, or `nil` when the entity is not a town.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:227`: `local sanitizedTownState = town_util.getTownState(townsState, townState.townEntity.entity)`
- `setTownState : function(TownsState, Engine.Entity, TownState) : TownState`  (call as `town_util.setTownState(…)`): Stores the state of a town in the script state, replacing an existing entry or appending a new one.
    - param `#1`: State of the town script; changed in place.
    - param `#2`: Town entity.
    - param `#3`: State of the town.
    - returns: Declared as the town state, but the implementation returns nothing.
- `externalGetTownState : function(Engine.Entity) : TownState`  (call as `town_util.externalGetTownState(…)`): State of a town, read from the `town.gs` game script component. Use it outside the town script.
    - param `#1`: Town entity.
    - returns: State of the town.
    - used 38x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:80`: `local townState = town_util.externalGetTownState(townEntity)`
- `externalGetTownsState : function() : TownsState`  (call as `town_util.externalGetTownsState(…)`): Full state of the `town.gs` game script, read from its component.
    - returns: State of the town script.
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:214`: `local townsState = town_util.externalGetTownsState()`
- `getTrafficSpeedRating : function(Engine.Entity, number) : {number, {integer}}`  (call as `town_util.getTrafficSpeedRating(…)`): Raw traffic data of a town from `api.engine.util.town.computeTownsTrafficSpeedMap`.
    - param `#1`: Town entity.
    - param `#2`: Samples coefficient passed to the engine.
    - returns: The raw traffic rating and the street counts per traffic level, or `nil` without data.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util_parallel.script.tl:34`: `local rating = town_util.getTrafficSpeedRating(townEntity, advancedOptions.trafficSpeedSensitivityScale)`
- `getItemsAtDestinationPerYear : function(Engine.Entity, CargoTypeId) : integer`  (call as `town_util.getItemsAtDestinationPerYear(…)`)
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:142`: `return town_util.getItemsAtDestinationPerYear(townEntity, cargoTypeId)`
- `getRatingFromCache : function(TownUtil.Rating, Engine.Entity, TownState) : TownState.CachedRating`  (call as `town_util.getRatingFromCache(…)`): Value of a rating from `townState.cachedRatings`, computed with `rating.getRating` when it is not cached.
    - param `#1`: Rating.
    - param `#2`: Town entity.
    - param `#3`: State of the town.
    - returns: Value and critical flag of the rating.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:231`: `local cachedRating = town_util.getRatingFromCache(rating, townState.townEntity.entity, sanitizedTownState)`
- `calcAuthorityScore : function(Engine.Entity, TownState) : number`  (call as `town_util.calcAuthorityScore(…)`): Overall rating of a town, the lowest value of the six ratings. Also refreshes `townState.cachedRatings` and their critical flags.
    - param `#1`: Town entity.
    - param `#2`: State of the town; `cachedRatings` is changed.
    - returns: Score between 0 and 1.
- `logAuthorityRatings : function(Engine.Entity, TownState) : number`  (call as `town_util.logAuthorityRatings(…)`): Writes the values of `GetRatingsToLog` (times 1000) to the town's journal.
    - param `#1`: Town entity.
    - param `#2`: State of the town.
    - returns: Not used by the base game.
- `calculateProposalImpact : function(ProposalStats.TownProposalStats) : TownProposalImpact`  (call as `town_util.calculateProposalImpact(…)`): Reputation penalties of one town from the statistics of a proposal.
    - param `#1`: Proposal statistics of the town.
    - returns: Penalty per reason.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:3825`: `local impact = town_util.calculateProposalImpact(townStats)`
- `applyEventDecay : function(TownAuthorityEventFactors, integer, integer, number) : TownAuthorityEventFactors`  (call as `town_util.applyEventDecay(…)`): Advances the reputation state of a town by the time since the last update. Ends a finished marketing campaign, updates the moving item average and moves `itemDelivery` and `onTimeItemDelivery` back towards 1.
    - param `#1`: Previous reputation state.
    - param `#2`: Time since the last update, in ms.
    - param `#3`: Current game time, in ms.
    - param `#4`: Recovery bonus; `1 + bonus` scales the recovery rate of `itemDelivery`.
    - returns: New reputation state; `score` and `reasonToPenalty` are copied unchanged.
- `applyEventStats : function(TownAuthorityEventFactors, ProposalStats.TownProposalStats) : TownAuthorityEventFactors`  (call as `town_util.applyEventStats(…)`): Adds the penalties of a proposal (trees, rocks, terrain modification, demolished buildings) to a copy of the reputation state.
    - param `#1`: Previous reputation state.
    - param `#2`: Proposal statistics of the town.
    - returns: New reputation state.
- `applyMarketingCamaign : function(TownAuthorityEventFactors, params : TownMarketingMetadata.ConstructionDesc) : TownAuthorityEventFactors`  (call as `town_util.applyMarketingCamaign(…)`): Starts a marketing campaign at the current game time in a copy of the reputation state.
    - param `#1`: Previous reputation state.
    - param `params`: Campaign settings.
    - returns: New reputation state, or the previous one when `params` is `nil`.
- `applyItemDelivered : function(TownAuthorityEventFactors, boolean, Engine.Entity) : TownAuthorityEventFactors`  (call as `town_util.applyItemDelivered(…)`): Counts one delivered item in a copy of the reputation state and updates the on-time share as a running average over at least 50 items.
    - param `#1`: Previous reputation state.
    - param `#2`: Whether the item arrived on time.
    - param `#3`: Town entity (not used).
    - returns: New reputation state.
- `getXpFactor : function(Engine.Entity) : number`  (call as `town_util.getXpFactor(…)`): Experience factor of a town, `authority2xpFactor[growthLevel] * (1 + constructionBoni.xpIncrease)`.
    - param `#1`: Town entity.
    - returns: Factor applied to the experience the town gains.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:876`: `result.currentMultiplier = town_util.getXpFactor(params.entity)`
- `getSeveredMainConnections : function(townEntity : Engine.Entity) : {Engine.Entity}`  (call as `town_util.getSeveredMainConnections(…)`): Towns whose main connection with this town is disconnected or too long (town problems `Disconnected` and `Overlength`).
    - param `townEntity`: Town entity.
    - returns: Entities of the other towns.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:88`: `local numSeveredConnections = #town_util.getSeveredMainConnections(townEntity)`
- `getTownEmissionEmitters : function(Engine.Entity, boolean, byLine? : {Engine.Entity : number}) : {EmitterCategory : number}`  (call as `town_util.getTownEmissionEmitters(…)`): Share of each source category in the noise or pollution of a town. Noise counts the emitters within 150 m of the affected districts (`getNoiseLandUseTypes`).
    - param `#1`: Town entity.
    - param `#2`: True for pollution, false for noise.
    - param `byLine`: Optional table that receives the share per line of the player's vehicles.
    - returns: Share per category (the shares add up to 1), or an empty table without emission.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util_parallel.script.tl:79`: `local emissionSources = town_util.getTownEmissionEmitters(params.townEntity, params.pollution, params.byLine and pollutionPerLine or nil)`
- `getNoiseLandUseTypes : function() : {LandUseType}`  (call as `town_util.getNoiseLandUseTypes(…)`): District types affected by noise, from `townNoiseResidentialAffected`, `townNoiseCommercialAffected` and `townNoiseIndustrialAffected` of the base config's emission settings.
    - returns: List of district types.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1796`: `local noisePerDistrict = api.engine.util.town.getTownNoisePerDistrict(params.entity, town_util.getNoiseLandUseTypes())`
- `getRatingHappinessFromStats : function(unhappyAndTotal : UtilTown.TownHappinessStats, townState : TownState) : number`  (call as `town_util.getRatingHappinessFromStats(…)`): Happiness rating from happiness statistics. People travelling inside the town count twice; at least 15 people are assumed. The share of happy people is mapped between bounds that depend on the sensitivity.
    - param `unhappyAndTotal`: Happiness statistics of the town.
    - param `townState`: State of the town.
    - returns: Rating between 0 and 1; `1` when the sensitivity is 0.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1566`: `local rating = town_util.getRatingHappinessFromStats(value.stats, townState)`
- `getFactorLevelNumber : function(number) : integer`  (call as `town_util.getFactorLevelNumber(…)`): Level number of a rating value, `floor(value * 6)` limited to 0 to 5.
    - param `#1`: Rating value between 0 and 1.
    - returns: Level number.
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:648`: `local ratingValueShort : integer = town_util.getFactorLevelNumber(ratingValue)`
- `getRatingLevel : function(integer) : RatingLevel`  (call as `town_util.getRatingLevel(…)`): Rating level of a level number; 0 is `VeryPoor`, 5 and above is `Excellent`.
    - param `#1`: Level number.
    - returns: Rating level.
    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:267`: `local passengerRatingLevel = town_util.getRatingLevel(state:old().passengerRating)`
- `getFactorRatingLevel : function(number) : RatingLevel`  (call as `town_util.getFactorRatingLevel(…)`): Rating level of a rating value.
    - param `#1`: Rating value between 0 and 1.
    - returns: Rating level.
    - used 22x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:241`: `local ratingLevel = town_util.getFactorRatingLevel(ratingValue)`
- `isValidRating : function(Rating, RatingLevel) : boolean`  (call as `town_util.isValidRating(…)`): Whether a rating level is shown for a rating. Only a hidden rating at `Mediocre` is not valid.
    - param `#1`: Rating.
    - param `#2`: Rating level.
    - returns: False for a hidden rating at `Mediocre`, otherwise true.
    - used 1x in the base game, e.g. `base/content/gui/gui/statistics/statistic_towns.tl:279`: `if town_util.isValidRating(staticRating, ratingLevelName) and key ~= "station_demand" then`
- `getRatingSensitivity : function(TownState, ratingName : TownRatingKey) : number`  (call as `town_util.getRatingSensitivity(…)`): Sensitivity of a rating for a town; the town's own value from `ratingSensitivity`, otherwise the default from `town_ratings_sensitivity.res` scaled through the script option.
    - param `#1`: State of the town.
    - param `ratingName`: Rating key.
    - returns: Sensitivity; 0 or less turns the rating off (it returns 1).
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebrations.script.tl:83`: `local sensitivity = town_util.getRatingSensitivity(townState, "urban_care")`
- `getTownsToPositions : function() : {Engine.Entity : Vec3f}`  (call as `town_util.getTownsToPositions(…)`): Position of every town.
    - returns: Position per town entity.
- `makeGetClosestTownAndDistanceFunction : function() : function(Vec3f) : {Engine.Entity, number}`  (call as `town_util.makeGetClosestTownAndDistanceFunction(…)`): Builds a function that finds the closest town to a position. The town positions are read once, when this is called.
    - returns: Function that returns the closest town and its distance in metres, or `nil` without towns.
- `makeGetAffectedTownsFunction : function() : function(Vec3f, number) : {Engine.Entity}`  (call as `town_util.makeGetAffectedTownsFunction(…)`): Builds a function that finds the towns affected by a circle (position and radius). The town positions are read once, when this is called.
    - returns: Function that returns the closest town and all towns within two radii of its distance.
- `getProposalStats : function(Type.Proposal, Type.ProposalData, boolean) : ProposalStats`  (call as `town_util.getProposalStats(…)`): Effects of a construction proposal on nearby towns; removed and placed trees and rocks, demolished town buildings, terrain and tunnel volume.
    - param `#1`: Proposal.
    - param `#2`: Proposal data.
    - param `#3`: Whether the player started the proposal.
    - returns: Statistics per affected town; empty when not player-initiated.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:3819`: `local proposalStats : ProposalStats = town_util.getProposalStats(proposal, proposalData, true)`
- `getProposalActions : function(Proposal, data : ProposalData) : TownProposalActions`  (call as `town_util.getProposalActions(…)`): Actions of a proposal that need a minimum town rating (station, airport, demolition) and the towns they affect.
    - param `#1`: Proposal.
    - param `data`: Proposal data.
    - returns: Actions per affected town.
- `getActionAllowed : function(TownState, TownProposalActions.Action) : boolean`  (call as `town_util.getActionAllowed(…)`): Whether a town's rating allows an action.
    - param `#1`: State of the town.
    - param `#2`: Action.
    - returns: True when the rating level of `authorityScore` is high enough for the action.
- `getTownName : function(Engine.Entity) : string`  (call as `town_util.getTownName(…)`): Name of a town.
    - param `#1`: Town entity.
    - returns: Town name.
- `getTownSize : function(Engine.Entity) : integer`  (call as `town_util.getTownSize(…)`): Average person capacity of the three districts of a town, rounded. No base-game script calls it.
    - param `#1`: Town entity.
    - returns: Average capacity; `0` for an unknown town.
- `getSubventionsEnabled : function(Engine.Entity) : boolean`  (call as `town_util.getSubventionsEnabled(…)`): Whether a town offers subsidies. The subsidy scripts check this.
    - param `#1`: Town entity.
    - returns: True when the town's rating level is `VeryGood`.
- `changeTownAuthorityPenalty : function(TownAuthorityEventFactors, TownAuthorityEventFactors.EventType, number, ?number) : number`  (call as `town_util.changeTownAuthorityPenalty(…)`): Changes the stored penalty of one reason. The new penalty stays between 0 and the larger of the maximum and the current penalty. The score is not changed.
    - param `#1`: Reputation state; changed in place.
    - param `#2`: Reason.
    - param `#3`: Change of the penalty.
    - param `#4`: Maximum penalty, default 1.
    - returns: Actual change of the penalty.
- `addTownAuthorityPenalties : function(TownAuthorityEventFactors, {TownAuthorityEventFactors.EventType : number})`  (call as `town_util.addTownAuthorityPenalties(…)`): Changes the penalties of several reasons and lowers or raises `score` by the total change.
    - param `#1`: Reputation state; changed in place.
    - param `#2`: Change of the penalty per reason.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:3831`: `town_util.addTownAuthorityPenalties(`
- `recoverReputation : function(TownAuthorityEventFactors, number, {TownAuthorityEventFactors.EventType : number})`  (call as `town_util.recoverReputation(…)`): Raises `score` by up to an amount and shrinks the stored penalties in proportion. Penalties in the overrides stay at least at their override value.
    - param `#1`: Reputation state; changed in place.
    - param `#2`: Amount of score to recover.
    - param `#3`: Penalties that cannot recover, per reason (the base game passes severed connections).
- `getReputation : function(TownAuthorityEventFactors, ?{TownAuthorityEventFactors.EventType : number}) : number`  (call as `town_util.getReputation(…)`): Reputation score of a town; override penalties above the stored ones are subtracted.
    - param `#1`: Reputation state.
    - param `#2`: Optional penalties per reason that apply at least.
    - returns: Score between 0 and 1; `1` without reputation state.
- `getReputationBreakdown : function(TownAuthorityEventFactors, ?{TownAuthorityEventFactors.EventType : number}) : {TownAuthorityEventFactors.EventType : number}`  (call as `town_util.getReputationBreakdown(…)`): Lost reputation split by reason, in proportion to each reason's penalty.
    - param `#1`: Reputation state.
    - param `#2`: Optional penalties per reason that apply at least.
    - returns: Share of the lost reputation per reason; the shares add up to `1 - reputation`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1050`: `local penalties = town_util.getReputationBreakdown(townState.eventFactors, { SeveredConnection = targetPenalty })`
- `level2name : function(integer) : string`  (call as `town_util.level2name(…)`): Localized name of a town level, from "Small Hamlet" at level 1 to "Large Metropolis" at level 18.
  In game: "Small Hamlet", "Town", "Large Metropolis" (German: „Kleiner Weiler“, „Städtchen“, „Grosse Metropole“)
    - param `#1`: Town level; values outside 1 to 18 are clamped.
    - returns: Localized level name.
    - used 7x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:50`: `townType = town_util.level2name(value),`
- `getTownForSimEntity : function(simEntity : Engine.Entity, direction : boolean) : Engine.Entity`  (call as `town_util.getTownForSimEntity(…)`): Town of the station at the start or end stop of a sim person's or cargo's current line trip.
    - param `simEntity`: Sim entity in a vehicle or at a terminal.
    - param `direction`: True for the start stop (`lineStop0`), false for the end stop (`lineStop1`).
    - returns: Town entity, or `nil` when the station group has no stations.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/subventions/deliver_passengers/deliver_passengers.script.tl:268`: `local fromTown = town_util.getTownForSimEntity(simEntity, true)`
- `isMarketingActive : function(atTimestamp : integer, factors : TownAuthorityEventFactors) : boolean`  (call as `town_util.isMarketingActive(…)`): Whether a marketing campaign runs at a given time.
    - param `atTimestamp`: Game time in ms.
    - param `factors`: Reputation state of the town.
    - returns: True when `atTimestamp` is before the campaign's start plus `durationMs`.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1065`: `local hasMarketingCampaign = town_util.isMarketingActive(now, townState.eventFactors)`
- `forEachMatchingStocklist : function(stockList : Engine.Component.StockList, callback : function(Engine.Entity, CargoTypeId)) : {CargoTypeId : boolean}`  (call as `town_util.forEachMatchingStocklist(…)`): return input/output cargo types (depending on argument stockListType)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry.tl:240`: `town_util.forEachMatchingStocklist(stockList,`
- `isCapital : function(townEntity : Engine.Entity) : boolean`  (call as `town_util.isCapital(…)`): Whether a town is the player's capital, the town closest to the player's headquarters.
    - param `townEntity`: Town entity.
    - returns: True for the capital.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/town_hud_react_util.tl:63`: `local isHQTown = town_util.isCapital(params.entity)`

### TownUtil.Rating (record)

```teal
record Rating
```

Description of one town rating with its name, icons and the functions that compute it.

- `name : string`: Localized display name.
- `icon : string`: Path of the small icon.
- `iconLarge : string`: Path of the 32 px icon.
- `range : {integer, integer}`
- `added : boolean`: When true, `calcAuthorityScore` adds `(value - 0.5) * 2` to the score instead of taking the minimum. No base-game rating sets it.
- `hidden : boolean`: When true, `isValidRating` treats the `Mediocre` level as not valid.
- `getRating : function(Engine.Entity, TownState) : number`: Computes the rating value.
    - param `#1`: Town entity.
    - param `#2`: State of the town.
    - returns: Rating value between 0 and 1.
- `getRatingFnName : ResName`: Script path of the matching `TownUtilParallel` function, used by the GUI with `engine_react_util.useStepStateParallel`.
- `getRatingFnExtraParam : any`: Passed as `extraParam` to the function named in `getRatingFnName`.
- `key : string`: Rating key, also the key in `TownState.cachedRatings`.
- `debugGetInfo : function(Engine.Entity, TownState) : string`: Debug text for the rating; the noise and pollution ratings return their random offset in dB.
    - param `#1`: Town entity.
    - param `#2`: State of the town.
    - returns: Debug text.
- `onClickEvent : {string, any}`: GUI event name and param fired when the rating is clicked, for example `{"openLayerRidge", { layer = "menu.layers.noiseButton", stack = true }}`.

### TownUtil.RatingDeliveries (record)

```teal
record RatingDeliveries
```

Combined supplies rating of a town (`getRatingDeliveries`).

- `ratingLevelNumber : integer`: Level number from 0 (`VeryPoor`) to 5 (`Excellent`).
- `noDeliveries : boolean`: True when the town has demand but nothing is supplied.

### TownUtil.EmitterCategory (enum)

```teal
enum EmitterCategory
```

Categories of noise and pollution sources (`getTownEmissionEmitters`).

- `"company_vehicles"`: Vehicles of the player's transport lines.
  In game: "Company Vehicles" (German: „Unternehmensfahrzeuge“)
- `"residents_vehicles"`: Other vehicles, such as the residents' cars.
  In game: "Residents' Vehicles" (German: „Einwohnerfahrzeuge“)
- `"industries"`: Industries and fields.
  In game: "Industries" (German: „Industrien“)
- `"depots"`: Vehicle depots with track or street access.
  In game: "Depots" (German: „Depots“)
- `"maintenance_buildings"`: Vehicle depots without in or out nodes.
  In game: "Maintenance Buildings" (German: „Wartungsgebäude“)
- `"stations"`: Stations and station groups.
  In game: "Stations" (German: „Stationen“)
- `"tracks"`: Tracks and streets (base edges).
  In game: "Tracks" (German: „Gleise“)
- `"warehouses"`: Warehouses.
  In game: "Warehouses" (German: „Lager“)
- `"other"`: Any other source.
  In game: "Other" (German: „Sonstiges“)
