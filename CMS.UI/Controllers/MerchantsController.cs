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

	public class MerchantsController : Controller
	{
		private readonly IGenericRepository<Merchant> _merchantRepository;
		private readonly UserManager<AppUser> _userManager;
		private readonly IToastNotification _toastr;
		private readonly IValidator<MerchantVM> _validator;
		public MerchantsController(IGenericRepository<Merchant> merchantRepository,
			UserManager<AppUser> userManager,
			IToastNotification toastr,
			IValidator<MerchantVM> validator)
		{
			_merchantRepository = merchantRepository;
			_userManager = userManager;
			_toastr = toastr;
			_validator = validator;
		}
		public async Task<IActionResult> Index()
		{
			List<MerchantVM> models = new();
			var merchants = await _merchantRepository.GetAll();
			foreach (var merchant in merchants)
			{
				models.Add(new()
				{
					Id = merchant.Id,
					Name = merchant.Name,
					Description = merchant.Description,
				});
			}
			return View(models);
		}
		[Authorize(Roles = "Admin")]
		public IActionResult Create()
		{
			string userId = _userManager.GetUserId(User);
			MerchantVM model = new()
			{
				AppUserId = userId
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(MerchantVM model)
		{
			var result = await _validator.ValidateAsync(model);
			if (result.IsValid)
			{
				Merchant merchant = new()
				{
					Name = model.Name,
					Description = model.Description,
					TerminalNo = model.TerminalNo,
					AppUserId = model.AppUserId,
				};
				await _merchantRepository.Create(merchant);
				await _merchantRepository.SaveAsync();
				_toastr.AddSuccessToastMessage("Merchant created successfully");
				return RedirectToAction("Index", "Merchants");
			}
			else{
				result.AddToModelState(ModelState);
				return View(model);
			}
		}

	}
}
