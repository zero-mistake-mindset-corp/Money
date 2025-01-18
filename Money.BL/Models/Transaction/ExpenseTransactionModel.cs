namespace Money.BL.Models.Transaction;

public class ExpenseTransactionModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public Guid MoneyAccountId { get; set; }
    public Guid? ExpenseTypeId { get; set; }
}
