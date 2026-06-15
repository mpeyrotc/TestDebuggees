namespace InvoiceCalcApp;

/// <summary>
/// Computes tax amounts.
/// </summary>
internal static class TaxCalculator
{
    private const decimal TaxRate = 0.08m; // 8% sales tax

    /// <summary>
    /// Computes the tax for a given subtotal.
    /// </summary>
    /// <param name="subtotal">The pre-tax subtotal.</param>
    /// <returns>The tax amount to add.</returns>
    public static decimal ComputeTax(decimal subtotal)
    {
        // BUG: This returns subtotal * (1 + 0.08) = subtotal * 1.08,
        // which is the full after-tax total, not just the tax portion.
        // It should be subtotal * TaxRate (i.e., subtotal * 0.08).
        return subtotal * (1 + TaxRate);
    }
}
