using Money.BL.Models.Type;
using Money.Data;
using Money.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Money.Data.Entities;
using Money.BL.Interfaces;

namespace Money.BL.Services;

public class IncomeTypeService : IIncomeTypeService
{
    private readonly AppDbContext _context;

    public IncomeTypeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateIncomeTypeAsync(CreateIncomeTypeModel model, Guid userId)
    {
        Validator.ValidateString(model.Name);
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);
        var newIncomeType = new IncomeTypeEntity
        {
            Name = model.Name,
            UserId = userId
        };

        await _context.IncomeTypes.AddAsync(newIncomeType);
        await _context.SaveChangesAsync();
    }

    public async Task<List<IncomeTypeModel>> GetAllIncomeTypesAsync(Guid userId)
    {
        var incomeTypes = await _context.IncomeTypes.AsNoTracking().Where(it => it.UserId == userId)
            .Select(it => new IncomeTypeModel
            {
                Id = it.Id,
                Name = it.Name
            }).ToListAsync();

        return incomeTypes;
    }

    public async Task UpdateIncomeTypeAsync(Guid incomeTypeId, Guid userId, string newIncomeTypeName)
    {
        Validator.ValidateString(newIncomeTypeName);
        var incomeType = await _context.IncomeTypes.Where(it => it.Id == incomeTypeId && it.UserId == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(incomeType);
        incomeType.Name = newIncomeTypeName;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteIncomeTypeAsync(Guid userId, Guid incomeTypeId)
    {
        await _context.IncomeTypes.Where(it => it.Id == incomeTypeId && it.UserId == userId).ExecuteDeleteAsync();
    }
}
