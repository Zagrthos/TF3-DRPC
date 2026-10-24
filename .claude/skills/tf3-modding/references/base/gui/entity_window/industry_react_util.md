# gui.entity_window.industry_react_util

Definition file: `base/tealdef/gui/entity_window/industry_react_util.d.tl` (in the game folder).

React recipes of the industry and perk windows that show production rules (inputs, outputs, progress and duration) and the workers boost.

## RuleDurationInfoParams (record)

```teal
global record RuleDurationInfoParams
```

Result of `IndustryReactUtil.GetRuleDurationTextAndBoosted`.

- `durationText : string`: Formatted duration of one production cycle (in days, or minutes and seconds when no day length is set), optionally with the change due to delivery time.
- `boosted : boolean`: True when the industry boost is above 1 (the rule runs faster).
- `slowdown : boolean`: True when the text includes a duration change due to delivery time.

## IndustryWindowRuleDurationParams (record)

```teal
global record IndustryWindowRuleDurationParams
```

Parameters of `IndustryReactUtil.IndustryWindowRuleDurationWidget`.

- `entity : Engine.Entity`: Entity with the `STOCK_LIST` component holding the production rules.
- `ruleNumber : integer`: 1-based index of the production rule.

## RuleProgressBarParams (record)

```teal
global record RuleProgressBarParams
```

Parameters of `IndustryReactUtil.RuleProgressBar`.

- `entity : Engine.Entity`: Entity with the `STOCK_LIST` component holding the production rules.
- `ruleNumber : integer`: 1-based index of the production rule; when nil the first running non-booster rule is shown.
- `durationAsTooltip : boolean`: Shows the cycle duration only as tooltip instead of a text below the bar.
- `invertProgressBar : boolean`: Shows the remaining instead of the elapsed part of the cycle.

## RuleInputOutputWidgetParams (record)

```teal
global record RuleInputOutputWidgetParams
```

Parameters of `IndustryReactUtil.IndustryWindowRuleInputOutputWidget`.

- `entity : Engine.Entity`: Entity with the production rules.
- `ruleNumber : integer`: 1-based index of the production rule.
- `isInput : boolean`: Shows the rule's inputs (true) or outputs (false).
- `recipe : Recipe<IndustryUtil.RuleInputOutputCargoParams>`: Recipe created once per cargo of the rule (with `index` set to the cargo's position).
- `isIndustry : boolean`: Passed on to `recipe`; true for industries.
- `onlyOneProductionLine : boolean`: Passed on to `recipe`; the base game sets it when the rule has no input cargo.

## RuleWidgetParams (record)

```teal
global record RuleWidgetParams
```

Parameters of `IndustryReactUtil.ProductionRuleWidget`.

- `entity : Engine.Entity`: Entity with the production rules.
- `ruleNumber : integer`: 1-based index of the production rule.
- `durationAsTooltip : boolean`: Not read by the widget, which always shows the duration as tooltip.
- `invertProgressBar : boolean`: Not read by the widget, which always shows the elapsed part.
- `isBoosted : boolean`: Adds the `boosted` style class.
- `isIndustry : boolean`: True for industries; adds the "Max." label when the rule has input cargo.

## RuleProgressBarParams (record)

```teal
global record RuleProgressBarParams
```

Parameters of `IndustryReactUtil.RuleProgressBar`.

- `entity : Engine.Entity`: Entity with the `STOCK_LIST` component holding the production rules.
- `ruleNumber : integer`: 1-based index of the production rule; when nil the first running non-booster rule is shown.
- `durationAsTooltip : boolean`: Shows the cycle duration only as tooltip instead of a text below the bar.
- `invertProgressBar : boolean`: Shows the remaining instead of the elapsed part of the cycle.

## ProductionRulesWidgetParams (record)

```teal
global record ProductionRulesWidgetParams
```

Parameters of `IndustryReactUtil.ProductionRulesWidget`.

- `entity : Engine.Entity`: Entity with the production rules.
- `isIndustry : boolean`: True for industries.
- `maxLevel : boolean`: Adds the note "Industry is expanded to it's full potential.".
- `failedToExpand : boolean`: Adds the note "Industry is blocked from further expansion." (when `maxLevel` is false).

## IconAndLabelParams (record)

```teal
global record IconAndLabelParams
```

Parameters of `IndustryReactUtil.IconAndLabelWidget`.

- `icon : string`: Image path of the icon.
- `iconTooltip : string`: Tooltip of the icon.
- `count : string`: Text next to the icon.
- `countTooltip : string`: Tooltip of the text.
- `annualCount : string`: optional
- `annualCountTooltip : string`: optional

## ProductionIconWithBubbleParams (record)

```teal
global record ProductionIconWithBubbleParams
```

Parameters of `IndustryReactUtil.ProductionIconWithBubble`.

- `icon : string`: Image path of the icon (e.g. a cargo icon).
- `iconTooltip : string`: Tooltip of the icon.
- `count : string`: Text in the bubble over the icon.
- `countTooltip : string`: Tooltip of the bubble.

## WorkplacesBoosterParams (record)

```teal
global record WorkplacesBoosterParams
```

Parameters of `IndustryReactUtil.WorkplacesBoosterRow`.

- `entity : Engine.Entity`: Entity with a `PERSON_CAPACITY` component (workplaces).
- `boosted : boolean`: Adds the `boosted` style class to the cells.

## WorkplacesBoosterMinimumIconParams (record)

```teal
global record WorkplacesBoosterMinimumIconParams
```

Parameters of the workers count icon in the workplaces booster row.

- `entity : Engine.Entity`: Entity with a `PERSON_CAPACITY` component.

## WorkplacesBoosterProgressBarParams (record)

```teal
global record WorkplacesBoosterProgressBarParams
```

Parameters of the workers progress bar in the workplaces booster row.

- `entity : Engine.Entity`: Entity with a `PERSON_CAPACITY` component.

## WorkplacesBoostFactorIconParams (record)

```teal
global record WorkplacesBoostFactorIconParams
```

Parameters of the boost factor icon in the workplaces booster row.

- `entity : Engine.Entity`: Entity with a `PERSON_CAPACITY` component.

## IndustryReactUtil (record)

```teal
global record IndustryReactUtil
```
Load: `local industry_react_util = ug_require "::/gui/entity_window/industry_react_util.tl" as IndustryReactUtil`

React recipes and helpers for production rules and the workers boost in the industry and perk windows.

- `makeIconAndLabel : function(icon : string, iconTooltip : string, count : string, countTooltip : string) : {TreeNodeId}`  (call as `industry_react_util.makeIconAndLabel(…)`)
- `isPersonCapacityBoosted : function(entity : Engine.Entity) : boolean`  (call as `industry_react_util.isPersonCapacityBoosted(…)`): Returns whether an entity with workplaces is currently boosted.
    - param `entity`: Entity with a `PERSON_CAPACITY` component.
    - returns: True if its industry boost is above 1; nil if it has no workplaces.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/perk/perk.tl:81`: `result.boosted = industry_react_util.isPersonCapacityBoosted(subconEntity)`
- `GetRuleDurationTextAndBoosted : function(entity : Engine.Entity, ruleNumber : integer, showDecayFactor? : boolean) : RuleDurationInfoParams`  (call as `industry_react_util.GetRuleDurationTextAndBoosted(…)`): Calculates the cycle duration of a production rule as text.
  In game: "{total} (+ {diff} Due to Delivery Time)", "{total} (- {diff} Due to Delivery Time)" (German: „{total} (+ {diff} aufgrund der Lieferzeit)“, „{total} (- {diff} aufgrund der Lieferzeit)“)
    - param `entity`: Entity with the `STOCK_LIST` component.
    - param `ruleNumber`: 1-based index of the rule.
    - param `showDecayFactor`: Adds the change of the duration caused by the delivery time of the inputs.
    - returns: Duration text and boost/slowdown flags.
    - notes: While the rule is running, the interval of its last application is used; otherwise one game year divided by the rule's capacity, divided by the industry boost for non-booster rules.
- `IndustryWindowRuleDurationWidget : Recipe<IndustryWindowRuleDurationParams>`: Text with the cycle duration of a production rule, styled `success` when boosted.
- `RuleProgressBar : Recipe<RuleProgressBarParams>`: Progress bar of the running cycle of a production rule, with the duration as tooltip or text; styled `slowdown` when delivery time slows it down.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:138`: `industry_react_util.RuleProgressBar{`
- `IndustryWindowRuleInputOutputWidget : Recipe<RuleInputOutputWidgetParams>`: Row with one `recipe` widget per input or output cargo of a production rule.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:126`: `industry_react_util.IndustryWindowRuleInputOutputWidget{`
- `ProductionRuleWidget : Recipe<RuleWidgetParams>`: One production rule as a bar split into inputs and outputs (cargo icons with amounts, current and maximum production) and a progress bar.
  In game: "Current Production", "No Input", "{count} Plots" (German: „Aktuelle Produktion“, „Keine Eingabe“, „{count} Parzellen“)
- `ProductionRulesWidget : Recipe<ProductionRulesWidgetParams>`: All non-booster production rules of an entity, plus a note when the industry is fully expanded or blocked from expansion.
  In game: "Industry is expanded to its full potential.", "Industry is blocked from further expansion." (German: „Maximale Industrieerweiterung erreicht.“, „Industrieerweiterung ist blockiert.“)
    - used 3x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:731`: `industry_react_util.ProductionRulesWidget,`
- `ProductionIconWithBubble : Recipe<ProductionIconWithBubbleParams>`: Icon with a small text bubble on top, e.g. a cargo icon with its amount.
- `IconAndLabelWidget : Recipe<IconAndLabelParams>`: Icon with a text next to it and an optional annual count below.
    - used 1x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:87`: `children[#children + 1] = industry_react_util.IconAndLabelWidget{`
- `WorkplacesBoosterRow : Recipe<WorkplacesBoosterParams>`: Table row showing the workers boost of an entity: current/required workers, a progress bar towards
the required number (capacity times `industry_productivity_util.RequiredWorkersFractionForBoost`),
the potential boost in percent and a tick icon.
    - used 2x in the base game, e.g. `base/content/gui/gui/entity_window/industry/industry_eow.script.tl:610`: `workerBoosterRows[#workerBoosterRows + 1] = industry_react_util.WorkplacesBoosterRow{`
