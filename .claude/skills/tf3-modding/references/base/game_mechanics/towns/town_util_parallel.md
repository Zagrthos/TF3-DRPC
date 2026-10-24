# game_mechanics.towns.town_util_parallel

Definition file: `base/tealdef/game_mechanics/towns/town_util_parallel.d.tl` (in the game folder).

Town functions for GUI widgets that run through `engine_react_util.useStepStateParallel` and return plain tables.

## TownUtilParallel (record)

```teal
global record TownUtilParallel
```

Town functions for GUI widgets. The GUI calls them by script path through `engine_react_util.useStepStateParallel` or `useStepStateParallelSimple`, so they return plain tables without userdata.

Example:
```lua
local state = engine_react_util.useStepStateParallelSimple(
    "::/game_mechanics/towns/town_util_parallel.script@town_util_parallel.getDeliveriesState",
    params.entity) as ReactStateT<TownUtilParallel.DeliveriesState>
```

- `getCargoSupplyDemandsLandUseWithResidentialLineUsage : function(param : GetCargoSupplyDemandsLandUseParam) : {CargoTypeSupplyDemand}`: Calls `TownUtil.getCargoSupplyDemandsLandUseWithResidentialLineUsage`.
    - param `param`: Town and district type.
    - returns: List of supply and demand entries.
- `getDeliveriesState : function(townEntity : Engine.Entity) : DeliveriesState`: Supply ratings of a town.
    - param `townEntity`: Town entity.
    - returns: Supply rating levels per district and for passengers.
- `getTrafficWidgetState : function(townEntity : Engine.Entity) : TrafficWidgetState`: Data of the traffic widget of a town.
    - param `townEntity`: Town entity.
    - returns: Traffic rating level and street counts.
- `getHappinessWidgetState : function(townEntity : Engine.Entity) : HappinessWidgetState`: Happiness statistics of a town as a plain table.
    - param `townEntity`: Town entity.
    - returns: Happiness statistics.
- `getNoisePollutionWidgetState : function(params : TownUtilParallel.NoisePollutionWidgetStateParam) : TownUtilParallel.NoisePollutionWidgetState`: Noise or pollution rating level of a town and the share of each source.
    - param `params`: Town, emission kind and line options.
    - returns: Rating level and shares.
- `getDeliveryTimeWidgetState : function(townEntity : Engine.Entity) : DeliveryTimeWidgetState`: Delivery time rating of a town and the deliveries per line.
    - param `townEntity`: Town entity.
    - returns: Rating and line statistics.
- `getRatingUrbanCare : function(param : GetRatingParam) : RatingLevel`: Level of `TownUtil.getRatingUrbanCare` for a town.
    - param `param`: Town entity and extra param.
    - returns: Rating level.
- `getRatingTraffic : function(param : GetRatingParam) : RatingLevel`: Level of `TownUtil.getRatingTraffic` for a town.
    - param `param`: Town entity and extra param.
    - returns: Rating level.
- `getRatingHappiness : function(param : GetRatingParam) : RatingLevel`: Level of `TownUtil.getRatingHappiness` for a town.
    - param `param`: Town entity and extra param.
    - returns: Rating level.
- `getRatingCargoDelivery : function(param : GetRatingParam) : RatingLevel`: Level of `TownUtil.getRatingCargoDelivery` for a town.
    - param `param`: Town entity and extra param.
    - returns: Rating level.
- `getRatingNoise : function(param : GetRatingParam) : RatingLevel`: Level of `TownUtil.getRatingNoise` for a town.
    - param `param`: Town entity and extra param.
    - returns: Rating level.
- `getRatingPollution : function(param : GetRatingParam) : RatingLevel`: Level of `TownUtil.getRatingPollution` for a town.
    - param `param`: Town entity and extra param.
    - returns: Rating level.
- `getRatingBonuses : function(param : TownUtilParallel.GetRatingParam) : RatingLevel`: Level of `TownUtil.getRatingBonuses` for a town (`Excellent` with an experience bonus, otherwise `Good`).
    - param `param`: Town entity and extra param.
    - returns: Rating level.
- `getRatingDeliveries : function(param : TownUtilParallel.GetRatingParam) : RatingLevel`: Level of `TownUtil.getRatingDeliveries` for a town.
    - param `param`: Town entity and extra param.
    - returns: Rating level.
- `getTownDistrictCountsState : function(townEntity : Engine.Entity) : TownDistrictCountsState`: Latest district counts of a town.
    - param `townEntity`: Town entity.
    - returns: Counts per district.
- `getTownLevelWidgetState : function(townEntity : Engine.Entity) : TownLevelWidgetState`: Experience, growth factor and growth level of a town.
    - param `townEntity`: Town entity.
    - returns: Town level data; experience 0, growth factor 1 and `VeryPoor` without growth state.
- `getTownCapacityUsages : function(Engine.Entity) : TownCapacityUsages`: Copy of `TownUtil.getTownCapacityUsages` as plain tables.
    - param `#1`: Town entity.
    - returns: Capacity usage per district.

### TownUtilParallel.GetCargoSupplyDemandsLandUseParam (record)

```teal
record GetCargoSupplyDemandsLandUseParam
```

Param of `getCargoSupplyDemandsLandUseWithResidentialLineUsage`.

- `townEntity : Engine.Entity`: Town entity.
- `landUse : LandUseType`: District type.

### TownUtilParallel.DeliveriesState (record)

```teal
record DeliveriesState
```

Supply ratings of a town for the supplies widgets.

- `suppliesRating : RatingLevel`: Combined supplies rating level (`TownUtil.getRatingDeliveries`).
- `passengerRating : integer`: Level number (0 to 5) of the passenger supply; `nil` without residential capacity.
- `noPassengers : boolean`: True when there is no passenger supply.
- `cargoRatingResidential : integer`: Level number (0 to 5) of the residential cargo supply; `nil` without demand.
- `noCargoResidential : boolean`: True when no residential cargo is supplied or demanded.
- `cargoRatingIndustrial : integer`: Level number (0 to 5) of the industrial cargo supply; `nil` without demand.
- `noCargoIndustrial : boolean`: True when no industrial cargo is supplied or demanded.
- `cargoRatingCommercial : integer`: Level number (0 to 5) of the commercial cargo supply; `nil` without demand.
- `noCargoCommercial : boolean`: True when no commercial cargo is supplied or demanded.

### TownUtilParallel.TrafficWidgetState (record)

```teal
record TrafficWidgetState
```

Data of the traffic widget.

- `ratingLevel : RatingLevel`: Level of the traffic rating.
- `streetsPerLevel : {integer}`: Street counts per traffic level from `TownUtil.getTrafficSpeedRating`; `{0}` without data.

### TownUtilParallel.HappinessWidgetState (record)

```teal
record HappinessWidgetState
```

Data of the happiness widget.

- `stats : UtilTown.TownHappinessStats`: Copy of the town's happiness statistics with the top `TownUtil.NumLinesForHappinessStats` lines.

### TownUtilParallel.NoisePollutionWidgetStateParam (record)

```teal
record NoisePollutionWidgetStateParam
```

Param of `getNoisePollutionWidgetState`.

- `townEntity : Engine.Entity`: Town entity.
- `pollution : boolean`: True for pollution, false for noise.
- `byLine : boolean`: Also compute the share per line.
- `maxLines : integer`: Maximum number of lines listed; the rest is summed into one entry with line `-1`.

### TownUtilParallel.NoisePollutionWidgetState (record)

```teal
record NoisePollutionWidgetState
```

Data of the noise and pollution widgets.

- `emissionRatingLevel : RatingLevel`
- `emissionPerSource : {{TownUtil.EmitterCategory, integer}}`: Pairs of source category and percentage, highest first; only entries above 0 %.
- `emissionPerLine : {{Engine.Entity, integer}}`: Pairs of line entity and percentage, highest first; line `-1` stands for all other lines. Set only when `byLine` is true.

### TownUtilParallel.DeliveryTimeWidgetState (record)

```teal
record DeliveryTimeWidgetState
```

Data of the delivery time widget.

- `cargoDeliveryRating : integer`: Delivery time rating as a percentage (0 to 100).
- `level : RatingLevel`: Level of the delivery time rating.
- `lineStats : {LineStats}`: Line statistics sorted by late items, then total; at most five entries, the fifth combines the remaining lines.

#### TownUtilParallel.DeliveryTimeWidgetState.LineStats (record)

```teal
record LineStats
```

Cargo deliveries of one line to the town in the last half game year.

- `entity : Engine.Entity`: Line entity; `-1` for the combined remaining lines.
- `cargoTypes : {CargoTypeId}`: Cargo types delivered by the line, most delivered first.
- `total : integer`: Number of items delivered.
- `late : integer`: Number of items delivered late.

### TownUtilParallel.GetRatingParam (record)

```teal
record GetRatingParam
```

Param of the parallel rating functions; the GUI builds it from `TownUtil.Rating`.

- `townEntity : Engine.Entity`: Town entity.
- `extraParam : any`: Value of `TownUtil.Rating.getRatingFnExtraParam`; the base-game functions ignore it.

### TownUtilParallel.TownDistrictCountsState (record)

```teal
record TownDistrictCountsState
```

Latest district counts of a town from its logbook.

- `numResidential : integer`: Most recent logbook value `townResidentialCount`.
- `numCommercial : integer`: Most recent logbook value `townCommercialCount`.
- `numIndustrial : integer`: Most recent logbook value `townIndustrialCount`.

### TownUtilParallel.TownLevelWidgetState (record)

```teal
record TownLevelWidgetState
```

Data of the town level widget.

- `experience : integer`: Experience of the town.
- `growthFactor : number`: Growth factor from `TownCargoUtil.getGrowthFactor`.
- `growthLevel : RatingLevel`: Growth level from `TownUtil.getGrowthLevelMult`.

### TownUtilParallel.TownCapacityUsage (record)

```teal
record TownCapacityUsage
```

- `used : integer`: Used capacity.
- `capacity : integer`: Total capacity.

### TownUtilParallel.TownCapacityUsages (record)

```teal
record TownCapacityUsages
```

Used and total capacity per district, as plain tables.

- `residential : TownCapacityUsage`: Capacity usage of the residential district.
- `commercial : TownCapacityUsage`: Capacity usage of the commercial district.
- `industrial : TownCapacityUsage`: Capacity usage of the industrial district.
