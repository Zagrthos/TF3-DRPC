# scripts.construction.construction_specialization_metadata

Definition file: `base/tealdef/scripts/construction/construction_specialization_metadata.d.tl` (in the game folder).

Types of `/scripts/construction/construction_specialization_metadata.tl`: metadata accessors for a
construction's `constructionSpecialization` and a module's `specialization` entry.

## ConstructionSpecializationMetadata (record)

```teal
global record ConstructionSpecializationMetadata
```
Load: `local construction_specialization_metadata = ug_require "::/scripts/construction/construction_specialization_metadata.tl" as ConstructionSpecializationMetadata`

Metadata accessors for construction and module specializations.

- `constructionDesc : MetadataAccessor<ConstructionDescMetadata, ConstructionSpecializationMetadata.SpecializationConfig>`: Accessor for the `constructionSpecialization` key of a construction description's metadata.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3767`: `local constructionSpecializationAccessor = construction_specialization_metadata.constructionDesc.get(definition.metadata)`
- `getKey : function() : string`  (call as `construction_specialization_metadata.getKey(…)`): Returns the construction metadata key, `"constructionSpecialization"`.
- `moduleDesc : MetadataAccessor<ConstructionDescMetadata, string>`: Accessor for the `specialization` key (a string) of a module's metadata.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:3770`: `local moduleSpecialization = construction_specialization_metadata.moduleDesc.get(definition.metadata)`
- `getModuleKey : function() : string`  (call as `construction_specialization_metadata.getModuleKey(…)`): Module metadata key, `"specialization"`.
    - notes: Declared in the definition, but the base game's implementation table does not export it; use `moduleDesc` instead.

### ConstructionSpecializationMetadata.SpecializationConfig (record)

```teal
record SpecializationConfig
```

The `constructionSpecialization` entry of a construction description's metadata.

- `specialization : string`: Name of the specialization.
