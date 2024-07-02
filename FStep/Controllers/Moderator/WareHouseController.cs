﻿using AutoMapper;
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

namespace FStep.Controllers.ManagePost
{
	public class WareHouseController : Controller
	{
		private readonly FstepDBContext db;
		private readonly IMapper _mapper;

		public WareHouseController(FstepDBContext context, IMapper mapper)
		{
			db = context;
			_mapper = mapper;
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
				var viewModel = new WareHouseServiceVM();

				List<WareHouseVM> exchangeList = new List<WareHouseVM>();
				foreach (var item in exchangeTransactions)
				{
					var post = db.Posts.SingleOrDefault(post => post.IdPost == item.IdPost);
					var comment = db.Comments.SingleOrDefault(comment => comment.IdComment == item.IdComment);
					var userBuyer = db.Users.SingleOrDefault(user => user.IdUser == item.IdUserBuyer);
					var userSeller = db.Users.SingleOrDefault(user => user.IdUser == item.IdUserSeller);
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
					exchangeList.Add(new WareHouseVM()
					{
						CommentExchangeVM = commentExchangeVM,
						PostVM = postVM,
						TransactionVM = _mapper.Map<TransactionVM>(item),
						Type = item.Type,
						UserBuyer = userBuyer,
						UserSeller = userSeller
					});
				}
				viewModel.ExchangeList = exchangeList.ToPagedList(pageNumber, pageSize);
				List<WareHouseVM> saleList = new List<WareHouseVM>();
				foreach (var item in saleTransactions)
				{
					var post = db.Posts.SingleOrDefault(post => post.IdPost == item.IdPost);
					var userBuyer = db.Users.SingleOrDefault(user => user.IdUser == item.IdUserBuyer);
					var userSeller = db.Users.SingleOrDefault(user => user.IdUser == item.IdUserSeller);
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
					saleList.Add(new WareHouseVM()
					{
						PostVM = postVM,
						TransactionVM = _mapper.Map<TransactionVM>(item),
						Type = item.Type,
						UserBuyer = userBuyer,
						UserSeller = userSeller
					});
				}
				viewModel.SaleList = saleList.ToPagedList(pageNumber, pageSize);
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
				return View(new WareHouseServiceVM()); // Return an empty view model in case of error
			}
		}
		[HttpGet]
		public IActionResult CompleteTransaction(string code)
		{
			var transaction = db.Transactions.FirstOrDefault(p => p.IdTransaction == int.Parse(code));
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

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> RecieveImg(IFormFile img, string type, string id)
		{
			try
			{
				var trans = db.Transactions.SingleOrDefault(trans => trans.IdTransaction == int.Parse(id));
				if (type == "Seller")
				{
					if (img != null)
					{
						FileInfo fileInfo = new FileInfo("wwwroot/img/userAvar/" + trans.SentImg);
						if (fileInfo.Exists)
						{
							fileInfo.Delete();
						}
						trans.SentImg = Util.UpLoadImg(img, "userAvar");
					}
				}
				else
				{
					if (img != null)
					{
						FileInfo fileInfo = new FileInfo("wwwroot/img/userAvar/" + trans.RecieveImg);
						if (fileInfo.Exists)
						{
							fileInfo.Delete();
						}
						trans.RecieveImg = Util.UpLoadImg(img, "userAvar");
					}
				}
				db.Update(trans);
				db.SaveChanges();
				return RedirectToAction("Profile");
			}
			catch (Exception ex) { }
			return View();
		}
	}
}