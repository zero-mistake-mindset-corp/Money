using Money.BL.Models.Transaction;

namespace Money.BL.Interfaces.Transactions;

public interface IIncomeTransactionService
{
    Task CreateIncomeTransactionAsync(CreateIncomeTransactionModel model, Guid userId);
    Task<List<IncomeTransactionModel>> GetAllIncomeTransactionsAsync(Guid userId, int pageIndex, int pageSize);
    Task<List<IncomeTransactionModel>> GetIncomeTransactionsByAccAsync(Guid userId, Guid moneyAccountId, int pageIndex, int pageSize);
    Task UpdateIncomeTransactionCommentAsync (Guid userId, Guid incomeTransactionId, string newComment);
}
