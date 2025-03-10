using Money.BL.Models.Type;
using Money.Data;
using Money.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Money.Data.Entities;
using Money.BL.Interfaces.TransactionTypes;
using AutoMapper;
using Money.BL.Models.TransactionTypes;

namespace Money.BL.Services.TransactionTypes;

public class IncomeTypeService : IIncomeTypeService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public IncomeTypeService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task CreateIncomeTypeAsync(CreateIncomeTypeModel model, Guid userId)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        ValidationHelper.EnsureEntityFound(user);

        var newIncomeType = _mapper.Map<IncomeTypeEntity>(model);
        newIncomeType.UserId = user.Id;

        _context.IncomeTypes.Add(newIncomeType);
        await _context.SaveChangesAsync();
    }

    public async Task<List<IncomeTypeModel>> GetAllIncomeTypesAsync(Guid userId)
    {
        var incomeTypes = await _context.IncomeTypes
            .AsNoTracking()
            .Where(it => it.UserId == userId)
            .Select(it => _mapper.Map<IncomeTypeModel>(it))
            .ToListAsync();

        return incomeTypes;
    }

    public async Task UpdateIncomeTypeAsync(UpdateIncomeTypeModel model, Guid userId)
    {
        var incomeType = await _context.IncomeTypes.Where(it => it.Id == model.Id && it.UserId == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(incomeType);
        _mapper.Map(model, incomeType);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteIncomeTypeAsync(Guid userId, Guid incomeTypeId)
    {
        await _context.IncomeTypes.Where(it => it.Id == incomeTypeId && it.UserId == userId).ExecuteDeleteAsync();
    }
}
