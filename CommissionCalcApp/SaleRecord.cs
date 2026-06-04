namespace CommissionCalcApp;

/// <summary>
/// Represents a single sale transaction for commission calculation.
/// </summary>
public record SaleRecord(string SalespersonName, decimal SalesAmount, string CommissionTier);
