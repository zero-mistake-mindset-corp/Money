using Microsoft.EntityFrameworkCore;
using Money.BL.Interfaces.Infrastructure;
using Money.BL.Interfaces.User;
using Money.BL.Models.Email;
using Money.BL.Models.UserAccount;
using Money.Common;
using Money.Common.Exceptions;
using Money.Common.Helpers;
using Money.Data;
using Money.Data.Entities;

namespace Money.BL.Services.User;

public class UserProfileService : IUserProfileService
{
    private readonly AppDbContext _context;
    private readonly IUserExistenceService _userExistenceService;
    private readonly IEmailService _emailService;

    public UserProfileService(AppDbContext context, IUserExistenceService userExistenceService, IEmailService emailService)
    {
        _context = context;
        _userExistenceService = userExistenceService;
        _emailService = emailService;
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
    
    public async Task RequestTwoFactorAuthChangeAsync(Guid userId, bool enable)
    {
        var userModel = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new {Email = u.Email, Username = u.Username})
            .FirstOrDefaultAsync();
        
        ValidationHelper.EnsureEntityFound(userModel);

        string code = CodeCreator.GenerateCode();

        var confirmationCode = new ConfirmationCodeEntity
        {
            Value = code,
            IsUsed = false,
            Expiration = DateTime.UtcNow.AddMinutes(5),
            Metadata = enable ? ConfirmationCodeMetadata.Enable2FA : ConfirmationCodeMetadata.Disable2FA,
            UserId = userId,
        };

        _context.ConfirmationCodes.Add(confirmationCode);
        await _context.SaveChangesAsync();
        var emailTemplateModel = new EmailTemplateModel
        {
            UserName = userModel.Username,
            Code = code,
            ToggleAction = enable ? "Turn on" : "Turn off"
        };

        await _emailService.SendAsync(userModel.Email, EmailTemplateType.TwoFactorAuthToggle, emailTemplateModel);
    }

    public async Task ConfirmTwoFactorAuthChangeAsync(Guid userId, string code)
    {
        var confirmationCode = await _context.ConfirmationCodes
            .Include(c => c.User)
            .Where(c => c.UserId == userId && c.Value == code)
            .FirstOrDefaultAsync();

        ValidationHelper.EnsureEntityFound(confirmationCode);
        if (confirmationCode.IsUsed 
            || confirmationCode.Expiration < DateTime.UtcNow )
        {
            throw new InvalidInputException("Invalid confirmation code");
        }

        confirmationCode.User.IsTwoFactorAuthEnabled = confirmationCode.Metadata == ConfirmationCodeMetadata.Enable2FA;
        confirmationCode.IsUsed = true;

        await _context.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
    {
        var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        BaseValidator.ValidatePassword(newPassword);
        if (InformationHasher.VerifyText(oldPassword, user.PasswordHash) == false)
        {
            throw new PermissionException("Invalid old password");
        }
        
        user.PasswordHash = InformationHasher.HashText(newPassword);
        await _context.SaveChangesAsync();
    }

    public async Task RequestEmailChangingAsync(Guid userId, string newEmail)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        BaseValidator.ValidateEmail(newEmail);

        var confirmationCode = new ConfirmationCodeEntity
        {
            Value = CodeCreator.GenerateCode(),
            Expiration = DateTime.UtcNow.AddMinutes(5),
            Metadata = newEmail,
            UserId = userId,
        };
        
        _context.ConfirmationCodes.Add(confirmationCode);
        await _context.SaveChangesAsync();
        var emailTemplateModel = new EmailTemplateModel
        {
            UserName = user.Username,
            Code = confirmationCode.Value,
            NewEmail = newEmail,
        };

        await _emailService.SendAsync(user.Email, EmailTemplateType.EmailChange, emailTemplateModel);
    }

    public async Task ConfirmEmailChangingAsync(Guid userId, string codeValue, string newEmail)
    {
        var user = await _context.Users
            .Include(u => u.ConfirmationCodes)
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        
        var code = user.ConfirmationCodes.FirstOrDefault(cc => cc.Value == codeValue);
        if (code == null || code.IsUsed || code.Expiration < DateTime.UtcNow || code.Metadata != newEmail)
        {
            throw new InvalidInputException("Invalid confirmation code");
        }

        user.Email = newEmail;
        code.IsUsed = true;
        await _context.SaveChangesAsync();
    }
}
