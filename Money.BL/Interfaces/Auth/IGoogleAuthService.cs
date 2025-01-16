using Money.BL.Models.Auth;

namespace Money.BL.Interfaces.Auth;

public interface IGoogleAuthService
{
    Task<TokensInfo> GoogleSignIn(GoogleSignInModel signInModel);
}
