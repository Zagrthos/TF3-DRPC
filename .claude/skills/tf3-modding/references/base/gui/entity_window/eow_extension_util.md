# gui.entity_window.eow_extension_util

Definition file: `base/tealdef/gui/entity_window/eow_extension_util.d.tl` (in the game folder).

Shared parameter interface and ownership helper for the extension points (plugins) of the entity windows.

## EowExtensionUtil (record)

```teal
global record EowExtensionUtil
```
Load: `local eow_extension_util = ug_require "::/gui/entity_window/eow_extension_util.tl" as EowExtensionUtil`

Shared parts of the entity window extension points.

- `useOwnershipStepState : function(entity : Engine.Entity) : ReactStateT<OwnershipState>`  (call as `eow_extension_util.useOwnershipStepState(…)`): React hook returning the ownership state of an entity, updated every step.
    - param `entity`: Entity to check.
    - returns: React state; read it with `:old()`.
    - used 13x in the base game, e.g. `base/content/gui/gui/entity_window/animal/animal.tl:40`: `ownershipState = eow_extension_util.useOwnershipStepState(entityId):old(),`

### EowExtensionUtil.IEowWidgetsExtensionParams (interface)

```teal
interface IEowWidgetsExtensionParams
```

Base parameters passed to every plugin of an entity window extension point (e.g. `VehicleEow.VehicleEowExtensionPoint`).

Notes:

Plugins are registered by resource files of type `"react-plugin ::<ExtensionPointName>"` with
`data = { filePath = "<script>@<PluginRecipe>", condition = "<script>@<function>", order = <number> }`.
The window calls `gui_react_util.usePlugins(extensionPoint, ..., extensionParams)`; plugins are shown
in ascending `order`, and a plugin whose `condition` function returns false for the parameters is skipped.

- `entityId : Engine.Entity`: Entity shown by the window.
- `ownershipState : OwnershipState`: Who owns the entity, see `EowExtensionUtil.OwnershipState`.
- `gameCtx : GameContext`: Game context.
- `showCalloutOnRightSide : boolean`: Whether info callouts of content cards are shown on the right side of the window.
- `setCollapsibleExpanded : function(string, boolean)`: Stores the expanded state of a collapsible content card by key.
- `getCollapsibleExpanded : function(string)`: Returns the stored expanded state of a collapsible content card by key.
- `firstWidgetRef : ReactRefWrap`
- `lastWidgetRef : ReactRefWrap`

### EowExtensionUtil.OwnershipState (enum)

```teal
enum OwnershipState
```

Ownership of an entity from the local player's point of view.

- `"None"`: The entity has no `PLAYER_OWNED` component.
- `"Player"`: Owned by the local player.
  In game: "Manage Line", "Manage Vehicles", "Configure" (German: „Linie verwalten“, „Fahrzeuge verwalten“, „Konfigurieren“)
- `"Foreign"`: Owned by another player.
