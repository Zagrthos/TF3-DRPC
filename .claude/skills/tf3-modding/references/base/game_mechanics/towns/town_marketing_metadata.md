# game_mechanics.towns.town_marketing_metadata

Definition file: `base/tealdef/game_mechanics/towns/town_marketing_metadata.d.tl` (in the game folder).

Construction metadata (`town_marketing`) of marketing campaigns.

## TownMarketingMetadata (record)

```teal
global record TownMarketingMetadata
```
Load: `local town_marketing_metadata = ug_require "::/game_mechanics/towns/town_marketing_metadata.tl" as TownMarketingMetadata`

Accessor for the `town_marketing` construction metadata of marketing campaigns. The marketing campaign tool reads it and sends it with the `startMarketingCampaign` event.

- `constructionDesc : MetadataAccessor<ConstructionDescMetadata, TownMarketingMetadata.ConstructionDesc>`: Accessor for `TownMarketingMetadata.ConstructionDesc` in construction definition metadata.
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_react_util.tl:586`: `local townMarketingMeta = town_marketing_metadata.constructionDesc.get(metadata)`
- `getKey : function() : string`  (call as `town_marketing_metadata.getKey(…)`): Metadata key of marketing campaigns.
    - returns: `"town_marketing"`

### TownMarketingMetadata.ConstructionDesc (record)

```teal
record ConstructionDesc
```

Settings of one marketing campaign.

- `durationMs : integer`: Duration of the campaign in ms of game time.
- `lineCostFactor : number`: Line cost factor of the town in pathfinding while the campaign runs (`OnModifyPathfinding`). Values below `0.1` are reset to `0.1`.
