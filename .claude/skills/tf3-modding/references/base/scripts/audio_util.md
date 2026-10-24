# scripts.audio_util

Definition file: `base/tealdef/scripts/audio_util.d.tl` (in the game folder).

Types of `/scripts/audioutil.lua` (`AudioUtil`), helpers that build and sample the gain and pitch
curves of sound sets (lists of `{x, y}` nodes).

## AudioUtil (record)

```teal
global record AudioUtil
```

Sound curve helpers from `/scripts/audioutil.lua`.

- `plotSqrtY : function(refSpeed : number, y0 : number, y1 : number) : {number, number}`: Builds a curve of `{speed, sqrt(speed / refSpeed)}` nodes, used as a pitch curve that rises with the square root of speed.
    - param `refSpeed`: Speed at which the curve value is 1.
    - param `y0`: Curve value at the first node.
    - param `y1`: Curve value at the last node.
    - returns: List of `{x, y}` nodes.
- `plotSqrt : function(x0 : number, y0 : number, x1 : number, y1 : number, num : number) : {number, number}`: Builds `num + 1` nodes from `{x0, y0}` to `{x1, y1}` where y rises linearly and x quadratically.
    - returns: List of `{x, y}` nodes.
- `sampleCurve : function(nodes : {{number, number}}, x : number) : number`: Evaluates a piecewise-linear curve at `x`, clamped to the end values.
    - param `nodes`: List of `{x, y}` nodes sorted by `x`.
    - param `x`: Position at which to sample.
    - returns: The interpolated value; with a single node, its `y`.
