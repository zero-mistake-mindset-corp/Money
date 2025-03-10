using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.MoneyAccount;
using Money.BL.Interfaces.User;
using Money.BL.Models.MoneyAccount;

namespace Money.API.Controllers.MoneyAccount;

[Route("[controller]")]
[ApiController]
public class MoneyAccountsController : ControllerBase
{
    private readonly IMoneyAccountService _moneyAccountService;
    private readonly ICurrentUserService _currentUserService;

    public MoneyAccountsController(IMoneyAccountService moneyAccountService, ICurrentUserService currentUserService)
    {
        _moneyAccountService = moneyAccountService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateMoneyAccount(CreateMoneyAccountModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _moneyAccountService.CreateAccountAsync(model, userId);
        return Created();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllAccounts()
    {
        var userId = _currentUserService.GetUserId();
        var accounts = await _moneyAccountService.GetAllAccountsAsync(userId);
        return Ok(accounts);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateAccount(UpdateMoneyAccountModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _moneyAccountService.UpdateAccountNameAsync(model, userId);
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteAccount(Guid accountId)
    {
        var userId = _currentUserService.GetUserId();
        await _moneyAccountService.DeleteAccountAsync(accountId, userId);
        return Ok();
    }
}
