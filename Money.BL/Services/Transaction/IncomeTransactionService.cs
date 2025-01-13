using Money.BL.Models.Transaction;
using Money.BL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Money.Common.Helpers;
using Money.Data.Entities;
using Money.Data;
using Microsoft.Extensions.Logging;

namespace Money.BL.Services;

public class IncomeTransactionService : IIncomeTransactionService
{
    private readonly ILogger<IncomeTransactionService> _logger;
    private readonly AppDbContext _context;
    private readonly IMoneyAccountService _moneyAccountService;

    public IncomeTransactionService(AppDbContext context, IMoneyAccountService moneyAccountService, ILogger<IncomeTransactionService> logger)
    {
        _context = context;
        _moneyAccountService = moneyAccountService;
        _logger = logger;
    }

    public async Task CreateIncomeTransactionAsync(CreateIncomeTransactionModel model, Guid userId)
    {
        ValidationHelper.ValidateMoneyValue(model.Amount);
        BaseValidator.ValidateString(model.Name, maxLength: 100);

        var user = await _context.Users.AsNoTracking()
            .Include(u => u.MoneyAccounts)
            .Include(u => u.IncomeTypes)
            .FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);

        var moneyAccount = user.MoneyAccounts.FirstOrDefault(acc => acc.Id == model.MoneyAccountId);
        ValidationHelper.EnsureEntityFound(moneyAccount);

        var incomeType = user.IncomeTypes.FirstOrDefault(it => it.Id == model.IncomeTypeId);
        ValidationHelper.EnsureEntityFound(incomeType);

        var newIncomeTransaction = new IncomeTransactionEntity
        {
            TransactionDate = model.TransactionDate,
            Amount = model.Amount,
            MoneyAccountId = moneyAccount.Id,
            IncomeTypeId = incomeType.Id,
            Name = model.Name,
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _moneyAccountService.UpdateBalanceAsync(moneyAccount.Id, userId, model.Amount, true);
            _context.IncomeTransactions.Add(newIncomeTransaction);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<List<IncomeTransactionModel>> GetAllIncomeTransactionsAsync(Guid userId, int pageIndex, int pageSize)
    {
        var incomeTransactions = await _context.IncomeTransactions.AsNoTracking()
            .Where(it => it.MoneyAccount.UserId == userId)
            .OrderByDescending(it => it.TransactionDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(it => new IncomeTransactionModel
            {
                Id = it.Id,
                Name = it.Name,
                TransactionDate = it.TransactionDate,
                Amount = it.Amount,
                AccountId = it.MoneyAccountId,
                IncomeTypeId = it.IncomeTypeId
            }).ToListAsync();

        return incomeTransactions;
    }

    public async Task<List<IncomeTransactionModel>> GetLatestIncomeTransactionsAsync(Guid userId)
    {
        var incomeTransactions = await _context.IncomeTransactions.AsNoTracking()
            .Where(it => it.MoneyAccount.UserId == userId)
            .OrderByDescending(it => it.TransactionDate)
            .Take(10)
            .Select(it => new IncomeTransactionModel
            {
                Id = it.Id,
                TransactionDate = it.TransactionDate,
                Amount = it.Amount,
                AccountId = it.MoneyAccountId,
                IncomeTypeId = it.IncomeTypeId,
                Name = it.Name
            }).ToListAsync();

        return incomeTransactions;
    }

    public async Task<List<IncomeTransactionModel>> GetIncomeTransactionsByAccAsync(Guid userId, Guid accountId, int pageIndex, int pageSize)
    {
        var incomeTransactions = await _context.IncomeTransactions.AsNoTracking()
            .Where(it => it.MoneyAccount.UserId == userId && it.MoneyAccountId == accountId)
            .OrderByDescending(it => it.TransactionDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(it => new IncomeTransactionModel
            {
                Id = it.Id,
                TransactionDate = it.TransactionDate,
                Amount = it.Amount,
                AccountId = it.MoneyAccountId,
                IncomeTypeId = it.IncomeTypeId,
                Name = it.Name
            }).ToListAsync();

        return incomeTransactions;
    }
}
