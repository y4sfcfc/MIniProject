using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using CMS.DAL.Entities;
using CMS.DAL.Enums;
using CMS.DAL.Repository.Interface;
using CMS.UI.Enums;
using CMS.UI.Models;

namespace CMS.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin,Employee")]
	public class LoansController : Controller
	{
		private readonly IGenericRepository<Loan> _loanRepository;
		private readonly IGenericRepository<Customer> _customerRepository;
		private readonly IGenericRepository<Employee> _employeeRepository;
		private readonly IGenericRepository<Product> _productRepository;
		private readonly IGenericRepository<LoanItem> _loanItemRepository;
		private readonly IGenericRepository<LoanDetail> _loanDetailRepository;
		private readonly IGenericRepository<Branch> _branchRepository;
		private readonly IGenericRepository<Payment> _paymentRepository;
		private readonly UserManager<AppUser> _userManager;

		public LoansController(IGenericRepository<Loan> loanRepository,
			IGenericRepository<Customer> customerRepository,
			IGenericRepository<Employee> employeeRepository,
			IGenericRepository<Product> productRepository,
			IGenericRepository<LoanItem> loanItemRepository,
			UserManager<AppUser> userManager,
			IGenericRepository<Branch> branchRepository,
			IGenericRepository<Payment> paymentRepository,
			IGenericRepository<LoanDetail> loanDetailRepository)
		{
			_loanRepository = loanRepository;
			_customerRepository = customerRepository;
			_employeeRepository = employeeRepository;
			_productRepository = productRepository;
			_loanItemRepository = loanItemRepository;
			_userManager = userManager;
			_branchRepository = branchRepository;
			_paymentRepository = paymentRepository;
			_loanDetailRepository = loanDetailRepository;
		}

		public async Task<IActionResult> Index()
		{
			List<LoanVM> models = new();
			var loans = (await _loanRepository.GetAll()).ToList();

			foreach (var loan in loans)
			{
				var customer = await _customerRepository.Get(loan.CustomerId);
				var employee = await _employeeRepository.Get(loan.EmployeeId ?? 0);
				models.Add(new()
				{
					Id = loan.Id,
					Title = loan.Title,
					MonthlyPrice = loan.MonthlyPrice,
					TotalPrice = loan.TotalPrice,
					Terms = loan.Terms,
					Customer = customer,
					Employee = employee,
					Status = loan.Status,
				});
			}
			return View(models);
		}
		public async Task<IActionResult> Create()
		{
			var customers = (await _customerRepository.GetAll()).ToList();
			var employees = await _employeeRepository.GetAll();
			var products = (await _productRepository.GetAll()).ToList();
			string userId =  _userManager.GetUserId(User);
			var employee =employees.FirstOrDefault();
				LoanVM model = new()
			{
				Customers = customers,
				EmployeeId=employee.Id,
				Products = products,
				TermList = new(),

			};
			foreach (var item in Enum.GetValues(typeof(Terms)))
			{
				int term = (int)item;
				model.TermList.Add(term);
			}
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(LoanVM model)
		{
			var product = await _productRepository.Get(model.ProductId);
			decimal monthlyPrice = (product.Price / model.Terms) * (decimal)1.05;
			decimal totalPrice = monthlyPrice * model.Terms;
			Loan loan = new()
			{
				Title = model.Title,
				MonthlyPrice = monthlyPrice,
				TotalPrice = totalPrice,
				Terms = model.Terms,
				EmployeeId = model.EmployeeId,
				CustomerId = model.CustomerId,
				Status = Status.Accept,
				UpdateDate = DateTime.Now,
			};
			int count = model.Count;
			decimal price = count * totalPrice;
			await _loanRepository.Create(loan);
			await _loanRepository.SaveAsync();
			LoanItem loanItem = new()
			{
				LoanId = loan.Id,
				ProductId = model.ProductId,
				Count = count,
				Price = price
			};
			LoanDetail loanDetail = new()
			{
				LoanId = loan.Id,
				CurrentAmount = totalPrice
			};
			product.Count += loanItem.Count;
			product.Count -= count;
			_productRepository.Update(product);
			_productRepository.Save();
		
			await _loanItemRepository.Create(loanItem);
			await _loanItemRepository.SaveAsync();
			await _loanDetailRepository.Create(loanDetail);
			await _loanDetailRepository.SaveAsync();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Edit(int Id)
		{
			var loan = await _loanRepository.Get(Id);
			var loanItems = await _loanItemRepository.GetAll();
			var loanItem = loanItems.FirstOrDefault(l => l.LoanId == Id);
			var customers = (await _customerRepository.GetAll()).ToList();
			var employees = (await _employeeRepository.GetAll()).ToList();
			var products = (await _productRepository.GetAll()).ToList();
			LoanVM model = new()
			{
				Title = loan.Title,
				CustomerId = loan.CustomerId,
				Customers = customers,
				EmployeeId = loan.EmployeeId,
				Employees = employees,
				ProductId = loanItem.ProductId,
				Count = customers.Count,
				Products = products,
				Terms = loan.Terms,
				TermList = new(),
				Statuses = new()

			};
			foreach (var item in Enum.GetValues(typeof(Terms)))
			{
				int term = (int)item;
				model.TermList.Add(term);
			}
			foreach (var status in Enum.GetValues(typeof(Status)))
			{
				model.Statuses.Add(status.ToString());
			}
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(LoanVM model, int id, string assess)
		{
			var loan = await _loanRepository.Get(model.Id != 0 ? model.Id : id);
			var loanItems = await _loanItemRepository.GetAll();
			var loanItem = loanItems.FirstOrDefault(li => li.LoanId == loan.Id);
			var loanDetails = await _loanDetailRepository.GetAll();
			var loanDetail = loanDetails.FirstOrDefault(ld => ld.LoanId == loan.Id);
			var product = await _productRepository.Get(model.ProductId);
			decimal monthlyPrice = (product.Price / model.Terms) * (decimal)1.05;
			decimal totalPrice = monthlyPrice * model.Terms;
			loan.Title = model.Title;
			loan.CustomerId = model.CustomerId;
			loan.EmployeeId = model.EmployeeId;
			loan.Terms = model.Terms;
			loan.TotalPrice = totalPrice;
			loan.MonthlyPrice = monthlyPrice;
			loan.Status = model.Status;
			int count = model.Count;
			decimal price = count * totalPrice;
			if (model.Status == Status.Accept)
			{
				product.Count += loanItem.Count;
				product.Count -= count;
			}
			loanItem.ProductId = model.ProductId;
			loanItem.Count = count;
			loanItem.Price = price;
			loanItem.UpdateDate = DateTime.Now;
			_productRepository.Update(product);
			_productRepository.Save();
			_loanRepository.Update(loan);
			_loanRepository.Save();
			_loanItemRepository.Update(loanItem);
			_loanItemRepository.Save();

			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Delete(int Id)
		{
			var loan = await _loanRepository.Get(Id);
			var customer = await _customerRepository.Get(loan.CustomerId);
			var employee = await _employeeRepository.Get(loan.EmployeeId ?? 0);
			/*		var product=(await _loanItemRepository.GetAll()).Where(l=>l.LoanId==Id);*/
			LoanVM model = new()
			{
				Id = Id,
				Title = loan.Title,
				Customer = customer,
				Employee = employee,
				Terms = loan.Terms,
				MonthlyPrice = loan.MonthlyPrice,
				TotalPrice = loan.TotalPrice,
			};

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Delete(LoanVM model)
		{
			var loan = await _loanRepository.Get(model.Id);
			var payments = await _paymentRepository.GetAll();
			var payment = payments.FirstOrDefault(p => p.LoanId == loan.Id);
			_paymentRepository.Remove(payment.Id);
			_loanRepository.Save();
			_loanRepository.Remove(loan.Id);
			_loanRepository.Save();
			return RedirectToAction("Index");
		}
		[Authorize(Roles ="Employee")]
		public async Task<IActionResult> Assess(int? loanId)
		{
			var loan = await _loanRepository.Get(loanId ?? 0);
			var customer = await _customerRepository.Get(loan.CustomerId);
			string userId = _userManager.GetUserId(User);
			var employees = await _employeeRepository.GetAll();
			var employee = employees.ToList().FirstOrDefault(e => e.AppUserId == userId);
			var loanItems = await _loanItemRepository.GetAll();
			var loanItem = loanItems.FirstOrDefault(li => li.LoanId == loan.Id);
			int productId = loanItems.FirstOrDefault(p => p.LoanId == loanId).ProductId;
			var product = await _productRepository.Get(productId, "Branch");
			LoanVM model = new()
			{
				Id = loan.Id,
				Title = loan.Title,
				Customer = customer,
				CustomerId = customer.Id,
				Product = product,
				ProductId = productId,
				EmployeeId = employee.Id,
				Terms = loan.Terms,
				MonthlyPrice = loan.MonthlyPrice,
				TotalPrice = loan.TotalPrice,
				Status = loan.Status,
				Count = loanItem.Count
			};


			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Assess(LoanVM model, int Id)
		{
			var loan = await _loanRepository.Get(model.Id != 0 ? model.Id : Id);
			var loanItems = await _loanItemRepository.GetAll();
			var loanItem = loanItems.FirstOrDefault(li => li.LoanId == loan.Id);
			var product = await _productRepository.Get(model.ProductId);
			loan.Status = model.Status;
			loan.UpdateDate = DateTime.Now;
			loan.EmployeeId = model.EmployeeId;
			if (model.Status == Status.Accept)
			{
				product.Count -= loanItem.Count;
				product.UpdateDate = DateTime.Now;
			}
			else if (model.Status == Status.Reject)
			{
				loan.Status = Status.Reject;
			}
			_productRepository.Update(product);
			_loanRepository.Save();
			_loanRepository.Update(loan);
			_loanRepository.Save();
			return RedirectToAction("Index");
		}
	}
}
