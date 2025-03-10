using Money.BL.Models.Pagination;
using Money.BL.Models.Transaction;
using Money.Data.Entities;

namespace Money.BL.Interfaces.Transactions;

public interface IIncomeTransactionService
{
    Task CreateIncomeTransactionAsync(CreateIncomeTransactionModel model, Guid userId);
    Task<PaginatedResult<IncomeTransactionModel>> GetAllIncomeTransactionsAsync(Guid userId, PaginatedDataRequest request);
    Task<PaginatedResult<IncomeTransactionEntity>> GetIncomeTransactionsByAccAsync(Guid userId, Guid moneyAccountId, PaginatedDataRequest request);
    Task UpdateIncomeTransactionAsync(Guid userId, UpdateIncomeTransactionModel model);
}
