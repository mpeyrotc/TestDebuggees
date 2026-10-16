# DiscountLibrary

A .NET 8 pricing library that calculates the amount due after a percentage
discount. The caller supplies an `IDiscountProvider` to the calculator's
constructor. That provider resolves discounts using the host application's
pricing context; its implementation, configuration, and runtime logs are not
included in this source package.

## Reported behavior

Repeated calls to `DiscountCalculator.CalculateTotal(100m, 20)` intermittently
return `100m` instead of `80m`, with the same inputs and no exception. No trace
of the effective discount returned by the provider has been captured.

## Reproduction request

Open `DiscountLibrary.sln` by itself and ask:

> Diagnose why DiscountCalculator.CalculateTotal(100m, 20) intermittently returns
> 100 instead of 80 for the same inputs, without an exception. The caller supplies
> an IDiscountProvider implementation that is not included in this checkout,
> and we have no runtime trace of its returned values.
