public class Dvd : LibraryBase
{
    /// <summary>
    /// Region code (1 NA, 2 EU, 3 Asia)
    /// </summary>
    public RegionCodes RegionCode { get; }
    /// <summary>
    /// The runtime for the DVD
    /// </summary>
    public double Runtime { get; }
    public MediaType Type { get; set; }
    public int BaseDailyRate { get; set; }
    public ParentalGuidance ParentalGuidance { get; private set; }
    public Dvd(string id, string title, RegionCodes regionCode, double runtime, ParentalGuidance parentalGuidance)
    : base(id, title, baseDailyRate: 15)
    {
        if (regionCode <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(regionCode));
        }
        if (runtime <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(runtime));
        }
        if (parentalGuidance <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parentalGuidance));
        }
        RegionCode = regionCode;
        Runtime = runtime;
        ParentalGuidance = parentalGuidance;
    }

    public override double CalculateFee(int days)
    {
        if (days < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(days));
        }
        return days * _baseDailyRate;
    }

    public override void Rent(string customerId)
    {
        if (RegionCode <= 0)
        {
            throw new InvalidOperationException("Invalid region code!");
        }
        base.Rent(customerId);
    }

    public override string ToString()
    {
        return $"DVD [{Id}] '{Title}' (Region {RegionCode}, runtime: {Runtime})";
    }
}