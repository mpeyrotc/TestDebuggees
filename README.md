# TestDebuggees

## Discount console example

Open `DiscountConsoleApp\DiscountConsoleApp.sln`, or run:

```powershell
dotnet run --project DiscountConsoleApp\DiscountConsoleApp.csproj
```

The console application exercises the unchanged `DiscountLibrary` calculator
with a prebuilt `ExternalDiscountProvider.dll`. The provider's implementation
source and symbols are not included. Repeated calls with the same inputs
produce totals of either 80 or 100.

The standalone `DiscountLibrary\DiscountLibrary.sln` remains available separately.