using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using CMS.DAL.Entities;
using CMS.UI.Models;

namespace CMS.UI.Areas.Admin.Controllers
{
	[Area("admin")]
	[Authorize(Roles ="Admin,Employee")]
	public class AccountController : Controller
	{
		private readonly SignInManager<AppUser> _signInmanagerManager;
		private readonly UserManager<AppUser> _userManager;
		private readonly IToastNotification _toastr;
		public AccountController(SignInManager<AppUser> signInmanagerManager, UserManager<AppUser> userManager, IToastNotification toastr)
		{
			_signInmanagerManager = signInmanagerManager;
			_userManager = userManager;
			_toastr = toastr;
		}

		public IActionResult LogIn()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> LogIn(LogInVM model)
		{

			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user != null)
				{
					var signInResult = await _signInmanagerManager.PasswordSignInAsync(user, model.Password, false, false);
					_toastr.AddSuccessToastMessage($"Welcome {user.UserName}");
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", "Email or Password wrong");
				}
			}
			return View(model);
		}
	}
}
