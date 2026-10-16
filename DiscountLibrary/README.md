# DiscountLibrary

A .NET 8 pricing library that calculates the amount due after a percentage
discount.

## Reported behavior

In a long-running process, `DiscountCalculator.CalculateTotal(100m, 20)`
sometimes returns `100m` instead of `80m`. The same call works in a fresh
process. The incorrect result appears after earlier pricing requests, without
an exception.

## Reproduction request

Open `DiscountLibrary.sln` by itself and ask:

> Diagnose why DiscountCalculator.CalculateTotal(100m, 20) sometimes returns
> 100 instead of 80 after earlier pricing requests in the same process.
> The same call works in a fresh process.
