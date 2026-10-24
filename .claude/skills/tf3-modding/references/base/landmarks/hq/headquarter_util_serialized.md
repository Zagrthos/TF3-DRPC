# landmarks.hq.headquarter_util_serialized

Definition file: `base/tealdef/landmarks/hq/headquarter_util_serialized.d.tl` (in the game folder).

Types of the company headquarter construction (`/landmarks/hq/hq/headquarter.script.tl`): a modular
construction on an 8 m grid whose modules add further slots around themselves.

## HeadquarterUtilSerialized (record)

```teal
global record HeadquarterUtilSerialized
```

Types of the company headquarter construction.

### HeadquarterUtilSerialized.SlotData (record)

```teal
record SlotData
```

Size and id of one headquarter slot type.

- `id : integer`: Number of the slot type, encoded into slot ids.
- `height : number`: Slot height for collision.
- `size : Vec2Serialized.Vec2`: Slot size in metres.

### HeadquarterUtilSerialized.SlotSetup (record)

```teal
record SlotSetup
```

A slot that a placed module adds (entry of the module's `metadata.slots`).

- `slotType : string`: Type of the added slot, e.g. `"hq_medium_building"`.
- `offset : Vec2Serialized.Vec2`: Position in grid cells relative to the module, rotated with the module.
- `orientation : integer`: Rotation in quarter turns, added to the module's orientation.
- `skipOrientation : boolean`
- `heightOffset : number`: Height of the slot above the module in metres (roof flags use 15).
- `perimeter : boolean`: Adds a row of slots around the module's cell area instead of one slot at `offset`.
- `cellSize : {Vec2Serialized.Vec2}`: Cell area of the slot type for `perimeter` slots, `{xRange, yRange}`.
- `shift : Vec2Serialized.Vec2`: Offset in metres within the cell.
- `variant : integer`: Variant encoded into the slot id.

### HeadquarterUtilSerialized.HeadquarterConnector (record)

```teal
record HeadquarterConnector
```

Extra models a module adds when a neighbouring cell holds a matching slot type.

- `slotTypes : {string}`: Slot types of the neighbour that trigger the connector; `"*any"` matches any occupied cell and
`"*empty"` an empty one. The first matching connector of a group wins.
- `group : string`: Name of the generated data group to add.
- `offset : Vec2iSerialized.Vec2i`: Neighbour cell relative to the module, rotated with the module.

### HeadquarterUtilSerialized.HeadquarterModuleMetadata (record)

```teal
record HeadquarterModuleMetadata
```

Metadata of a headquarter module.

- `cellSize : {Vec2Serialized.Vec2}`: Cells the module occupies, `{{x = minX, y = maxX}, {x = minY, y = maxY}}` relative to its own cell.
- `fenceCellSize : {Vec2Serialized.Vec2}`: Cells enclosed by the module's fence, in the same form; defaults to `cellSize` grown by one cell.
- `fenceEntrance : boolean`: Leaves the front side of the fence open.
- `slots : {SlotSetup}`: Slots the module adds around itself.
- `size : Vec2iSerialized.Vec2i`: Size of the module's own slot in metres.
- `connectors : {string : {HeadquarterConnector}}`: Connector groups (e.g. `left`, `right`, `top`, `bottom`); each group adds at most one connector.
- `canRotate : boolean`: Rotates the module's models by its variant in quarter turns.

### HeadquarterUtilSerialized.HeadquarterConstructionResult (record)

```teal
record HeadquarterConstructionResult is ConstructionUtilSerialized.IConstructionResult
```

Construction result of the headquarter with the grid helpers its module scripts call.

- `slotIds : {integer : integer}`: Index into `slots` by slot id.
- `cells : {number : {number : integer}}`: Occupied grid cells, `cells[x][y]` = slot id, or -1 for fence cells.
- `runLaterFns : {function()}`: Functions registered with `runLater`.
- `encodeSlotId : function(coord : Vec2Serialized.Vec2, slotType : string, orientation : integer) : integer`: Packs grid coordinate, slot type, orientation and variant into one slot id.
- `decodeSlotId : function(slotId : integer) : Vec2Serialized.Vec2, string, integer, integer`: Unpacks a slot id.
    - returns: Grid coordinate, slot type, orientation (0-3) and variant.
- `isOccupied : function(coord : Vec2Serialized.Vec2) : integer`: Slot id occupying the grid cell, -1 for fence cells, or nil if the cell is free.
- `markOccupied : function(coord : Vec2Serialized.Vec2, slotId : integer)`: Marks a grid cell as occupied by the slot id.
- `addSlot : function(coord : Vec2Serialized.Vec2, type : string, sizex : integer, sizey : integer, slotHeight : number, orientation : integer, heightOffset? : Vec3Serialized.Vec3, variant? : integer)`: Adds a slot at a grid cell (8 m grid), unless a slot with the same id exists or the cell is more than 20 cells from the origin.
- `occupySlot : function(cellSize : {Vec2Serialized.Vec2}, fenceCellSize : {Vec2Serialized.Vec2}, coord : Vec2Serialized.Vec2, orientation : integer, slotId : integer)`: Marks the cells of a module and its fence as occupied.
- `makeSlots : function(cellSize : {SlotSetup}, cellSize : {Vec2Serialized.Vec2}, coord : Vec2Serialized.Vec2, orientation : integer, variant? : integer)`: Adds the slots of a module's `metadata.slots` around the module.
- `addAlignmentColliderAndGroundTexture : function(cellSize : {Vec2Serialized.Vec2}, fenceCellSize : {Vec2Serialized.Vec2}, transf2 : Mat4Serialized.Mat4)`: Adds a flat terrain alignment and a box collider (30 m high) over the module's fence area.
- `addFence : function(cellSize : {Vec2Serialized.Vec2}, fenceCellSize : {Vec2Serialized.Vec2}, coord : Vec2Serialized.Vec2, tag : string, transf2 : Mat4Serialized.Mat4, orientation : integer, skipFront : boolean)`: Adds fence and fence post models around the module's fence area.
- `rotate : function(coord : Vec2Serialized.Vec2, orientation : integer) : Vec2Serialized.Vec2`: Rotates a grid offset by `orientation` quarter turns counter-clockwise.
- `runLater : function(function())`: Registers a function that runs after all module scripts, when the construction script calls `terminateConstructionHook`.
    - notes: Module scripts use it to add models only once every module has added its slots and occupied its cells.
