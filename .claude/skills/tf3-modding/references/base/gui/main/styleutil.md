# gui.main.styleutil

Definition file: `base/tealdef/gui/main/styleutil.d.tl` (in the game folder).

Helper that builds an `api.gui.StyleSheet` from a plain Lua table.

## StyleUtil (record)

```teal
global record StyleUtil
```
Load: `local styleutil = ug_require "::/gui/main/styleutil.tl" as StyleUtil`

Style sheet helper.

- `makeStyle : function(styleTable : table) : StyleSheet`  (call as `styleutil.makeStyle(…)`): Creates an `api.gui.StyleSheet` from a table.
    - param `styleTable`: Table with any of `borderColor`, `color`, `backgroundColor`, `backgroundColor1`,
`backgroundColor2`, `borderWidth` (each `{r, g, b, a}` or a `Vec4f`), `size`,
`anchorPoint` (`{x, y}` or a `Vec2f`), `backgroundImage1`, `backgroundImage2`
(nine-patch) and `fontSize` (integer). Other keys are ignored.

    - returns: The new style sheet.
    - used 48x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_cargo_lifetime.tl:161`: `styleSheet = styleutil.makeStyle{`
