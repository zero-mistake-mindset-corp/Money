using Microsoft.EntityFrameworkCore;
using Money.BL.Interfaces;
using Money.BL.Models.Type;
using Money.Common.Helpers;
using Money.Data;
using Money.Data.Entities;

namespace Money.BL.Services;

public class ExpenseTypeService : IExpenseTypeService
{
    private readonly AppDbContext _context;

    public ExpenseTypeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateExpenseTypeAsync(CreateExpenseTypeModel model, Guid userId)
    {
        Validator.ValidateString(model.Name);
        bool isDuplicate = await _context.ExpenseTypes
            .AsNoTracking()
            .AnyAsync(et => et.Name == model.Name && et.UserId == userId);
        if (isDuplicate)
            throw new InvalidOperationException("Expense category with this name already exists");
        
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
        Validator.ValidateString(newExpenseTypeName);
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
