# TwoBugsApp

Tiny console asset used by the **DebuggerI2R-TwoBugsApp** offline-eval test. It
exercises the full issue-resolution (I2R) workflow for the Copilot debugger agent and is
intentionally designed so the agent has to use the debugger — not just static code
reading — to diagnose and verify the fixes.

## What the program does
`Main` builds three hard-coded line items, a discount code (`SAVE10` = 10% off), and calls
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
   last line item is silently dropped. With the test data the buggy subtotal is 80.00
   instead of 100.00.
2. **`DiscountResolver.ApplyDiscount`** — multiplies by `(1 + rate)` instead of
   `(1 - rate)`, so the "discount" is applied as a markup. With a correct subtotal of 100
   this returns 110 instead of 90.

Both bugs together yield `80 · 1.10 = 88.00`. Fixing only the first bug yields `100 · 1.10
= 110.00` — still wrong, with a *different* wrong value, which is what forces the agent to
re-enter the workflow and form a new hypothesis after the first verification pass.

## Why the bugs require runtime evidence
The I2R system prompt explicitly mandates: *"CONFIRMED: Breakpoint hit + runtime values
match predicted failure + you can explain causation. **DO NOT confirm with code inspection
alone**."* The agent must set breakpoints inside `ComputeSubtotal` / `ApplyDiscount` (or
read the locals at the call site in `Main`) and observe the actual numeric values before
calling `set_issue_resolution_mode_state` with `DiagnosticsConfirmedIssue`.

## Target framework
`net8.0`. Built bits under `bin\Debug\net8.0\` (`TwoBugsApp.exe` + `.pdb`) are kept in
the asset so the offline test runner can launch the binary directly under the debugger.
After the agent applies an edit and `updateTestAssetFiles: true` is set, the runner
rebuilds the solution before the next `debugger_launch`, so the fixes are exercised in the
verification round.
