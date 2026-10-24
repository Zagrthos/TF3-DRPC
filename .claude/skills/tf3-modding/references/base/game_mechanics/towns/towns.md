# game_mechanics.towns.towns

Definition file: `base/tealdef/game_mechanics/towns/towns.d.tl` (in the game folder).

State records of the town reputation script (`town.gs`) and the shared town types (ratings, supply and demand, proposal statistics).

## Globals

- `TownRatingsSensitivityConfig : {TownRatingsSensitivityConfigName : integer}`: Option index per sensitivity key, as stored in `town_ratings_sensitivity.res` (`2` for urban care, `4` for the others). `difficulty_util.getScale` turns the index into the sensitivity value.

## TownAuthoritySeveredConnection (record)

```teal
global record TownAuthoritySeveredConnection
```

- `penalty : number`
- `refund : number`
- `severedTimestamp : integer`

## TownProposalImpact (record)

```teal
global record TownProposalImpact
```

Reputation penalties of one town caused by a construction proposal, from `TownUtil.calculateProposalImpact`. Positive values lower the reputation.

- `trees : number`: Penalty for removed trees minus planted trees (`0.0025` per weighted tree).
- `rocks : number`: Penalty for removed rocks minus placed rocks (`0.0025` per weighted rock).
- `terrainModification : number`: Penalty for terrain brush, terrain alignment and tunnel volume (about 2 million cubic metres for a full penalty of 1).
- `demolishedTownBuilding : number`: Penalty for demolished town buildings, `fractionDistrictRemoved / 0.405`.
- `landscaping : number`: Combined landscaping value; `TownUtil.applyEventStats` does not use it.

## TownAuthorityEventFactors (record)

```teal
global record TownAuthorityEventFactors
```

Reputation state of a town (`TownState.eventFactors`). It holds the score, the penalty per reason, item delivery statistics and the running marketing campaign.

- `score : number`: range 0 .. 1
- `marketingInitiatedTimestamp : integer`: Game time (ms) when the current marketing campaign started; `0` when there is none.
- `marketingParams : TownMarketingMetadata.ConstructionDesc`: Settings of the current marketing campaign.
- `itemDelivery : number`: range 0 .. 1
- `numItemsDelivered : integer`: Items delivered to the town since the last town update.
- `averageItemsDelivered : number`: Items delivered within a moving window of half a game year.
- `onTimeItemDelivery : number`: range 0 .. 1
- `reasonToPenalty : {EventType : number}`: Current reputation penalty per reason, each between 0 and 1.

### TownAuthorityEventFactors.EventType (enum)

```teal
enum EventType
```

Reasons for a reputation penalty.

- `"Unknown"`: Penalty from an older save that had no reasons stored.
- `"Trees"`: Trees removed near the town.
  In game: "Removed Trees", "Replant some trees to improve reputation." (German: „Entfernte Bäume“, „Pflanze neue Bäume, um den Ruf zu verbessern.“)
- `"Rocks"`: Rocks removed near the town.
  In game: "Removed Rocks", "Place some rocks to improve reputation." (German: „Entfernte Felsen“, „Platziere Felsen, um den Ruf zu verbessern.“)
- `"TerrainModification"`: Terrain changed near the town (terrain brush, terrain alignment, tunnels).
  In game: "Landscaping" (German: „Landschaftsbau“)
- `"SeveredConnection"`: A main connection to another town is disconnected or too long.
  In game: "Interrupted Main Connections", "Restore main connection to improve reputation." (German: „Unterbrochene Hauptverbindungen“, „Stelle die Hauptverbindungen wieder her, um den Ruf zu verbessern.“)
- `"FailedSubsidy"`: Failed subsidy; the default reason of the `decreaseReputation` event.
  In game: "Failed Subsidy" (German: „Gescheiterte Subvention“)
- `"DemolishedTownBuilding"`: Town buildings demolished.
  In game: "Demolished Buildings" (German: „Abgerissene Gebäude“)

## TownConstructionBoni (record)

```teal
global record TownConstructionBoni
```

Bonuses of a town from nearby constructions with `town_growth` metadata, summed and refreshed every 20 updates.

- `xpIncrease : number`: Sum of `xpIncrease`; the experience multiplier is `1 + xpIncrease`.
- `trafficRatingIncrease : number`: Sum of `trafficRatingIncrease`, added to the traffic rating.
- `reputationRecoveryBoost : number`: Sum of `reputationRecoveryBoost`, passed to `TownUtil.applyEventDecay`.

## TownRatingsSensitivityConfigName (enum)

```teal
global enum TownRatingsSensitivityConfigName
```

Keys of the default rating sensitivities in `town_ratings_sensitivity.res`, one per `TownRatingKey`.

- `"SensitivityUrbanCare"`: Default sensitivity of the reputation rating (`urban_care`).
- `"SensitivityTrafficCongestion"`: Default sensitivity of the traffic rating.
- `"SensitivityPeopleHappiness"`: Default sensitivity of the happiness rating.
- `"SensitivityCargoDelivery"`: Default sensitivity of the delivery time rating.
- `"SensitivityNoise"`: Default sensitivity of the noise rating.
- `"SensitivityPollution"`: Default sensitivity of the pollution rating.

## TownState (record)

```teal
global record TownState
```

Reputation and rating state of one town in the `town.gs` game script. Get it with `TownUtil.getTownState` or `TownUtil.externalGetTownState`.

- `townEntity : EntityUtil.EntityAndRevision`: Town entity with its revision.
- `sizeFactors : {number}`
- `authorityScore : number`: Overall town rating between 0 and 1, the lowest of the six ratings (`TownUtil.calcAuthorityScore`).
- `deliveryRating : RatingLevel`: Supplies rating level, updated on each town update. A change sends the `Towns`/`DeliveryRatingChanged` event.
- `constructionBoni : TownConstructionBoni`: Bonuses from nearby constructions.
- `eventFactors : TownAuthorityEventFactors`: Reputation state of the town.
- `lastApplyTime : integer`: Game time (ms) of the last town update.
- `cachedRatings : {string : CachedRating}`: Last computed ratings per rating key, written by `TownUtil.calcAuthorityScore`.
- `ratingSensitivity : {string : number}`: Sensitivity per rating key set for this town with the `setTownRatingSensitivity` event; `nil` uses the defaults.

### TownState.CachedRating (record)

```teal
record CachedRating
```

Last computed value of one rating.

- `value : number`: Rating value between 0 and 1.
- `critical : boolean`: Set when the rating falls to `VeryPoor`; cleared once it is above `Poor` again.

## TownsState (record)

```teal
global record TownsState
```

State of the town game script `town.gs` (`towns.script`).

- `townStates : {TownState}`: State of each town.
- `version : integer`: Version of the state layout; `TownLegacyUtil.updateLegacyState` migrates older versions.

## CargoTypeSupplyDemand (record)

```teal
global record CargoTypeSupplyDemand
```

Supply and demand of one cargo type in a town district.

- `cargoTypeId : CargoTypeId`: Cargo type.
- `supply : integer`: Supplied amount, capped at `demand`.
- `demand : integer`: Demanded amount.

## CargoTypeSupplyDemands (record)

```teal
global record CargoTypeSupplyDemands
```

Supply and demand per cargo type for each district type.

- `residential : {CargoTypeSupplyDemand}`: Cargo supply and demand of the residential district.
- `commercial : {CargoTypeSupplyDemand}`: Cargo supply and demand of the commercial district.
- `industrial : {CargoTypeSupplyDemand}`: Cargo supply and demand of the industrial district.

## TownCapacities (record)

```teal
global record TownCapacities
```

Person capacity of a town per district type.

- `residential : integer`: Person capacity of the residential district.
- `commercial : integer`: Person capacity of the commercial district.
- `industrial : integer`: Person capacity of the industrial district.

## TownCapacityUsages (record)

```teal
global record TownCapacityUsages
```

Used and total capacity of a town per district type.

- `residential : UtilTown.TownCapacityUsage`: Capacity usage of the residential district.
- `commercial : UtilTown.TownCapacityUsage`: Capacity usage of the commercial district.
- `industrial : UtilTown.TownCapacityUsage`: Capacity usage of the industrial district.

## GrowthSummand (record)

```teal
global record GrowthSummand
```

- `label : string`
- `value : integer`

## LandUseGrowth (record)

```teal
global record LandUseGrowth
```

- `summands : {GrowthSummand}`

## RatingLevel (enum)

```teal
global enum RatingLevel
```

Six-step level of a town rating. `TownUtil.getFactorRatingLevel` maps a value between 0 and 1 to a level in steps of 1/6.

- `"VeryPoor"`: Lowest level (value below 1/6).
  In game: "Very Poor", "Stagnant" (German: „Sehr schlecht“, „Stagnierend“)
- `"Poor"`: Second level.
  In game: "Poor", "Growing Very Slowly" (German: „Schlecht“, „Sehr Langsames Wachstum“)
- `"Mediocre"`: Third level.
  In game: "Mediocre", "Growing Slowly" (German: „Mittelmässig“, „Langsames Wachstum“)
- `"Good"`: Fourth level.
  In game: "Good", "Growing" (German: „Gut“, „Wachsend“)
- `"VeryGood"`: Fifth level. Towns allow subsidies at this level (`TownUtil.getSubventionsEnabled`).
  In game: "Very Good", "Growing Fast" (German: „Sehr gut“, „Schnell Wachsend“)
- `"Excellent"`: Highest level (value of 5/6 or more).
  In game: "Excellent", "Growing Lightning Fast" (German: „Ausgezeichnet“, „Unfassbar schnell wachsend“)

## StationSamples (record)

```teal
global record StationSamples
```

Loaded and lost items per year of a town's stations. Only commented-out code in `town_util.tl` uses it.

- `loaded : integer`: Items loaded per year (logbook value `itemsLoaded`).
- `lost : integer`: Items lost per year (logbook value `itemsLost`).

## ProposalStats (record)

```teal
global record ProposalStats
```

Effects of a construction proposal on each town nearby, from `TownUtil.getProposalStats`. The town script turns them into reputation penalties before the proposal is built.

- `townStats : {Engine.Entity : TownProposalStats}`: Effects per affected town entity. Empty when the proposal is not player-initiated.

### ProposalStats.TownProposalStats (record)

```teal
record TownProposalStats
```

Effects of the proposal on one town. Tree and rock counts are weighted by distance to the town (`api.engine.util.town.getImpactFromDistance`).

- `addedTrees : number`: Weighted number of trees planted, after cancelling out trees removed in the same proposal.
- `removedTrees : number`: Weighted number of trees removed, scaled by each tree model's `reputationFactor`.
- `addedRocks : number`: Weighted number of rocks placed, after cancelling out rocks removed in the same proposal.
- `removedRocks : number`: Weighted number of rocks removed.
- `removedBuildings : {Building}`: Town buildings the proposal removes without replacing them.
- `fractionDistrictRemoved : number`: Removed person capacity divided by the capacity of the building's district, summed over all removed buildings.
- `terrainMod : TerrainMod`: Terrain volume changes.
- `tunnelVolumeAdded : number`: Net tunnel volume added (length times width times height), weighted by distance to the town.

#### ProposalStats.TownProposalStats.Building (record)

```teal
record Building
```

A town building the proposal removes.

- `plotSize : integer`: Plot size of the building, parcel count times depth.
- `level : integer`: Level of the town building.

#### ProposalStats.TownProposalStats.TerrainMod (record)

```teal
record TerrainMod
```

Terrain volume changes of the proposal.

- `brushVolumeAdded : number`: Terrain volume added with the terrain brush, in cubic metres.
- `brushVolumeRemoved : number`: Terrain volume removed with the terrain brush, in cubic metres.
- `terrainAlignmentVolumeAdded : number`: Terrain volume added by terrain alignment of constructions. It is scaled by a factor that falls from 1 to 0.5 while the proposal's construction area grows between 10 000 and 50 000.
- `terrainAlignmentVolumeRemoved : number`: Terrain volume removed by terrain alignment of constructions, scaled like `terrainAlignmentVolumeAdded`.

## TownProposalActions (record)

```teal
global record TownProposalActions
```

Actions of a proposal that a town's rating may forbid, per affected town (`TownUtil.getProposalActions`).

Notes:

The base game checks these only in code that is commented out (`guiHandleEvent` in `towns.script.tl`).
In `getProposalActions` the catchments of stations with an airport go to `BuildStation` and those
without one to `BuildAirport`.

- `townActions : {Engine.Entity : {Action}}`: Actions per affected town entity.

### TownProposalActions.Action (enum)

```teal
enum Action
```

Action types checked by `TownUtil.getActionAllowed`.

- `"BuildStation"`: Build a station or stop; allowed above `VeryPoor`.
- `"DestroyBuilding"`: Demolish a town building; allowed above `Poor`.
- `"BuildAirport"`: Build an airport; allowed above `Mediocre`.

## TownLegacyUtil (record)

```teal
global record TownLegacyUtil
```

Migration of old `TownsState` layouts (`game_mechanics/towns/town_legacy_util.tl`).

- `currentVersion : integer`: Current state version (`8`).
- `updateLegacyState : function(townsState : TownsState)`: Migrates a town script state from an older version to `currentVersion` and sets `version`. The town script calls it on the `handleLegacy` event.
    - param `townsState`: State of the town script; changed in place.
