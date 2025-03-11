namespace Money.Data.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Username {  get; set; }
    public string PasswordHash { get; set; }
    public virtual ICollection<RefreshTokenEntity> RefreshTokens { get; set; }
    public virtual ICollection<MoneyAccountEntity> MoneyAccounts { get; set; }
    public virtual ICollection<IncomeTypeEntity> IncomeTypes { get; set; }
    public virtual ICollection<ExpenseTypeEntity> ExpenseTypes { get; set; }
    public virtual ICollection<IncomeTransactionEntity> IncomeTransactions { get; set; }
    public virtual ICollection<ExpenseTransactionEntity> ExpenseTransactions { get; set; }
}
