using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin,Employee")]
	public class PaymentsController : Controller
	{
		private readonly IGenericRepository<Payment> _paymentRepository;
		private readonly IGenericRepository<LoanItem> _loanItemRepository;
		private readonly IGenericRepository<LoanDetail> _loanDetailRepository;
		private readonly IGenericRepository<Loan> _loanRepository;
		private readonly IGenericRepository<Customer> _customerRepository;

		public PaymentsController(IGenericRepository<Payment> paymentRepository, IGenericRepository<LoanItem> loanItemRepository, IGenericRepository<LoanDetail> loanDetailRepository,
			IGenericRepository<Loan> loanRepository, IGenericRepository<Customer> customerRepository)
		{
			_paymentRepository = paymentRepository;
			_loanItemRepository = loanItemRepository;
			_loanDetailRepository = loanDetailRepository;
			_loanRepository = loanRepository;
			_customerRepository = customerRepository;
		}

		public async Task<IActionResult> Index()
		{
			List<PaymentVM> models = new();
			var payments = await _paymentRepository.GetAll();
			
			var loanItems = await _loanItemRepository.GetAll();
			foreach (var item in payments.ToList())
			{
				var loan = await _loanRepository.Get(item.LoanId);
				var loanDetails = await _loanDetailRepository.GetAll();
				var loanDetail = loanDetails.FirstOrDefault(c => c.LoanId == loan.Id);
				var customer = await _customerRepository.Get(loan.CustomerId);

				models.Add(new()
				{
					Id = item.Id,
					Amount = item.Amount,
					Date = item.PaymentDate,
					Loan = loan,
					Customer=customer,
					LoanDetail = loanDetail,
					PaymentType=item.PaymentType

				});
			}
			return View(models);
		}
	}
}
