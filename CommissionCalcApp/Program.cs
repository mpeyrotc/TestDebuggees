namespace CommissionCalcApp;

class Program
{
    static void Main()
    {
        var sale = new SaleRecord("Alice Johnson", 50_000m, "Standard");

        Console.WriteLine($"Calculating commission for {sale.SalespersonName}...");
        Console.WriteLine($"Sales amount: {sale.SalesAmount:C}");
        Console.WriteLine($"Commission tier: {sale.CommissionTier}");

        decimal commission = CommissionCalculator.CalculateCommission(sale.SalesAmount, sale.CommissionTier);

        Console.WriteLine($"Commission: {commission:C}");
        Console.WriteLine("Done.");
    }
}
