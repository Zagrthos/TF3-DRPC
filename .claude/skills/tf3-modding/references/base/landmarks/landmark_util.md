# landmarks.landmark_util

Definition file: `base/tealdef/landmarks/landmark_util.d.tl` (in the game folder).

Types of `/landmarks/landmark_util.tl` (`LandmarkEngineUtil`), which adds up the town bonuses that
finished landmarks and the headquarter grant to their closest town.

## LandmarkEngineUtil (record)

```teal
global record LandmarkEngineUtil
```
Load: `local landmark_util = ug_require "::/landmark_util.tl" as LandmarkEngineUtil`

Landmark bonus helpers returned by `ug_require "/landmarks/landmark_util.tl"`.

- `collectTownGrowthMetadata : function(constructionEntity : Engine.Entity, construction : Engine.Component.Construction, townBoni : {Engine.Entity : TownBonuses}, singleTown? : Engine.Entity)`  (call as `landmark_util.collectTownGrowthMetadata(…)`): Adds a construction's `town_growth` bonuses to the entry of its closest town in `townBoni`.
    - param `singleTown`: If set, only that town is considered.
- `collectVehiclesBonusesMetadata : function(constructionEntity : Engine.Entity, construction : Engine.Component.Construction, townBoni : {Engine.Entity : TownBonuses}, singleTown? : Engine.Entity)`  (call as `landmark_util.collectVehiclesBonusesMetadata(…)`): Adds the `effects` of a construction's `constructionConfig` to the `carrierModifiers` of its closest town in `townBoni`.
    - param `singleTown`: If set, only that town is considered.
- `getTownBonuses : function(includeTownGrowth : boolean, includeVehicles : boolean, town : Engine.Entity) : TownBonuses`  (call as `landmark_util.getTownBonuses(…)`): Sums the bonuses of all constructions whose closest town is `town`.
    - param `includeTownGrowth`: Include `town_growth` bonuses.
    - param `includeVehicles`: Include vehicle bonuses from `constructionConfig.effects`.
    - param `town`: The town entity.
    - returns: The bonuses; an empty table (and an error in the log) if both flags are false.

### LandmarkEngineUtil.TownBonuses (record)

```teal
record TownBonuses
```

Bonuses a town receives from landmarks and headquarters.

- `xpIncrease : LandmarksModifierValue`: Town experience bonus (from `town_growth` metadata).
- `trafficRatingIncrease : LandmarksModifierValue`: Traffic rating bonus (from `town_growth` metadata).
- `reputationRecoveryBoost : LandmarksModifierValue`: Reputation recovery bonus (from `town_growth` metadata).
- `carrierModifiers : {string : {string : LandmarksModifierValue}}`: Vehicle bonuses by carrier and effect type, from the `effects` of `constructionConfig`.

#### LandmarkEngineUtil.TownBonuses.LandmarksModifierValue (record)

```teal
record LandmarksModifierValue
```

Sum of one bonus and the constructions that contribute to it.

- `value : number`: Summed bonus value.
- `landmarks : {Engine.Entity}`: Construction entities that contribute.
