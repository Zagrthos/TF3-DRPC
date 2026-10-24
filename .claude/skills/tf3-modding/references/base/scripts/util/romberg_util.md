# scripts.util.romberg_util

Definition file: `base/tealdef/scripts/util/romberg_util.d.tl` (in the game folder).

Types of `/scripts/util/romberg.tl` (`RombergUtil`), numerical integration with Romberg's method.

## RombergUtil (record)

```teal
global record RombergUtil
```

Numerical integration returned by `ug_require "/scripts/util/romberg.tl"`.

- `rombergIntegration : function(a : number, h : number, tolerance : number, maxCols : integer, fn : function(number) : number) : {number, number}`: Integrates `fn` from `a` to `a + h` with Romberg's method (trapezoidal rule plus Richardson extrapolation).
    - param `a`: Lower limit of integration.
    - param `h`: Length of the interval; the upper limit is `a + h`.
    - param `tolerance`: Stop when the estimate changes by less than this.
    - param `maxCols`: Maximum number of extrapolation columns (capped at 27).
    - param `fn`: The function to integrate.
    - returns: `{integral, err}`: `err` is 0 if the tolerance was reached and -1 if not.
