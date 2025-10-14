public class VideoGames : LibraryBase
{
    /// <summary>
    /// Platforms available: Playstation, Nintendo(Switch, WiiU, Wii, 3DS..), Xbox, PC
    /// </summary>
    public Platforms Platform { get; private set; }
    public ParentalGuidance ParentalGuidance { get; }
    public VideoGames(string id, string title, Platforms platform, ParentalGuidance parentalGuidance)
    : base(id, title, baseDailyRate: 15)
    {
        if (platform <= 0 && parentalGuidance <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(platform), nameof(parentalGuidance));
        }
        Platform = platform;
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

    public override string ToString()
    {
        return $"Video game: [{Id}] '{Title}' (Platform: {Platform}, Parental Guidance Rating: {ParentalGuidance})";
    }
}