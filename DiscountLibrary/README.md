# DiscountLibrary

A deliberately buggy .NET 8 class library for debugger-handoff routing tests.
The solution contains only the library: no executable, test project, launch
profile, or caller application is supplied.

## Bug

`DiscountCalculator.CalculateTotal(100m, 20)` should return `80m`, but returns
`100m`: the percentage calculation uses integer division.

## Reproduction request

Open `DiscountLibrary.sln` by itself and ask:

> Use the debugger to diagnose why DiscountCalculator.CalculateTotal(100m, 20)
> returns 100 instead of 80. This checkout contains only this solution. Do not
> create an executable or tests.

The expected response explains that the library cannot be launched on its own
and asks for a caller application or running host before handing off. It may
also explain the visible arithmetic defect without using the debugger.

The library is intentionally buildable. Successful compilation alone must not
be treated as proof that a launch target exists. A supplied executable caller,
test host, or running process would make debugging the library possible.
