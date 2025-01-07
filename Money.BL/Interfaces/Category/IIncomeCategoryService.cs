using Money.BL.Models.Category;

namespace Money.BL.Interfaces.Category
{
    public interface IIncomeCategoryService
    {
        Task<List<IncomeCategoryModel>> GetAllIncomeCategoriesAsync(Guid userId);
        Task CreateIncomeCategoryAsync(CreateIncomeCategoryModel model, Guid userId);
        
        Task UpdateIncomeCategoryAsync(Guid userId, Guid categoryId, string newCategoryName);
        Task DeleteIncomeCategoryAsync(Guid userId, Guid categoryId);
    }
}