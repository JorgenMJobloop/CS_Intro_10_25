public class FlatRatePricing : IPricingStrategy
{
    public double Calculate(int days, int baseRate)
    {
        if (days < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(days));
        }
        return days * baseRate;
    }
}