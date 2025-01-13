using Money.BL.Models.Transaction;

namespace Money.BL.Interfaces;

public interface IIncomeTransactionService
{
    Task CreateIncomeTransactionAsync(CreateIncomeTransactionModel model, Guid userId);
    Task<List<IncomeTransactionModel>> GetAllIncomeTransactionsAsync(Guid userId, int pageIndex, int pageSize);
    Task<List<IncomeTransactionModel>> GetLatestIncomeTransactionsAsync(Guid userId);
    Task<List<IncomeTransactionModel>> GetIncomeTransactionsByAccAsync(Guid userId, Guid accountId, int pageIndex, int pageSize);
}
