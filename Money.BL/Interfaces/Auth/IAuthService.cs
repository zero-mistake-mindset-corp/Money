using Money.BL.Models.Auth;
using Money.Data.Entities;

namespace Money.BL.Interfaces.Auth;

public interface IAuthService
{
    Task<TokensInfo> SignInAsync(SignInModel signInModel);
    Task<TokensInfo> RefreshTokensAsync(string refreshToken);
    Task<string> Send2FACodeAsync(string emailOrUsername, string password);
    Task<TokensInfo> SignInWithCodeAsync(string emailOrUsername, string password, string code);
    Task<bool> IsTwoFactorAuthEnabled(string emailOrUsername);
    Task<TokensInfo> GenerateAndSaveTokensAsync(UserEntity user);
}
