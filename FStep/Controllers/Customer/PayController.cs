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


namespace FStep.Controllers.Customer
{
	public class PayController : Controller
	{

		private readonly FstepDbContext db;
		private readonly IMapper _mapper;
		private readonly IEmailSender emailSender;
		private readonly IVnPayService _vnPayService;
		private readonly IRazorViewEngine _viewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<RegistrationController> _logger;

		public PayController(FstepDbContext context, IMapper mapper, IVnPayService vnPayService, IEmailSender emailSender, IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider, ILogger<RegistrationController> logger)
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

				//lấy thông tin transaction

				var invoice = GetInvoiceById(db.Transactions.FirstOrDefault(p => p.CodeTransaction == transaction.CodeTransaction).IdTransaction);
				string emailBody = await RenderViewToStringAsync("Invoice", invoice);

				//sent email
				bool sentBuyer = await emailSender.EmailSendAsync(buyer.Email, "Đơn hàng của bạn đã được tạo", emailBody);
				bool sentSeller = await emailSender.EmailSendAsync(seller.Email, "Sản phẩm của của bạn đã được mua", emailBody);

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

		//public string GenerateInvoiceHtml(TransactionVM invoice)
		//{
		//	var html = new StringBuilder();
		//	html.Append("<html>");
		//	html.Append("<head>");
		//	html.Append("<meta charset='utf-8'/>"); // Đảm bảo mã hóa UTF-8
		//	html.Append("<title>Hóa đơn</title>");
		//	html.Append("</head>");
		//	html.Append("<body>");
		//	html.Append("<h1>Hóa đơn</h1>");
		//	html.Append($"<p>Mã hóa đơn: {invoice.CodeTransaction}</p>");
		//	html.Append($"<p>Tên khách hàng: {invoice.UserName}</p>");
		//	html.Append($"<p>Tổng số tiền: {invoice.Amount}</p>");
		//	// Thêm các thông tin khác của hóa đơn
		//	html.Append("</body>");
		//	html.Append("</html>");
		//	return html.ToString();
		//}

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

		private TransactionVM GetInvoiceById(int id)
		{
			var transaction = db.Transactions.FirstOrDefault(p => p.IdTransaction == id);
			var post = db.Posts.FirstOrDefault(p => p.IdPost == transaction.IdPost);
			var seller = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserSeller);
			var buyer = db.Users.FirstOrDefault(p => p.IdUser == transaction.IdUserBuyer);
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

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Cancel(CancelVM model)
		{
			try
			{
				var transaction = db.Transactions.SingleOrDefault(p => p.IdTransaction == model.TransactionId);
				transaction.Status = "Canceled";
				db.Update(transaction);

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

				//tạo invoice
				var invoice = GetInvoiceById(db.Transactions.FirstOrDefault(p => p.CodeTransaction == transaction.CodeTransaction).IdTransaction);
				string emailBody = await RenderViewToStringAsync("Invoice", invoice);

				var buyer = db.Users.SingleOrDefault(p => p.IdUser == transaction.IdUserBuyer);
				var seller = db.Users.SingleOrDefault(p => p.IdUser == transaction.IdUserSeller);
				//sent email
				bool sentSeller = await emailSender.EmailSendAsync(seller.Email, "Sản phẩm của của bạn đã bị huỷ bởi người mua", emailBody);
				bool sentBuyer = await emailSender.EmailSendAsync(buyer.Email, "Sản phẩm của của bạn đã được huỷ", emailBody);

				db.SaveChanges();

				return RedirectToAction("TransactionHistory", "Home");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Can't send your email ");
				return RedirectToAction("Error", "Home");
			}

		}
	}
}
