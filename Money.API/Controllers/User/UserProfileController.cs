using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.User;
using Money.BL.Services.User;

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
}
