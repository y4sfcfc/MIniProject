using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly IGenericRepository<Category> _categoryRepository;

        public CategoryViewComponent(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await GetItemsAsync();
            List<CategoryVM> models = new();
            foreach (var category in categories)
            {

                var subCategories =  categories.Where(c=>c.ParentId == category.Id).ToList();
                models.Add(new()
                {
                    Id=category.Id,
                    Name = category.Name,
                    Level = category.Level,
                    ParentId = category.ParentId,
                    Categories = subCategories
                });
            }
            return View(models);
        }

        private async Task<List<Category>> GetItemsAsync()
        {
            var categories = await _categoryRepository.GetAll();
            return categories.ToList();
        }
    }
}
