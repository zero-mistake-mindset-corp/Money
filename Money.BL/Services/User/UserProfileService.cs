using Microsoft.EntityFrameworkCore;
using Money.BL.Models.UserAccount;
using Money.Common.Helpers;
using Money.Data;

namespace Money.BL.Services.User;

public class UserProfileService : IUserProfileService
{
    private readonly AppDbContext _context;
    private readonly IUserExistenceService _userExistenceService;

    public UserProfileService(AppDbContext context, IUserExistenceService userExistenceService)
    {
        _context = context;
        _userExistenceService = userExistenceService;
    }

    public async Task<GetUserProfileModel> GetUserProfile(Guid userId)
    {
        var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var userProfile = new GetUserProfileModel
        {
            UserName = user.Username,
            Email = user.Email,
            Is2FAEnabled = user.IsTwoFactorAuthEnabled
        };

        return userProfile;
    }

    public async Task UpdateUsernameAsync(Guid userId, string newUsername)
    {
        BaseValidator.ValidateString(newUsername);
        var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        await _userExistenceService.EnsureUserDoesNotExist(newUsername);

        user.Username = newUsername;
        await _context.SaveChangesAsync();
    }
}
