using Microsoft.EntityFrameworkCore;
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
        var user = await _context.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Username == signInModel.Username);
        ValidationHelper.EnsureEntityFound(user);
        ValidationHelper.ValidateSignInData(user.PasswordHash, signInModel.Password);

        var tokens = await GenerateAndSaveTokensAsync(user);
        return tokens;
    }

    public async Task SignUpAsync(SignUpModel signUpModel)
    {
        ValidationHelper.ValidateSignUpData(signUpModel.Username, signUpModel.Password, signUpModel.Email);
        EnsureUserDoesNotExist(signUpModel.Email);

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

    private void EnsureUserDoesNotExist(string email)
    {
        if (_context.Users.Any(u => u.Email == email))
        {
            throw new EntityExistsException("User with this email already exists.");
        }
    }
}
