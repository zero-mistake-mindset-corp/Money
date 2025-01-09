namespace Money.BL.Models.Transaction;

public class IncomeTransactionModel
{
    public Guid Id { get; set; } 
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string AccountName { get; set; }
    public string IncomeTypeName { get; set; }
}
