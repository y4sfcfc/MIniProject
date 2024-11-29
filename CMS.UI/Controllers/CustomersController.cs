using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using CMS.DAL.Entities;
using CMS.DAL.Enums;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Controllers
{
	public class CustomersController : Controller
	{
		private readonly IGenericRepository<Customer> _customerRepository;
		private readonly IGenericRepository<Loan> _loanRepository;
		private readonly IGenericRepository<LoanItem> _loanItemRepository;
		private readonly UserManager<AppUser> _userManager;
		private readonly IToastNotification _toastr;

		public CustomersController(IGenericRepository<Customer> customerRepository,
			UserManager<AppUser> userManager,
			IGenericRepository<Loan> loanRepository,
			IGenericRepository<LoanItem> loanItemRepository,
			IToastNotification toastr)
		{
			_customerRepository = customerRepository;
			_userManager = userManager;
			_loanRepository = loanRepository;
			_loanItemRepository = loanItemRepository;
			_toastr = toastr;
		}
		public async Task<IActionResult> Details()
		{
			string userId = _userManager.GetUserId(User);
			var customers =await _customerRepository.GetAll();
			var customer = customers.FirstOrDefault(c=>c.AppUserId==userId);
			if (userId == null||customer==null)
			{
				_toastr.AddWarningToastMessage("You haven't registered as a customer");
				return RedirectToAction("Create","Customers");
			}
			var user = await _userManager.FindByIdAsync(userId);
			/*var customer = (await _customerRepository.GetAll()).FirstOrDefault(c => c.AppUserId == userId);*/
			var loans = (await _loanRepository.GetAll()).Where(l => l.CustomerId == customer.Id && l.Status == Status.Accept).ToList();
			List<LoanItemVM> LoanItemModels = new();
			foreach (var loan in loans)
			{
				var loanItems = (await _loanItemRepository.GetAll()).Where(l => l.LoanId == loan.Id).ToList();
				foreach (var loanItem in loanItems)
				{
					var loanInc = await _loanRepository.Get(loan.Id,"LoanDetail");
					LoanItemModels.Add(new()
					{
						LoanId = loan.Id,
						Loan = loanInc,
						Count = loanItem.Count,
						Price = loan.TotalPrice,
						ProductId = loanItem.ProductId,
						
					});

				}
			}
			CustomerVM model = new()
			{
				Id = customer.Id,
				Name = customer.Name,
				Surname = customer.Surname,
				Address = customer.Address,
				Occupation = customer.Occupation,
				UserName = user.UserName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				LoanItems = LoanItemModels,
				
			};
			return View(model);
		}
		public async Task<IActionResult> Edit(int Id)
		{
			string userId = _userManager.GetUserId(User);
			var user = await _userManager.FindByIdAsync(userId);
			var customer = await _customerRepository.Get(Id);
			CustomerVM model = new()
			{
				Id = customer.Id,
				Name = customer.Name,
				Surname = customer.Surname,
				Address = customer.Address,
				Occupation = customer.Occupation,
				UserName = user.UserName,
				UserId = userId

			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(CustomerVM model, int id)
		{
			string userId = _userManager.GetUserId(User);
			var user = await _userManager.FindByIdAsync(userId);
			user.UserName = model.UserName;
			Customer customer = new()
			{
				Id = model.Id,
				Name = model.Name,
				Surname = model.Surname,
				Address = model.Address,
				Occupation = model.Occupation,
				AppUserId = model.UserId
			};
			await _userManager.UpdateAsync(user);
			_customerRepository.Update(customer);
			_customerRepository.Save();
			return RedirectToAction("Details");
		}
		public async Task<IActionResult> Create()
		{

/*			var userId = _userManager.GetUserId(User);
			if(userId == null)
			{
				_toastr.AddWarningToastMessage("You havent registered  as a user");
				return RedirectToAction("LogIn","Account");
			}*/
/*			CustomerVM model = new()
			{
				UserId = userId,
			};*/
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(CustomerVM model)
		{
			var userId = _userManager.GetUserId(User);
			if (userId == null)
			{
				_toastr.AddWarningToastMessage("You haven't registered  as a user");
				return RedirectToAction("LogIn", "Account");
			}
			Customer customer = new()
			{
				Name = model.Name,
				Surname = model.Surname,
				Occupation = model.Occupation,
				Address = model.Address,
				AppUserId = userId,
			};
			await _customerRepository.Create(customer);
			await _customerRepository.SaveAsync();
			var referer = Request.Headers["Referer"].ToString();
			return Redirect(referer);
		}
	}
}
