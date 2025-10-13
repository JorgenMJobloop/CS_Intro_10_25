public class Book : LibraryBase
{
    public string Author { get; }
    public Genre Genre { get; }
    private readonly IPricingStrategy _pricing;
    public Book(string id, string title, string author, Genre genre, IPricingStrategy pricing, int baseDailyRate)
        : base(id, title, baseDailyRate)
    {
        Author = author ?? "Unknown author";
        Genre = genre;
        _pricing = pricing ?? new FlatRatePricing();
    }

    public override double CalculateFee(int days) => _pricing.Calculate(days, baseRate: _baseDailyRate);
}