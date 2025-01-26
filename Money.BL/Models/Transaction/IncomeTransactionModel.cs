namespace Money.BL.Models.Transaction;

public class IncomeTransactionModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Comment { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public Guid UserId { get; set; }
    public Guid? MoneyAccountId { get; set; }
    public Guid? IncomeTypeId { get; set; }
}
