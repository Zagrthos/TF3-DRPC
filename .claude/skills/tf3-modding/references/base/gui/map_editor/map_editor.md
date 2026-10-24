# gui.map_editor.map_editor

Definition file: `base/tealdef/gui/map_editor/map_editor.d.tl` (in the game folder).

Windows of the map editor for generating or importing the height map, generating towns and industries and generating main connections, loaded with `ug_require "/gui/map_editor/map_editor.tl"`.

## MapEditor (record)

```teal
global record MapEditor
```
Load: `local map_editor = ug_require "::/gui/map_editor/map_editor.tl" as MapEditor`

The map editor tools.

- `MapEditorTool : ReactToolDefinition<MapEditor.MapEditorToolParams>`: Tool that shows one of the map editor windows, selected by `variant`. The windows are created beforehand by `prepareWindows` and only shown and hidden.
    - used 3x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar.tl:41`: `getToolStackApi(gameCtx).pop(map_editor.MapEditorTool, nil)`
- `prepareWindows : function(gameCtx : GameContext, MapEditor.MapEditorConfig)`  (call as `map_editor.prepareWindows(…)`): for performance reasons, the windows are created initially and reused later
    - used 1x in the base game, e.g. `base/content/gui/gui/main/game.tl:366`: `map_editor.prepareWindows(gameCtx, toolParam)`

### MapEditor.MapEditorToolVariant (enum)

```teal
enum MapEditorToolVariant
```

Which map editor window the tool shows.

- `"map-editor-generate-heightmap"`: "Generate Heightmap" window, with an export tab if `hasHeightMapExporter` is set.
  In game: "Generate Heightmap", "Generate", "Export" (German: „Heightmap erstellen“, „Generieren“, „Export“)
- `"map-editor-import-heightmap"`: "Import Heightmap" window with height map and biome import tabs (requires `hasHeightMapImporter`).
  In game: "Import Heightmap", "Heightmap", "Biomes" (German: „Heightmap importieren“, „Heightmap“, „Biome“)
- `"map-editor-generate-townsIndustries"`: "Towns / Industries" window with generator tabs (if enabled) and import and export tabs.
  In game: "Towns / Industries", "Towns", "Industries" (German: „Städte / Industrien“, „Städte“, „Industrien“)
- `"map-editor-generate-street"`: "Main Connections" window for generating the main street connections.
  In game: "Main Connections" (German: „Hauptverbindungen“)

### MapEditor.MapEditorConfig (record)

```teal
record MapEditorConfig
```

Configuration passed to `prepareWindows`.

- `mapEditorGlobalStateRef : ReactRefT<MapEditor.GlobalState>`: State shared by all map editor windows.
- `hasHeightMapImporter : boolean`: Whether height maps can be imported (map editor, not on consoles).
- `hasHeightMapExporter : boolean`: Whether the height map can be exported (map editor, not on consoles).
- `hasTownGenerator : boolean`: Whether the town generator tab is shown.
- `hasIndustryGenerator : boolean`: Whether the industry generator tab is shown.
- `hasTownIndustryImporter : boolean`: Whether the import tab for towns and industries is shown. The map editor sets it together with `hasIndustryGenerator`.
- `hasTownIndustryExporter : boolean`: Whether the export tab for towns and industries is shown. The map editor sets it together with `hasIndustryGenerator`.

### MapEditor.GlobalState (record)

```teal
record GlobalState
```

State shared by the map editor windows.

- `operationInProgress : string`: Key of the editor tab whose operation (for example a generation) is running; while set, the other tabs are unavailable. `nil` when idle.

### MapEditor.MapEditorToolParams (record)

```teal
record MapEditorToolParams is ToolReactUtil.IToolParam
```

Tool parameters of `MapEditorTool`.

- `variant : MapEditorToolVariant`: Window to show.
- `mapEditorGlobalStateRef : ReactRefT<MapEditor.GlobalState>`: State shared by all map editor windows.
- `hasHeightMapImporter : boolean`: Whether height maps can be imported.
- `hasHeightMapExporter : boolean`: Whether the height map can be exported.
- `hasTownGenerator : boolean`: Whether the town generator tab is shown.
- `hasIndustryGenerator : boolean`: Whether the industry generator tab is shown.
- `hasTownIndustryImporter : boolean`: Whether the import tab for towns and industries is shown.
- `hasTownIndustryExporter : boolean`: Whether the export tab for towns and industries is shown.
