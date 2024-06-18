using FStep.Data;
using FStep.ViewModels.WareHouse;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace FStep.Controllers.ManagePost
{
	public class WareHouseController : Controller
	{
		private readonly FstepDbContext _db;

		public WareHouseController(FstepDbContext context) => _db = context;
		public IActionResult WareHouse(String? query, int? page)
		{
			int pageSize = 30;
			int pageNumber = (page ?? 1);
			var ListProduct = _db.Posts.AsQueryable();
			ListProduct = ListProduct.Where(p => p.Status == "true" && p.Location != null);

			if (!string.IsNullOrEmpty(query))
			{
				ListProduct = ListProduct.Where(p => p.IdUserNavigation.StudentId.Contains(query));
			}

			var result = ListProduct.Select(s => new WareHouseVM
			{
				IdPost = s.IdPost,
				NameProduct = s.Content ?? string.Empty,
				IdStudent = s.IdUserNavigation.StudentId ?? string.Empty,
				Price = s.IdProductNavigation != null && s.IdProductNavigation.Price.HasValue ? s.IdProductNavigation.Price.Value : null,
				Quantity = s.IdProductNavigation != null && s.IdProductNavigation.Quantity.HasValue ? s.IdProductNavigation.Quantity.Value : 1,
				Location = s.Location ?? string.Empty,
				Category = s.Category ?? string.Empty
			});
			var pageList = result.ToPagedList(pageNumber, pageSize);

			ViewBag.Query = query;
			return View(pageList);
		}
		public IActionResult TransactionDetail(int id)
		{
			var transactionDetail = _db.Transactions.AsQueryable();
			transactionDetail = transactionDetail.Where(p => p.IdPost == id);

			var result = transactionDetail.Select(t => new TransactionVM
			{
				CodeTransaction = t.CodeTransaction ?? string.Empty,
				Date = t.Date ?? DateTime.Now,
				IdUserBuyer = t.IdUserBuyer ?? string.Empty,
				Quantity = t.Quantity ?? 1,
				Amount = t.Amount ?? 0,
			}).OrderByDescending(o => o.IdTransaction);
			return View(result);
		}
	}	
}
