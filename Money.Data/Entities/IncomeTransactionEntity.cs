namespace Money.Data.Entities;

public class IncomeTransactionEntity
{
    public Guid Id { get; set; } 
    public DateTime TransactionDate { get; set; } //not just "Date", because "Date" is DateTime property
    public decimal Amount { get; set; }
    public string AccountName { get; set; }
    public string IncomeTypeName { get; set; }
    public Guid AccountId { get; set; }
    public virtual MoneyAccountEntity MoneyAccount { get; set; }
    public Guid IncomeTypeId { get; set; }
    public virtual IncomeTypeEntity IncomeType { get; set; }
}
