# gui.main.menu_category_util

Definition file: `base/tealdef/gui/main/menu_category_util.d.tl` (in the game folder).

Filtering of selectable objects (bridges, tunnels, street types, ...) by menu category.
Filter categories are `menu_filter_category` resources; objects carry a `menuCategory`
in their metadata. Includes the filter panel, the object picker and helpers that list the
objects available in a given year.

## MenuCategoryUtil (record)

```teal
global record MenuCategoryUtil
```
Load: `local menu_category_util = ug_require "::/gui/main/menu_category_util.tl" as MenuCategoryUtil`

Menu category filtering helpers and recipes.

- `filterCategoryLess : function(a : FilterCategory, b : FilterCategory) : boolean`  (call as `menu_category_util.filterCategoryLess(…)`): Sort comparator for filter categories. `"*"` comes first, then sorting is by `order`, `name` and `category`.
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:2316`: `table.sort(categories, menu_category_util.filterCategoryLess)`
- `fetchMenuFilterCategories : function() : {string : FilterCategory}`  (call as `menu_category_util.fetchMenuFilterCategories(…)`): Loads all `menu_filter_category` resources.
    - returns: The filter categories, keyed by `category`.
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:4341`: `menuCategoryDataRef:set(menu_category_util.fetchMenuFilterCategories())`
- `CategoryFilterPanel : Recipe<CategoryFilterPanelParam>`: Row of toggle buttons, one per filter category; hidden when the only categories are `"*"` and one other.
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:2390`: `local categoryFilterPanelRef : ReactRefWrapT<MenuCategoryUtil.CategoryFilterPanel> = react.useNodeRef(nil)`
- `ObjectFilterBox : Recipe<ObjectFilterBoxParam>`: Object picker. It shows a category filter panel above a flow of toggle buttons with the objects' icons (and speed limit, if any).
  In game: "Change Type" (German: „Typ ändern“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/bridge_and_tunnel.tl:215`: `menu_category_util.ObjectFilterBox{`
- `FilterObjectsCalloutButton : Recipe<ObjectsCalloutButtonParam>`: Button showing the icon of the current object that opens an `ObjectFilterBox` in a callout.
  In game: "Change Type" (German: „Typ ändern“)
    - used 1x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:1179`: `isFilterObjectCalloutButton and menu_category_util.FilterObjectsCalloutButton{`
- `createNoObject : function(objectType : ObjectType) : FilterObject`  (call as `menu_category_util.createNoObject(…)`): Creates the "No Bridge" or "No Tunnel" entry (id `-1`, always available, sorted first).
  In game: "No Bridge", "No Tunnel" (German: „Keine Brücke“, „Kein Tunnel“)
    - returns: The entry, or `nil` for object types without such an entry.
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:310`: `availableObjects[1] = menu_category_util.createNoObject(objectType)`
- `getIds : function(objects : {MenuCategoryUtil.FilterObject}) : {integer}`  (call as `menu_category_util.getIds(…)`): Returns the ids of the given objects, in the same order.
    - used 5x in the base game, e.g. `base/content/gui/gui/construction/construction.tl:1081`: `local newFilterObjectIds = (newApiParams and newApiParams.filterObjects) and menu_category_util.getIds(newApiParams.filterObjects) or nil`
- `getAvailableObjectsSorted : function(rep : ResTypeRep<integer, table>, year : integer, previousId? : integer) : {FilterObject}`  (call as `menu_category_util.getAvailableObjectsSorted(…)`): get all objects based on rep which are available at the given year, sorted by the types inherent MenuCategory order
    - param `rep`: repository
    - param `year`: year in-game
    - param `previousId`: optional id of the object which was previously selected, if it is available it will be included in the result regardless of the current year
    - used 2x in the base game, e.g. `base/content/gui/gui/construction/construction_react_util.tl:314`: `local available = menu_category_util.getAvailableObjectsSorted(rep as ResTypeRep<integer, table>, api.engine.util.getYear())`

### MenuCategoryUtil.FilterCategory (record)

```teal
record FilterCategory
```

A filter category, as defined by a `menu_filter_category` resource.

Example:
```lua
-- gui/construction/menu/filter_categories/bridge_suspension.res.lua
function data()
    return {
        type = "menu_filter_category",
        data = {
            category = "bridge_suspension",
            icon = resolve("bridge_suspension.tga"),
            name = _("Suspension"),
            order = 16300,
        }
    }
end
```

- `category : string`: Category key, referenced by the `filterCategories` of an object's menu category; `"*"` stands for all.
- `name : string`: Display name (tooltip of the filter button).
- `order : number`: Sort order; lower values come first.
- `icon : string`: Icon path of the filter button.

### MenuCategoryUtil.CategoryFilterPanelParam (record)

```teal
record CategoryFilterPanelParam
```

Parameters of `CategoryFilterPanel`.

- `categories : {MenuCategoryUtil.FilterCategory}`: Categories to show, one toggle button each.
- `updateSelection : function(index : integer, isSelected : boolean)`: Called with the 1-based index of a category and its new selected state when the user toggles it.
- `selection : {boolean}`: Selected state per entry of `categories`.
- `isListEmpty : function() : boolean`: Returns whether the filtered list is empty; while it is, `IA_OPTION2` cannot leave the panel.
- `giveUpFocus : function()`: Called on `IA_OPTION2` or `IA_MENU_BACK` to move the focus away from the panel (e.g. back to the object list).

### MenuCategoryUtil.FilterObject (record)

```teal
record FilterObject
```

An object that can be picked in an `ObjectFilterBox` (usually created by `getAvailableObjectsSorted`).

- `id : integer`: must be unique within a provided set!
- `name : string`: Display name (from the object's description).
- `iconPath : string`: Icon path (from the object's description).
- `availability : ModelMetadata.Availability`: Years in which the object is available; used for sorting and the year check.
- `menuCategory : ModelMetadata.MenuCategory`: Menu category metadata of the object; its `filterCategories` decide in which filter categories it appears, the order of its first category decides the sort order.
- `speedLimit : number`: optional
- `carriers : {Carrier}`: optional
- `isAutoSelectable : boolean`: optional

### MenuCategoryUtil.ObjectFilterBoxParam (record)

```teal
record ObjectFilterBoxParam
```

Parameters of `ObjectFilterBox`.

- `objectsSorted : {FilterObject}`: All objects to choose from, already sorted.
- `currentObjectId : integer`: Id of the selected object; when it is not in the list, the first object is selected via `onSelectionChange`.
- `onSelectionChange : function(selectedObjectId : integer)`: Called with the id of the newly selected object.
- `onFilterChange : function(filteredObjectIds : {integer})`: Called with the ids of the objects that pass the category filter after it changed.
- `tooltip : string`: Tooltip of the button of `FilterObjectsCalloutButton`.

### MenuCategoryUtil.ObjectsCalloutButtonParam (record)

```teal
record ObjectsCalloutButtonParam
```

Parameters of `FilterObjectsCalloutButton`.

- `filterBoxParam : ObjectFilterBoxParam`: Parameters of the object picker shown in the callout.
- `gameCtx : GameContext`: Game context; its callout container is used.

### MenuCategoryUtil.ObjectType (enum)

```teal
enum ObjectType
```

Kinds of objects for which `createNoObject` can create a "none" entry.

- `"Bridge"`: Bridge type ("No Bridge").
  In game: "No Bridge" (German: „Keine Brücke“)
- `"Tunnel"`: Tunnel type ("No Tunnel").
  In game: "No Tunnel" (German: „Kein Tunnel“)
- `"RailroadCrossing"`: Railroad crossing type; `createNoObject` currently returns `nil` for it.
