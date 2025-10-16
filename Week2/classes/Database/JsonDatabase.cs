using System.Text.Json;


/// <summary>
/// Not techniqually a database, but can be persistenty written to and read from.
/// </summary>
public class JsonDatabase
{
    private readonly string? FilePath;

    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
    {
        WriteIndented = true
    };

    public JsonDatabase(string filePath)
    {
        FilePath = filePath;
    }

    /// <summary>
    /// We open up the database file in read mode, and read it's content in memory
    /// </summary>
    /// <returns>LibraryState</returns>
    public LibraryState? Load()
    {
        // Check whether or not the file exists on the current path. :: /home/User/MyProject/File.json
        if (!File.Exists(FilePath))
        {
            return null!;
        }
        // fs -> filesystem
        using var fs = File.OpenRead(FilePath);
        return JsonSerializer.Deserialize<LibraryState>(fs, Options);
    }

    /// <summary>
    /// Save new content to our main library content repository
    /// </summary>
    /// <param name="libraryState">the current state of the saved library content</param>
    public void Save(LibraryState libraryState)
    {
        var directory = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fs = File.Create(FilePath!);
        JsonSerializer.Serialize(fs, libraryState, Options);
    }
}