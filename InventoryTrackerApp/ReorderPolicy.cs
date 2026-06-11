namespace InventoryTrackerApp;

/// <summary>
/// Defines the reorder policy including seasonal adjustments to reorder thresholds.
/// During peak months (configurable), the reorder point is increased by a multiplier
/// to account for higher demand.
/// </summary>
public class ReorderPolicy
{
    private readonly HashSet<int> peakMonths;
    private readonly double peakMultiplier;

    /// <summary>
    /// Creates a new reorder policy.
    /// </summary>
    /// <param name="peakMonths">Month numbers (1-12) considered peak season.</param>
    /// <param name="peakMultiplier">Multiplier applied to reorder points during peak months.</param>
    public ReorderPolicy(IEnumerable<int> peakMonths, double peakMultiplier)
    {
        this.peakMonths = new HashSet<int>(peakMonths);
        this.peakMultiplier = peakMultiplier;
    }

    /// <summary>
    /// Applies seasonal adjustment to a product's reorder point.
    /// During peak months the reorder point is scaled up by the peak multiplier.
    /// </summary>
    public int ApplySeasonalAdjustment(Product product)
    {
        if (this.peakMonths.Contains(DateTime.Now.Month))
        {
            return (int)(product.ReorderPoint * this.peakMultiplier);
        }

        return product.ReorderPoint;
    }

    /// <summary>
    /// Loads a reorder policy from a configuration file.
    /// Expected format (CSV): first line is comma-separated peak month numbers,
    /// second line is the multiplier.
    /// </summary>
    public static ReorderPolicy LoadFromFile(string path)
    {
        string[] lines = File.ReadAllLines(path);

        int[] months = lines[0].Split(',', StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .ToArray();

        double multiplier = double.Parse(lines[1].Trim());

        return new ReorderPolicy(months, multiplier);
    }

    /// <summary>
    /// Creates a default policy with no seasonal adjustments.
    /// </summary>
    public static ReorderPolicy Default => new(Array.Empty<int>(), 1.0);
}
