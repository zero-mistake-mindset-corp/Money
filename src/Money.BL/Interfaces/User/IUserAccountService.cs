using Money.BL.Models.Auth;
using Money.BL.Models.UserAccount;

namespace Money.BL.Interfaces.User;

public interface IUserAccountService
{
    Task SignUpAsync(SignUpModel signUpModel);
    Task<GetUserProfileModel> GetUserAccountAsync(Guid userId);
    Task UpdateAccountAsync(UpdateAccountModel model, Guid userId);
}
