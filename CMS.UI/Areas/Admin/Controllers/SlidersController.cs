using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Enums;
using CMS.UI.Models;
using System.IO;

namespace CMS.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin,Employee")]
	public class SlidersController : Controller
	{
		private readonly IGenericRepository<Slider> _sliderRepository;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public SlidersController(IGenericRepository<Slider> sliderRepository, IWebHostEnvironment webHostEnvironment)
		{
			_sliderRepository = sliderRepository;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<IActionResult> Index()
		{
			var sliders = await _sliderRepository.GetAll();
			List<SliderVM> models = new();
			foreach (var slider in sliders)
			{
				models.Add(new SliderVM
				{
					Title = slider.Title,
					Description = slider.Description,
					ImageUrl = slider.ImageUrl,
					IsActive = slider.IsActive,
				});

			}
			return View(models);
		}

		public async Task<IActionResult> Create()
		{
            return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(SliderVM model)
		{
			string imageurl = await UploadImage(model.ImageFile);
			Slider slider = new()
			{
				Title = model.Title,
				Description = model.Description,
				ImageUrl = imageurl,
				IsActive = model.IsActive,
			};
			await _sliderRepository.Create(slider);
			await _sliderRepository.SaveAsync();
			return RedirectToAction("Index");
		}

		public async Task<string> UploadImage(IFormFile imageFile)
		{
			string localPath = _webHostEnvironment.WebRootPath;
			string folderName = "SliderImages";
			string fileName =Guid.NewGuid().ToString()+Path.GetExtension(imageFile.FileName);
			string filePath = Path.Combine(localPath,folderName,fileName);
			using FileStream fileStream = new FileStream(filePath, FileMode.Create);
			await imageFile.CopyToAsync(fileStream);
			return fileName;
		}


	}
}
