# scripts.util

Definition file: `base/tealdef/scripts/util.d.tl` (in the game folder).

Types of `/scripts/util.tl` (`Util`): resolving script function references and formatting game-time
durations. Load it with `local util = ug_require "/scripts/util.tl" as Util`.

## Util (record)

```teal
global record Util
```
Load: `local util = ug_require "::/scripts/util.tl" as Util`

Miscellaneous script helpers returned by `ug_require "/scripts/util.tl"`.

teal only extension of util --> call with util.funYouWannaCall(...) and util = ug_require "path/to/this"
- `useFn : function(string) : function`  (call as `util.useFn(…)`)
    - used 70x in the base game, e.g. `base/content/game_mechanics/game_mechanics/notifications/gui/notification_log.tl:31`: `local dataStateFn = util.useFn(param.entry.notification.type .. "@useDataState") as (function(any, any) : NotificationGuiData)`
- `formatTimePoint : function(number) : string`  (call as `util.formatTimePoint(…)`)
- `formatDurationWithCurrentCalenderSpeed : function(durationInDefaultSpeedMs : integer, integer, ?DurationRounding) : string`  (call as `util.formatDurationWithCurrentCalenderSpeed(…)`): Formats a duration as years, months and days at the current calendar speed.
    - param `durationInDefaultSpeedMs`: Duration in milliseconds of game time.
    - param `#2`: The current `millisPerDay` of the `GAME_SPEED` component; `0` (calendar paused) formats the plain game time instead.
    - param `#3`: Optional rounding mode; without it the result lists years, months and days.
    - returns: Translated text such as "1 Year 3 Months".
    - used 27x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_util.tl:216`: `remainingTime = util.formatDurationWithCurrentCalenderSpeed(math_util.round(remainingCooldown.remainingMs), millisPerDay)`

### Util.DurationRounding (enum)

```teal
enum DurationRounding
```

Rounding mode for `formatDurationWithCurrentCalenderSpeed`.

- `"OnlyMonth"`: Show the duration in months only (years converted to months).
- `"Month"`: Show years and months, rounded to the nearest month.
- `"Year"`: Show whole years, rounded to the nearest year.
