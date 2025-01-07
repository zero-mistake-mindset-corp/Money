using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Money.BL.Interfaces;
using Money.BL.Interfaces.Category;
using Money.BL.Models.Category;

namespace Money.API.Controllers.Category
{
    [Route("[controller]")]
    [ApiController]
    public class IncomeCategoryController : ControllerBase 
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIncomeCategoryService _incomeCategoryService;

        public IncomeCategoryController(ICurrentUserService currentUserService, IIncomeCategoryService incomeCategoryService)
        {
            _currentUserService = currentUserService;
            _incomeCategoryService = incomeCategoryService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateIncomeCategory(CreateIncomeCategoryModel model)
        {
            var userId = _currentUserService.GetUserId();
            await _incomeCategoryService.CreateIncomeCategoryAsync(model, userId);
            return Created();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllIncomeCategories()
        {
            var userId = _currentUserService.GetUserId();
            var categories = await _incomeCategoryService.GetAllIncomeCategoriesAsync(userId);
            return Ok(categories);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateIncomeCategory(Guid categoryId, string newCategoryName)
        {
            var userId = _currentUserService.GetUserId();
            await _incomeCategoryService.UpdateIncomeCategoryAsync(categoryId, userId, newCategoryName);
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteIncomeCategory(Guid categoryId)
        {
            var userId = _currentUserService.GetUserId();
            await _incomeCategoryService.DeleteIncomeCategoryAsync(categoryId, userId);
            return Ok();
        }
    }
}