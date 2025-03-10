using Money.BL.Models.Transaction;
using Microsoft.EntityFrameworkCore;
using Money.Common.Helpers;
using Money.Data.Entities;
using Money.Data;
using Microsoft.Extensions.Logging;
using Money.Common.Exceptions;
using Money.BL.Interfaces.Transactions;
using Money.BL.Interfaces.MoneyAccount;
using Money.BL.Models.Pagination;
using AutoMapper;

namespace Money.BL.Services.Transactions;

public class IncomeTransactionService : IIncomeTransactionService
{
    private readonly ILogger<IncomeTransactionService> _logger;
    private readonly AppDbContext _context;
    private readonly IMoneyAccountService _moneyAccountService;
    private readonly IMapper _mapper;

    public IncomeTransactionService(AppDbContext context,
        IMoneyAccountService moneyAccountService,
        ILogger<IncomeTransactionService> logger,
        IMapper mapper)
    {
        _context = context;
        _moneyAccountService = moneyAccountService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task CreateIncomeTransactionAsync(CreateIncomeTransactionModel model, Guid userId)
    {
        var user = await _context.Users.AsNoTracking().Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var isMoneyAccountExist = await _context.MoneyAccounts.Where(ma => ma.UserId == user.Id && ma.Id == model.MoneyAccountId).AnyAsync();
        if (isMoneyAccountExist == false)
        {
            throw new NotFoundException("Money account with this ID does not exist.");
        }

        var isIncomeTypeExists = await _context.IncomeTypes.Where(it => it.UserId == userId && it.Id == model.IncomeTypeId).AnyAsync();
        if (isIncomeTypeExists == false)
        {
            throw new NotFoundException("Income type with this ID does not exist.");
        }

        var newIncomeTransaction = _mapper.Map<IncomeTransactionEntity>(model);
        newIncomeTransaction.UserId = userId;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _moneyAccountService.UpdateBalanceAsync(model.MoneyAccountId.Value, userId, model.Amount.Value, true);
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

    public async Task<PaginatedResult<IncomeTransactionModel>> GetAllIncomeTransactionsAsync(Guid userId, PaginatedDataRequest request)
    {
        var query = _context.IncomeTransactions.AsNoTracking().Where(it => it.UserId == userId);

        if (!string.IsNullOrEmpty(request.SearchedTitle))
        {
            query = query.Where(it => it.Name.ToLower().Contains(request.SearchedTitle.ToLower()));
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var incomeEntities = await query.OrderByDescending(it => it.TransactionDate)
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

        var incomeTransactions = _mapper.Map<List<IncomeTransactionModel>>(incomeEntities);
        var result = new PaginatedResult<IncomeTransactionModel>
        {
            CurrentPage = request.PageIndex,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Data = incomeTransactions
        };

        return result;
    }

    public async Task<PaginatedResult<IncomeTransactionEntity>> GetIncomeTransactionsByAccAsync(Guid userId, Guid moneyAccountId, PaginatedDataRequest request)
    {
        var query = _context.IncomeTransactions.AsNoTracking().Where(it => it.UserId == userId && it.MoneyAccountId == moneyAccountId);

        if (!string.IsNullOrEmpty(request.SearchedTitle))
        {
            query = query.Where(it => it.Name.Contains(request.SearchedTitle));
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var incomeEntities = await query.OrderByDescending(it => it.TransactionDate)
                                        .Skip((request.PageIndex - 1) * request.PageSize)
                                        .Take(request.PageSize)
                                        .ToListAsync();

        var expenseTransactions = _mapper.Map<List<IncomeTransactionEntity>>(incomeEntities);
        var result = new PaginatedResult<IncomeTransactionEntity>
        {
            CurrentPage = request.PageIndex,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Data = expenseTransactions
        };

        return result;
    }

    public async Task UpdateIncomeTransactionAsync(Guid userId, UpdateIncomeTransactionModel model)
    {
        var incomeTransactions = await _context.IncomeTransactions
            .Where(it => it.UserId == userId && it.Id == model.Id)
            .FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(incomeTransactions);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (incomeTransactions.MoneyAccountId != model.MoneyAccountId)
            {
                if (incomeTransactions.MoneyAccountId.HasValue)
                {
                    await _moneyAccountService.UpdateBalanceAsync(incomeTransactions.MoneyAccountId.Value, userId, incomeTransactions.Amount, false);
                }
                if (model.MoneyAccountId.HasValue)
                {
                    await _moneyAccountService.UpdateBalanceAsync(model.MoneyAccountId.Value, userId, model.Amount, true);
                }
            }
            else
            {
                if (incomeTransactions.Amount != model.Amount)
                {
                    decimal difference = model.Amount - incomeTransactions.Amount;
                    if (difference > 0)
                    {
                        await _moneyAccountService.UpdateBalanceAsync(model.MoneyAccountId.Value, userId, difference, true);
                    }
                    else if (difference < 0)
                    {
                        await _moneyAccountService.UpdateBalanceAsync(model.MoneyAccountId.Value, userId, Math.Abs(difference), false);
                    }
                }
            }

            _mapper.Map(model, incomeTransactions);
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
