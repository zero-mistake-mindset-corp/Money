namespace Money.BL.Models.Transaction;

public class CreateIncomeTransactionModel
{
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
    public Guid IncomeTypeId { get; set; }
    public string Name { get; set; }
}
