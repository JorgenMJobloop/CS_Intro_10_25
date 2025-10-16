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
        // TODO: Implement the main CLI loop
        while (true)
        {
            PrintMenu();
            Console.Write("> ");
            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    // each case, will match a helper method
                    case "1":
                        Console.WriteLine("DEBUG::One was pressed!");
                        ListCatalog();
                        break;
                    case "2":
                        AddItemFlow();
                        //Save();
                        break;
                    /*    case "3":
                            RentFlow();
                            Save();
                            break;
                        case "4":
                            ReturnFlow();
                            Save();
                            break;
                        case "5":
                            FeeFlow();
                            break;
                        case "6":
                            SearchFlow();
                            break;
                        case "7":
                            EditMetadataFlow();
                            Save();
                            break;
                        */
                    case "0" or "exit":
                        Console.WriteLine("Exiting program..");
                        break;
                    default:
                        Console.WriteLine("Unknown command entered!");
                        return;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured: {e.Message}");
            }
            Console.WriteLine();

            if (choice!.Contains("exit") || choice.Contains("0"))
            {
                break;
            }
        }
    }

    private void PrintMenu()
    {
        // TODO: Print the main menu
        Console.WriteLine("\nMenu:");
        Console.WriteLine("1: List library catalog");
        Console.WriteLine("2: Add a new item(Book / DVD / VHS / Video Game)");
        Console.WriteLine("3: Rent a new item");
        Console.WriteLine("4: Return an item");
        Console.WriteLine("5: Calcuate rental fee");
        Console.WriteLine("6: Search for item");
        Console.WriteLine("7: Edit metadata (rename, author, current condition(vhs tapes))");
        Console.WriteLine("0 or 'exit': Exit the program..");
    }

    private void ListCatalog()
    {
        Console.WriteLine("\nCatalog:");
        foreach (var item in _service!.All())
        {
            Console.WriteLine(item);
        }
    }
    private void AddItemFlow()
    {
        Console.WriteLine("not yet created..");
    }
}