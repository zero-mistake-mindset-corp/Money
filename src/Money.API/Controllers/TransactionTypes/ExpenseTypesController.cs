using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.TransactionTypes;
using Money.BL.Interfaces.User;
using Money.BL.Models.TransactionTypes;
using Money.BL.Models.Type;

namespace Money.API.Controllers.TransactionTypes;

[Route("[controller]")]
[ApiController]
public class ExpenseTypesController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IExpenseTypeService _expenseTypeService;

    public ExpenseTypesController (ICurrentUserService currentUserService, IExpenseTypeService expenseTypeService)
    {
        _currentUserService = currentUserService;
        _expenseTypeService = expenseTypeService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateExpenseType(CreateExpenseTypeModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _expenseTypeService.CreateExpenseTypeAsync(model, userId);
        return Created();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllExpenseTypes()
    {
        var userId = _currentUserService.GetUserId();
        var expenseTypes = await _expenseTypeService.GetAllExpenseTypesAsync(userId);
        return Ok(expenseTypes);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateExpenseType(UpdateExpenseTypeModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _expenseTypeService.UpdateExpenseTypeAsync(userId, model);
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteExpenseType(Guid expenseTypeId)
    {
        var userId = _currentUserService.GetUserId();
        await _expenseTypeService.DeleteExpenseTypeAsync(userId, expenseTypeId);
        return Ok();
    }
}
