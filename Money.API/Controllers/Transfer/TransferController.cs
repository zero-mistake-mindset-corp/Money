using Google.Apis.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces.Transfer;
using Money.BL.Interfaces.User;
using Money.BL.Models.Transfer;

namespace Money.API.Controllers.Transfer;

[ApiController]
[Route("[controller]")]
public class TransferController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ITransferService _transferService;

    public TransferController(ICurrentUserService currentUserService, ITransferService transferService)
    {
        _currentUserService = currentUserService;
        _transferService = transferService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTransfer(CreateTransferModel model)
    {     
        var userId = _currentUserService.GetUserId();
        await _transferService.CreateTransferAsync(model, userId);
        return Created();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllTransfers(int pageIndex, int pageSize)
    {
        var userId = _currentUserService.GetUserId();
        var transfers = await _transferService.GetAllTransfersAsync(userId, pageIndex, pageSize);
        return Ok(transfers);
    }

    [HttpPut("{transferId}")]
    [Authorize]
    public async Task<IActionResult> UpdateTransfer(UpdateTransferModel model, Guid transferId)
    {
        var userId = _currentUserService.GetUserId();
        await _transferService.UpdateTransferInfoAsync(model, userId, transferId);
        return Ok();
    }

    [HttpDelete("{transferId}")]
    [Authorize]
    public async Task<IActionResult> DeleteTransfer(Guid transferId)
    {
        var userId = _currentUserService.GetUserId();
        await _transferService.DeleteTransferAsync(userId, transferId);
        return Ok();
    }

}