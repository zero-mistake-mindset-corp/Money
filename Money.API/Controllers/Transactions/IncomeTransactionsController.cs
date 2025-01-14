using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces;
using Money.BL.Models.Transaction;

namespace Money.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IncomeTransactionsController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIncomeTransactionService _incomeTransactionService;

    public IncomeTransactionsController(ICurrentUserService currentUserService, IIncomeTransactionService incomeTransactionService)
    {
        _currentUserService = currentUserService;
        _incomeTransactionService = incomeTransactionService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateIncomeTransaction(CreateIncomeTransactionModel createTransactionModel)
    {
        var userId = _currentUserService.GetUserId();
        await _incomeTransactionService.CreateIncomeTransactionAsync(createTransactionModel, userId);
        return Created();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllIncomeTransactions(int pageIndex = 1, int pageSize = 10)
    {
        var userId = _currentUserService.GetUserId();
        var incomeTransactions = await _incomeTransactionService.GetAllIncomeTransactionsAsync(userId, pageIndex, pageSize);
        return Ok(incomeTransactions);
    }

    [HttpGet("latest")]
    [Authorize]
    public async Task<IActionResult> GetLatestTransactions()
    {
        var userId = _currentUserService.GetUserId();
        var incomeTransactions = await _incomeTransactionService.Get10LastIncomeTransactionsAsync(userId);
        return Ok(incomeTransactions);
    }

    [HttpGet("{accountId}")]
    [Authorize]
    public async Task<IActionResult> GetIncomeTransactionsByAcc(Guid accountId, int pageIndex = 1, int pageSize = 10)
    {
        var userId = _currentUserService.GetUserId();
        var incomeTransactions = await _incomeTransactionService.GetIncomeTransactionsByAccAsync(userId, accountId, pageIndex, pageSize);
        return Ok(incomeTransactions);
    }
}
