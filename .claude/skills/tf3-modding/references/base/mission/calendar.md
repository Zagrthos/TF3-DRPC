# mission.calendar

Definition file: `base/tealdef/mission/calendar.d.tl` (in the game folder).

Date helper for missions.

## Calendar (record)

```teal
global record Calendar
```
Load: `local calendar = ug_require "::/mission/calendar.lua" as Calendar`

Date helper for missions.

- `syncDate : function(integer)`  (call as `calendar.syncDate(…)`): Sets the game date to the given start year plus the time elapsed in the current two-year cycle of game time.
    - param `#1`: Start year.
    - used 6x in the base game, e.g. `mods/release/urbangames_campaign_mission_01/content/mission/mission/tasks/everything/everything.tl:36`: `calendar.syncDate(params.missionStartYear)`
