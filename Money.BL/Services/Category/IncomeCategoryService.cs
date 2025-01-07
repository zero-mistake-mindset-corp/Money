using Money.BL.Interfaces.Category;
using Money.BL.Models.Category;
using Money.Data;
using Money.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Money.Data.Entities;

namespace Money.BL.Services.Category
{
    public class IncomeCategoryService : IIncomeCategoryService
    {
        //
        private readonly AppDbContext _context;
        public IncomeCategoryService(AppDbContext context)
        {
            _context = context;
        }



        public async Task CreateIncomeCategoryAsync(CreateIncomeCategoryModel model, Guid userId)
        {
            Validator.ValidateString(model.CategoryName);
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId); 
            ValidationHelper.EnsureEntityFound(user);                                                

            var newIncomeCategory = new IncomeCategoryEntity
            {
                CategoryName = model.CategoryName,
                UserId = userId
            };

            await _context.IncomeCategories.AddAsync(newIncomeCategory);
            await _context.SaveChangesAsync();
        }


        public async Task<List<IncomeCategoryModel>> GetAllIncomeCategoriesAsync(Guid userId)
        {
            var categories = await _context.IncomeCategories.AsNoTracking().Where(cat => cat.UserId == userId)
                .Select(cat => new IncomeCategoryModel
                {
                    Id = cat.Id,
                    CategoryName = cat.CategoryName
                }).ToListAsync();

            return categories;
        }



        public async Task UpdateIncomeCategoryAsync(Guid categoryId, Guid userId,  string newCategoryName)
        {
            Validator.ValidateString(newCategoryName);

            var category = await _context.IncomeCategories.Where(cat => cat.Id == categoryId && cat.UserId == userId).FirstOrDefaultAsync();
            ValidationHelper.EnsureEntityFound(category);
            category.CategoryName = newCategoryName;

            await _context.SaveChangesAsync();
        }



        public async Task DeleteIncomeCategoryAsync(Guid userId, Guid categoryId)
        {
            await _context.IncomeCategories.Where(cat => cat.Id == categoryId && cat.UserId == userId).ExecuteDeleteAsync();
        }

    }
}