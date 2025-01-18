using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.Auth;
using Money.BL.Models.Auth;

namespace Money.API.Controllers.User;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGoogleAuthService _googleAuthService;

    public AuthController(IAuthService authService, IGoogleAuthService googleAuthService)
    {
        _authService = authService;
        _googleAuthService = googleAuthService;
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInModel signInModel)
    {
        var is2FAEnabled = await _authService.IsTwoFactorAuthEnabled(signInModel.EmailOrUsername);
        if (is2FAEnabled)
        {
            var notifyEmail = await _authService.Send2FACodeAsync(signInModel.EmailOrUsername, signInModel.Password);
            return Accepted(notifyEmail);
        }

        var tokensInfo = await _authService.SignInAsync(signInModel);
        return Ok(tokensInfo);
    }

    [HttpPost("sign-in/2fa")]
    public async Task<IActionResult> SignInWithCode(SignInWithCodeModel signInModel)
    {
        var tokensInfo = await _authService.SignInWithCodeAsync(signInModel.EmailOrUsername, signInModel.Password, signInModel.Code);
        return Ok(tokensInfo);
    }

    [HttpPost("tokens")]
    public async Task<IActionResult> Refresh(RefreshTokensModel refreshTokensModel)
    {
        var refreshedTokensInfo = await _authService.RefreshTokensAsync(refreshTokensModel.RefreshToken);
        return Ok(refreshedTokensInfo);
    }

    [HttpPost("sign-in/google")]
    public async Task<IActionResult> SignInWithGoogle(GoogleSignInModel googleSignInModel)
    {
        var tokensInfo = await _googleAuthService.GoogleSignIn(googleSignInModel);
        return Ok(tokensInfo);
    }
}
