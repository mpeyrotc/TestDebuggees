namespace DiscountLibrary;

/// <summary>Calculates the amount due after a percentage discount.</summary>
public sealed class DiscountCalculator
{
    private readonly IDiscountProvider discountProvider;

    /// <summary>Creates a calculator using the host's discount provider.</summary>
    /// <param name="discountProvider">The provider supplied by the caller.</param>
    public DiscountCalculator(IDiscountProvider discountProvider)
    {
        ArgumentNullException.ThrowIfNull(discountProvider);
        this.discountProvider = discountProvider;
    }

    /// <summary>Applies a whole-number percentage discount to a subtotal.</summary>
    /// <param name="subtotal">The nonnegative amount before the discount.</param>
    /// <param name="discountPercent">The discount percentage, from zero through one hundred.</param>
    /// <returns>The discounted amount due.</returns>
    public decimal CalculateTotal(decimal subtotal, int discountPercent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(subtotal);
        ArgumentOutOfRangeException.ThrowIfNegative(discountPercent);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(discountPercent, 100);

        int effectiveDiscountPercent = this.discountProvider.GetDiscountPercent(subtotal, discountPercent);
        ArgumentOutOfRangeException.ThrowIfNegative(effectiveDiscountPercent);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(effectiveDiscountPercent, 100);

        return subtotal * (1 - effectiveDiscountPercent / 100m);
    }
}
