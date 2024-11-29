using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin,Branch")]
	public class EmployeesController : Controller
	{

		private readonly IGenericRepository<Employee> _employeeRepository;
		private readonly IGenericRepository<Branch> _branchRepository;
		private readonly UserManager<AppUser> _userManager;
		public EmployeesController(IGenericRepository<Employee> employeeRepository, UserManager<AppUser> userManager, IGenericRepository<Branch> branchRepository)
		{
			_employeeRepository = employeeRepository;
			_userManager = userManager;
			_branchRepository = branchRepository;
		}

		public async Task<IActionResult> Index()
		{
			List<EmployeeVM> models = new();
			var employees = await _employeeRepository.GetAll();
			foreach (var employee in employees.ToList())
			{
				var branch = await _branchRepository.Get(employee.BranchId);
				var user = await _userManager.FindByIdAsync(employee.AppUserId);
				models.Add(new()
				{
					Id = employee.Id,
					Name = employee.Name,
					Surname = employee.Surname,
					Position = employee.Position,
					BranchName = branch.Name,
					UserName = user.UserName
				});
			}
			return View(models);
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
			Employee employee = new()
			{
				Name = model.Name,
				Surname = model.Surname,
				Position = model.Position,
				BranchId = model.BranchId,
				AppUserId = model.AppUserId,
				UpdateDate = DateTime.Now
			};
			await _employeeRepository.Create(employee);
			await _employeeRepository.SaveAsync();
			return RedirectToAction("Index", "Home");
		}
		public async Task<IActionResult> Edit(int Id)
		{
			var employee = await _employeeRepository.Get(Id);
			var branchs = await _branchRepository.GetAll();
			var users = _userManager.Users.ToList();
			EmployeeVM model = new()
			{
				Name = employee.Name,
				Surname = employee.Surname,
				Position = employee.Position,
				Branchs = branchs.ToList(),
				Users = users
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(EmployeeVM model, int id)
		{
			var employee = await _employeeRepository.Get(id);
			employee.Name = model.Name;
			employee.Surname = model.Surname;
			employee.Position = model.Position;
			employee.BranchId = model.BranchId;
			employee.AppUserId = model.AppUserId;
			employee.UpdateDate = DateTime.Now;
			_employeeRepository.Update(employee);
			_employeeRepository.Save();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Delete(int Id)
		{
			var employee = await _employeeRepository.Get(Id);
			var branch = await _branchRepository.Get(employee.BranchId);
			var user = await _userManager.FindByIdAsync(employee.AppUserId);
			EmployeeVM model = new()
			{
				Name = employee.Name,
				Surname = employee.Surname,
				Position = employee.Position,
				BranchName = branch.Name,
				UserName = user.UserName
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(EmployeeVM model, int id)
		{
			_employeeRepository.Remove(id);
			_employeeRepository.Save();
			return RedirectToAction("Index");
		}
	}
}

