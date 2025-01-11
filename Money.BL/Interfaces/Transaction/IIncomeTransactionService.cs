using Money.BL.Models.Transaction;

namespace Money.BL.Interfaces;

public interface IIncomeTransactionService
{
    Task CreateIncomeTransactionAsync(CreateIncomeTransactionModel model, Guid userId);
    Task<List<IncomeTransactionModel>> GetAllIncomeTransactionsAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
    Task<List<IncomeTransactionModel>> GetLatestIncomeTransactionsAsync(Guid userId);
    Task<List<IncomeTransactionModel>> GetIncomeTransactionsByAccAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
}
