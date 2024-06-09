using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.Services;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

		[HttpGet]
		public IActionResult CheckoutExchange(int id)
		{
			var exchange = db.Posts.SingleOrDefault(x => x.IdPost == id);
			ViewData["Content"] = exchange.Content;
			ViewData["Img"] = exchange.Img;
			ViewData["Type"] = exchange.Type;
			return View("Checkout");
		}
		[Authorize]
		[HttpPost]
		public IActionResult CheckoutExchange(CheckoutVM model)
		{
			if (ModelState.IsValid)
			{
				var transaction = _mapper.Map<Transaction>(model);
				transaction.IdUserBuyer = User.FindFirst("UserID").Value;
				transaction.IdUserSeller = "";
				transaction.Amount = model.Amount;
				transaction.Date = DateTime.Now;
				transaction.IdPost = model.IdPost;
				db.Add(transaction);
				db.SaveChanges();
			}
			return RedirectToAction("Index");
		}
		[HttpGet]
		public IActionResult CheckoutSale(int id)
		{
			var post = db.Posts.SingleOrDefault(x => x.IdPost == id);
			ViewData["Content"] = post.Content;
			ViewData["Img"] = post.Img;
			ViewData["Price"] = db.Products.SingleOrDefault(p => p.IdProduct == id).Price;
			ViewData["Quantity"] = db.Products.SingleOrDefault(p => p.IdProduct == id).Quantity;
			ViewData["Amount"] = db.Products.SingleOrDefault(p => p.IdProduct == id).Price * db.Products.SingleOrDefault(p => p.IdProduct == id).Quantity;
			ViewData["Type"] = post.Type;
			return View("Checkout");
		}
		[Authorize]
		[HttpPost]
		public IActionResult CheckoutSale(CheckoutVM model, string paymentMethod = "COD")
		{
			if (paymentMethod == "VnPay")
			{
				var vnPayModel = new VnPaymentRequestModel
				{
					Amount = 1000000,
					CreatedDate = DateTime.Now,
					Description = "Thanh toan don hang",
					FullName = User.FindFirst("UserID").Value,
					TransactionId = new Random().Next(1000, 100000)
				};
				return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
			}

			if (ModelState.IsValid)
			{
				var transaction = _mapper.Map<Transaction>(model);
				transaction.IdUserBuyer = User.FindFirst("UserID").Value;
				transaction.IdUserSeller = "";
				transaction.Amount = model.Amount;
				transaction.Date = DateTime.Now;
				transaction.IdPost = model.IdPost;
				db.Add(transaction);
				db.SaveChanges();
			}
			return RedirectToAction("Index");
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

			TempData["Message"] = $"VnPay success";
			return RedirectToAction("PaymentSuccess");

		}

		public IActionResult Refund()
		{
			return View();
		}
	}
}
