using AutoMapper;
using FStep.Data;
using FStep.ViewModels;
using FStep.ViewModels.WareHouse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using X.PagedList;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using FStep.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FStep.Services;
using Microsoft.AspNetCore.Http.Extensions;
using System.Configuration;


namespace FStep.Controllers.ManagePost
{
	public class WareHouseController : Controller
	{
		private readonly FstepDBContext db;
		private readonly IMapper _mapper;

		private readonly NotificationServices notificationServices;
		private readonly IConfiguration _configuration;

		public WareHouseController(FstepDBContext context, IMapper mapper, IConfiguration configuration)
		{
			db = context;
			_mapper = mapper;
			notificationServices = new NotificationServices(db);
			_configuration = configuration;

		}

		[HttpGet]
		public IActionResult WareHouse(int? page, string searchString, string activeTab = "exchange")
		{
			int pageSize = 20;
			int pageNumber = page ?? 1;

			try
			{
				// Base query for transactions
				var listTransactions = db.Transactions.ToList();

				// Create separate queries for Exchange and Sale
				var exchangeTransactions = listTransactions.Where(t => t.Type == "Exchange");
				var saleTransactions = listTransactions.Where(t => t.Type == "Sale");

				// Apply search filter if searchString is provided
				if (!string.IsNullOrEmpty(searchString))
				{
					searchString = searchString.ToLower();
					exchangeTransactions = exchangeTransactions.Where(t =>
				(t.IdPostNavigation?.Location?.ToLower().Contains(searchString) ?? false) ||
				(t.CodeTransaction?.ToLower().Contains(searchString) ?? false) ||
				(t.IdUserBuyerNavigation?.StudentId?.ToLower().Contains(searchString) ?? false) ||
				(t.IdPostNavigation?.Content?.ToLower().Contains(searchString) ?? false)
					);
					saleTransactions = saleTransactions.Where(t =>
						(t.IdPostNavigation?.Location?.ToLower().Contains(searchString) ?? false) ||
				(t.CodeTransaction?.ToLower().Contains(searchString) ?? false) ||
				(t.IdUserBuyerNavigation?.StudentId?.ToLower().Contains(searchString) ?? false) ||
				(t.IdPostNavigation?.Content?.ToLower().Contains(searchString) ?? false)

					);
				}

				// Project to ViewModels
				var viewModel = new WareHouseServiceVM();

				List<WareHouseVM> exchangeList = new List<WareHouseVM>();
				foreach (var item in exchangeTransactions)
				{
					if (item.Status == "Processing")
					{
						var post = db.Posts.SingleOrDefault(post => post.IdPost == item.IdPost);
						var comment = db.Comments.SingleOrDefault(comment => comment.IdComment == item.IdComment);
						var userBuyer = db.Users.SingleOrDefault(user => user.IdUser == item.IdUserBuyer);
						var userSeller = db.Users.SingleOrDefault(user => user.IdUser == item.IdUserSeller);
						var postVM = new PostVM
						{
							IdPost = post.IdPost,
							Img = post.Img,
							Type = post.Type,
							IdUser = post.IdUser,
							CreateDate = post.Date,
							Title = post.Content,
							Location = post.Location,
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
						exchangeList.Add(new WareHouseVM()
						{
							CommentExchangeVM = commentExchangeVM,
							PostVM = postVM,
							TransactionVM = item,
							Type = item.Type,
							UserBuyer = userBuyer,
							UserSeller = userSeller
						});
					}
				}
				viewModel.ExchangeList = exchangeList.ToPagedList(pageNumber, pageSize);
				List<WareHouseVM> saleList = new List<WareHouseVM>();
				foreach (var item in saleTransactions)
				{
					if (item.Status == "Processing")
					{
						var post = db.Posts.SingleOrDefault(post => post.IdPost == item.IdPost);
						var userBuyer = db.Users.SingleOrDefault(user => user.IdUser == item.IdUserBuyer);
						var userSeller = db.Users.SingleOrDefault(user => user.IdUser == item.IdUserSeller);
						var postVM = new PostVM
						{
							IdPost = post.IdPost,
							Type = post.Type,
							Img = post.Img,
							IdUser = post.IdUser,
							CreateDate = post.Date,
							Title = post.Content,
							DetailProduct = post.Detail,
							Location = post.Location,
							FeedbackNum = post.Feedbacks.Count
						};
						saleList.Add(new WareHouseVM()
						{
							PostVM = postVM,
							TransactionVM = item,
							Type = item.Type,
							UserBuyer = userBuyer,
							UserSeller = userSeller
						});
					}
				}
				viewModel.SaleList = saleList.ToPagedList(pageNumber, pageSize);
				// Calculate counts for different statuses
				viewModel.ProcessCount = listTransactions.Count(t => t.Status == "Processing");
				viewModel.FinishCount = listTransactions.Count(t => t.Status == "Finished");
				viewModel.CancelCount = listTransactions.Count(t => t.Status == "Cancel");
				viewModel.FinishCount = listTransactions.Count(t => t.Status == "Completed");
				viewModel.CancelCount = listTransactions.Count(t => t.Status == "Canceled");

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
				return View(new WareHouseServiceVM()); // Return an empty view model in case of error
			}
		}
		[HttpGet]
		public IActionResult CompleteTransaction(int id, string url)
		{
			string fullUrl = HttpContext.Request.GetDisplayUrl();
			var transaction = db.Transactions.FirstOrDefault(p => p.IdTransaction == id);

			transaction.Status = "Completed";
			db.Update(transaction);
			db.SaveChanges();

			//Pay money for Seller

			//Create Payment
			var payment = new Payment();
			payment.IdTransaction = transaction.IdTransaction;
			payment.PayTime = DateTime.Now;
			payment.Amount = transaction.Amount;
			payment.Status = "True";
			payment.Type = "Seller";
			db.Add(payment);

			var post = db.Posts.FirstOrDefault(p => p.IdPost == transaction.IdPost);
			post.Status = "False";
			db.Update(post);

			var product = db.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct);
			product.Status = "False";
			db.Update(product);
			
			var buyer = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserBuyer);
			var seller = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserSeller);
			var maxPoint = _configuration.GetValue<float>("MaxPoint");
			if(buyer.PointRating < maxPoint)
			{
				buyer.PointRating += 5;
			}
			if(seller.PointRating < maxPoint)
			{
				seller.PointRating += 5;
			}
			db.Update(buyer);
			db.Update(seller);
			db.SaveChanges();

			return RedirectToAction("WareHouse");
			return Redirect(url);
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

				if (transaction != null)
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
							TransactionVM = transaction,
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
							TransactionVM = transaction,
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

		[HttpPost]
		public async Task<IActionResult> RecieveImg(IFormFile img, string type, string id)
		{
			string activeTab = "";
			try
			{
				var trans = db.Transactions.SingleOrDefault(trans => trans.IdTransaction == int.Parse(id));
				var userBuyer = db.Users.SingleOrDefault(user => user.IdUser == trans.IdUserBuyer);
				var userSeller = db.Users.SingleOrDefault(user => user.IdUser == trans.IdUserSeller);

				switch (type)
				{
					case "SellerSent":
						if (img != null)
						{
							if (trans.SentImg != null)

							{
								await notificationServices.CreateNotification(userBuyer.IdUser, "TransactionExchangeAlready", "Transaction", userSeller.Name, trans.IdTransaction);
								await notificationServices.CreateNotification(userSeller.IdUser, "TransactionRecieveGoods", "Transaction", userBuyer.Name, trans.IdTransaction);
							}
							FileInfo fileInfo = new FileInfo("wwwroot/img/postPic/" + trans.SentImg);
							if (fileInfo.Exists)
							{
								fileInfo.Delete();
							}
							trans.SentImg = Util.UpLoadImg(img, "postPic");
							trans.SentSellerDate = DateTime.Now;
							activeTab = "exchange";
						}
						break;

					case "SellerReceive":
						if (img != null)
						{
							if (trans.RecieveImg != null)
							{
								await notificationServices.CreateNotification(userSeller.IdUser, "TransactionExchangeRecieveSuccess", "Transaction", userSeller.Name, trans.IdTransaction);
							}
							FileInfo fileInfo = new FileInfo("wwwroot/img/postPic/" + trans.RecieveImg);
							if (fileInfo.Exists)
							{
								fileInfo.Delete();
							}
							trans.RecieveImg = Util.UpLoadImg(img, "postPic");
							trans.ReceivedSellerDate = DateTime.Now;
							activeTab = "exchange";
						}
						break;

					case "BuyerSent":
						if (img != null)
						{
							if (trans.SentBuyerImg != null)
							{
								await notificationServices.CreateNotification(userSeller.IdUser, "TransactionExchangeAlready", "Transaction", userBuyer.Name, trans.IdTransaction);
								await notificationServices.CreateNotification(userBuyer.IdUser, "TransactionRecieveGoods", "Transaction", userSeller.Name, trans.IdTransaction);
							}
							FileInfo fileInfo = new FileInfo("wwwroot/img/postPic/" + trans.SentBuyerImg);
							if (fileInfo.Exists)
							{
								fileInfo.Delete();
							}
							trans.SentBuyerImg = Util.UpLoadImg(img, "postPic");
							trans.SentBuyerDate = DateTime.Now;
							activeTab = "exchange";
						}
						break;

					case "BuyerReceive":
						if (img != null)
						{
							if (trans.RecieveBuyerImg != null)
							{
								await notificationServices.CreateNotification(userBuyer.IdUser, "TransactionExchangeRecieveSuccess", "Transaction", userBuyer.Name, trans.IdTransaction);
							}
							FileInfo fileInfo = new FileInfo("wwwroot/img/postPic/" + trans.RecieveBuyerImg);
							if (fileInfo.Exists)
							{
								fileInfo.Delete();
							}
							trans.RecieveBuyerImg = Util.UpLoadImg(img, "postPic");
							trans.ReceivedBuyerDate = DateTime.Now;
							activeTab = "exchange";
						}
						break;
					case "SellerSent-Sell":
						if (img != null)
						{
							if (trans.SentImg != null)
							{
								await notificationServices.CreateNotification(userSeller.IdUser, "TransactionSellSent", "Transaction", userSeller.Name, trans.IdTransaction);
							}
							FileInfo fileInfo = new FileInfo("wwwroot/img/postPic/" + trans.SentImg);
							if (fileInfo.Exists)
							{
								fileInfo.Delete();
							}
							trans.SentImg = Util.UpLoadImg(img, "postPic");
							trans.SentSellerDate = DateTime.Now;
							activeTab = "sell";
						}
						break;
					case "SellerRevieve-Sell":
						if (img != null)
						{
							if (trans.RecieveBuyerImg != null)
							{
								await notificationServices.CreateNotification(userBuyer.IdUser, "TransactionSellRecieve", "Transaction", userBuyer.Name, trans.IdTransaction);
							}
							FileInfo fileInfo = new FileInfo("wwwroot/img/postPic/" + trans.RecieveBuyerImg);
							if (fileInfo.Exists)
							{
								fileInfo.Delete();
							}
							trans.RecieveBuyerImg = Util.UpLoadImg(img, "postPic");
							trans.ReceivedBuyerDate = DateTime.Now;
							activeTab = "sell";
						}
						break;
					default:
						break;
				}
				db.Update(trans);
				db.SaveChanges();
				return RedirectToAction("WareHouse", "WareHouse", new { idTrans = id, activeTab = activeTab });
			}
			catch (Exception ex) { }
			return Json(new { transactionId = id });

		}
	}
}