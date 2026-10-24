# game_mechanics.emission.emission_metadata

Definition file: `base/tealdef/game_mechanics/emission/emission_metadata.d.tl` (in the game folder).

Noise and pollution emitted by constructions (`metadata.emission`), and the emission config.

## EmissionDb (record)

```teal
global record EmissionDb
```

Emission of a construction (`metadata.emission`); industries list one entry per production level.

- `noise : number`: in db (decibel)
- `pollution : number`: Pollution; negative values reduce pollution (pollution cleanup facility).
- `radius : number`: Noise radius in metres.
- `pollutionRadius : number`: Pollution radius in metres; defaults to `radius`.

## EmissionMetadata (record)

```teal
global record EmissionMetadata
```
Load: `local emission_metadata = ug_require "::/game_mechanics/emission/emission_metadata.tl" as EmissionMetadata`

Accessors for emission metadata.

- `constructionDesc : MetadataAccessor<ConstructionDescMetadata, EmissionDb>`: Accessor for the `emission` metadata of construction resources.
    - used 5x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_react_util.tl:579`: `local emissionMeta = emission_metadata.constructionDesc.get(metadata)`
- `subconstructionInstance : MetadataAccessor<SubconstructionInstanceMetadata, {EmissionDb}>`: Accessor for the `emissions` metadata of subconstruction instances.

## EmissionConfig (record)

```teal
global record EmissionConfig
```
Load: `local emission_config = ug_require "::/game_mechanics/emission/emission_config.tl" as EmissionConfig`

Emission config (`emission/emission_config.tl`), used for the layers and the noise and pollution town ratings.

- `noiseLowerBoundDB : number`: Lowest noise level of the noise layer, in dB (63).
    - used 3x in the base game, e.g. `base/content/base/base/mod.script.tl:177`: `baseConfig.emission.noiseLowerBoundDB = emissions.noiseLowerBoundDB as integer`
- `noiseUpperBoundDB : number`: Highest noise level of the noise layer, in dB (85).
    - used 5x in the base game, e.g. `base/content/base/base/mod.script.tl:178`: `baseConfig.emission.noiseUpperBoundDB = emissions.noiseUpperBoundDB as integer`
- `noiseNumSteps : integer`: Number of steps of the noise layer (12).
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game_tooltips.tl:250`: `numberOfLevels = emission_config.noiseNumSteps`
- `pollutionLowerBoundDB : number`: Lowest value of the pollution layer (57).
    - used 3x in the base game, e.g. `base/content/base/base/mod.script.tl:179`: `baseConfig.emission.pollutionLowerBoundDB = emissions.pollutionLowerBoundDB as integer`
- `pollutionUpperBoundDB : number`: Highest value of the pollution layer (80).
    - used 5x in the base game, e.g. `base/content/base/base/mod.script.tl:180`: `baseConfig.emission.pollutionUpperBoundDB = emissions.pollutionUpperBoundDB as integer`
- `pollutionNumSteps : integer`: Number of steps of the pollution layer (12).
    - used 7x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:26`: `averageMapColors.numberOfLevels = emission_config.pollutionNumSteps`
- `noiseRatingLowerBoundDB : number`: Average noise at town buildings that maps to the best noise rating (65 dB).
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1800`: `local noisePercentage = mathutil.mapClamp(noiseAndCapacity[1], emission_config.noiseRatingLowerBoundDB, emission_config.noiseRatingUpperBoundDB, 0, 1)`
- `noiseRatingUpperBoundDB : number`: Average noise that maps to the worst noise rating (80 dB).
    - used 4x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1800`: `local noisePercentage = mathutil.mapClamp(noiseAndCapacity[1], emission_config.noiseRatingLowerBoundDB, emission_config.noiseRatingUpperBoundDB, 0, 1)`
- `noiseRatingRandomOffsetMinMax : {number, number}`: Range of a random offset added to the noise rating (`{0, 0}` in the base game).
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:174`: `result.noise  = mathutil.lerp(emission_config.noiseRatingRandomOffsetMinMax[1], emission_config.noiseRatingRandomOffsetMinMax[2], noiseRand)`
- `pollutionRatingLowerBoundDB : number`: Average pollution in the town area that maps to the best rating (60).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:430`: `return mathutil.mapClamp(pollutionDB, emission_config.pollutionRatingLowerBoundDB, emission_config.pollutionRatingUpperBoundDB, 1, 0)`
- `pollutionRatingUpperBoundDB : number`: Average pollution that maps to the worst rating (74).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:430`: `return mathutil.mapClamp(pollutionDB, emission_config.pollutionRatingLowerBoundDB, emission_config.pollutionRatingUpperBoundDB, 1, 0)`
- `pollutionRatingRandomOffsetMinMax : {number, number}`: Range of a random offset added to the pollution rating (`{0, 0}` in the base game).
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:171`: `result.pollution = mathutil.lerp(emission_config.pollutionRatingRandomOffsetMinMax[1], emission_config.pollutionRatingRandomOffsetMinMax[2], pollutionRand)`
- `pollutionRatingAreaMinSquareKm : number`: Minimum sampled area of a town for the pollution rating, in km² (5).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:421`: `emission_config.pollutionRatingAreaMinSquareKm, emission_config.pollutionRatingAreaMaxSquareKm)`
- `pollutionRatingAreaMaxSquareKm : number`: Maximum sampled area, in km² (10).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:421`: `emission_config.pollutionRatingAreaMinSquareKm, emission_config.pollutionRatingAreaMaxSquareKm)`
- `emissionReductionPerEcoLevel : number`: Emission reduction per eco level of an industry (0.8, so eco level 1 cuts emissions by 80 %).
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_react_util.tl:725`: `value = 1 - emission_config.emissionReductionPerEcoLevel,`
