using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		/*
		 Payment API not now 
		 */
		private readonly IGenericRepository<Payment> _paymentRepository;

		public PaymentController(IGenericRepository<Payment> paymentRepository)
		{
			_paymentRepository = paymentRepository;
		}

		public async Task<IActionResult> Create(PaymentVM model)
		{
			if (ModelState.IsValid)
			{
				Payment payment = new()
				{
					
				};
				await _paymentRepository.Create(payment);
				await _paymentRepository.SaveAsync();
			}
			return Ok();
		}
	}
}
