using Money.BL.Models.Account;

namespace Money.BL.Interfaces;

public interface IMoneyAccountService
{
    Task CreateAccountAsync(CreateMoneyAccountModel model, Guid userId);
    Task<List<MoneyAccountModel>> GetAllAccountsAsync(Guid userId);
    Task UpdateAccountNameAsync(Guid accountId, Guid userId, string newAccountName);
    Task DeleteAccountAsync(Guid accountId, Guid userId);
}