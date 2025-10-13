namespace Week2;

class Program
{
    static void Main(string[] args)
    {
        var flat = new FlatRatePricing();
        var progressive = new ProgressivePricing();
        var rentalService = new RentalService();

        var book1 = new Book("B-100", "Clean Code", "Robert C. Martin", Genre.Science, flat, 2);
        var book2 = new Book("B-200", "The Hobbit", "J.R.R Tolkien", Genre.Fantasy, flat, 1);

        // add our books to the rental service
        rentalService.Add(book1);
        rentalService.TryRent("B-100", "J-1", out var _);
        rentalService.Add(book2);

        Console.WriteLine(book1.IsRented); // true
        Console.WriteLine($"Fee 5 days: {book1.CalculateFee(5)}");
    }
}
