/// <summary>
/// Our superclass (base). Marked as abstract, so that we can override it when inherited by other classes in our namespace.
/// </summary>
public abstract class LibraryBase : IRentable
{
    // Initalize fields
    private string? _title;
    protected int _baseDailyRate;

    // Interface properties
    public string Id { get; } // We are going to initalize this, using a constructor.

    public string Title
    {
        get => _title!;
        private set
        {
            // here we check that we dont get a empty string, null or whitespace
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("The title cannot be empty!");
            }
            _title = value.Trim();
        }
    }

    public bool IsRented { get; private set; }

    protected LibraryBase(string id, string title, int baseDailyRate)
    {
        Id = id ?? throw new ArgumentException(nameof(id));
        Title = title;
        _baseDailyRate = baseDailyRate;
    }

    /// <summary>
    /// Using the abstract keyword, in our abstract superclass, we make it available for classes that inherit this method, to override it when needed.
    /// </summary>
    /// <param name="days">number of days since first rented</param>
    /// <returns>double</returns>
    public abstract double CalculateFee(int days);

    /// <summary>
    /// Calculate the fee, since a book, dvd or other media was rented from the library.
    /// </summary>
    /// <param name="days">number of days since first rented</param>
    /// <returns>double</returns>
    public double CalculateFee(DateTime from, DateTime to)
    {
        // Convert our daytime parameters to integer
        int days = Math.Max(0, (to.Date - from.Date).Days);
        // Calculate the fee
        return CalculateFee(days);
    }

    public virtual void Rent(string customerId)
    {
        // Check whether or not a book/media is rented out already (IsRented = true) otherwise (IsRented = false) <- this means we can rent something out.
        if (IsRented)
        {
            throw new Exception("Already rented!");
        }
        if (string.IsNullOrWhiteSpace(customerId))
        {
            throw new Exception("Customer ID is required when renting from the library.");
        }
        // If both of these conditions are not met, we can set IsRented to true and treat this a new rented resource(book, dvd etc.)
        IsRented = true;
        Console.WriteLine($"[{Id}] '{Title}' was rented to {customerId}");
    }

    public virtual void Return()
    {
        if (!IsRented)
        {
            throw new Exception("Not rented!");
        }
        IsRented = false;
        Console.WriteLine($"[{Id}] '{Title}' was returned to the library.");
    }

    // static Helper method
    public static bool IsOverdue(int days)
    {
        if (days >= 14)
        {
            return true;
        }
        return false;
    }
}