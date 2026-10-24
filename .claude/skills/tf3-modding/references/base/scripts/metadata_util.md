# scripts.metadata_util

Definition file: `base/tealdef/scripts/metadata_util.d.tl` (in the game folder).

Types of `/scripts/metadata_util.tl` (`MetadataUtil`): typed accessors for the free-form `metadata`
tables of construction descriptions, construction instances, subconstruction instances and savegame
scripts. Each feature registers its own key and reads/writes only that key.

## Globals

- `ConstructionDescMetadata` (type): Type of the `metadata` table of a construction description. The API declares its `emissionConfig` entry (`noise`, `pollution`); other entries are read and written through the `MetadataUtil` accessors.
- `ConstructionInstanceMetadata` (type): Opaque type of the metadata stored with a built construction.
- `SubconstructionInstanceMetadata` (type): Opaque type of the metadata stored with a subconstruction instance.
- `SavegameScriptMetadata` (type): Opaque type of the metadata a game script stores in the savegame (returned from the `storeSavegameMetadata` event).

## MetadataAccessor (record)

```teal
global record MetadataAccessor<C, T>
```

Typed access to one key of a metadata table, obtained from `MetadataTypeUtil.register(key)`.
`C` is the kind of metadata table, `T` the type stored under the key.

- `set : function(container : C, value : T)`: Stores `value` under this accessor's key in the metadata table `container`.
- `get : function(container : C) : T`: Reads this accessor's key from the metadata table `container`.
    - returns: The stored value, or nil if `container` is nil or has no value under the key.
- `make : function(value : T) : function(C)`: creates a function that will insert into the container

## MetadataUtil (record)

```teal
global record MetadataUtil
```
Load: `local metadata_util = ug_require "::/scripts/metadata_util.tl" as MetadataUtil`

Accessor factories for the four kinds of metadata tables; returned by `ug_require "/scripts/metadata_util.tl"`.

- `constructionDesc : MetadataTypeUtil<ConstructionDescMetadata>`: Accessor factory for construction description metadata.
    - used 19x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_metadata.tl:8`: `constructionDesc = metadata_util.constructionDesc.register(getKey()) as MetadataAccessor<ConstructionDescMetadata, CompanyMetadata.ConstructionDesc>,`
- `constructionInstance : MetadataTypeUtil<ConstructionInstanceMetadata>`: Accessor factory for construction instance metadata.
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_metadata.tl:9`: `constructionInstance = metadata_util.constructionInstance.register(getKey()) as MetadataAccessor<ConstructionInstanceMetadata, CompanyMetadata.ConstructionInsta`
- `subconstructionInstance : MetadataTypeUtil<SubconstructionInstanceMetadata>`: Accessor factory for subconstruction instance metadata.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/emission/emission_config_metadata.tl:5`: `subconstructionInstance = metadata_util.subconstructionInstance.register("emissions") as MetadataAccessor<SubconstructionInstanceMetadata, {EmissionConfigMeta}>`
- `savegameScript : MetadataTypeUtil<SavegameScriptMetadata>`: Accessor factory for savegame script metadata.
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_growth.script.tl:139`: `return metadata_util.savegameScript.make({company_metadata.savegameScript.make(metadata)})`

### MetadataUtil.MetadataTypeUtil (record)

```teal
record MetadataTypeUtil<C>
```

Accessor factory for one kind of metadata table `C`.

- `register : function<T>(key : string) : MetadataAccessor<C, T>`: Creates an accessor that reads and writes the given key of metadata tables of this kind.
    - param `key`: The key under which the feature stores its data, for example `"emissionConfig"`.
    - returns: An accessor with `get`, `set` and `make`.
- `make : function(uses : {function(C)}) : C`: Builds a new metadata table by calling each function (usually from `accessor.make(value)`) on it.
    - param `uses`: Functions that each insert one value into the table.
    - returns: The filled metadata table.
