using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NToastNotify;
using CMS.DAL.Entities;
using CMS.UI.Models;
using CMS.UI.Services.Interfaces;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CMS.UI.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IToastNotification _toastr;
		private readonly IEmailService _emailService;
		private readonly IValidator<SignUpVM> _validator;
		public AccountController(UserManager<AppUser> userManager,
			SignInManager<AppUser> signInManager,
			IToastNotification toastr,
			IEmailService emailService,
			IValidator<SignUpVM> validator)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_toastr = toastr;
			_emailService = emailService;
			_validator = validator;
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
				var userPassword = await _userManager.CheckPasswordAsync(user, model.Password);
				if (!userPassword)
				{
					_toastr.AddErrorToastMessage("Email or Password wrong");
				}
				else
				{
					var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.Persistent, false);
					if (signInResult.Succeeded)
					{
						_toastr.AddSuccessToastMessage($"Welcome {user.UserName}");
						return RedirectToAction("Index", "Merchants");
					}
				}
			}

			return View(model);
		}
		public IActionResult SignUp()
		{

			return View();
		}
		[HttpPost]
		public async Task<IActionResult> SignUp(SignUpVM model)
		{
			var validateResult = await _validator.ValidateAsync(model);
			if (validateResult.IsValid)
			{
				AppUser user = new()
				{
					UserName = model.UserName,
					Email = model.Email,
					PhoneNumber = model.PhoneNumber,
				};
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await _userManager.AddToRoleAsync(user,"User");
					return RedirectToAction("LogIn", "Account");
				}
				else
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
					return View(model);
				}
			}
			else
			{
				validateResult.AddToModelState(ModelState);
				return View(model);
			}
		}
		public IActionResult ForgotPassword()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null)
				{
					ViewBag.ErrorMessage = "Email not found";
					return View();
				}
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);

				string createdUrl = Url.Action("ChangePassword", "Account", new { userId = user.Id, Token = token }, HttpContext.Request.Scheme);
				await _emailService.SendEmail(createdUrl, user.Email);
				ViewBag.SuccessMessage = "Email sent to mail";
			}
			return View(model);
		}

		public IActionResult ChangePassword()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ChangePassword(string userId, string token, ChangePasswordVM model)
		{

			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(userId);
				if (user == null)
				{
					return View();
				}

				var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
				/*            var samePasswordErr = await _userManager.CheckPasswordAsync(user, model.Password);
                            if (samePasswordErr)
                            {
                                ModelState.AddModelError("", "Dont type previous password");
                                return View();
                            }*/
				if (result.Succeeded)
				{
					return RedirectToAction("LogIn", "Account");
				}

			}
			return View(model);
		}
	}
}
