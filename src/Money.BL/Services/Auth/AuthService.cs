using Microsoft.EntityFrameworkCore;
using Money.BL.Interfaces.Auth;
using Money.BL.Models.Auth;
using Money.Common.Helpers;
using Money.Data;
using Money.Data.Entities;

namespace Money.BL.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;
    private const int REFRESH_TOKEN_EXPIRATION_DAYS = 30;

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

    public async Task<TokensInfo> RefreshTokensAsync(string refreshToken)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .Where(u => u.RefreshTokens.Any(rt => rt.Value == refreshToken))
            .FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var userRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.Value == refreshToken);
        ValidationHelper.ValidateRefreshToken(userRefreshToken.Expiration);

        userRefreshToken.Expiration = DateTime.UtcNow;
        var tokens = await GenerateAndSaveTokensAsync(user);
        return tokens;
    }

    private async Task<TokensInfo> GenerateAndSaveTokensAsync(UserEntity user)
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
