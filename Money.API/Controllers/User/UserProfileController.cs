using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.User;
using Money.BL.Models.UserAccount;

namespace Money.API.Controllers.User;

[Route("[controller]")]
[ApiController]
public class UserProfileController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserProfileService _profileService;

    public UserProfileController(ICurrentUserService currentUserService, IUserProfileService profileService)
    {
        _currentUserService = currentUserService;
        _profileService = profileService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUserProfile()
    {
        var userId = _currentUserService.GetUserId();
        var profile = await _profileService.GetUserProfile(userId);
        return Ok(profile);
    }

    [HttpPut("username")]
    [Authorize]
    public async Task<IActionResult> UpdateUsername(string username)
    {
        var userId = _currentUserService.GetUserId();
        await _profileService.UpdateUsernameAsync(userId, username);
        return Ok();
    }

    [HttpPut("2fa")]
    [Authorize]
    public async Task<IActionResult> Request2FaChange(bool enable)
    {
        var userId = _currentUserService.GetUserId();
        await _profileService.RequestTwoFactorAuthChangeAsync(userId, enable);
        return Created();
    }

    [HttpPut("2fa/confirmation")]
    [Authorize]
    public async Task<IActionResult> Request2FaConfirmation(string code)
    {
        var userId = _currentUserService.GetUserId();
        await _profileService.ConfirmTwoFactorAuthChangeAsync(userId, code);
        return Ok();
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordModel updatePasswordModel)
    {
        var userId = _currentUserService.GetUserId();
        await _profileService.ChangePasswordAsync(userId, updatePasswordModel.OldPassword, updatePasswordModel.NewPassword);
        return Ok();
    }

    [HttpPut("email-changing")]
    [Authorize]
    public async Task<IActionResult> UpdateEmail(string newEmail)
    {
        var userId = _currentUserService.GetUserId();
        await _profileService.RequestEmailChangingAsync(userId, newEmail);
        return Created();
    }

    [HttpPut("email-changing/confirmation")]
    [Authorize]
    public async Task<IActionResult> ConfirmEmailChanging(string code, string newEmail)
    {
        var userId = _currentUserService.GetUserId();
        await _profileService.ConfirmEmailChangingAsync(userId, code, newEmail);
        return Ok();
    }
}
