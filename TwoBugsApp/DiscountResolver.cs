namespace TwoBugsApp;

internal sealed class DiscountResolver
{
    public decimal ApplyDiscount(decimal subtotal, string discountCode)
    {
        decimal rate = LookupRate(discountCode);
        return ApplyRate(subtotal, rate);
    }

    // Applies a signed rate adjustment to the given amount.
    // Pass a NEGATIVE rate for a discount (e.g. -0.10m for 10% off).
    // Pass a POSITIVE rate for a surcharge (e.g. +0.05m for 5% fee).
    private static decimal ApplyRate(decimal amount, decimal rate)
    {
        decimal adjustment = amount * rate;
        return amount + adjustment;
    }

    private static decimal LookupRate(string discountCode) => discountCode switch
    {
        "SAVE10" => 0.10m,
        "SAVE25" => 0.25m,
        _ => 0m,
    };
}
