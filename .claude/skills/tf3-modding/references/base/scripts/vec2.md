# scripts.vec2

Definition file: `base/tealdef/scripts/vec2.d.tl` (in the game folder).

Types of the plain-Lua 2D vector library `/scripts/vec2.tl` (`Vec2Serialized`). Unlike the engine's
`api.type.Vec2f`, these vectors are ordinary Lua tables `{x = ..., y = ...}` with arithmetic
metamethods, so they can be stored in script state and construction results.
Load it with `local vec2 = ug_require "/scripts/vec2.tl" as Vec2Serialized`.

## Vec2iSerialized (record)

```teal
global record Vec2iSerialized
```

Namespace of the integer 2D vector type `Vec2i`.

### Vec2iSerialized.Vec2i (record)

```teal
record Vec2i
```

2D vector with integer components, as a plain table `{x = ..., y = ...}`.

- `x : integer`: X component.
- `y : integer`: Y component.

## Vec2Serialized (record)

```teal
global record Vec2Serialized
```
Load: `local vec2 = ug_require "::/scripts/vec2.tl" as Vec2Serialized`

The 2D vector library returned by `ug_require "/scripts/vec2.tl"`, and the namespace of its `Vec2` type.

- `new : function(x : number, y : number) : Vec2`  (call as `vec2.new(…)`): Creates a 2D vector with arithmetic operators attached.
    - param `x`: X component.
    - param `y`: Y component.
    - returns: The new vector.
    - used 26x in the base game, e.g. `base/content/climates/climates/gen/mapgenutil.tl:11`: `local newStart = vec2.new(math_util.randf(bounds.min.x, bounds.max.x), math_util.randf(bounds.min.y, bounds.max.y))`
- `add : function(v1 : Vec2, v2 : Vec2) : Vec2`  (call as `vec2.add(…)`): Returns `v1 + v2`.
    - used 12x in the base game, e.g. `base/content/climates/climates/gen/mapgenutil.tl:22`: `local secondPos = vec2.add(newStart, vec2.mul(samplePointDistance, dirVec))`
- `sub : function(v1 : Vec2, v2 : Vec2) : Vec2`  (call as `vec2.sub(…)`): Returns `v1 - v2`.
    - used 5x in the base game, e.g. `base/content/climates/climates/gen/mapgenutil.tl:52`: `local dist = vec2.sub(center, start.pos )`
- `mul : function(f : number, v : Vec2) : Vec2`  (call as `vec2.mul(…)`): Returns the vector `v` scaled by the number `f`.
    - used 14x in the base game, e.g. `base/content/climates/climates/gen/mapgenutil.tl:22`: `local secondPos = vec2.add(newStart, vec2.mul(samplePointDistance, dirVec))`
- `div : function(v : Vec2, f : number) : Vec2`  (call as `vec2.div(…)`): Returns the vector `v` with each component divided by `f`.
    - used 5x in the base game, e.g. `base/content/climates/climates/gen/mapgenutil.tl:341`: `local subdivVec = vec2.div(segmentVec, subdiv)`
- `componentwiseMul : function(v1 : Vec2, v2 : Vec2) : Vec2`  (call as `vec2.componentwiseMul(…)`): Returns `{x = v1.x * v2.x, y = v1.y * v2.y}`.
    - notes: The result is a plain table without the operator metatable; wrap it in `new` if you need `+`/`-`.
- `componentwiseDiv : function(v1 : Vec2, v2 : Vec2) : Vec2`  (call as `vec2.componentwiseDiv(…)`): Returns `{x = v1.x / v2.x, y = v1.y / v2.y}`.
    - notes: The result is a plain table without the operator metatable; wrap it in `new` if you need `+`/`-`.
- `length : function(v : Vec2) : number`  (call as `vec2.length(…)`): Get the length (magnitude) of the vector.
    - param `v`: vector
    - returns: length of the vector
    - used 2x in the base game, e.g. `base/content/climates/climates/gen/mapgenutil.tl:151`: `table.insert(valley.tangents, vec2.mul(vec2.length(vec2.sub(position, oldPosition)), vec2.fromAngle(newDir + randomAngle)))`
- `distance : function(v1 : Vec2, v2 : Vec2) : number`  (call as `vec2.distance(…)`): Get the distance between two vectors.
    - param `v1`: first vector
    - param `v2`: second vector
    - returns: distance between the vectors
    - used 3x in the base game, e.g. `base/content/climates/climates/gen/mapgenutil.tl:156`: `local numSegments = math.ceil(vec2.distance(positionToCheck, oldPosition) / distanceCheck) * 4`
- `dot : function(v1 : Vec2, v2 : Vec2) : number`  (call as `vec2.dot(…)`): Compute the dot product of two vectors.
    - param `v1`: first vector
    - param `v2`: second vector
    - returns: dot product
- `angle : function(v1 : Vec2, v2 : Vec2) : number`  (call as `vec2.angle(…)`): Get the angle between two vectors.
    - param `v1`: first vector
    - param `v2`: second vector
    - returns: angle in radians
- `normalize : function(v : Vec2) : Vec2`  (call as `vec2.normalize(…)`): Normalize the vector (make it unit length).
    - param `v`: vector to normalize
    - returns: normalized vector
- `rotate90 : function(v : Vec2) : Vec2`  (call as `vec2.rotate90(…)`): Rotate the vector by 90 degrees.
    - param `v`: vector to rotate
    - returns: rotated vector
- `lerp : function(v1 : Vec2, v2 : Vec2, s : number) : Vec2`  (call as `vec2.lerp(…)`): Linear interpolation between two vectors.
    - param `v1`: first vector
    - param `v2`: second vector
    - param `s`: interpolation factor [0, 1]
    - returns: interpolated vector
    - used 2x in the base game, e.g. `base/content/climates/climates/gen/rivermapgenutil.tl:44`: `local newWidth = vec2.lerp(river.widths[i], river.widths[i+1], h)`
- `herp : function(p0 : Vec2, p1 : Vec2, t0 : Vec2, t1 : Vec2, s : number) : Vec2`  (call as `vec2.herp(…)`): Hermite interpolation between two points with tangents.
    - param `p0`: start point
    - param `p1`: end point
    - param `t0`: start tangent
    - param `t1`: end tangent
    - param `s`: interpolation factor [0, 1]
    - returns: interpolated vector
    - used 1x in the base game, e.g. `base/content/climates/climates/gen/rivermapgenutil.tl:40`: `local newPoint = vec2.herp(river.points[i], river.points[i+1], river.tangents[i], river.tangents[i+1], h)`
- `herpPrime : function(p0 : Vec2, p1 : Vec2, t0 : Vec2, t1 : Vec2, s : number) : Vec2`  (call as `vec2.herpPrime(…)`): Derivative of Hermite interpolation.
    - param `p0`: start point
    - param `p1`: end point
    - param `t0`: start tangent
    - param `t1`: end tangent
    - param `s`: interpolation factor [0, 1]
    - returns: derivative vector
    - used 1x in the base game, e.g. `base/content/climates/climates/gen/rivermapgenutil.tl:41`: `local newTangent = vec2.div(vec2.herpPrime(river.points[i], river.points[i+1], river.tangents[i], river.tangents[i+1], h), nSegments)`
- `fromAngle : function(a : number) : Vec2`  (call as `vec2.fromAngle(…)`): Create a unit vector from an angle.
    - param `a`: angle in radians
    - returns: unit vector
    - used 5x in the base game, e.g. `base/content/climates/climates/gen/mapgenutil.tl:20`: `local dirVec = vec2.fromAngle(angle)`
- `toArray : function(v : Vec2) : Table2`  (call as `vec2.toArray(…)`): Convert vector to an array of two numbers.
    - param `v`: vector
    - returns: array with x and y components
- `fromArray : function(t : Table2) : Vec2`  (call as `vec2.fromArray(…)`): Create vector from an array of two numbers.
    - param `t`: array with x and y components
    - returns: vector
- `print : function(v : Vec2) : string`  (call as `vec2.print(…)`): Convert vector to a string representation.
    - param `v`: vector
    - returns: string representation

### Vec2Serialized.Table2 (record)

```teal
record Table2 is {number}
```

of lenght 2

### Vec2Serialized.Vec2 (record)

```teal
record Vec2
```

2D vector as a plain table `{x = ..., y = ...}`. Vectors made by `new` (and by the operators) carry a
metatable, so `v1 + v2`, `v1 - v2`, `f * v` and `v / d` work.

- `x : number`: X component.
- `y : number`: Y component.
- `__add : function(v1 : Vec2, v2 : Vec2) : Vec2`: Component-wise sum `v1 + v2`.
- `__sub : function(v1 : Vec2, v2 : Vec2) : Vec2`: Component-wise difference `v1 - v2`.
- `__mul : function(f : number, v : Vec2) : Vec2`: Scalar product `f * v`; the number must be the left operand.
- `__div : function(v : Vec2, d : number) : Vec2`: Division of every component by a number, `v / d`.
