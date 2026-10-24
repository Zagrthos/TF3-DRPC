# api.util

Definition file: `api/tealdef/api/util.d.tl` (in the game folder).

Contains generic util functions that are not associated with a specific game instance.
All functions in this namespace can be called from anywhere.

## ApiUtil (record)

```teal
global record ApiUtil
```
Access: `api.util`

main util interface

- `getLuaUsedMemory : function() : integer`  (call as `api.util.getLuaUsedMemory(…)`): Obtains the total used memory by this lua state.
    - returns: int used memory in kb
- `toStringPercent : function(percentage : number) : string`  (call as `api.util.toStringPercent(…)`): Formats a decimal value to percent.
    - param `percentage`: the number to format
    - returns: string with formatted percentage
    - used 28x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_loan_gui.tl:347`: `api.util.toStringPercent(loan.percentage),`
- `toStringPercentPrecision : function(percentage : number, decimals : integer) : string`  (call as `api.util.toStringPercentPrecision(…)`): Formats a decimal value to percent.
    - param `percentage`: the number to format
    - param `decimals`: wished precision
    - returns: string with formatted percentage
    - used 49x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_react_util.tl:244`: `label =  api.util.toStringPercentPrecision(state:old().passengerLineUsage, 0),`
- `formatTimestamp : function(timestamp : integer) : string`  (call as `api.util.formatTimestamp(…)`): Formats a timestamp with appropriate localization.
    - param `timestamp`: the timestamp to format
    - returns: string with formatted time
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/savegame_react_util.tl:842`: `text = api.util.formatTimestamp(param.save.timestamp)`
- `formatPosixTimestamp : function(timestamp : integer, dateFormat : DateFormat) : string`  (call as `api.util.formatPosixTimestamp(…)`): Formats a timestamp with appropriate localization and according date format.
    - param `timestamp`: long number The timestamp
    - param `dateFormat`: DateFormat format The format to use for formatting
    - returns: string with formatted timestamp
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:17`: `return unixTime and api.util.formatPosixTimestamp(unixTime, api.type.enum.DateFormat.MEDIUM) or _("N/A")`
- `formatSpeed : function(speed : number) : string`  (call as `api.util.formatSpeed(…)`): Defaults to km/h unless the user selected another system of measurement.
    - param `speed`: speed in meter per second
    - returns: string with formatted speed
    - used 12x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:256`: `label = api.util.formatSpeed(param.topSpeed),`
- `formatAltitude : function(meters : number, decimals? : integer) : string`  (call as `api.util.formatAltitude(…)`): Formats an altitude with the correct locale and unit of measurement.
Defaults to meters unless the user selected another system of measurement.
    - param `meters`: length distance in meters
    - param `decimals`: number of decimals (default: 1 decimal for numbers < 10, otherwise 0)
    - returns: string with formatted altitude
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game_tooltips.tl:241`: `return api.util.formatAltitude(value, 0)`
- `formatLength : function(meters : number, decimals? : integer, forceSmallUnit? : boolean) : string`  (call as `api.util.formatLength(…)`): Formats a distance with the correct locale and unit of measurement.
Defaults to meters unless the user selected another system of measurement.
    - param `meters`: length distance in meters
    - param `decimals`: number of decimals (default: 1 decimal for numbers < 10, otherwise 0)
    - param `forceSmallUnit`: : boolean If true, use m or ft even for big numbers (default: false)
    - returns: string with formatted distance
    - used 22x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:97`: `label = api.util.formatLength(total),`
- `formatMoney : function(money : integer) : string`  (call as `api.util.formatMoney(…)`): Formats a money amount with the correct locale and currency.
    - param `money`: amount of money or nil if money does not matter (aka is infinite)
    - returns: string with formatted amount of money
    - used 52x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_charts.tl:150`: `return api.util.formatMoney(mathutil.round(value))`
- `formatMoneyAlt : function(money : integer) : string`  (call as `api.util.formatMoneyAlt(…)`): Formats a money amount with the opposite locale and currency (if current format is set to full it shows short and vice versa)
    - param `money`: amount of money or nil if money does not matter (aka is infinite)
    - returns: string with formatted amount of money
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:243`: `tooltip = (not isMoneyEditable) and api.util.formatMoneyAlt(balanceState:old()) or nil`
- `formatMoneyNumber : function(money : integer) : string`  (call as `api.util.formatMoneyNumber(…)`): Formats a money amount with the correct locale.
    - param `money`: amount of money or nil if money does not matter (aka is infinite)
    - returns: string with formatted amount of money
    - used 1x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:245`: `text = api.util.formatMoneyNumber(balanceState:old())`
- `formatWeight : function(weight : number) : string`  (call as `api.util.formatWeight(…)`): Formats a number as weight with the correct locale.
    - param `weight`: the number in metric tons
    - returns: string with formatted weight
    - used 1x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_window.tl:574`: `formatDelta(vehicleData.weight, comparisonData and comparisonData.weight or nil, api.util.formatWeight),`
- `formatPower : function(power : number) : string`  (call as `api.util.formatPower(…)`): Formats a number as power with the correct locale.
    - param `power`: the number in kW
    - returns: string with formatted power
    - used 3x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_table.tl:147`: `text = api.util.formatPower(elem.static.vehicleData.power)`
- `decibelToPower : function(decibel : number) : number`  (call as `api.util.decibelToPower(…)`): Converts Decibel To Power
    - param `decibel`: the number to convert
    - returns: number with converted power
- `powerToDecibel : function(power : number) : number`  (call as `api.util.powerToDecibel(…)`): Converts Power To Decibel
    - param `power`: the number to convert
    - returns: number with converted decibel
    - used 5x in the base game, e.g. `base/content/game_mechanics/game_mechanics/towns/town_util.tl:425`: `local pollutionDB = api.util.powerToDecibel(avgPollution)`
- `formatForce : function(force : number) : string`  (call as `api.util.formatForce(…)`): Formats a number as force with the correct locale.
    - param `force`: the number in kN
    - returns: string with formatted force
    - used 5x in the base game, e.g. `base/content/gui/gui/line_vehicle_mgmt/vehicle_store_table.tl:144`: `text = api.util.formatForce(elem.static.vehicleData.thrust)`
- `formatMemory : function(memory : integer) : string`  (call as `api.util.formatMemory(…)`): Formats a number as memory with the correct locale.
    - param `memory`: the number in bytes
    - returns: string with formatted memory
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:353`: `local installSize = api.util.formatMemory(modInfo.installSize)`
- `getMonthDuration : function(currentMillisPerDay : integer) : integer`  (call as `api.util.getMonthDuration(…)`): Obtains month duration in the game regarding the current game speed's millis per day.
    - param `currentMillisPerDay`: number of milliseconds in a day
    - returns: integer with the current game ticks per month
    - used 3x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_cargo_lifetime.tl:28`: `local monthDuration = api.util.getMonthDuration(millisPerDay)`
- `getYearDuration : function(currentMillisPerDay : integer) : integer`  (call as `api.util.getYearDuration(…)`): Obtains year duration in the game regarding the current game speed's millis per day.
    - param `currentMillisPerDay`: number of milliseconds in a day
    - returns: integer with the current game ticks per year
    - used 8x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_cargo_lifetime.tl:23`: `local yearDuration = api.util.getYearDuration(millisPerDay)`
- `getDefaultDayDuration : function() : integer`  (call as `api.util.getDefaultDayDuration(…)`): Obtains the default day duration in the game.
    - returns: integer with the default game ticks per day
    - used 13x in the base game, e.g. `base/content/base/base/mod.script.tl:166`: `baseConfig.economy.industryDevelopment.spawnTargetTimeSpan = 50 * 365.25 * api.util.getDefaultDayDuration()`
- `getDefaultMonthDuration : function() : integer`  (call as `api.util.getDefaultMonthDuration(…)`): Obtains the default month duration in the game.
    - returns: integer with the default game ticks per month
    - used 25x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company_util.tl:14`: `company_util.prospectionTimeMs = 6 * api.util.getDefaultMonthDuration()`
- `getDefaultYearDuration : function() : integer`  (call as `api.util.getDefaultYearDuration(…)`): Obtains the default year duration in the game.
    - returns: integer with the default game ticks per year
    - used 35x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_loan_gui.tl:374`: `local initialDuration = 10 * api.util.getDefaultYearDuration()`
- `formatSeconds : function(seconds : integer) : string`  (call as `api.util.formatSeconds(…)`): Formats number of seconds into string
    - param `seconds`: the number of seconds to format
    - returns: string with formatted time
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/line/line_eow.script.tl:65`: `return exact and api.util.formatMinutesSeconds(frequency) or api.util.formatSeconds(frequency)`
- `formatMinutesSeconds : function(seconds : integer, hideZeroSeconds? : boolean) : string`  (call as `api.util.formatMinutesSeconds(…)`): Formats number of seconds into string
    - param `seconds`: the number of seconds to format
    - param `hideZeroSeconds`: omit the "0 s" suffix (optional, default: false)
    - returns: string with formatted time
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_loan_gui.tl:210`: `return api.util.formatMinutesSeconds(roundedToMinutes, true)`
- `formatDays : function(days : integer) : string`  (call as `api.util.formatDays(…)`): Formats number of days into string
    - param `days`: the number of days to format
    - returns: string with formatted time
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/industry_react_util.tl:27`: `return api.util.formatDays(numDays)`
- `formatGameTime : function(gameTime : integer) : string`  (call as `api.util.formatGameTime(…)`): Formats given gameTime as real time string
    - param `gameTime`: the gameTime to format
    - returns: string with formatted date
    - used 2x in the base game, e.g. `base/content/scripts/scripts/util.tl:34`: `return api.util.formatGameTime((timestamp) as integer)`
- `formatDate : function(date : Date, dateFormat : DateFormat) : string`  (call as `api.util.formatDate(…)`): Formats given date with the correct locale.
    - param `date`: the date to format
    - param `dateFormat`: the format to use for formatting
    - returns: string with formatted date
    - used 11x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_assets.tl:374`: `return api.util.formatDate(date, api.type["enum"].DateFormat.YEAR)`
- `getLocalTime : function() : integer`  (call as `api.util.getLocalTime(…)`): Get the local real time of the user machine
    - returns: the time
    - used 2x in the base game, e.g. `base/content/game_mechanics/game_mechanics/game_time/game_time.script.tl:193`: `local date = os.date("*t", api.util.getLocalTime())`
- `formatTime : function(time : integer) : string`  (call as `api.util.formatTime(…)`): Formats given time
    - param `time`: the time to format
    - returns: string with formatted time
    - used 1x in the base game, e.g. `base/content/gui/gui/main/pause_menu.tl:240`: `local time = api.util.formatDate(api.type.Date.now(), api.type.enum.DateFormat.FULL) .. " " .. api.util.formatTime(api.util.getLocalTime())`
- `formatKMB : function(number : number) : string`  (call as `api.util.formatKMB(…)`): Formats a number with K/M/B suffix.
    - param `number`: the number to format
    - returns: string with formatted number
    - used 1x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_react_util.tl:116`: `label = api.util.formatKMB(legendParam.chartData.series[i][2][#legendParam.chartData.series[i][2]]),`
- `splitSentences : function(sentences : string) : {string}`  (call as `api.util.splitSentences(…)`): Splits string into list of sentences
    - param `sentences`: the string containing sentences
    - returns: list of sentences
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_react_util.tl:182`: `local sentences = api.util.splitSentences(subtitle.text)`
- `getRainbowColors : function(number : integer, alpha? : number) : {Vec4f}`  (call as `api.util.getRainbowColors(…)`): Generate n colors, equally distributed over the "rainbow"
    - param `number`: number of wished colors
    - param `alpha`: alpha, if not set, alpha is set to 1 (optional)
    - returns: vector or Vec4f colors
- `getApplicationTime : function() : number`  (call as `api.util.getApplicationTime(…)`): Get seconds since application start. Only available in GUI thread.
    - returns: duration since application started
    - used 53x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:187`: `calloutCooldownStartTimeRef:set(api.util.getApplicationTime())`
- `getInputMode : function() : InputMode`  (call as `api.util.getInputMode(…)`): Returns the current input mode. Only available in GUI thread.
Get current input mode. Only available in GUI thread.
    - returns: the input mode
    - used 54x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_table.tl:49`: `return api.util.getInputMode() == api.type["enum"].InputMode.Gamepad`
- `getGamepadType : function() : GamepadType`  (call as `api.util.getGamepadType(…)`): Returns the current gamepad type.
Get current gamepad type.
    - returns: the gamepad type
    - used 3x in the base game, e.g. `base/content/mission/mission/tutorial/guide_system/guide_system_story.tl:4`: `if api.util.getGamepadType() == api.type.enum.GamepadType.PlayStation4 or api.util.getGamepadType() == api.type.enum.GamepadType.PlayStation5 then`
- `isControllerConnected : function() : boolean`  (call as `api.util.isControllerConnected(…)`): Returns if a controller is connected. Only available in GUI thread.
    - returns: if a controller is connected
- `isKeyboardConnected : function() : boolean`  (call as `api.util.isKeyboardConnected(…)`): Returns if a keyboard is connected. Only available in GUI thread.
    - returns: if a keyboard is connected
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:702`: `if not isConsole or api.util.isKeyboardConnected() then`
- `isMouseConnected : function() : boolean`  (call as `api.util.isMouseConnected(…)`): Returns if a mouse is connected. Only available in GUI thread.
    - returns: if a mouse is connected
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:658`: `if not isConsole or api.util.isMouseConnected() then`
- `getKeyName : function(keyCommand : string) : string`  (call as `api.util.getKeyName(…)`): Returns the key sequence associated to the command (eg. `api.util.getKeyName("constructOptPrecise1")`)
See the ''settings_keys_v3.lua'' file in the userdata local folder for a list of all available key commands.
Get key combos for this input action. Only available in GUI thread.
    - param `keyCommand`: the command to which the key sequence is associated.
    - returns: the key sequence associated with the given key command
- `getBuildVersion : function() : string`  (call as `api.util.getBuildVersion(…)`): Returns the build version
    - returns: the build version
- `getBuildVersionString : function() : string`  (call as `api.util.getBuildVersionString(…)`): Returns the build version string
    - returns: the build version
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/main_page.tl:764`: `text = api.util.getBuildVersionString(),`
- `isBuildTypeTesting : function() : boolean`  (call as `api.util.isBuildTypeTesting(…)`): Returns whether build is development build
    - returns: true if build is development build
- `isBuildTypeRelease : function() : boolean`  (call as `api.util.isBuildTypeRelease(…)`): Returns whether build is release build
    - returns: true if build is release build
- `isConsoleMode : function() : boolean`  (call as `api.util.isConsoleMode(…)`): Returns whether the game is running on a console
    - returns: true if the game is running on a console
    - used 26x in the base game, e.g. `base/content/base/base/mod.script.tl:130`: `maxNumberPerArea = api.util.isConsoleMode() and 0.64 or 0.8,			-- km^(-2)`
- `getModMemoryLimitMB : function() : integer`  (call as `api.util.getModMemoryLimitMB(…)`): Memory budget for active mods, in megabytes.
    - returns: The budget in MB; a negative value means there is no limit (the mod selector then skips its memory check).
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:882`: `local memoryLimitMB = api.util.getModMemoryLimitMB()`
- `isPlatform : function(platform : Platform) : boolean`  (call as `api.util.isPlatform(…)`): Returns whether build is of certain platform
    - param `platform`: the platform to check
    - returns: true if build is of platform
    - used 6x in the base game, e.g. `base/content/base/base/mod.lua:187`: `if api.util.isPlatform(api.type["enum"].Platform.PS) then`
- `string2Platform : function(platform : string) : Platform`  (call as `api.util.string2Platform(…)`): Transforms string to platform
    - param `platform`: the platform string
    - returns: the plafrom enum value
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/releasenotes_util.tl:96`: `local plat = api.util.string2Platform(platform)`
- `isVendor : function(vendor : Vendor) : boolean`  (call as `api.util.isVendor(…)`): Returns whether build is of certain vendor
    - param `vendor`: the vendor
    - returns: bool true if build is from the vendor
    - used 18x in the base game, e.g. `base/content/gui/gui/menu/paradox_react_util.tl:14`: `if api.util.isVendor(api.type["enum"].Vendor.Steam) then`
- `string2Vendor : function(vendor : string) : Vendor`  (call as `api.util.string2Vendor(…)`): Transforms string to vendor
    - param `vendor`: the vendor string
    - returns: the value from the vendor enum
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/releasenotes_util.tl:86`: `local ven = api.util.string2Vendor(vendor)`
- `saveAppConfig : function()`  (call as `api.util.saveAppConfig(…)`): Saves the game settings
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/license_page.tl:107`: `api.util.saveAppConfig()`
- `getAppConfig : function() : AppConfig`  (call as `api.util.getAppConfig(…)`): Returns the game settings
    - returns: the game settings
    - used 40x in the base game, e.g. `base/content/game_mechanics/game_mechanics/celebrations/celebration_react_util.tl:149`: `if not (api.util.getAppConfig() as table).enableCelebrations then`
- `getPlaformDeviceSpec : function() : PlatformDeviceSpec`  (call as `api.util.getPlaformDeviceSpec(…)`): Returns the platform device specs
    - returns: platform device spec
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/graphics_presets_util.tl:164`: `local deviceSpec = api.util.getPlaformDeviceSpec()`
- `getGraphicsPresetParam : function() : GraphicsPresetParam`  (call as `api.util.getGraphicsPresetParam(…)`): Returns the parameter required to determine the graphic preset

Hardware data for choosing the initial desktop graphics preset. On computers, the main menu and the "Reset to Default" button of the graphics settings pass it to `GraphicsPresetsUtil.getInitialGraphicsPresetDesktop`.
    - returns: the graphic preset param
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/main_menu.tl:514`: `setInitialGraphicsPresetDekstop(api.util.getGraphicsPresetParam())`
- `getVramBudgetStatus : function(predicted : boolean) : VramBudgetStatus`  (call as `api.util.getVramBudgetStatus(…)`): Returns how well the VRAM budgets fit into the VRAM with the current graphics settings

Tells how well the video memory budgets of the current graphics settings fit into the video memory (VRAM) of the graphics card. The graphics settings page calls it with `true` and shows the result next to the texture, terrain texture and geometry quality.
    - param `predicted`: true: predicted from the graphics settings and the graphics card alone. false: into the VRAM available right now, which also depends on the loaded map and on what other applications use.
`true` predicts the status from the graphics settings and the graphics card alone. `false` checks against the VRAM available right now, which also depends on the loaded map and on other applications.
    - returns: the VRAM budget status
A `VramBudgetStatus` value. The settings page shows no indicator for `Unknown`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:1423`: `return api.util.getVramBudgetStatus(true)`
- `setAppConfig : function(appConfig : AppConfig, previewAudio : boolean)`  (call as `api.util.setAppConfig(…)`): Sets the game settings (does not save)
    - param `appConfig`: game settings to set
    - param `previewAudio`: if should play an audio preview for audio settings that changed
    - used 8x in the base game, e.g. `base/content/gui/gui/menu/main_menu.tl:66`: `api.util.setAppConfig(appConfig, false)`
- `setMusicVolume : function(volume : number)`  (call as `api.util.setMusicVolume(…)`): Set the music volume for the game
    - param `volume`: the music volume between 0 and 1
    - used 3x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:57`: `api.util.setMusicVolume(sliderToGain(value))`
- `getLanguage : function() : table`  (call as `api.util.getLanguage(…)`): Returns the current language settings
    - returns: table language settings
- `getMemoryStats : function() : table`  (call as `api.util.getMemoryStats(…)`): Returns some memory statistics.
- `setAllowSharing : function(allowSharing : boolean)`  (call as `api.util.setAllowSharing(…)`): Sets whether the game can be shared or not
    - used 4x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:1223`: `api.util.setAllowSharing(false)`

### ApiUtil.lang (record)

```teal
record lang
```
Access: `api.util.lang`

Locale-aware string comparison and number formatting (`api.util.lang`).

Example:
```lua
local text = api.util.lang.formatInt(1234567)
local minutes = api.util.lang.formatIntWithWidth(7, 2)  -- music player: two-digit minutes
```

- `compareStrings : function(a : string, b : string) : integer`  (call as `api.util.lang.compareStrings(…)`): Tests if a string is equal to another
    - param `a`: a string to test
    - param `b`: another string to test
    - returns: -1 if the first string has a lower value, 0 if both strings are equal, 1 if the first string has a greater
- `stringContains : function(text : string, test : string) : boolean`  (call as `api.util.lang.stringContains(…)`): Tests if a string contains another string
    - param `text`: string to test against
    - param `test`: string to test for
    - returns: true if the first string contains the second
- `formatInt : function(number : integer) : string`  (call as `api.util.lang.formatInt(…)`): Formats an integer with the correct locale.
    - param `number`: number to format
    - returns: string with formatted number
    - used 2x in the base game, e.g. `base/content/mission/mission/tutorial/tasks/camera_movement.tl:47`: `ctx.genericState.subTasksProgress[3] = { text = api.util.lang.formatInt(math.floor(turnprogress)) .. "/" .. api.util.lang.formatInt(targetTurn) .. " " .. _("deg`
- `formatIntWithWidth : function(number : integer, width : integer) : string`  (call as `api.util.lang.formatIntWithWidth(…)`): Formats an integer with the correct locale.
    - param `number`: number to format
    - param `integer`: width to format
    - returns: string with formatted number
    - used 2x in the base game, e.g. `base/content/gui/gui/music_player/music_player.tl:164`: `minutes = api.util.lang.formatIntWithWidth(minutes, 2),`
- `formatNumber : function(number : number, decimals : integer) : string`  (call as `api.util.lang.formatNumber(…)`): Formats a decimal number with the correct locale and given number of digits.
    - param `number`: number to format
    - param `decimals`: number of decimals to display
    - returns: string with formatted number

### ApiUtil.UtilDebug (record)

```teal
record UtilDebug
```

Debug helpers; `draw` holds functions to draw and clear debug points in the world.

- `draw : Draw`: The `Draw` debug drawing functions.

#### ApiUtil.UtilDebug.Draw (record)

```teal
record Draw
```

Debug drawing of coloured points at world positions, organised in layers (bit mask).

- `clear : function(layers : integer)`: Clear all debug draw points of the given layer.
    - param `layers`: the layer mask to clear
- `point : function(pos : Vec3f, color : Vec3f, layers : integer)`: Draw a point at the given position with the given color in the given layer.
    - param `pos`: the world position of the point
    - param `color`: the color of the point
    - param `layers`: the layer mask to draw on
