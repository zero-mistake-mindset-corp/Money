namespace Money.Data.Entities;

public class ExpenseTransactionEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Comment { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public Guid UserId { get; set; }
    public virtual UserEntity User { get; set; }
    public Guid MoneyAccountId { get; set; }
    public virtual MoneyAccountEntity MoneyAccount { get; set; }
    public Guid? ExpenseTypeId { get; set; }
    public virtual ExpenseTypeEntity ExpenseType { get; set; }
}
