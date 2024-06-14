﻿using AutoMapper;
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

		//[HttpGet]
		//public IActionResult CheckoutExchange(int id)
		//{
		//	var exchange = db.Posts.SingleOrDefault(x => x.IdPost == id);
		//	ViewData["Content"] = exchange.Content;
		//	ViewData["Img"] = exchange.Img;
		//	ViewData["Type"] = exchange.Type;
		//	return View("Checkout");
		//}
		//[Authorize]
		//[HttpPost]
		//public IActionResult CheckoutExchange(CheckoutVM model)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		var transaction = _mapper.Map<Transaction>(model);
		//		transaction.IdUserBuyer = User.FindFirst("UserID").Value;
		//		transaction.IdUserSeller = "";
		//		transaction.Amount = model.Amount;
		//		transaction.Date = DateTime.Now;
		//		transaction.IdPost = model.IdPost;
		//		db.Add(transaction);
		//		db.SaveChanges();
		//	}
		//	return RedirectToAction("Index");
		//}

		[HttpGet]
		public IActionResult CheckoutSale(int id)
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
			checkout.Quantity = db.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct).Quantity;
			checkout.Amount = db.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct).Price * db.Products.SingleOrDefault(p => p.IdProduct == post.IdProduct).Quantity;
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
			TempData["Message"] = $"VnPay success";
			return RedirectToAction("PaymentSuccess");
		}
	}
}
