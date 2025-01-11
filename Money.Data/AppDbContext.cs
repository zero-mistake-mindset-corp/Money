using Microsoft.EntityFrameworkCore;
using Money.Data.Entities;

namespace Money.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<ConfirmationCodeEntity> ConfirmationCodes { get; set; }
    public DbSet<MoneyAccountEntity> MoneyAccounts { get; set; }
    public DbSet<IncomeTypeEntity> IncomeTypes { get; set; }
    public DbSet<ExpenseTypeEntity> ExpenseTypes { get; set; }
    public DbSet<IncomeTransactionEntity> IncomeTransactions { get; set; }
}
