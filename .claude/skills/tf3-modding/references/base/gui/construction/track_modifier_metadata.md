# gui.construction.track_modifier_metadata

Definition file: `base/tealdef/gui/construction/track_modifier_metadata.d.tl` (in the game folder).

Metadata accessor (`track_modifier`) that attaches the noise, pollution and speed modifiers of a street type or edge decoration to its construction menu entry.

## TrackModifierMetadata (record)

```teal
global record TrackModifierMetadata
```

Metadata accessor that stores noise, pollution and speed modifiers in the metadata of a construction menu entry.

Notes:

Written for street types (their modifiers plus those of their default edge decorations) and for
edge decorations; `ConstructionDescReactUtil.collectNumbers` turns `noiseModifier`,
`pollutionModifier` and `maxSpeedModifier` into effects shown in the info panel.

- `constructionDesc : MetadataAccessor<ConstructionDescMetadata, Modifiers>`: Accessor registered as `track_modifier` on construction description metadata.
