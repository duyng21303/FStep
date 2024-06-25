using FStep.Data;
using FStep.ViewModels.WareHouse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Linq;

namespace FStep.Controllers.ManagePost
{
    public class WareHouseController : Controller
    {
        private readonly FstepDbContext _db;

        public WareHouseController(FstepDbContext context)
        {
            _db = context;
        }

        public IActionResult WareHouse(int? page, string searchString, string activeTab = "exchange")
        {
            int pageSize = 20;
            int pageNumber = page ?? 1;

            try
            {
                // Base query for transactions
                var listTransactions = _db.Transactions
                    .Include(t => t.IdPostNavigation)
                    .Include(t => t.IdUserBuyerNavigation)
                    .Where(t => t.CodeTransaction != null);

                // Create separate queries for Exchange and Sale
                var exchangeTransactions = listTransactions.Where(t => t.Type == "Exchange");
                var saleTransactions = listTransactions.Where(t => t.Type == "Sale");

                // Apply search filter if searchString is provided
                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();
                    exchangeTransactions = exchangeTransactions.Where(t =>
                        t.IdPostNavigation.Location.ToLower().Contains(searchString) ||
                        t.CodeTransaction.ToLower().Contains(searchString) ||
                        t.IdUserBuyerNavigation.StudentId.ToLower().Contains(searchString) ||
                        t.IdPostNavigation.Content.ToLower().Contains(searchString)
                    );

                    saleTransactions = saleTransactions.Where(t =>
                        t.IdPostNavigation.Location.ToLower().Contains(searchString) ||
                        t.CodeTransaction.ToLower().Contains(searchString) ||
                        t.IdUserBuyerNavigation.StudentId.ToLower().Contains(searchString) ||
                        t.IdPostNavigation.Content.ToLower().Contains(searchString)
                    );
                }

                // Project to ViewModels
                var viewModel = new WareHouseVM();

                viewModel.ExchangeList = exchangeTransactions
                    .Select(t => new WareHouseTransactionVM
                    {
                        IdPost = t.IdPost,
                        Location = t.IdPostNavigation.Location ?? string.Empty,
                        CodeTransaction = t.CodeTransaction ?? string.Empty,
                        Date = t.Date ?? DateTime.Now,
                        NameProduct = t.IdPostNavigation.Content ?? string.Empty,
                        Quantity = t.Quantity ?? 0,
                        Amount = t.Amount ?? 0
                    }).ToPagedList(pageNumber, pageSize);

                viewModel.SaleList = saleTransactions
                    .Select(t => new WareHouseTransactionVM
                    {
                        IdPost = t.IdPost,
                        Location = t.IdPostNavigation.Location ?? string.Empty,
                        CodeTransaction = t.CodeTransaction ?? string.Empty,
                        Date = t.Date ?? DateTime.Now,
                        IdUserBuyer = t.IdUserBuyerNavigation.StudentId ?? string.Empty,
                        NameProduct = t.IdPostNavigation.Content ?? string.Empty,
                        Quantity = t.Quantity ?? 0,
                        Amount = t.Amount ?? 0
                    }).ToPagedList(pageNumber, pageSize);

                // Calculate counts for different statuses
                viewModel.ProcessCount = listTransactions.Count(t => t.Status == "Processing");
                viewModel.FinishCount = listTransactions.Count(t => t.Status == "Finished");
                viewModel.CancelCount = listTransactions.Count(t => t.Status == "Cancel");

                // Pass query parameters, searchString, and activeTab to view
                ViewBag.SearchString = searchString;
                ViewBag.ActiveTab = activeTab;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine("An error occurred: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                return View(new WareHouseVM()); // Return an empty view model in case of error
            }
        }
    }
}