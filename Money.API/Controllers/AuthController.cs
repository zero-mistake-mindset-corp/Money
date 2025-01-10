using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.Auth;
using Money.BL.Models.Auth;

namespace Money.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInModel signInModel)
    {
        var is2FAEnabled = await _authService.IsTwoFactorAuthEnabled(signInModel.Email);
        if (is2FAEnabled)
        {
            var notifyEmail = await _authService.Send2FACodeAsync(signInModel.Email, signInModel.Password);
            return Accepted(notifyEmail);
        }

        var tokensInfo = await _authService.SignInAsync(signInModel);
        return Ok(tokensInfo);
    }

    [HttpPost("sign-in/2fa")]
    public async Task<IActionResult> SignInWithCode(SignInWithCodeModel signInModel)
    {
        var tokensInfo = await _authService.SignInWithCodeAsync(signInModel.Email, signInModel.Password, signInModel.Code);
        return Ok(tokensInfo);
    }

    [HttpPost("tokens")]
    public async Task<IActionResult> Refresh(RefreshTokensModel refreshTokensModel)
    {
        var refreshedTokensInfo = await _authService.RefreshTokensAsync(refreshTokensModel.RefreshToken);
        return Ok(refreshedTokensInfo);
    }
}
