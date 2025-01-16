using Money.BL.Models.UserAccount;

namespace Money.BL.Services.User;

public interface IUserProfileService
{
    Task UpdateUsernameAsync(Guid userId, string newUsername);
    Task<GetUserProfileModel> GetUserProfile(Guid userId);
}