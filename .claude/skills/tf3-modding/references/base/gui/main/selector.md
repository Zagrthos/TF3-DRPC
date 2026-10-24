# gui.main.selector

Definition file: `base/tealdef/gui/main/selector.d.tl` (in the game folder).

Result and parameter types of a line/station/vehicle selector. No base-game script uses
these types; the selectors of the game UI are built with `SelectorReactUtil` and
`builtin.Selector`.

## LinePick (record)

```teal
global record LinePick
```

- `line : Engine.Entity`
- `stop : integer`
- `waypoint : integer`

## StationPick (record)

```teal
global record StationPick
```

- `stationGroupEntity : Engine.Entity`
- `stationEntity : Engine.Entity`
- `terminalNumber : integer`
- `stationTerminal : Type.StationTerminal`

## SignalPick (record)

```teal
global record SignalPick
```

- `signalEntity : Engine.Entity`
- `signalIndex : integer`

## SelectorPick (record)

```teal
global record SelectorPick
```

- `line : LinePick`
- `station : StationPick`
- `signal : SignalPick`
- `transportVehicle : Engine.Entity`
- `vehicleDepotEntity : Engine.Entity`

## SelectorTooltipConfig (record)

```teal
global record SelectorTooltipConfig
```

- `actionTooltipFn : Recipe<any>`
- `actionTooltipParam : any`

## LineStationVehicleSelectorParams (record)

```teal
global record LineStationVehicleSelectorParams
```

- `parent : ReactNodeRef`
- `onBack : function()`
- `onSelectNothing : function() : boolean`
- `onSelect : function(SelectorPick) : boolean`: return true to reset hover state
- `onSelectSecondary : function(SelectorPick) : boolean`: return true to reset hover state
- `processMouseEvent : function(Gui.Mouse.Event)`
- `onHover : function(SelectorPick)`
- `filter : function(entity : Engine.Entity) : boolean`
- `terminalFilter : function(stationGroupOrSignal : Engine.Entity, stationTerminal : StationTerminal, flatTerminalIndex : integer)`
- `lineViewerVisible : boolean`
- `lineViewerSupport : boolean`
- `showTerminals : boolean`
- `lineDataLayer : boolean`
- `lineSelectable : boolean`
- `mouseCursor : integer`
- `carriers : {Carrier}`
- `highlightedEntities : {Engine.Entity}`
- `highlightedTerminal : StationTerminal`
- `highlightedTerminalNodes : {{Engine.Entity, StationTerminal}}`
- `wholeLinePick : boolean`
- `fallbackHover : SelectorPick`
- `tooltipStateHolder : ReactRefT<ReactStateT<SelectorTooltipConfig>>`
- `hudIconManagerConfig : Builtin.HudIconManagerConfigParam`
