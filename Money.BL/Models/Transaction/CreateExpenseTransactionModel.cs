namespace Money.BL.Models.Transaction;

public class CreateExpenseTransactionModel
{
    public string Name { get; set; }
    public string Comment { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public Guid MoneyAccountId { get; set; }
    public Guid ExpenseTypeId { get; set; }
}
