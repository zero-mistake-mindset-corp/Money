using Money.BL.Interfaces.Transfers;
using Money.BL.Models.Transfer;
using Money.BL.Interfaces.MoneyAccount;
using Money.Data;
using Money.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Money.Data.Entities;
using Money.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace Money.BL.Services.Transfers;

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
        BaseValidator.ValidateString(model.Name, maxLength: 100);

        if (model.SendingMoneyAccountId == model.ReceivingMoneyAccountId)
        {
            throw new InvalidInputException("Sender account cannot be a reciever account");
        }

        var user = await _context.Users.Include(u => u.MoneyAccounts).Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var areAccountsExist = user.MoneyAccounts.Where(ma => ma.Id == model.ReceivingMoneyAccountId || ma.Id == model.SendingMoneyAccountId);
        if (areAccountsExist.Count() < 2)
        {
            throw new NotFoundException("Money accounts with this ID are not exist.");
        }

        var newTransfer = new TransferEntity
        {
            Name = model.Name,
            Comment = model.Comment,
            Amount = model.Amount,
            TransferDate = model.TransferDate,
            SendingMoneyAccountId = model.SendingMoneyAccountId,
            ReceivingMoneyAccountId = model.ReceivingMoneyAccountId,
            UserId = userId
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _moneyAccountService.UpdateBalanceAsync(model.SendingMoneyAccountId, userId, model.Amount, false);
            await _moneyAccountService.UpdateBalanceAsync(model.ReceivingMoneyAccountId, userId, model.Amount, true);
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
        var transfers = await _context.Transfers.AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.TrackingDate)
            .Skip((pageIndex-1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransferModel
            {
                Id = t.Id,
                Name = t.Name,
                Comment = t.Comment,
                Amount = t.Amount,
                TransferDate = t.TransferDate,
                TrackingDate = t.TrackingDate,
                SendingMoneyAccountId = t.SendingMoneyAccountId,
                ReceivingMoneyAccountId = t.ReceivingMoneyAccountId
            }).ToListAsync();

        return transfers;
    }

    public async Task UpdateTransferInfoAsync(UpdateTransferModel updateModel, Guid userId, Guid transferId)
    { 
        BaseValidator.ValidateString(updateModel.Comment, maxLength: 250);
        BaseValidator.ValidateString(updateModel.Name, maxLength: 100);
        ValidationHelper.ValidateMoneyValue(updateModel.Amount);

        if (updateModel.SendingMoneyAccountId == updateModel.ReceivingMoneyAccountId)
        {
            throw new InvalidInputException("Sender account cannot be a receiver account.");
        }
        var transfer = await _context.Transfers
            .Include(t => t.SendingMoneyAccount)
            .Include(t => t.ReceivingMoneyAccount)
            .Where(t => t.UserId == userId)
            .FirstOrDefaultAsync(t => t.Id == transferId);

        ValidationHelper.EnsureEntityFound(transfer);

        var sendingAccount = await _context.MoneyAccounts.FirstOrDefaultAsync(ma => ma.Id == updateModel.SendingMoneyAccountId && ma.UserId == userId);
        var receivingAccount = await _context.MoneyAccounts.FirstOrDefaultAsync(ma => ma.Id == updateModel.ReceivingMoneyAccountId && ma.UserId == userId);

        if (sendingAccount == null || receivingAccount == null)
        {
            throw new NotFoundException("One or both money accounts do not exist or do not belong to the user.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (transfer.SendingMoneyAccountId != updateModel.SendingMoneyAccountId)
            {
                await _moneyAccountService.UpdateBalanceAsync(transfer.SendingMoneyAccountId.Value, userId, transfer.Amount, isDeposit: true);
                await _moneyAccountService.UpdateBalanceAsync(updateModel.SendingMoneyAccountId, userId, updateModel.Amount, isDeposit: false);
                transfer.SendingMoneyAccountId = updateModel.SendingMoneyAccountId;
            }
            else
            {
                var amountDiff = updateModel.Amount - transfer.Amount;
                if (amountDiff != 0)
                {
                    var isRevert = amountDiff < 0;
                    await _moneyAccountService.UpdateBalanceAsync(transfer.SendingMoneyAccountId.Value, userId, Math.Abs(amountDiff), isRevert);
                }
            }

            if (transfer.ReceivingMoneyAccountId != updateModel.ReceivingMoneyAccountId)
            {
                await _moneyAccountService.UpdateBalanceAsync(transfer.ReceivingMoneyAccountId.Value, userId, transfer.Amount, isDeposit: false);
                await _moneyAccountService.UpdateBalanceAsync(updateModel.ReceivingMoneyAccountId, userId, updateModel.Amount, isDeposit: true);
                transfer.ReceivingMoneyAccountId = updateModel.ReceivingMoneyAccountId;
            }
            else
            {
                var amountDiff = updateModel.Amount - transfer.Amount;
                if (amountDiff != 0)
                {
                    var isRevert = amountDiff > 0 ? true : false;
                    await _moneyAccountService.UpdateBalanceAsync(transfer.ReceivingMoneyAccountId.Value, userId, Math.Abs(amountDiff), isRevert);
                }
            }

            transfer.Name = updateModel.Name;
            transfer.Comment = updateModel.Comment;
            transfer.Amount = updateModel.Amount;
            transfer.TransferDate = updateModel.TransferDate;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating transfer info");
            throw;
        }
    }

    public async Task DeleteTransferAsync(Guid userId, Guid transferId)
    {
        var transfer = await _context.Transfers.Where(t => t.UserId == userId).FirstOrDefaultAsync(t => t.Id == transferId);
        ValidationHelper.EnsureEntityFound(transfer);

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (transfer.SendingMoneyAccountId.HasValue)
            {
                await _moneyAccountService.UpdateBalanceAsync(transfer.SendingMoneyAccountId.Value, userId, transfer.Amount, isDeposit: false);
            }

            if (transfer.ReceivingMoneyAccountId.HasValue)
            {
                await _moneyAccountService.UpdateBalanceAsync(transfer.ReceivingMoneyAccountId.Value, userId, transfer.Amount, isDeposit: true);
            }

            _context.Transfers.Remove(transfer);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting transfer");
            throw;
        }
    }
}
