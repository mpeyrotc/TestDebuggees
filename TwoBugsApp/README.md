# TwoBugsApp

A tiny .NET 8 console application that builds an order from a few hard-coded line items,
applies a discount code, and computes the final total. The program is intentionally short
and contains two latent bugs (see below) so it can be used as a fixture for tooling that
exercises a debugging workflow.

## What the program does

`Main` creates three line items, a discount code (`SAVE10` = 10% off), and calls
`OrderProcessor.Process(items, discountCode)`. The final amount is held in the local
`finalTotal` and **never printed**. The only way to observe it is to break inside `Main`
after the call and read the local off the stack frame.

| Item     | Qty | Unit Price |
|----------|----:|-----------:|
| Widget   |  3  | 10.00      |
| Gadget   |  2  | 25.00      |
| Sprocket |  5  |  4.00      |

Expected behaviour:
- subtotal = 3·10 + 2·25 + 5·4 = **100.00**
- final total after 10% discount = 100 · 0.9 = **90.00**

## Hidden bugs

1. **`OrderProcessor.ComputeSubtotal`** — loop iterates while `i < items.Length - 1` so the
   last line item is silently dropped. With the data above the buggy subtotal is 80.00
   instead of 100.00.
2. **`DiscountResolver.ApplyDiscount`** — multiplies by `(1 + rate)` instead of
   `(1 - rate)`, so the "discount" is applied as a markup. With a correct subtotal of 100
   this returns 110 instead of 90.

Both bugs together yield `80 · 1.10 = 88.00`. Fixing only the first yields `100 · 1.10 =
110.00` — still wrong, but with a *different* wrong value. `Program.Main` contains a sanity
check that throws an unhandled `InvalidOperationException` when `finalTotal > 100`, so the
intermediate "loop-fixed, discount-still-broken" state surfaces an obvious runtime failure
rather than passing silently.

## Building and running

```
dotnet build TwoBugsApp.csproj
dotnet run --project TwoBugsApp.csproj
```

With both bugs present the program prints the (wrong) subtotal of 80.00 and exits without
the sanity check tripping. With only the first bug fixed it prints 100.00, then throws.
With both bugs fixed it prints 100.00 and exits cleanly.