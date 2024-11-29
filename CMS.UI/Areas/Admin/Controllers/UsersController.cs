using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.DAL.Entities;
using CMS.UI.Models;

namespace CMS.UI.Area.Admin
{
    [Area("Admin")]

    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UsersController(UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            List<UserVM> userVM = new();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userVM.Add(new()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                });
            }
            return View(userVM);
        }
        public async Task<IActionResult> Edit(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            UserVM model = new()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserVM model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.UserName = model.UserName;

            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            UserVM model = new()
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,

            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(UserVM model, string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = _roleManager.Roles.ToList();
            UserRoleVM model = new()
            {
                Roles = new List<RoleVM>()
            };
            model.UserName = user.UserName;
            model.UserId = userId;
            foreach (var role in roles)
            {
                model.Roles.Add(new RoleVM
                {
                    Id = role.Id,
                    Name = role.Name,
                });
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(UserRoleVM model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            var user = await _userManager.FindByIdAsync(model.UserId);
            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains(role.Name))
            {
                await _userManager.AddToRoleAsync(user, role.Name);
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
