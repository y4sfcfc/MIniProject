using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Controllers
{
	public class BranchsController : Controller
	{
		private readonly IGenericRepository<Branch> _branchRepository;
		private readonly IGenericRepository<Merchant> _merchantRepository;
		private readonly UserManager<AppUser> _userManager;
		private readonly IToastNotification _toastr;
		private readonly IValidator<BranchVM> _validator;

		public BranchsController(IGenericRepository<Branch> branchRepository,
			IGenericRepository<Merchant> merchantRepository,
			UserManager<AppUser> userManager,
			IToastNotification toastr,
			IValidator<BranchVM> validator)
		{
			_branchRepository = branchRepository;
			_merchantRepository = merchantRepository;
			_userManager = userManager;
			_toastr = toastr;
			_validator = validator;
		}

		public async Task<IActionResult> Index(int? merchantId)
		{
			var branchs = await _branchRepository.GetAll();
			var branchList = branchs.Where(b => b.MerchantId == merchantId);
			List<BranchVM> models = new();
			foreach (var item in branchList.ToList())
			{
				models.Add(new()
				{
					Id = item.Id,
					Name = item.Name,
				});
			}
			return View(models);
		}

		[Authorize(Roles = "Merchant,Admin")]

		public async Task<IActionResult> Create()
		{
			string userId = _userManager.GetUserId(User);
			var merchants = await _merchantRepository.GetAll();
			BranchVM model = new()
			{
				AppUserId = userId,
				Merchants = merchants.ToList()
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(BranchVM model)
		{
			var result = await _validator.ValidateAsync(model);
			if (result.IsValid)
			{
				string userId = _userManager.GetUserId(User);
				Branch branch = new()
				{
					Name = model.Name,
					Description = model.Description,
					Address = model.Address,
					MerchantId = model.MerchantId,
					AppUserId = model.AppUserId,
				};
				await _branchRepository.Create(branch);
				await _branchRepository.SaveAsync();
				_toastr.AddSuccessToastMessage("Branch Added successfully");
				return RedirectToAction("Index", "Merchants");
			}
			else
			{
				result.AddToModelState(ModelState);
				var merchants = await _merchantRepository.GetAll();
				model.Merchants = merchants.ToList();
				return View(model);
			}
		}
	}
}
