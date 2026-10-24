# game_mechanics.subventions.subventions_gui

Definition file: `base/tealdef/game_mechanics/subventions/subventions_gui.d.tl` (in the game folder).

Window content for a single subsidy.

## SubventionsGui (record)

```teal
global record SubventionsGui
```
Load: `local subventions_gui = ug_require "::/game_mechanics/subventions/subventions_gui.tl" as SubventionsGui`

Builder of the subsidy window.

- `makeSubventionWindowContent : function(tagPrefix : string, nonEntity : string, gameCtx : GameContext, isMapEditor : boolean, isSandboxMode : boolean, setActionFnAndProxy : {function(actionFn? : (function() : TreeNodeId), key2? : string), any}, closeFnAndProxy : {function(), any}, extraParam : any) : ViewManager.MakeEntityWindowResult`  (call as `subventions_gui.makeSubventionWindowContent(…)`): Builds the window content for one subsidy. The non-entity window calls it for keys that start with `subsidy_`.
  In game: "Task", "On Acceptance", "On Fulfillment" (German: „Aufgabe“, „Bei Annahme“, „Bei Erfüllung“)
    - param `tagPrefix`: Prefix of the GUI tag; the content gets `tagPrefix .. "subsidy"`.
    - param `nonEntity`: Key of the non-entity window.
    - param `gameCtx`: Game context.
    - param `isMapEditor`: True in the map editor.
    - param `isSandboxMode`: True in sandbox mode.
    - param `setActionFnAndProxy`: Function that sets the window's action button, and its compare proxy.
    - param `closeFnAndProxy`: Function that closes the window, and its compare proxy.
    - param `extraParam`: The `ISubvention` to show.
    - returns: Recipe, its parameters and the subsidy title as fallback window title.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/make_non_entity_window.tl:21`: `return subventions_gui.makeSubventionWindowContent(tagPrefix, nonEntity, gameCtx, isMapEditor, isSandboxMode, setActionFnAndProxy, closeFnAndProxy, extraParam)`
