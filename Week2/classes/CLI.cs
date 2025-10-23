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
        UI.MarkupLine("[bold]Rent an item[/]");
        // Here, we utilize our PromptExistingId() method
        var id = PromptExistingId("Enter ID of the item you wish to rent");
        var customerID = PromptText("Customer id:");
        _service!.Rent(id, customerID);
        UI.MarkupLine("[green]Rented[/]");
    }
    private void ReturnFlow()
    {
        UI.MarkupLine("[bold]Return item[/]");
        var id = PromptExistingId("Enter the ID of the item you want to return");
        _service!.Return(id);
        UI.MarkupLine($"[green]Item with id: {id} has been returned.[/]");
    }

    private void CalcuateFeeFlow()
    {
        UI.MarkupLine("[bold]Calculate fee[/]");
        var id = PromptExistingId("Enter id");
        var days = UI.Prompt(
            new TextPrompt<int>("Days since first rented:")
                .Validate(d => d >= 0 ? ValidationResult.Success() : ValidationResult.Error("When calculating the fee, days cannot be less than 1"))
        );
        var fee = _service!.Fee(id, days);
        var overdue = LibraryBase.IsOverdue(days) ? "[red]OVERDUE[/]" : "";
        UI.MarkupLine($"Fee: [yellow]{fee:0.00}[/]{overdue}");
    }

    private void SearchFlow()
    {
        // get out search term, this prompt allows empty input: i.e:: "" or "B-100"
        var searchTerm = UI.Prompt(new TextPrompt<string>("Search term:").AllowEmpty());
        var results = _service!.SearchForItem(searchTerm).ToList();
        // check that we do not get 0 results
        if (results.Count == 0)
        {
            UI.MarkupLine("[yellow]WARN[/]No results found.");
            return;
        }
        // Create our main table
        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("[bold]Id[/]");
        table.AddColumn("[bold]Type of media[/]");
        table.AddColumn("[bold]Details[/]");
        table.AddColumn("[bold]Status[/]");

        foreach (var item in results)
        {
            var (type, title, details) = DescibeItem(item);
            var status = item.IsRented ? "[red]Rented[/]" : "[green]Available[/]";
            table.AddRow($"[yellow]{item.Id}[/]", type, $"[white]{title}[/]", details, status);
        }

        UI.Write(table);
    }

    private void EditMetadataFlow()
    {
        UI.MarkupLine("[bold]Edit metadata[/]");
        var id = PromptExistingId("Enter the id of the item you wish to edit");
        var item = _service!.Find(id);

        var choices = new List<string> { "Rename title" };
        if (item is Book)
        {
            choices.Add("Change author");
        }

        if (item is Vhs)
        {
            choices.Add("Update current condition");
        }

        var choice = UI.Prompt(new SelectionPrompt<string>().Title("Choose the item you wish to edit").AddChoices(choices));

        switch(choice)
        {
            case "Rename title":
                var t = PromptText("New title");
                (item as LibraryBase)!.Rename(t);
                UI.MarkupLine("[green]Title has been updated[/]");
                break;
            case "Change author" when item is Book book:
                var a = PromptText("New author");
                book.ChangeAuthor(a);
                UI.MarkupLine("[green]Author updated.[/]");
                break;
            case "Update condition" when item is Vhs vhs:
                var cond = PromptText("Update current condition");
                vhs.UpdateVHSCondition(cond);
                UI.MarkupLine("[green]Condtion updated.[/]");
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