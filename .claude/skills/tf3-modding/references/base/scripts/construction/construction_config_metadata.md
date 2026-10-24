# scripts.construction.construction_config_metadata

Definition file: `base/tealdef/scripts/construction/construction_config_metadata.d.tl` (in the game folder).

Types of `/scripts/construction/construction_config_metadata.tl`: the `constructionConfig` metadata of
constructions that are built up in levels by delivering cargo (the landmarks), with their requirements,
stocks and the effects they grant when finished.

## ConstructionConfigMetadata (record)

```teal
global record ConstructionConfigMetadata
```
Load: `local construction_config_metadata = ug_require "::/scripts/construction/construction_config_metadata.tl" as ConstructionConfigMetadata`

Metadata accessors for the `constructionConfig` key (see `getKey`).

- `constructionDesc : MetadataAccessor<ConstructionDescMetadata, ConstructionConfigMetadata.ConstructionConfig>`: Accessor for the configuration in a construction description's metadata.
    - used 4x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_react_util.tl:585`: `local constructionConfig = construction_config_metadata.constructionDesc.get(metadata)`
- `constructionInstance : MetadataAccessor<ConstructionInstanceMetadata, ConstructionConfigMetadata.ConstructionConfig>`: Accessor for the configuration in a built construction's persistent metadata.
    - used 9x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:523`: `local constructionData = construction_config_metadata.constructionInstance.get(con.persistentMetadata)`
- `getKey : function() : string`  (call as `construction_config_metadata.getKey(…)`): Returns the metadata key, `"constructionConfig"`.

    ```lua
    -- from base/content/landmarks/landmarks.script.tl
    api.engine.system.streetConnectorSystem.forEachConstructionWithMetadata(construction_config_metadata.getKey(), false, true,
        function(entity : Engine.Entity, con : Engine.Component.Construction, __ : ConstructionId)
            local constructionData = construction_config_metadata.constructionInstance.get(con.persistentMetadata)
        end)
    ```

    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/notifications.script.tl:520`: `api.engine.system.streetConnectorSystem.forEachConstructionWithMetadata(construction_config_metadata.getKey(), false, true,`

### ConstructionConfigMetadata.ConstructionConfig (record)

```teal
record ConstructionConfig
```

Level-up configuration of a construction that is completed by delivering cargo (landmarks). It is
stored in the construction description's metadata and copied to the built instance.

- `level : integer`: Current construction level, starting at 1.
- `numLevels : integer`: Number of levels; the construction is finished when `level` reaches `numLevels`.
- `underConstructionMessage : string`: Short message shown in the notification while the construction is not finished.
- `underConstructionDescription : string`: Longer description of the construction site shown to the player.
- `effects : {Effect}`: Bonuses granted when finished.
- `requirements : {Requirement}`: Cargo deliveries needed; the level rises with the delivered share of the total.
- `stocks : {Stock}`: Stocks of the construction site.
- `constructionFinishedSound : ResName`: Sound played with the notification when the construction is finished.

#### ConstructionConfigMetadata.ConstructionConfig.Requirement (record)

```teal
record Requirement
```

Cargo that must be delivered to finish the construction.

- `cargoType : string`: Cargo type resource, e.g. `"::/cargos/stone/stone.cargo"`.
- `amount : integer`: Amount needed; scaled by the game's `landmarkResources` advanced option.

#### ConstructionConfigMetadata.ConstructionConfig.Stock (record)

```teal
record Stock
```

A stock of the construction site.

- `type : string`: Stock type, e.g. `"INPUT_STOCK"`.
- `cargoType : ResName`: Cargo type resource stored in the stock.
- `capacity : integer`: Stock capacity.

#### ConstructionConfigMetadata.ConstructionConfig.Effect (record)

```teal
record Effect
```

Bonus the finished construction grants to its closest town.

- `carrier : string`: Carrier the bonus applies to, e.g. `"ROAD"`.
- `type : string`: Kind of bonus, e.g. `"comfort"`.
- `value : number`: Bonus value; values of several landmarks for the same town, carrier and type are added up.
