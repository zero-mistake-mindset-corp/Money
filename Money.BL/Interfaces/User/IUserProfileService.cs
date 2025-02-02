using Money.BL.Models.UserAccount;

namespace Money.BL.Interfaces.User;

public interface IUserProfileService
{
    Task UpdateUsernameAsync(Guid userId, string newUsername);
    Task<GetUserProfileModel> GetUserProfile(Guid userId);
    Task RequestTwoFactorAuthChangeAsync(Guid userId, bool enable);
    Task ConfirmTwoFactorAuthChangeAsync(Guid userId, string code);
}
