# gui.main.game_windows

Definition file: `base/tealdef/gui/main/game_windows.d.tl` (in the game folder).

Parameter types of some in-game windows and tools, notably the context help window and the construction tool.

## UILoansWindowParam (record)

```teal
global record UILoansWindowParam
```

- `onClose : function()`

## UIMarketingWindowParam (record)

```teal
global record UIMarketingWindowParam
```

- `onClose : function()`
- `townEntityId : Engine.Entity`

## ContextHelperWindowParam (record)

```teal
global record ContextHelperWindowParam
```

Parameters of the context help window (`gui/context_helper/context_helper_react.tl`), which the game bar opens as a singleton window.

- `onClose : function()`: Called when the window is closed.
- `gameCtx : GameContext`: The in-game GUI context.
- `startWithWelcome : boolean`

## ConstructionToolParam (record)

```teal
global record ConstructionToolParam is ToolReactUtil.IToolParam
```

Tool parameters of the construction menu tool (`construction.ConstructionTool` in `gui/construction/construction.tl`).

Notes:

`gui/main/game.tl` sets most flags from the game mode: map editor only (`hasHeightMapGenerator`, `hasTownGenerator`,
`hasIndustryGenerator`), map editor except on consoles (`hasHeightMapImporter`, `hasHeightMapExporter`),
map editor or sandbox (`hasHeightMapTextureTool`).

- `terrainToolData : TerrainToolData`: Terrain tool size limits.
- `hasHeightMapGenerator : boolean`: Whether height map generator tools are available (map editor).
- `hasHeightMapImporter : boolean`: Whether a height map can be imported (map editor, not on consoles).
- `hasHeightMapExporter : boolean`: Whether the height map can be exported (map editor, not on consoles).
- `hasHeightMapTextureTool : boolean`: Whether the terrain height map texture tools are available (map editor and sandbox mode).
- `hasTownGenerator : boolean`: Whether the town generator is available (map editor).
- `hasIndustryGenerator : boolean`: Whether the industry generator is available (map editor).
- `bulldozerHudIconFilter : {Engine.ComponentType}`: Component types whose HUD icons are shown while the bulldozer category is active. Must not change at runtime.
- `onBack : function()`
- `tabKey : string`: Id of the construction menu category (tab) the tool was pushed for; returned as the tool's active variant.
- `onPop : function()`: Called when the tool is popped, except when it is popped because the menu switches to another tab.
- `popDueToPushRef : ReactRefT<boolean>`: Set to `true` by the construction menu while it switches tabs by pushing a new tool, so that `onPop` is not called for the replaced one.

### ConstructionToolParam.TerrainToolData (record)

```teal
record TerrainToolData
```

Size limits of the terrain tools, taken from the base config.

- `minSize : number`: Minimum brush size, from `baseConfig.terrainPainterMinSize`.
- `maxSize : number`: Maximum brush size, from `baseConfig.terrainToolMaxSizeEditor` in the map editor and `baseConfig.terrainToolMaxSize` otherwise.
- `scale : number`: Terrain tool scale, from `baseConfig.terrainToolScale`.

## ConstructionToolWindowParam (record)

```teal
global record ConstructionToolWindowParam is ToolReactUtil.IToolWindowParam<ConstructionToolParam>
```

Parameters of the construction menu window (`ConstructionListWindow`). The payload carries the `ConstructionToolParam`.
