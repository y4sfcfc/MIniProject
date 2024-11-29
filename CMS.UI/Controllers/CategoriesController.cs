using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IGenericRepository<Category> _categoryRepository;

        public CategoriesController(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(int? parentId, string searchCategory)
        {
            
            List<CategoryVM> models = new();
            if(searchCategory!=null)
            {
                var categories = await _categoryRepository.GetAll();
                List<Category> searchCategories = categories.Where(c => c.Name.ToLower() == searchCategory.ToLower()).ToList();
                foreach (var category in searchCategories)
                {
                    var parentCategory = await _categoryRepository.Get(category.ParentId);
                    models.Add(new()
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Level = category.Level,
                        ParentCategory=parentCategory.Name??"No Parent category"
                    });
                }
            }
            else if(parentId != null) 
            {
                var categories = await _categoryRepository.GetAll();
                foreach (var category in categories.Where(c => c.ParentId == parentId).ToList())
                {
                    models.Add(new CategoryVM
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Level = category.Level,
                    });
                }
            }
            return View(models);
        }
        
    }
}
