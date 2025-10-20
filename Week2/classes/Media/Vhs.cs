public class Vhs : LibraryBase
{
    public string Condition { get; private set; }
    public MediaType Type { get; set; } = MediaType.VHS;
    public ParentalGuidance ParentalGuidance { get; }
    public Vhs(string id, string title, string condition, ParentalGuidance parentalGuidance)
    : base(id, title, baseDailyRate: 8)
    {
        if (parentalGuidance <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(parentalGuidance));
        }
        Condition = string.IsNullOrWhiteSpace(condition) ? "Unknown" : condition.Trim();
        ParentalGuidance = parentalGuidance;
    }

    public override double CalculateFee(int days)
    {
        if (days < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(days));
        }
        return days * _baseDailyRate;
    }

    public void UpdateVHSCondition(string condition)
    {
        Condition = string.IsNullOrWhiteSpace(condition) ? Condition : Condition.Trim();
    }

    public override string ToString()
    {
        return $"VHS [{Id}] '{Title}' (Condition: {Condition})";
    }
}