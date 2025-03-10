using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.Transactions;
using Money.BL.Interfaces.User;
using Money.BL.Models.Pagination;
using Money.BL.Models.Transaction;

namespace Money.API.Controllers.Transactions;

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
    public async Task<IActionResult> GetAllIncomeTransactions(PaginatedDataRequest request)
    {
        var userId = _currentUserService.GetUserId();
        var incomeTransactions = await _incomeTransactionService.GetAllIncomeTransactionsAsync(userId, request);
        return Ok(incomeTransactions);
    }

    [HttpGet("{moneyAccountId}")]
    [Authorize]
    public async Task<IActionResult> GetIncomeTransactionsByAcc(Guid moneyAccountId, PaginatedDataRequest request)
    {
        var userId = _currentUserService.GetUserId();
        var incomeTransactions = await _incomeTransactionService.GetIncomeTransactionsByAccAsync(userId, moneyAccountId, request);
        return Ok(incomeTransactions);
    }

    [HttpPut("{incomeTransactionId}")]
    [Authorize]
    public async Task<IActionResult> UpdateIncomeTransactionComment(UpdateIncomeTransactionModel model)
    {
        var userId = _currentUserService.GetUserId();
        await _incomeTransactionService.UpdateIncomeTransactionAsync(userId, model);
        return Ok();
    }
}
