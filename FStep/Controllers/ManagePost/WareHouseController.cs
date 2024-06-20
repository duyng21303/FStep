using FStep.Data;
using FStep.ViewModels.WareHouse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace FStep.Controllers.ManagePost
{
    public class WareHouseController : Controller
    {
        private readonly FstepDbContext db;

        public WareHouseController(FstepDbContext context) => db = context;
        public IActionResult WareHouse(int? query, int? page)
        {
            int pageSize = 30;
            int pageNumber = page ?? 1;

            // Start with transactions
            var ListProduct = db.Transactions.AsQueryable();

            // Filter out transactions with null CodeTransaction
            ListProduct = ListProduct.Where(p => p.CodeTransaction != null);

            // Apply additional filtering if query parameter is provided
            if (query.HasValue && query != 0)
            {
                ListProduct = ListProduct.Where(p => p.IdPost == query.Value);
            }

            // Project to ViewModel
            var result = ListProduct.Select(s => new WareHouseVM
            {
                IdPost = s.IdPost,
                Location = s.IdPostNavigation.Location ?? string.Empty,
                CodeTransaction = s.CodeTransaction ?? string.Empty,
                Date = s.Date.HasValue ? s.Date.Value : DateTime.Now,
                IdStudent = db.Users.SingleOrDefault(p => p.IdUser == s.IdUserBuyer).StudentId ?? string.Empty,
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
    }
}
