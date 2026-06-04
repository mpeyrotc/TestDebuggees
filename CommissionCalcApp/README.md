# CommissionCalcApp

A tiny .NET 8 console app that calculates sales commission. It contains **two related bugs** in the commission calculation pipeline, designed for testing the debugger I2R (Issue to Resolution) retry flow.

## The Scenario

A salesperson (Alice Johnson) made a $50,000 sale on the "Standard" commission tier (8%). The expected commission is **$4,000**, but the app outputs **$6,250**.

## The Bugs

### Bug 1: Wrong operator in `CalculateCommission`

`CommissionCalculator.CalculateCommission()` uses division (`salesAmount / rate`) instead of multiplication (`salesAmount * rate`).

### Bug 2: Rate as whole number instead of decimal fraction

`CommissionCalculator.GetCommissionRate()` returns `8` for an 8% rate instead of `0.08m`. The comments say "8% commission" but the returned value is the integer 8.

## Math Walkthrough

| State | Calculation | Result |
|-------|-----------|--------|
| Both bugs present | 50000 / 8 | $6,250 (reported by user) |
| Bug 1 fixed only | 50000 × 8 | $400,000 (user discovers after first fix) |
| Both fixed | 50000 × 0.08 | $4,000 ✓ |

## How to Run

```bash
dotnet run --project CommissionCalcApp.csproj
```

## Test Design

This app is used for an offline eval test that exercises the I2R **retry flow**:

1. The agent identifies the division bug and fixes it
2. The program runs without error but produces $400,000 (invisible to agent — stdout not surfaced)
3. The user comes back and says the fix didn't work, provides the wrong value
4. The agent formulates a new hypothesis about the rate values
5. The agent fixes the rate representation and the issue is resolved
