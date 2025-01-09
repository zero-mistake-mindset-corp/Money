namespace Money.BL.Models.Transaction;

public class CreateIncomeTransactionModel
{
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string AccountName { get; set; }
    public string IncomeTypeName { get; set; }
}
