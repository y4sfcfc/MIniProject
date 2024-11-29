using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using CMS.DAL.Entities;
using CMS.DAL.Enums;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;
using System.Diagnostics;

namespace CMS.UI.Area.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin,Employee")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGenericRepository<Merchant> _merchantRepository;
        private readonly IGenericRepository<Loan> _loanRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IToastNotification _toastr;

		public HomeController(ILogger<HomeController> logger,
			IGenericRepository<Loan> loanRepository,
			IGenericRepository<Customer> customerRepository,
			IGenericRepository<Employee> employeeRepository,
			IGenericRepository<Merchant> merchantRepository,
			IToastNotification toastr)
		{
			_logger = logger;
			_loanRepository = loanRepository;
			_customerRepository = customerRepository;
			_employeeRepository = employeeRepository;
			_merchantRepository = merchantRepository;
			_toastr = toastr;
		}

		public async Task<IActionResult> Index()
        {
            List<MerchantVM> models = new();
            var merchants=await _merchantRepository.GetAll();
            foreach (var merchant in merchants)
            {
                models.Add(new MerchantVM() {
                Id = merchant.Id,
                Name = merchant.Name,
                Description = merchant.Description,
                TerminalNo = merchant.TerminalNo,
                });
            }

			return View(models);
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
