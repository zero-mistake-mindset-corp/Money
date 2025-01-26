using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.Transactions;
using Money.BL.Interfaces.User;
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
    public async Task<IActionResult> GetAllExpenseTransactions(int pageIndex = 1, int pageSize = 10)
    {
        var userId = _currentUserService.GetUserId();
        var expenseTransactions = await _expenseTransactionService.GetAllExpenseTransactionsAsync(userId, pageIndex, pageSize);
        return Ok(expenseTransactions);
    }

    [HttpGet("latest")]
    [Authorize]
    public async Task<IActionResult> GetLatestExpenseTransactions()
    {
        var userId = _currentUserService.GetUserId();
        var expenseTransactions = await _expenseTransactionService.Get10LastExpenseTransactionsAsync(userId);
        return Ok(expenseTransactions);
    }

    [HttpGet("{moneyAccountId}")]
    [Authorize]
    public async Task<IActionResult> GetExpenseTransactionsByAcc(Guid moneyAccountId, int pageIndex = 1, int pageSize = 10)
    {
        var userId = _currentUserService.GetUserId();
        var expenseTransactions = await _expenseTransactionService.GetExpenseTransactionsByAccAsync(userId, moneyAccountId, pageIndex, pageSize);
        return Ok(expenseTransactions);
    }

    [HttpPut("{expenseTransactionId}/comment")]
    [Authorize]
    public async Task<IActionResult> UpdateExpenseTransactionComment(Guid expenseTransactionId, string newComment)
    {
        var userId = _currentUserService.GetUserId();
        await _expenseTransactionService.UpdateExpenseTransactionCommentAsync(userId, expenseTransactionId, newComment);
        return Ok();
    }
}
