# scripts.mat4

Definition file: `base/tealdef/scripts/mat4.d.tl` (in the game folder).

Types of the plain-Lua 4x4 matrix library `/scripts/mat4.tl` (`Mat4Serialized`). A matrix is a flat
table of 16 numbers in column-major order (elements 13, 14, 15 hold the translation), the same layout
the game uses for model and construction transforms (`transf`). The base game usually loads it as
`local mat4 = ug_require "/scripts/mat4.tl" as Mat4Serialized`.

## Mat4Serialized (record)

```teal
global record Mat4Serialized
```
Load: `local transf = require "::/scripts/mat4.tl" as Mat4Serialized`

The 4x4 matrix library returned by `ug_require "/scripts/mat4.tl"`, and the namespace of its `Mat4`
type. Matrices are flat 16-number tables in column-major order; the translation is in elements 13-15.

- `new : function(col1 : Vec4Serialized.Vec4, col2 : Vec4Serialized.Vec4, col3 : Vec4Serialized.Vec4, col4 : Vec4Serialized.Vec4) : Mat4`  (call as `transf.new(…)`): Creates a matrix from four column vectors and attaches the `+`, `-` and `*` operators.
    - param `col1`: First column (elements 1-4).
    - param `col2`: Second column (elements 5-8).
    - param `col3`: Third column (elements 9-12).
    - param `col4`: Fourth column (elements 13-16); `x`, `y`, `z` are the translation for affine transforms.
    - returns: The matrix as a flat 16-element table.
- `identity : function() : Mat4`  (call as `transf.identity(…)`): Returns the 4x4 identity matrix.

    ```lua
    -- from base/content/landmarks/landmark_util_serialized.tl
    lane.transf = mat4.identity()
    ```

    - used 29x in the base game, e.g. `base/content/depots/rail/rail/rail_depot.script.lua:359`: `local transform = transf.identity`
- `add : function(m1 : Mat4, m2 : Mat4) : Mat4`  (call as `transf.add(…)`): Returns the element-wise sum `m1 + m2`.
- `sub : function(m1 : Mat4, m2 : Mat4) : Mat4`  (call as `transf.sub(…)`): Returns the element-wise difference `m1 - m2`.
- `mul : function(m1 : Mat4, m2 : Mat4) : Mat4`  (call as `transf.mul(…)`): Returns the matrix product `m1 * m2`, i.e. the transform that applies `m2` first and then `m1`.
    - notes: To place a part relative to a parent transform, write `mat4.mul(parentTransf, localTransf)`.

    ```lua
    -- from base/content/landmarks/hq/hq/headquarter.script.tl
    coll.transf = mat4.mul(transf2, mat4.transl(vec3.new(0, 0, 14)))
    ```

    - used 60x in the base game, e.g. `base/content/buildings/townbuildingutil.lua:154`: `model.transf = transf.mul(transf.transl({ x = 0, y = 0, z = math.lerp(p1[3], p2[3], (alignmentPos[1] - p1[1]) / dx) }), model.transf)`
- `gemv : function(m : Mat4, v : Vec4Serialized.Vec4) : Vec4Serialized.Vec4`  (call as `transf.gemv(…)`): General matrix-vector multiplication.
    - param `m`: matrix
    - param `v`: vector
    - returns: resulting vector
    - used 30x in the base game, e.g. `base/content/scripts/scripts/construction/modulesutil.lua:358`: `modulesutil.mul = transf.gemv`
- `flipY : function(m : Mat4) : Mat4`  (call as `transf.flipY(…)`): Multiply the y coordinate of each column with -1.
    - param `m`: matrix
    - returns: matrix with Y flipped
- `transl : function(t : Vec3Serialized.Vec3) : Mat4`  (call as `transf.transl(…)`): Create translation matrix from a translation vector.
    - param `t`: translation vector
    - returns: translation matrix
    - used 222x in the base game, e.g. `base/content/buildings/townbuildingutil.lua:154`: `model.transf = transf.mul(transf.transl({ x = 0, y = 0, z = math.lerp(p1[3], p2[3], (alignmentPos[1] - p1[1]) / dx) }), model.transf)`
- `scale : function(t : Vec3Serialized.Vec3) : Mat4`  (call as `transf.scale(…)`): Create scale matrix.
    - param `t`: scale vector
    - returns: scale matrix
    - used 48x in the base game, e.g. `base/content/base/base/metadataanimationutil.lua:79`: `return { times = { 0 }, transfs = { transf.scale(vec3.new(1,1,1)) }, interpolation = "NEAREST" }`
- `rotZ : function(rotZ : number) : Mat4`  (call as `transf.rotZ(…)`): Create rotation matrix for the rotation around Z axis.
    - param `rotZ`: rotation angle in radians
    - returns: rotation matrix
    - used 18x in the base game, e.g. `base/content/base/base/metadataanimationutil.lua:304`: `local animation = generateRotateAnimation(maxAngle, transf.rotZ)`
- `rotY : function(rotY : number) : Mat4`  (call as `transf.rotY(…)`): Create rotation matrix for the rotation around Y axis.
    - param `rotY`: rotation angle in radians
    - returns: rotation matrix
    - used 2x in the base game, e.g. `base/content/base/base/metadataanimationutil.lua:296`: `local animation = generateRotateAnimation(maxAngle, transf.rotY)`
- `rotX : function(rotX : number) : Mat4`  (call as `transf.rotX(…)`): Create rotation matrix around X axis.
    - param `rotX`: rotation angle in radians
    - returns: rotation matrix
    - used 7x in the base game, e.g. `base/content/base/base/metadataanimationutil.lua:323`: `local animation = generateRotateAnimation(maxAngle, transf.rotX)`
- `rotZTransl : function(rotZ : number, transl : Vec3Serialized.Vec3) : Mat4`  (call as `transf.rotZTransl(…)`): Create matrix who has rotation around Z and translation
    - param `rotZ`: rotation angle
    - param `transl`: translation vector
    - returns: combined matrix
    - used 315x in the base game, e.g. `base/content/depots/rail/rail/rail_depot.script.lua:360`: `transform = transf.rotZTransl(4.71238898,vec3.new(-21, 0, 0.00000))`
- `rotZYXTransl : function(rot : Vec3Serialized.Vec3, transl : Vec3Serialized.Vec3) : Mat4`  (call as `transf.rotZYXTransl(…)`): Create matrix for ZYX rotations and translation.
    - param `rot`: rotation vector (x is rotZ, y is rotY, z is rotX) and performed in this order
    - param `transl`: translation vector
    - returns: combined matrix
    - used 34x in the base game, e.g. `base/content/landmarks/hq/hq/headquarter.script.tl:337`: `transf = mat4.rotZYXTransl(vec3.degToRad(orientation and orientation * 90 or 0, 0, 0), center),`
- `rotZYTransl : function(rot : Vec3Serialized.Vec3, transl : Vec3Serialized.Vec3) : Mat4`  (call as `transf.rotZYTransl(…)`): Create a matrix for ZY rotations and translation.
    - param `rot`: rotation vector (x is rotZ, y is rotY, z is unused) and performed in this order
    - param `transl`: translation vector
    - returns: combined matrix
- `rotYCntTransl : function(rotY : number, cnt : Vec3Serialized.Vec3, transl : Vec3Serialized.Vec3) : Mat4`  (call as `transf.rotYCntTransl(…)`): Create matrix for rotation around Y with a center specified and a translation.
    - param `rotY`: rotation angle
    - param `cnt`: center point
    - param `transl`: translation vector
    - returns: combined matrix
- `rotZY : function(rot : Vec3Serialized.Vec3) : Mat4`  (call as `transf.rotZY(…)`): Create a matrix for ZY rotations.
    - param `rot`: rotation vector (x is rotZ, y is rotY, z is unused) and performed in this order
    - returns: rotation matrix
- `scaleRotZTransl : function(scale : number, rotZ : number, transl : Vec3Serialized.Vec3) : Mat4`  (call as `transf.scaleRotZTransl(…)`): Create matrix for (homogenous) scaling, rotation around Z, and translation.
    - param `scale`: scale factor
    - param `rotZ`: rotation angle
    - param `transl`: translation vector
    - returns: combined matrix
    - used 93x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/gui/celebration.css.lua:16`: `transform = transf.scaleRotZTransl(1.0, 0, vec3.new(0.0, -200.0, 0.0)),`
- `scaleXYZRotZTransl : function(scale : Vec3Serialized.Vec3, rotZ : number, transl : Vec3Serialized.Vec3) : Mat4`  (call as `transf.scaleXYZRotZTransl(…)`): Create matrix for (inhomogenous) scaling, rotation around Z, and translation.
    - param `scale`: scale vector
    - param `rotZ`: rotation angle
    - param `transl`: translation vector
    - returns: combined matrix
    - used 5x in the base game, e.g. `base/content/infrastructure/bridge/bridge/bridgeutil.lua:223`: `local modelTransf = transf.scaleXYZRotZTransl(scaleVec, .0, posVec)`
- `scaleRotZYXTransl : function(scale : Vec3Serialized.Vec3, rot : Vec3Serialized.Vec3, transl : Vec3Serialized.Vec3) : Mat4`  (call as `transf.scaleRotZYXTransl(…)`): Create matrix for (inhomogenous) scaling, ZYX rotations and translation.
    - param `scale`: scale vector
    - param `rot`: rotation vector (x is rotZ, y is rotY, z is rotX) and performed in this order
    - param `transl`: translation vector
    - returns: combined matrix
- `getTransl : function(m : Mat4) : Vec3Serialized.Vec3`  (call as `transf.getTransl(…)`): Extract translation vector from matrix.
    - param `m`: matrix
    - returns: translation vector
- `print : function(v : Mat4) : string`  (call as `transf.print(…)`): Convert matrix to a string representation.
    - param `v`: matrix
    - returns: string representation

### Mat4Serialized.Mat4 (record)

```teal
record Mat4 is {number}
```

16 entries, column major

- `__add : function(m1 : Mat4, m2 : Mat4) : Mat4`: Element-wise sum `m1 + m2`.
- `__sub : function(m1 : Mat4, m2 : Mat4) : Mat4`: Element-wise difference `m1 - m2`.
- `__mul : function(m1 : Mat4, m2 : Mat4) : Mat4`: Matrix product `m1 * m2`. Applied to a point, `m1 * m2` first applies `m2`, then `m1` (column vectors).
