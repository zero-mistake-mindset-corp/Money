using Money.BL.Models.Auth;
using Money.Data.Entities;

namespace Money.BL.Interfaces.Auth;

public interface IAuthService
{
    Task<TokensInfo> SignInAsync(SignInModel signInModel);
    Task SignUpAsync(SignUpModel signUpModel);
    Task<TokensInfo> RefreshTokensAsync(string refreshToken);
    Task<TokensInfo> GenerateAndSaveTokensAsync(UserEntity user);
}
