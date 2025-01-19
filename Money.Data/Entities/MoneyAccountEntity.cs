namespace Money.Data.Entities;

public class MoneyAccountEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public Guid UserId { get; set; }
    public virtual UserEntity User { get; set; }
    public virtual ICollection<IncomeTransactionEntity> IncomeTransactions { get; set; }
    public virtual ICollection<ExpenseTransactionEntity> ExpenseTransactions { get; set; }
    public virtual ICollection<TransferEntity> SentTransfers { get; set; }
    public virtual ICollection<TransferEntity> RecievedTransfers { get; set; }
}
