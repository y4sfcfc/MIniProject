using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles ="Admin,Employee")]
	public class CustomersController : Controller
	{
		private readonly IGenericRepository<Customer> _customerRepository;
		private readonly UserManager<AppUser> _userManager;

		public CustomersController(IGenericRepository<Customer> customerRepository,
			UserManager<AppUser> userManager)
		{
			_customerRepository = customerRepository;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			List<CustomerVM> models = new();
			var customers = (await _customerRepository.GetAll());
			var customerlist = customers.ToList();
			foreach (var customer in customerlist)
			{
				if (customer.AppUserId != null)
				{
					var user = _userManager.Users.FirstOrDefault(u => u.Id == customer.AppUserId);

					models.Add(new()
					{
						Id = customer.Id,
						Name = customer.Name,
						Surname = customer.Surname,
						Occupation = customer.Occupation,
						Address = customer.Address,
						UserId = user.Id,
						UserName= user.UserName,
					});
				}
			}
			return View(models);
		}

		public async Task<IActionResult> Create()
		{
			var users = await _userManager.Users.ToListAsync();
			CustomerVM model = new()
			{
				Users = users
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(CustomerVM model)
		{
			Customer customer = new()
			{
				Name = model.Name,
				Surname = model.Surname,
				Occupation = model.Occupation,
				Address = model.Address,
				AppUserId = model.UserId,
				UpdateDate= DateTime.Now,
			};
			await _customerRepository.Create(customer);
			await _customerRepository.SaveAsync();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Delete(int id)
		{
			var customer = await _customerRepository.Get(id);
			if (customer == null)
			{
				return NotFound();
			}
			var user = await _userManager.Users.FirstOrDefaultAsync(u=>u.Id==customer.AppUserId);
			CustomerVM model = new()
			{
				Id = customer.Id,
				Name = customer.Name,
				Surname=customer.Surname,
				Occupation=customer.Occupation,
				Address=customer.Address,
				UserName=user.UserName??"No username"

			};
			return View(model);
		}
		[HttpPost]
		public IActionResult Delete(CustomerVM model)
		{
			_customerRepository.Remove(model.Id);
			_customerRepository.Save();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Edit(int id)
		{
			var customer = await _customerRepository.Get(id);
			var users = await _userManager.Users.ToListAsync();
			CustomerVM model = new()
			{
				Name = customer.Name,
				Surname = customer.Surname,
				Occupation = customer.Occupation,
				Address = customer.Address,
				UserId = customer.AppUserId,
				Users=users
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(CustomerVM model,int id)
		{
			var customer = await _customerRepository.Get(id);
			customer.Name = model.Name;
			customer.Surname= model.Surname;
			customer.Occupation = model.Occupation;
			customer.Address = model.Address;
			customer.AppUserId = model.UserId;
			customer.UpdateDate = DateTime.Now;
			 _customerRepository.Update(customer);
			_customerRepository.Save();
			return RedirectToAction("Index");
		}
	}
}
