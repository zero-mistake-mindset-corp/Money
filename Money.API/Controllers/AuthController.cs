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

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp(SignUpModel signUpModel)
    {
        await _authService.SignUpAsync(signUpModel);
        return Created();
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInModel signInModel)
    {
        var tokensInfo = await _authService.SignInAsync(signInModel);
        return Ok(tokensInfo);
    }

    [HttpPost("tokens")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var newTokensInfo = await _authService.RefreshTokensAsync(refreshToken);
        return Ok(newTokensInfo);
    }
}
