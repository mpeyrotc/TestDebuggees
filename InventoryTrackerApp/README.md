# InventoryTrackerApp

A console app that processes daily warehouse sales against product inventory and generates reorder recommendations. Supports seasonal adjustments to reorder thresholds via a configurable policy file (`data/reorder_policy.csv`).

## Usage

```bash
dotnet run -- data/inventory.csv data/sales.csv
```

## Used by

- `InventoryTrackerApp-AskQuestion.json` — I2R offline eval test exercising `ask_question` + auto-responder.
