# game_mechanics.finance.loan_util

Definition file: `base/tealdef/game_mechanics/finance/loan_util.d.tl` (in the game folder).

Creation and payment calculation of loans.

## LoanUtil (record)

```teal
global record LoanUtil
```

Creation and payment calculation of loans.

- `maximalObtainableLoans : integer`: Maximum number of loans at the same time (4).
- `createSmallLoan : function() : Loan`: Creates a random small offer. The amount range scales with the game year (1900 to 2020).
    - returns: Loan offer.
- `createMediumLoan : function() : Loan`: Creates a random medium offer.
    - returns: Loan offer.
- `createLargeLoan : function() : Loan`: Creates a random large offer.
    - returns: Loan offer.
- `createExtraLargeLoan : function() : Loan`: Creates a random extra-large offer.
    - returns: Loan offer.
- `createAvailableLoans : function() : {Loan}`: Creates four offers labelled Small, Medium, Large and ExtraLarge, each with random terms of any size class.
    - returns: Loan offers.
- `createNewObtainedLoan : function(Loan, integer) : Loan`: Turns an offer into a taken loan.
    - param `#1`: Offer to copy.
    - param `#2`: Loan id.
    - returns: Taken loan with `timesPaid = 0`.
- `getDurationInMonths : function(integer) : integer`: Converts a term into game months.
    - param `#1`: Term in ms.
    - returns: Number of months, rounded.
- `getMonthlyLoanAndInterestWithErrors : function(integer, integer, number) : integer, integer, integer, integer`: Splits a loan into rounded monthly repayment and interest, plus the rounding remainders.
    - param `#1`: Amount.
    - param `#2`: Term in ms.
    - param `#3`: Interest fraction.
    - returns: Monthly repayment, monthly interest, repayment remainder, interest remainder.
- `getMonthlyPaymentRounded : function(integer, integer, number, integer) : integer`: Returns the monthly payment (repayment plus interest) rounded to a step.
    - param `#1`: Amount.
    - param `#2`: Term in ms.
    - param `#3`: Interest fraction.
    - param `#4`: Rounding step (0 returns 0).
    - returns: Monthly payment.
- `getAnnualPaymentRounded : function(integer, integer, number) : integer`: Returns the yearly payment (repayment plus interest), counting at least one year.
    - param `#1`: Amount.
    - param `#2`: Term in ms.
    - param `#3`: Interest fraction.
    - returns: Yearly payment.
- `getPaymentRoundedPerMinute : function(integer, integer, number) : integer`: Returns the payment per minute of game time.
    - param `#1`: Amount.
    - param `#2`: Term in ms.
    - param `#3`: Interest fraction.
    - returns: Payment per minute.
- `getPaymentLeftoverRounded : function(integer, integer, number, integer, integer) : integer`: Returns the amount still to repay (without interest) rounded to a step.
    - param `#1`: Amount.
    - param `#2`: Term in ms.
    - param `#3`: Interest fraction.
    - param `#4`: Monthly payments made.
    - param `#5`: Rounding step (0 returns 0).
    - returns: Remaining amount.
- `getFullLoanAndInterestPayBack : function(Loan) : integer, integer`: Returns what an early full repayment costs.
    - param `#1`: Taken loan.
    - returns: Remaining repayment and remaining interest.
- `isEqual : function({Loan}, {Loan}) : boolean`: Compares two loan lists field by field.
    - param `#1`: First list.
    - param `#2`: Second list.
    - returns: `true` if equal.
- `makeLoan : function(integer, integer) : JournalEntry`: Builds a journal entry of type `LOAN`.
    - param `#1`: Amount.
    - param `#2`: Time.
    - returns: Journal entry.
- `makeInterest : function(integer, integer) : JournalEntry`: Builds a journal entry of type `INTEREST`.
    - param `#1`: Amount.
    - param `#2`: Time.
    - returns: Journal entry.
