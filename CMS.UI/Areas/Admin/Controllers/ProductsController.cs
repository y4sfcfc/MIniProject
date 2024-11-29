using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;
using System.IO;

namespace CMS.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class ProductsController : Controller
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Branch> _branchRepository;
        private readonly IGenericRepository<ProductImage> _productImageRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
       
		public ProductsController(IGenericRepository<Product> productRepository,
			IGenericRepository<Category> categoryRepository,
			IWebHostEnvironment webHostEnvironment,
			IGenericRepository<Branch> branchRepository,
			IGenericRepository<ProductImage> productImageRepository)
		{
			_productRepository = productRepository;
			_categoryRepository = categoryRepository;
			_webHostEnvironment = webHostEnvironment;
			_branchRepository = branchRepository;
			_productImageRepository = productImageRepository;
			
		}

		public async Task<IActionResult> Index()
        {
            List<ProductVM> models = new();
            var products = (await _productRepository.GetAll()).ToList();
            foreach (var product in products)
            {
                var category = await _categoryRepository.Get(product.CategoryId);
                models.Add(new()
                {
                    Id = product.Id,
                    Name = product.Name,
                    CategoryId = product.CategoryId,
                    CategoryName = category.Name,
                    Description = product.Description,
                    Count = product.Count,
                    Price = product.Price,
                    Brand = product.Brand,
                    Model = product.Model,
                    Thumbnail = product.Thumbnail,
                });
            }
            return View(models);
        }
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAll();
            var branchs = await _branchRepository.GetAll();
            ProductVM model = new()
            {
                Categories = categories.ToList(),
                Branchs = branchs.ToList(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductVM productModel)
        {

                List<ProductImage> productImages = new();
                foreach (var imageFile in productModel.ImageFiles)
                {
                    productImages = await UploadImage(imageFile, productImages);
                }
                Product product = new()
                {

                    Name = productModel.Name,
                    CategoryId = productModel.CategoryId,
                    BranchId = productModel.BranchId,
                    Description = productModel.Description,
                    Count = productModel.Count,
                    Price = productModel.Price,
                    Brand = productModel.Brand,
                    Model = productModel.Model,
                    UpdateDate = DateTime.Now,
                };
                product.ProductImages = productImages;
                product.Thumbnail = Path.Combine(productImages[0].Directory, productImages[0].Name);

                await _productRepository.Create(product);
                await _productRepository.SaveAsync();
                return RedirectToAction("Index");
            
        }
        public async Task<List<ProductImage>> UploadImage(IFormFile file, List<ProductImage> productImages)
        {

            string localPath = _webHostEnvironment.WebRootPath;
            string folderName = "productImages";
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(localPath, folderName, fileName);
            using FileStream fileStream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(fileStream);
            productImages.Add(new()
            {
                Name = fileName,
                Directory = folderName
            });
            return productImages;
        }
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.Get(id);
            var category = await _categoryRepository.Get(product.CategoryId);
            var branch = await _branchRepository.Get(product.BranchId);
            ProductVM model = new()
            {
                Id = product.Id,
                Name = product.Name,
                CategoryName = category.Name,
                BranchName = branch.Name,
                Description = product.Description,
                Count = product.Count,
                Price = product.Price,
                Brand = product.Brand,
                Model = product.Model,

            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(ProductVM productModel)
        {
            var product = await _productRepository.Get(productModel.Id,"ProductImages");
            if (product == null)
            {
                return NotFound();
            }
            foreach (var productImage in product.ProductImages)
            {
                DeleteImage(productImage.Name, productImage.Directory);
              _productImageRepository.Remove(productImage.Id); 
            }
            _productRepository.Remove(productModel.Id);
            _productRepository.Save();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.Get(id);
            var categories = (await _categoryRepository.GetAll()).ToList();
            var branchs = (await _branchRepository.GetAll()).ToList();
            ProductVM model = new()
            {
                Name = product.Name,
                CategoryId = product.CategoryId,
                BranchId = product.BranchId,
                Description = product.Description,
                Count = product.Count,
                Price = product.Price,
                Brand = product.Brand,
                Model = product.Model,
                Categories = categories,
                Branchs = branchs,

            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ProductVM productModel, int id)
        {

            if (ModelState.IsValid)
            {
                var product = await _productRepository.Get(id, "ProductImages");
                product.Id = productModel.Id;
                product.Name = productModel.Name;
                product.CategoryId = productModel.CategoryId;
                product.BranchId = productModel.BranchId;
                product.Description = productModel.Description;
                product.Count = productModel.Count;
                product.Price = productModel.Price;
                product.Brand = productModel.Brand;
                product.Model = productModel.Model;
                product.UpdateDate = DateTime.Now;
                if (productModel.ImageFiles != null)
                {
                    List<ProductImage> productImages = new();
                    foreach (var productImage in product.ProductImages)
                    {
                        DeleteImage(productImage.Name, productImage.Directory);
                        _productImageRepository.Remove(productImage.Id);
                        _productImageRepository.Save();
                    }
                    product.ProductImages.Clear();
                    foreach (var imageFile in productModel.ImageFiles)
                    {
                        productImages = await UploadImage(imageFile, productImages);
                    }
                    product.Thumbnail = Path.Combine(productImages[0].Directory, productImages[0].Name);
                    product.ProductImages = productImages;
                }
                _productRepository.Update(product);
                _productRepository.Save();
                return RedirectToAction("Index");
            }
            return View(productModel);
        }
        public void DeleteImage(string fileName, string folderName)
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
