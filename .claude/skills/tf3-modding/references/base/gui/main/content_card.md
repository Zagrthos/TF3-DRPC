# gui.main.content_card

Definition file: `base/tealdef/gui/main/content_card.d.tl` (in the game folder).

Content cards: the titled boxes that make up entity windows and similar panels. A card
has a permanent part, an optional collapsible part that expands on click, and optional
callout texts shown next to it on hover or focus.

## ContentCard (record)

```teal
global record ContentCard
```
Load: `local content_card = ug_require "::/gui/main/content_card.tl" as ContentCard`

Content card recipes and helpers.

Example:
```lua
-- from sim_person_eow.script.tl
content_card.ContentCard{
    title = _("Locations"),
    recipeAndParamPermanent = content_card.makeRecipeAndParam(
        DestinationsTable,
        { personEntityId = extensionParams.entityId }
    ),
    gameCtx = extensionParams.gameCtx,
    showOnRightSide = extensionParams.showCalloutOnRightSide,
}
```

- `InfoCalloutWrapper : Recipe<CalloutWrapperParam>`: Wraps content (a recipe plus extra children) and shows `calloutText` in a callout beside it while it has attention.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/entity_window_util.tl:229`: `content_card.InfoCalloutWrapper{`
- `ContentCard : RecipeWithApi<ContentCardParam, ContentCardApi>`: A titled card with a permanent part, an optional collapsible part and optional callouts.
    - notes: When both a permanent and a collapsible part exist, the permanent part becomes the button that expands and collapses the card (with an arrow). The expanded state is owned by the caller: pass `collapsibleExpanded` and update it in `setCollapsibleExpanded`, e.g. with the functions from `makeContentCardsCollapsibleFunctions`.
    - used 98x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/company.tl:598`: `scrollAreaChildren[#scrollAreaChildren + 1] = content_card.ContentCard{`
- `makeRecipeAndParam : function<T>(recipe : Recipe<T>, param : T) : RecipeAndParamErased`  (call as `content_card.makeRecipeAndParam(…)`): Bundles a recipe and its parameter into a `RecipeAndParamErased` for a content card.
    - used 61x in the base game, e.g. `base/content/game_mechanics/game_mechanics/company/explorations/exploration_plane_entity_window.script.tl:48`: `recipeAndParamPermanent = content_card.makeRecipeAndParam(`
- `makeRecipe0 : function(recipe : Recipe0) : RecipeAndParamErased`  (call as `content_card.makeRecipe0(…)`): Bundles a recipe without parameters into a `RecipeAndParamErased` (with an empty parameter table).
    - used 5x in the base game, e.g. `base/content/gui/gui/game_bar/game_bar_widgets.tl:783`: `recipeAndParamPermanent = content_card.makeRecipe0(CalendarWeatherControl),`
- `makeContentCardsCollapsibleFunctions : function(collapsibleCardsState : ReactStateT<{string : boolean}>, onlyOneExpandable : boolean) : function(string, boolean), function(string)`  (call as `content_card.makeContentCardsCollapsibleFunctions(…)`): Creates the expand/collapse functions for several content cards that share one state table keyed by card.
    - param `collapsibleCardsState`: React state holding the expanded flag per card key.
    - param `onlyOneExpandable`: When `true`, expanding one card collapses all others.
    - returns: `setExpanded(key, expanded)` and `isExpanded(key)`.

    ```lua
    -- from sim_person.tl
    local collapsibleCardsState = react.useState({})
    local updateCollapsibleState, isExpanded = content_card.makeContentCardsCollapsibleFunctions(collapsibleCardsState, true)
    -- per card:
    --   setCollapsibleExpanded = function(v) updateCollapsibleState(cardKey, v) end,
    --   collapsibleExpanded = isExpanded(cardKey),
    ```

    - used 14x in the base game, e.g. `base/content/gui/gui/entity_window/animal/animal.tl:32`: `local updateCollapsibleState, isExpanded = content_card.makeContentCardsCollapsibleFunctions(collapsibleCardsState, true)`
- `useShowCalloutOnRight : function(refWindow : ReactRefWrap) : boolean`  (call as `content_card.useShowCalloutOnRight(…)`): Hook that returns `true` while the given window is in the left half of the screen, so callouts should open on its right side.
    - used 15x in the base game, e.g. `base/content/gui/gui/entity_window/animal/animal.tl:29`: `local showOnRightSide = content_card.useShowCalloutOnRight(refSelf)`
- `makeInfoHint : function(hint : string, addIcon : boolean) : TreeNodeId`  (call as `content_card.makeInfoHint(…)`): Creates a small hint text, optionally with an info icon in front of it.
    - used 6x in the base game, e.g. `base/content/game_mechanics/game_mechanics/finance/finances_cargo_lifetime.tl:339`: `content_card.makeInfoHint(`

### ContentCard.CalloutWrapperParam (record)

```teal
record CalloutWrapperParam is IRecipeParamWithMeta
```

Parameters of `InfoCalloutWrapper`.

- `recipeAndParam : ContentCard.RecipeAndParamErased`: Main content, built from a recipe and its parameter.
- `extraChildren : {TreeNodeId}`: Note: only use these for elements that are always visible
- `separatorBetweenExtraChildren : boolean`: When `true`, a separator is placed between the `extraChildren`.
- `calloutText : string`: Text shown in a callout while the content has attention (hover or focus); empty text means no callout.
- `gameCtx : GameContext`: Game context; its `calloutContainerRef` is used to open the callout.
- `showOnRightSide : boolean`: When `true`, the callout opens to the right of the content, otherwise to the left.
- `isPermanent : boolean`: Marks the permanent part of a card (adds the card outline and formatting and makes it focusable).
- `isCollapsibleButton : boolean`: Marks content that is also the expand/collapse button of the card (style class `is-button`).
- `hovering : boolean`: Adds the style class `hovering`.

### ContentCard.RecipeAndParamErased (record)

```teal
record RecipeAndParamErased
```

A recipe together with its parameter, stored without its parameter type; create it with `makeRecipeAndParam` or `makeRecipe0`.

- `recipe : Recipe<any>`: The recipe to call.
- `param : any`: The parameter passed to the recipe.

### ContentCard.ContentCardParam (record)

```teal
record ContentCardParam
```

Parameters of `ContentCard`.

- `title : string`: Card title, shown as headline above the card; `nil` for no title.
- `titleOnHoverFocus : function(isSelected : boolean)`: Called with `true`/`false` when the title gains or loses attention (hover or focus).
- `initialCalloutTextPermanent : string`: Initial callout text of the permanent part; change it later with `ContentCardApi.setPermanentCalloutText`.
- `recipeAndParamPermanent : RecipeAndParamErased`: Content of the permanent (always visible) part.
- `extraChildrenPermanent : {TreeNodeId}`: Additional nodes appended to the permanent part.
- `separatorBetweenExtraChildren : boolean`: When `true`, a separator is placed between the `extraChildrenPermanent`.
- `hasPermanentFocusables : boolean`: Set to `true` when the permanent part contains focusable elements, so that they stay focusable although the part is the collapse button.
- `initialCalloutTextCollapsible : string`: Initial callout text of the collapsible part; change it later with `ContentCardApi.setCollapsibleCalloutText`.
- `recipeAndParamCollapsible : RecipeAndParamErased`: Content of the collapsible part, shown only while `collapsibleExpanded` is `true`.
- `setCollapsibleExpanded : function(boolean)`: Called with the new expanded state when the user expands or collapses the card (click, or `IA_MENU_BACK` while expanded).
- `collapsibleExpanded : boolean`: Whether the collapsible part is currently expanded; the card itself keeps no expanded state.
- `gameCtx : GameContext`: Game context, used to open the callouts.
- `showOnRightSide : boolean`: When `true`, callouts open to the right of the card (see `useShowCalloutOnRight`).

### ContentCard.ContentCardApi (record)

```teal
record ContentCardApi
```

API of a `ContentCard` node, available through its node reference.

- `setPermanentCalloutText : function(string)`: Replaces the callout text of the permanent part (`nil` is ignored).
- `setCollapsibleCalloutText : function(string)`: Replaces the callout text of the collapsible part (`nil` is ignored).
