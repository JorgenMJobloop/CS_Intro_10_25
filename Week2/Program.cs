namespace Week2;

class Program
{
    static void Main(string[] args)
    {
        var path = args.Length > 0 ? args[0] : "library.json";
        var db = new JsonDatabase(path);
        var service = new LibraryService();

        // load or write to our database
        var state = db.Load();
        if (state is null)
        {
            SeedDemoData(service);
            db.Save(LibraryState.FromService(service));
            Console.WriteLine($"New database file created at: '{path}'");
        }
        else
        {
            state.ApplyToService(service);
            Console.WriteLine($"Database loaded from: '{path}'");
        }

        var cli = new CLI(service, db);
        cli.Run();
    }

    private static void SeedDemoData(LibraryService service)
    {
        service.Add(new Book("B-100", "Clean Code", "Rober C. Martin", "9780132350884"));
        service.Add(new Book("B-200", "The Hobbit", "J.R.R Tolkien"));
        service.Add(new Dvd("D-100", "Interstellar", RegionCodes.Europe, runtime: 169, ParentalGuidance.PEGI));
        service.Add(new Vhs("V-100", "Back to the Future", condition: "Worn", ParentalGuidance.PEGI));
        service.Add(new VideoGames("VG-100", "Persona 5", Platforms.Playstation, ParentalGuidance.ESRB));
    }
}
