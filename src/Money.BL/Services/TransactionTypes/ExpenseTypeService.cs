using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Money.BL.Interfaces.TransactionTypes;
using Money.BL.Models.TransactionTypes;
using Money.BL.Models.Type;
using Money.Common.Helpers;
using Money.Data;
using Money.Data.Entities;

namespace Money.BL.Services.TransactionTypes;

public class ExpenseTypeService : IExpenseTypeService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ExpenseTypeService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task CreateExpenseTypeAsync(CreateExpenseTypeModel model, Guid userId)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);

        var newExpenseType = _mapper.Map<ExpenseTypeEntity>(model);
        newExpenseType.UserId = userId;

        _context.ExpenseTypes.Add(newExpenseType);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ExpenseTypeModel>> GetAllExpenseTypesAsync(Guid userId)
    {
        var expenseTypes = await _context.ExpenseTypes
            .AsNoTracking()
            .Where(et => et.UserId == userId)
            .Select(et => _mapper.Map<ExpenseTypeModel>(et))
            .ToListAsync();

        return expenseTypes;
    }

    public async Task UpdateExpenseTypeAsync(Guid userId, UpdateExpenseTypeModel model)
    {
        var expenseType = await _context.ExpenseTypes.Where(et => et.Id == model.Id && et.UserId == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(expenseType);

        _mapper.Map(model, expenseType);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpenseTypeAsync(Guid userId, Guid expenseTypeId)
    {
        await _context.ExpenseTypes.Where(et => et.Id == expenseTypeId && et.UserId == userId).ExecuteDeleteAsync();
    }
}
