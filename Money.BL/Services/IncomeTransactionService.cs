using Money.BL.Models.Transaction;
using Money.BL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Money.Common.Helpers;
using Money.Data.Entities;
using Money.Data;

namespace Money.BL.Services;

public class IncomeTransactionService : IIncomeTransactionService
{
    private readonly AppDbContext _context;
    private readonly IMoneyAccountService _moneyAccountService;

    public IncomeTransactionService(AppDbContext context, IMoneyAccountService moneyAccountService)
    {
        _context = context;
        _moneyAccountService = moneyAccountService;
    }

    public async Task CreateIncomeTransactionAsync(CreateIncomeTransactionModel model, Guid userId)
    {   
        ValidationHelper.ValidateNonNegative(model.Amount);

        var user = await _context.Users.AsNoTracking()
            .Include(u => u.MoneyAccounts)
            .Include(u => u.IncomeTypes)
            .FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);

        var account = user.MoneyAccounts
            .FirstOrDefault(acc => acc.Id == model.AccountId);
        ValidationHelper.EnsureEntityFound(account);                

        var incomeType = user.IncomeTypes
            .FirstOrDefault(it => it.Id == model.AccountId);
        ValidationHelper.EnsureEntityFound(incomeType);

        var newIncomeTransaction = new IncomeTransactionEntity
        {   
            TransactionDate = model.TransactionDate,
            Amount = model.Amount,
            AccountId = account.Id,
            IncomeTypeId = incomeType.Id
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _moneyAccountService.UpdateBalanceAsync(account.Id, userId, model.Amount, true);
            _context.IncomeTransactions.Add(newIncomeTransaction);
            await _context.SaveChangesAsync(); 
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<IncomeTransactionModel>> GetAllIncomeTransactionsAsync(Guid accountId, Guid userId)
    {
        var incomeTransactions = await _context.IncomeTransactions.AsNoTracking()
            .Where(it => it.MoneyAccount.Id == accountId && it.MoneyAccount.UserId == userId)
            .Select(it => new IncomeTransactionModel
            {
                Id = it.Id,
                TransactionDate = it.TransactionDate,
                Amount = it.Amount,
                AccountId = it.AccountId,
                IncomeTypeId = it.IncomeTypeId
            }).ToListAsync();
        return incomeTransactions;
    }
}
