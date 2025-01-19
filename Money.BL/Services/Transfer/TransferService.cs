using Money.BL.Interfaces.Transfer;
using Money.BL.Models.Transfer;
using Money.BL.Interfaces.MoneyAccount;
using Money.Data;
using Money.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Money.Data.Entities;
using Money.Common.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Money.BL.Services.Transfer;

public class TransferService : ITransferService
{
    private readonly ILogger<TransferService> _logger;
    private readonly AppDbContext _context;
    private readonly IMoneyAccountService _moneyAccountService;

    public TransferService(AppDbContext context, IMoneyAccountService moneyAccountService, ILogger<TransferService> logger)
    {
        _moneyAccountService = moneyAccountService;
        _context = context; 
        _logger = logger;
    }

    public async Task CreateTransferAsync(CreateTransferModel model, Guid userId)
    {
        ValidationHelper.ValidateMoneyValue(model.Amount);
        BaseValidator.ValidateString(model.Comment, maxLength: 250);

        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);

        if (model.SendingMoneyAccountId == model.RecievingMoneyAccountId)
        {
            throw new Exception("Sender account cannot be a reciever account");
        }

        var isRecievingMoneyAccountExist = await _context.MoneyAccounts.AnyAsync(ma => ma.UserId == user.Id && ma.Id == model.RecievingMoneyAccountId);
        if (isRecievingMoneyAccountExist == false)
        {
            throw new NotFoundException("Money account with this ID does not exist.");
        }

        var isSendingMoneyAccountExists = await _context.MoneyAccounts.AnyAsync(ma => ma.UserId == userId && ma.Id == model.SendingMoneyAccountId);
        if (isSendingMoneyAccountExists == false)
        {
            throw new NotFoundException("Money account with this ID does not exist.");
        }

        var newTransfer = new TransferEntity
        {
            Amount = model.Amount,
            TransferDate = model.TransferDate,
            Comment = model.Comment,
            SendingMoneyAccountId = model.SendingMoneyAccountId,
            RecievingMoneyAccountId = model.RecievingMoneyAccountId
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _moneyAccountService.UpdateBalanceAsync(model.SendingMoneyAccountId, userId, model.Amount, false);
            await _moneyAccountService.UpdateBalanceAsync(model.RecievingMoneyAccountId, userId, model.Amount, true);
            _context.Transfers.Add(newTransfer);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch(Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task<List<TransferModel>> GetAllTransfersAsync(Guid userId, int pageIndex, int pageSize)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);

        var transfers = await _context.Transfers.AsNoTracking()
            .Where(t => t.RecievingMoneyAccount.UserId == userId && t.SendingMoneyAccount.UserId == userId)
            .OrderByDescending(t => t.TrackingDate)
            .Skip((pageIndex-1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransferModel
            {
                Id = t.Id,
                Amount = t.Amount,
                TransferDate = t.TransferDate,
                TrackingDate = t.TrackingDate,
                Comment = t.Comment,
                SendingMoneyAccountId = t.SendingMoneyAccountId,
                RecievingMoneyAccountId = t.RecievingMoneyAccountId
            }).ToListAsync();

        return transfers;
    }

    public async Task UpdateTransferInfoAsync(UpdateTransferModel updModel, Guid userId, Guid transferId)
    {
        BaseValidator.ValidateString(updModel.Comment, maxLength: 250);
        ValidationHelper.ValidateMoneyValue(updModel.Amount);
        
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);

        if (updModel.SendingMoneyAccountId == updModel.RecievingMoneyAccountId)
        {
            throw new Exception("Sender account cannot be a reciever account");
        }

        var isRecievingMoneyAccountExist = await _context.MoneyAccounts
            .AnyAsync(ma => ma.UserId == user.Id && ma.Id == updModel.RecievingMoneyAccountId);
        if (isRecievingMoneyAccountExist == false)
        {
            throw new NotFoundException("Money account with this ID does not exist.");
        }

        var isSendingMoneyAccountExists = await _context.MoneyAccounts
            .AnyAsync(ma => ma.UserId == userId && ma.Id == updModel.SendingMoneyAccountId);
        if (isSendingMoneyAccountExists == false)
        {
            throw new NotFoundException("Money account with this ID does not exist.");
        }

        var transfer = await _context.Transfers.FirstOrDefaultAsync(t => t.Id == transferId);
        ValidationHelper.EnsureEntityFound(transfer);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (transfer.SendingMoneyAccountId != updModel.SendingMoneyAccountId)
            {   
                await _moneyAccountService
                    .UpdateBalanceAsync(transfer.SendingMoneyAccountId, userId, transfer.Amount, true);
                await _moneyAccountService
                    .UpdateBalanceAsync(updModel.SendingMoneyAccountId, userId, updModel.Amount, false);
                transfer.SendingMoneyAccountId = updModel.SendingMoneyAccountId;
            }
            
            if (transfer.RecievingMoneyAccountId != updModel.RecievingMoneyAccountId)
            {
                await _moneyAccountService
                    .UpdateBalanceAsync(transfer.RecievingMoneyAccountId, userId, transfer.Amount, false);
                await _moneyAccountService
                    .UpdateBalanceAsync(updModel.RecievingMoneyAccountId, userId, updModel.Amount, true);
                transfer.RecievingMoneyAccountId = updModel.RecievingMoneyAccountId;
            }

            transfer.Amount = updModel.Amount;
            transfer.TransferDate = updModel.TransferDate;
            transfer.TrackingDate = DateTime.UtcNow;
            transfer.Comment = updModel.Comment;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch(Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex.Message);
            throw;
        }
    }

    public async Task DeleteTransferAsync(Guid userId, Guid transferId)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);
        var transfer = await _context.Transfers.FirstOrDefaultAsync(t => t.Id == transferId);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _moneyAccountService.UpdateBalanceAsync(transfer.SendingMoneyAccountId, userId, transfer.Amount, true);
            await _moneyAccountService.UpdateBalanceAsync(transfer.RecievingMoneyAccountId, userId, transfer.Amount, false);
            
            await _context.Transfers.Where(t=> t.Id == transferId).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex.Message);
            throw;
        }
    
    }
}
