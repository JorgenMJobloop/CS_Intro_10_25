public interface IRentable
{
    // Properties
    string Id { get; }
    string Title { get; }
    bool IsRented { get; }

    // Methods
    void Rent(string customerId);
    void Return();
    double CalculateFee(int days);
}