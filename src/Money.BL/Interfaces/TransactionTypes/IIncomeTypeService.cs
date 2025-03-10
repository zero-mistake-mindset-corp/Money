using Money.BL.Models.TransactionTypes;
using Money.BL.Models.Type;

namespace Money.BL.Interfaces.TransactionTypes;

public interface IIncomeTypeService
{
    Task<List<IncomeTypeModel>> GetAllIncomeTypesAsync(Guid userId);
    Task CreateIncomeTypeAsync(CreateIncomeTypeModel model, Guid userId);
    Task UpdateIncomeTypeAsync(UpdateIncomeTypeModel model, Guid userId);
    Task DeleteIncomeTypeAsync(Guid userId, Guid incomeTypeId);
}
