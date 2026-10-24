# scripts.lang_util

Definition file: `base/tealdef/scripts/lang_util.d.tl` (in the game folder).

Types of `/scripts/lang_util.tl` (`LangUtil`): formatting of translated texts and numbers for the GUI.
It wraps `StringUtil.format` and the engine's `api.util.lang` functions.
Load it with `local lang_util = ug_require "/scripts/lang_util.tl" as LangUtil`.

## LangUtil (record)

```teal
global record LangUtil
```
Load: `local lang_util = ug_require "::/scripts/lang_util.tl" as LangUtil`

Text formatting helpers for the GUI, returned by `ug_require "/scripts/lang_util.tl"`.

- `format : function(string, {string : string}) : string`  (call as `lang_util.format(…)`): Same as `StringUtil.format`; fills `{name}` placeholders, typically in a translated `_("...")` text.

    ```lua
    return lang_util.format(_("There is no electrified path for {vehicle}."), { vehicle = names[1] })
    ```

    - used 432x in the base game, e.g. `base/content/base/base/difficulty_util.tl:239`: `log.warning(lang_util.format("Did not find {modParam}, hence no difficulty scale can be provided.", { modParam = modParam }))`
- `formatInt : function(integer) : string`  (call as `lang_util.formatInt(…)`): Formats an integer for display, using the engine's `api.util.lang.formatInt`.
    - used 241x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.script.tl:603`: `errorMessages[lang_util.format(nGetText("{numPermits} Permit Used Up", "All {numPermits} Permits Used Up", maxPermits), {numPermits = lang_util.formatInt(maxPer`
- `formatPlainInt : function(integer) : string`  (call as `lang_util.formatPlainInt(…)`): Converts an integer to a string with `tostring`, without any locale formatting.
    - used 25x in the base game, e.g. `base/content/game_mechanics/game_mechanics/achievements/achievements.script.tl:86`: `fromVersion = lang_util.formatPlainInt(stateVersion),`
- `formatNumber : function(value : number, decimals : integer) : string`  (call as `lang_util.formatNumber(…)`): Formats a number with the given number of decimals, using the engine's `api.util.lang.formatNumber`.
    - param `value`: The number.
    - param `decimals`: Number of decimal places.
    - used 28x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_react_util.tl:58`: `return lang_util.formatNumber(minMax.min, minMax.min < 1 and 1 or 0)`
- `stringContains : function(string, string) : boolean`  (call as `lang_util.stringContains(…)`): Text search used by the GUI (`api.util.lang.stringContains`); true if the first string contains the second.
    - used 30x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/manager_window.tl:1666`: `if searchString == "" or lang_util.stringContains(lineName, searchString) then`
- `compareStrings : function(string, string) : integer`  (call as `lang_util.compareStrings(…)`): Compares two strings for sorting (`api.util.lang.compareStrings`); negative if the first sorts before the second.

    ```lua
    -- from base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl
    table.sort(list, function(a, b) return lang_util.compareStrings(a, b) < 0 end)
    ```

    - used 10x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:209`: `return lang_util.compareStrings(a, b) < 0`
- `formatPercentageBoost : function(value : number) : string`  (call as `lang_util.formatPercentageBoost(…)`): Formats a fraction as a signed percentage (`+12%`, `-5%`) with `api.util.toStringPercent`; zero gets no sign.
    - param `value`: The fraction (for example `0.12` for +12 %).
    - used 7x in the base game, e.g. `base/content/gui/gui/construction/construction_desc_react_util.tl:121`: `text = lang_util.format(_("Comfort: {percent}"), { percent = lang_util.formatPercentageBoost(range.min) }),`
