using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMS.DAL.Entities;
using CMS.DAL.Repository.Interface;
using CMS.UI.Models;

namespace CMS.UI.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin,Employee")]
	public class LoanItemsController : Controller
	{
		private readonly IGenericRepository<LoanItem> _loanItemRepository;
		private readonly IGenericRepository<Loan> _loanRepository;
		private readonly IGenericRepository<Product> _productRepository;

		public LoanItemsController(IGenericRepository<LoanItem> loanItemRepository,
			IGenericRepository<Product> productRepository,
			IGenericRepository<Loan> loanRepository)
		{
			_loanItemRepository = loanItemRepository;
			_productRepository = productRepository;
			_loanRepository = loanRepository;
		}

		public async Task<IActionResult> Index()
		{
			List<LoanItemVM> models = new();
			var loanItems = (await _loanItemRepository.GetAll()).ToList();
			foreach (var loanItem in loanItems)
			{
				var product = await _productRepository.Get(loanItem.ProductId);
				var loan = await _loanRepository.Get(loanItem.LoanId);
				models.Add(new()
				{
					Id = loanItem.Id,
					Product = product,
					Loan = loan,
					Count = loanItem.Count,
					Price = loanItem.Price,
				});
			}
			return View(models);
		}
		public async Task<IActionResult> Create()
		{
			var products = (await _productRepository.GetAll()).ToList();
			var loans = (await _loanRepository.GetAll()).ToList();
			LoanItemVM model = new()
			{
				Products = products,
				Loans = loans,
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Create(LoanItemVM model)
		{
			var product = await _productRepository.Get(model.ProductId);
			var loan = await _loanRepository.Get(model.LoanId);
			int count = model.Count;
			decimal price = count * loan.TotalPrice;
			if (count < product.Count)
			{
				LoanItem loanItem = new()
				{
					ProductId = model.ProductId,
					LoanId = model.LoanId,
					Count = count,
					Price = price,
				};
				product.Count -= count;
				_productRepository.Update(product);
				_productRepository.Save();
				await _loanItemRepository.Create(loanItem);
				await _loanItemRepository.SaveAsync();
			}
			else
			{
				return View(model);
			}
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Edit(int Id)
		{
			var loanItem = await _loanItemRepository.Get(Id);
			var products = (await _productRepository.GetAll()).ToList();
			var loans = (await _loanRepository.GetAll()).ToList();
			LoanItemVM model = new()
			{
				Products = products,
				Loans = loans,
				ProductId = loanItem.ProductId,
				LoanId = loanItem.LoanId,
				Count = loanItem.Count,
				Price = loanItem.Price,
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(LoanItemVM loanItemModel, int id)
		{
			var loanItem = await _loanItemRepository.Get(id);
			var loan = await _loanRepository.Get(loanItemModel.LoanId);
			var product = await _productRepository.Get(loanItemModel.ProductId);
			int count = loanItemModel.Count;
			decimal price = count * loan.TotalPrice;
			loanItem.ProductId = loanItemModel.ProductId;
			loanItem.Price = price;
			loanItem.LoanId = loanItemModel.LoanId;
			product.Count += loanItem.Count;
			product.Count -= count;
			loanItem.Count = loanItemModel.Count;


			_loanItemRepository.Update(loanItem);
			_loanItemRepository.Save();
			_productRepository.Update(product);
			_productRepository.Save();
			return RedirectToAction("Index");
		}
		public async Task<IActionResult> Delete(int Id)
		{
			var loanItem =await _loanItemRepository.Get(Id);
			var product = await _productRepository.Get(loanItem.ProductId);
			var loan = await _loanRepository.Get(loanItem.LoanId);
			LoanItemVM model = new()
			{
				Id = loanItem.Id,
				Product = product,
				Loan = loan,
				Count = loanItem.Count,
				Price = loanItem.Price,
			};
			return View(model);
		}
		[HttpPost]
		public IActionResult Delete(LoanItemVM model)
		{
			_loanItemRepository.Remove(model.Id);
			_loanItemRepository.Save();
			return RedirectToAction("Index");
		}
	}
}
