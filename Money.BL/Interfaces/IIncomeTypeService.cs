using Money.BL.Models.Type;

namespace Money.BL.Interfaces;

public interface IIncomeTypeService
{
    Task<List<IncomeTypeModel>> GetAllIncomeTypesAsync(Guid userId);
    Task CreateIncomeTypeAsync(CreateIncomeTypeModel model, Guid userId);
    Task UpdateIncomeTypeAsync(Guid userId, Guid incomeTypeId, string newIncomeTypeName);
    Task DeleteIncomeTypeAsync(Guid userId, Guid incomeTypeId);
}
