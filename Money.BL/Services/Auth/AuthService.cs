using Microsoft.EntityFrameworkCore;
using Money.BL.Interfaces.Auth;
using Money.BL.Models.Auth;
using Money.Common.Exceptions;
using Money.Common.Helpers;
using Money.Data;
using Money.Data.Entities;

namespace Money.BL.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;
    private const int REFRESH_TOKEN_EXPIRATION = 30;

    public AuthService(ITokenService tokenService, AppDbContext context)
    {
        _tokenService = tokenService;
        _context = context;
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
        ValidationHelper.ValidateSignUpData(signUpModel.Username, signUpModel.Password);
        EnsureUserDoesNotExistAsync(signUpModel.Username);

        var passwordHash = InformationHasher.HashText(signUpModel.Password);

        var userEntity = new UserEntity
        {
            Username = signUpModel.Username,
            PasswordHash = passwordHash
        };

        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();
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
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(REFRESH_TOKEN_EXPIRATION);

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

    private void EnsureUserDoesNotExistAsync(string username)
    {
        if (_context.Users.Any(u => u.Username == username))
        {
            throw new EntityExistsException("Данный username уже занят.");
        }
    }
}
