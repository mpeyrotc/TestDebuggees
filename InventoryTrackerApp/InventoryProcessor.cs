namespace InventoryTrackerApp;

/// <summary>
/// Processes daily sales transactions against the inventory and generates reorder
/// recommendations when stock falls below a product's reorder point.
/// </summary>
public class InventoryProcessor
{
    private readonly List<Product> products;
    private readonly List<ReorderRecommendation> recommendations = new();
    private readonly ReorderPolicy policy;

    public InventoryProcessor(List<Product> products, ReorderPolicy policy)
    {
        this.products = products;
        this.policy = policy;
    }

    public IReadOnlyList<ReorderRecommendation> Recommendations => this.recommendations;

    /// <summary>
    /// Applies a batch of daily sales to the inventory. Each sale reduces the on-hand
    /// quantity by the sold amount. After processing, products whose stock drops below
    /// the reorder point are flagged for reorder.
    /// </summary>
    public void ProcessDailySales(IEnumerable<SaleTransaction> sales)
    {
        foreach (var sale in sales)
        {
            var product = this.products.Find(p => p.Sku == sale.Sku);
            if (product is null)
            {
                Console.Error.WriteLine($"Warning: SKU {sale.Sku} not found in inventory, skipping.");
                continue;
            }

            product.QuantityOnHand -= sale.QuantitySold;

            if (product.QuantityOnHand < 0)
            {
                Console.Error.WriteLine($"Warning: SKU {sale.Sku} ({product.Name}) stock went negative ({product.QuantityOnHand}). Possible data error.");
            }
        }

        this.EvaluateReorders();
    }

    /// <summary>
    /// Checks each product and generates a reorder recommendation if stock has
    /// dropped below the reorder point.
    /// </summary>
    private void EvaluateReorders()
    {
        foreach (var product in this.products)
        {
            int effectiveReorderPoint = this.policy.ApplySeasonalAdjustment(product);

            if (product.QuantityOnHand < effectiveReorderPoint)
            {
                bool alreadyRecommended = this.recommendations.Exists(r => r.Sku == product.Sku);
                if (!alreadyRecommended)
                {
                    this.recommendations.Add(new ReorderRecommendation
                    {
                        Sku = product.Sku,
                        ProductName = product.Name,
                        CurrentStock = product.QuantityOnHand,
                        ReorderPoint = effectiveReorderPoint,
                        ReorderQuantity = product.ReorderQuantity,
                        EstimatedCost = product.ReorderQuantity * product.UnitCost,
                    });
                }
            }
        }
    }
}

public class SaleTransaction
{
    public required string Sku { get; init; }
    public int QuantitySold { get; init; }
}

public class ReorderRecommendation
{
    public required string Sku { get; init; }
    public required string ProductName { get; init; }
    public int CurrentStock { get; init; }
    public int ReorderPoint { get; init; }
    public int ReorderQuantity { get; init; }
    public decimal EstimatedCost { get; init; }
}
