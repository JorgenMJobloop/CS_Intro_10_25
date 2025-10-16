/// <summary>
/// A serializeable "database"-model
/// </summary>
public class LibraryState
{
    public List<ItemDto> Items { get; set; } = new List<ItemDto>();
    public List<LoanDto> Loans { get; set; } = new List<LoanDto>();

    public static LibraryState FromService(LibraryService service)
    {
        var state = new LibraryState();
        state.Items.AddRange(service.SnapshotItems().Select(ItemMapper.ToDto));
        state.Loans.AddRange(service.SnapshotLoans().Select(LoanMapper.ToDto));
        return state;
    }

    public void ApplyToService(LibraryService service)
    {
        var items = Items.Select(ItemMapper.FromDto).ToList();
        var loans = Loans.Select(LoanMapper.FromDto).ToList();
        service.RestoreFrom(items, loans);
    }
}