# gui.menu.menu_icon_react_util

Definition file: `base/tealdef/gui/menu/menu_icon_react_util.d.tl` (in the game folder).

Building blocks of the main menu pages (buttons, image cards, paged card grids, page frame, hint panel), loaded with `ug_require "/gui/menu/menu_icon_react_util.tl"`.

## MenuIconReactUtil (record)

```teal
global record MenuIconReactUtil
```
Load: `local menu_icon_react_util = ug_require "::/gui/menu/menu_icon_react_util.tl" as MenuIconReactUtil`

Layout helpers and recipes for main menu pages.

- `CardButton : Recipe<CardButtonParams>`: Large menu button with cycling background images and a label at the bottom.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/campaign_page.tl:160`: `table.insert(cards, menu_icon_react_util.CardButton{`
- `makeButton : function(text : string, onClick : function(), classes : string, enabled? : boolean, tooltip? : string, ref? : ReactRefWrap, textColorClass? : string, buttonClass? : string) : TreeNodeId`  (call as `menu_icon_react_util.makeButton(…)`): Creates a text button.
    - param `text`: Button text.
    - param `onClick`: Click handler.
    - param `classes`: Style classes of the button.
    - param `enabled`: Whether the button is enabled.
    - param `tooltip`: Tooltip text.
    - param `ref`: Optional node reference for the button.
    - param `textColorClass`: Additional style class of the text.
    - param `buttonClass`: Additional style class of the button, for example `primary` or `secondary`.
    - used 4x in the base game, e.g. `base/content/gui/gui/menu/mod_manager_react_util.tl:4709`: `menu_icon_react_util.makeButton(_("Reset Filters"), function()`
- `makeSecondaryButton : function(text : string, onClick : function(), classes? : string, enabled? : boolean, tooltip? : string, ref? : ReactRefWrap) : TreeNodeId`  (call as `menu_icon_react_util.makeSecondaryButton(…)`): Creates a text button with the `secondary` style class (see `makeButton`).
    - used 7x in the base game, e.g. `base/content/gui/gui/menu/advanced_settings.tl:578`: `menu_icon_react_util.makeSecondaryButton(`
- `makePrimaryButton : function(text : string, onClick : function(), classes? : string, enabled? : boolean, tooltip? : string, ref? : ReactRefWrap) : TreeNodeId`  (call as `menu_icon_react_util.makePrimaryButton(…)`): Creates a text button with the `primary` style class (see `makeButton`).
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/mod_selector_page.tl:295`: `not missingFilterState:old() and menu_icon_react_util.makePrimaryButton(_("Show All"),`
- `makeCardButton : function(text : string, onClick : function(), onAttention : function(x : number, y : number), tooltip : string, images : {string | Builtin.ImageViewParam}, initialImageIndex : integer, imageSwapOffsetSeconds? : number, displayDurationSeconds? : number, class? : string, extraChildren? : {TreeNodeId}, enabled? : boolean) : TreeNodeId`  (call as `menu_icon_react_util.makeCardButton(…)`): Creates a `CardButton` whose bottom label shows `text` as main card title.
    - used 7x in the base game, e.g. `base/content/gui/gui/menu/main_page.tl:556`: `local campaignCard = menu_icon_react_util.makeCardButton(`
- `makeTabWidget : function(elements : {TreeNodeId}, maxNumPerPage : integer, makeEmptyElem : (function(last : boolean) : TreeNodeId), numRows : integer, selectedTabState : ReactStateT<integer>, ref? : ReactRefWrap) : TreeNodeId`  (call as `menu_icon_react_util.makeTabWidget(…)`): Arranges elements in a grid of `numRows` rows with `maxNumPerPage` elements each. If they do not fit on one page, the pages become tabs with dot indicators, chevron buttons and mouse-wheel paging.
    - param `elements`: Elements to arrange.
    - param `maxNumPerPage`: Number of elements per row.
    - param `makeEmptyElem`: Creates the filler for empty cells; `last` is true for the last cell of a row.
    - param `numRows`: Number of rows per page (default 2).
    - param `selectedTabState`: State holding the index of the shown page.
    - param `ref`: Optional node reference.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/campaign_page.tl:250`: `table.insert(tabs, menu_icon_react_util.makeTabWidget(cards, campaign and maxMissionsPerPage or maxCampaignsPerPage, function( lastElem : boolean ) : TreeNodeId`
- `makePage : function(commonParams : CommonMenuParams, text : string, back : function(), centerItem : TreeNodeId, extra : {TreeNodeId}, campaignBriefingScreen? : boolean) : TreeNodeId`  (call as `menu_icon_react_util.makePage(…)`): Creates the frame of a menu page with a background canvas, an optional centred item and a top bar with title and back button.
    - param `commonParams`: Common parameters of the menu page.
    - param `text`: Page title; no top bar is created if `nil`.
    - param `back`: Called by the back button and the `IA_MENU_BACK` input action.
    - param `centerItem`: Item placed in the centre of the page.
    - param `extra`: Additional floating layout children.
    - param `campaignBriefingScreen`: Whether to use the campaign briefing style for the top bar.
    - used 15x in the base game, e.g. `base/content/gui/gui/menu/achievements_page.tl:14`: `return menu_icon_react_util.makePage(`
- `makeCardLabelBottomComponent : function(textTitle : string, textDescription : string, textRight : string, postTitleChild : TreeNodeId, mainTitleCard : boolean) : TreeNodeId`  (call as `menu_icon_react_util.makeCardLabelBottomComponent(…)`): Creates the bottom label of a card with title, optional description and optional right-aligned text on an effect background.
    - param `textTitle`: Title text.
    - param `textDescription`: Optional description below the title.
    - param `textRight`: Optional text on the right.
    - param `postTitleChild`: Optional element placed after the title.
    - param `mainTitleCard`: Whether to use the large title font of main cards.
    - used 2x in the base game, e.g. `base/content/gui/gui/menu/campaign_page.tl:147`: `return menu_icon_react_util.makeCardLabelBottomComponent(`
- `makeMainOuterCard : function(addSpacer : boolean, children : {TreeNodeId}) : TreeNodeId`  (call as `menu_icon_react_util.makeMainOuterCard(…)`): Wraps elements in the vertical `main-outer-card` container, optionally with a top spacer.
    - used 8x in the base game, e.g. `base/content/gui/gui/menu/achievements_page.tl:22`: `menu_icon_react_util.makeMainOuterCard(`
- `makeTabAnalogue : function(headerChildren : {TreeNodeId}, bodyChildren : {TreeNodeId}) : TreeNodeId`  (call as `menu_icon_react_util.makeTabAnalogue(…)`): Creates a container that looks like a tab widget, with a horizontal header row and a vertical body.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/achievements_page.tl:25`: `menu_icon_react_util.makeTabAnalogue(`
- `HintWidget : RecipeWithApi<HintWidgetParam, HintWidgetApi>`: Panel showing the title and description of the hint for the hovered item of the selected tab.
    - used 6x in the base game, e.g. `base/content/gui/gui/menu/advanced_settings.tl:71`: `local hintWidgetRef = react.useNodeRef(menu_icon_react_util.HintWidget)`
- `HintImage : RecipeWithApi<HintWidgetParam, HintWidgetApi>`: Default hint image of the settings pages. Its API functions do nothing.
    - used 3x in the base game, e.g. `base/content/gui/gui/menu/advanced_settings.tl:625`: `menu_icon_react_util.HintImage{`

### MenuIconReactUtil.CardButtonParams (record)

```teal
record CardButtonParams
```

Parameters of `CardButton`.

- `bottomComponent : TreeNodeId`: Content of the label at the bottom of the card, for example from `makeCardLabelBottomComponent`.
- `onClick : function()`: Called when the card is clicked.
- `onAttention : function(x : number, y : number)`: Called when the card gets attention (hover or focus), with a position on the card (from the node's `getPosition`) for placing a popup next to it.
- `tooltip : string`: Tooltip of the card.
- `images : {string | Builtin.ImageViewParam}`: Background images (paths or image view parameters). With more than one image they are shown in turn with a cross-fade.
- `initialImageIndex : integer`: Index of the first image to show (default 1).
- `imageSwapOffsetSeconds : number`: Time offset of the image cycle in seconds, to keep several cards from switching at the same moment (default 0).
- `displayDurationSeconds : number`: Time each image is shown, in seconds (default 24).
- `class : string`: Style class of the card (default `small-card`); `main-menu-card` is always added.
- `extraChildren : {TreeNodeId}`: Additional floating layout children placed on the card.
- `localKey : string`: Local key of the card button.
- `enabled : boolean`: Whether the card is enabled.
- `clipper : boolean`: Whether to clip the images to the card.

### MenuIconReactUtil.HintWidgetParam (record)

```teal
record HintWidgetParam
```

Parameters of `HintWidget` and `HintImage`.

- `tabId2Hints : {any : {Data}}`: integer|number|string
- `selectedTab : any`: integer|number|string
- `placeHolderHint : Data`: Hint shown when no item is hovered or the tab has no hints.

#### MenuIconReactUtil.HintWidgetParam.Data (record)

```teal
record Data
```

Text of one hint.

- `title : string`: Hint title.
- `description : string`: Hint text.
- `imagePath : string`: Image of the hint; set by the settings page but not displayed by `HintWidget` or `HintImage`.

### MenuIconReactUtil.HintWidgetApi (record)

```teal
record HintWidgetApi
```

API of `HintWidget` for setting the hovered item.

- `getHovered : function() : integer`: Returns the index of the hovered item (`0` for none).
- `setHovered : function(index : integer)`: Sets the index of the hovered item, whose hint is then shown.
