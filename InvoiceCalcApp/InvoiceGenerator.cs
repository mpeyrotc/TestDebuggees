namespace InvoiceCalcApp;

/// <summary>
/// Generates an invoice from item details.
/// </summary>
internal static class InvoiceGenerator
{
    /// <summary>
    /// Creates an invoice for a single line item.
    /// </summary>
    public static Invoice Generate(string itemName, decimal unitPrice, int quantity)
    {
        decimal subtotal = unitPrice * quantity;
        decimal taxAmount = TaxCalculator.ComputeTax(subtotal);
        decimal total = subtotal + taxAmount;
        return new Invoice(itemName, unitPrice, quantity, subtotal, taxAmount, total);
    }
}
