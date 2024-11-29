using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Enums;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.ViewComponents
{
	public class LoanViewComponent:ViewComponent
	{
		private readonly IGenericRepository<Loan> _loanRepository;
		private readonly IGenericRepository<Customer> _customerRepository;
		private readonly IGenericRepository<Employee> _employeeRepository;

		public LoanViewComponent(IGenericRepository<Loan> loanRepository,
			IGenericRepository<Customer> customerRepository, 
			IGenericRepository<Employee> employeeRepository)
		{
			_loanRepository = loanRepository;
			_customerRepository = customerRepository;
			_employeeRepository = employeeRepository;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			List<LoanVM> models = new();
			var pendingLoans = await GetItemsAsync();
			foreach (var loan in pendingLoans)
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
					Status = loan.Status

				});
			}
			return View(models);
		}

		private async Task<List<Loan>> GetItemsAsync()
		{
			var loans = await _loanRepository.GetAll();
			var pendingloans = loans.Where(l => l.Status == Status.Pending).ToList();
			return pendingloans.ToList();
		}
	}
}
