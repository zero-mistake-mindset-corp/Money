using Money.BL.Models.Pagination;
using Money.BL.Models.Transaction;

namespace Money.BL.Interfaces.Transactions;

public interface IExpenseTransactionService
{
    Task CreateExpenseTransactionAsync(CreateExpenseTransactionModel model, Guid userId);
    Task<PaginatedResult<ExpenseTransactionModel>> GetAllExpenseTransactionsAsync(Guid userId, PaginatedDataRequest request);
    Task<PaginatedResult<ExpenseTransactionModel>> GetExpenseTransactionsByAccAsync(Guid userId, Guid moneyAccountId, PaginatedDataRequest request);
    Task UpdateExpenseTransactionAsync(Guid userId, UpdateExpenseTransactionModel model);
}
