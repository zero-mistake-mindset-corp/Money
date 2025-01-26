using Microsoft.EntityFrameworkCore;
using Money.Data.Entities;

namespace Money.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IncomeTransactionEntity>()
            .HasOne(it => it.IncomeType)
            .WithMany(itype => itype.IncomeTransactions)
            .HasForeignKey(it => it.IncomeTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<IncomeTransactionEntity>()
            .HasOne(it => it.MoneyAccount)
            .WithMany(ma => ma.IncomeTransactions)
            .HasForeignKey(it => it.MoneyAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ExpenseTransactionEntity>()
            .HasOne(et => et.ExpenseType)
            .WithMany(etype => etype.ExpenseTransactions)
            .HasForeignKey(et => et.ExpenseTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ExpenseTransactionEntity>()
            .HasOne(et => et.MoneyAccount)
            .WithMany(ma => ma.ExpenseTransactions)
            .HasForeignKey(et => et.MoneyAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<TransferEntity>()
            .HasOne(t => t.SendingMoneyAccount)
            .WithMany(ma => ma.SentTransfers)
            .HasForeignKey(t => t.SendingMoneyAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<TransferEntity>()
            .HasOne(t => t.ReceivingMoneyAccount)
            .WithMany(ma => ma.ReceivedTransfers)
            .HasForeignKey(t => t.ReceivingMoneyAccountId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<ConfirmationCodeEntity> ConfirmationCodes { get; set; }
    public DbSet<MoneyAccountEntity> MoneyAccounts { get; set; }
    public DbSet<IncomeTypeEntity> IncomeTypes { get; set; }
    public DbSet<ExpenseTypeEntity> ExpenseTypes { get; set; }
    public DbSet<IncomeTransactionEntity> IncomeTransactions { get; set; }
    public DbSet<ExpenseTransactionEntity> ExpenseTransactions { get; set; }
    public DbSet<TransferEntity> Transfers { get; set; }
}
