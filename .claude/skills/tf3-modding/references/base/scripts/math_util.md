# scripts.math_util

Definition file: `base/tealdef/scripts/math_util.d.tl` (in the game folder).

Types of the scalar math helpers in `/scripts/mathutil.lua` (`MathUtil`): interpolation, clamping,
range mapping and Hermite curves. Loading the file also installs the same functions into Lua's
global `math` table (`math.clamp`, `math.lerp`, `math.mapClamp`, ...).

## MathUtil (record)

```teal
global record MathUtil
```

Scalar math helpers from `/scripts/mathutil.lua`. Loading that file also defines the same functions on
Lua's global `math` table.

- `lerp : function<T>(T, T, T) : T`: Linear interpolation `a + (b - a) * s`.
    - param `#1`: Start value `a` (returned for `s = 0`).
    - param `#2`: End value `b` (returned for `s = 1`).
    - param `#3`: Interpolation factor `s`; not clamped.
- `invLerp : function<T>(T, T, T) : T`: Inverse of `lerp`, `(x - a) / (b - a)`; returns where `x` lies between `a` and `b` (0 at `a`, 1 at `b`).
    - param `#1`: The value `x`.
    - param `#2`: Range start `a`.
    - param `#3`: Range end `b`.
    - notes: Not clamped; values outside the range give results below 0 or above 1.
- `herp : function<T>(T, T, T, T, number) : T`: Cubic Hermite interpolation between `p0` and `p1` with tangents `t0` and `t1` at parameter `s` (0 to 1).
    - param `#1`: Start point `p0`.
    - param `#2`: End point `p1`.
    - param `#3`: Tangent `t0` at the start.
    - param `#4`: Tangent `t1` at the end.
    - param `#5`: Curve parameter `s`, 0 at `p0` and 1 at `p1`.
    - notes: Works on numbers and on vector types that support `+`, `-` and multiplication by a number (the transformator utilities use it with `Vec3f`).
- `herpPrime : function<T>(T, T, T, T, number) : T`: Derivative of `herp` with respect to `s`, i.e. the curve tangent at `s`.
    - param `#1`: Start point `p0`.
    - param `#2`: End point `p1`.
    - param `#3`: Tangent `t0` at the start.
    - param `#4`: Tangent `t1` at the end.
    - param `#5`: Curve parameter `s`.
- `randf : function(number, number) : number`: Uniform random number between the two arguments, based on `math.random()`.
- `log2 : function(number) : number`: Base-2 logarithm.
- `clamp : function<T>(T, T, T) : T`: Limits a value to a range.
    - param `#1`: The value.
    - param `#2`: Lower bound.
    - param `#3`: Upper bound.
    - returns: The lower bound if the value is below it, the upper bound if above, otherwise the value.
- `map : function<T>(T, T, T, T, T) : T`: Maps `x` linearly from the range `a`..`b` to the range `c`..`d`, without clamping.
    - param `#1`: The value `x`.
    - param `#2`: Source range start `a`.
    - param `#3`: Source range end `b`.
    - param `#4`: Target value `c` (for `x = a`).
    - param `#5`: Target value `d` (for `x = b`).
- `mapClamp : function<T>(T, T, T, T, T) : T`: Like `map`, but the result is clamped to the target range (in either order of `c` and `d`).
    - param `#1`: The value `x`.
    - param `#2`: Source range start `a`.
    - param `#3`: Source range end `b`.
    - param `#4`: Target value `c` (for `x <= a`).
    - param `#5`: Target value `d` (for `x >= b`).

    ```lua
    -- from base/content/game_mechanics/game_mechanics/industries/industries.script.tl
    local spawnProbability = math_util.clamp(math_util.invLerp(rating, 0.25, 0.75), 0, 1)
    ```

- `round : function(number) : integer`: Rounds to the nearest integer (`math.floor(x + 0.5)`; halves round up).
- `equal : function(number, number, number) : boolean`: Returns true if two numbers differ by at most the given epsilon (third argument).
- `llongMax : integer`: Large positive integer constant (`92233720368548`) used as an "infinite" upper limit.
- `llongMin : integer`: Large negative integer constant (`-92233720368548`) used as an "infinite" lower limit.
