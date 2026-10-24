# scripts.construction.construction_util_serialized

Definition file: `base/tealdef/scripts/construction/construction_util_serialized.d.tl` (in the game folder).

Plain lua table type definitions for construction resource scripts.
These types describe the format of lua tables produced by construction scripts
and read by C++ via converters. They must NOT reference C++ userdata types.

## ConstructionUtilSerialized (record)

```teal
global record ConstructionUtilSerialized
```

Plain-table types of what construction scripts (`updateFn`, module `updateFn`, `getModelsFn`,
`createTemplateFn`) return; the engine converts these tables. The legacy helper
`/scripts/construction/constructionutil.lua` is loaded with this type to get `addModelsAndGroups`.

- `GeneratedData : {string : GeneratedDataGroup}`: Generated data by group name (e.g. `"static"`, `"base"`, `"variant_0"`).
    - used 4x in the base game, e.g. `base/content/landmarks/hq/hq/headquarter_addon.script.tl:57`: `local generatedData = captureParams.generatedData as ConstructionUtilSerialized.GeneratedData`
- `ConstructionTemplateResult : {integer : string}`: Result of a `createTemplateFn`: module file per slot id, i.e. the modules a new construction starts with.
The headquarter uses `{ [rootSlotId] = "::/landmarks/hq/modules/hq_main.module" }`.
    - used 2x in the base game, e.g. `base/content/landmarks/hq/hq/headquarter.script.tl:125`: `function hq.createTemplateFn(captureParams : table, params : table) : ConstructionUtilSerialized.ConstructionTemplateResult`
- `ModelsFnData : {TransformedModel}`: Return value of a module's `getModelsFn`, the module's models with their transforms.
    - used 4x in the base game, e.g. `base/content/landmarks/hq/hq/headquarter_addon.script.tl:100`: `function data.getModelsFn(captureParams : table, _params : ConstructionUtilSerialized.ModuleGetModelsFnParams) : ConstructionUtilSerialized.ModelsFnData`
- `addModelsAndGroups : function(params : any, group : GeneratedDataGroup, subconstruction : SubConstruction, tag? : string, transf2? : Mat4Serialized.Mat4)`: Adds everything a generated data group describes to a subconstruction: its models, random models from
asset groups, ground faces, terrain alignment and colliders.
    - param `params`: The construction parameters; `params.seed` is required (it seeds the random group choices).
    - param `group`: The generated data group.
    - param `subconstruction`: Where models, ground faces, alignment lists and colliders are appended.
    - param `tag`: Optional tag set on the added models.
    - param `transf2`: Optional transform applied to everything that is added.
    - notes: Implemented in `/scripts/construction/constructionutil.lua`; load that file with `ug_require "::/scripts/construction/constructionutil.lua" as ConstructionUtilSerialized`. Asset groups are looked up first in `params.state.groups`, then in `AssetUtil.assets`; a missing group logs a warning and uses a placeholder model.

    ```lua
    -- from base/content/landmarks/landmark_util_serialized.tl
    if generatedData[dataLevel] ~= nil then
        constructionutil.addModelsAndGroups(params, generatedData[dataLevel], subconstruction, nil, transform)
    end
    ```

### ConstructionUtilSerialized.ScriptRef (record)

```teal
record ScriptRef
```

Reference to a script function as `"file.script@function"` plus the parameters passed to it.

- `fileName : string`: Script function reference, e.g. `"small_stops.script@updateFn"`.
- `params : table`: Table passed to the function as its first argument (`captureParams`).

### ConstructionUtilSerialized.TransportMode (enum)

```teal
enum TransportMode
```

Transport mode names used in lane lists, depots, snap points and terminals; the same set as `api.type.TransportMode`.

- `"PERSON"`: Pedestrians.
- `"CARGO"`: Cargo items moving on foot paths of stations and industries.
- `"CAR"`: Private cars.
- `"BUS"`: Buses.
- `"TRUCK"`: Trucks.
- `"TRAM"`: Trams.
- `"ELECTRIC_TRAM"`: Electric trams.
- `"TRAIN"`: Trains.
- `"ELECTRIC_TRAIN"`: Electric trains.
- `"AIRCRAFT"`: Large aircraft.
- `"SHIP"`: Large ships.
- `"SMALL_AIRCRAFT"`: Small aircraft.
- `"SMALL_SHIP"`: Small ships.
- `"HELICOPTER"`: Helicopters.
- `"TRAM_TRACK"`: Tram track.
- `"ELECTRIC_TRAM_TRACK"`: Electrified tram track.

### ConstructionUtilSerialized.BaseEdgeType (enum)

```teal
enum BaseEdgeType
```

Kind of an edge list's edges.

- `"NORMAL"`: Ordinary edge on the ground.
- `"BRIDGE"`: Bridge; the bridge type goes into `edgeTypeName`.
- `"TUNNEL"`: Tunnel; the tunnel type goes into `edgeTypeName`.

### ConstructionUtilSerialized.CatchmentArea (record)

```teal
record CatchmentArea
```

Cylinder with a radius around a position; it is the area a station, stop or depot covers.

- `position : Vec3Serialized.Vec3`: Centre, in construction-local coordinates (the base game writes `{0, 0, 0}`).
- `radius : number`: Radius in metres.

### ConstructionUtilSerialized.CargoTypeSet (record)

```teal
record CargoTypeSet
```

Selects cargo types by class and by type, with include and exclude lists.

- `cargoClassesIncluded : {string}`: Cargo classes included, e.g. `"PASSENGERS"`, `"UNIVERSAL"`.
- `cargoClassesExcluded : {string}`: Cargo classes excluded.
- `cargoTypesIncluded : {string}`: Individual cargo types included.
- `cargoTypesExcluded : {string}`: Individual cargo types excluded.

### ConstructionUtilSerialized.SlotConfig (record)

```teal
record SlotConfig
```

Extra settings for all slots of one slot type (key of `slotConfig`).

- `maxModules : integer`: Maximum number of modules of this type in the whole construction. `-1` means no limit. With `-2` the module button is enabled while `message` is empty and disabled when a message is set (used by `airport.script.lua` for terminal B).
- `message : string`: Message for when the limit is reached.
- `skipCollisionCheck : boolean`: Slots of this type do not collide with other objects (only the slot itself; the finished
construction still has to pass the normal collision checks).

### ConstructionUtilSerialized.StreetTerminal (record)

```teal
record StreetTerminal
```

Terminal of a street-side stop (an edge object such as the small bus and truck stops).

- `capacity : integer`: Number of passengers or cargo units that can wait.
- `cargo : boolean`: Whether the stop handles cargo.
- `passengers : boolean`: Whether the stop handles passengers.
- `cargoLoad : boolean`: Whether cargo can be loaded.
- `passengersLoad : boolean`: Whether passengers can board.
- `cargoUnload : boolean`: Whether cargo can be unloaded.
- `passengersUnload : boolean`: Whether passengers can alight.
- `comfortFactor : number`: How quickly the happiness of waiting passengers decreases.
- `wantSymmetric : boolean`: True for stops on both sides of the street (the small stops set it from their `twoSided` parameter).
- `catchmentArea : CatchmentArea`: Catchment area of the stop.
- `cargoTransferSpeeds : {TerminalDef.CargoTransferSpeed}`: Loading speed settings per cargo type set.

### ConstructionUtilSerialized.GeneratedDataGroup (record)

```teal
record GeneratedDataGroup
```

One group of data exported from the model editor (the `generatedData` tables in construction scripts):
named curves and model placements.

- `GeneratedCurve : {string : {{{number}}}}`: Named curves: each key maps to a list of curves, each curve a list of `{x, y, z}` points. Special
names are interpreted by `addModelsAndGroups`, e.g. `collider`, keys containing `.gtex` (ground faces)
and `equal_faces|low|high` (terrain alignment).
- `curves : GeneratedCurve`: Named curves of the group.
- `models : {string : {Mat4Serialized.Mat4}}`: Model placements; each key is a model file and maps to the list of its transforms.

### ConstructionUtilSerialized.ModuleUpdateFnParams (record)

```teal
record ModuleUpdateFnParams
```

Parameters passed to a modular construction's update script and to each module's update script.

- `seed : integer`: Random seed; while a module script runs it is the construction's seed plus the slot id.
- `modules : {integer : Module}`: Placed modules by slot id; modules are processed in ascending slot id order.

#### ConstructionUtilSerialized.ModuleUpdateFnParams.Module (record)

```teal
record Module
```

One module placed in a slot.

- `updateScript : ScriptRef`: The module's update script; called with its `params`, the construction result, the slot transform, a tag, the slot id, an add-model function and these parameters.
- `metadata : any`: The module's metadata, e.g. `price`, `maintenanceCost`, `bulldozePrice`.
- `name : string`: The module's resource name.
- `variant : integer`: Changed with M/N while placing the module. Can be negative, default 0. The headquarter uses it as a rotation in quarter turns.

### ConstructionUtilSerialized.ModuleGetModelsFnParams (record)

```teal
record ModuleGetModelsFnParams
```

Parameters of a module's `getModelsFn` (no fields are defined).

### ConstructionUtilSerialized.TransformedModel (record)

```teal
record TransformedModel
```

A model placed by a construction.

- `id : string`: Model file, e.g. `"station/rail/platform.mdl"`.
- `transf : Mat4Serialized.Mat4`: Placement transform (16 numbers, column-major) relative to the construction.
- `tag : string`: Tag of the model; models added by a module get `"__module_<slotId>"`, which links them to that module.

### ConstructionUtilSerialized.GroundFace (record)

```teal
record GroundFace
```

A ground texture face (`face` points and texture `modes`); no fields are declared in the definition.

### ConstructionUtilSerialized.TerrainAlignmentList (record)

```teal
record TerrainAlignmentList
```

A set of faces to which the terrain is adapted.

- `type : AlignmentType`: Alignment type.
- `faces : {{{number, number, number}}}`: Polygons, each a list of `{x, y, z}` points.
- `slopeLow : number`: Embankment slope at the face border (the base game uses e.g. 0.275).
- `slopeHigh : number`: Maximum embankment slope (the base game uses e.g. 0.6).

#### ConstructionUtilSerialized.TerrainAlignmentList.AlignmentType (enum)

```teal
enum AlignmentType
```

How the terrain is adapted to the faces.

- `"EQUAL"`: Terrain is set to the height of the faces.
- `"LESS"`: Terrain is lowered where it is higher than the faces.
- `"GREATER"`: Terrain is raised where it is lower than the faces.

### ConstructionUtilSerialized.Slot (record)

```teal
record Slot
```

A slot of a modular construction, where modules of the same `type` can be placed.

- `id : integer`: Unique slot id; modules in `params.modules` are keyed by it. Can encode the slot's coordinates.
- `transf : Mat4Serialized.Mat4`: Transform relative to the construction origin; module models are placed relative to it.
- `type : string`: Slot type; only modules of the same type can be built here.
- `spacing : {number, number, number, number}`: Extent of the slot around its origin, `{left, right, south, north}`: distances along -X, +X, -Y and +Y
in slot coordinates.
- `height : number`: Height of the slot for collision.
- `shape : integer`: 0 = square, 1 = triangle, 2 = transverse rectangle, 3 = longitudinal rectangle.
- `autoFill : boolean`: Fills the slot automatically when the construction spawns (fields use it to add their satellites).
    - notes: The shipped code (`fieldutil.lua`) and the wiki write `autofill` in lower case.
- `alignToTerrain : boolean`: Moves the slot vertically to the terrain height (used e.g. for fields).
- `replaceable : boolean`: Modules in this slot can be replaced by other modules of the same type without bulldozing first.

### ConstructionUtilSerialized.Depot (record)

```teal
record Depot
```

Depot part of a subconstruction.

- `catchmentArea : CatchmentArea`: Area around the depot in which vehicles are maintained.
- `transportModes : {TransportMode}`: Transport modes of the vehicles the depot handles.
- `maintenancePool : integer`: Number of vehicles that can be maintained at the same time.

### ConstructionUtilSerialized.Industry (record)

```teal
record Industry
```

Industry part of a subconstruction.

- `productionLevel : integer`: Current production level.
- `maxLevel : integer`: Highest production level.

### ConstructionUtilSerialized.EdgeListType (enum)

```teal
enum EdgeListType
```

Network type of an edge list.

- `"STREET"`: Street edges.
- `"TRACK"`: Track edges.

### ConstructionUtilSerialized.EdgeList (record)

```teal
record EdgeList
```

Street or track edges a construction creates in the transport network.

- `edgeType : BaseEdgeType`: Normal, bridge or tunnel.
- `edgeTypeName : string`: Bridge or tunnel type used when `edgeType` is `BRIDGE` or `TUNNEL`.
- `type : EdgeListType`: Street or track.
- `edges : {{Vec3Serialized.Vec3, Vec3Serialized.Vec3}}`: Nodes as `{position, tangent}` pairs; every two consecutive entries form one edge. Build them with
`StreetUtilSerialized.addStraightEdge`, `addEdge` and friends.
- `snapNodes : {integer}`: 0-based indices of the nodes in `edges` that snap to existing streets or tracks.
- `freeNodes : {integer}`: 0-based indices of the nodes in `edges` that are not frozen to the construction (see `StreetUtilSerialized.freeAllNodes`).
- `trafficLightYes : {integer}`
- `trafficLightNo : {integer}`
- `tag2nodes : {string : {integer}}`: Named groups of node indices (0-based) for later reference.
- `alignTerrain : boolean`

### ConstructionUtilSerialized.LaneList (record)

```teal
record LaneList
```

Lanes inside a construction (paths for persons, cargo and vehicles that are not streets or tracks).

- `transf : Mat4Serialized.Mat4`: Transform applied to the lane nodes.
- `transportModes : {TransportMode}`: Transport modes allowed on the lanes.
- `speedLimit : number`: Speed limit of the lanes, in m/s (per the official wiki).
- `nodes : {{Vec3Serialized.Vec3, Vec3Serialized.Vec3, number}}`: Nodes as `{position, tangent, width}`; every two consecutive entries form one lane edge (see `LaneUtilSerialized.createLane`).
- `linkable : boolean`: Whether the automatically generated short footpath links can connect to these lanes.
- `pedestrianWalkPenalty : number`: Penalty for pedestrians walking on these lanes.
- `forwardOnly : boolean`: Lanes may only be travelled in forward direction.
- `needsReservation : boolean`: Lanes need reservation before vehicles may enter.

### ConstructionUtilSerialized.RunwayDef (record)

```teal
record RunwayDef
```

A landing or take-off path for aircraft and ships, defined on lane nodes.

- `type : string`: `"LANDING"` or `"TAKEOFF"`.
- `node : {number, number}`: Runway node as `{laneListIndex, nodeIndex}` (both 0-based).
- `edges : {{number, number}}`: Runway edges as `{laneListIndex, edgeIndex}` pairs (0-based).

### ConstructionUtilSerialized.TerminalDef (record)

```teal
record TerminalDef
```

A station terminal, defined on the lanes of its subconstruction.

- `comfortFactor : number`: How quickly the happiness of waiting passengers decreases.
- `personEdges : {{integer, integer}}`: Edges of the waiting area as `{laneListIndex, edgeIndex}` pairs (0-based).
- `capacity : integer`: How many passengers or cargo units fit the terminal.
- `cargoTransferSpeeds : {CargoTransferSpeed}`: Loading speed settings.

#### ConstructionUtilSerialized.TerminalDef.CargoTransferSpeed (record)

```teal
record CargoTransferSpeed
```

Loading speed setting for a set of cargo types.

- `cargoTypeSet : CargoTypeSet`: Cargo types the setting applies to.
- `loadSpeedModifier : number`: Factor on the loading speed.
- `length : number`: Length of the terminal section.

### ConstructionUtilSerialized.StationDef (record)

```teal
record StationDef
```

The station of a subconstruction.

- `pool : Pool`: Shared waiting area.
- `terminals : {TerminalDef}`: The station's terminals.

#### ConstructionUtilSerialized.StationDef.Pool (record)

```teal
record Pool
```

Shared waiting area that takes overflowing terminal capacity.

- `capacity : integer`: Size of the shared pool.
- `edges : {{integer, integer}}`: Edges of the shared waiting area as `{laneListIndex, edgeIndex}` pairs.

### ConstructionUtilSerialized.StockRules (record)

```teal
record StockRules
```

A production rule of an industry-like subconstruction.

- `input : {{integer}}`: orRule to stock to amount
- `output : {string : integer}`: Produced amounts by output stock.
- `capacity : integer`: Production capacity of the rule.

### ConstructionUtilSerialized.Stocks (record)

```teal
record Stocks
```

A cargo stock of a subconstruction.

- `type : StockType`: Role of the stock.
- `cargoType : string`: Cargo type resource stored.
- `capacity : integer`: Capacity of the stock.

#### ConstructionUtilSerialized.Stocks.StockType (enum)

```teal
enum StockType
```

Role of the stock.

- `"INPUT_STOCK"`: Receives delivered cargo.
- `"OUTPUT_STOCK"`: Holds produced cargo for pickup.
- `"STORAGE_STOCK"`: Stores cargo.

### ConstructionUtilSerialized.SubConstruction (record)

```teal
record SubConstruction
```

One part of a construction result: its models, ground faces, colliders, terrain alignment, lanes and
optionally a station, depot or industry. Most constructions use a single subconstruction
(`result.subconstructions[1]`).

- `models : {TransformedModel}`: Placed models.
- `groundFaces : {GroundFace}`: Ground texture faces.
- `colliders : {ColliderUtilSerialized.Collider}`: Collision shapes.
- `depot : Depot`: Depot data, if the subconstruction is a depot.
- `terrainAlignmentLists : {TerrainAlignmentList}`: Terrain alignment.
- `laneLists : {LaneList}`: Lanes; terminals and runways refer to them by 0-based index.
- `runways : {RunwayDef}`: Landing and take-off paths.
- `station : StationDef`: Station data, if the subconstruction is a station.
- `industry : Industry`: Industry data, if the subconstruction is an industry.
- `rules : {StockRules}`: Production rules.
- `stocks : {Stocks}`: Cargo stocks.
- `tag : integer`: Tag identifying the subconstruction (e.g. between upgrades).
- `metadata : table`: Metadata stored with the subconstruction instance.

### ConstructionUtilSerialized.SnapPoint (record)

```teal
record SnapPoint
```

Where and to what the construction snaps while it is being placed.

- `snapToBaseEdgeTypes : {EdgeListType}`: Network types to snap to, e.g. `{"STREET"}`.
- `allowSnapToBaseEdgeEnds : boolean`: Also allow snapping to the end of a street or track.
- `alignWithCoast : boolean`: Align the construction with the coastline (harbours, ship depots).
- `placeAtTerrainHeight : boolean`
- `transf : Mat4Serialized.Mat4`: Snap point transform relative to the construction.
- `transportModes : {TransportMode}`: Transport modes the snap point applies to.

### ConstructionUtilSerialized.IConstructionResult (interface)

```teal
interface IConstructionResult
```

What a construction's `updateFn` returns.

Notes:

Costs are in game currency. If `maintenanceCost` is nil or 0, the construction script sets it to
`cost / 6`; `cost` and `bulldozeCost` are scaled by the game's infrastructure cost scale.

Example:
```lua
-- from base/content/stations/street/street/underground_station/underground_station.script.lua
result.snapPoint = {
    transf = transf.transl(vec3.new(0, 8.0, 0)),
    snapToBaseEdgeTypes = {"STREET"},
    allowSnapToBaseEdgeEnds = true,
}
```

- `subconstructions : {SubConstruction}`: The construction's parts.
- `metadata : table`: Metadata stored with the built construction (its persistent metadata).
- `snapPoint : SnapPoint`: Snap point used while placing the construction.
- `slots : {Slot}`: Module slots of a modular construction.
- `slotConfig : {string : SlotConfig}`: Extra settings per slot type.
- `cost : number`: Build cost in game currency; module prices are added to it.
- `maintenanceCost : number`: Maintenance cost per year in game currency; defaults to `cost / 6`.
- `streetTerminal : StreetTerminal`: Terminal data for street-side stops.
- `bulldozeCost : number`: Cost of bulldozing the construction, in game currency.
- `noCostAtAll : boolean`: disable all cost for the entire proposal

Only used within constructWithModules
- `terminateConstructionHook : function()`
- `callInvalidModules : boolean`: Also run the update scripts of modules whose slot id is not in `slots`.

### ConstructionUtilSerialized.ConstructionResult (record)

```teal
record ConstructionResult is IConstructionResult
```

Concrete construction result type (see `IConstructionResult`).
