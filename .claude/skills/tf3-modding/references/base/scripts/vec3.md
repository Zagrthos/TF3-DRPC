# scripts.vec3

Definition file: `base/tealdef/scripts/vec3.d.tl` (in the game folder).

Types of the plain-Lua 3D vector library `/scripts/vec3.tl` (`Vec3Serialized`): vectors are ordinary
tables `{x = ..., y = ..., z = ...}` with `+`, `-`, `*` (number times vector) and `/` (vector by number).
Construction scripts use it together with `/scripts/mat4.tl`.
Load it with `local vec3 = ug_require "/scripts/vec3.tl" as Vec3Serialized`.

## Vec3iSerialized (record)

```teal
global record Vec3iSerialized
```

Namespace of the integer 3D vector type `Vec3i`.

### Vec3iSerialized.Vec3i (record)

```teal
record Vec3i
```

3D vector with integer components, as a plain table `{x = ..., y = ..., z = ...}`.

- `x : integer`: X component.
- `y : integer`: Y component.
- `z : integer`: Z component (height in world coordinates).

## Vec3Serialized (record)

```teal
global record Vec3Serialized
```
Load: `local vec3 = require "::/scripts/vec3.tl" as Vec3Serialized`

The 3D vector library returned by `ug_require "/scripts/vec3.tl"`, and the namespace of its `Vec3` type.

- `new : function(x : number, y : number, z : number) : Vec3`  (call as `vec3.new(…)`): Creates a 3D vector with arithmetic operators attached.
    - param `x`: X component.
    - param `y`: Y component.
    - param `z`: Z component.
    - returns: The new vector.

    ```lua
    -- from base/content/scripts/scripts/construction/street_util_serialized.tl
    local normalizedXYTangent = vec3.normalize(vec3.new(p1.x - p0.x, p1.y - p0.y, 0))
    ```

    - used 1041x in the base game, e.g. `base/content/base/base/metadataanimationutil.lua:79`: `return { times = { 0 }, transfs = { transf.scale(vec3.new(1,1,1)) }, interpolation = "NEAREST" }`
- `degToRad : function(dx : number, dy : number, dz : number) : Vec3`  (call as `vec3.degToRad(…)`): Converts three angles from degrees to radians and returns them as a vector.
    - param `dx`: Angle in degrees, stored in `x`.
    - param `dy`: Angle in degrees, stored in `y`.
    - param `dz`: Angle in degrees, stored in `z`.
    - returns: Vector of the angles in radians.
    - used 34x in the base game, e.g. `base/content/landmarks/hq/hq/headquarter.script.tl:337`: `transf = mat4.rotZYXTransl(vec3.degToRad(orientation and orientation * 90 or 0, 0, 0), center),`
- `add : function(v1 : Vec3, v2 : Vec3) : Vec3`  (call as `vec3.add(…)`): Returns `v1 + v2`.
    - used 14x in the base game, e.g. `base/content/infrastructure/street_construction/street_construction/street_constructions.script.lua:47`: `local pm = vec3.add(`
- `sub : function(v1 : Vec3, v2 : Vec3) : Vec3`  (call as `vec3.sub(…)`): Returns `v1 - v2`.
    - used 7x in the base game, e.g. `base/content/infrastructure/street_construction/street_construction/street_constructions.script.lua:49`: `vec3.mul(0.125, vec3.sub(q0, q1))`
- `mul : function(f : number, v : Vec3) : Vec3`  (call as `vec3.mul(…)`): Returns the vector `v` scaled by the number `f`.
    - used 20x in the base game, e.g. `base/content/infrastructure/street_construction/street_construction/street_constructions.script.lua:43`: `q0 = vec3.mul(scale, q0)`
- `div : function(v : Vec3, d : number) : Vec3`  (call as `vec3.div(…)`): Returns the vector `v` with each component divided by `d`.
- `dot : function(v1 : Vec3, v2 : Vec3) : number`  (call as `vec3.dot(…)`): Dot product `v1.x * v2.x + v1.y * v2.y + v1.z * v2.z`.
- `cross : function(v1 : Vec3, v2 : Vec3) : Vec3`  (call as `vec3.cross(…)`): Cross product `v1 × v2` (right-handed).
- `length : function(v : Vec3) : number`  (call as `vec3.length(…)`): Get the length (magnitude) of the vector.
    - param `v`: vector
    - returns: length of the vector
    - used 3x in the base game, e.g. `base/content/industries/industryutil.lua:334`: `local length = vec3.length(vec3.new( curve[2][1] - curve[1][1], curve[2][2] - curve[1][2], curve[2][3] - curve[1][3] ))`
- `distance : function(v1 : Vec3, v2 : Vec3) : number`  (call as `vec3.distance(…)`): Get the distance between two vectors.
    - param `v1`: first vector
    - param `v2`: second vector
    - returns: distance between the vectors
    - used 7x in the base game, e.g. `base/content/industries/fieldutil.lua:269`: `if vec3.distance(p, positions[j]) < 4 then`
- `distance2 : function(v1 : Vec3, v2 : Vec3) : number`  (call as `vec3.distance2(…)`): Get the squared distance between two vectors.
    - param `v1`: first vector
    - param `v2`: second vector
    - returns: squared distance between the vectors
- `normalize : function(v : Vec3) : Vec3`  (call as `vec3.normalize(…)`): Normalize the vector (make it unit length).
    - param `v`: vector to normalize
    - returns: normalized vector
    - used 7x in the base game, e.g. `base/content/infrastructure/street_construction/street_construction/street_constructions.script.lua:35`: `local q0 = vec3.normalize(t0)`
- `normalizeWithFallback : function(v : Vec3, f : Vec3) : Vec3`  (call as `vec3.normalizeWithFallback(…)`): Normalize the vector with a fallback if zero length.
    - param `v`: vector to normalize
    - param `f`: fallback vector
    - returns: normalized vector or fallback
    - used 2x in the base game, e.g. `dlcs/urbangames_deluxe_upgrade_pack/content/animal/animal/animal.script.lua:13`: `vec = vec3.normalizeWithFallback(vec, vecFallback)`
- `angleUnit : function(v1 : Vec3, v2 : Vec3) : number`  (call as `vec3.angleUnit(…)`): Get the angle between two unit vectors.
    - param `v1`: first unit vector
    - param `v2`: second unit vector
    - returns: angle in radians
    - used 3x in the base game, e.g. `base/content/infrastructure/street_construction/street_construction/street_constructions.script.lua:39`: `local angle = vec3.angleUnit(q0, q1)`
- `angle : function(v1 : Vec3, v2 : Vec3) : number`  (call as `vec3.angle(…)`): Get the angle between two vectors.
    - param `v1`: first vector
    - param `v2`: second vector
    - returns: angle in radians
- `xyAngle : function(v : Vec3) : number`  (call as `vec3.xyAngle(…)`): Get the angle in the XY plane.
    - param `v`: vector
    - returns: angle in radians
    - used 2x in the base game, e.g. `base/content/scripts/scripts/construction/constructionutil.lua:479`: `transf = transf.rotZTransl(vec3.xyAngle(fenceVector) + (angle or 0), fence_point),`
- `toArray : function(v : Vec3) : Table3`  (call as `vec3.toArray(…)`): Convert vector to an array of three numbers.
    - param `v`: vector
    - returns: array with x, y, z components
    - used 16x in the base game, e.g. `base/content/scripts/scripts/construction/collider_util_serialized.tl:17`: `result.params.halfExtents = vec3.toArray(halfExtents)`
- `fromArray : function(t : Table3) : Vec3`  (call as `vec3.fromArray(…)`): Create vector from an array of three numbers.
    - param `t`: array with x, y, z components
    - returns: vector
    - used 1x in the base game, e.g. `base/content/scripts/scripts/construction/constructionutil.lua:336`: `points[#points + 1] = vec3.fromArray(point)`
- `print : function(v : Vec3) : string`  (call as `vec3.print(…)`): Convert vector to a string representation.
    - param `v`: vector
    - returns: string representation

### Vec3Serialized.Table3 (record)

```teal
record Table3 is {number}
```

of lenght 3

### Vec3Serialized.Vec3 (record)

```teal
record Vec3
```

3D vector as a plain table `{x = ..., y = ..., z = ...}`. Vectors made by `new` (and by the operators)
carry a metatable, so `v1 + v2`, `v1 - v2`, `f * v` and `v / d` work.

- `x : number`: X component.
- `y : number`: Y component.
- `z : number`: Z component (up in world and model coordinates).
- `__add : function(v1 : Vec3, v2 : Vec3) : Vec3`: Component-wise sum `v1 + v2`.
- `__sub : function(v1 : Vec3, v2 : Vec3) : Vec3`: Component-wise difference `v1 - v2`.
- `__mul : function(f : number, v : Vec3) : Vec3`: Scalar product `f * v`; the number must be the left operand.
- `__div : function(v : Vec3, d : number) : Vec3`: Division of every component by a number, `v / d`.
