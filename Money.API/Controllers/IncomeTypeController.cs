using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces;
using Money.BL.Models.Type;

namespace Money.API.Controllers;

[Route("[controller]")]
[ApiController]
public class IncomeTypeController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIncomeTypeService _incomeTypeService;

    public IncomeTypeController(ICurrentUserService currentUserService, IIncomeTypeService incomeTypeService)
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
    public async Task<IActionResult> UpdateIncomeType(Guid categoryId, string newCategoryName)
    {
        var userId = _currentUserService.GetUserId();
        await _incomeTypeService.UpdateIncomeTypeAsync(categoryId, userId, newCategoryName);
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteIncomeType(Guid categoryId)
    {
        var userId = _currentUserService.GetUserId();
        await _incomeTypeService.DeleteIncomeTypeAsync(categoryId, userId);
        return Ok();
    }
}
