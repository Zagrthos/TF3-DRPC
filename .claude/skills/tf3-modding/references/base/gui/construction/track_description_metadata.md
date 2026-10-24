# gui.construction.track_description_metadata

Definition file: `base/tealdef/gui/construction/track_description_metadata.d.tl` (in the game folder).

Metadata accessor (`track_description`) that attaches the speed and lane count of a street or track type to its construction menu entry.

## TrackDescriptionMetadata (record)

```teal
global record TrackDescriptionMetadata
```
Load: `local track_description_metadata = ug_require "::/gui/construction/track_description_metadata.tl" as TrackDescriptionMetadata`

Metadata accessor that stores a street or track type's speed and lane count in the metadata of its construction menu entry.

Notes:

The construction menu writes it with `constructionDesc.make{speed = ...}` for street and track
definitions; `ConstructionDescReactUtil.collectNumbers` reads it back to show the top speed
(`speedLimit`) and number of lanes.

- `constructionDesc : MetadataAccessor<ConstructionDescMetadata, TrackDetails>`: Accessor registered as `track_description` on construction description metadata (`get` reads, `make` creates the entry).
    - used 3x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_react_util.tl:582`: `local trackDescriptionMeta = track_description_metadata.constructionDesc.get(metadata)`

### TrackDescriptionMetadata.TrackDetails (record)

```teal
record TrackDetails
```

Data stored by the `track_description` metadata.

- `speed : number`: Speed of the first lane of the street or track type; shown as "Top Speed" in the info panel.
- `numLanes : integer`: Number of lanes.
