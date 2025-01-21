namespace Money.BL.Models.Transaction;

public class CreateIncomeTransactionModel
{
    public string Name { get; set; }
    public string Comment { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public Guid MoneyAccountId { get; set; }
    public Guid IncomeTypeId { get; set; }
}
