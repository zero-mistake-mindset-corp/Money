using Money.BL.Models.Transaction;

namespace Money.BL.Interfaces.Transactions;

public interface IExpenseTransactionService
{
    Task CreateExpenseTransactionAsync(CreateExpenseTransactionModel model, Guid userId);
    Task<List<ExpenseTransactionModel>> GetAllExpenseTransactionsAsync(Guid userId, int pageIndex, int pageSize);
    Task<List<ExpenseTransactionModel>> GetExpenseTransactionsByAccAsync(Guid userId, Guid moneyAccountId, int pageIndex, int pageSize);
    Task UpdateExpenseTransactionCommentAsync (Guid userId, Guid expenseTransactionId, string newComment);
}
