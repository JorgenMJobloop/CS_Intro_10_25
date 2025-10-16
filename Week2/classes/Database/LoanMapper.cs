public static class LoanMapper
{
    public static LoanDto ToDto(LoanRecord loan) => new LoanDto
    {
        ItemId = loan.ItemId,
        CustomerId = loan.CustomerId,
        From = loan.From
    };

    public static LoanRecord FromDto(LoanDto dto) => new LoanRecord
    {
        ItemId = dto.ItemId,
        CustomerId = dto.CustomerId,
        From = dto.From
    };
}