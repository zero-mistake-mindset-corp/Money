using Money.BL.Models.Transaction;

namespace Money.BL.Interfaces;

public interface IIncomeTransactionService
{
    Task CreateIncomeTransactionAsync(CreateIncomeTransactionModel model, Guid userId);
    Task<List<IncomeTransactionModel>> GetAllIncomeTransactionsAsync(Guid accountId, Guid userId);
}
