# stations.station_metadata

Definition file: `base/tealdef/stations/station_metadata.d.tl` (in the game folder).

Types of `/stations/station_metadata.tl` (`StationMetadata`), accessors for station module metadata.

## MoreCapacityMetadata (record)

```teal
global record MoreCapacityMetadata
```

Extra capacity a station module adds (the `moreCapacity` metadata key).

- `passenger : integer`: Additional passenger capacity.

## StationMetadata (record)

```teal
global record StationMetadata
```

Accessors returned by `ug_require "/stations/station_metadata.tl"`.

- `moreCapacity : MetadataAccessor<ConstructionDescMetadata, MoreCapacityMetadata>`: Accessor for the `moreCapacity` key of a construction description's metadata.
- `comfortFactor : MetadataAccessor<ConstructionDescMetadata, number>`: Accessor for the `comfortFactor` key; module comfort factors are summed for the station.
