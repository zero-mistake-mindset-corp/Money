using Money.BL.Models.Category;

namespace Money.BL.Interfaces;

public interface IIncomeTypeService
{
    Task<List<IncomeTypeModel>> GetAllIncomeCategoriesAsync(Guid userId);
    Task CreateIncomeCategoryAsync(CreateIncomeTypeModel model, Guid userId);
    Task UpdateIncomeCategoryAsync(Guid userId, Guid incomeTypeId, string newIncomeTypeName);
    Task DeleteIncomeCategoryAsync(Guid userId, Guid incomeTypeId);
}
