using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.Transactions;
using Money.BL.Interfaces.User;
using Money.BL.Models.Pagination;
using Money.BL.Models.Transaction;

namespace Money.API.Controllers.Transactions;

[ApiController]
[Route("[controller]")]
public class ExpenseTransactionsController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IExpenseTransactionService _expenseTransactionService;

    public ExpenseTransactionsController (ICurrentUserService currentUserService, IExpenseTransactionService expenseTransactionService)
    {
        _currentUserService = currentUserService;
        _expenseTransactionService = expenseTransactionService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateExpenseTransaction(CreateExpenseTransactionModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _expenseTransactionService.CreateExpenseTransactionAsync(model, userId);
        return Created();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllExpenseTransactions(PaginatedDataRequest request)
    {
        var userId = _currentUserService.GetUserId();
        var expenseTransactions = await _expenseTransactionService.GetAllExpenseTransactionsAsync(userId, request);
        return Ok(expenseTransactions);
    }

    [HttpGet("{moneyAccountId}")]
    [Authorize]
    public async Task<IActionResult> GetExpenseTransactionsByAcc(Guid moneyAccountId, PaginatedDataRequest request)
    {
        var userId = _currentUserService.GetUserId();
        var expenseTransactions = await _expenseTransactionService.GetExpenseTransactionsByAccAsync(userId, moneyAccountId, request);
        return Ok(expenseTransactions);
    }

    [HttpPut("{expenseTransactionId}")]
    [Authorize]
    public async Task<IActionResult> UpdateExpenseTransaction(UpdateExpenseTransactionModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _expenseTransactionService.UpdateExpenseTransactionAsync(userId, model);
        return Ok();
    }
}
