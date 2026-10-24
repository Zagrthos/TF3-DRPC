# gui.layers.layer_colors

Definition file: `base/tealdef/gui/layers/layer_colors.d.tl` (in the game folder).

Type of the GUI resource `::/gui/layers/layer_colors.gres` (defined in `layer_colors.gres.lua`): the colors every map layer uses. Read it with `api.gui.genericRep.get(api.gui.genericRep.find("::/gui/layers/layer_colors.gres")).data`.

## LayerColors (record)

```teal
global record LayerColors
```

Content of the GUI resource `::/gui/layers/layer_colors.gres`, the colors of all map layers.

Notes:

Colors are converted with `ColorUtil` (`toVec4`, `toRange4`, `toArray4`) before they go into a `LayerConfig`.
Most layers use the same fallback: light gray surfaces (`gray80`) and dark gray details (`gray40`).

Example:
```lua
local layerColors = api.gui.genericRep.get(api.gui.genericRep.find("::/gui/layers/layer_colors.gres")).data
colorPassFn.fallbackColor = layer_react_util.createDefaultFallbackColor(layerColors.Traffic.Fallback.Surface, layerColors.Traffic.Fallback.Details)
```

- `Cargo : CargoConfig`: Colors of the Cargo Satisfaction layer (`LayerCargo`).
- `PublicTransport : PublicTransportConfig`: Colors of the Passenger Satisfaction layer (`LayerPublicTransport`).
- `CargoFlow : CargoFlowConfig`: Colors of the Cargo Flow layer (`LayerCargoFlow`).
- `Civilization : CivilizationColors`: Colors of the Towns layer.
- `Infrastructure : InfrastructureColors`: Colors of the Infrastructure layer.
- `PassengerFlow : PassengerFlowConfig`: Colors of the Passenger Flow layer.
- `Noise : NoiseColors`: Colors of the Noise layer (`LayerNoise`).
- `Pollution : PollutionColors`: Colors of the Pollution layer (`LayerPollution`).
- `Terrain : TerrainConfig`: Colors of the Terrain layer.
- `Traffic : TrafficColors`: Colors of the Traffic layer (`LayerTraffic`).
- `Construction : ConstructionColors`: Layer colors used by the construction menu.

### LayerColors.Fallback (record)

```teal
record Fallback
```

Fallback colors of a layer, passed to `LayerReactUtil.createDefaultFallbackColor`.

- `Surface : GResColor`: Color of terrain, water and other surfaces.
- `Details : GResColor`: Color of edges, nodes and parallel strips.

### LayerColors.SignConfig (record)

```teal
record SignConfig
```

Settings of the labels (signs) a layer draws along the network.

- `RenderDistance : number`: Value for `signRenderDistance` of the layer's `LayerConfig.SignConfig` (1500 for infrastructure and flow layers, 0 for the satisfaction layers).
- `BackgroundColor : GResColor`: Background color of the signs, applied through the layer stylesheet (`layers.css.lua`).
- `Color : GResColor`: Text color of the signs, applied through the layer stylesheet (`layers.css.lua`).

### LayerColors.CargoConfig (record)

```teal
record CargoConfig
```

Colors of the Cargo Satisfaction layer.

- `Sign : SignConfig`: Sign settings of the layer.
- `Voronoi : GResColorArray`: Colors of the town areas (Voronoi regions) drawn by the layer.
- `Fallback : Fallback`: Fallback colors of the layer.

### LayerColors.PublicTransportConfig (record)

```teal
record PublicTransportConfig
```

Colors of the Passenger Satisfaction layer.

- `Sign : SignConfig`: Sign settings of the layer.
- `Voronoi : GResColorArray`: Colors of the town areas (Voronoi regions) drawn by the layer.
- `Fallback : Fallback`: Fallback colors of the layer.

### LayerColors.CargoFlowConfig (record)

```teal
record CargoFlowConfig
```

Colors of the Cargo Flow layer and of the cargo flow arrows drawn in the 3D view.

- `PlayerTransport : GResColorRange`: Color range for cargo transported by the player.
- `Sign : SignConfig`: Sign settings of the layer.
- `Fallback : Fallback`: Fallback colors of the layer.
- `Suppliers3D : Color3D`: Arrow colors for flows from suppliers into an industry (shown in the industry window).
- `Consumers3D : Color3D`: Arrow colors for flows from an industry to its consumers; also used by the cargo and passenger delivery subventions.

#### LayerColors.CargoFlowConfig.Color3D (record)

```teal
record Color3D
```

Arrow colors of cargo flows in the 3D view (`CargoFlowRenderableConfig.FlowData.arrowColors`).

- `Actual : GResColorArray`: Arrow colors for flows that currently carry cargo.
- `Potential : GResColorArray`: Arrow colors for possible flows without cargo (a washed-out version of `Actual`).

### LayerColors.CivilizationColors (record)

```teal
record CivilizationColors
```

Colors of the Towns layer.

- `Industry : GResColor`: Color of industries in the Towns layer.
- `Voronoi : GResColorArray`: Colors of the town areas (Voronoi regions).
- `Fallback : Fallback`: Fallback colors of the layer.

### LayerColors.InfrastructureColors (record)

```teal
record InfrastructureColors
```

Colors of the Infrastructure layer.

- `Speed : {GResColor, GResColor, GResColor}`: Three colors from slow to fast for speed limits on the network (also used, more transparent, for underground edges).
- `Tunnel : GResColor`: Outline color of tunnels.
- `Sign : SignConfig`: Settings of the speed limit signs.
- `Fallback : Fallback`: Fallback colors of the layer.

### LayerColors.PassengerFlowConfig (record)

```teal
record PassengerFlowConfig
```

Colors of the Passenger Flow layer.

- `Origin : GResColorRange`: Color range for origins of passenger trips.
- `Destination : GResColorRange`: Color range for destinations of passenger trips.
- `PrivateTransport : GResColorRange`: Color range for trips by private transport.
- `PublicTransport : GResColorRange`: Color range for trips by the player's public transport.
- `Sign : SignConfig`: Sign settings of the layer.
- `Fallback : Fallback`: Fallback colors of the layer.

### LayerColors.NoiseColors (record)

```teal
record NoiseColors
```

Colors of the Noise layer.

- `Impact : GResColorRange`: Color range for town buildings affected by noise.
- `Emission : GResColorRange`: Color range for noise emitters such as vehicles.
- `EmissionGood : GResColorRange`: Color range for emitters with a negative (reducing) noise value.
- `Average : GResColorArray`: Colors of the averaged noise map on the terrain.
- `Fallback : Fallback`: Fallback colors of the layer.

### LayerColors.PollutionColors (record)

```teal
record PollutionColors
```

Colors of the Pollution layer.

- `Emission : GResColorRange`: Color range for pollution emitters.
- `EmissionGood : GResColorRange`: Color range for emitters that reduce pollution.
- `Average : GResColorArray`: Colors of the averaged pollution map on the terrain.
- `Voronoi : GResColorArray`: Colors of the town areas (Voronoi regions).
- `Fallback : Fallback`: Fallback colors of the layer; the surface color is also used for town buildings.

### LayerColors.TerrainConfig (record)

```teal
record TerrainConfig
```

Colors and contour lines of the Terrain layer.

- `Contours : {Contour}`: Contour lines used with metric units (every 50, 10 and 2 metres).
- `ContoursImperial : {Contour}`: Contour lines used with imperial units (every 100, 25 and 5 feet, given in metres).
- `Base : GResColor`: Base color of the terrain.
- `Fallback : Fallback`: Fallback colors of the layer.
- `TownBuilding : GResColor`: Color of town buildings.
- `Vehicle : GResColor`: Color of vehicles.
- `Industry : GResColor`: Color of industries.
- `Station : GResColor`: Color of stations.
- `Water : GResColor`: Color of water.
- `NavigableWater : GResColor`: Color of navigable water.
- `HeightMap : GResColorArray`: Colors of the height map, from low to high (thresholds 0, 150, 300 and 450 in the base game).

#### LayerColors.TerrainConfig.Contour (record)

```teal
record Contour
```

One set of contour lines on the terrain.

- `color : GResColor`: Line color.
- `level : integer`: <length unit> over sea
- `thickness : number`: Line thickness (0.6 to 1.2 in the base game).
- `fadeDistance : number`: Camera distance at which the lines fade out; -1 means they never fade.

### LayerColors.TrafficColors (record)

```teal
record TrafficColors
```

Colors of the Traffic layer.

- `Fluid : GResColor`: Color of roads with flowing traffic (kept in sync with `GameplayColors` satisfaction "Happy").
- `Intermediate : GResColor`: Color of roads with dense traffic (kept in sync with satisfaction "Aloof").
- `Jammed : GResColor`: Color of jammed roads (kept in sync with satisfaction "Sad").
- `Tunnel : GResColor`: Outline color of tunnels.
- `Fallback : Fallback`: Fallback colors of the layer.

### LayerColors.ConstructionColors (record)

```teal
record ConstructionColors
```

Layer colors used by the construction menu.

- `Voronoi : GResColorArray`: Colors of the town areas (Voronoi regions) while building.
- `Fallback : Fallback`: Fallback colors while building, also used for the "white screen" view.
