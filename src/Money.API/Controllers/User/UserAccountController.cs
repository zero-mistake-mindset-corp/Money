using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.User;
using Money.BL.Models.Auth;
using Money.BL.Models.UserAccount;

namespace Money.API.Controllers.User;

[Route("account")]
[ApiController]
public class UserAccountController : ControllerBase
{
    private readonly IUserAccountService _userAccountSerivce;
    private readonly ICurrentUserService _currentUserService;

    public UserAccountController(IUserAccountService userSerivce, ICurrentUserService currentUserService)
    {
        _userAccountSerivce = userSerivce;
        _currentUserService = currentUserService;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp(SignUpModel signUpModel)
    {
        await _userAccountSerivce.SignUpAsync(signUpModel);
        return Created();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUserAccount()
    {
        var userId = _currentUserService.GetUserId();
        var profile = await _userAccountSerivce.GetUserAccountAsync(userId);
        return Ok(profile);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUsername(UpdateAccountModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _userAccountSerivce.UpdateAccountAsync(model, userId);
        return Ok();
    }
}
