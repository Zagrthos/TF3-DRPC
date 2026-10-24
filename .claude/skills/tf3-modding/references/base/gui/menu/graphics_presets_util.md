# gui.menu.graphics_presets_util

Definition file: `base/tealdef/gui/menu/graphics_presets_util.d.tl` (in the game folder).

Graphics quality presets (desktop and console) and helpers to detect or apply a preset on an `AppConfig`.

## GraphicsPresetsUtil (record)

```teal
global record GraphicsPresetsUtil
```
Load: `local graphics_presets_util = ug_require "::/gui/menu/graphics_presets_util.tl" as GraphicsPresetsUtil`

Util table of `graphics_presets_util.tl`.

- `getGraphicPreset : function(presets : {{string : {any, any}}}, config : AppConfig) : integer`  (call as `graphics_presets_util.getGraphicPreset(…)`): Returns the index of the first preset whose values all match the given config.
    - param `presets`: Preset list, e.g. from `getDesktopPresets()` or `getConsolePresets()`.
    - param `config`: Configuration to compare, usually `api.util.getAppConfig()`.
    - returns: Index of the matching preset, or `#presets + 1` ("custom") when none matches. Numbers are compared with a tolerance of 0.001.
    - used 4x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:801`: `local graphicPreset = graphics_presets_util.getGraphicPreset(desktopPresetSettings, appConfig)`
- `setGraphicPreset : function(presets : {{string : {any, any}}}, config : AppConfig, option : integer, inGame : boolean)`  (call as `graphics_presets_util.setGraphicPreset(…)`): Writes the values of one preset into an `AppConfig` object.
    - param `presets`: Preset list, e.g. from `getDesktopPresets()` or `getConsolePresets()`.
    - param `config`: Config object to change, e.g. `api.type.AppConfig.new(api.util.getAppConfig())`; it still has to be applied with `api.util.setAppConfig`.
    - param `option`: Index of the preset. `#presets + 1` (custom) does nothing.
    - param `inGame`: When true, entries whose second element is true are skipped.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:920`: `graphics_presets_util.setGraphicPreset(desktopPresetSettings, config, graphicsPresets, inGame)`
- `getDesktopPresets : function() : {{string : {any, any}}}`  (call as `graphics_presets_util.getDesktopPresets(…)`): Returns the four desktop presets, in order low, medium, high, very high.
    - returns: Preset list covering `textureQuality`, `terrainTextureQuality`, `numMSAASamples`, `useSsao`, `ssrQuality`, `shadowQuality`, `geometryQuality`, `terrainQuality` and `grassQuality`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:800`: `local desktopPresetSettings = graphics_presets_util.getDesktopPresets()`
- `getConsolePresets : function() : {{string : {any, any}}}`  (call as `graphics_presets_util.getConsolePresets(…)`): Returns the two console presets, quality (30 Hz) and performance (60 Hz), merged with the base settings of the console generation reported by `api.util.getPlaformDeviceSpec()`.
    - returns: Preset list; besides the quality settings it also sets `refreshRate`, `resolutionScale`, `terrainTesselationQuality` and `useReflection`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:1035`: `local consolePresetsSettings = graphics_presets_util.getConsolePresets()`
- `getInitialGraphicsPresetDesktop : function(param : GraphicsPresetParam, appConfig : AppConfig)`  (call as `graphics_presets_util.getInitialGraphicsPresetDesktop(…)`): Writes the initial desktop preset into `appConfig`. With a dedicated graphics card, `vram` selects medium from 7000, high from 11000 and very high from 15000 (low below 7000); without one the preset is low.
    - param `param`: Hardware data from `api.util.getGraphicsPresetParam()`.
    - param `appConfig`: Config object to change, e.g. `api.type.AppConfig.new(api.util.getAppConfig())`; it still has to be applied with `api.util.setAppConfig`.
    - notes: Also sets `resolutionScale` from the screen height `screenExtent[2]`: 0.75 from 2160 pixels, 0.8 from 1800, 0.85 from 1600, 0.9 from 1440, otherwise 1.0. The main menu calls it on the `setInitialGraphicsPreset` event and then applies the config. The graphics tab of the settings uses it for "Reset to Default". Both use `getInitialGraphicsPresetConsole` instead when `api.util.isConsoleMode()` is true.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:1545`: `graphics_presets_util.getInitialGraphicsPresetDesktop(api.util.getGraphicsPresetParam(), copy)`
- `getInitialGraphicsPresetConsole : function(appConfig : AppConfig)`  (call as `graphics_presets_util.getInitialGraphicsPresetConsole(…)`): Writes the console preset quality (the first entry of `getConsolePresets()`) into `appConfig`, including the base settings of the console generation.
    - param `appConfig`: Config object to change; it still has to be applied with `api.util.setAppConfig`.
    - used 1x in the base game, e.g. `base/content/gui/gui/menu/settings_page.tl:1543`: `graphics_presets_util.getInitialGraphicsPresetConsole(copy)`
