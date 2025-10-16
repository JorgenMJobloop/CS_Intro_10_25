public class ItemDto
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public MediaType _MediaType { get; set; }
    public int BaseDailyRate { get; set; } // We dont really have to save this, but it can be okay to keep track of.

    // Books
    public string? Author { get; set; }
    public string? Isbn { get; set; }

    // DVDs
    public RegionCodes Region { get; set; }
    public double Runtime { get; set; }

    // VHS
    public string? Condition { get; set; }

    // Video Games
    public Platforms Platform { get; set; }
    public ParentalGuidance ParentalGuidanceRating { get; set; }

    // current state
    public bool IsRented { get; set; }
    // Comic books
    // TODO: Implement the ComicBooks.cs Media class
}