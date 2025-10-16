namespace Week2;

class Program
{
    static void Main(string[] args)
    {
        var rentalService = new RentalService();
        var libraryService = new LibraryService();

        var book1 = new Book("B-100", "Clean Code", "Robert C. Martin");
        var book2 = new Book("B-200", "The Hobbit", "J.R.R Tolkien");

        // add our books to the rental service
        rentalService.Add(book1);
        rentalService.TryRent("B-100", "J-1", out var _);
        rentalService.Add(book2);

        libraryService.Add(book1);

        var file = libraryService.SearchForItem("B-100");

        foreach (var f in file)
        {
            Console.WriteLine(f.Title);
        }

        Console.WriteLine(book1.IsRented); // true
        Console.WriteLine($"Fee 5 days: {book1.CalculateFee(5)}");

        Console.WriteLine("DEBUG::16.10.2025, current working implementation");

        var database = args.Length > 0 ? args[0] : "library.json";
        var db = new JsonDatabase(database);
        var service = new LibraryService();

        var cli = new CLI(service, db);
        cli.Run();
    }
}
