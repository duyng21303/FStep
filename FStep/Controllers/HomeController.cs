using AutoMapper;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Storage;
using FStep.Data;
using FStep.Helpers;
using FStep.Models;
using FStep.ViewModels;
using FStep.ViewModels.WareHouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Protocol.Plugins;
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Net;
using X.PagedList;

namespace FStep.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly FstepDBContext db;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;

		public HomeController(FstepDBContext context, IMapper mapper, IConfiguration configuration)
		{
			db = context;
			_mapper = mapper;
			_configuration = configuration;
		}

		public IActionResult Index(String? query, int suggestedPage = 1, int highRatedPage = 1)
		{
			if (User.IsInRole("Moderator"))
			{
				return Redirect("/ModeManagePost/ManagePosts");
			}
			else if (User.IsInRole("Admin"))
			{
				return Redirect("/Admin/Index");
			}
			int pageSize = 15; // số lượng sản phẩm mỗi trang 
			var ExchangePost = db.Posts
				.Include(t => t.IdUserNavigation)
				.AsQueryable();
			ExchangePost = ExchangePost.Where(p => p.Type == "Exchange" && p.Status == "True");    //check exchangePost là những post thuộc type "exhcange" và có status = 1

			if (!string.IsNullOrEmpty(query))
			{
				ExchangePost = ExchangePost.Where(p => p.Content.Contains(query));
			}
			var suggestedPosts = ExchangePost.Select(s => new PostVM
			{
				IdProduct = s.IdProduct,
				IdPost = s.IdPost,
				Title = s.Content,
				Description = s.Detail,
				PointRating = s.IdUserNavigation.PointRating,
				Img = s.Img,
				NameBoss = s.IdUserNavigation.Name,
				CreateDate = s.Date.HasValue ? s.Date.Value : DateTime.Now
			})?.OrderByDescending(o => o.IdPost);

			var suggestedPostList = suggestedPosts.ToPagedList(suggestedPage, pageSize);

			//hight rating
			var highRatedPosts = suggestedPosts
				.Where(p => p.PointRating > 50)
				.OrderBy(p => Guid.NewGuid())
				.Take(8);

			var highRatedPostList = highRatedPosts.ToPagedList(highRatedPage, pageSize);

			ViewBag.Query = query;
			string checkInfo;
			string id = User.FindFirst("UserID")?.Value;
			if (id != null)
			{
				var user = db.Users.FirstOrDefault(p => p.IdUser == id);
				checkInfo = (user?.StudentId != null /*|| user.BankAccountNumber != null || user.BankName != null*/).ToString();
			}
			else
			{
				checkInfo = "notLogin";
			}
			ViewBag.checkInfo = checkInfo;

			var model = new PostVM
			{
				SuggestedPosts = suggestedPostList,
				highRatedPostList = highRatedPostList
			};



			return View(model);
		}

		public IActionResult Sale(String? query, int suggestedPage = 1, int highRatedPage = 1)
		{
			var pageSize = 15;
			var SalePost = db.Posts
				.Include(t => t.IdUserNavigation)
				.AsQueryable();
			SalePost = SalePost.Where(p => p.Type == "Sale" && p.Status == "True");

			if (!string.IsNullOrEmpty(query))
			{
				SalePost = SalePost.Where(p => p.Content.Contains(query));
			}

			var suggestedPosts = SalePost.Select(s => new PostVM
			{
				IdProduct = s.IdProduct,
				IdPost = s.IdPost,
				Title = s.Content,
				Description = s.Detail,
				PointRating = s.IdUserNavigation.PointRating,
				Img = s.Img,
				NameBoss = s.IdUserNavigation.Name,
				CreateDate = s.Date.HasValue ? s.Date.Value : DateTime.Now,
				Price = s.IdProductNavigation.Price
			}).OrderByDescending(o => o.IdPost);

			var suggestedPostList = suggestedPosts.ToPagedList(suggestedPage, pageSize);

			//hight rating
			var highRatedPosts = suggestedPosts
				.Where(p => p.PointRating > 50)
				.OrderBy(p => Guid.NewGuid())
			.Take(8);

			var highRatedPostList = highRatedPosts.ToPagedList(highRatedPage, pageSize);

			ViewBag.Query = query;
			string checkInfo;
			string id = User.FindFirst("UserID")?.Value;
			if (id != null)
			{
				var user = db.Users.FirstOrDefault(p => p.IdUser == id);
				checkInfo = (user?.StudentId != null /*&& user.BankAccountNumber != null && user.BankName != null*/).ToString();
			}
			else
			{
				checkInfo = "notLogin";
			}
			ViewBag.checkInfo = checkInfo;
			var model = new PostVM
			{
				SuggestedPosts = suggestedPostList,
				highRatedPostList = highRatedPostList
			};

			return View(model);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			var status = (int)HttpStatusCode.InternalServerError;
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, StatusCode = ((int)HttpStatusCode.InternalServerError).ToString() });
		}
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Create(PostVM model, IFormFile img)
		{
			try
			{
				//var fileUpload = img;
				//FileStream fs;

				var product = _mapper.Map<Product>(model);
				product.Quantity = 1;
				if (model.Type == "Sale" && (model.Price == null || model.Price < 10_000 || model.Price > 9_000_000))
				{
					return View("FailToCreate");
				}
				product.Status = "True";
				db.Add(product);
				db.SaveChanges();

				var post = _mapper.Map<Post>(model);
				post.Content = model.Title;
				post.Date = DateTime.Now;
				//Helpers.Util.UpLoadImg(model.Img, "")
				post.Img = Util.UpLoadImg(img, "postPic");

				// Upload file to firebase
				//if (fileUpload.Length > 0)
				//{
				//	string foldername = "firebaseImg";
				//	string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", $"img/{foldername}");

				//	if (Directory.Exists(path))
				//	{
				//		fs = new FileStream(Path.Combine(path, fileUpload.FileName), FileMode.Open);

				//	}
				//	else
				//	{
				//		Directory.CreateDirectory(path);
				//	}

				//	//Firebase uploading stuffs
				//	FirebaseAuthProvider auth;

				//	var cancellation = new CancellationTokenSource();

				//	var upload = new FirebaseStorage(
				//		_configuration["Firebase:StorageBucket"],
				//		new FirebaseStorageOptions
				//		{
				//			ThrowOnCancel = true
				//		}
				//		)
				//		.Child("assets")
				//		.Child($"{fileUpload}.{Path.GetExtension(fileUpload.FileName).Substring(1)}");
				//		//.PutAsync(fs, cancellation.Token);
				//}

				post.Status = "Waiting";
				post.Type = model.Type;
				post.Detail = model.Description;
				post.IdUser = User.FindFirst("UserID").Value;
				//get IdProduct from database map to Post
				post.IdProduct = db.Products.Max(p => p.IdProduct);
				db.Add(post);
				db.SaveChanges();
				return Redirect("/");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				ModelState.AddModelError("Error", "Đã xảy ra một số lỗi khi phản hồi yêu cầu của bạn");
				return RedirectToAction("Error", "Home");
			}
		}

		[Authorize]
		[HttpGet]
		public IActionResult TransactionHistory(string? query, int? page, string activeTab = "exchange")
		{
			int pageSize = 12; // số lượng sản phẩm mỗi trang 
			int pageNumber = (page ?? 1);   // số trang hiện tại, mặc định là trang 1 nếu ko có page được chỉ định
			try
			{
				// Base query for transactions
				var transaction = db.Transactions.AsQueryable();
				transaction = transaction.Where(p => p.IdUserBuyer == User.FindFirst("UserID").Value);

				var exchangeTransactions = transaction.Where(t => t.Type == "Exchange");
				var saleTransactions = transaction.Where(t => t.Type == "Sale");

				//search operating
				if (!string.IsNullOrEmpty(query))
				{
					transaction = transaction.Where(p => p.IdPostNavigation.Content.Contains(query));
					if (activeTab == "exchange")
					{
						query = query.ToLower();
						exchangeTransactions = exchangeTransactions.Where(t =>
							t.CodeTransaction.ToLower().Contains(query) ||
							t.IdPostNavigation.Content.ToLower().Contains(query)
						);
					}
					else
					{
						saleTransactions = saleTransactions.Where(t =>
							t.IdPostNavigation.Location.ToLower().Contains(query) ||
							t.IdPostNavigation.Content.ToLower().Contains(query)
						);
					}
				}

				var viewResult = new TransactionServiceVM();

				var exchangeList = exchangeTransactions.Select(s => new TransactionVM
				{
					TransactionId = s.IdTransaction,
					Transaction = s,
					Post = db.Posts.FirstOrDefault(p => p.IdPost == s.IdPost),
					UserBuyer = db.Users.FirstOrDefault(p => p.IdUser == s.IdUserBuyer),
					UserSeller = db.Users.FirstOrDefault(p => p.IdUser == s.IdUserSeller),
					CommentExchangeVM = new CommentExchangeVM()
					{
						Content = db.Comments.SingleOrDefault(comment => comment.IdComment == s.IdComment).Content,
						IdPost = db.Comments.SingleOrDefault(comment => comment.IdComment == s.IdComment).IdPost.ToString(),
						IdUser = db.Comments.SingleOrDefault(comment => comment.IdComment == s.IdComment).IdUser,
						Img = db.Comments.SingleOrDefault(comment => comment.IdComment == s.IdComment).Img,
						Type = db.Comments.SingleOrDefault(comment => comment.IdComment == s.IdComment).Type
					},
					CreateDate = s.Date,
					DeliveryDate = db.Payments.FirstOrDefault(p => p.IdTransaction == s.IdTransaction && p.Type == "Seller").PayTime,
					CancelDate = db.Payments.FirstOrDefault(p => p.IdTransaction == s.IdTransaction).CancelDate,
					CheckFeedback = db.Feedbacks.Any(p => p.IdPost == s.IdPost),
				}).OrderByDescending(o => o.TransactionId);
				viewResult.ExchangeList = exchangeList.ToPagedList(pageNumber, pageSize);

				var saleList = saleTransactions.Select(s => new TransactionVM
				{
					TransactionId = s.IdTransaction,
					Transaction = s,
					Post = db.Posts.FirstOrDefault(p => p.IdPost == s.IdPost),
					UserBuyer = db.Users.FirstOrDefault(p => p.IdUser == s.IdUserBuyer),
					UserSeller = db.Users.FirstOrDefault(p => p.IdUser == s.IdUserSeller),
					CreateDate = s.Date,
					DeliveryDate = db.Payments.FirstOrDefault(p => p.IdTransaction == s.IdTransaction && p.Type == "Seller").PayTime,
					CancelDate = db.Payments.FirstOrDefault(p => p.IdTransaction == s.IdTransaction).CancelDate,
					CheckFeedback = db.Feedbacks.Any(p => p.IdPost == s.IdPost),
				}).OrderByDescending(o => o.TransactionId);
				viewResult.SaleList = saleList.ToPagedList(pageNumber, pageSize);



				ViewBag.Query = query;
				ViewBag.ActiveTab = activeTab;

				return View(viewResult);
			}
			catch (Exception ex)
			{
				Console.WriteLine("An error occurred: " + ex.Message);
				Console.WriteLine("Stack Trace: " + ex.StackTrace);
				return View("Error", "Home");
			}
		}
		[Authorize]
		[HttpGet]
		public IActionResult OrderHistory(string? query, int? page, string activeTab = "exchange")
		{
			int pageSize = 12; // số lượng sản phẩm mỗi trang 
			int pageNumber = (page ?? 1);   // số trang hiện tại, mặc định là trang 1 nếu ko có page được chỉ định
			try
			{
				// Base query for transactions
				var transaction = db.Transactions.AsQueryable();
				transaction = transaction.Where(p => p.IdUserSeller == User.FindFirst("UserID").Value);

				var exchangeTransactions = transaction.Where(t => t.Type == "Exchange");
				var saleTransactions = transaction.Where(t => t.Type == "Sale");

				//search operating
				if (!string.IsNullOrEmpty(query))
				{
					transaction = transaction.Where(p => p.IdPostNavigation.Content.Contains(query));
					if (activeTab == "exchange")
					{

						query = query.ToLower();
						exchangeTransactions = exchangeTransactions.Where(t =>
							t.CodeTransaction.ToLower().Contains(query) ||
							t.IdPostNavigation.Content.ToLower().Contains(query)
						);
					}
					else
					{
						saleTransactions = saleTransactions.Where(t =>
							t.IdPostNavigation.Location.ToLower().Contains(query) ||
							t.IdPostNavigation.Content.ToLower().Contains(query)
						);
					}
				}

				var viewResult = new TransactionServiceVM();

				var exchangeList = exchangeTransactions.Select(s => new TransactionVM
				{
					TransactionId = s.IdTransaction,
					Transaction = s,
					Post = db.Posts.FirstOrDefault(p => p.IdPost == s.IdPost),
					UserBuyer = db.Users.FirstOrDefault(p => p.IdUser == s.IdUserBuyer),
					UserSeller = db.Users.FirstOrDefault(p => p.IdUser == s.IdUserSeller),
					CommentExchangeVM = new CommentExchangeVM()
					{
						Content = db.Comments.SingleOrDefault(comment => comment.IdComment == s.IdComment).Content,
						IdPost = db.Comments.SingleOrDefault(comment => comment.IdComment == s.IdComment).IdPost.ToString(),
						IdUser = db.Comments.SingleOrDefault(comment => comment.IdComment == s.IdComment).IdUser,
						Img = db.Comments.SingleOrDefault(comment => comment.IdComment == s.IdComment).Img,
						Type = db.Comments.SingleOrDefault(comment => comment.IdComment == s.IdComment).Type
					},
					CreateDate = s.Date,
					DeliveryDate = db.Payments.FirstOrDefault(p => p.IdTransaction == s.IdTransaction && p.Type == "Seller").PayTime,
					CancelDate = db.Payments.FirstOrDefault(p => p.IdTransaction == s.IdTransaction).CancelDate,
					CheckFeedback = db.Feedbacks.Any(p => p.IdPost == s.IdPost),
				}).OrderByDescending(o => o.TransactionId);
				viewResult.ExchangeList = exchangeList.ToPagedList(pageNumber, pageSize);

				var saleList = saleTransactions.Select(s => new TransactionVM
				{
					TransactionId = s.IdTransaction,
					Transaction = s,
					Post = db.Posts.FirstOrDefault(p => p.IdPost == s.IdPost),
					UserBuyer = db.Users.FirstOrDefault(p => p.IdUser == s.IdUserBuyer),
					UserSeller = db.Users.FirstOrDefault(p => p.IdUser == s.IdUserSeller),
					CreateDate = s.Date,
					DeliveryDate = db.Payments.FirstOrDefault(p => p.IdTransaction == s.IdTransaction && p.Type == "Seller").PayTime,
					CancelDate = db.Payments.FirstOrDefault(p => p.IdTransaction == s.IdTransaction).CancelDate,
					CheckFeedback = db.Feedbacks.Any(p => p.IdPost == s.IdPost),
				}).OrderByDescending(o => o.TransactionId);
				viewResult.SaleList = saleList.ToPagedList(pageNumber, pageSize);



				ViewBag.Query = query;
				ViewBag.ActiveTab = activeTab;

				return View(viewResult);
			}
			catch (Exception ex)
			{
				Console.WriteLine("An error occurred: " + ex.Message);
				Console.WriteLine("Stack Trace: " + ex.StackTrace);
				return View("Error", "Home");
			}
		}
		[Authorize]
		[HttpGet]
		public ActionResult TransactionSaleDetail(int id)
		{
			var transaction = db.Transactions.FirstOrDefault(p => p.IdTransaction == id);
			var post = db.Posts.FirstOrDefault(p => p.IdPost == transaction.IdPost);
			var seller = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserSeller);
			var buyer = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserBuyer);
			return View(new TransactionVM()
			{
				TransactionId = id,
				Transaction = transaction,
				Post = post,
				UserBuyer = buyer,
				UserSeller = seller,
				DeliveryDate = db.Payments.FirstOrDefault(p => p.IdTransaction == id && p.Type == "Seller")?.PayTime,
				CreateDate = transaction.Date,
				CancelDate = db.Payments.FirstOrDefault(p => p.IdTransaction == id).CancelDate,
				CheckFeedback = db.Feedbacks.Any(p => p.IdPost == transaction.IdPost)
			});
		}
		[Authorize]
		[HttpGet]
		public ActionResult TransactionExchangeDetail(int id)
		{
			var transaction = db.Transactions.FirstOrDefault(p => p.IdTransaction == id);
			var post = db.Posts.FirstOrDefault(p => p.IdPost == transaction.IdPost);
			var comment = db.Comments.SingleOrDefault(comment => comment.IdComment == transaction.IdComment);
			var seller = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserSeller);
			var buyer = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserBuyer);

			return View(new TransactionVM()
			{
				TransactionId = id,
				Transaction = transaction,
				Post = post,
				UserBuyer = buyer,
				UserSeller = seller,
				CommentExchangeVM = new CommentExchangeVM()
				{
					Content = comment.Content,
					IdPost = comment.IdPost.ToString(), // Convert int to string
					IdUser = comment.IdUser,
					Img = comment.Img,
					Type = comment.Type
				},
				CreateDate = transaction.Date,
				DeliveryDate = db.Payments.FirstOrDefault(p => p.IdTransaction == id && p.Type == "Seller")?.PayTime,
				CancelDate = db.Payments.FirstOrDefault(p => p.IdTransaction == id)?.CancelDate,
				CheckFeedback = db.Feedbacks.Any(p => p.IdPost == transaction.IdPost)
			});
		}
	}
}