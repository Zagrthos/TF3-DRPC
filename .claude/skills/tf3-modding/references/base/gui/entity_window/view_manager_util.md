# gui.entity_window.view_manager_util

Definition file: `base/tealdef/gui/entity_window/view_manager_util.d.tl` (in the game folder).

Helpers that decide whether and for which entity a window is opened when an entity is selected.

## ViewManagerUtil (record)

```teal
global record ViewManagerUtil
```
Load: `local view_manager_util = ug_require "::/gui/entity_window/view_manager_util.tl" as ViewManagerUtil`

Helpers that decide whether and for which entity an entity window is opened.

- `baseNodeCanToggleDoubleSlipSwitch : function(Engine.Entity) : boolean`  (call as `view_manager_util.baseNodeCanToggleDoubleSlipSwitch(…)`): Returns whether a track node is a double slip switch that can be toggled.
    - param `#1`: Node entity.
    - returns: True if exactly four track segments meet at the node and exactly two pairs of them are nearly straight (angle between about 3 and 22 degrees).
    - used 4x in the base game, e.g. `base/content/gui/gui/entity_window/make_entity_window.tl:651`: `meta = { tag = tag .. (view_manager_util.baseNodeCanToggleDoubleSlipSwitch(entity) and "doubleSlipSwitch" or "crossing")},`
- `baseNodeCanConfigTrafficLight : function(Engine.Entity) : boolean`  (call as `view_manager_util.baseNodeCanConfigTrafficLight(…)`): Returns whether a street node can have traffic lights configured.
    - param `#1`: Node entity.
    - returns: True if more than two street segments meet at the node.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/double_slip_switch.tl:629`: `if view_manager_util.baseNodeCanConfigTrafficLight(param.keyEntity) then`
- `wouldCreateView : function(entity : Engine.Entity, componentFilter? : {Engine.ComponentType : boolean}, withSoundEffect? : boolean) : Engine.Entity, {string}, boolean, Engine.Entity`  (call as `view_manager_util.wouldCreateView(…)`): Resolves which entity's window is opened when an entity is selected.
    - param `entity`: Selected entity (e.g. the entity under the mouse).
    - param `componentFilter`: Optional set of component types; only these kinds of windows are considered.
    - param `withSoundEffect`: Also look up the `select` sound effect of the entity.
    - returns: The entity whose window is shown (e.g. the owning vehicle of a carriage, the station group of a
station, the construction of a subconstruction) or nil if no window exists; the select sound
effect file list; whether the title renames the construction instead of the subconstruction; and,
for subconstructions, the original entity.

    - notes: Used by the view manager to open windows and by the HUD to decide which entities are hoverable.
    - used 6x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_hud.tl:873`: `local entity = view_manager_util.wouldCreateView(hoveredEntity, hudIconFilterSetRef:get(), false)`
- `makeEntityReplaceAndComponentTypeFilter : function(componentTypes : {Engine.ComponentType}) : function(Engine.Entity) : boolean`  (call as `view_manager_util.makeEntityReplaceAndComponentTypeFilter(…)`): Creates a filter that accepts entities for which a window of one of the given component types would be opened.
    - param `componentTypes`: Component types to accept.
    - returns: Function returning true when `wouldCreateView(entity, componentTypes)` finds a window.
    - used 4x in the base game, e.g. `base/content/gui/gui/layers/layer_noise.tl:210`: `filter = view_manager_util.makeEntityReplaceAndComponentTypeFilter(layer_noise.getHudIconPreFilter()),`
