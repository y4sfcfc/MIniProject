using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class MerchantsController : Controller
    {
        private readonly IGenericRepository<Merchant> _merchantRepository;
        private readonly IGenericRepository<Loan> _loanRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<Employee> _employeeRepository;

        public MerchantsController(
            IGenericRepository<Loan> loanRepository,
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<Employee> employeeRepository,
            IGenericRepository<Merchant> merchantRepository)
        {
            _loanRepository = loanRepository;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _merchantRepository = merchantRepository;
        }

        public async Task<IActionResult> Index()
        {
            List<MerchantVM> models = new();
            var merchants = await _merchantRepository.GetAll();
            foreach (var merchant in merchants)
            {
                models.Add(new MerchantVM()
                {
                    Id = merchant.Id,
                    Name = merchant.Name,
                    Description = merchant.Description,
                    TerminalNo = merchant.TerminalNo,
                });
            }
            return View(models);
        }
    }
}
