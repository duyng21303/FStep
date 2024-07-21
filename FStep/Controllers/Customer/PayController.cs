using AutoMapper;
using Azure.Identity;
using FStep.Controllers.Auth;
using FStep.Data;
using FStep.Helpers;
using FStep.Repostory.Interface;
using FStep.Services;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Protocol.Plugins;
using System;
using System.Net.Mail;
using System.Text;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace FStep.Controllers.Customer
{
	public class PayController : Controller
	{

		private readonly FstepDBContext db;
		private readonly IMapper _mapper;
		private readonly IEmailSender emailSender;
		private readonly IVnPayService _vnPayService;
		private readonly IRazorViewEngine _viewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<RegistrationController> _logger;

		public PayController(FstepDBContext context, IMapper mapper, IVnPayService vnPayService, IEmailSender emailSender, IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ILogger<RegistrationController> logger)
		{
			db = context;
			_mapper = mapper;
			_vnPayService = vnPayService;
			this.emailSender = emailSender;
			_viewEngine = viewEngine;
			_tempDataProvider = tempDataProvider;
			_serviceProvider = serviceProvider;
			_logger = logger;
		}
		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> AutoCheckTransaction()
		{
			var transaction = db.Transactions.AsQueryable();
			foreach (var x in transaction)
			{
				if (DateTime.Now.CompareTo(x.Date?.AddHours(1)) <= 0 && x.Type == "Exchange" && x.Status == "Processing")
				{
					var buyer = db.Users.SingleOrDefault(p => p.IdUser == x.IdUserBuyer);
					var seller = db.Users.SingleOrDefault(p => p.IdUser == x.IdUserSeller);
					TransactionVM invoice;
					//lấy thông tin transaction
					invoice = GetInvoiceExchangeById(db.Transactions.FirstOrDefault(p => p.CodeTransaction == x.CodeTransaction).IdTransaction);
					string emailBody = await RenderViewToStringAsync($"Invoice{x.Type}", invoice);

					//sent email
					bool sentBuyer = await emailSender.EmailSendAsync(buyer.Email, "Đơn hàng của bạn đã được tạo", emailBody);
					bool sentSeller = await emailSender.EmailSendAsync(seller.Email, "Sản phẩm của của bạn đã được tiến hành giao dịch", emailBody);
				}

				if (DateTime.Now.CompareTo(x.Date?.AddDays(3)) > 0 && x.Status == "Processing")
				{
					x.Status = "Canceled";
					db.Update(x);
					var payment = new Payment();
					payment.CancelDate = DateTime.Now;
					payment.IdTransaction = x.IdTransaction;
					payment.Type = "Seller";
					payment.Note = "Huỷ bởi người đăng, người đăng không giao đơn hàng";
					db.SaveChanges();

					//Refund operating here
					//End refund
					var buyer = db.Users.SingleOrDefault(p => p.IdUser == x.IdUserBuyer);
					var seller = db.Users.SingleOrDefault(p => p.IdUser == x.IdUserSeller);
					TransactionVM invoice;
					//lấy thông tin transaction
					if (x.Type == "Exchange")
					{
						invoice = GetInvoiceExchangeById(db.Transactions.FirstOrDefault(p => p.CodeTransaction == x.CodeTransaction).IdTransaction);
					}
					else
					{
						invoice = GetInvoiceSaleById(db.Transactions.FirstOrDefault(p => p.CodeTransaction == x.CodeTransaction).IdTransaction);
					}
					string emailBody = await RenderViewToStringAsync($"Invoice{x.Type}", invoice);

					//sent email
					bool sentBuyer = await emailSender.EmailSendAsync(buyer.Email, "Đơn hàng của bạn đã được huỷ", emailBody);
					bool sentSeller = await emailSender.EmailSendAsync(seller.Email, "Sản phẩm của bạn đã bị huỷ giao dịch", emailBody);
				}
			}
			return RedirectToAction("Index","Home");

		}

		[Authorize]
		[HttpGet]
		public IActionResult CheckoutSale(int id, int quantity)

		{
			var post = db.Posts.SingleOrDefault(x => x.IdPost == id);
			var checkout = new CheckoutVM();
			checkout.IdPost = id;
			checkout.Title = post.Content;
			checkout.Img = post.Img;
			checkout.IdUserBuyer = User.FindFirst("UserID").Value;
			checkout.IdUserSeller = db.Posts.SingleOrDefault(p => p.IdPost == id).IdUser;
			checkout.ProductId = db.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct).IdProduct;
			checkout.UnitPrice = db.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct).Price;
			checkout.Quantity = 1;
			checkout.Amount = db.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct).Price;

			checkout.Type = post.Type;

			HttpContext.Session.Set<CheckoutVM>("CHECKOUT_INFO", checkout);
			return View("Checkout", checkout);
		}
		[Authorize]
		[HttpPost]
		public IActionResult CheckoutSale(CheckoutVM model)
		{
			CheckoutVM info = HttpContext.Session.Get<CheckoutVM>("CHECKOUT_INFO");
			var vnPayModel = new VnPayRequestModel
			{
				Amount = model.Amount,
				CreatedDate = DateTime.Now,
				Description = "Thanh toan don hang",
				FullName = User.FindFirst("UserID").Value,
				TransactionCode = info.ProductId,
			};

			info.Note = model.Note;
			HttpContext.Session.Set<CheckoutVM>("CHECKOUT_INFO", info);
			return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
		}

		[Authorize]
		public IActionResult PaymentSuccess()
		{
			return View();
		}

		[Authorize]
		public IActionResult PaymentFail()
		{
			return View();
		}

		[Authorize]
		public async Task<IActionResult> PaymentCallBack()
		{
			try
			{
				var response = _vnPayService.PaymentExecute(Request.Query);

				if (response == null || response.VnPayResponseCode != "00")
				{
					TempData["Message"] = $"VnPay fail: {response.VnPayResponseCode}";
					return RedirectToAction("PaymentFail");
				}
				CheckoutVM info = HttpContext.Session.Get<CheckoutVM>("CHECKOUT_INFO");

				var transaction = new FStep.Data.Transaction();
				transaction.Date = DateTime.Now;
				transaction.Status = "Processing";
				transaction.Quantity = info.Quantity;
				transaction.UnitPrice = info.UnitPrice;
				transaction.Amount = float.Parse(response.Amount.ToString()) / 100;

				transaction.IdPost = info.IdPost;
				transaction.IdUserBuyer = info.IdUserBuyer;
				transaction.IdUserSeller = info.IdUserSeller;
				transaction.Type = info.Type;
				transaction.CodeTransaction = response.TransactionCode;
				db.Add(transaction);
				db.SaveChanges();

				var payment = new Payment();
				payment.PayTime = transaction.Date;
				payment.Amount = transaction.Amount;
				payment.VnpayTransactionCode = transaction.CodeTransaction;
				payment.Type = "Buyer";
				payment.Status = "True";
				payment.IdTransaction = db.Transactions.SingleOrDefault(p => p.CodeTransaction == transaction.CodeTransaction).IdTransaction;
				db.Add(payment);
				db.SaveChanges();

				var post = db.Posts.SingleOrDefault(p => p.IdPost == info.IdPost);
				var product = db.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct);
				post.Status = "Trading";
				product.Status = "False";

				db.Update(product);
				db.SaveChanges();

				var buyer = db.Users.SingleOrDefault(p => p.IdUser == info.IdUserBuyer);
				var seller = db.Users.SingleOrDefault(p => p.IdUser == info.IdUserSeller);

				TransactionVM invoice;
				//lấy thông tin transaction
				if (transaction.Type == "Exchange")
				{
					invoice = GetInvoiceExchangeById(db.Transactions.FirstOrDefault(p => p.CodeTransaction == transaction.CodeTransaction).IdTransaction);
				}
				else
				{
					invoice = GetInvoiceSaleById(db.Transactions.FirstOrDefault(p => p.CodeTransaction == transaction.CodeTransaction).IdTransaction);
				}
				string emailBody = await RenderViewToStringAsync($"Invoice{transaction.Type}", invoice);

				//sent email
				bool sentBuyer = await emailSender.EmailSendAsync(buyer.Email, "Đơn hàng của bạn đã được tạo", emailBody);
				bool sentSeller = await emailSender.EmailSendAsync(seller.Email, "Sản phẩm của của bạn đã được tiến hành giao dịch", emailBody);

				if (!sentBuyer || !sentSeller)
				{
					return RedirectToAction("Error", "Home");
				}

				TempData["Message"] = $"VnPay success";
				return RedirectToAction("PaymentSuccess");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Can't send your email ");
				return RedirectToAction("Error", "Home");
			}
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Cancel(CancelVM model)
		{
			try
			{
				var transaction = db.Transactions.SingleOrDefault(p => p.IdTransaction == model.TransactionId);
				transaction.Status = "Canceled";
				transaction.CancelDate = DateTime.Now;
				db.Update(transaction);

				var buyer = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserBuyer);
				buyer.PointRating -= 5;
				db.Update(buyer);

				var seller = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserSeller);
				seller.PointRating -= 5;
				db.Update(seller);

				var post = db.Posts.SingleOrDefault(p => p.IdPost == transaction.IdPost);
				post.Status = "True";
				db.Update(post);

				var product = db.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct);
				product.Status = "True";
				db.Update(product);

				var payment = db.Payments.FirstOrDefault(p => p.IdTransaction == model.TransactionId);
				payment.Status = "False";
				payment.CancelDate = DateTime.Now;
				payment.Note = model.Note;
				db.Update(payment);

				TransactionVM invoice;
				//tạo invoice
				if (transaction.Type == "Exchange")
				{
					invoice = GetInvoiceExchangeById(db.Transactions.FirstOrDefault(p => p.CodeTransaction == transaction.CodeTransaction).IdTransaction);
				}
				else
				{
					invoice = GetInvoiceSaleById(db.Transactions.FirstOrDefault(p => p.CodeTransaction == transaction.CodeTransaction).IdTransaction);
				}
				string emailBody = await RenderViewToStringAsync($"Invoice{transaction.Type}", invoice);

				//sent email
				bool sentSeller = await emailSender.EmailSendAsync(seller.Email, "Sản phẩm của bạn đã bị huỷ giao dịch", emailBody);
				bool sentBuyer = await emailSender.EmailSendAsync(buyer.Email, "Đơn hàng của bạn đã được huỷ", emailBody);
				buyer.PointRating -= 5;
				db.Update(buyer);
				db.SaveChanges();

				return Redirect(model.ReturnUrl);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Can't send your email ");
				return RedirectToAction("Error", "Home");
			}

		}

		public async Task SendExchangeMail(int id)
		{
			try
			{
				var transaction = db.Transactions.FirstOrDefault(p => p.IdTransaction == id);
				var buyer = db.Users.SingleOrDefault(p => p.IdUser == transaction.IdUserBuyer);
				var seller = db.Users.SingleOrDefault(p => p.IdUser == transaction.IdUserSeller);

				TransactionVM invoice;
				//lấy thông tin transaction
				invoice = GetInvoiceExchangeById(id);
				string emailBody = await RenderViewToStringAsync($"Invoice{transaction.Type}", invoice);

				//sent email
				bool sentBuyer = await emailSender.EmailSendAsync(buyer.Email, "Đơn hàng của bạn đã được tạo", emailBody);
				bool sentSeller = await emailSender.EmailSendAsync(seller.Email, "Sản phẩm của của bạn đã được tiến hành giao dịch", emailBody);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Can't send your email ");
			}
		}
		private async Task<string> RenderViewToStringAsync(string viewName, object model)
		{
			var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor, ModelState);
			using (var sw = new StringWriter())
			{
				var viewResult = _viewEngine.FindView(actionContext, viewName, false);
				if (viewResult.View == null)
				{
					throw new ArgumentNullException($"{viewName} does not match any available view");
				}

				var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
				{
					Model = model
				};

				var viewContext = new ViewContext(
				actionContext,
				viewResult.View,
					viewDictionary,
					new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
					sw,
					new HtmlHelperOptions()
				);

				await viewResult.View.RenderAsync(viewContext);
				return sw.ToString();
			}
		}
		private TransactionVM GetInvoiceExchangeById(int id)
		{
			var transaction = db.Transactions.FirstOrDefault(p => p.IdTransaction == id);
			var post = db.Posts.FirstOrDefault(p => p.IdPost == transaction.IdPost);
			var seller = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserSeller);
			var buyer = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserBuyer);
			var comment = db.Comments.SingleOrDefault(comment => comment.IdComment == transaction.IdComment) ?? null;
			// Lấy thông tin hóa đơn từ database
			return new TransactionVM
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
				} ?? null,
				DeliveryDate = db.Payments.FirstOrDefault(p => p.IdTransaction == id && p.Type == "Seller")?.PayTime,
				CreateDate = transaction.Date,
				CancelDate = db.Payments.FirstOrDefault(p => p.IdTransaction == id).CancelDate,
			};
		}
		private TransactionVM GetInvoiceSaleById(int id)
		{
			var transaction = db.Transactions.FirstOrDefault(p => p.IdTransaction == id);
			var post = db.Posts.FirstOrDefault(p => p.IdPost == transaction.IdPost);
			var seller = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserSeller);
			var buyer = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserBuyer);
			var comment = db.Comments.SingleOrDefault(comment => comment.IdComment == transaction.IdComment) ?? null;
			// Lấy thông tin hóa đơn từ database
			return new TransactionVM
			{
				TransactionId = id,
				Transaction = transaction,
				Post = post,
				UserBuyer = buyer,
				UserSeller = seller,
				DeliveryDate = db.Payments.FirstOrDefault(p => p.IdTransaction == id && p.Type == "Seller")?.PayTime,
				CreateDate = transaction.Date,
				CancelDate = db.Payments.FirstOrDefault(p => p.IdTransaction == id).CancelDate,
			};
		}
	}
}
