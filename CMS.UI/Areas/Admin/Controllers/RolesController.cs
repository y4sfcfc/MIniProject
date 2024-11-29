using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.UI.Models;
using System.Data;

namespace CMS.UI.Area.Admin
{
    [Area("Admin")]

    public class RolesController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            List<RoleVM> model = new();
            var roles = _roleManager.Roles.ToList();
            foreach (var role in roles)
            {
                model.Add(new RoleVM
                {
                    Id = role.Id,
                    Name = role.Name,
                });
            }
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleVM model)
        {
            AppRole appRole = new()
            {
                Name = model.Name,
            };
           var result = await _roleManager.CreateAsync(appRole);
            if(result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
		public async Task<IActionResult> Delete(string Id)
		{
            var role = await _roleManager.FindByIdAsync(Id);
			RoleVM model = new()
			{
				Id = role.Id,
				Name = role.Name
			};

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Delete(RoleVM model,string id)
		{
            var role =await _roleManager.FindByIdAsync(id);
			await _roleManager.DeleteAsync(role);
		
			return RedirectToAction("Index");
		}
        public async Task<IActionResult> Edit(string Id)
		{
            var role = await _roleManager.FindByIdAsync(Id);
			RoleVM model = new()
			{
				Name = role.Name
			};

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(RoleVM model,string id)
		{
            var role =await _roleManager.FindByIdAsync(id);
            role.Name=model.Name;
			await _roleManager.UpdateAsync(role);
			return RedirectToAction("Index");
		}
	}
}
