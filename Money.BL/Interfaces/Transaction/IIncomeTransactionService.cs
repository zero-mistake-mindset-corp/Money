using Money.BL.Models.Transaction;

namespace Money.BL.Interfaces;

public interface IIncomeTransactionService
{
    Task CreateIncomeTransactionAsync(CreateIncomeTransactionModel model, Guid userId);
    Task<List<IncomeTransactionModel>> GetAllIncomeTransactionsAsync(Guid userId, int pageIndex, int pageSize);
    Task<List<IncomeTransactionModel>> Get10LastIncomeTransactionsAsync(Guid userId);
    Task<List<IncomeTransactionModel>> GetIncomeTransactionsByAccAsync(Guid userId, Guid moneyAccountId, int pageIndex, int pageSize);
}
