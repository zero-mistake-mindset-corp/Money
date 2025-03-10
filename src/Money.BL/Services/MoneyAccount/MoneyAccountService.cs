using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Money.BL.Interfaces.MoneyAccount;
using Money.BL.Models.MoneyAccount;
using Money.Common.Exceptions;
using Money.Common.Helpers;
using Money.Data;
using Money.Data.Entities;

namespace Money.BL.Services.MoneyAccount;

public class MoneyAccountService : IMoneyAccountService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public MoneyAccountService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task CreateAccountAsync(CreateMoneyAccountModel model, Guid userId)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);

        var newAccount = _mapper.Map<MoneyAccountEntity>(model);
        newAccount.UserId = userId;

        _context.MoneyAccounts.Add(newAccount);
        await _context.SaveChangesAsync();
    }

    public async Task<List<MoneyAccountModel>> GetAllAccountsAsync(Guid userId)
    {
        var accounts = await _context.MoneyAccounts
            .AsNoTracking()
            .Where(acc => acc.UserId == userId)
            .Select(acc => _mapper.Map<MoneyAccountModel>(acc))
            .ToListAsync();

        return accounts;
    }

    public async Task UpdateAccountNameAsync(UpdateMoneyAccountModel model, Guid userId)
    {
        var account = await _context.MoneyAccounts.Where(acc => acc.Id == model.Id && acc.UserId == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(account);

        account = _mapper.Map<MoneyAccountEntity>(model);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAccountAsync(Guid accountId, Guid userId)
    {
        await _context.MoneyAccounts.Where(acc => acc.Id == accountId && acc.UserId == userId).ExecuteDeleteAsync();
    }

    public async Task UpdateBalanceAsync(Guid accountId, Guid userId, decimal amount, bool isDeposit)
    {
        var account = await _context.MoneyAccounts.Where(acc => acc.Id == accountId && acc.UserId == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(account);

        if (isDeposit)
        {
            account.Balance += amount;
        }
        else
        {
            account.Balance -= amount;
            if (account.Balance < 0)
            {
                throw new InvalidInputException("Account balance must be > 0");
            }
        }
        await _context.SaveChangesAsync();
    }
}
