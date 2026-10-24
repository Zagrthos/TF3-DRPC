# game_mechanics.game_time.game_time

Definition file: `base/tealdef/game_mechanics/game_time/game_time.d.tl` (in the game folder).

State, config and events of the game script that drives the time of day and the cloud coverage.

## GameTimeStateMode (enum)

```teal
global enum GameTimeStateMode
```

How the game script `game_time.gs` moves the time of day. The calendar's "Cycle" box sets it.

- `"Constant"`: The script leaves the time of day alone; the player sets it with the slider ("Custom" in the GUI).
- `"Automatic"`: The time of day runs on continuously ("Continuous" in the GUI).
- `"Local"`: The time of day follows the computer's local clock.
- `"Dynamic"`: The script picks random phases (noon, morning, evening) and moves the time of day toward each, holding it for a few minutes.

## WeatherStateMode (enum)

```teal
global enum WeatherStateMode
```

How the game script `game_time.gs` changes the cloud coverage.

- `"Constant"`: The script leaves the cloud coverage alone; the player sets it with the slider.
- `"Dynamic"`: The script picks random weather (sunny, cloudy, rainy) and moves the cloud coverage toward it.

## GameTimeState (record)

```teal
global record GameTimeState
```

State of the game script `::/game_mechanics/game_time/game_time.gs`, which sets the time of day and the cloud coverage.

Notes:

The script handles events with id `"GameTime"` (`SetMode` with `SetModeCommand`,
`SkipPhase`, `OnManualGameTimeChanged`) and id `"CloudCoverage"` (`SetMode` with
`SetWeatherModeCommand`, `SkipPhase`, `OnManualCloudCoverageChanged`). After a manual
change in `Dynamic` mode it waits about 60 seconds of game time (times the speed-up)
before picking the next phase.

The game time itself is in the `GAME_TIME` component of the world entity
(`gameTime` in ms, `timeOfDaySec` in seconds since midnight).

Example:
```lua
-- base game, gui/game_bar/game_bar_widgets.tl
local scriptEntity = api.engine.system.gameScriptSystem.getEntityForGameScript("::/game_mechanics/game_time/game_time.gs")
local gameScript = api.engine.getComponent(scriptEntity, api.type.ComponentType.GAME_SCRIPT) as Engine.Component.GameScript<GameTimeState>
local mode = gameScript.state.timeOfDayMode

local gameTime : Engine.Component.GameTime = api.engine.getComponent(api.engine.util.getWorld(), api.type.ComponentType.GAME_TIME)
print(gameTime.gameTime, gameTime.timeOfDaySec)
```

- `version : integer`: Version of the state layout (1).
- `timeOfDayMode : GameTimeStateMode`: Current time-of-day mode.
- `currentTimeOfDayState : integer`: Index of the current `Dynamic` phase (1 = noon, 2 = morning, 3 = evening).
- `timeOfDayTarget : integer`: Time of day the `Dynamic` mode moves toward, in seconds since midnight (0 to 86400).
- `timeOfDayTargetSpeed : number`: Rate of the `Dynamic` time-of-day transition, multiplied by the update's `dt`. `SkipPhase` sets it to 7000.
- `nextTimeOfDayChange : integer`: Game time (ms) at which `Dynamic` mode picks the next phase.
- `weatherMode : WeatherStateMode`: Current weather mode.
- `currentWeatherState : integer`: Index of the current `Dynamic` weather (1 = sunny, 2 = cloudy, 3 = rainy).
- `cloudCoverageTarget : number`: Cloud coverage the `Dynamic` mode moves toward, 0 to 1.
- `cloudCoverageTargetSpeed : number`: Rate of the cloud coverage transition, multiplied by the update's `dt`. `SkipPhase` sets it to 0.08.
- `nextChange : integer`: Game time (ms) at which `Dynamic` mode picks the next weather.

## GameTimeConfig (record)

```teal
global record GameTimeConfig
```

Initial time-of-day mode of a new game, in the generic resource `::/game_mechanics/game_time/game_time_config.res`. The base mod sets it from the game settings.

- `timeOfDayMode : GameTimeStateMode`: Initial time-of-day mode.

## WeatherConfig (record)

```teal
global record WeatherConfig
```

Initial weather mode of a new game, in the generic resource `::/game_mechanics/game_time/weather_config.res`. The base mod sets it from the game settings.

- `weatherMode : WeatherStateMode`: Initial weather mode.

## GameTimeGUIState (record)

```teal
global record GameTimeGUIState
```

GUI state of the game-time script. It is empty; the script has no GUI functions.

## SetModeCommand (record)

```teal
global record SetModeCommand
```

Param of the `GameTime`/`SetMode` script event, which changes the time-of-day mode.

Example:
```lua
-- base game, gui/game_bar/game_bar_widgets.tl
api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "GameTime", "SetMode", { mode = "Dynamic" }))
```

- `mode : GameTimeStateMode`: New time-of-day mode.

## SetWeatherModeCommand (record)

```teal
global record SetWeatherModeCommand
```

Param of the `CloudCoverage`/`SetMode` script event, which changes the weather mode.

Example:
```lua
-- base game, gui/game_bar/game_bar_widgets.tl
api.cmd.sendCommand(api.cmd.makeScriptingSendEventCmd("", "CloudCoverage", "SetMode", { weatherMode = "Constant" }))
```

- `weatherMode : WeatherStateMode`: New weather mode.
