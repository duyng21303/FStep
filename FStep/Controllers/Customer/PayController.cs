using AutoMapper;
using Azure.Identity;
using FStep.Data;
using FStep.Helpers;
using FStep.Repostory.Interface;
using FStep.Services;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Protocol.Plugins;
using System.Transactions;

namespace FStep.Controllers.Customer
{
	public class PayController : Controller
	{

		private readonly FstepDbContext db;
		private readonly IMapper _mapper;
		private readonly IEmailSender emailSender;
		private readonly IVnPayService _vnPayService;

		public PayController(FstepDbContext context, IMapper mapper, IVnPayService vnPayService, IEmailSender emailSender)
		{
			db = context;
			_mapper = mapper;
			_vnPayService = vnPayService;
			this.emailSender = emailSender;
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
		public IActionResult PaymentCallBack()
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


			if (product.Quantity <= 0)
			{
				post.Status = "Trading";
				product.Status = "False";
			}
			db.Update(product);
			db.SaveChanges();

			var buyer = db.Users.SingleOrDefault(p => p.IdUser == info.IdUserBuyer);
			var seller = db.Users.SingleOrDefault(p => p.IdUser == info.IdUserSeller);

			//sent email
			emailSender.EmailSendAsync(buyer.Email, "Đơn hàng của bạn đã được tạo", "Thông tin chi tiết đơn hàng");
			emailSender.EmailSendAsync(seller.Email, "Sản phẩm của của bạn đã được mua", "Thôn tin chi tiết đơn hàng");

			TempData["Message"] = $"VnPay success";
			return RedirectToAction("PaymentSuccess");
		}

		[Authorize]
		[HttpPost]
		public IActionResult Cancel(CancelVM model)
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

			var payment = new Payment();
			payment.VnpayTransactionCode = transaction.CodeTransaction;
			payment.PayTime = DateTime.Now;
			payment.Type = "Buyer";
			payment.Status = "False";
			payment.Note = model.Note;
			payment.IdTransaction = transaction.IdTransaction;
			db.Add(payment);

			db.SaveChanges();

			return RedirectToAction("TransactionHistory","Home");
		}

	}
}
