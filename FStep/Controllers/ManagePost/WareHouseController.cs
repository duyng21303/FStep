using FStep.Data;
using FStep.ViewModels.WareHouse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace FStep.Controllers.ManagePost
{
    public class WareHouseController : Controller
    {
        private readonly FstepDBContext db;

        public WareHouseController(FstepDBContext context) => db = context;

        [HttpGet]
        public IActionResult WareHouse(int? query, int? page)
        {
            int pageSize = 30;
            int pageNumber = page ?? 1;

            // Start with transactions
            var ListTransaction = db.Transactions.AsQueryable();

            // Filter out transactions with null CodeTransaction
            ListTransaction = ListTransaction.Where(p => p.CodeTransaction != null);

            // Apply additional filtering if query parameter is provided
            if (query.HasValue && query != 0)
            {
                ListTransaction = ListTransaction.Where(p => p.IdPost == query.Value);
            }

            // Project to ViewModel
            var result = ListTransaction.Select(s => new WareHouseVM
            {
                IdPost = s.IdPost,
                Location = s.IdPostNavigation.Location ?? string.Empty,
                CodeTransaction = s.CodeTransaction ?? string.Empty,
                Date = s.Date.HasValue ? s.Date.Value : DateTime.Now,
                Status = s.Status,
                IdBuyer = s.IdUserBuyer,
                IdSeller = s.IdUserSeller,
                IdStudentBuyer = db.Users.SingleOrDefault(p => p.IdUser == s.IdUserBuyer).StudentId ?? string.Empty,
                IdStudentSeller = db.Users.SingleOrDefault(p => p.IdUser == s.IdUserSeller).StudentId ?? string.Empty,
                NameProduct = s.IdPostNavigation.Content ?? string.Empty,
                Quantity = s.Quantity ?? 0,
                Amount = s.Amount ?? 0,
            }).ToList(); // Execute query and materialize results

            // Pagination using PagedList (assuming you have this implementation)
            var pageList = result.ToPagedList(pageNumber, pageSize);

            // Pass query parameter to view
            ViewBag.Query = query;

            return View(pageList);
        }
        
        [HttpGet]
		public IActionResult CompleteTransaction(string code)
		{
			var transaction = db.Transactions.FirstOrDefault(p => p.CodeTransaction == code);
			transaction.Status = "Completed";
			db.Update(transaction);
			db.SaveChanges();

			var payment = new Payment();
			payment.IdTransaction = transaction.IdTransaction;
			payment.PayTime = DateTime.Now;
			payment.Amount = transaction.Amount;
			payment.Type = "Seller";
			db.Add(payment);
			db.SaveChanges();

			return RedirectToAction("WareHouse");
		}

        public IActionResult UpdateLocation(string code, string location)
        {
            return RedirectToAction("WareHouse");
        }
	}
}
