using Money.BL.Models.Transaction;
using Microsoft.EntityFrameworkCore;
using Money.Common.Helpers;
using Money.Data.Entities;
using Money.Data;
using Microsoft.Extensions.Logging;
using Money.Common.Exceptions;
using Money.BL.Interfaces.Transactions;
using Money.BL.Interfaces.MoneyAccount;
using AutoMapper;
using Money.BL.Models.Pagination;

namespace Money.BL.Services.Transactions;

public class ExpenseTransactionService : IExpenseTransactionService
{
    private readonly ILogger<ExpenseTransactionService> _logger;
    private readonly AppDbContext _context;
    private readonly IMoneyAccountService _moneyAccountService;
    private readonly IMapper _mapper;

    public ExpenseTransactionService(AppDbContext context,
        IMoneyAccountService moneyAccountService,
        ILogger<ExpenseTransactionService> logger,
        IMapper mapper)
    {
        _context = context;
        _moneyAccountService = moneyAccountService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task CreateExpenseTransactionAsync(CreateExpenseTransactionModel model, Guid userId)
    {
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

        var newExpenseTransaction = _mapper.Map<ExpenseTransactionEntity>(model);
        newExpenseTransaction.UserId = userId;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _moneyAccountService.UpdateBalanceAsync(model.MoneyAccountId.Value, userId, model.Amount.Value, false);
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

    public async Task<PaginatedResult<ExpenseTransactionModel>> GetAllExpenseTransactionsAsync(Guid userId, PaginatedDataRequest request)
    {
        var query = _context.ExpenseTransactions.AsNoTracking().Where(et => et.UserId == userId);

        if (!string.IsNullOrEmpty(request.SearchedTitle))
        {
            query = query.Where(et => et.Name.ToLower().Contains(request.SearchedTitle.ToLower()));
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int) Math.Ceiling(totalCount / (double)request.PageSize);

        var expenseEntities = await query.OrderByDescending(et => et.TransactionDate)
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

        var expenseTransactions = _mapper.Map<List<ExpenseTransactionModel>>(expenseEntities);
        var result = new PaginatedResult<ExpenseTransactionModel>
        {
            CurrentPage = request.PageIndex,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Data = expenseTransactions
        };

        return result;
    }

    public async Task<PaginatedResult<ExpenseTransactionModel>> GetExpenseTransactionsByAccAsync(Guid userId, Guid moneyAccountId, PaginatedDataRequest request)
    {
        var query = _context.ExpenseTransactions.AsNoTracking().Where(et => et.UserId == userId && et.MoneyAccountId == moneyAccountId);

        if (!string.IsNullOrEmpty(request.SearchedTitle))
        {
            query = query.Where(et => et.Name.ToLower().Contains(request.SearchedTitle.ToLower()));
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var expenseEntities = await query.OrderByDescending(et => et.TransactionDate)
                                        .Skip((request.PageIndex - 1) * request.PageSize)
                                        .Take(request.PageSize)
                                        .ToListAsync();

        var expenseTransactions = _mapper.Map<List<ExpenseTransactionModel>>(expenseEntities);
        var result = new PaginatedResult<ExpenseTransactionModel>
        {
            CurrentPage = request.PageIndex,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Data = expenseTransactions
        };

        return result;
    }

    public async Task UpdateExpenseTransactionAsync(Guid userId, UpdateExpenseTransactionModel model)
    {
        var expenseTransaction = await _context.ExpenseTransactions
            .Where(et => et.UserId == userId && et.Id == model.Id)
            .FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(expenseTransaction);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (expenseTransaction.MoneyAccountId != model.MoneyAccountId)
            {
                if (expenseTransaction.MoneyAccountId.HasValue)
                {
                    await _moneyAccountService.UpdateBalanceAsync(expenseTransaction.MoneyAccountId.Value, userId, expenseTransaction.Amount, true);
                }
                if (model.MoneyAccountId.HasValue)
                {
                    await _moneyAccountService.UpdateBalanceAsync(model.MoneyAccountId.Value, userId, model.Amount, false);
                }
            }
            else
            {
                if (expenseTransaction.Amount != model.Amount)
                {
                    decimal difference = model.Amount - expenseTransaction.Amount;
                    if (difference > 0)
                    {
                        await _moneyAccountService.UpdateBalanceAsync(model.MoneyAccountId.Value, userId, difference, false);
                    }
                    else if (difference < 0)
                    {
                        await _moneyAccountService.UpdateBalanceAsync(model.MoneyAccountId.Value, userId, Math.Abs(difference), true);
                    }
                }
            }

            _mapper.Map(model, expenseTransaction);
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
}
