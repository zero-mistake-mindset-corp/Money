using Microsoft.EntityFrameworkCore;
using Money.BL.Interfaces.TransactionTypes;
using Money.BL.Models.Type;
using Money.Common.Helpers;
using Money.Data;
using Money.Data.Entities;

namespace Money.BL.Services.TransactionTypes;

public class ExpenseTypeService : IExpenseTypeService
{
    private readonly AppDbContext _context;

    public ExpenseTypeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateExpenseTypeAsync(CreateExpenseTypeModel model, Guid userId)
    {
        BaseValidator.ValidateString(model.Name);
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);

        var newExpenseType = new ExpenseTypeEntity
        {
            Name = model.Name,
            UserId = userId
        };
       
        await _context.ExpenseTypes.AddAsync(newExpenseType);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ExpenseTypeModel>> GetAllExpenseTypesAsync(Guid userId)
    {
        var expenseTypes = await _context.ExpenseTypes.AsNoTracking().Where(et => et.UserId == userId)
            .Select(et => new ExpenseTypeModel
            {
                Id = et.Id,
                Name = et.Name
            }).ToListAsync();

        return expenseTypes;
    }

    public async Task UpdateExpenseTypeAsync(Guid userId, Guid expenseTypeId, string newExpenseTypeName)
    {
        BaseValidator.ValidateString(newExpenseTypeName);
        var expenseType = await _context.ExpenseTypes.Where(et => et.Id == expenseTypeId && et.UserId == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(expenseType);
        expenseType.Name = newExpenseTypeName;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpenseTypeAsync(Guid userId, Guid expenseTypeId)
    {
        await _context.ExpenseTypes.Where(et => et.Id == expenseTypeId && et.UserId == userId).ExecuteDeleteAsync();
    }
}
