using System.Globalization;
using InventoryTrackerApp;

return new Program().Run(args);

partial class Program
{
    public int Run(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: InventoryTrackerApp <inventory.csv> <sales.csv>");
            return 1;
        }

        string inventoryPath = Path.GetFullPath(args[0]);
        string salesPath = args.Length > 1 ? Path.GetFullPath(args[1]) : "";

        if (!File.Exists(inventoryPath))
        {
            Console.Error.WriteLine($"Error: inventory file not found – {inventoryPath}");
            return 1;
        }

        var products = LoadInventory(inventoryPath);

        // Load reorder policy if a config file exists next to the inventory
        string policyPath = Path.Combine(Path.GetDirectoryName(inventoryPath)!, "reorder_policy.csv");
        var policy = File.Exists(policyPath)
            ? ReorderPolicy.LoadFromFile(policyPath)
            : ReorderPolicy.Default;

        var processor = new InventoryProcessor(products, policy);

        if (salesPath.Length > 0 && File.Exists(salesPath))
        {
            var sales = LoadSales(salesPath);
            processor.ProcessDailySales(sales);
        }

        Console.WriteLine($"Processed {products.Count} products.");
        Console.WriteLine($"Reorder recommendations: {processor.Recommendations.Count}");

        foreach (var rec in processor.Recommendations)
        {
            Console.WriteLine($"  REORDER: {rec.Sku} ({rec.ProductName}) — stock={rec.CurrentStock}, " +
                              $"reorderPoint={rec.ReorderPoint}, orderQty={rec.ReorderQuantity}, " +
                              $"cost={rec.EstimatedCost:C}");
        }

        if (processor.Recommendations.Count == 0)
        {
            Console.WriteLine("  No reorders needed.");
        }

        // Validate totals
        decimal totalCost = 0;
        foreach (var rec in processor.Recommendations)
        {
            totalCost += rec.EstimatedCost;
        }

        // Sanity check: reorder cost should not exceed $50,000 per day
        if (totalCost > 50_000m)
        {
            throw new InvalidOperationException(
                $"Daily reorder cost of {totalCost:C} exceeds the $50,000 budget cap. " +
                $"Review reorder recommendations for errors.");
        }

        Console.WriteLine($"Total estimated reorder cost: {totalCost:C}");
        return 0;
    }

    private static List<Product> LoadInventory(string path)
    {
        var products = new List<Product>();
        string[] lines = File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.Length == 0) continue;

            string[] f = line.Split(',');
            products.Add(new Product
            {
                Sku = f[0].Trim(),
                Name = f[1].Trim(),
                QuantityOnHand = int.Parse(f[2].Trim()),
                ReorderPoint = int.Parse(f[3].Trim()),
                ReorderQuantity = int.Parse(f[4].Trim()),
                UnitCost = decimal.Parse(f[5].Trim(), CultureInfo.InvariantCulture),
            });
        }

        return products;
    }

    private static List<SaleTransaction> LoadSales(string path)
    {
        var sales = new List<SaleTransaction>();
        string[] lines = File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.Length == 0) continue;

            string[] f = line.Split(',');
            sales.Add(new SaleTransaction
            {
                Sku = f[0].Trim(),
                QuantitySold = int.Parse(f[1].Trim()),
            });
        }

        return sales;
    }
}
