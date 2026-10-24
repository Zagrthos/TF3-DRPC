# scripts.vec4

Definition file: `base/tealdef/scripts/vec4.d.tl` (in the game folder).

Types of the plain-Lua 4D vector library `/scripts/vec4.tl` (`Vec4Serialized`); `mat4` uses these
vectors as matrix columns. Load it with `local vec4 = ug_require "/scripts/vec4.tl" as Vec4Serialized`.

## Vec4iSerialized (record)

```teal
global record Vec4iSerialized
```

Namespace of the integer 4D vector type `Vec4i`.

### Vec4iSerialized.Vec4i (record)

```teal
record Vec4i
```

4D vector with integer components, as a plain table `{x = ..., y = ..., z = ..., w = ...}`.

- `x : integer`: X component.
- `y : integer`: Y component.
- `z : integer`: Z component.
- `w : integer`: W component.

## Vec4Serialized (record)

```teal
global record Vec4Serialized
```
Load: `local vec4 = require "::/scripts/vec4.tl" as Vec4Serialized`

The 4D vector library returned by `ug_require "/scripts/vec4.tl"`, and the namespace of its `Vec4` type.

- `new : function(x : number, y : number, z : number, w : number) : Vec4`  (call as `vec4.new(…)`): Creates a 4D vector with arithmetic operators attached.
    - param `x`: X component.
    - param `y`: Y component.
    - param `z`: Z component.
    - param `w`: W component.
    - returns: The new vector.
    - used 94x in the base game, e.g. `base/content/scripts/scripts/mat4.tl:10`: `vec4.new(m1[1] + m2[1], m1[2] + m2[2], m1[3] + m2[3], m1[4] + m2[4]),`
- `add : function(v1 : Vec4, v2 : Vec4) : Vec4`  (call as `vec4.add(…)`): Returns `v1 + v2`.
- `sub : function(v1 : Vec4, v2 : Vec4) : Vec4`  (call as `vec4.sub(…)`): Returns `v1 - v2`.
- `mul : function(f : number, v : Vec4) : Vec4`  (call as `vec4.mul(…)`): Returns the vector `v` scaled by the number `f`.
- `div : function(v : Vec4, d : number) : Vec4`  (call as `vec4.div(…)`): Returns the vector `v` with each component divided by `d`.
- `toArray : function(v : Vec4) : Table4`  (call as `vec4.toArray(…)`): Convert vector to an array of four numbers.
    - param `v`: vector
    - returns: array with x, y, z, w components
    - used 21x in the base game, e.g. `base/content/stations/street/street/modular_street_station/modules.script.lua:105`: `local tng = vec4.toArray(transf.gemv(transform, vec4.new(0.0, -20.0, 0.0, 0.0)))`
- `fromArray : function(t : Table4) : Vec4`  (call as `vec4.fromArray(…)`): Create vector from an array of four numbers.
    - param `t`: array with x, y, z, w components
    - returns: vector
- `print : function(v : Vec4) : string`  (call as `vec4.print(…)`): Convert vector to a string representation.
    - param `v`: vector
    - returns: string representation

### Vec4Serialized.Table4 (record)

```teal
record Table4 is {number}
```

of lenght 4

### Vec4Serialized.Vec4 (record)

```teal
record Vec4
```

4D vector as a plain table `{x = ..., y = ..., z = ..., w = ...}` with arithmetic operators. `mat4.new`
takes four of them as matrix columns.

- `x : number`: X component.
- `y : number`: Y component.
- `z : number`: Z component.
- `w : number`: W component (1 for points and 0 for directions when used with `mat4.gemv`).
- `__add : function(v1 : Vec4, v2 : Vec4) : Vec4`: Component-wise sum `v1 + v2`.
- `__sub : function(v1 : Vec4, v2 : Vec4) : Vec4`: Component-wise difference `v1 - v2`.
- `__mul : function(f : number, v : Vec4) : Vec4`: Scalar product `f * v`; the number must be the left operand.
- `__div : function(v : Vec4, d : number) : Vec4`: Division of every component by a number, `v / d`.
