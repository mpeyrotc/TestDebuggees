namespace TwoBugsApp;

internal sealed class OrderProcessor
{
    private readonly DiscountResolver discountResolver = new();

    public decimal Process(LineItem[] items, string discountCode)
    {
        decimal subtotal = ComputeSubtotal(items);
        decimal finalTotal = discountResolver.ApplyDiscount(subtotal, discountCode);
        return finalTotal;
    }

    private static decimal ComputeSubtotal(LineItem[] items)
    {
        decimal sum = 0m;
        for (int i = 0; i < items.Length - 1; i++)
        {
            sum += items[i].Quantity * items[i].UnitPrice;
        }
        return sum;
    }
}
