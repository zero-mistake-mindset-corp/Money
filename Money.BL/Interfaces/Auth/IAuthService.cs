using Money.BL.Models.Auth;
using Money.Data.Entities;

namespace Money.BL.Interfaces.Auth;

public interface IAuthService
{
    Task<TokensInfo> SignInAsync(SignInModel signInModel);
    Task<TokensInfo> RefreshTokensAsync(string refreshToken);
    Task<string> Send2FACodeAsync(string email, string password);
    Task<TokensInfo> SignInWithCodeAsync(string email, string password, string code);
    Task<bool> IsTwoFactorAuthEnabled(string email);
    Task<TokensInfo> GenerateAndSaveTokensAsync(UserEntity user);
}
