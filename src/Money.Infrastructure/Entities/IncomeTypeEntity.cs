namespace Money.Data.Entities;

public class IncomeTypeEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public virtual UserEntity User { get; set; }
    public virtual ICollection<IncomeTransactionEntity> IncomeTransactions { get; set; }
}
