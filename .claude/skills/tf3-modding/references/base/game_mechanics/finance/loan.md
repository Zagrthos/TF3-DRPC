# game_mechanics.finance.loan

Definition file: `base/tealdef/game_mechanics/finance/loan.d.tl` (in the game folder).

Loans offered to and taken by the player (state of `loan.gs`).

## LoanType (enum)

```teal
global enum LoanType
```

Size class of a loan.

- `"Small"`: Small loan (1 to 5 years, 3 to 5 % interest).
- `"Medium"`: Medium loan (3 to 8 years, 5 to 8 %).
- `"Large"`: Large loan (5 to 12 years, 8 to 10 %).
- `"ExtraLarge"`: Extra-large loan (10 to 24 years, 10 to 12 %).
- `"Custom"`: Custom loan (not offered in the base game).
  In game: "Custom" (German: „Benutzerdefiniert“)

## Loan (record)

```teal
global record Loan
```

A loan offer or a taken loan.

- `type : LoanType`: Size class.
- `amount : integer`: Borrowed amount in game currency.
- `duration : integer`: Term in game time (ms).
- `percentage : number`: Interest as a fraction of the amount, paid in equal monthly parts over the term.
- `lastPayDay : integer`: Game time (ms) of the last monthly payment.
- `timesPaid : integer`: Monthly payments made.
- `birthDay : integer`: Game time (ms) at which the offer was created; offers are renewed after half a game year.
- `cooldownUntil : integer`: Game time (ms) until which a taken offer slot stays empty (4 to 8 game months).
- `id : integer`: Id of a taken loan.

## LoanTable (record)

```teal
global record LoanTable
```

State of the loan game script.

- `availableLoans : {Loan}`: Current offers, one per size class.
- `obtainedLoans : {Loan}`: Loans taken by the player.
- `freeId : integer`: Next free loan id.
