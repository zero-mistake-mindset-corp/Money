using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces;
using Money.BL.Models.Transaction;

namespace Money.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IncomeTransactionController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIncomeTransactionService _incomeTransactionService;

    public IncomeTransactionController(ICurrentUserService currentUserService, IIncomeTransactionService incomeTransactionService)
    {
        _currentUserService = currentUserService;
        _incomeTransactionService = incomeTransactionService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateIncomeTransaction(CreateIncomeTransactionModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _incomeTransactionService.CreateIncomeTransactionAsync(model, userId);
        return Created();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllIncomeTransactions(int pageNumber, int pageSize)
    {
        var userId = _currentUserService.GetUserId();
        var incomeTransactions = await _incomeTransactionService.GetAllIncomeTransactionsAsync(userId, pageNumber, pageSize);
        return Ok(incomeTransactions);
    }

    [HttpGet("latest")]
    [Authorize]
    public async Task<IActionResult> GetLatestTransactions()
    {
        var userId = _currentUserService.GetUserId();
        var incomeTransactions = await _incomeTransactionService.GetLatestIncomeTransactionsAsync(userId);
        return Ok(incomeTransactions);
    }

    [HttpGet("byacc")]
    [Authorize]
    public async Task<IActionResult> GetIncomeTransactionsByAcc(int pageNumber, int pageSize)
    {
        var userId = _currentUserService.GetUserId();
        var incomeTransactions = await _incomeTransactionService.GetIncomeTransactionsByAccAsync(userId, pageNumber, pageSize);
        return Ok(incomeTransactions);
    }
}
