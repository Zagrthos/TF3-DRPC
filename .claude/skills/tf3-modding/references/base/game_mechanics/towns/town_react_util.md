# game_mechanics.towns.town_react_util

Definition file: `base/tealdef/game_mechanics/towns/town_react_util.d.tl` (in the game folder).

React recipes and label helpers for the town window and town rating icons.

## TownReactUtil (record)

```teal
global record TownReactUtil
```
Load: `local town_react_util = ug_require "::/game_mechanics/towns/town_react_util.tl" as TownReactUtil`

React recipes and label helpers for the town window (`game_mechanics/towns/town_react_util.tl`).

- `getRatingClass : function(RatingLevel) : string`  (call as `town_react_util.getRatingClass(…)`): CSS class of a rating level, `town-growth-rank-1` for `VeryPoor` up to `town-growth-rank-6` for `Excellent`.
    - param `#1`: Rating level.
    - returns: CSS class name.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town_eow.script.tl:100`: `class = "font-scale-headline, growth-state," .. town_react_util.getRatingClass(stepState:old().growthLevel),`
- `getStringForTownAuthorityEventFactors : function(TownAuthorityEventFactors.EventType) : string`  (call as `town_react_util.getStringForTownAuthorityEventFactors(…)`): Localized label of a penalty reason, such as "Removed Trees".
  In game: "Removed Trees", "Landscaping", "Failed Subsidy" (German: „Entfernte Bäume“, „Landschaftsbau“, „Gescheiterte Subvention“)
    - param `#1`: Penalty reason.
    - returns: Label, or `nil` for `Unknown`.
- `getAdviceForTownAuthorityEventFactors : function(TownAuthorityEventFactors.EventType, integer) : string`  (call as `town_react_util.getAdviceForTownAuthorityEventFactors(…)`): Localized advice to recover from a penalty, for trees, rocks and severed connections.
  In game: "Replant some trees to improve reputation.", "Place some rocks to improve reputation.", "Restore main connection to improve reputation." (German: „Pflanze neue Bäume, um den Ruf zu verbessern.“, „Platziere Felsen, um den Ruf zu verbessern.“, „Stelle die Hauptverbindungen wieder her, um den Ruf zu verbessern.“)
    - param `#1`: Penalty reason.
    - param `#2`: Number of severed main connections; the severed connection advice needs more than 0.
    - returns: Advice text, or `nil` for other reasons.
- `RatingBar : Recipe<RatingBarParams>`: Recipe of a segmented rating bar with an optional icon and rating level label.
    - used 2x in the base game, e.g. `base/content/gui/gui/layers/layer_react_util.tl:129`: `children[#children + 1] = town_react_util.RatingBar{`
- `getEditorSupplyDetails : function() : Recipe<TownReactUtil.TownDetailsParam>`  (call as `town_react_util.getEditorSupplyDetails(…)`): Recipe of the supplies card in the map editor's town window.
  In game: "Public transport coverage is {percent}.", "{percent} of demand for {cargoName} is currently met." (German: „Die Abdeckung des öffentlichen Verkehrsnetzes beträgt {percent}.“, „{percent} der Nachfrage nach "{cargoName}" wird derzeit gedeckt.“)
    - returns: Recipe taking `TownDetailsParam`.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town_eow.script.tl:455`: `town_react_util.getEditorSupplyDetails(),`
- `GetRatingsDashboard : function() : {TownInfoTabType : {TownUtil.Rating, Recipe<TownDetailsParam>, string}}`  (call as `town_react_util.GetRatingsDashboard(…)`): Content of the rating tabs of the town window. Each entry has the rating, the recipe of its detail widget and a hint text. `Overview` has no entry.
  In game: "Reputation Rating", "Traffic Rating", "Noise Rating" (German: „Bewertung des Rufs“, „Verkehrsbewertung“, „Lärmbewertung“)
    - returns: Rating, detail recipe and hint per tab.
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:672`: `local ratings = town_react_util.GetRatingsDashboard()`
- `TownRatingIconWithText : Recipe<TownRatingIconWithTextParams>`: Recipe of a town window tab button with the rating icon, coloured by the current rating level.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/town/town.tl:683`: `content = town_react_util.TownRatingIconWithText{`
- `getGrowthLevelText : function(growthLevel : RatingLevel) : string`  (call as `town_react_util.getGrowthLevelText(…)`): Localized growth text of a growth level, from "Stagnant" (`VeryPoor`) to "Growing Lightning Fast" (`Excellent`).
  In game: "Stagnant", "Growing", "Growing Lightning Fast" (German: „Stagnierend“, „Wachsend“, „Unfassbar schnell wachsend“)
    - param `growthLevel`: Growth level.
    - returns: Localized text.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town_eow.script.tl:102`: `text = town_react_util.getGrowthLevelText(stepState:old().growthLevel),`
- `TownPopulationWidget : Recipe<TownDetailsParam>`: Recipe of the expandable part of the town level card. It shows the experience needed for the next level, the next level's name, the supplies state and the growth rate.
  In game: "Town Authority", "Growth Rate:", "Earn {exp} Experience Points" (German: „Stadtverwaltung“, „Wachstumsrate:“, „Verdiene {exp} Erfahrungspunkte“)
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/town/town_eow.script.tl:136`: `town_react_util.TownPopulationWidget,`
- `TownRatingIcon : Recipe<TownRatingIconParams>`: Recipe of a rating icon coloured by the town's current level of that rating. The level comes from `rating.getRatingFnName` through `useStepStateParallelSimple`.
    - used 1x in the base game, e.g. `base/content/mission/mission/mission_framework/mission_framework_react_util.tl:151`: `return town_react_util.TownRatingIcon{`

### TownReactUtil.TownInfoTabType (enum)

```teal
enum TownInfoTabType
```

Tabs of the town window.

- `"Overview"`: Overview tab, titled "District Sizes".
  In game: "District Sizes" (German: „Gebietsgrössen“)
- `"Reputation"`: Reputation tab.
  In game: "Reputation" (German: „Ruf“)
- `"Bonuses"`: Bonuses tab (landmark bonuses).
  In game: "Bonuses" (German: „Boni“)
- `"Supplies"`: Supplies tab.
  In game: "Supplies" (German: „Versorgung“)
- `"Traffic"`: Traffic tab.
  In game: "Traffic" (German: „Verkehr“)
- `"Happiness"`: Happiness tab.
  In game: "Happiness", "Passenger Happiness Rating" (German: „Zufriedenheit“, „Bewertung der Passagierzufriedenheit“)
- `"Delivery"`: Delivery time tab.
  In game: "Delivery", "Cargo Supply Rating" (German: „Lieferung“, „Bewertung der Güterversorgung“)
- `"Noise"`: Noise tab.
  In game: "Noise" (German: „Lärm“)
- `"Pollution"`: Pollution tab.
  In game: "Pollution" (German: „Verschmutzung“)

### TownReactUtil.RatingBarParams (record)

```teal
record RatingBarParams
```

Params of `RatingBar`.

- `icon : string`: Icon path shown when `showIcon` is true.
- `tooltip : string`: Tooltip of the icon.
- `barTooltip : string`: Tooltip of the bar.
- `value : number`: Rating value between 0 and 1.
- `maxValue : number`
- `numFractions : integer`: Number of bar segments; the base game uses 5.
- `onClick : function()`: Called when the bar is clicked; when set, the bar is wrapped in a button.
- `hideText : boolean`: Hides the rating level label next to the bar.
- `showIcon : boolean`: Shows `icon` before the bar.

### TownReactUtil.TownDetailsParam (record)

```teal
record TownDetailsParam
```

Params of the town window detail widgets.

- `entity : Engine.Entity`: Town entity.
- `setLinesExpandedState : function(ratingKey : string, value : boolean)`: Reports whether the line list of a tab is expanded; the noise and pollution widgets call it.
    - param `ratingKey`: Tab key, `"Noise"` or `"Pollution"`.
    - param `value`: True when the list is expanded.

### TownReactUtil.TownRatingIconWithTextParams (record)

```teal
record TownRatingIconWithTextParams
```

Params of `TownRatingIconWithText`.

- `entity : Engine.Entity`: Town entity.
- `rating : TownUtil.Rating`: Rating to show.
- `text : string`: Headline text next to the icon.
- `tooltip : string`: Tooltip of the icon.
- `wideIcon : boolean`: Uses the wide layout with a large `TownRatingIcon`.
- `activeTab : boolean`: Marks the button as the active tab.

### TownReactUtil.TownRatingIconParams (record)

```teal
record TownRatingIconParams
```

Params of `TownRatingIcon`.

- `entity : Engine.Entity`: Town entity.
- `rating : TownUtil.Rating`: Rating to show.
- `tooltip : string`: Tooltip of the icon.
- `iconSize : IconSize`: Icon size.

#### TownReactUtil.TownRatingIconParams.IconSize (enum)

```teal
enum IconSize
```

Icon size of `TownRatingIcon`.

- `"Default"`: Uses `rating.icon`.
- `"Large"`: Uses `rating.iconLarge`.
