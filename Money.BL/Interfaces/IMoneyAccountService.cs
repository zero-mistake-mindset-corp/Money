using Money.BL.Models.MoneyAccount;

namespace Money.BL.Interfaces;

public interface IMoneyAccountService
{
    Task CreateAccountAsync(CreateMoneyAccountModel model, Guid userId);
    Task<List<MoneyAccountModel>> GetAllAccountsAsync(Guid userId);
    Task UpdateAccountNameAsync(Guid accountId, Guid userId, string newAccountName);
    Task DeleteAccountAsync(Guid accountId, Guid userId);
    Task UpdateBalanceAsync(Guid accountId, Guid userId, decimal amount, bool isDeposit);
}