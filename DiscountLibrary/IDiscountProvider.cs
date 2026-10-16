namespace DiscountLibrary;

/// <summary>Resolves discounts using the host application's pricing policy.</summary>
public interface IDiscountProvider
{
    /// <summary>Gets the effective discount for the current pricing context.</summary>
    /// <param name="subtotal">The amount before the discount.</param>
    /// <param name="requestedDiscountPercent">The discount percentage requested by the caller.</param>
    /// <returns>The effective discount percentage, from zero through one hundred.</returns>
    int GetDiscountPercent(decimal subtotal, int requestedDiscountPercent);
}
