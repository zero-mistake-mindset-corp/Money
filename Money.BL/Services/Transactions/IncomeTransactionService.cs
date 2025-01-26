using Money.BL.Models.Transaction;
using Microsoft.EntityFrameworkCore;
using Money.Common.Helpers;
using Money.Data.Entities;
using Money.Data;
using Microsoft.Extensions.Logging;
using Money.Common.Exceptions;
using Money.BL.Interfaces.Transactions;
using Money.BL.Interfaces.MoneyAccount;

namespace Money.BL.Services.Transactions;

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
        BaseValidator.ValidateString(model.Comment, maxLength: 250);
        BaseValidator.ValidateDate(model.TransactionDate);

        var user = await _context.Users.AsNoTracking().Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var isMoneyAccountExist = await _context.MoneyAccounts.AnyAsync(ma => ma.UserId == user.Id && ma.Id == model.MoneyAccountId);
        if (isMoneyAccountExist == false)
        {
            throw new NotFoundException("Money account with this ID does not exist.");
        }

        var isIncomeTypeExist = await _context.IncomeTypes.AnyAsync(it => it.UserId == userId && it.Id == model.IncomeTypeId);
        if (isIncomeTypeExist == false)
        {
            throw new NotFoundException("Income type with this ID does not exist.");
        }

        var newIncomeTransaction = new IncomeTransactionEntity
        {
            TransactionDate = model.TransactionDate,
            Amount = model.Amount,
            MoneyAccountId = model.MoneyAccountId,
            IncomeTypeId = model.IncomeTypeId,
            Name = model.Name,
            Comment = model.Comment
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _moneyAccountService.UpdateBalanceAsync(model.MoneyAccountId, userId, model.Amount, true);
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
                MoneyAccountId = it.MoneyAccountId,
                IncomeTypeId = it.IncomeTypeId,
                Comment = it.Comment
            }).ToListAsync();

        return incomeTransactions;
    }

    public async Task<List<IncomeTransactionModel>> Get10LastIncomeTransactionsAsync(Guid userId)
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
                MoneyAccountId = it.MoneyAccountId,
                IncomeTypeId = it.IncomeTypeId,
                Name = it.Name,
                Comment = it.Comment
            }).ToListAsync();

        return incomeTransactions;
    }

    public async Task<List<IncomeTransactionModel>> GetIncomeTransactionsByAccAsync(Guid userId, Guid moneyAccountId, int pageIndex, int pageSize)
    {
        var incomeTransactions = await _context.IncomeTransactions.AsNoTracking()
            .Where(it => it.MoneyAccount.UserId == userId && it.MoneyAccount.Id == moneyAccountId)
            .OrderByDescending(it => it.TransactionDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(it => new IncomeTransactionModel
            {
                Id = it.Id,
                TransactionDate = it.TransactionDate,
                Amount = it.Amount,
                MoneyAccountId = it.MoneyAccountId,
                IncomeTypeId = it.IncomeTypeId,
                Name = it.Name,
                Comment = it.Comment
            }).ToListAsync();

        return incomeTransactions;
    }

    public async Task UpdateIncomeTransactionCommentAsync(Guid userId, Guid incomeTransactionId, string newComment)
    {
        BaseValidator.ValidateString(newComment, maxLength: 250);

        var incomeTransaction = await _context.IncomeTransactions
            .Include(et => et.MoneyAccount)
            .Where(it => it.MoneyAccount.UserId == userId && it.Id == incomeTransactionId).FirstOrDefaultAsync();
        
        incomeTransaction.Comment = newComment;
        await _context.SaveChangesAsync();
    }
}
