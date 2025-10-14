public class LibraryService
{
    /// <summary>
    /// Our main library inventory, using a Dictionary data structure with string as key, and IRentable as values
    /// </summary>
    private readonly Dictionary<string, IRentable> _inventory = new Dictionary<string, IRentable>();

    // Methods
    public IEnumerable<IRentable> All() => _inventory.Values.OrderBy(inv => inv.Title);

    //public bool Exists(string id) => _inventory.ContainsKey(id);

    public bool Exists(string id)
    {
        if (_inventory.ContainsKey(id))
        {
            return true;
        }
        return false;
    }

    public void Add(IRentable item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (_inventory.ContainsKey(item.Id))
        {
            throw new InvalidOperationException($"An item with the id: {item.Id} already exists..");
        }
        _inventory[item.Id] = item;
    }

    public IRentable? Find(string id) => _inventory.TryGetValue(id, out var itm) ? itm : null;

    public void Rent(string id, string customerId)
    {
        var itm = Find(id) ?? throw new KeyNotFoundException($"Could not find the item with id: {id}");
        itm.Rent(customerId);
    }

    public void Return(string id)
    {
        var itm = Find(id) ?? throw new KeyNotFoundException($"Could not find the item with id: {id}");
        itm.Return();
    }

    public double Fee(string id, int days)
    {
        var itm = Find(id) ?? throw new KeyNotFoundException($"Could not find the item with id: {id}");
        return itm.CalculateFee(days);
    }

    // Search the inventory for a specific item
    public IEnumerable<IRentable> SearchForItem(string searchTerm)
    {
        searchTerm = searchTerm.Trim() ?? string.Empty;

        return All().Where(itm => itm.Title.Contains(searchTerm, StringComparison.Ordinal) || itm.Id.Contains(searchTerm, StringComparison.Ordinal));
    }
}