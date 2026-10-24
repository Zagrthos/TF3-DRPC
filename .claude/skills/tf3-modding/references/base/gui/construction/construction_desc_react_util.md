# gui.construction.construction_desc_react_util

Definition file: `base/tealdef/gui/construction/construction_desc_react_util.d.tl` (in the game folder).

Helper module for the info panel of construction menu entries
(`::/gui/construction/construction_desc_react_util.tl`): collects the numbers (cost, maintenance,
emissions, effects, permits) from a definition and its metadata, formats them for display, and
has the default HUD icon configurations used by `configureHudIconsScript` scripts.

## ConstructionDescReactUtil (record)

```teal
global record ConstructionDescReactUtil
```
Load: `local construction_desc_react_util = ug_require "::/gui/construction/construction_desc_react_util.tl" as ConstructionDescReactUtil`

Helper module for the info panel of construction menu entries. Load it with `ug_require "::/gui/construction/construction_desc_react_util.tl"`.

- `getAttributesFromConstructionResult : function(updateFnResult1 : ConstructionResult) : {string : {number, number, {string}}}`  (call as `construction_desc_react_util.getAttributesFromConstructionResult(…)`): Extracts the info panel attributes from the result of a construction's update function.
    - returns: Attributes by key (`cost`, `maintenanceCost`, `noise`, `pollution`, `radius`, `pollutionRadius`, `capacityPassenger`, `comfortFactor`, `maintenanceCapacity`, ...), each `{min, max}` with `max = -1` for a single value.
    - notes: The maintenance cost is scaled by the base config's `infrastructureMaintenanceScale`.

cache may be nil. Is only required for getting the number available permits.
- `collectNumbers : function(metadata : ConstructionDescMetadata, definition : ConstructionDefinition, cache : ConstructionReactUtil.ItemDisableCache, attributes : {string : {number, number, {string}}}) : Numbers`  (call as `construction_desc_react_util.collectNumbers(…)`)
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_react_util.tl:70`: `newPerk.numbers = construction_desc_react_util.collectNumbers(perk.metadata, perk, nil, nil)`
- `formatNumbers : function(numbers : Numbers) : {{FormatNumberDesc}}`  (call as `construction_desc_react_util.formatNumbers(…)`): Formats collected numbers for the info panel.
  In game: "Unique Use", "Uses: {numAvailable}", "Cooldown: {cooldown}" (German: „Einzigartige Nutzung“, „Nutzungen: {numAvailable}“, „Abklingzeit: {cooldown}“)
    - returns: Lines of formatted numbers; each line is placed by the `layoutType` of its first item.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:125`: `and construction_desc_react_util.formatNumbers(currentPerk.numbers)`
- `ConstructionInfoNumberList : Recipe<ConstructionInfoNumberListParam>`: Recipe that shows one line of formatted numbers (icon and text per item).
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:138`: `boxLayoutChildren[#boxLayoutChildren + 1] = construction_desc_react_util.ConstructionInfoNumberList {`
- `getDepotHudIconsConfig : function(capturedParams : ConstructionDescReactUtil.DepotHudIconsConfigParams) : ConstructionActionHudIcons`  (call as `construction_desc_react_util.getDepotHudIconsConfig(…)`): HUD icons for placing a depot (stations, depots, signals for rail, and the maintenance station pool), filtered by carrier.
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_hud_icons.script.tl:8`: `return construction_desc_react_util.getDepotHudIconsConfig(capturedParams)`
- `getStationHudIconsConfig : function(capturedParams : ConstructionDescReactUtil.StationHudIconsConfigParams) : ConstructionActionHudIcons`  (call as `construction_desc_react_util.getStationHudIconsConfig(…)`): HUD icons for placing a station (stations, towns, industries, plus signals or warehouses), with districts shown.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_hud_icons.script.tl:5`: `return construction_desc_react_util.getStationHudIconsConfig(capturedParams)`
- `getStreetBuilderHudIconsConfig : function() : ConstructionActionHudIcons`  (call as `construction_desc_react_util.getStreetBuilderHudIconsConfig(…)`): HUD icons for the street builder (nodes, industries, towns, stations, road depots, warehouses).
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_hud_icons.script.tl:32`: `return construction_desc_react_util.getStreetBuilderHudIconsConfig()`
- `getTrackBuilderHudIconsConfig : function() : ConstructionActionHudIcons`  (call as `construction_desc_react_util.getTrackBuilderHudIconsConfig(…)`): HUD icons for the track builder (industries, towns, signals, stations, rail depots, warehouses).
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_hud_icons.script.tl:35`: `return construction_desc_react_util.getTrackBuilderHudIconsConfig()`
- `getLandmarkConstructionHudIconsConfig : function() : ConstructionActionHudIcons`  (call as `construction_desc_react_util.getLandmarkConstructionHudIconsConfig(…)`): HUD icons for placing a landmark (towns and landmark constructions, with `showPerksAndTowns`).
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_hud_icons.script.tl:38`: `return construction_desc_react_util.getLandmarkConstructionHudIconsConfig()`
- `getDefaultHudIconsConfig : function() : ConstructionActionHudIcons`  (call as `construction_desc_react_util.getDefaultHudIconsConfig(…)`): Default HUD icons (towns, with town borders).
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_hud_icons.script.tl:41`: `return construction_desc_react_util.getDefaultHudIconsConfig()`
- `formatEffectPercentage : function(value : number) : string`  (call as `construction_desc_react_util.formatEffectPercentage(…)`): Formats an effect factor as a percentage change (factor minus 1, for example `1.2` as +20 %).
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1324`: `local percentageStr = construction_desc_react_util.formatEffectPercentage(effectModifier.value)`
- `vehicleEffectMoreIsBetter : function(effectType : string) : boolean`  (call as `construction_desc_react_util.vehicleEffectMoreIsBetter(…)`): Tells whether a higher value of a vehicle effect is an improvement.
    - returns: `true` for `topSpeed` and `comfort`, `false` for `noise` and `pollution`; logs an error and returns `true` for other types.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1325`: `local isPositive = effectModifier.value > 1 == construction_desc_react_util.vehicleEffectMoreIsBetter(effect)`
- `getVehicleEffectName : function(effect : string, carrier : string) : string`  (call as `construction_desc_react_util.getVehicleEffectName(…)`): Returns the translated label of a vehicle effect for a carrier, for example "Train Top Speed".
  In game: "Road Vehicle Top Speed", "Train Noise", "Ship Comfort" (German: „Strassenfahrzeug-Höchstgeschwindigkeit“, „Zug-Lärm“, „Schiff-Komfort“)
    - param `effect`: Effect type: `topSpeed`, `noise`, `pollution` or `comfort`.
    - param `carrier`: Carrier (`ROAD`, `RAIL`, `AIR`, `WATER`, `TRAM`); other values give the generic "Vehicle ..." label.
    - returns: The label, or `nil` for an unknown effect.
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:1381`: `text = construction_desc_react_util.getVehicleEffectName(row.effect, row.carrier),`

### ConstructionDescReactUtil.CargoAndAmount (record)

```teal
record CargoAndAmount
```

A cargo type and amount (for example a landmark's resource requirement).

- `cargoType : integer`: Cargo type id.
- `amount : integer`: Amount; `-1` where only the cargo type matters.

### ConstructionDescReactUtil.MinMax (record)

```teal
record MinMax
```

A value range for the info panel.

- `min : number`: Lower value (or the single value).
- `max : number`: Upper value; `-1` for cost values marks a single value.
- `range : number`: Radius of a noise or pollution emission.

### ConstructionDescReactUtil.Effect (record)

```teal
record Effect
```

An effect shown in the info panel.

- `type : string`: Effect type, for example `topSpeed`, `noise`, `pollution`, `comfort`, `marketing`, `greenifyNoise` or `greenifyPollution`.
- `value : number`: Effect factor (`1` is no change; shown as a percentage), or the duration in milliseconds for `marketing`.
- `carrier : string`: Carrier the vehicle effect applies to (`ROAD`, `RAIL`, `AIR`, `WATER`, `TRAM`); other values give a generic vehicle label.

### ConstructionDescReactUtil.Numbers (record)

```teal
record Numbers
```

Key numbers of a menu entry collected by `collectNumbers` and formatted by `formatNumbers`.

- `companyRank : MinMax`
- `xpIncrease : MinMax`: Town growth increase (from town growth metadata).
- `trafficRatingIncrease : MinMax`: Traffic rating increase (from town growth metadata).
- `reputationRecoveryBoost : MinMax`: Reputation recovery boost (from town growth metadata).
- `noise : MinMax`: Noise emission; negative values are shown as noise reduction.
- `pollution : MinMax`: Pollution emission; negative values are shown as pollution cleaning.
- `noiseModifier : MinMax`
- `pollutionModifier : MinMax`
- `maxSpeedModifier : MinMax`
- `speedLimit : MinMax`: Top speed (from track description metadata or the `speedLimit` attribute).
- `numLanes : MinMax`: Number of lanes (from track description metadata).
- `numAvailable : integer`: Remaining uses of the company permit; needs the cache.
- `capacityPassenger : MinMax`: Passenger capacity.
- `comfortFactor : MinMax`: Comfort bonus.
- `effects : {Effect}`: Effects such as vehicle speed, noise or pollution modifiers.
- `cost : MinMax`: Construction cost in game currency.
- `maintenanceCost : MinMax`: Maintenance cost in game currency per year.
- `maintenanceCapacity : MinMax`: Maintenance capacity.
- `costPerKm : boolean`: Shows costs and emissions per km or mile (from `attributesCostPerKm`).
- `cargoCosts : {ConstructionDescReactUtil.CargoAndAmount}`: Cargo required to build (landmark requirements, scaled by the `landmarkResources` option).
- `depotCapacity : MinMax`
- `permitKey : ResName`: Company permit key of the entry.
- `perkCategory : CompanyMetadata.ConstructionDesc.PerkCategory`: Perk category of the entry.
- `cooldownInSeconds : number`: Cooldown of the permit in seconds.

### ConstructionDescReactUtil.LayoutType (enum)

```teal
enum LayoutType
```

Where a formatted number is placed in the info panel.

- `"TopLeft"`: Top left of the preview image (used for negative values such as emissions).
- `"TopRight"`: Top right of the preview image (used for positive values such as top speed).
- `"BottomLeft"`: Bottom left of the preview image (durations, cooldowns, remaining uses).
- `"BottomRight"`: Bottom right of the preview image.
- `"BelowImageLeft"`: Below the image, left, in the title font (cost, cargo costs).
- `"BelowImageRight"`: Below the image, right (maintenance cost).

### ConstructionDescReactUtil.FormatNumberDesc (record)

```teal
record FormatNumberDesc
```

A formatted number for the info panel.

- `icon : string`: Icon shown with the text.
- `text : string`: Text to show.
- `singleLine : boolean`: Puts the item on a line of its own.
- `attributeBubbleStyle : string`: Extra style class of the attribute bubble (for example `noise`, `noise-good`, `pollution`, `pollution-good`).
- `textFirst : boolean`: Shows the text before the icon.
- `layoutType : LayoutType`: Placement in the info panel.

### ConstructionDescReactUtil.ConstructionInfoNumberListParam (record)

```teal
record ConstructionInfoNumberListParam
```

Parameters of `ConstructionInfoNumberList`.

- `line : {FormatNumberDesc}`: Formatted numbers to show.
- `vertical : boolean`: Stacks the items vertically instead of a flow layout.
- `useTitleFont : boolean`: Uses the headline font instead of the body font.

### ConstructionDescReactUtil.DepotHudIconsConfigParams (record)

```teal
record DepotHudIconsConfigParams
```

Parameters of `getDepotHudIconsConfig`.

- `depotType : ConstructionActionHudIconConstructionType`: Carrier of the depot.

### ConstructionDescReactUtil.StationHudIconsConfigParams (record)

```teal
record StationHudIconsConfigParams
```

Parameters of `getStationHudIconsConfig`.

- `cargo : boolean`: Cargo station; adds warehouse icons.
- `rail : boolean`: Rail station; adds signal icons.
