using AutoMapper;
using Azure.Identity;
using FStep.Data;
using FStep.Helpers;
using FStep.Services;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace FStep.Controllers.Customer
{
	public class PayController : Controller
	{

		private readonly FstepDBContext db;
		private readonly IMapper _mapper;
		private readonly IVnPayService _vnPayService;

		public PayController(FstepDBContext context, IMapper mapper, IVnPayService vnPayService)
		{
			db = context;
			_mapper = mapper;
			_vnPayService = vnPayService;
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
			checkout.Quantity = quantity;
			checkout.Amount = db.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct).Price * quantity;

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


			var transaction = new Transaction();
			transaction.Date = DateTime.Now;
			transaction.Status = "Processing";
			transaction.Quantity = info.Quantity;
			transaction.UnitPrice = info.UnitPrice;
			transaction.Amount = float.Parse(response.Amount.ToString()) / 100;
			transaction.Note = info.Note;
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
			payment.Type = "buyer";
			payment.IdTransaction = db.Transactions.SingleOrDefault(p => p.CodeTransaction == transaction.CodeTransaction).IdTransaction;
			db.Add(payment);
			db.SaveChanges();

			var product = db.Products.SingleOrDefault(p => p.IdProduct == db.Posts.SingleOrDefault(p => p.IdPost == info.IdPost).IdProduct);
			var post = db.Posts.SingleOrDefault(p => p.IdPost == info.IdPost);
			product.Quantity -= info.Quantity;
			if (product.SoldQuantity == null)
				product.SoldQuantity = 0;
			product.SoldQuantity += info.Quantity;

			if (product.Quantity <= 0)
			{
				post.Status = "false";
				product.Status = "false";

			}
			db.Update(product);
			db.SaveChanges();


			TempData["Message"] = $"VnPay success";
			return RedirectToAction("PaymentSuccess");
		}
	}
}
