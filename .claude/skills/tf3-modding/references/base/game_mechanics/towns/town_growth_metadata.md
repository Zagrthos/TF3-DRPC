# game_mechanics.towns.town_growth_metadata

Definition file: `base/tealdef/game_mechanics/towns/town_growth_metadata.d.tl` (in the game folder).

Construction metadata (`town_growth`) for town bonuses of landmarks, and the town level-up notification records.

## TownGrowthMetadata (record)

```teal
global record TownGrowthMetadata
```
Load: `local town_growth_metadata = ug_require "::/game_mechanics/towns/town_growth_metadata.tl" as TownGrowthMetadata`

Accessors for the `town_growth` construction metadata, which gives the closest town a bonus while the construction stands.

Example:
```lua
-- base/content/landmarks/hq/hq/headquarter.con.lua
metadata = {
    town_growth = {
        xpIncrease = 0.05,
    },
},
```

- `constructionDesc : MetadataAccessor<ConstructionDescMetadata, TownGrowthMetadata.ConstructionDesc>`: Accessor for `TownGrowthMetadata.ConstructionDesc` in construction definition metadata.
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_react_util.tl:578`: `local townGrowthMeta = town_growth_metadata.constructionDesc.get(metadata)`
- `constructionInstance : MetadataAccessor<ConstructionInstanceMetadata, TownGrowthMetadata.ConstructionInstance>`: Accessor for `TownGrowthMetadata.ConstructionInstance` in a construction's persistent metadata.
    - used 3x in the base game, e.g. `base/content/landmarks/hq/hq/headquarter.script.tl:512`: `town_growth_metadata.constructionInstance.set(metadata, {`
- `getKey : function() : string`  (call as `town_growth_metadata.getKey(…)`): Metadata key of town bonuses.
    - returns: `"town_growth"`
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/towns.script.tl:197`: `town_growth_metadata.getKey(), false, true, function(constructionEntity : Engine.Entity, construction : Engine.Component.Construction, __ : ConstructionId)`

### TownGrowthMetadata.ConstructionDesc (record)

```teal
record ConstructionDesc
```

Town bonuses in a construction definition's metadata. The construction menu shows them.

- `xpIncrease : number`: Added to the town's experience multiplier; `0.05` means 5 % more experience.
- `trafficRatingIncrease : number`: Added to the town's traffic rating (0 to 1) before clamping.
- `reputationRecoveryBoost : number`: Passed as `bonus` to `TownUtil.applyEventDecay`.

### TownGrowthMetadata.ConstructionInstance (record)

```teal
record ConstructionInstance
```

Town bonuses in the persistent metadata of a built construction. `landmark_util.collectTownGrowthMetadata` sums them per closest town.

- `xpIncrease : number`: Added to the town's experience multiplier.
- `trafficRatingIncrease : number`: Added to the town's traffic rating before clamping.
- `reputationRecoveryBoost : number`: Passed as `bonus` to `TownUtil.applyEventDecay`.

## TownLevelNotificationInput (record)

```teal
global record TownLevelNotificationInput
```

Params of the town level-up notification (`town_notification.script`).

In game: "Town Level Up", "Thanks to the excellent care, {town} was able to grow steadily and reached a new level: {townType}." (German: „Stadtlevel gestiegen“, „Dank der ausgezeichneten Versorgung konnte {town} stetig wachsen und erreichte einen neue Stadtstufe: {townType}.“)

- `townEntity : EntityUtil.EntityAndRevision`: Town that reached the new level.
- `level : integer`: New town level; `TownUtil.level2name` gives its name.

## TownLevelNotificationSimInput (record)

```teal
global record TownLevelNotificationSimInput
```

Data the notification's sim update script fills in.

- `townName : string`: Current name of the town.
