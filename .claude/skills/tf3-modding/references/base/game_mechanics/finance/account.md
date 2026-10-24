# game_mechanics.finance.account

Definition file: `base/tealdef/game_mechanics/finance/account.d.tl` (in the game folder).

Params of the finances window.

## FinanceToolParam (record)

```teal
global record FinanceToolParam is ToolReactUtil.IToolParam
```
Load: `local FinanceTool = ug_require "::/game_mechanics/finance/account.tl" as FinanceToolParam`

Params of the finances window.

- `initialTabKey : TabKey`: Tab shown when the window opens.
- `isMoneyEditable : boolean`: Shows the money editor (sandbox and similar modes).

### FinanceToolParam.TabKey (enum)

```teal
enum TabKey
```

Tab of the finances window.

- `"Overview"`: Overview.
- `"Finances"`: Income and expenses.
- `"Assets"`: Assets.
- `"Loans"`: Loans.
- `"CargoLifetime"`: Cargo statistics over the game's lifetime.
