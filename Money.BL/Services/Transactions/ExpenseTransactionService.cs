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

public class ExpenseTransactionService : IExpenseTransactionService
{
    private readonly ILogger<ExpenseTransactionService> _logger;
    private readonly AppDbContext _context;
    private readonly IMoneyAccountService _moneyAccountService;

    public ExpenseTransactionService(AppDbContext context, IMoneyAccountService moneyAccountService, ILogger<ExpenseTransactionService> logger)
    {
        _context = context;
        _moneyAccountService = moneyAccountService;
        _logger = logger;
    }

    public async Task CreateExpenseTransactionAsync(CreateExpenseTransactionModel model, Guid userId)
    {
        ValidationHelper.ValidateMoneyValue(model.Amount);
        BaseValidator.ValidateString(model.Name, maxLength: 100);
        BaseValidator.ValidateString(model.Comment, maxLength: 250);
        BaseValidator.ValidateDate(model.TransactionDate);

        var user = await _context.Users.AsNoTracking().Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var isMoneyAccountExist = await _context.MoneyAccounts.Where(ma => ma.UserId == user.Id && ma.Id == model.MoneyAccountId).AnyAsync();
        if (isMoneyAccountExist == false)
        {
            throw new NotFoundException("Money account with this ID does not exist.");
        }

        var isExpenseTypeExist = await _context.ExpenseTypes.Where(etype => etype.UserId == userId && etype.Id == model.ExpenseTypeId).AnyAsync();
        if (isExpenseTypeExist == false)
        {
            throw new NotFoundException("Expense type with this ID does not exist.");
        }

        var newExpenseTransaction = new ExpenseTransactionEntity
        {
            Name = model.Name,
            Comment = model.Comment,
            TransactionDate = model.TransactionDate,
            Amount = model.Amount,
            MoneyAccountId = model.MoneyAccountId,
            ExpenseTypeId = model.ExpenseTypeId,
            UserId = userId,
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _moneyAccountService.UpdateBalanceAsync(model.MoneyAccountId, userId, model.Amount, false);
            _context.ExpenseTransactions.Add(newExpenseTransaction);
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

    public async Task<List<ExpenseTransactionModel>> GetAllExpenseTransactionsAsync(Guid userId, int pageIndex, int pageSize)
    {
        var expenseTransactions = await _context.ExpenseTransactions.AsNoTracking()
            .Where(et => et.UserId == userId)
            .OrderByDescending(et => et.TransactionDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(et => new ExpenseTransactionModel
            {
                Id = et.Id,
                Name = et.Name,
                TransactionDate = et.TransactionDate,
                Amount = et.Amount,
                MoneyAccountId = et.MoneyAccountId,
                ExpenseTypeId = et.ExpenseTypeId,
                UserId = et.UserId,
                Comment = et.Comment
            }).ToListAsync();

        return expenseTransactions;
    }

    public async Task<List<ExpenseTransactionModel>> GetExpenseTransactionsByAccAsync(Guid userId, Guid moneyAccountId, int pageIndex, int pageSize)
    {
        var expenseTransactions = await _context.ExpenseTransactions.AsNoTracking()
            .Where(et => et.UserId == userId && et.MoneyAccount.Id == moneyAccountId)
            .OrderByDescending(et => et.TransactionDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(et => new ExpenseTransactionModel
            {
                Id = et.Id,
                TransactionDate = et.TransactionDate,
                Amount = et.Amount,
                MoneyAccountId = et.MoneyAccountId,
                ExpenseTypeId = et.ExpenseTypeId,
                UserId = et.UserId,
                Name = et.Name,
                Comment = et.Comment
            }).ToListAsync();

        return expenseTransactions;
    }

    public async Task UpdateExpenseTransactionCommentAsync (Guid userId, Guid expenseTransactionId, string newComment)
    {
        BaseValidator.ValidateString(newComment, maxLength: 250);
        var expenseTransaction = await _context.ExpenseTransactions.Where(et => et.UserId == userId && et.Id == expenseTransactionId).FirstOrDefaultAsync();
        
        expenseTransaction.Comment = newComment;
        await _context.SaveChangesAsync();
    }

}
