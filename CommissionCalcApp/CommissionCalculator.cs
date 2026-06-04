namespace CommissionCalcApp;

/// <summary>
/// Calculates sales commission based on the sale amount and commission tier.
/// </summary>
public static class CommissionCalculator
{
    /// <summary>
    /// Computes the commission for a given sale.
    /// </summary>
    public static decimal CalculateCommission(decimal salesAmount, string tier)
    {
        decimal rate = GetCommissionRate(tier);
        return salesAmount / rate;
    }

    /// <summary>
    /// Returns the commission rate for the given tier.
    /// </summary>
    private static decimal GetCommissionRate(string tier)
    {
        // Commission rates by tier
        return tier switch
        {
            "Standard" => 8,    // 8% commission
            "Premium" => 12,    // 12% commission
            "Elite" => 15,      // 15% commission
            _ => 5              // 5% default commission
        };
    }
}
