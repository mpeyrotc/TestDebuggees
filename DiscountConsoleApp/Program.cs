using DiscountLibrary;
using ExternalDiscountProvider;

namespace DiscountConsoleApp;

internal static class Program
{
    private static void Main()
    {
        var calculator = new DiscountCalculator(new DiscountProvider());

        for (int attempt = 1; attempt <= 100; attempt++)
        {
            decimal total = calculator.CalculateTotal(100m, 20);
            Console.WriteLine($"Attempt {attempt}: total={total}");
        }
    }
}
