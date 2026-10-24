# climates.gen.map_gen_util_serialized

Definition file: `base/tealdef/climates/gen/map_gen_util_serialized.d.tl` (in the game folder).

Types of the map generator's river and ridge helpers, `/climates/gen/mapgenutil.tl` (`MapGenUtil`)
and `/climates/gen/rivermapgenutil.tl` (`RiverMapGenUtil`). The map editor's node editor
(`gui/node_editor/layer_nodes.script.tl`) uses them to place rivers, lakes and mountain ridges.
Coordinates are in metres.

## Globals

- `RiverId : integer`: Number identifying a generated river.

## MapBounds (record)

```teal
global record MapBounds
```

Rectangle of the map area, in metres.

- `min : Vec2Serialized.Vec2`: Lower-left corner.
- `max : Vec2Serialized.Vec2`: Upper-right corner.

## RiverStartConfig (record)

```teal
global record RiverStartConfig
```

Settings for `MapGenUtil.findGoodRiverStarts`.

- `getStartWeight : function(Vec2Serialized.Vec2) : number`: Weight of a position; positions with a negative weight are river sources.

## RiverStartResult (record)

```teal
global record RiverStartResult
```

A river start point.

- `pos : Vec2Serialized.Vec2`: Start position.
- `angle : number`: Start direction in radians.
- `weight : number`: Start weight at the position.

## RiverData (record)

```teal
global record RiverData
```

A sampled river position in the collision grid.

- `riverId : integer`: River the sample belongs to.
- `position : Vec2Serialized.Vec2`: Sample position.

## RiverMap (record)

```teal
global record RiverMap
```

Collision grid of river samples, used to keep rivers apart.

- `riverMap : {{RiverData}}`: Samples per grid block (800 m blocks).

## RiverBuilderConfig (record)

```teal
global record RiverBuilderConfig
```

Settings for `MapGenUtil.makeRivers`.

- `curvature : number`: Random direction change per segment (default 0.2).
- `map : RiverMap`: Collision grid; created if nil.
- `getRiverWeight : function(Vec2Serialized.Vec2) : number`: Weight of a position; each step picks the direction with the highest weight below 1.2.
- `depthScale : number`: Factor on the river depth.
- `segmentLength : number`: Length of a river segment in metres; derived from the remaining length if nil.
- `minLength : number`: Rivers shorter than this (in metres) are discarded.
- `bounds : MapBounds`: Map bounds; rivers stop 5 km outside them.
- `maxOrder : integer`: Highest branching order of tributaries.
- `baseProbability : number`: Factor on the probability of tributaries per segment.
- `minDist : number`: Minimum number of segments between two tributaries.
- `currentRiverId : RiverId`: Id of the river being built; incremented for every river.

## RiverResult (record)

```teal
global record RiverResult
```

A generated river with values per point.

- `points : {Vec2Serialized.Vec2}`: Point positions.
- `widths : {Vec2Serialized.Vec2}`: Bank widths per point (`x` left, `y` right).
- `depths : {number}`: Depth per point.
- `tangents : {Vec2Serialized.Vec2}`: Curve tangent per point.
- `widthTangents : {Vec2Serialized.Vec2}`: Width tangent per point.
- `depthTangents : {number}`: Depth tangent per point.
- `features : {Feature}`: Flags per point.
- `feeders : {integer}`: Index of the tributary that joins at a point (-1 or nil for none).

### RiverResult.Feature (record)

```teal
record Feature
```

Flags of one river point.

- `curve : boolean`: The point was added as a meander.
- `lake : boolean`: The point is part of a lake.
- `lakeDist : number`: Distance in points to the nearest lake or tributary.

## RidgeResult (record)

```teal
global record RidgeResult
```

A generated mountain ridge.

- `points : {Vec2Serialized.Vec2}`: Ridge points.
- `heights : {number}`: Height per point.
- `directions : {number}`: Direction per point in radians.

## RidgesBuilderConfig (record)

```teal
global record RidgesBuilderConfig
```

Settings for `MapGenUtil.makeRidges`.

- `bounds : MapBounds`: Map bounds.
- `density : number`: Ridges per 2048 m tile; if nil, 0 to 2 are chosen with `probabilityLow` and `probabilityHigh`.
- `minHeight : number`: Lowest peak height.
- `maxHeight : number`: Highest ridge height.
- `probabilityLow : number`: A random number above this gives no ridge in a tile.
- `probabilityHigh : number`: A random number above this (and not above `probabilityLow`) gives one ridge, otherwise two.
- `ridgeMask : function(Vec2f) : number`: Mask value of a position; ridges stop where it is below 0.5.

## RidgesResult (record)

```teal
global record RidgesResult
```

A generated ridge with points as `{x, y}` pairs.

- `points : {{number, number}}`: Ridge points.
- `heights : {number}`: Height per point.
- `directions : {number}`: Direction per point in radians.

## SubdivideConfig (record)

```teal
global record SubdivideConfig
```

Settings for `RiverMapGenUtil.subdivide`.

- `nSegments : integer`: Number of segments each river segment is split into.

## LakeConfig (record)

```teal
global record LakeConfig
```

Settings for `RiverMapGenUtil.makeLakes`.

- `probability : function(Vec2Serialized.Vec2) : number`: Lake probability at a position.
- `startProbability : function(Vec2Serialized.Vec2) : number`: A lake starts where `probability` exceeds this value.
- `endProbability : function(number, Vec2Serialized.Vec2) : number`: A lake ends where `1 - probability` exceeds this value; receives the current lake size in points.
- `makeProfile : function(number) : Vec2Serialized.Vec2, number`: Width and depth of a lake point; the argument runs from -1 at the lake start to 1 at its end.

## RidgesConfig (record)

```teal
global record RidgesConfig
```

Settings of `MapGenUtil.makeRidges` (see `RidgesBuilderConfig`).

- `bounds : MapBounds`: Map bounds.
- `probabilityLow : number`: A random number above this gives no ridge in a tile.
- `probabilityHigh : number`: A random number above this (and not above `probabilityLow`) gives one ridge, otherwise two.
- `minHeight : number`: Lowest peak height.
- `maxHeight : number`: Highest ridge height.
- `ridgeMask : function(pos : Vec2f) : number`: Mask value of a position; ridges stop where it is below 0.5.

## RiverResult2 (record)

```teal
global record RiverResult2
```

A river in the format the terrain generator takes, with `{x, y}` pairs instead of vectors.

- `points : {{number, number}}`: Point positions.
- `tangents : {{number, number}}`: Curve tangent per point.
- `widthTangents : {{number, number}}`: Width tangent per point.
- `widths : {{number, number}}`: Bank widths per point.
- `depths : {number}`: Depth per point.
- `depthTangents : {number}`: Depth tangent per point.

## MapGenUtil (record)

```teal
global record MapGenUtil
```

River and ridge generation, returned by `ug_require "/climates/gen/mapgenutil.tl"`.

- `findGoodRiverStartBounds : function(bounds : MapBounds) : RiverStartResult`: Random start point 100 m outside a random edge of the bounds, pointing to the centre.
- `findGoodRiverStarts : function(bounds : MapBounds, startConfig : RiverStartConfig, attempts : integer) : {RiverStartResult}`: Samples random positions and keeps those with a negative start weight; the direction points to the highest weight 200 m away (8 directions tried).
    - param `attempts`: Number of sampled positions.
    - returns: The found start points.
- `makeRivers : function(valleys : {RiverResult}, config : RiverBuilderConfig, startLength : number, startPosition : Vec2Serialized.Vec2, startDirection : number, order? : integer, feederId? : integer, feederDir? : number, effluentIndex? : integer, i? : integer) : number`: Builds a river from a start point and recursively its tributaries, and appends them to `valleys`.
    - param `valleys`: List the rivers are appended to.
    - param `config`: River settings; also holds the collision grid and the current river id.
    - param `startLength`: Length budget in metres; the node editor uses 120000.
    - returns: Total river volume, or nil if the river was discarded (blocked by another river or too short).

    ```lua
    -- from base/content/gui/gui/node_editor/layer_nodes.script.tl
    local river = map_gen_util.makeRivers(rivers, riverConfig, 120000, start.pos, start.angle)
    ```

- `makeRidges : function(RidgesConfig) : {RidgeResult}`: Places random mountain ridges in 2048 m tiles over the bounds.
- `convertRiver : function(valleys : {RiverResult}) : {RiverResult2}`: Converts rivers to `RiverResult2`, with vectors replaced by `{x, y}` pairs.

## RiverMapGenUtil (record)

```teal
global record RiverMapGenUtil
```

River post-processing, returned by `ug_require "/climates/gen/rivermapgenutil.tl"`.

- `makeSubdivision : function(river : RiverResult, subdivideConfig : SubdivideConfig) : RiverResult`: Returns a copy of the river with every segment split into `nSegments` along the Hermite curve.
- `makeLake : function(river : RiverResult, lakeConfig : LakeConfig, prepare? : boolean)`: Marks stretches of the river as lakes with the probabilities from `lakeConfig` and sets their width and depth from `makeProfile`.
    - param `prepare`: If true, only marks the points without changing width and depth.
- `makeCurve : function(river : RiverResult, curveConfig : CurveConfig)`: Bends the tangents of points away from lakes and tributaries to form meanders, and scales widths with `getWidthMultiplier`.
- `adjustFeeder : function(river : RiverResult, rivers : {RiverId : RiverResult}, _adjustConfig : AdjustFeederConfig)`: Sets the depth of each tributary at its junction point to the depth of the receiving river.
- `subdivide : function(rivers : {RiverResult}, subdivideConfig : SubdivideConfig)`: Applies `makeSubdivision` to every river.
- `makeLakes : function(rivers : {RiverResult}, lakeConfig : LakeConfig)`: Applies `makeLake` to every river.
- `makeCurves : function(rivers : {RiverResult}, curveConfig : CurveConfig)`: Applies `makeCurve` to every river.
- `adjustFeeders : function(rivers : {RiverResult}, adjustConfig : AdjustFeederConfig)`: Applies `adjustFeeder` to every river.

## CurveConfig (record)

```teal
global record CurveConfig
```

Settings for `RiverMapGenUtil.makeCurves`.

- `baseStrength : number`: Base strength of the meanders.
- `getCurviness : function(Vec2Serialized.Vec2) : number`: Meander strength factor at a position.
- `newRiver : boolean`: Use the newer meander method that rotates tangents.
- `getWidthMultiplier : function(Vec2Serialized.Vec2) : number`: Factor on the river width at a position.

## AdjustFeederConfig (record)

```teal
global record AdjustFeederConfig
```

Settings for `RiverMapGenUtil.adjustFeeders` (no fields are used).
