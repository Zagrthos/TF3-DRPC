# gui.layers.layers_button_ridge

Definition file: `base/tealdef/gui/layers/layers_button_ridge.d.tl` (in the game folder).

The layers button and its expandable ridge of layer buttons (terrain, towns, traffic, flows, noise, pollution, HUD filters), plus the parameter types shared by all layer windows.

## LayersButtonRidge (record)

```teal
global record LayersButtonRidge
```

The layers button with its ridge of layer buttons and the types shared by the layer windows.

- `ExpandableLayerButton : Recipe<LayerButtonsParam>`: Recipe of the layers button that expands into the ridge of layer buttons.
    - notes: Listens to the events `openLayerRidge` (payload `OpenLayerRidgeEventParams`), `closeLayerRidge`, `closeAllWindows` and `constructionMenuActive`. No layer buttons are shown while `gameCtx.disableFeatures["Layers"]` is set.

### LayersButtonRidge.LayersMode (enum)

```teal
enum LayersMode
```

How a layer is opened from the ridge.

- `"StandaloneTool"`: The layer is pushed as a tool on the tool stack.
- `"ConstructionMenuAssist"`: The layer window is opened without pushing a tool, used while the construction menu is active.

### LayersButtonRidge.LayerButtonsParam (record)

```teal
record LayerButtonsParam
```

Parameters of `ExpandableLayerButton`.

- `layersEnabled : {ButtonKey : boolean}`: Which layer buttons are shown; in the map editor the base game enables only Terrain, Civilization and HUDFilter.
- `gameCtx : GameContext`: Game GUI context.

#### LayersButtonRidge.LayerButtonsParam.ButtonKey (enum)

```teal
enum ButtonKey
```

Keys of the layer buttons in the ridge.

- `"Terrain"`: Terrain layer.
- `"Civilization"`: Towns layer.
- `"Infrastructure"`: Infrastructure layer.
- `"Traffic"`: Traffic layer (`LayerTraffic`).
- `"Satisfaction"`: Passenger Satisfaction layer (`LayerPublicTransport`).
- `"DeliveryOnTime"`: Cargo Satisfaction layer (`LayerCargo`).
- `"PassengerFlow"`: Passenger Flow layer.
- `"CargoFlow"`: Cargo Flow layer (`LayerCargoFlow`).
- `"Noise"`: Noise layer (`LayerNoise`).
- `"Pollution"`: Pollution layer (`LayerPollution`).
- `"HUDFilter"`: HUD Filters window (`HUDFilter`).

### LayersButtonRidge.LayerContentParam (record)

```teal
record LayerContentParam is ToolReactUtil.ToolWindowParamPayload<LayerToolParam>
```

Parameter of every layer content recipe (the `Layer` field of the layer modules); carries the `LayerToolParam` as `toolParam`.

- `meta : Meta`: Node meta data; the layer recipes set a tag such as `menu.layers.noise` on it.

### LayersButtonRidge.LayerToolParam (record)

```teal
record LayerToolParam is ToolReactUtil.IToolParam
```

Tool parameter of an open layer window, created by the ridge for the selected button.

- `data : Data`: Window title, style class and content recipe of the layer.
- `onBack : function()`: Called when the layer is left; forwards to the ridge's `onBack` only if the layer was opened with tool stacking.
- `switchToPrevious : function()`: Switches to the previous layer in the ridge (used for `IA_NAV_UP` in the layer windows).
- `switchToNext : function()`: Switches to the next layer in the ridge (used for `IA_NAV_DOWN` in the layer windows).
- `reduce : function()`: Closes the layer (calls `onBack` and pops the layer tool); in gamepad mode it also closes the ridge.
- `onMount : function(ReactRefWrap)`: Layer windows call it with their own node reference when they mount.

#### LayersButtonRidge.LayerToolParam.Data (record)

```teal
record Data
```

What the layer window shows.

- `title : string`: Window title.
- `class : string`: Style class added to the window.
- `content : Recipe<LayerContentParam>`: Recipe of the window content, e.g. `LayerNoise.Layer`.
- `gameCtx : GameContext`: Game GUI context.

### LayersButtonRidge.HudIconUserParam (interface)

```teal
interface HudIconUserParam is HudIconToolbox.HudIconMasterUserParam
```

User parameter of the layer HUD icon master recipes; same as `HudIconToolbox.HudIconMasterUserParam` (the layers fill `accessNotificationCacheFn`).

### LayersButtonRidge.OpenLayerRidgeEventParams (record)

```teal
record OpenLayerRidgeEventParams
```

Payload of the `openLayerRidge` event, which expands the layer ridge and optionally opens one layer.

Example:
```lua
-- from game_mechanics/towns/town_util.tl
onClickEvent = {"openLayerRidge", { layer = "menu.layers.trafficButton", stack = true }},
```

- `layer : string`: Button id of the layer to open, e.g. `"menu.layers.noiseButton"`.
- `stack : boolean`: If true, the layer tool is stacked on top of the current tool instead of replacing it.

### LayersButtonRidge.PreferredLayerConfigEvent (record)

```teal
record PreferredLayerConfigEvent
```

Payload of the `preferredLayerConfig` event that layer windows fire when they open (`nil` when closed); the construction tools use the config instead of their own.

- `config : LayerConfig`: Layer config of the open layer.
