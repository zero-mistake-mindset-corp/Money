using Money.BL.Models.MoneyAccount;

namespace Money.BL.Interfaces.MoneyAccount;

public interface IMoneyAccountService
{
    Task CreateAccountAsync(CreateMoneyAccountModel model, Guid userId);
    Task<List<MoneyAccountModel>> GetAllAccountsAsync(Guid userId);
    Task UpdateAccountNameAsync(UpdateMoneyAccountModel model, Guid userId);
    Task DeleteAccountAsync(Guid accountId, Guid userId);
    Task UpdateBalanceAsync(Guid accountId, Guid userId, decimal amount, bool isDeposit);
}
