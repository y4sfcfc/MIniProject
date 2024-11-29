using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Controllers
{
	[Authorize(Roles ="Admin,Branch")]
	public class EmployeesController : Controller
	{
		private readonly IGenericRepository<Employee> _employeeRepository;
		private readonly IGenericRepository<Branch> _branchRepository;
		private readonly UserManager<AppUser> _userManager;
		private readonly IToastNotification _toastr;
		private readonly IValidator<EmployeeVM> _validator;
		public EmployeesController(IGenericRepository<Employee> employeeRepository,
			IGenericRepository<Branch> branchRepository,
			UserManager<AppUser> userManager,
			IToastNotification toastr,
			IValidator<EmployeeVM> validator)
		{
			_employeeRepository = employeeRepository;
			_branchRepository = branchRepository;
			_userManager = userManager;
			_toastr = toastr;
			_validator = validator;
		}

		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> Create()
		{
			string userId = _userManager.GetUserId(User);
			var branchs = await _branchRepository.GetAll();
			EmployeeVM model = new()
			{
				AppUserId = userId,
				Branchs = branchs.ToList()
			};

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(EmployeeVM model)
		{
			var result =await _validator.ValidateAsync(model);
			if (result.IsValid)
			{
				Employee employee = new()
				{
					Name = model.Name,
					Surname = model.Surname,
					Position = model.Position,
					BranchId = model.BranchId,
					AppUserId = model.AppUserId,
				};
				await _employeeRepository.Create(employee);
				await _employeeRepository.SaveAsync();
				_toastr.AddSuccessToastMessage("Employee added successfully");
				return RedirectToAction("Index", "Merchants");
			}
			else
			{
				result.AddToModelState(ModelState);
				var branchs =await _branchRepository.GetAll();
				model.Branchs=branchs.ToList();
				return View(model);
			}
		}
	}
}
