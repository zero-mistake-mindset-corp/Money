using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.TransactionTypes;
using Money.BL.Interfaces.User;
using Money.BL.Models.Type;

namespace Money.API.Controllers;

[Route("[controller]")]
[ApiController]
public class IncomeTypesController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIncomeTypeService _incomeTypeService;

    public IncomeTypesController(ICurrentUserService currentUserService, IIncomeTypeService incomeTypeService)
    {
        _currentUserService = currentUserService;
        _incomeTypeService = incomeTypeService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateIncomeType(CreateIncomeTypeModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _incomeTypeService.CreateIncomeTypeAsync(model, userId);
        return Created();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllIncomeTypes()
    {
        var userId = _currentUserService.GetUserId();
        var categories = await _incomeTypeService.GetAllIncomeTypesAsync(userId);
        return Ok(categories);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateIncomeType(Guid incomeTypeId, string newIncomeTypeName)
    {
        var userId = _currentUserService.GetUserId();
        await _incomeTypeService.UpdateIncomeTypeAsync(incomeTypeId, userId, newIncomeTypeName);
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteIncomeType(Guid incomeTypeId)
    {
        var userId = _currentUserService.GetUserId();
        await _incomeTypeService.DeleteIncomeTypeAsync(userId, incomeTypeId);
        return Ok();
    }
}
