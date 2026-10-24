# gui.main.tile_list_react_util

Definition file: `base/tealdef/gui/main/tile_list_react_util.d.tl` (in the game folder).

Tile lists: grids of tiles with an image, a title, info icons and buttons, as used for save
games and mods in the main menu.

## TileListReactUtil (record)

```teal
global record TileListReactUtil
```
Load: `local tile_list_react_util = ug_require "::/gui/main/tile_list_react_util.tl" as TileListReactUtil`

Tile grid and tile recipes.

- `TileList : Recipe<TileListParams>`: Arranges tiles in rows of `numCols`.
    - used 4x in the base game, e.g. `base/content/gui/gui/menu/load_game_page.tl:197`: `tile_list_react_util.TileList {`
- `TileElement : Recipe<TileElementParams>`: A tile with title, image, info row and buttons; `IA_CONTEXT_HELP` clicks the details button.
  In game: "Details" (German: „Details“)
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:449`: `local tileElement = tile_list_react_util.TileElement{`

### TileListReactUtil.TileListParams (record)

```teal
record TileListParams
```

Parameters of `TileList`.

- `elements : {TreeNodeId}`: The tiles, filled row by row.
- `numCols : integer`: Number of tiles per row.
- `numRows : integer`: Minimum number of rows (empty rows are added when there are fewer tiles).

### TileListReactUtil.TileElementParams (record)

```teal
record TileElementParams
```

Parameters of `TileElement`.

- `title : string`: Title of the tile.
- `tileTooltip : string`: Tooltip of the whole tile.
- `infoIcons : {InfoIconParams}`: Entries of the info row.
- `detailsAsPrimaryButton : boolean`: Styles the details (gear) button as primary button (mouse mode only).
- `onClickMain : function()`: optional
- `onClickDetails : function()`: optional
- `onDoubleClickMain : function()`: optional
- `createImage : function() : TreeNodeId`: Builds the image of the tile.
- `createButtons : function() : {TreeNodeId}, function()`: Builds the buttons of the tile; a details (gear) button is appended when there is at least one.
    - returns: The buttons and an optional click handler used when `onClickMain` or `onClickDetails` is not set.
- `calloutContainerRef : ReactRefWrapApi0<CalloutApi>`: Callout container for the callout of a critical info entry.

#### TileListReactUtil.TileElementParams.InfoIconParams (record)

```teal
record InfoIconParams
```

An icon with a label in the info row of a tile.

- `path : string`: Icon path; `nil` for a label without icon.
- `label : string`: Text next to the icon.
- `tooltip : string`: Tooltip of the icon and label; for a critical entry it is shown in a callout instead.
- `iconTooltip : string`: Tooltip of the icon alone.
- `labelTooltip : string`: Tooltip of the label alone.
- `secondary : boolean`: Places the entry on the right side of the info row instead of the left.
- `critical : boolean`: true ==> ERROR, false ==> WARNING, nil ==> no problem
