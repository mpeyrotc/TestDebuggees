using System.Collections.Concurrent;

namespace DiscountLibrary;

/// <summary>Calculates the amount due after a percentage discount.</summary>
public static class DiscountCalculator
{
    private static readonly ConcurrentDictionary<decimal, decimal> TotalCache = new();

    /// <summary>Applies a whole-number percentage discount to a subtotal.</summary>
    /// <param name="subtotal">The nonnegative amount before the discount.</param>
    /// <param name="discountPercent">The discount percentage, from zero through one hundred.</param>
    /// <returns>The discounted amount due.</returns>
    public static decimal CalculateTotal(decimal subtotal, int discountPercent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(subtotal);
        ArgumentOutOfRangeException.ThrowIfNegative(discountPercent);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(discountPercent, 100);

        return TotalCache.GetOrAdd(subtotal, amount => amount * (1 - discountPercent / 100m));
    }
}
