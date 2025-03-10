using Money.BL.Models.Auth;

namespace Money.BL.Interfaces.Auth;

public interface IAuthService
{
    Task<TokensInfo> SignInAsync(SignInModel signInModel);
    Task<TokensInfo> RefreshTokensAsync(string refreshToken);
}
