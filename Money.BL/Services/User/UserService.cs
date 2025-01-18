using Microsoft.EntityFrameworkCore;
using Money.BL.Helpers;
using Money.BL.Interfaces.Infrastructure;
using Money.BL.Interfaces.User;
using Money.BL.Models.Auth;
using Money.BL.Models.Email;
using Money.Common;
using Money.Common.Exceptions;
using Money.Common.Helpers;
using Money.Data;
using Money.Data.Entities;

namespace Money.BL.Services.User;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IUserExistenceService _userExistenceService;

    public UserService(AppDbContext context, IEmailService emailService, IUserExistenceService userExistenceService)
    {
        _context = context;
        _emailService = emailService;
        _userExistenceService = userExistenceService;
    }

    public async Task SignUpAsync(SignUpModel signUpModel)
    {
        ValidationHelper.ValidateSignUpData(signUpModel.Username, signUpModel.Password, signUpModel.Email);
        await _userExistenceService.EnsureUserDoesNotExist(signUpModel.Email, signUpModel.Username);

        var code = new ConfirmationCodeEntity
        {
            Value = CodeCreator.GenerateCode(),
            IsUsed = false,
            Expiration = DateTime.UtcNow.AddMinutes(5),
            Metadata = ConfirmationCodeMetadata.EmailConfirmation
        };

        var userEntity = new UserEntity
        {
            Username = signUpModel.Username,
            Email = signUpModel.Email,
            IsEmailConfirmed = false,
            IsTwoFactorAuthEnabled = true,
            PasswordHash = InformationHasher.HashText(signUpModel.Password),
            ConfirmationCodes = new List<ConfirmationCodeEntity> { code }
        };

        var emailModel = new EmailTemplateModel
        {
            UserName = signUpModel.Username,
            Code = code.Value
        };

        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();
        await _emailService.SendAsync(userEntity.Email, EmailTemplateType.EmailConfirmation, emailModel);
    }

    public async Task ConfirmEmailAsync(string email, string code)
    {
        var user = await _context.Users.Include(u => u.ConfirmationCodes).Where(u => u.Email == email).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        if (user.IsEmailConfirmed)
        {
            throw new PermissionException("You have already confirmed your email address");
        }

        var userCode = user.ConfirmationCodes.FirstOrDefault(c => c.Value == code);
        if (userCode == null
            || userCode.IsUsed
            || userCode.Expiration < DateTime.UtcNow
            || userCode.Metadata != ConfirmationCodeMetadata.EmailConfirmation)
        {
            throw new InvalidInputException("Invalid confirmation code");
        }

        userCode.IsUsed = true;
        user.IsEmailConfirmed = true;
        await _context.SaveChangesAsync();
    }

    public async Task ResendEmailConfirmationCodeAsync(string email)
    {
        var user = await _context.Users.Include(u => u.ConfirmationCodes).Where(u => u.Email == email).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        if (user.IsEmailConfirmed)
        {
            throw new PermissionException("You have already confirmed your email address");
        }

        ConfirmationCodesInvalidator.InvalidatePreviousConfirmationCodes(user.ConfirmationCodes);

        var code = new ConfirmationCodeEntity
        {
            Value = CodeCreator.GenerateCode(),
            IsUsed = false,
            Expiration = DateTime.UtcNow.AddMinutes(5),
            Metadata = ConfirmationCodeMetadata.EmailConfirmation
        };

        var emailModel = new EmailTemplateModel
        {
            UserName = user.Username,
            Code = code.Value
        };

        user.ConfirmationCodes.Add(code);
        await _context.SaveChangesAsync();
        await _emailService.SendAsync(user.Email, EmailTemplateType.EmailConfirmation, emailModel);
    }
}
