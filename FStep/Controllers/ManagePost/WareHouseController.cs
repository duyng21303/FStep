using AutoMapper;
using FStep.Data;
using FStep.ViewModels;
using FStep.ViewModels.WareHouse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using X.PagedList;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using FStep.Helpers;

namespace FStep.Controllers.ManagePost
{
    public class WareHouseController : Controller
    {
        private readonly FstepDBContext _db;
	public class WareHouseController : Controller
	{
		private readonly FstepDBContext db;
		private readonly IMapper _mapper;

        public WareHouseController(FstepDBContext context)
        {
            _db = context;
        }
		public WareHouseController(FstepDBContext context, IMapper mapper)
		{
			db = context;
			_mapper = mapper;
		}

        public IActionResult WareHouse(int? page, string searchString, string activeTab = "exchange")
        {
            int pageSize = 20;
            int pageNumber = page ?? 1;
		[HttpGet]
		public IActionResult WareHouse(int? query, int? page)
		{
			int pageSize = 30;
			int pageNumber = page ?? 1;

            try
            {
                // Base query for transactions
                var listTransactions = _db.Transactions
                    .Include(t => t.IdPostNavigation)
                    .Include(t => t.IdUserBuyerNavigation)
                    .Where(t => t.CodeTransaction != null);
			// Start with transactions
			var ListTransaction = db.Transactions.AsQueryable();

                // Create separate queries for Exchange and Sale
                var exchangeTransactions = listTransactions.Where(t => t.Type == "Exchange");
                var saleTransactions = listTransactions.Where(t => t.Type == "Sale");
			// Filter out transactions with null CodeTransaction
			ListTransaction = ListTransaction.Where(p => p.CodeTransaction != null);

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
			// Apply additional filtering if query parameter is provided
			if (query.HasValue && query != 0)
			{
				ListTransaction = ListTransaction.Where(p => p.IdPost == query.Value);
			}
			var transactionList = ListTransaction.OrderByDescending(p => p.Date).ToList(); // Load data into memory
															// Create a list to hold the projected ViewModel results
			List<WareHouseVM> result = new List<WareHouseVM>();

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
			// Project to ViewModel
			foreach (var item in transactionList)
			{
				var post = db.Posts.SingleOrDefault(post => post.IdPost == item.IdPost);
				var comment = db.Comments.SingleOrDefault(comment => comment.IdComment == item.IdComment);
				var userBuyer = db.Users.SingleOrDefault(user => user.IdUser == item.IdUserBuyer);
				var userSeller = db.Users.SingleOrDefault(user => user.IdUser == item.IdUserSeller);

				if (post == null)
				{
					continue; // Skip if post is not found
				}

				var postVM = new PostVM
				{
					Type = post.Type,
					Img = post.Img,
					IdUser = post.IdUser,
					CreateDate = post.Date,
					Title = post.Content,
					DetailProduct = post.Detail,
					FeedbackNum = post.Feedbacks.Count
				};

				if (item.Type != "Sale")
				{
					if (comment == null)
					{
						continue; // Skip if comment is not found
					}

					var commentExchangeVM = new CommentExchangeVM
					{
						Content = comment.Content,
						IdPost = comment.IdPost.ToString(), // Convert int to string
						IdUser = comment.IdUser,
						Img = comment.Img,
						Type = comment.Type
					};

					result.Add(new WareHouseVM
					{
						CommentExchangeVM = commentExchangeVM,
						PostVM = postVM,
						TransactionVM = _mapper.Map<TransactionVM>(item),
						Type = item.Type,
						UserBuyer = userBuyer,
						UserSeller = userSeller
					});
				}
				else
				{
					result.Add(new WareHouseVM
					{
						TransactionVM = _mapper.Map<TransactionVM>(item),
						Type = item.Type,
						PostVM = postVM,
						UserBuyer = userBuyer,
						UserSeller = userSeller
					});
				}
			}

                // Calculate counts for different statuses
                viewModel.ProcessCount = listTransactions.Count(t => t.Status == "Processing");
                viewModel.FinishCount = listTransactions.Count(t => t.Status == "Finished");
                viewModel.CancelCount = listTransactions.Count(t => t.Status == "Cancel");
			// Pagination using PagedList (assuming you have this implementation)
			var pageList = result.ToPagedList(pageNumber, pageSize);

                // Pass query parameters, searchString, and activeTab to view
                ViewBag.SearchString = searchString;
                ViewBag.ActiveTab = activeTab;
			// Pass query parameter to view
			ViewBag.Query = query;

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
		[HttpPost]
		public IActionResult DetailTransaction([FromBody] JsonElement data)
		{
			if (data.ValueKind == JsonValueKind.Object && data.TryGetProperty("id", out JsonElement idElement) && idElement.ValueKind == JsonValueKind.String && data.TryGetProperty("type", out JsonElement typeElement) && idElement.ValueKind == JsonValueKind.String)
			{
				string id = idElement.GetString();
				string type = typeElement.GetString();
                var transaction = db.Transactions.SingleOrDefault(trans => trans.IdTransaction == int.Parse(id));

				if(transaction != null)
				{
                    var post = db.Posts.SingleOrDefault(post => post.IdPost == transaction.IdPost);
                    // Retrieve the model/data needed for the partial view using the id
                    var userBuyer = db.Users.SingleOrDefault(user => user.IdUser == transaction.IdUserBuyer);
                    var userSeller = db.Users.SingleOrDefault(user => user.IdUser == transaction.IdUserSeller);
					userBuyer.AvatarImg = Util.ConvertImgUser(userBuyer);
                    userSeller.AvatarImg = Util.ConvertImgUser(userSeller);
					if (type == "Exchange")
					{
						var comment = db.Comments.SingleOrDefault(comment => comment.IdComment == transaction.IdComment);
						var postVM = new PostVM
						{
							Type = post.Type,
							Img = post.Img,
							IdUser = post.IdUser,
							CreateDate = post.Date,
							Title = post.Content,
							DetailProduct = post.Detail,
							FeedbackNum = post.Feedbacks.Count
						};
						var commentExchangeVM = new CommentExchangeVM
						{
							Content = comment.Content,
							IdPost = comment.IdPost.ToString(), // Convert int to string
							IdUser = comment.IdUser,
							Img = comment.Img,
							Type = comment.Type
						};
						WareHouseVM wareHouse = new WareHouseVM()
						{
							CommentExchangeVM = commentExchangeVM,
							PostVM = postVM,
							TransactionVM = _mapper.Map<TransactionVM>(transaction),
							Type = transaction.Type,
							UserBuyer = userBuyer,
							UserSeller = userSeller
						};
						return PartialView("_DetailTransactionExchange", wareHouse); // Return a partial view with the model
					}
					else
					{
						var postVM = new PostVM
						{
							Type = post.Type,
							Img = post.Img,
							IdUser = post.IdUser,
							CreateDate = post.Date,
							Title = post.Content,
							DetailProduct = post.Detail,
							FeedbackNum = post.Feedbacks.Count
						};
						WareHouseVM wareHouse = new WareHouseVM()
						{
							PostVM = postVM,
							TransactionVM = _mapper.Map<TransactionVM>(transaction),
							Type = transaction.Type,
							UserBuyer = userBuyer,
							UserSeller = userSeller
						};
						return PartialView("_DetailTransactionSale", wareHouse); // Return a partial view with the model
					}
                }
				return Redirect("/");
			}
			else
			{
				return BadRequest("Invalid data");
			}
		}

		
	}
}