# scripts.builtin

Definition file: `base/tealdef/scripts/builtin.d.tl` (in the game folder).

## Builtin (record)

```teal
global record Builtin
```

The built-in widget library returned by `ug_require "::/gui/main/builtin.lua"`: recipes for layouts,
text, images, buttons, inputs, windows, charts and the game's tool and renderer components. Most widget
parameters are engine classes, so a parameter table may only contain the declared fields (an unknown key
raises an error). Enumerations are in `builtin.type`.

- `type : Type`: The enumerations and helper classes (`builtin.type.Orientation`, `builtin.type.ScrollBarPolicy`, ...).
- `WithComponentParams : function(WithComponentParamsParam, ?any) : TreeNodeId`: Wraps a node with the settings above; used internally for `meta` fields.
- `FloatingLayout : Recipe<FloatingLayoutParam>`: Layout that stacks its children on top of each other and positions each one inside the layout.
- `FloatingLayoutChild : Recipe<FloatingLayoutChildParam>`: Wrapper that places one child in a floating layout.
- `AbsoluteLayout : function({TreeNodeId}) : TreeNodeId`
- `AbsoluteLayoutChild : Recipe<AbsoluteLayoutChildParam>`: Wrapper for one child of an absolute layout.
- `BoxLayout : Recipe<BoxLayoutParam>`: Layout that arranges its children in a row or a column.
- `FlowLayout : Recipe<FlowLayoutParam>`: Layout that places children in a row and wraps to the next line when the space runs out.
- `Chart : Recipe<ChartParam>`
- `Series : Recipe<SeriesParam>`: Data series of a chart.
- `List : Recipe<ListParam>`: List of selectable items.
- `ScrollArea : Recipe<ScrollAreaParam>`: Scrollable area around one content node.
- `Component : Recipe<ComponentParam>`: Container node around one layout; styled through its style classes.
- `TextView : Recipe<TextViewParam>`: Plain text label.
- `RichTextView : Recipe<RichTextViewParam>`: Formatted text (HTML or Markdown).
- `KeybindingHintDisplay : Recipe<KeybindingHintDisplayParam>`: Shows the key or gamepad button of an input action.
- `ParticleSystem2d : Recipe<ParticleSystem2dParam>`: 2D particle effect in the GUI.
- `ImageView : Recipe<ImageViewParam>`: Image, optionally an animated sprite sheet.
- `ShaderQuad : Recipe<ShaderQuadParam>`: Image drawn with a GUI shader (lighting, ripple and shine effects).
- `Button : Recipe<ButtonParam>`: Push button.
- `ToggleButton : Recipe<ToggleButtonParam>`: Button with an on and an off state.
- `ToggleButtonGroup : Recipe<ToggleButtonGroupParam>`: Group of toggle buttons of which one can be selected.
- `ToolButton : Recipe<ToolButtonParam>`: Button that opens and closes a tool (window or building mode).
- `DoubleSpinBox : RecipeWithApi<DoubleSpinBoxParam, DoubleSpinBoxApi>`: Number input with steps; offers `DoubleSpinBoxApi`.
- `TextInputField : Recipe<TextInputFieldParam>`: Single-line text input.
- `Slider : Recipe<SliderParam>`: Integer slider.
- `ColorPicker : Recipe<ColorPickerParam>`: Colour picker.
- `ColorChooser : Recipe<ColorChooserParam>`: Colour swatches.
- `ColorChooserButton : Recipe<ColorChooserButtonParam>`: Button that opens colour swatches.
- `GradientLegend : Recipe<GradientLegendParam>`: Colour legend, e.g. of map layers.
- `ProgressBar : Recipe<ProgressBarParam>`: Progress bar.
- `CheckBox : Recipe<CheckBoxParam>`: Check box with optional label; values are 0, 1 and -1 (indeterminate).
- `ComboBox : Recipe<ComboBoxParam>`: Drop-down list.
- `ComboBoxItem : Recipe<ComboBoxItemParam>`: Entry of a combo box.
- `NativeFileDialogButton : Recipe<NativeFileDialogButtonParam>`: Button that opens a file dialog.
- `DropComponent : Recipe<DropComponentParam>`: Area that accepts files dropped from outside the game.
- `RadialMenuChild : Recipe<RadialMenuChildParam>`: Entry of a radial menu.
    - used 1x in the base game, e.g. `base/content/gui/gui/main/radial_menu_extension.tl:6`: `radial_menu_extension.RadialMenuExtension = react.RegisterWrapperExtensionPoint0(getOwningModId(), "RadialMenuExtension", builtin.RadialMenuChild) as WrapperExt`
- `RadialMenu : Recipe<RadialMenuParam>`: Circular menu (gamepad).
- `TabWidget : Recipe<TabWidgetParam>`: Widget with tabs; the steps of hidden tabs are paused.
- `TabWidgetChild : Recipe<TabWidgetChildParam>`: One tab of a tab widget.
- `Row : Recipe<RowParam>`: Row of a table layout.
- `TableLayout : Recipe<TableLayoutParam>`: Layout that aligns rows of cells in columns.
- `ColumnDesc : Recipe<ColumnDescParam>`: Column description of a data table.
- `DataTable : RecipeWithApi<DataTableParam, DataTableApi>`: Sortable table with one cell recipe per column; offers `DataTableApi`.
- `Window : Recipe<WindowParam>`: Window with title bar; usually opened through the window container API or a tool.
- `WindowContainerDelegate : RecipeWithApi<WindowContainerDelegateParam, WindowAPI>`: The container that holds the game's windows; offers `WindowAPI`.
- `ToolStack : RecipeWithApi<ToolStackParam, ToolStackAPI>`: Stack of tools (windows, building modes); only the top tool is active, the others are shelved.
Offers `ToolStackAPI`.
- `HudIconManagerConfig : Recipe<HudIconManagerConfigParam<any>>`: Configures the HUD icons of a map layer.
- `UseDefaultHudIcon : Recipe<HudIconParam>`: Shows the game's default HUD icon for the entity.
- `HudIconChildConfig : Recipe<HudIconChildConfigParam>`: Wraps one HUD icon node.
- `RendererComponent : RecipeWithApi<RendererComponentParam, RenderComponentAPI>`: The 3D game view; offers `RenderComponentAPI`.
- `EntityRendererComponent : Recipe<EntityRendererComponentParams>`: View that renders a single entity.
- `EdgeRenderable : Recipe<EdgeRenderableParam>`: Draws coloured lines along edges in the 3D view.
- `MissionArrowRenderable : Recipe<MissionArrowRenderableParam>`: Draws mission arrows in the 3D view.
- `ActionDescriptor : Recipe<ActionDescriptorParam>`: Describes the current action of the 3D view.
- `ActionTooltip : function<T>(ActionTooltipParam<T>) : TreeNodeId`: Creates a tooltip shown at the cursor during an action.
- `ConstructionAction : Recipe<ConstructionActionParam>`: Building action in the 3D view (placing, modifying or bulldozing).
- `SimpleInputActions : Recipe<SimpleInputActionsParam>`: Registers simple input actions.
- `Selector : Recipe<SelectorParam>`: Entity selector in the 3D view.
- `FullScreenComponent : function<T>(FullScreenComponentParam<T>) : TreeNodeId`: Creates a full-screen overlay from a recipe and its parameter.
- `NodeViewer : Recipe<NodeViewerParam>`
- `LineViewer : Recipe<LineViewerParam>`: Draws lines and stops in the 3D view.
- `ProposalViewer : Recipe<ProposalViewerParam>`: Previews a building proposal in the 3D view.
- `MapPreviewComp : Recipe<MapPreviewCompParam>`: Map preview.
- `LayerConfig : Recipe<LayerConfig>`: Configures the active map layer.
- `RailroadCrossingComp : Recipe<RailroadCrossingCompParam>`
- `SignalComp : Recipe<SignalCompParam>`
- `ReactAdapterGuestContainer : RecipeWithApi0<ReactAdapterGuestContainerAPI>`

### Builtin.Type (record)

```teal
record Type
```

Enumerations and helper classes of the built-in widgets, reached as `builtin.type` (lower case).

#### Builtin.Type.Orientation (record)

```teal
record Orientation
```

Layout direction of box layouts and lists.

- `Horizontal : Orientation`: Children side by side.
- `Vertical : Orientation`: Children one below the other.

#### Builtin.Type.TabOrientation (record)

```teal
record TabOrientation
```

Side of a tab widget on which the tabs are placed.

- `North : TabOrientation`: Tabs above the content.
- `South : TabOrientation`: Tabs below the content.
- `East : TabOrientation`: Tabs right of the content.
- `West : TabOrientation`: Tabs left of the content.

#### Builtin.Type.ImageViewScaling (record)

```teal
record ImageViewScaling
```

How an image is scaled to the size of its view.

- `AutoFit : ImageViewScaling`
- `AutoZoom : ImageViewScaling`
- `Stretch : ImageViewScaling`: Stretches the image to the view size.

#### Builtin.Type.ScrollBarPolicy (record)

```teal
record ScrollBarPolicy
```

When a scroll area or list shows its scroll bar.

- `AlwaysOff : ScrollBarPolicy`: Never shows the scroll bar.
- `AlwaysOn : ScrollBarPolicy`: Always shows the scroll bar.
- `AsNeededButAlwaysReserveSpace : ScrollBarPolicy`: Shows the scroll bar when the content does not fit, and keeps its space free otherwise.
- `AsNeeded : ScrollBarPolicy`: Shows the scroll bar only when the content does not fit.
- `Simple : ScrollBarPolicy`

#### Builtin.Type.ListBehavior (record)

```teal
record ListBehavior
```

Selection behaviour of a list.

- `Default : ListBehavior`: Normal list selection.
- `AutoDeselectDisabledItems : ListBehavior`: Deselects items that become disabled.
- `Navigational : ListBehavior`

#### Builtin.Type.ToolTipPosition (record)

```teal
record ToolTipPosition
```

Where a tooltip appears relative to its widget.

- `Cursor : ToolTipPosition`: At the mouse cursor.
- `Left : ToolTipPosition`: Left of the widget.
- `Right : ToolTipPosition`: Right of the widget.
- `Below : ToolTipPosition`: Below the widget.
- `Above : ToolTipPosition`: Above the widget.
- `Center : ToolTipPosition`: Centred on the widget.

#### Builtin.Type.EdgeRenderable (record)

```teal
record EdgeRenderable
```

Types for `Builtin.EdgeRenderable`.

##### Builtin.Type.EdgeRenderable.Edge (record)

```teal
record Edge
```

A coloured line drawn along an edge geometry in the 3D view.

- `new : function(EdgeGeometry) : Edge`: Creates an edge for the given geometry.
- `geometry : EdgeGeometry`: The curve to draw.
- `colors : {Vec4f}`: Colours (RGBA) of the line.
- `width : number`: Line width.
- `offsetZ : number`: Height offset above the geometry.
- `stepSize : number`

#### Builtin.Type.MissionArrowRenderable (record)

```teal
record MissionArrowRenderable
```

Types for `Builtin.MissionArrowRenderable`.

##### Builtin.Type.MissionArrowRenderable.Arrow (record)

```teal
record Arrow
```

An arrow drawn in the 3D view to point at an entity or position (mission markers).

- `new : function() : Arrow`: Creates a new, empty `Arrow`.
- `entity : Engine.Entity`: Entity the arrow points at.
- `position : Vec3f`: Position the arrow points at.
- `scaling : number`: Size factor.
- `color : Vec4f`: Colour (RGBA).

#### Builtin.Type.ConstructionAction (record)

```teal
record ConstructionAction
```

Descriptor classes for `Builtin.ConstructionAction`; each one configures one kind of building tool.

##### Builtin.Type.ConstructionAction.ConstructionBuilder (record)

```teal
record ConstructionBuilder
```

Construction builder action descriptor.

- `new : function() : ConstructionBuilder`: Creates a new, empty `ConstructionBuilder`.
- `constructions : {ResName}`: Resource names of the constructions
- `constructionTemplate : integer`: Construction template (-1 for no template)
- `params : table`: Parameters of the construction
- `height : number`: Height of the construction.
- `rotation : number`: Rotation around the vertical axis, in radians.
- `rotationY : number`: Rotation around the Y axis, in radians.
- `rotationX : number`: Rotation around the X axis, in radians.
- `assetRandomize : boolean`: Randomize option for assets
- `builderAudioRes : {ResName}`: Set audio played when building
- `onRotationChangeFn : function(newRotation : number)`: Set rotation change function

##### Builtin.Type.ConstructionAction.StreetEdgeBuilder (record)

```teal
record StreetEdgeBuilder
```

Edge builder action descriptor.

- `new : function() : StreetEdgeBuilder`: Creates a new, empty `StreetEdgeBuilder`.
- `resName : ResName`: Resource name of the street
- `busLane : boolean`: Bus lane option
- `tramTrackType : TramTrackType`: Tram track type option
- `playerOwned : boolean`: Player owned option
- `curved : boolean`: Curved option
- `alignToTerrain : number`: Align to terrain option
- `enableSnapping : boolean`: Enable snapping option
- `level : number`: Level option
- `height : number`: Height option
- `slope : number | nil`: Slope option
- `bridgeTypeId : integer`: Bridge type ID
- `tunnelTypeId : integer`: Tunnel type ID
- `railroadCrossingTypeId : integer`: Railroad crossing type ID
- `undergroundMode : boolean`: Enable underground mode option
- `onClearFn : function()`: Set align to terrain function
- `onFixSlopeToggledFn : function(enabled : boolean)`: Set slope fixed
- `onHeightChangeFn : function(newHeight : number)`: Set height change function
- `onFollowTerrainEnabledFn : function(enabled : boolean)`: Set follow terrain enabled function
- `bridgeHeight : number`
- `bridgeMinHeight : number`
- `tunnelHeight : number`
- `tunnelMinHeight : number`
- `builderAudioRes : ResName`: Audio played while building.

##### Builtin.Type.ConstructionAction.ModifierColor (record)

```teal
record ModifierColor
```

Highlight colours (RGBA) of modifier tools.

- `new : function() : ModifierColor`: Creates a new, empty `ModifierColor`.
- `validSubtle : Vec4f`: Light highlight of a valid target.
- `validIntense : Vec4f`: Strong highlight of a valid target.
- `invalidSubtle : Vec4f`: Light highlight of an invalid target.
- `invalidIntense : Vec4f`: Strong highlight of an invalid target.
- `upgrade : Vec4f`: Highlight of an upgrade.
- `downgrade : Vec4f`: Highlight of a downgrade.
- `inProgress : Vec4f`: Highlight while the change is in progress.

##### Builtin.Type.ConstructionAction.StreetEdgeNodeModifier (record)

```teal
record StreetEdgeNodeModifier
```

Edge and node modifier action descriptor.

- `new : function() : StreetEdgeNodeModifier`: Creates a new, empty `StreetEdgeNodeModifier`.
- `resName : ResName`: Resource name of the street
- `busLane : boolean`: Bus lane option
- `tramTrackType : TramTrackType`: Tram track type option
- `playerOwned : boolean`: Player owned option
- `roadDevelopmentLocked : boolean`: Prevent towns from updating or extending this road
- `makePlayerOwnedOnChange : boolean`: Make player owned when changing something
- `symmetricLanes : boolean`: Symmetric lanes option
- `trafficLights : boolean`: Traffic lights option
- `overrideLaneConfigs : boolean`: Override lane configs option
- `overrideEdgeDecorations : boolean`: Override edge decoration options
- `undergroundMode : boolean`: Set underground mode
- `color : ModifierColor`: Set color
- `builderAudioRes : ResName`: Set audio played when building

##### Builtin.Type.ConstructionAction.TrackEdgeBuilder (record)

```teal
record TrackEdgeBuilder
```

Track edge builder action descriptor.

- `new : function() : TrackEdgeBuilder`: Creates a new, empty `TrackEdgeBuilder`.
- `resName : ResName`: Resource name of the track
- `catenary : boolean`: Build the track with catenary.
- `alignToTerrain : number`: Align to terrain option
- `enableSnapping : boolean`: Enable snapping option
- `level : number`: Level option
- `height : number`: Height option
- `slope : number | nil`: Slope option
- `bridgeTypeId : integer`: Bridge type ID
- `tunnelTypeId : integer`: Tunnel type ID
- `railroadCrossingTypeId : integer`: Railroad crossing type ID
- `undergroundMode : boolean`: Set underground mode
- `onClearFn : function()`: Set align to terrain function
- `onFixSlopeToggledFn : function(enabled : boolean)`: Set slope fixed
- `onHeightChangeFn : function(newHeight : number)`: Set height change function
- `onFollowTerrainEnabledFn : function(enabled : boolean)`: Set follow terrain enabled function
- `bridgeHeight : number`
- `bridgeMinHeight : number`
- `tunnelHeight : number`
- `tunnelMinHeight : number`
- `builderAudioRes : ResName`: Audio played while building.

##### Builtin.Type.ConstructionAction.TrackEdgeModifier (record)

```teal
record TrackEdgeModifier
```

Track edge modifier action descriptor.

- `new : function() : TrackEdgeModifier`: Creates a new, empty `TrackEdgeModifier`.
- `resName : ResName`: Resource name of the track
- `resNameRevert : ResName`: Resource name of the track to revert to
- `catenary : boolean`: Catenary option
- `highSpeed : boolean`: High speed option
- `edgeDecoration : integer`: Edge decoration option
- `overrideLaneConfigs : boolean`: Override lane configs option
- `overrideEdgeDecorations : boolean`: Override edge decoration options
- `undergroundMode : boolean`: Set underground mode
- `color : ModifierColor`: Set color
- `builderAudioRes : ResName`: Set audio played when building

##### Builtin.Type.ConstructionAction.EdgeObjectBuilder (record)

```teal
record EdgeObjectBuilder
```

Street terminal builder action descriptor.

- `new : function() : EdgeObjectBuilder`: Creates a new, empty `EdgeObjectBuilder`.
- `resName : ResName`: Resource name of the terminal
- `params : table`: Construction parameters of the edge object.
- `oneWay : boolean`: One way choice
- `undergroundMode : boolean`: Enable underground mode
- `builderAudioRes : ResName`: Audio played while building.

##### Builtin.Type.ConstructionAction.LaneModifier (record)

```teal
record LaneModifier
```

Lane modifier action descriptor.

- `new : function() : LaneModifier`: Creates a new, empty `LaneModifier`.
- `color : ModifierColor`: Highlight colours.
- `lineColors : {Vec3f}`: Colours of the lines.
- `tramMode : boolean`: Work on tram lanes.
- `undergroundMode : boolean`: Set underground mode
- `builderAudioRes : ResName`: Audio played while building.

##### Builtin.Type.ConstructionAction.CrosswalkModifier (record)

```teal
record CrosswalkModifier
```

Crosswalk modifier action descriptor.

- `new : function() : CrosswalkModifier`: Creates a new, empty `CrosswalkModifier`.
- `color : ModifierColor`: Highlight colours.
- `undergroundMode : boolean`: Set underground mode
- `builderAudioRes : ResName`: Audio played while building.

##### Builtin.Type.ConstructionAction.BrushParams (record)

```teal
record BrushParams
```

Brush parameters action descriptor.

- `new : function() : BrushParams`: Creates a new, empty `BrushParams`.
- `radius : number`: Radius of the brush
- `brushIndex : integer`: Index of the brush
- `opacity : number`: Opacity/strength of the brush
- `rotation : number`: Rotation of the brush
- `material : number`: Material of the brush

##### Builtin.Type.ConstructionAction.TerrainModifier (record)

```teal
record TerrainModifier
```

Terrain modifier action descriptor.

- `new : function() : TerrainModifier`: Creates a new, empty `TerrainModifier`.
- `mode : Mode`: Mode of the terrain modifier
- `brushParams : ConstructionAction.BrushParams`: Brush parameters
- `invert : boolean`: Invert option
- `heightMap : string`: Height map file path
- `builderAudioRes : ResName`: Audio played while modifying.

###### Builtin.Type.ConstructionAction.TerrainModifier.Mode (record)

```teal
record Mode
```

Terrain modifier mode

- `Raise : Mode`: Raises the terrain.
- `Lower : Mode`: Lowers the terrain.
- `Smooth : Mode`: Smooths the terrain.
- `Flatten : Mode`: Flattens the terrain.
- `Textured : Mode`

##### Builtin.Type.ConstructionAction.TerrainPainter (record)

```teal
record TerrainPainter
```

Terrain painter action descriptor.

- `new : function() : TerrainPainter`: Creates a new, empty `TerrainPainter`.
- `brushParams : ConstructionAction.BrushParams`: Brush parameters
- `slope : number`: Slope of the brush
- `slopeInvert : boolean`: Invert slope option
- `builderAudioRes : ResName`: Audio played while painting.

##### Builtin.Type.ConstructionAction.AssetBrush (record)

```teal
record AssetBrush
```

Asset brush action descriptor.

- `new : function() : AssetBrush`: Creates a new, empty `AssetBrush`.
- `brushParams : ConstructionAction.BrushParams`: Brush parameters
- `ignoreCollision : boolean`: Skip collision option
- `assets : {ResName}`: List of asset resource names
- `slope : number`: Slope of the brush
- `slopeInvert : boolean`: Invert slope option
- `randomRotation : boolean`: Random rotation option
- `eraseWhenEmpty : boolean`: When no assets are provided, this flag signals if assets should be erased instead
- `builderAudioRes : ResName`: Audio played while placing assets.

##### Builtin.Type.ConstructionAction.TownBuilder (record)

```teal
record TownBuilder
```

Town builder action descriptor.

- `new : function() : TownBuilder`: Creates a new, empty `TownBuilder`.
- `initialLandUseCapacity : {integer, integer, integer}`: Initial land use capacity
- `landUse2CargoNeeds : {{CargoTypeId | nil}, {CargoTypeId | nil}, {CargoTypeId | nil}}`: Land use to cargo needs mapping
- `initialMajorStreetAngleRange : number`: Initial major street angle range
- `capacityScalingFactor : number`: Factor on the land use capacity of the new town.
- `builderAudioRes : ResName`: Audio played while building.

##### Builtin.Type.ConstructionAction.Bulldozer (record)

```teal
record Bulldozer
```

Bulldozer action descriptor.

- `new : function() : Bulldozer`: Creates a new, empty `Bulldozer`.
- `undergroundMode : boolean`: Set to bulldozer underground mode
- `circleBorderColor : Vec4f`: Circle border color for bulldozer action
- `circleFillColor : Vec4f`: Circle fill color for bulldozer action
- `circleBorderWidth : number`: Circle border width for bulldozer action
- `circleAnimEasing : Easing`: Circle animation easing for bulldozer action
- `circleAnimDuration : number`: Circle animation duration for bulldozer action
- `crosshairColor : Vec4f`: Crosshair color for bulldozer action
- `builderAudioResFallback : ResName`: Audio played when the bulldozed object defines none.

##### Builtin.Type.ConstructionAction.ModuleBuilder (record)

```teal
record ModuleBuilder
```

Module builder action descriptor.

- `new : function() : ModuleBuilder`: Creates a new, empty `ModuleBuilder`.
- `constructionEntity : Engine.Entity`: Construction entity
- `moduleResName : ResName`: Module resource name
- `builderAudioRes : ResName`: Audio played while building.

##### Builtin.Type.ConstructionAction.ModuleBulldozer (record)

```teal
record ModuleBulldozer
```

Module bulldozer action descriptor.

- `new : function() : ModuleBulldozer`: Creates a new, empty `ModuleBulldozer`.
- `constructionEntity : Engine.Entity`: Construction entity
- `builderAudioResFallback : ResName`: Audio played when the bulldozed module defines none.

#### Builtin.Type.ControlPointInfo (record)

```teal
record ControlPointInfo
```

A draggable control point shown by the proposal viewer.

- `new : function(Vec3f, number, number, State) : ControlPointInfo`: Creates a control point from position, height offset, radius and state.
- `position : Vec3f`: Position of the point.
- `offsetZ : number`: Height offset.
- `radius : number`: Radius of the marker.
- `state : State`: Display state.

##### Builtin.Type.ControlPointInfo.State (record)

```teal
record State
```

Display state of a control point.

- `Invalid : State`
- `Normal : State`: Normal display.
- `Hover : State`: The mouse is over the point.
- `Dragging : State`: The point is being dragged.
- `Idle : State`
- `IdleOverEdge : State`

#### Builtin.Type.FloatingLayoutOverflowMode (record)

```teal
record FloatingLayoutOverflowMode
```

How a floating layout child may extend beyond its parent.

- `None : FloatingLayoutOverflowMode`
- `Overflow : FloatingLayoutOverflowMode`: this mode will ignore outerSpacing, borderWidth, padding but allow leaving the parent rect
- `KeepInside : FloatingLayoutOverflowMode`: The child is kept inside the parent rectangle.

#### Builtin.Type.InputActionBubbleUpFilter (record)

```teal
record InputActionBubbleUpFilter
```

Which input actions a window passes on to its parent, set with `WindowParam.inputActionBubbleUpFilter`.

- `allowAll : function() : InputActionBubbleUpFilter`: Passes all input actions on to the parent.
- `allowOnly : function(inputActionIds : {string}) : InputActionBubbleUpFilter`: Passes only the listed input actions on.
- `filterOutAll : function() : InputActionBubbleUpFilter`: Passes no input actions on (the pause menu window uses this).
- `filterOut : function(inputActionIds : {string}) : InputActionBubbleUpFilter`: Passes all input actions except the listed ones.

#### Builtin.Type.LineVisualization (record)

```teal
record LineVisualization
```

A line to draw in the line viewer (`LineViewerParam.showLines`).

- `new : function() : LineVisualization`: Creates a new, empty `LineVisualization`.
- `entity : Engine.Entity`: The line entity.
- `transparency : number`: Transparency of the drawn line.

#### Builtin.Type.VisibilityThresholds (record)

```teal
record VisibilityThresholds
```

Camera distance rules for HUD icons, given to `HudIconManagerConfigParam.visibilityThresholds`.

- `new : function() : VisibilityThresholds`: Creates a new, empty `VisibilityThresholds`.
- `cameraHeightThreshold : number`
- `cameraHeightLowerCutoff : number`
- `hudLodFarRangeThreshold : number`
- `forComponents : {{Engine.ComponentType, OfComponent}}`: Rules per component type, as `{componentType, OfComponent}` pairs.
- `forSubEntities : {{Gui.SelectionDetails.Type, OfSubEntity}}`: Rules per sub-entity type, as `{selectionType, OfSubEntity}` pairs.

##### Builtin.Type.VisibilityThresholds.OfComponent (record)

```teal
record OfComponent
```

Visibility rule for the HUD icons of entities with one component type.

- `new : function() : OfComponent`: Creates a new, empty `OfComponent`.
- `forceVisible : boolean`: Forces the hud icon to always be visible
- `minDist : number`: Minimum camera distance from which the hud icon gets drawn
- `scale : number`: Scale multiplier for hud icon
- `maxDist : number`: Maximum camera distance until which the hud icon gets drawn
- `randomizeMaxDistBase : number`: add random value to add to MaxDist
- `layer : integer`: Controls the draw order of the main hud icon (lower toward front)
- `physicsImpact : integer`: Controls hud icon physics impact, 0 is disabled, 1 is light, >= 2 is heavy

##### Builtin.Type.VisibilityThresholds.OfSubEntity (record)

```teal
record OfSubEntity
```

Visibility rule for the icons of sub-entities of one selection type.

- `new : function() : OfSubEntity`: Creates a new, empty `OfSubEntity`.
- `detailsThreshold : number`: when exceeding this camera distance, the main icon is shown instead of sub icons, when set to negative number, sub icons are never shown, when set to nil, both parent icon and sub entity icons are shown
- `layer : integer`: Controls the draw order of the subentity hud icon (lower toward front)
- `physicsImpact : integer`: Controls hud icon physics impact, 0 is disabled, 1 is light, >= 2 is heavy

#### Builtin.Type.ParticleSystem2dEmitter (record)

```teal
record ParticleSystem2dEmitter
```

one emitter in a particle systen

- `new : function() : ParticleSystem2dEmitter`: Creates a new, empty `ParticleSystem2dEmitter`.
- `emissionEnabled : boolean`: whether or not this emitter should currently emit
ignored for sub-emitters with enableAtBirth or enableAtDeath set to true
- `speed : number`: allow to slowdown or speed up the emission rate
- `duration : ParticleParamFloat`: if > 0, particle system will stop after duration
if -1 and this is a sub-emitter, it will stop when the parent particle dies, otherwise it will never stop
- `texture : string`: texture of the particle
- `emissionInterval : ParticleParamFloat`: spawn rate
- `burstCount : ParticleParamFloat`: if >= 0, limits spawned particles per burst (-1 = unlimited)
- `burstInterval : ParticleParamFloat`: wait time between bursts (only used when burstCount >= 0)
- `lifeTime : ParticleParamFloat`: lifetime of each particle
- `subEmitters : {ParticleSystem2dSubEmitter}`: list of sub-emitters started from this emitter's particles
- `shape : ParticleSystem2dEmitterShape`: emitter shape (ContentRect or Circle)
- `velocity : ParticleParamVec2f`: initial velocity
- `velocityByDirection : ParticleParamFloat`: velocity component along emission direction
- `velocityOverLifeTime : ParticleParamCurveVec2f`: additional velocity curve over the lifetime of the particles (added to base velocity)
- `gravity : ParticleParamVec2f`: gravity applied to the particles (added to velocity every frame)
- `orientation : Vec2f`: reference orientation vector for rotation calculations (default: {0, -1})
if 0, 0 and this is a sub-emitter, orientation will be inhertted from parent particle current veloctiy
- `rotationByDirection : ParticleParamFloat`: rotation component along emission direction (multiplier for direction angle in degrees)
- `rotation : ParticleParamFloat`: initial rotation of the particles
- `rotationSpeed : ParticleParamFloat`: rotation speed of the particles (added to rotation every frame)
- `rotationSpeedByDirection : ParticleParamFloat`: rotation speed component along emission direction (multiplier for direction angle in degrees)
- `colorOverLifeTime : ParticleParamCurveVec3f`: color curve over the lifetime of the particles (RGB)
- `alphaOverLifeTime : ParticleParamCurveFloat`: alpha curve over the lifetime of the particles (A)
- `size : ParticleParamVec2f`: initial size of a particle
- `scaleOverLifeTime : ParticleParamCurveVec2f`: size curve over the lifetime of the particles (XY)

#### Builtin.Type.ParticleSystem2dSubEmitter (record)

```teal
record ParticleSystem2dSubEmitter
```

Starts another emitter of the same particle system from the particles of this one.

- `new : function() : ParticleSystem2dSubEmitter`: Creates a new, empty `ParticleSystem2dSubEmitter`.
- `new : function(emitterIndex : integer, enableAtBirth? : boolean, enableAtDeath? : boolean) : ParticleSystem2dSubEmitter`: Creates a new, empty `ParticleSystem2dSubEmitter`.
- `emitterIndex : integer`: index into ParticleSystem2d.emitters
- `enableAtBirth : boolean`: starts sub-emitter when parent particle is born
- `enableAtDeath : boolean`: starts sub-emitter when parent particle dies

#### Builtin.Type.ParticleSystem2dEmitterShape (interface)

```teal
interface ParticleSystem2dEmitterShape
```

marker interface for emitter shapes

#### Builtin.Type.ParticleSystem2dEmitterShapeContentRect (record)

```teal
record ParticleSystem2dEmitterShapeContentRect is ParticleSystem2dEmitterShape
```

Emits from a point inside the widget's content rectangle.

- `new : function() : ParticleSystem2dEmitterShapeContentRect`: Creates a new, empty `ParticleSystem2dEmitterShapeContentRect`.
- `new : function(pos01 : Vec2f) : ParticleSystem2dEmitterShapeContentRect`: Creates a new, empty `ParticleSystem2dEmitterShapeContentRect`.
- `pos01 : ParticleParamVec2f`: Position in the rectangle, 0 to 1 on both axes.

#### Builtin.Type.ParticleSystem2dEmitterShapeCircle (record)

```teal
record ParticleSystem2dEmitterShapeCircle is ParticleSystem2dEmitterShape
```

Emits from a ring or circle sector.

- `new : function() : ParticleSystem2dEmitterShapeCircle`: Creates a new, empty `ParticleSystem2dEmitterShapeCircle`.
- `new : function(minRadius : number, maxRadius : number, minAngle? : number, maxAngle? : number) : ParticleSystem2dEmitterShapeCircle`: Creates a new, empty `ParticleSystem2dEmitterShapeCircle`.
- `minRadius : number`: Inner radius.
- `maxRadius : number`: Outer radius.
- `minAngle : number`: Start angle of the sector.
- `maxAngle : number`: End angle of the sector.

#### Builtin.Type.EventTrigger (record)

```teal
record EventTrigger
```

- `new : function() : EventTrigger`: Creates a new, empty `EventTrigger`.
- `new : function(time : number, pos : Vec2f) : EventTrigger`: Creates a new, empty `EventTrigger`.
- `time : number`
- `pos : Vec2f`

### Builtin.WithComponentParamsParam (record)

```teal
record WithComponentParamsParam
```

Settings applied to a node by wrapping it. React creates this wrapper from `meta` fields such as
`tooltip`, `tag` or `enabled`; set them in `meta`; the recipe is not meant to be called directly.

- `id : string`: Element id, as used by `api.gui.byId`.
- `tag : string`: Tag; the context help looks up its help page by tag.
- `tooltip : string`: Tooltip text.
- `tooltipModifierInputAction : string`: append this IA's key to the tooltip (regardless of what the key currently does)
- `forceFocusable : boolean`: Makes the node focusable.
- `disableFocusableRecursive : boolean`: Makes the node and its children non-focusable.
- `enabled : boolean`: False disables the node.
- `item : TreeNodeId`: The wrapped node.
- `styleSheet : StyleSheet`: Style sheet for the node.

### Builtin.FloatingLayoutParam (record)

```teal
record FloatingLayoutParam
```

Parameters of a floating layout, in which children are placed on top of each other.

- `id : string`: Element id.
- `focusTraversalOrientation : Type.Orientation`: Direction for gamepad focus movement between the children.
- `children : {TreeNodeId}`: Children; wrap them in `FloatingLayoutChild` to set their position.

### Builtin.FloatingLayoutChildParam (record)

```teal
record FloatingLayoutChildParam
```

Placement of one child of a floating layout.

- `h : number`: Horizontal position as a fraction of the free space (0 left, 0.5 centre, 1 right); `-1` makes the child fill the width.
- `v : number`: Vertical position as a fraction of the free space (0 top, 0.5 centre, 1 bottom); `-1` makes the child fill the height.
- `overflowMode : boolean`: Whether the child may leave the parent; the base game passes a `builtin.type.FloatingLayoutOverflowMode`
value here although the field is declared boolean.
- `localKey : string`: Key that keeps the child's identity between renders.
- `item : TreeNodeId`: The child node (required).

### Builtin.AbsoluteLayoutChildParam (record)

```teal
record AbsoluteLayoutChildParam
```

One child of an absolute layout.

- `localKey : string`: Key that keeps the child's identity between renders.
- `item : TreeNodeId`: The child node (required).

### Builtin.BoxLayoutParam (record)

```teal
record BoxLayoutParam
```

Parameters of a box layout.

- `id : string`: Element id.
- `children : {TreeNodeId}`: Children in order.
- `child : TreeNodeId`: shortcut to setting children = {child}
- `orientation : Type.Orientation`: Horizontal or vertical arrangement.

### Builtin.FlowLayoutParam (record)

```teal
record FlowLayoutParam
```

Parameters of a flow layout.

- `children : {TreeNodeId}`: Children in order.

### Builtin.ChartParam (record)

```teal
record ChartParam
```

Parameters of a chart.

- `xAxisLabelFormatter : function(number) : string`: Returns the label text for an X axis value.
- `yAxisLabelFormatter : function(number) : string`: Returns the label text for a Y axis value.
- `xAxisMarkers : {number}`: X values at which axis markers are drawn.
- `yAxisMarkers : {number}`: Y values at which axis markers are drawn.
- `xAxisMin : number`: Lowest X value shown.
- `xAxisMax : number`: Highest X value shown.
- `yAxisMin : number`: Lowest Y value shown.
- `yAxisMax : number`: Highest Y value shown.

### Builtin.SeriesParam (record)

```teal
record SeriesParam
```

One data series of a chart.

- `label : string`: Name of the series.
- `type : Type`: Chart type.
- `color : {Vec4f}`: Colour(s) of the series.
- `lineWidth : number`: Line width.
- `xValues : {number}`: X values.
- `yValues : {number}`: Y values, one per X value.
- `gradientFill : boolean`: adds a color gradient below the line (only available for Type LINE at the moment)

#### Builtin.SeriesParam.Type (enum)

```teal
enum Type
```

Chart type of a series.

- `"LINE"`: Line.
- `"LINE_STEP"`: Stepped line.
- `"STACKED_LINE"`: Line stacked on the previous series.
- `"BAR"`: Bars.
- `"STACKED_BAR"`: Bars stacked on the previous series.

### Builtin.ListParam (record)

```teal
record ListParam
```

Parameters of a list of selectable items.

- `drawBackground : boolean`: Draw the list background.
- `orientation : Type.Orientation`: Horizontal or vertical list.
- `behavior : Type.ListBehavior`: Selection behaviour.
- `cycle : boolean`: Wrap around from the last item to the first when navigating.
- `deselectAllowed : boolean`: Allow deselecting the selected item.
- `emitSignalOnReselect : boolean`: Call `onSelect` again when the selected item is selected again.
- `onHover : function(integer)`: Called with the index of the hovered item.
- `onSelect : function(integer)`: Called with the index of the selected item.
- `onActivate : function(integer)`: Called with the index of the activated item.
- `horizontalScrollBarPolicy : Type.ScrollBarPolicy`: When to show the horizontal scroll bar.
- `verticalScrollBarPolicy : Type.ScrollBarPolicy`: When to show the vertical scroll bar.
- `disableGamepadNavigation : boolean`: Do not navigate the list with the gamepad.
- `disableMouseScrolling : boolean`: Do not scroll with the mouse wheel.
- `children : {TreeNodeId}`: The items.
- `selectionIndex : integer`: Index of the selected item.
- `keepSelectedVisible : boolean`: Scroll so that the selected item stays visible.

### Builtin.ScrollAreaParam (record)

```teal
record ScrollAreaParam
```

Parameters of a scroll area.

- `content : TreeNodeId`: The scrolled node.
- `horizontalPolicy : Type.ScrollBarPolicy`: When to show the horizontal scroll bar.
- `verticalPolicy : Type.ScrollBarPolicy`: When to show the vertical scroll bar.
- `disableGamepadNavigation : boolean`: Do not scroll with the gamepad.
- `disableMouseScrolling : boolean`: Do not scroll with the mouse wheel.
- `onScroll : function(Vec2i, Vec2i)`: Called when the scroll position changes.

### Builtin.ComponentParam (record)

```teal
record ComponentParam
```

Parameters of a component, the basic styled container.

- `layout : TreeNodeId`: Layout node with the component's content.
- `name : string`: Name of the component.
- `mouseTransparent : boolean`: Let mouse events pass through.

### Builtin.TextViewParam (record)

```teal
record TextViewParam
```

Parameters of a text view.

- `text : string`: The text.
- `inlineHintSupport : boolean`
- `useUnicodeCompatibilityFont : boolean`: Use the font with wider Unicode coverage (the language selection uses it).
- `tooltipWhenClipped : string`: Tooltip shown when the text does not fit.

### Builtin.RichTextViewParam (record)

```teal
record RichTextViewParam
```

Parameters of a rich text view.

- `text : string`: The text.
- `isHtml : boolean`: The text is HTML.
- `isMarkdown : boolean`: The text is Markdown.

### Builtin.KeybindingHintDisplayParam (record)

```teal
record KeybindingHintDisplayParam
```

Parameters of a key binding hint.

- `inputAction : string`: Input action whose key is shown, e.g. `"IA_SELECT"`.
- `source : ReactRefWrap`: Node the hint belongs to.
- `sourceId : string`: Element id of the node the hint belongs to.

### Builtin.ParticleSystem2dParam (record)

```teal
record ParticleSystem2dParam
```

Parameters of a 2D particle system widget.

- `emitters : {Builtin.Type.ParticleSystem2dEmitter}`: The emitters; sub-emitters refer to them by index.

### Builtin.ImageViewParam (record)

```teal
record ImageViewParam
```

Parameters of an image view.

- `meta : Meta`: Common node settings.
- `path : string`: Image file, e.g. `"::/gui/builtin/checkbox/checked_16.tga"`.
- `scaling : Type.ImageViewScaling`: How the image is scaled.
- `data : {integer}`
- `data_native : Vector<integer>`
- `flippedHorizontally : boolean`: Mirror the image.
- `size : Vec2i`
- `coord : Vec2i`: Grid of frames in a sprite sheet, columns and rows (e.g. `6, 6` for 36 frames).
- `numFrames : integer`: Number of frames in the sprite sheet.
- `frame : integer`: Frame to show.
- `reverseAnimation : boolean`: Play the frames backwards.
- `animationSpeed : integer`: Animation speed; 0 shows the fixed `frame`.
- `waitNCycles : integer`: Pause in animation cycles between two runs of the animation.
- `stopOnLastFrame : boolean`: Stop at the last frame instead of looping.
- `animationFrameBottomToTop : boolean`: Frames are ordered from the bottom of the sheet upwards.
- `rotation : number`: Rotation of the image.

### Builtin.ShaderQuadParam (record)

```teal
record ShaderQuadParam
```

Parameters of a shader quad, an image drawn with lighting and mouse effects.

- `meta : Meta`: Common node settings.
- `scaling : Type.ImageViewScaling`: How the image is scaled.
- `path0 : string`: First texture.
- `path1 : string`: Second texture.
- `path2 : string`: Third texture.
- `path3 : string`: Fourth texture.
- `eventTriggerR : Builtin.Type.EventTrigger`
- `eventTriggerG : Builtin.Type.EventTrigger`
- `eventTriggerB : Builtin.Type.EventTrigger`
- `eventTriggerA : Builtin.Type.EventTrigger`
- `eventTriggerBig : Builtin.Type.EventTrigger`
- `lightDirection : Vec3f`: Direction of the light.
- `lightStrength : number`: Strength of the light.
- `specularColor : Vec3f`: Colour of the specular highlight.
- `specularMixAmount : number`: Share of the specular colour in the result.
- `animatedRippleStrength : number`
- `mouseGradDist : number`
- `mouseGradBaseStr : number`
- `mouseClickStr : number`
- `scrollBase : boolean`
- `useNormalMap : boolean`: Use a normal map for the lighting.
- `useSpecularLighting : boolean`: Draw specular highlights.
- `useFullOpacity : boolean`: Draw fully opaque.
- `useAnimatedColorVariation : boolean`
- `useBaseColorVariation : boolean`
- `rippleEffectOnClick : boolean`: Show a ripple effect on click.
- `shineEffectOnClick : boolean`: Show a shine effect on click.
- `mouseScrollHorizontal : boolean`
- `shineEffectOnBigEvent : boolean`
- `mouseGradAdditional : boolean`

### Builtin.ButtonParam (record)

```teal
record ButtonParam
```

Parameters of a button.

- `content : TreeNodeId`: Content of the button, e.g. a text view.
- `onClick : function()`: Called when the button is clicked or activated.

### Builtin.ToggleButtonParam (record)

```teal
record ToggleButtonParam
```

use initialValue if your ToggleButton is dumb (not referring to an external state)
use value AND onValueChange your ToggleButton refers to a state (in the parent recipe of the checkBox)

- `content : TreeNodeId`: Content of the button.
- `onValueChange : function(integer)`: Called with the new value, 0 (off) or 1 (on).
- `initialValue : integer`: Start value when the button keeps its own state.
- `value : integer`: Current value when a parent state controls the button (use with `onValueChange`).

### Builtin.ToggleButtonGroupChildParam (record)

```teal
record ToggleButtonGroupChildParam
```

One button of a toggle button group.

- `content : TreeNodeId`: Content of the button.
- `meta : Meta`: all transferred to ToggleButton inside ToggleButtonGroup!

### Builtin.ToggleButtonGroupParam (record)

```teal
record ToggleButtonGroupParam
```

Parameters of a group of toggle buttons of which one is selected.

- `buttons : {ToggleButtonGroupChildParam}`: The buttons.
- `onValueChange : function(index : integer, value : integer)`: Called with the index of the newly selected button.
- `deselectAllowed : boolean`: default is false
- `layout : Layout`: default is "Horizontal"
- `separators : {integer : TreeNodeId}`: Nodes inserted between buttons, keyed by position.

use -1 to avoid any selection at all (only working if deselectAllowed is set to true)
- `initialSelected : integer`
- `selected : integer`: Index of the selected button; -1 selects none (only with `deselectAllowed`).

#### Builtin.ToggleButtonGroupParam.Layout (enum)

```teal
enum Layout
```

Arrangement of the buttons.

- `"Horizontal"`: default, horizontal aligned buttins with individual widths
- `"Vertical"`: vertical aligned buttons with individual heights
- `"Flow"`: horizontal aligned buttons with individual widths, but wrapping to next line if not enough space
- `"Uniform"`: horizontal aligned buttons with equal widths, filling the available space

### Builtin.ToolButtonParam (record)

```teal
record ToolButtonParam
```

Parameters of a tool button, which pushes a tool onto the tool stack.

- `rendererComponentId : string`: defaults to "mainView"
- `toolDefinition : IReactToolDefinition`: Tool pushed by the button.
- `toolName : string`: can be used instead of toolDefinition for cases where the definition is not available
- `toolVariant : string`: Variant of the tool; the button shows as active when this variant is active.
- `toolStack : ReactRefWrapApiT<ToolStackParam, ToolStackAPI>`: Tool stack to use.
- `text : string`: Button text.
- `content : TreeNodeId`: Button content.
- `showFn : function()`
- `hideFn : function()`

### Builtin.DoubleSpinBoxApi (record)

```teal
record DoubleSpinBoxApi
```

API of a spin box node.

- `startEditing : function(onStop : function())`: Starts text editing; `onStop` is called when editing ends.

### Builtin.DoubleSpinBoxParam (record)

```teal
record DoubleSpinBoxParam
```

Parameters of a number input with up and down steps.

- `min : number`: nil: unbounded
- `max : number`: nil: unbounded
- `step : number`: Step size.
- `value : number`: Current value.
- `onValueChange : function(number)`: Called with the new value.
- `virtualKeyboardTitle : string`: Title of the on-screen keyboard.
- `virtualKeyboardDescription : string`: Description on the on-screen keyboard.
- `startInEditMode : boolean`: Start with text editing active.
- `onStopEditMode : function()`: Called when text editing ends.

### Builtin.TextInputFieldParam (record)

```teal
record TextInputFieldParam
```

Parameters of a text input field.

- `value : string`: Current text.
- `resetValueOnCancel : boolean`: show initialValue on cancelling (default true. Does not trigger onValueChange)
- `placeholderText : string`: Text shown while the field is empty.
- `filter : function(string) : boolean`: Returns false to reject a text.
- `onEditingModeChange : function(boolean)`: Called when editing starts (true) or ends (false).
- `onTyping : function(string)`: Called with the text after each change while typing.
- `onValueChange : function(string)`: Called with the accepted text.
- `onCancel : function()`: Called when editing is cancelled.
- `showVirtualKeyboardOnFocus : boolean`: Open the on-screen keyboard when the field gets the focus.
- `virtualKeyboardTitle : string`: Title of the on-screen keyboard.
- `virtualKeyboardDescription : string`: Description on the on-screen keyboard.
- `type : integer`
- `passwordMode : boolean`: Hide the typed characters.
- `acceptOnFocusLoss : boolean`: Accept the text when the field loses the focus.
- `deselectOnFocusLost : boolean`: Clear the text selection when the field loses the focus.
- `focusOnStartEditing : boolean`: Take the focus when editing starts.
- `popFocusOnStopEditing : boolean`: Give the focus back when editing stops.
- `maxLength : integer`: Maximum number of characters.
- `displayOnly : boolean`: defaults to false

### Builtin.SliderParam (record)

```teal
record SliderParam
```

Parameters of a slider.

- `min : integer`: Lowest value.
- `max : integer`: Highest value.
- `step : integer`: Step size.
- `pageStep : integer`: Larger step size.
- `horizontal : boolean`: Horizontal slider (otherwise vertical).
- `drawGroove : boolean`: Draw the groove.
- `disableGamepadNavigation : boolean`: Do not change the value with the gamepad.
- `withTicks : boolean`: defaults to false
- `stepsForDynamicKeyRepeatSpeed : integer`: If range has at least that many steps, keeping keys pressed will increase repeat frequency. Default: 10, set -1 to disable entirely.
- `initialValue : integer`: Start value when the slider keeps its own state.
- `value : integer`: Current value when a parent state controls the slider.
- `onValueChange : function(newValue : integer)`: Called with the new value.

### Builtin.ColorPickerParam (record)

```teal
record ColorPickerParam
```

Parameters of a colour picker.

- `color : Vec3f`: Current colour (RGB).
- `onValueChange : function(Vec3f)`: Called with the colour while it changes.
- `onApply : function(Vec3f)`: Called with the colour when the apply button is pressed.
- `showApplyButton : boolean`: Show an apply button.

### Builtin.ColorChooserParam (record)

```teal
record ColorChooserParam
```

Parameters of a colour chooser, a set of colour swatches.

- `colors : {Vec3f}`: Colours to choose from.
- `useCustomColors : boolean`
- `onValueChange : function(Vec3f)`: Called with the chosen colour.

### Builtin.ColorChooserButtonParam (record)

```teal
record ColorChooserButtonParam
```

Parameters of a button that opens a colour chooser.

- `colors : {Vec3f}`: Colours to choose from.
- `color : Vec3f`: Current colour.
- `onValueChange : function(Vec3f)`: Called with the chosen colour.
- `resetButton : boolean`: Show a reset button.
- `innerResetButton : boolean`
- `image : string`: Image on the button.
- `toolTipPosition : integer`: Position of the tooltip, a `builtin.type.ToolTipPosition` value.

### Builtin.GradientLegendParam (record)

```teal
record GradientLegendParam
```

Parameters of a colour gradient legend.

- `colors : table`: Colours of the gradient.
- `horizontal : boolean`: Horizontal legend (otherwise vertical).
- `discrete : boolean`: Show separate colour steps instead of a smooth gradient.
- `internalLabels : boolean`

### Builtin.ProgressBarParam (record)

```teal
record ProgressBarParam
```

Parameters of a progress bar.

- `value : number`: must be in [0, 1]!
- `thresholds : {number}`: must be in [0, 1]!
- `label : string`: Text on the bar.
- `applyGradient : boolean`: defaults to true

### Builtin.CheckBoxParam (record)

```teal
record CheckBoxParam
```

use initialValue if your CheckBox is dumb (not referring to an external state)
use value AND onValueChange your CheckBox refers to a state (in the parent recipe of the checkBox)

- `onValueChange : function(integer)`: Called with the new value, 0 (unchecked), 1 (checked) or -1 (indeterminate).
- `triStateSupport : boolean`: nil defaults to false --> Note that this is only for toggling through when clicking, otherwise tristate is alwayws possible
- `initialValue : integer`: Start value when the check box keeps its own state (0, 1 or -1).
- `value : integer`: Current value when a parent state controls the check box (0, 1 or -1).
- `label : string`: optional, click on text changes CheckBox state
- `moveBoxRight : boolean`: default is false: Box is left, label is right

### Builtin.ComboBoxParam (record)

```teal
record ComboBoxParam
```

Parameters of a combo box (drop-down list).

- `value : integer | number | string`: Value of the selected item.
- `items : {TreeNodeId}`: Items, created with `ComboBoxItem`.
- `onValueChange : function(newValue : integer | number | string)`: Called with the value of the newly selected item.
- `onPopupChange : function(popupShown : boolean)`: Called when the drop-down list opens (true) or closes (false).
- `compact : boolean`: defaults to false, decides about visual appearance

### Builtin.ComboBoxItemParam (record)

```teal
record ComboBoxItemParam
```

One entry of a combo box.

- `value : integer | number | string`: Value of the entry.
- `content : TreeNodeId`: Content shown for the entry.
- `available : boolean`: False shows the entry as unavailable.

### Builtin.NativeFileDialogButtonParam (record)

```teal
record NativeFileDialogButtonParam
```

Parameters of a button that opens the operating system's file dialog.

- `content : TreeNodeId`: Content of the button.
- `onOpen : function({string})`: Called with the selected file paths.
- `title : string`: Title of the dialog.
- `filters : {{string, string}}`: File type filters as `{label, extensions}` pairs; `extensions` is a comma-separated list without dots, for example `{{_("Images"), "png,jpg"}}`.
- `intiialPath : string`: Folder the dialog opens in (the field name is spelled this way).

### Builtin.DropComponentParam (record)

```teal
record DropComponentParam
```

Parameters of an area that accepts dropped files.

- `content : TreeNodeId`: Content of the area.
- `onDrop : function({string})`: Called with the paths of the dropped files.

### Builtin.RadialMenuChildParam (record)

```teal
record RadialMenuChildParam
```

One entry of a radial menu.

- `icon : string`: Icon of the entry.
- `onActivate : function()`: Called when the entry is chosen.
- `getInfoText : function() : string`: Returns the text shown while the entry is highlighted.
- `tag : string`: Tag of the entry.

### Builtin.RadialMenuParam (record)

```teal
record RadialMenuParam
```

Parameters of a radial menu.

- `onHide : function()`: Called when the menu closes.
- `getNoSelectionInfoText : function() : string`: Returns the text shown while no entry is highlighted.
- `mouseSupport : boolean`: Allow choosing with the mouse.
- `children : {TreeNodeId}`: of type Builtin.RadialMenuChild

### Builtin.TabWidgetParam (record)

```teal
record TabWidgetParam
```

Parameters of a tab widget.

- `orientation : Type.TabOrientation`: default: TabOrientation.North
- `deselectAllowed : boolean`: default: false
- `indicatorsScrollable : boolean`: default: true, works only with orientation = Type.North and Type.South
- `tabs : {TreeNodeId}`: Tabs, created with `TabWidgetChild`.
- `showIndicators : boolean`: default: true
- `onClose : function()`: Called when the tab widget is closed.
- `initialValue : integer | number | string`: default: first child
- `value : integer | number | string`: Index of the active tab.
- `onValueChange : function(newValue : integer | number | string)`: value of selected child, nil if nothing is selected (only if deselectAllowed == true)

### Builtin.TabWidgetChildParam (record)

```teal
record TabWidgetChildParam
```

One tab of a tab widget.

- `localKey : string`: Key that keeps the tab's identity between renders.
- `indicator : TreeNodeId`: Node shown in the tab bar.
- `item : TreeNodeId`: Content of the tab.
- `value : integer | number | string`: must be not nil and must be unique within their TabWidget

### Builtin.RowParam (record)

```teal
record RowParam
```

One row of a table layout.

- `cells : {TreeNodeId}`: Cells of the row.

### Builtin.TableLayoutParam (record)

```teal
record TableLayoutParam
```

Parameters of a table layout.

- `id : string`: Element id.
- `columnWeights : {number}`: Relative widths of the columns, e.g. `{3, 2}`.
- `rows : {TreeNodeId}`: instances of Builtin.Row

### Builtin.TableCellParam (record)

```teal
record TableCellParam
```

Parameters a data table passes to a cell recipe.

- `rowKey : integer`: Key of the row.
- `colKey : integer`: Key of the column.
- `userParam : any`: The table's user parameter.

### Builtin.TableCellParamT (record)

```teal
record TableCellParamT<T>
```

Typed parameters a data table passes to a cell recipe.

- `rowKey : integer`: Key of the row.
- `colKey : integer`: Key of the column.
- `userParam : T`: The table's user parameter.

### Builtin.ColumnDescParam (record)

```teal
record ColumnDescParam
```

Description of one column of a data table.

- `name : string`: Column header text.
- `tooltip : string`: Tooltip of the header.
- `path : string`: Icon of the header.
- `headerStyleClass : string`: Style class of the header.
- `weight : number`: Relative width of the column.
- `recipe : IRecipe`: with arg of type Builtin.TableCellParam
- `getCompareValue : function(rowKey : integer) : any`: Get value to be used when sorting the table at C++.
Sorting the table from C++ will use lang_util.compareStrings() to compare strings that are not themselves keys inside a table.
For example, this will cause "Train 2" to be sorted before "Train 10".
If needed, you can opt-out of this string comparison behavior by setting forceLexicographicalStringComparison = true.
- `forceLexicographicalStringComparison : boolean`: Force lexicographical string comparison for all strings (causing e.g. "Train 10" before "Train 2")

### Builtin.DataTableApi (record)

```teal
record DataTableApi
```

API of a data table node.

- `scrollToRowKey : function(rowKey : integer, forceTopAlign : boolean)`: note that this will not work in the 1st frame. You should always set
for initial selection, use preferredInitialSelectionRowKey

### Builtin.DataTableParam (record)

```teal
record DataTableParam
```

Parameters of a data table, a sortable table built from cell recipes.

- `columns : {TreeNodeId}`: instances of type Builtin.ColumnDesc
- `rowKeys : {integer}`: Keys of the rows to show.
- `preferredInitialSelectionRowKey : integer`: Row selected when the table first appears.
- `disableSortKey : boolean`
- `iaSort : string`: InputAction key to change sorting (default: IA_OPTION2)
- `compareFn : function(colIndex : integer, rowKeyA : integer, rowKeyB : integer) : boolean`: Deprecated: Function for table sorting. Prefer ColumnDescParam.getCompareValue instead.
- `userParam : any`: Input param given to *NEWLY CREATED* cells. Existing cells will *NOT* be updated! (-> use constants or pass a ref and read onStep inside the cell)
- `fnUserFilter : function(integer) : boolean`: Returns false for rows to hide.
- `initialSortColumn : {integer, boolean}`: 1-based index of initial column, true if ASC, false if DESC
- `onSortColumnChange : function(columnIndex : integer, ascending : boolean)`: Called when the sort column or order changes.
- `columnWeights : {number}`: Removed. Use weight of ColumDesc instead.
- `scrollPolicyHorizontal : Type.ScrollBarPolicy`: When to show the horizontal scroll bar.
- `scrollPolicyVertical : Type.ScrollBarPolicy`: When to show the vertical scroll bar.
- `keyboardNavigation : boolean`: use arrow keys to navigate cells (instead of controlling scrollbars directly)

### Builtin.WindowParam (record)

```teal
record WindowParam
```

Parameters of a window.

- `title : string`: Window title.
- `onClose : function()`: Called when the window is closed.
- `closable : boolean`: Show the close button.
- `movable : boolean`: Allow moving the window.
- `pinnable : boolean`: Show the pin button.
- `pinned : boolean`: Window is pinned.
- `autoFocusOnBecomingVisible : boolean`: Give the window the focus when it becomes visible.
- `autoVisibilityOnFocusChange : boolean`
- `content : TreeNodeId`: Content of the window (required).
- `initialX : number`: Start position, X.
- `initialY : number`: Start position, Y.
- `titleEditable : boolean`: Let the player edit the title (e.g. to rename an entity).
- `onTitleChange : function(string)`: Called with the edited title.
- `locateVisible : boolean`: Show the locate button.
- `onLocate : function()`: Called when the locate button is pressed.
- `onPinned : function(boolean)`: Called when the window is pinned or unpinned.
- `id : string`: Element id, e.g. `"menu.pause"`; `api.gui.byId` finds the window by it.
- `tool : string`: Name of the tool the window belongs to, e.g. `"notification-log"`.
- `expanded : boolean`: if true, specify expanded size with a css rule using !window-expanded, furthermore: !window-collapsed is set when returning from expanded mode (but not initially)
- `compact : boolean`: if true, the content fills the entire window. Title bar gets reduced to some button (via !window-compact css rules) and overlaps with the content.
- `emptyNameAllowed : boolean`: defaults to true
- `inputActionBubbleUpFilter : Type.InputActionBubbleUpFilter`: Which input actions the window passes on to its parent.
- `header : TreeNodeId`: Extra node in the window header.

### Builtin.WindowContainerDelegateParam (record)

```teal
record WindowContainerDelegateParam
```

Parameters of the window container.

- `redirectToComponentIdHack : string`

### Builtin.WindowAPI (record)

```teal
record WindowAPI
```

API of the window container, from `gameCtx.windowContainer:get():getApi()`.

note that if these reuse a window, they do *not* change its visibility
- `addSingletonWindow : function<T>(Recipe<T>, params : T)`: Opens a window of the recipe; there is at most one such window.
- `addSingletonWindow : function<T, A>(RecipeWithApi<T, A>, params? : T)`: Opens a window of the recipe; there is at most one such window.
- `addWindow : function<T>(Recipe<T>, key : string, params : T)`: Opens a window of the recipe under a key; several windows of one recipe can be open with different keys.

note that these only move the window in z order, they do *not* change its visibility
- `moveSingletonWindowToFront : function(IRecipe)`: Brings the single window of the recipe to the front.
- `moveSingletonWindowToFront : function<T>(Recipe<T>)`: Brings the single window of the recipe to the front.
- `moveSingletonWindowToFront : function<T, A>(RecipeWithApi<T, A>)`: Brings the single window of the recipe to the front.
- `moveWindowToFront : function(IRecipe, key : string)`: Brings the window of the recipe with the key to the front.
- `moveWindowToFront : function<T>(Recipe<T>, key : string)`: Brings the window of the recipe with the key to the front.
- `removeAllWindows : function<T>(Recipe<T>)`: Closes all windows of the recipe.
- `removeAllWindows : function<T, A>(?RecipeWithApi<T, A>)`: Closes all windows of the recipe.
- `removeWindow : function<T>(Recipe<T>, key : string)`: Closes the window of the recipe with the key.

### Builtin.ToolStackContext (record)

```teal
record ToolStackContext<T>
```

Context a tool's callbacks receive.

- `setActionFn : function(actionFn? : (function() : TreeNodeId), key2? : string)`: Sets the function that creates the tool's action node (e.g. a `ConstructionAction`) for the 3D view; nil removes it.
- `popSelf : function()`: shortcut to toolStackApi.pop(recipe, key)
- `setVariantOverride : function(string)`: Overrides the active variant of the tool.

### Builtin.ToolStackAPI (record)

```teal
record ToolStackAPI
```

API of the tool stack.

- `push : function<T>(ReactToolDefinition<T>, key : string, params? : T, allowStacking : boolean)`: Pushes a tool with a key and parameters; with `allowStacking` false, all other tools except the default tool are removed first.
- `pop : function<T>(ReactToolDefinition<T>, key : string)`: Removes the tool with the key from the stack.
- `pop : function(tool : IReactToolDefinition, key : string)`: Removes the tool with the key from the stack.
- `clear : function()`: Removes all tools except the default tool.
- `setActionsDisabled : function(block : boolean)`: start/end behaving as if actionFn would always be nil
- `getActiveTool : function() : IReactToolDefinition, string`: Returns the tool on top of the stack and its key.

### Builtin.ToolStackParam (record)

```teal
record ToolStackParam
```

Parameters of the tool stack.

- `rendererComponent : ReactRefWrapApiT<Builtin.RendererComponentParam, Builtin.RenderComponentAPI>`: Renderer component (3D view) the tools act on.
- `defaultTool : ReactToolDefinition`: Tool used when the stack is empty.
- `defaultToolParam : any`: Parameters of the default tool.
- `defaultToolKey : string`: Key of the default tool.
- `showDebugVisualization : boolean`: Show debug output of the tool stack.

### Builtin.HudIconParam (record)

```teal
record HudIconParam
```

Parameters a HUD icon recipe receives for one entity.

- `entity : Engine.Entity`: The entity the icon belongs to.
- `index : integer`
- `details : Gui.SelectionDetails`: Selection details of the entity or sub-entity.
- `internalLayer : integer`
- `internalPhysicsImpact : integer`

### Builtin.HudIconChildConfigParam (record)

```teal
record HudIconChildConfigParam
```

Wraps one HUD icon node.

- `item : TreeNodeId`: The icon node.
- `selectorSupport : boolean`: true to allow selector hover event while mouse hovers the item. nil to inherit from parent (root defaults to true).
- `overrideSelectionDetails : boolean`: true to override selectionDetails for the above feature, false to use the one from HudIconParamT. nil to inherit from parent (root defaults to false)
- `selectionDetails : Gui.SelectionDetails`: if overrideSelectionDetails == true, the selectionDetails to be used

### Builtin.HudIconManagerConfigParam (record)

```teal
record HudIconManagerConfigParam<T>
```

Configures the HUD icons of a map layer.

- `userParam : T`: Value passed to the master recipe as its second parameter.
- `masterRecipe : Recipe2<HudIconParam, T>`: Recipe that creates the icon for each entity.
- `preFilter : {Engine.ComponentType}`: List components for which the HUD icons will be created and shown
Subconstruction filter only applies to subconstruction that are of no other type
- `preFilterTvCarriers : {Carrier}`: In case you have TransportVehicle in you preFilter, this additionally filters for its carriers
- `showCosts : boolean`: Show cost pop-ups.
- `showIncome : boolean`: Show income pop-ups.
- `incomeSFX : {FilePath}`: only active if showIncome is true
- `costsRenderDistance : number`: Camera distance up to which cost pop-ups are shown.
- `transportVehiclePainter : any`: variant nil|LineVehicleColor|TrafficColor|EmissionEmitterColor|MaintainedTransportVehicleColor
- `fallbackColor : LayerConfig.ColorPassFn.FallbackColor`: should be set whenever transportVehiclePainter is provided
- `localKey : string`: Key that keeps the node's identity between renders.
- `visibilityThresholds : Builtin.Type.VisibilityThresholds`: Camera distance rules per entity type.
- `vehicleIconOffsets : {Carrier : Vec3f}`: Offset of vehicle icons per carrier.

### Builtin.RendererComponentParam (record)

```teal
record RendererComponentParam
```

Parameters of the renderer component (the 3D view).

- `toolStack : ReactRefWrapApiT<ToolStackParam, ToolStackAPI>`: Tool stack whose top tool acts in the view.
- `redirectToComponentIdHack : string`

### Builtin.RenderComponentAPI (record)

```teal
record RenderComponentAPI
```

API of the renderer component.

- `bindAction : function(ref : ReactRefWrap, (function() : TreeNodeId), key2? : string)`
- `unbindAction : function(ref : ReactRefWrap)`

### Builtin.EntityRendererComponentParams (record)

```teal
record EntityRendererComponentParams
```

Parameters of an entity view.

- `entity : Engine.Entity`: The entity to show.

### Builtin.EdgeRenderableParam (record)

```teal
record EdgeRenderableParam
```

Parameters of edge highlights in the 3D view.

- `edges : {Type.EdgeRenderable.Edge}`: The edges to draw.
- `ignoreDepth : boolean`: Draw the edges on top of everything.

### Builtin.MissionArrowRenderableParam (record)

```teal
record MissionArrowRenderableParam
```

Parameters of mission arrows.

- `arrows : {Type.MissionArrowRenderable.Arrow}`: The arrows to draw.

### Builtin.TerrainCirclePolicy (enum)

```teal
enum TerrainCirclePolicy
```

When the circle under the mouse cursor is drawn on the terrain.

- `"FocusSelection"`: Only around the focused selection.
- `"Always"`: Always.
- `"IfNoEntity"`: Only when the cursor is not over an entity.
- `"Never"`: Never.

### Builtin.ActionDescriptorParam (record)

```teal
record ActionDescriptorParam
```

Describes the active action in the 3D view (tool name, layer, input prompts).

- `tool : string`: Name of the tool.
- `layer : string`: Map layer shown during the action.
- `terrainCirclePolicy : TerrainCirclePolicy`: When to draw the terrain circle.
- `highlightedEntities : {Engine.Entity}`: Entities to highlight.
- `crosshair : boolean`: Show the crosshair (the base game shows it in gamepad mode).
- `parent : ReactNodeRef`: Parent node of the action.
- `onBack : function()`: Called when the player backs out of the action.
- `horizontalPromptList : boolean`: Lay out the button prompts horizontally.
- `children : {TreeNodeId}`: Child nodes of the action.

### Builtin.ActionTooltipParam (record)

```teal
record ActionTooltipParam<T>
```

Tooltip shown at the cursor during an action.

- `recipe : Recipe<T>`: Recipe of the tooltip content.
- `param : T`: Parameter passed to the recipe.

### Builtin.ConstructionActionParam (record)

```teal
record ConstructionActionParam
```

Parameters of a building action in the 3D view. Set the descriptor of the tool kind to use, e.g.
`constructionBuilder` to place a construction or `bulldozer` to remove objects.

- `modifierColor : Type.ConstructionAction.ModifierColor`: Highlight colours of modifier tools.
- `constructionBuilder : Type.ConstructionAction.ConstructionBuilder`: Settings for placing constructions.
- `streetEdgeBuilder : Type.ConstructionAction.StreetEdgeBuilder`: Settings for building streets.
- `trackEdgeBuilder : Type.ConstructionAction.TrackEdgeBuilder`: Settings for building tracks.
- `streetEdgeNodeModifier : Type.ConstructionAction.StreetEdgeNodeModifier`: Settings for changing streets.
- `trackEdgeModifier : Type.ConstructionAction.TrackEdgeModifier`: Settings for changing tracks.
- `edgeObjectBuilder : Type.ConstructionAction.EdgeObjectBuilder`: Settings for placing edge objects (street stops, signals).
- `laneModifier : Type.ConstructionAction.LaneModifier`: Settings for changing lanes.
- `crosswalkModifier : Type.ConstructionAction.CrosswalkModifier`: Settings for changing crosswalks.
- `terrainModifier : Type.ConstructionAction.TerrainModifier`: Settings for shaping the terrain.
- `terrainPainter : Type.ConstructionAction.TerrainPainter`: Settings for painting the terrain.
- `assetBrush : Type.ConstructionAction.AssetBrush`: Settings for placing assets with a brush.
- `townBuilder : Type.ConstructionAction.TownBuilder`: Settings for founding towns.
- `bulldozer : Type.ConstructionAction.Bulldozer`: Settings for bulldozing.
- `moduleBuilder : Type.ConstructionAction.ModuleBuilder`: Settings for placing modules.
- `moduleBulldozer : Type.ConstructionAction.ModuleBulldozer`: Settings for removing modules.
- `getProposalStringsFn : function(proposal : Proposal, proposalData : ProposalData) : {string}`: Returns text lines for the current proposal.
- `inputActions : {string : string}`: Extra input actions of the action, by name.
- `inputActionsHandler : function(string)`: Called with the name of a triggered input action.
- `inputActionPrompts : {string : string}`: Prompt text per input action.
- `inputActionActive : {string : boolean}`: Active flag per input action.
- `preventFocusStealFromIds : {string}`: Element ids from which the action does not take the focus.

### Builtin.SimpleInputActionsParam (record)

```teal
record SimpleInputActionsParam
```

Parameters of a set of input actions.

- `inputActions : {string : string}`: Input actions, by name.
- `inputActionsHandler : function(string)`: Called with the name of a triggered input action.

### Builtin.SelectorParam (record)

```teal
record SelectorParam
```

Parameters of the entity selector in the 3D view.

- `onSelect : function(entity : Engine.Entity, indices : {integer}, details : Gui.SelectionDetails) : boolean`: Called when an entity is selected.
- `onSelectSecondary : function(entity : Engine.Entity, indices : {integer}, details : Gui.SelectionDetails) : boolean`: Called when an entity is selected with the secondary button.
- `onHover : function(entity : Engine.Entity, indices : {integer}, details : Gui.SelectionDetails)`: Called when the cursor moves over an entity.
- `filter : function(entity : Engine.Entity)`: Returns false for entities that cannot be selected.
- `terminalFilter : function(stationGroupEntity : Engine.Entity, stationTerminal : StationTerminal, flatTerminalIndex : integer)`: Returns false for station terminals that cannot be selected.
- `lineViewerSupport : boolean`: Allow selecting lines drawn by the line viewer.
- `stopOnMenuBack : boolean`: End the selection with the back input action.
- `onProcessMouseEvent : function(evt : Gui.Mouse.Event) : boolean`: Called with mouse events before the selector handles them.
- `selectionColor : Vec4f`: Highlight colour of the selection.
- `selectionOutlineColor : Vec4f`: Outline colour of the selection.
- `selectionOutlineColor1 : Vec4f`: Second outline colour of the selection.
- `circleBorderColor : Vec4f`: Border colour of the selector circle.
- `circleFillColor : Vec4f`: Fill colour of the selector circle.
- `circleBorderWidth : number`: Border width of the selector circle.
- `circleAnimEasing : Easing`: Easing of the circle animation.
- `circleAnimDuration : number`: Duration of the circle animation.
- `crosshairColor : Vec4f`: Colour of the crosshair.

### Builtin.FullScreenComponentParam (record)

```teal
record FullScreenComponentParam<T>
```

Content of a full-screen overlay.

- `recipe : Recipe<T>`: Recipe of the overlay content.
- `param : T`: Parameter passed to the recipe.

### Builtin.LineViewerParam (record)

```teal
record LineViewerParam
```

Parameters of the line viewer, which draws lines and stops in the 3D view.

- `selectable : boolean`: Lines can be selected.
- `carriers : {Carrier}`: Carriers whose lines are drawn.
- `highlightedLine : Engine.Entity`: Line to highlight.
- `highlightedLineStop : integer`: Stop of the highlighted line to highlight.
- `highlightedLineWaypoint : integer`: Waypoint of the highlighted line to highlight.
- `showTerminals : boolean`: Draw the station terminals.
- `highlightedTerminal : StationTerminal`: Terminal to highlight.
- `maxSnapFactorView : number`
- `maxSnapFactorWorld : number`
- `fadingMinHeight : number`
- `fadingDeltaHeight : number`
- `highlightedTerminalNodes : {{Engine.Entity, StationTerminal}}`: Terminals to highlight, as `{station, terminal}` pairs.
- `showLineTerminalsFor : Engine.Entity`: Line whose terminals are shown.
- `highlightStop : Gui.SelectionDetails.Stop`: Stop to highlight.
- `highlightWaypoint : {Engine.Entity, integer, integer}`: waypoint to highlight (line entity, zero-based stopIndex, waypoint tag)
- `snapPreview : Gui.SelectionDetails.TransportNetworkEdge`
- `showLines : {Builtin.Type.LineVisualization}`: Lines to draw, with transparency.
- `hiddenLinesTransparent : boolean`: default is true --> if set to false, the lines are completely invisible (refers to all the lines filtered away with "showLines")
- `continuousRenderLine : Engine.Entity`
- `preferLineOverTransportNetworkEdge : boolean`: default is true --> if set to false, it enables building waypoints on (already existing) lines

### Builtin.ProposalViewerParam (record)

```teal
record ProposalViewerParam
```

Parameters of the proposal viewer, which previews a building proposal in the 3D view.

- `simpleProposal : SimpleProposal`: Proposal to preview (simple form).
- `proposal : Proposal`: Proposal to preview.
- `controlPointInfo : Type.ControlPointInfo`: Control point to show.
- `onCreateProposalData : function(ProposalData, Proposal)`: Called with the processed proposal data.
- `entityForRefundableContext : Engine.Entity`
- `proposalId : string`: Id of the proposal.

### Builtin.MapPreviewCompParam (record)

```teal
record MapPreviewCompParam
```

Parameters of the map preview of the new-game and map editor screens.

- `mapColoring : PreviewColoring`: Colouring of the preview.
- `showMapSize : boolean`: Show the map size.
- `showTownAndIndustryCount : boolean`: Show the numbers of towns and industries.
- `showBiomesColors : boolean`: Colour the biomes.
- `inputMap : GameMap`: Map to show.
- `mode : any`: HeighmapGenerationMode | TownIndustryGenerationMode
- `locationsConfig : BaseConfig.Locations`: Town and industry settings for the preview.
- `economy : string`: Economy setting.
- `onReady : function(map : GameMap, feedback : {string})`: Called when the preview map is ready.
- `onTownConnectionsChanged : function({{integer, integer}})`
- `initialMap : GameMap`: use this when creating for the 1st time (instead of recomputing)

### Builtin.NodeViewerParam (record)

```teal
record NodeViewerParam
```

- `nodeConfig : {Engine.Entity : {integer, Vec4f}}`

### Builtin.ICompParam (interface)

```teal
interface ICompParam
```

Parameters of an entity component widget.

- `entity : Engine.Entity`: The entity.

### Builtin.RailroadCrossingCompParam (record)

```teal
record RailroadCrossingCompParam is ICompParam
```

Parameters of the railroad crossing widget.

### Builtin.SignalCompParam (record)

```teal
record SignalCompParam is ICompParam
```

Parameters of the signal widget.

### Builtin.ReactAdapterHost (record)

```teal
record ReactAdapterHost
```

### Builtin.ReactAdapterHostEventParam (record)

```teal
record ReactAdapterHostEventParam
```

- `component : ReactAdapterHost`
- `type : string`
- `param : any`

### Builtin.ReactAdapterGuestContainerAPI (record)

```teal
record ReactAdapterGuestContainerAPI
```

- `add : function(host : ReactAdapterHost, recipeFn : Recipe, recipeParam : any)`
- `remove : function(host : ReactAdapterHost)`
