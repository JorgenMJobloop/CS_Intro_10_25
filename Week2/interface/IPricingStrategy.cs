public interface IPricingStrategy
{
    // we add this interface, to archieve composition, rather than inheritance
    double Calculate(int days, int baseRate);
}