namespace TwoBugsApp;

internal class Program
{
    static void Main()
    {
        var items = new LineItem[]
        {
            new("Widget", 3, 10.00m),
            new("Gadget", 2, 25.00m),
            new("Sprocket", 5, 4.00m),
        };
        const string discountCode = "SAVE10";

        var processor = new OrderProcessor();
        decimal finalTotal = processor.Process(items, discountCode);

        Console.WriteLine($"Order total: ${finalTotal:F2}");

        if (finalTotal > 100m)
        {
            throw new InvalidOperationException(
                $"Sanity check failed: order total ${finalTotal:F2} cannot exceed the $100.00 sum of line items.");
        }
    }
}
