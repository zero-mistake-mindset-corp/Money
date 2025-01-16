using Microsoft.EntityFrameworkCore;
using Money.BL.Helpers;
using Money.BL.Interfaces.Auth;
using Money.BL.Interfaces.Infrastructure;
using Money.BL.Models.Auth;
using Money.BL.Models.Email;
using Money.Common;
using Money.Common.Exceptions;
using Money.Common.Helpers;
using Money.Data;
using Money.Data.Entities;

namespace Money.BL.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private const int REFRESH_TOKEN_EXPIRATION_DAYS = 30;

    public AuthService(ITokenService tokenService, AppDbContext context, IEmailService emailService)
    {
        _tokenService = tokenService;
        _context = context;
        _emailService = emailService;
    }

    public async Task<TokensInfo> SignInAsync(SignInModel signInModel)
    {
        var user = await _context.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == signInModel.EmailOrUsername || u.Username == signInModel.EmailOrUsername);
        ValidationHelper.EnsureEntityFound(user);
        ValidationHelper.ValidateSignInData(user.PasswordHash, signInModel.Password);

        if (user.IsEmailConfirmed == false)
        {
            throw new PermissionException("You must confirm your email to authenticate.");
        }

        var tokens = await GenerateAndSaveTokensAsync(user);
        return tokens;
    }

    public async Task<TokensInfo> RefreshTokensAsync(string refreshToken)
    {
        var user = await _context.Users.Include(u => u.RefreshTokens).Where(u => u.RefreshTokens.Any(rt => rt.Value == refreshToken)).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var userRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.Value == refreshToken);
        ValidationHelper.ValidateRefreshToken(userRefreshToken.Expiration);

        userRefreshToken.Expiration = DateTime.UtcNow;
        var tokens = await GenerateAndSaveTokensAsync(user);
        return tokens;
    }

    public async Task<string> Send2FACodeAsync(string emailOrUsername, string password)
    {
        var user = await _context.Users.Include(u => u.ConfirmationCodes).FirstOrDefaultAsync(u => u.Email == emailOrUsername || u.Username == emailOrUsername); 
        ValidationHelper.EnsureEntityFound(user);
        ValidationHelper.ValidateSignInData(user.PasswordHash, password);
        if (user.IsEmailConfirmed == false)
        {
            throw new PermissionException("You must confirm your email to authenticate.");
        }

        ConfirmationCodesInvalidator.InvalidatePreviousConfirmationCodes(user.ConfirmationCodes);

        var code = new ConfirmationCodeEntity
        {
            Value = CodeCreator.GenerateCode(),
            IsUsed = false,
            Metadata = ConfirmationCodeMetadata.TwoFactorAuth,
            Expiration = DateTime.UtcNow.AddMinutes(5)
        };

        var emailTemplateModel = new EmailTemplateModel
        {
            UserName = user.Username,
            Code = code.Value,
        };

        user.ConfirmationCodes.Add(code);
        await _context.SaveChangesAsync();
        await _emailService.SendAsync(user.Email, EmailTemplateType.TwoFactorAuth, emailTemplateModel);
        return user.Email;
    }

    public async Task<TokensInfo> SignInWithCodeAsync(string emailOrUsername, string password, string code)
    {
        var user = await _context.Users.Include(u => u.ConfirmationCodes).Include(u => u.RefreshTokens).Where(u => u.Email == emailOrUsername || u.Username == emailOrUsername).FirstOrDefaultAsync();
        ValidationHelper.ValidateSignInData(user.PasswordHash, password);

        if (user.IsEmailConfirmed == false)
        {
            throw new PermissionException("You must confirm your email to use app.");
        }

        var userCode = user.ConfirmationCodes.FirstOrDefault(c => c.Value == code);
        if (userCode == null
            || userCode.IsUsed
            || userCode.Expiration < DateTime.UtcNow
            || userCode.Metadata != ConfirmationCodeMetadata.TwoFactorAuth)
        {
            throw new InvalidInputException("Invalid confirmation code");
        }

        userCode.IsUsed = true;
        var tokens = await GenerateAndSaveTokensAsync(user);
        return tokens;
    }

    public async Task<bool> IsTwoFactorAuthEnabled(string emailOrUsername)
    {
        var user = await _context.Users.Where(u => u.Email == emailOrUsername || u.Username == emailOrUsername).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        var is2FAEnabled = user.IsTwoFactorAuthEnabled;
        return is2FAEnabled;
    }

    public async Task<TokensInfo> GenerateAndSaveTokensAsync(UserEntity user)
    {
        var accessTokenInfo = _tokenService.GenerateAccessToken(user.Id);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(REFRESH_TOKEN_EXPIRATION_DAYS);

        var refreshTokenEntity = new RefreshTokenEntity
        {
            Value = refreshTokenValue,
            Expiration = refreshTokenExpiryTime
        };

        user.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        var tokensInfo = new TokensInfo
        {
            AccessToken = accessTokenInfo.Value,
            RefreshToken = refreshTokenValue,
            AccessTokenExpiration = accessTokenInfo.Expiration,
            RefreshTokenExpiration = refreshTokenExpiryTime
        };

        return tokensInfo;
    }
}
