/// <summary>
/// Main command-line interface class for our library software
/// </summary>
public class CLI
{
    private readonly LibraryService? _service;
    private readonly JsonDatabase? _context;

    public CLI(LibraryService service, JsonDatabase context)
    {
        _service = service;
        _context = context;
    }

    public void Run()
    {
        PrintMenu();

        while (true)
        {

            Console.Write("> ");
            var choice = Console.ReadLine()!;
            switch (choice.ToLower())
            {
                // each case, will match a helper method
                case "list":
                    ListCatalog();
                    break;
                case "add":
                    AddItemFlow();
                    Save();
                    break;
                case "rent":
                    RentFlow();
                    Save();
                    break;
                case "return":
                    ReturnFlow();
                    Save();
                    break;
                case "calculate":
                    CalcuateFeeFlow();
                    break;
                case "search":
                    SearchFlow();
                    break;
                case "edit":
                    EditMetadataFlow();
                    Save();
                    break;
                case "0" or "exit":
                    Console.WriteLine("Exiting program..");
                    break;
                default:
                    PrintMenu();
                    break;
            }
            if (choice!.Contains("exit") || choice.Contains("0"))
            {
                break;
            }
        }
    }

    private void PrintMenu()
    {
        Console.WriteLine("\nMenu:");
        Console.WriteLine("list: List library catalog");
        Console.WriteLine("add: Add a new item(Book / DVD / VHS / Video Game)");
        Console.WriteLine("rent: Rent a new item");
        Console.WriteLine("return: Return an item");
        Console.WriteLine("calculate: Calcuate rental fee");
        Console.WriteLine("search: Search for item");
        Console.WriteLine("edit: Edit metadata (rename, author, current condition(vhs tapes))");
        Console.WriteLine("'exit': Exit the program..");
    }

    private void ListCatalog()
    {
        Console.WriteLine("\nCatalog:");
        foreach (var item in _service!.All())
        {
            Console.WriteLine(DescibeItem(item));
        }
    }
    private void AddItemFlow()
    {
        Console.WriteLine("\nAdd item:");
        Console.WriteLine("1) Book\n2) DVD\n3) VHS\n4) Video game title");
        Console.Write("Select type: ");
        var typeStr = Console.ReadLine();
        if (!int.TryParse(typeStr, out var typeNum) || typeNum < 1 || typeNum > 4)
        {
            throw new Exception("Invalid media type selected!");
        }
        var typeOfMedia = (MediaType)typeNum;
        Console.Write("Id: ");
        var id = ReadNonEmpty(); // TODO: Create this method.
        if (_service!.Exists(id))
        {
            throw new ArgumentException($"Item with id {id} already exists..");
        }
        Console.Write("Title: ");
        var title = ReadNonEmpty();

        switch (typeOfMedia)
        {
            case MediaType.Book:
                Console.Write("Author: ");
                var author = ReadNonEmpty();
                Console.Write("ISBN (optional)");
                var isbn = Console.ReadLine();
                _service!.Add(new Book(id, title, author, string.IsNullOrWhiteSpace(isbn) ? null : isbn));
                break;

            case MediaType.DVD:
                Console.Write("Region (1=America, 2=Europe, 3=Asia): ");
                var region = (RegionCodes)ReadInt(min: 1, max: 3); // TODO: Create this method
                Console.Write("Duration (00:00)/(hh:mm): ");
                double runtime = ReadDouble(min: 1);
                Console.Write("Parental guidance: ");
                var guidance = (ParentalGuidance)ReadInt(min: 1, max: 3);
                _service!.Add(new Dvd(id, title, region, runtime, guidance));
                break;
            case MediaType.VHS:
                Console.Write("Condition: (Good/Worn/Bad): ");
                var condition = ReadNonEmpty();
                Console.Write("Parental guidance: ");
                var _guidance = (ParentalGuidance)ReadInt(min: 1, max: 2);
                _service.Add(new Vhs(id, title, condition, _guidance));
                break;
            case MediaType.VIDEO_GAMES:
                Console.Write("Platforms (1: Playstation, 2: Nintendo, 3: Xbox, 4: PC): ");
                var platform = (Platforms)ReadInt(min: 1, max: 4);
                Console.Write("Parental guidance: ");
                var parentalGuidance = (ParentalGuidance)ReadInt(min: 1, max: 2);
                _service.Add(new VideoGames(id, title, platform, parentalGuidance));
                break;
        }
        Console.WriteLine("Item added.");
    }

    private void RentFlow()
    {
        Console.Write("Enter id to rent: ");
        var id = ReadNonEmpty();
        Console.Write("Customer id: ");
        var customerId = ReadNonEmpty();
        _service!.Rent(id, customerId);
        Console.WriteLine("Item successfully rented out.");
    }

    private void ReturnFlow()
    {
        Console.Write("\nEnter the id of the item you want to return: ");
        var id = ReadNonEmpty();
        _service!.Return(id);
        Console.WriteLine("Item successfully returned.");
    }

    private void CalcuateFeeFlow()
    {
        Console.Write("\nEnter the id of the item you want the fee calculated for: ");
        var id = ReadNonEmpty();
        Console.Write("Days since the item was first rented: ");
        int days = ReadInt(min: 0);
        var fee = _service!.Fee(id, days);
        Console.WriteLine($"Fee: {fee:0.00} {(LibraryBase.IsOverdue(days) ? "(OVERDUE)" : "")}");
    }

    private void SearchFlow()
    {
        Console.Write("\nSearch term: ");
        var searchTerm = Console.ReadLine() ?? string.Empty;
        var results = _service!.SearchForItem(searchTerm).ToList();
        if (!results.Any())
        {
            Console.WriteLine($"Searched for {searchTerm} but found no matches.");
            return;
        }
        foreach (var item in results)
        {
            Console.WriteLine(DescibeItem(item));
        }
    }

    private void EditMetadataFlow()
    {
        // get the user input, for the items that they want to edit the metadata of
        Console.Write("\nEnter the id of the item you want to the edit the metadata of: ");
        var id = ReadNonEmpty();
        var item = _service!.Find(id) ?? throw new KeyNotFoundException($"No item with id: {id} was found!");

        // Call the DescribeItem() method, to print out information about the item so that they know it is the one they want to edit the metadata of.
        Console.WriteLine(DescibeItem(item));
        Console.WriteLine("1) Rename title");
        if (item is Book)
        {
            // If the item is of type: books, we add further choices for the user.
            Console.WriteLine("2) Change author");
        }
        if (item is Vhs)
        {
            Console.WriteLine("3) Update current condition");
        }
        Console.Write("Choice: ");
        var choices = Console.ReadLine();

        switch (choices)
        {
            case "1":
                Console.Write("New title: ");
                var title = ReadNonEmpty();
                (item as LibraryBase)!.Rename(title);
                Console.WriteLine("Title updated.");
                break;
            case "2" when item is Book book:
                Console.Write("New author: ");
                var author = ReadNonEmpty();
                book.ChangeAuthor(author);
                Console.WriteLine("Author updated.");
                break;
            case "3" when item is Vhs vhs:
                Console.Write("New condition: ");
                var condition = ReadNonEmpty();
                vhs.UpdateVHSCondition(condition);
                Console.WriteLine("Conditon updated.");
                break;
            default:
                Console.WriteLine("No changes made..");
                break;
        }
    }

    // Helper methods

    /// <summary>
    /// Save changes to the main database
    /// </summary>
    private void Save()
    {
        _context!.Save(LibraryState.FromService(_service!));
        Console.WriteLine("Changes was saved to the database!");
    }

    private static string DescibeItem(IRentable item)
    {
        string rented = item.IsRented ? "Rented" : "Available";
        return item switch
        {
            Book book => $"{book} - {rented}",
            Dvd dvd => $"{dvd} - {rented}",
            Vhs vhs => $"{vhs} - {rented}",
            VideoGames videoGames => $"{videoGames} - {rented}",
            _ => $"[{item.Id}] {item.Title} - {rented}"
        };
    }

    /// <summary>
    /// Read a string and determine if it empty or not.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string ReadNonEmpty()
    {
        var stringInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(stringInput))
        {
            throw new ArgumentException("Value cannot be empty!");
        }
        return stringInput.Trim();
    }

    /// <summary>
    /// Read a given type, and parse it as integer
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static int ReadInt(int? min = null, int? max = null)
    {
        var inputStr = Console.ReadLine();
        if (!int.TryParse(inputStr, out var value))
        {
            throw new ArgumentException("Expected an integer value!"); // valid input: "10" returns valid output: 10 (integer), invalid input: "ten" will return 0(integer) in our case, that is not valid input
        }
        if (min.HasValue && value < min.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        if (max.HasValue && value > max.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        return value;
    }
    /// <summary>
    /// Read a given type, and parse it as double
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static double ReadDouble(double? min = null, double? max = null)
    {
        var inputStr = Console.ReadLine();
        if (!double.TryParse(inputStr, out var value))
        {
            throw new ArgumentException("Expected an integer value!"); // valid input: "10" returns valid output: 10 (integer), invalid input: "ten" will return 0(integer) in our case, that is not valid input
        }
        if (min.HasValue && value < min.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        if (max.HasValue && value > max.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        return value;
    }
}