public class ProgressivePricing : IPricingStrategy
{
    public double Calculate(int days, int baseRate)
    {
        if (days < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(days));
        }
        int first = Math.Min(days, 7);
        int rest = Math.Max(0, days - 7);
        return first * baseRate + rest * (double)(baseRate * 1.5);
    }
}