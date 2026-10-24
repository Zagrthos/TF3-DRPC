# game_mechanics.company.prospection_metadata

Definition file: `base/tealdef/game_mechanics/company/prospection_metadata.d.tl` (in the game folder).

Company metadata used when a construction is placed with the prospection tool.

## ProspectionMetadata (record)

```teal
global record ProspectionMetadata
```

Accessor for the `prospection` metadata of construction resources.

- `constructionDesc : MetadataAccessor<ConstructionDescMetadata, ProspectionMetadata.ConstructionDesc>`: Accessor for `ConstructionDesc`.

### ProspectionMetadata.ConstructionDesc (record)

```teal
record ConstructionDesc
```

Prospection metadata of a construction resource.

- `company : CompanyMetadata.ConstructionDesc`: configuration when constructing this via the prospection tool (independent of when building e.g. via sandbox)
