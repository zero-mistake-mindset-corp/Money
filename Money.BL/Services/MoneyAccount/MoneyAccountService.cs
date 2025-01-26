using Microsoft.EntityFrameworkCore;
using Money.BL.Interfaces.MoneyAccount;
using Money.BL.Models.MoneyAccount;
using Money.Common.Helpers;
using Money.Data;
using Money.Data.Entities;

namespace Money.BL.Services.MoneyAccount;

public class MoneyAccountService : IMoneyAccountService
{
    private readonly AppDbContext _context;

    public MoneyAccountService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAccountAsync(CreateMoneyAccountModel model, Guid userId)
    {
        ValidationHelper.ValidateMoneyValue(model.Balance);
        BaseValidator.ValidateString(model.Name);

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);

        var newAccount = new MoneyAccountEntity
        {
            Balance = model.Balance,
            Name = model.Name,
            UserId = userId,
        };

        await _context.MoneyAccounts.AddAsync(newAccount);
        await _context.SaveChangesAsync();
    }

    public async Task<List<MoneyAccountModel>> GetAllAccountsAsync(Guid userId)
    {
        var accounts = await _context.MoneyAccounts.AsNoTracking().Where(acc => acc.UserId == userId)
            .Select(acc => new MoneyAccountModel
            {
                Id = acc.Id,
                Name = acc.Name,
                Balance = acc.Balance,
            }).ToListAsync();

        return accounts;
    }

    public async Task UpdateAccountNameAsync(Guid accountId, Guid userId, string newAccountName)
    {
        BaseValidator.ValidateString(newAccountName);
        var account = await _context.MoneyAccounts.Where(acc => acc.Id == accountId && acc.UserId == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(account);

        account.Name = newAccountName;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAccountAsync(Guid accountId, Guid userId)
    {
        await _context.MoneyAccounts.Where(acc => acc.Id == accountId && acc.UserId == userId).ExecuteDeleteAsync();
    }

    public async Task UpdateBalanceAsync(Guid accountId, Guid userId, decimal amount, bool isDeposit)
    {
        ValidationHelper.ValidateMoneyValue(amount);
        var account = await _context.MoneyAccounts.Where(acc => acc.Id == accountId && acc.UserId == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(account);

        if (isDeposit)
        {
            account.Balance += amount;
        }
        else
        {
            account.Balance -= amount;
            ValidationHelper.ValidateMoneyValue(account.Balance);
        }
        await _context.SaveChangesAsync();
    }
}
