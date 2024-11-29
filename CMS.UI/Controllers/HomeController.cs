using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;
using System.Diagnostics;

namespace CMS.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGenericRepository<Slider> _sliderRepository;
        private readonly IGenericRepository<Merchant> _merchantRepository;
        public HomeController(ILogger<HomeController> logger,
            IGenericRepository<Slider> sliderRepository,
            IGenericRepository<Merchant> merchantRepository)
        {
            _logger = logger;
            _sliderRepository = sliderRepository;
            _merchantRepository = merchantRepository;
        }

        public async Task<IActionResult> Index()
        {
            HomeVM model = new();
            List<SliderVM> sliderModels = new();
            List<MerchantVM> merchantModels = new();
            var sliders = await _sliderRepository.GetAll();
            foreach (var slider in sliders.Where(sl=>sl.IsActive==true).ToList())
            {
                sliderModels.Add(new()
                {
                    Title = slider.Title,
                    Description = slider.Description,
                    ImageUrl = slider.ImageUrl,
                    IsActive = slider.IsActive,
                });
            }
            var merchants = await _merchantRepository.GetAll();
            foreach (var merchant in merchants.ToList())
            {
                merchantModels.Add(new()
                {
                    Id = merchant.Id,
                    Name = merchant.Name,
                    Description = merchant.Description,
                });
            }
            model.Sliders = sliderModels;
            model.Merchants = merchantModels;
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
