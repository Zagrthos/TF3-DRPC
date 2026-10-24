# game_mechanics.emission.emission_config_metadata

Definition file: `base/tealdef/game_mechanics/emission/emission_config_metadata.d.tl` (in the game folder).

Noise and pollution figures of station modules shown in the construction menu (`metadata.emissionConfig`).

## EmissionConfigMeta (record)

```teal
global record EmissionConfigMeta
```

The `metadata.emissionConfig` table of a construction or module, shown in its description.

Example:
```lua
-- base game: stations/street/street/modular_street_station/passenger_platform.module.lua
metadata = {
    emissionConfig = {
        noise = 1,
        pollution = 5,
    },
},
```

- `noise : number`: Noise figure.
- `pollution : number`: Pollution figure.

## EmissionConfigMetadata (record)

```teal
global record EmissionConfigMetadata
```
Load: `local emission_config_metadata = ug_require "::/game_mechanics/emission/emission_config_metadata.tl" as EmissionConfigMetadata`

Accessors for `emissionConfig` metadata.

- `constructionDesc : MetadataAccessor<ConstructionDescMetadata, EmissionConfigMeta>`: Accessor for the `emissionConfig` metadata of construction resources.
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_react_util.tl:580`: `local emissionConfigMeta = emission_config_metadata.constructionDesc.get(metadata)`
- `subconstructionInstance : MetadataAccessor<SubconstructionInstanceMetadata, {EmissionConfigMeta}>`: Accessor for the `emissions` metadata of subconstruction instances.
