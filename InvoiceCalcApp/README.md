# InvoiceCalcApp

A .NET 8 console application that calculates a simple invoice with tax.
The app prompts for item details and displays the invoice with subtotal, tax, and total.

## What the program does

`Main` prompts the user for an item name, unit price, and quantity, then calls
`InvoiceGenerator.Generate(...)` which delegates tax computation to `TaxCalculator.ComputeTax(...)`.

Example input: item "Widget", price $50.00, quantity 2.

Expected behavior:
- Subtotal = 50 × 2 = **$100.00**
- Tax (8%) = 100 × 0.08 = **$8.00**
- Total = 100 + 8 = **$108.00**

## Bug

`TaxCalculator.ComputeTax` multiplies the subtotal by `(1 + TaxRate)` instead of just
`TaxRate`. This means `ComputeTax(100)` returns `100 × 1.08 = $108.00` (the full
after-tax total) instead of `100 × 0.08 = $8.00` (just the tax portion).

With the example input above:
- Subtotal: $100.00 ✓
- Tax (8%): **$108.00** ✗ (should be $8.00)
- Total: $100 + $108 = **$208.00** ✗ (should be $108.00)

## How to reproduce

1. Build and run the app:
   ```
   dotnet run --project InvoiceCalcApp.csproj
   ```
2. Enter any item name (e.g., "Widget")
3. Enter unit price: **50**
4. Enter quantity: **2**
5. Observe that Tax shows $108.00 and Total shows $208.00

## How to debug

1. Set a breakpoint on the `return` line in `TaxCalculator.ComputeTax`.
2. Launch the app under the debugger.
3. Enter the item details when prompted.
4. When the breakpoint hits, inspect the return value — it will be 108.00 instead of 8.00.
