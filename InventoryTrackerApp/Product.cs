namespace InventoryTrackerApp;

/// <summary>
/// Represents a single product in the warehouse inventory.
/// </summary>
public class Product
{
    public required string Sku { get; init; }
    public required string Name { get; init; }
    public int QuantityOnHand { get; set; }
    public int ReorderPoint { get; init; }
    public int ReorderQuantity { get; init; }
    public decimal UnitCost { get; init; }
}
