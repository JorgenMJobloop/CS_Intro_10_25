public class Book : LibraryBase
{
    public string Author { get; private set; }
    public MediaType Type { get; set; }
    public string? Isbn { get; private set; }
    public int BaseDailyRate { get; set; }
    public Book(string id, string title, string author, string? isbn = null)
        : base(id, title, baseDailyRate: 10)
    {
        Author = author ?? "Unknown author";
        Isbn = string.IsNullOrWhiteSpace(isbn) ? null : isbn.Trim();
    }

    public override double CalculateFee(int days)
    {
        if (days < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(days));
        }
        int first = Math.Min(days, 7);
        int rest = Math.Max(0, days - 7);
        return first * _baseDailyRate + rest * (_baseDailyRate * 1.5);
    }

    public void ChangeAuthor(string newAuthor)
    {
        Author = string.IsNullOrWhiteSpace(newAuthor) ? "Unknown" : newAuthor.Trim();
    }

    public void SetIsbn(string? isbn)
    {
        Isbn = string.IsNullOrWhiteSpace(isbn) ? null : isbn.Trim();
    }

    public override string ToString()
    {
        return $"Book [{Id}] '{Title}' by {Author}" + (Isbn is null ? "" : $"(ISBN: {Isbn})");
    }
}