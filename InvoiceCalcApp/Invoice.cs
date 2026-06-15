namespace InvoiceCalcApp;

/// <summary>
/// Represents a calculated invoice.
/// </summary>
/// <param name="ItemName">The name of the item.</param>
/// <param name="UnitPrice">The unit price of the item.</param>
/// <param name="Quantity">The quantity ordered.</param>
/// <param name="Subtotal">The pre-tax subtotal.</param>
/// <param name="TaxAmount">The calculated tax.</param>
/// <param name="Total">The final total including tax.</param>
internal record Invoice(
    string ItemName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal,
    decimal TaxAmount,
    decimal Total);
