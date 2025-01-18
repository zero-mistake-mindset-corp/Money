using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.User;
using Money.BL.Models.Auth;
using Money.BL.Models.UserAccount;

namespace Money.API.Controllers.User;

[Route("[controller]")]
[ApiController]
public class UserAccountController : ControllerBase
{
    private readonly IUserService _userSerivce;

    public UserAccountController(IUserService userSerivce)
    {
        _userSerivce = userSerivce;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp(SignUpModel signUpModel)
    {
        await _userSerivce.SignUpAsync(signUpModel);
        return Created();
    }

    [HttpPost("email/confirmation")]
    public async Task<IActionResult> ConfirmEmail(EmailConfirmationModel emailConfirmationModel)
    {
        await _userSerivce.ConfirmEmailAsync(emailConfirmationModel.Email, emailConfirmationModel.Code);
        return Ok();
    }

    [HttpPost("email/confirmation/code")]
    public async Task<IActionResult> ResendEmailConfirmationCode(string email)
    {
        await _userSerivce.ResendEmailConfirmationCodeAsync(email);
        return Ok();
    }
}
