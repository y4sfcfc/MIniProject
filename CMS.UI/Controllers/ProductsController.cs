using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Controllers
{
	public class ProductsController : Controller
	{
		private readonly IGenericRepository<Product> _productRepository;
		private readonly IGenericRepository<Category> _categoryRepository;
		private readonly IGenericRepository<Branch> _branchRepository;
		private readonly IGenericRepository<Merchant> _merchantRepository;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductsController(IGenericRepository<Product> productRepository,
			IGenericRepository<Category> categoryRepository,
			IWebHostEnvironment webHostEnvironment,
			IGenericRepository<Branch> branchRepository,
			IGenericRepository<Merchant> merchantRepository)
		{
			_productRepository = productRepository;
			_categoryRepository = categoryRepository;
			_webHostEnvironment = webHostEnvironment;
			_branchRepository = branchRepository;
			_merchantRepository = merchantRepository;
		}

		public async Task<IActionResult> Index(int? categoryId, int? merchantId,int branchId)
		{
			List<ProductVM> models = new();
			var products = (await _productRepository.GetAll()).ToList();
			var productCategory = products.Where(p => p.CategoryId == categoryId).ToList();
			var branchProducts = products.Where(p => p.BranchId == branchId).ToList();
			var productMerchant = new List<ProductVM>();
			if (merchantId != null)
			{
				foreach (var product in products)
				{
					var branch = await _branchRepository.Get(product.BranchId);
					if (branch.MerchantId == merchantId)
					{
						models.Add(new()
						{
							Id = product.Id,
							Name = product.Name,
							Thumbnail = product.Thumbnail,
						});
					}
				}
			}
			else if (categoryId != null)
			{
				foreach (var product in productCategory)
				{
					var category = await _categoryRepository.Get(product.Id);
					models.Add(new()
					{
						Id = product.Id,
						Name = product.Name,
						Thumbnail = product.Thumbnail,
					});
				}
			}
			else if (branchId != 0)
			{
				foreach (var product in branchProducts)
				{
					var category = await _categoryRepository.Get(product.Id);
					models.Add(new()
					{
						Id = product.Id,
						Name = product.Name,
						Thumbnail = product.Thumbnail,
					});
				}
			}
			else if (categoryId == null && merchantId == null&&branchId==0)
			{
				foreach (var product in products)
				{
					models.Add(new()
					{
						Id = product.Id,
						Name = product.Name,
						Thumbnail = product.Thumbnail,
					});
				}
			}
			return View(models);
		}
		public async Task<IActionResult> Details(int Id)
		{
			var product = await _productRepository.Get(Id, "ProductImages");
			var category = await _categoryRepository.Get(product.CategoryId);
			var branch = await _branchRepository.Get(product.BranchId);
			List<ProductVM> relatedProducts= new();
			var products=await _productRepository.GetAll();
			var relatedProductList = products.Where(rp => rp.CategoryId == product.CategoryId || rp.Brand == product.Brand).ToList();

			foreach (var item in relatedProductList)
            {
				if (item.Id != Id)
				{
					relatedProducts.Add(new()
					{
						Id = item.Id,
						Name = item.Name,
						Thumbnail = item.Thumbnail,
					});
				}
			
            }
            ProductVM model = new()
			{
				Id = product.Id,
				Name = product.Name,
				CategoryId = product.CategoryId,
				CategoryName = category.Name,
				BranchName = branch.Name,
				Description = product.Description,
				Count = product.Count,
				Price = product.Price,
				Brand = product.Brand,
				Model = product.Model,
				Thumbnail = product.Thumbnail,
				ProductImages = product.ProductImages.ToList(),
				RelatedProducts=relatedProducts
			};
			return View(model);
		}
		public async Task<IActionResult> Create()
		{
			var categories = _categoryRepository.GetAll().Result.ToList();
			ProductVM model = new()
			{
				Categories = categories,
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(ProductVM model)
		{
			List<ProductImage> productImages = new();
			foreach (var imageFile in model.ImageFiles)
			{
				productImages = UploadImage(imageFile);
			}
			Product product = new()
			{
				Id = model.Id,
				Name = model.Name,
				CategoryId = model.CategoryId,
				Description = model.Description,
				Count = model.Count,
				Price = model.Price,
				Brand = model.Brand,
				Model = model.Model,

			};
			product.ProductImages = productImages;
			product.Thumbnail = Path.Combine(productImages[0].Directory, productImages[0].Name);

			await _productRepository.Create(product);
			await _productRepository.SaveAsync();
			return RedirectToAction("Index");

		}
		public List<ProductImage> UploadImage(IFormFile file)
		{
			List<ProductImage> productImages = new();
			string localPath = _webHostEnvironment.WebRootPath;
			string folderName = "productImages";
			string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
			string filePath = Path.Combine(localPath, folderName, fileName);
			using (var localFile = System.IO.File.OpenWrite(filePath))
			using (var uploadedFile = file.OpenReadStream())
			{
				uploadedFile.CopyTo(localFile);
			}
			productImages.Add(new()
			{
				Name = fileName,
				Directory = folderName
			});
			return productImages;
		}

	}
}
