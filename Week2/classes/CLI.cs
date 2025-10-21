using Spectre.Console;
/// <summary>
/// Main command-line interface class for our library software
/// </summary>
public class CLI
{
    private readonly LibraryService? _service;
    private readonly JsonDatabase? _context;
    private readonly IAnsiConsole UI;

    public CLI(LibraryService service, JsonDatabase context, IAnsiConsole ui)
    {
        _service = service;
        _context = context;
        UI = ui;
    }

    public void Run()
    {
        while (true)
        {
            var choice = UI.Prompt(
                new SelectionPrompt<string>()
                .Title("[bold]Menu[/]:")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "list",
                    "add",
                    "rent",
                    "return",
                    "calculate",
                    "search",
                    "edit",
                    "exit"
                }));

            try
            {
                switch (choice)
                {
                    case "list": ListCatalog(); break;
                    case "add": AddItemFlow(); Save(); break;
                    case "rent": RentFlow(); Save(); break;
                    case "return": ReturnFlow(); Save(); break;
                    case "calculate": CalcuateFeeFlow(); break;
                    case "search": SearchFlow(); break;
                    case "edit": EditMetadataFlow(); Save(); break;
                    case "exit": UI.MarkupLine("[grey]Exiting program...[/]"); return;
                }

                if (choice is "exit")
                {
                    UI.MarkupLine("[grey]Exiting program...[/]");
                    break;
                }
            }
            catch (Exception e)
            {
                UI.MarkupLine($"[red]An error occured:{e.Message}[/]");
            }
        }
    }

    private void ListCatalog()
    {
        var items = _service!.All().ToList();
        if (items.Count == 0)
        {
            UI.MarkupLine("[yellow]The catalog is currently empty![/]");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("[bold]Id[/]");
        table.AddColumn("[bold]Type of media[/]");
        table.AddColumn("[bold]Title[/]");
        table.AddColumn("[bold]Details[/]");
        table.AddColumn("[bold]Status[/]");

        foreach (var item in items)
        {
            var (type, title, details) = DescibeItem(item);
            var status = item.IsRented ? "[red]Rented[/]" : "[green]Available[/]";
            table.AddRow($"[yellow]{item.Id}[/]", type, $"[white]{title}[/]", details, status);
        }

        UI.Write(table);
    }
    private void AddItemFlow()
    {
        UI.MarkupLine("[bold]Add item[/]:");

        var kindOfMedia = UI.Prompt(
            new SelectionPrompt<MediaType>()
            .Title("Select [green]type of media[/]:")
            .UseConverter(kind => kind.ToString())
            .AddChoices(MediaType.Book, MediaType.DVD, MediaType.VHS, MediaType.VIDEO_GAMES));

        var id = PromptText("Id", mustBeUnique: true);
        var title = PromptText("Title");

        switch (kindOfMedia)
        {
            case MediaType.Book:
                {
                    var author = PromptText("Author");
                    var isbn = UI.Prompt(
                        new TextPrompt<string>("ISBN (optional):")
                            .AllowEmpty());
                    _service!.Add(new Book(id, title, author, string.IsNullOrWhiteSpace(isbn) ? "" : ""));
                    break;
                }

            case MediaType.DVD:
                {
                    var regionCode = UI.Prompt(
                        new SelectionPrompt<RegionCodes>()
                        .Title("Region Code")
                        .UseConverter(rgnCode => $"{(int)rgnCode} - {rgnCode}")
                        .AddChoices(RegionCodes.America, RegionCodes.Europe, RegionCodes.Asia)
                    );

                    var minutes = UI.Prompt(
                        new TextPrompt<double>("Runtime:")
                        .Validate(min => min > 0 ? ValidationResult.Success() : ValidationResult.Error("Runtime cannot be less than 0 minutes."))
                    );

                    var pg = UI.Prompt(
                        new SelectionPrompt<ParentalGuidance>()
                        .Title("Parental Guidance")
                        .UseConverter(_pg => $"{(int)_pg} - {_pg}")
                        .AddChoices(ParentalGuidance.ESRB, ParentalGuidance.PEGI)
                    );

                    _service!.Add(new Dvd(id, title, regionCode, minutes, pg));
                    break;
                }
        }

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


    private string PromptText(string label, bool mustBeUnique = false)
    {
        while (true)
        {
            var value = UI.Prompt(new TextPrompt<string>($"{label}")
                .Validate(str => string.IsNullOrWhiteSpace(str)
                ? ValidationResult.Error("Value cannot be empty!")
                : ValidationResult.Success()));

            value = value.Trim();
            if (mustBeUnique && _service!.Exists(value))
            {
                UI.MarkupLine("[red]This piece of media already exists with this unique id[/].Try another one");
                continue;
            }
            return value;
        }
    }

    private string PromptExistingId(string label)
    {
        while (true)
        {
            var id = UI.Prompt(new TextPrompt<string>($"{label}:")
            .Validate(str => string.IsNullOrWhiteSpace(str) ? ValidationResult.Error("Value cannot be empty!") : ValidationResult.Success())).Trim();

            if (_service!.Exists(id))
            {
                return id;
            }

            UI.MarkupLine("[red]Unknown id[/]\n[grey]Use 'list' to find existing items.[/]");
        }
    }

    private static (string Type, string Title, string Details) DescibeItem(IRentable item)
    {
        string rented = item.IsRented ? "Rented" : "Available";
        return item switch
        {
            Book book => ("Book", book.Title, $"Author: [italic]{book.Author}[/]"),
            Dvd dvd => ("DVD", dvd.Title, $"Region: [blue]{dvd.RegionCode}[/]\nParental Guidance: {dvd.ParentalGuidance}"),
            Vhs vhs => ("VHS", vhs.Title, $"Condition: {vhs.Condition}\nParental Guidance: {vhs.ParentalGuidance}"),
            VideoGames videoGames => ("Video Games", videoGames.Title, $"[bold]Platform: {videoGames.Platform}[/]\nParental Guidance: {videoGames.ParentalGuidance}"),
            _ => ("Item", item.Title, "")
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