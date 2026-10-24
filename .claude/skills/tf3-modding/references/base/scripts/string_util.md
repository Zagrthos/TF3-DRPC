# scripts.string_util

Definition file: `base/tealdef/scripts/string_util.d.tl` (in the game folder).

Types of `/scripts/string_util.tl` (`StringUtil`): `{placeholder}` string interpolation and
prefix/suffix tests. The functions come from the legacy `/scripts/stringutil.lua`.

## StringUtil (record)

```teal
global record StringUtil
```
Load: `local string_util = ug_require "::/scripts/string_util.tl" as StringUtil`

String helpers returned by `ug_require "/scripts/string_util.tl"`.

- `format : function(string) : string`  (call as `string_util.format(…)`): Replaces `{name}` placeholders in a string with values from a table (the legacy `string.interp`).
    - notes: - With a table, `{name}` is replaced by `tab.name` (or `tab[number]` for `{1}`, `{2}`, ...); unknown placeholders stay as they are. - An empty `{}` is numbered automatically: the first `{}` becomes `{1}`, the next `{2}`, and so on. - With string arguments instead of a table (`format(s, a, b, ...)`), the `{}` placeholders are filled with `a`, `b`, ... in order.

    ```lua
    -- from base/content/scripts/scripts/table_util.tl
    log.error(string_util.format("No comparison of functions (key = {}), need '{}' sibling attribute on both tables to compare instead", toString(key), compareProxyKey))
    ```

    - used 26x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_util.tl:128`: `local str = string_util.format("Deprecated use of company metadata rankAndPermits together with permitKey ({permitKey} is already used for {otherUse})", {permit`
- `format : function(string, {string : string}) : string`  (call as `string_util.format(…)`): Replaces `{name}` placeholders in a string with values from a table (the legacy `string.interp`).
    - notes: - With a table, `{name}` is replaced by `tab.name` (or `tab[number]` for `{1}`, `{2}`, ...); unknown placeholders stay as they are. - An empty `{}` is numbered automatically: the first `{}` becomes `{1}`, the next `{2}`, and so on. - With string arguments instead of a table (`format(s, a, b, ...)`), the `{}` placeholders are filled with `a`, `b`, ... in order.

    ```lua
    -- from base/content/scripts/scripts/table_util.tl
    log.error(string_util.format("No comparison of functions (key = {}), need '{}' sibling attribute on both tables to compare instead", toString(key), compareProxyKey))
    ```

    - used 26x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_util.tl:128`: `local str = string_util.format("Deprecated use of company metadata rankAndPermits together with permitKey ({permitKey} is already used for {otherUse})", {permit`
- `format : function(string, {string : string}) : string`  (call as `string_util.format(…)`): Replaces `{name}` placeholders in a string with values from a table (the legacy `string.interp`).
    - notes: - With a table, `{name}` is replaced by `tab.name` (or `tab[number]` for `{1}`, `{2}`, ...); unknown placeholders stay as they are. - An empty `{}` is numbered automatically: the first `{}` becomes `{1}`, the next `{2}`, and so on. - With string arguments instead of a table (`format(s, a, b, ...)`), the `{}` placeholders are filled with `a`, `b`, ... in order.

    ```lua
    -- from base/content/scripts/scripts/table_util.tl
    log.error(string_util.format("No comparison of functions (key = {}), need '{}' sibling attribute on both tables to compare instead", toString(key), compareProxyKey))
    ```

    - used 26x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_util.tl:128`: `local str = string_util.format("Deprecated use of company metadata rankAndPermits together with permitKey ({permitKey} is already used for {otherUse})", {permit`
- `format : function(string, string, ... : any) : string`  (call as `string_util.format(…)`): Replaces `{name}` placeholders in a string with values from a table (the legacy `string.interp`).
    - notes: - With a table, `{name}` is replaced by `tab.name` (or `tab[number]` for `{1}`, `{2}`, ...); unknown placeholders stay as they are. - An empty `{}` is numbered automatically: the first `{}` becomes `{1}`, the next `{2}`, and so on. - With string arguments instead of a table (`format(s, a, b, ...)`), the `{}` placeholders are filled with `a`, `b`, ... in order.

    ```lua
    -- from base/content/scripts/scripts/table_util.tl
    log.error(string_util.format("No comparison of functions (key = {}), need '{}' sibling attribute on both tables to compare instead", toString(key), compareProxyKey))
    ```

    - used 26x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_util.tl:128`: `local str = string_util.format("Deprecated use of company metadata rankAndPermits together with permitKey ({permitKey} is already used for {otherUse})", {permit`
- `starts : function(string, string) : boolean`  (call as `string_util.starts(…)`): True if the first string begins with the second.
    - used 21x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:17`: `return string_util.starts(res, "::/vehicle")`
- `ends : function(string, string) : boolean`  (call as `string_util.ends(…)`): True if the first string ends with the second (always true for an empty suffix).
    - used 1x in the base game, e.g. `base/content/scripts/scripts/table_util.tl:24`: `local allowDirectComparison = allowFunctions == true or (type(key) == 'string' and string_util.ends(key as string, 'ecipe'))`
