public class RentalService
{
    // Our inventory
    private readonly Dictionary<string, IRentable> _inventory = new Dictionary<string, IRentable>();

    public void Add(IRentable item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        _inventory[item.Id] = item;
    }

    public IRentable? Find(string id) => _inventory.TryGetValue(id, out var itm) ? itm : null;

    public double PotentialTotalFee(int days, params string[] ids)
    {
        double total = 0;

        foreach (var id in ids)
        {
            var itm = Find(id) ?? throw new ArgumentException($"Unknown id: {id}");
            total += itm.CalculateFee(days);
        }
        return total;
    }

    public bool TryRent(string id, string customerId, out string? message)
    {
        var itm = Find(id);
        if (itm == null)
        {
            message = "The item was not found in the inventory!";
            return false;
        }
        // Here, we try to "rent" out a resource, catch any errors, if they occur
        try
        {
            itm.Rent(customerId);
            message = "OK";
            return true;
        }
        catch (Exception e)
        {
            message = e.Message;
            return false;
        }
    }
}