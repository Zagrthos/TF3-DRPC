# game_mechanics.fun_elements.fun_elements

Definition file: `base/tealdef/game_mechanics/fun_elements/fun_elements.d.tl` (in the game folder).

State and helpers of the fun elements game script (airships over towns, UFOs abducting cows).

## Globals

- `BlimpUtil` (type): Airship that flies to a town when a marketing campaign starts there, circles it while the campaign runs, and then leaves the map.
    - notes: The fun elements game script spawns one on the `Towns` event `startMarketingCampaign` and calls `update` for it every frame.
- `UfoUtil` (type): UFO that flies in from the map edge, lifts a cow from a livestock farm field and flies off again.
    - notes: The fun elements game script spawns one every 2 to 4 game years while no other UFO is in flight.

## FunElementsState (record)

```teal
global record FunElementsState
```

Saved state of the game script `::/game_mechanics/fun_elements/fun_elements.gs`.

- `blimps : {BlimpUtil.BlimpData}`: Airships in flight.
- `ufos : {UfoUtil.UfoData}`: UFOs in flight.
- `spawnNextUfoAtGameTime : number`: Game time (ms) of the next UFO, 2 to 4 game years after the previous one is gone.

## FunElementType (enum)

```teal
global enum FunElementType
```

Kind of a fun element entity.

- `"Blimp"`: Airship.
  In game: "Airship" (German: „Luftschiff“)
- `"Ufo"`: UFO.
- `"Cow"`: Cow spawned for a UFO to abduct.
  In game: "Cow" (German: „Kuh“)

## FunElementsUtil (record)

```teal
global record FunElementsUtil
```
Load: `local fun_elements_util = ug_require "::/game_mechanics/fun_elements/fun_elements_util.tl" as FunElementsUtil`

Access to the fun elements state.

- `externalGetFunElementsState : function() : FunElementsState`  (call as `fun_elements_util.externalGetFunElementsState(…)`): Reads the state of the fun elements game script.
    - returns: The state.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/fun_elements/fun_elements_eow.script.tl:23`: `local funElementState = fun_elements_util.externalGetFunElementsState()`
- `getFunElementType : function(entity : Engine.Entity) : FunElementType`  (call as `fun_elements_util.getFunElementType(…)`): Kind of fun element an entity is. The tooltip and the entity window use it.
    - param `entity`: Entity to check.
    - returns: The kind, or `nil` if the entity is no fun element.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/make_entity_window.tl:688`: `local funElementType = fun_elements_util.getFunElementType(entity)`
- `funElementTypeToString : function(elementType : FunElementType) : string`  (call as `fun_elements_util.funElementTypeToString(…)`): Translated name of a fun element kind ("Airship", "UFO", "Cow").
  In game: "Airship", "Cow" (German: „Luftschiff“, „Kuh“)
    - param `elementType`: Kind.
    - returns: The name, or `nil` for unknown kinds.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/make_entity_window.tl:704`: `fallbackTitle = fun_elements_util.funElementTypeToString(funElementType),`
