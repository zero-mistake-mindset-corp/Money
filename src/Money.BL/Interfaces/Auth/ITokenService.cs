using Money.BL.Models.Auth;

namespace Money.BL.Interfaces.Auth;

public interface ITokenService
{
    AccessTokenInfo GenerateAccessToken(Guid userId);
    string GenerateRefreshToken();
}
