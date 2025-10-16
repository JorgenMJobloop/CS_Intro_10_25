public class LibraryService
{
    /// <summary>
    /// Our main library inventory, using a Dictionary data structure with string as key, and IRentable as values
    /// </summary>
    private readonly Dictionary<string, IRentable> _inventory = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, LoanRecord> _loans = new(StringComparer.OrdinalIgnoreCase);

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

    // Snapshots & a restore point
    internal IEnumerable<IRentable> SnapshotItems() => _inventory.Values;
    internal IEnumerable<LoanRecord> SnapshotLoans() => _loans.Values;

    internal void RestoreFrom(IEnumerable<IRentable> items, IEnumerable<LoanRecord> loans)
    {
        // Clear the inventory
        _inventory.Clear();

        // loop through all the items
        foreach (var item in items)
        {
            _inventory[item.Id] = item;
        }
        // Clear saved loan information
        _loans.Clear();
        foreach (var loan in loans)
        {
            _loans[loan.ItemId] = loan;
            // Check that the IsRented condition reflects the current loans
            if (_inventory.TryGetValue(loan.ItemId, out var itm))
            {
                if (!itm.IsRented)
                {
                    itm.Rent(loan.CustomerId);
                }
            }
        }
    }
}