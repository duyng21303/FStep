using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.Models;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Blazor;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Diagnostics;
using X.PagedList;

namespace FStep.Controllers
{

	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly FstepDBContext db;
		private readonly IMapper _mapper;

		public HomeController(FstepDBContext context, IMapper mapper)
		{
			db = context;
			_mapper = mapper;
		}

		public IActionResult Index(String? query, int? page)
		{
			int pageSize = 12; // số lượng sản phẩm mỗi trang 
			int pageNumber = (page ?? 1);   // số trang hiện tại, mặc định là trang 1 nếu ko có page được chỉ định 
			var ExchangePost = db.Posts.AsQueryable();
			ExchangePost = ExchangePost.Where(p => p.Type == "Exchange" && p.Status == "true");    //check exchangePost là những post thuộc type "exhcange" và có status = 1

			if (!string.IsNullOrEmpty(query))
			{
				ExchangePost = ExchangePost.Where(p => p.Content.Contains(query));
			}
			var result = ExchangePost.Select(s => new PostVM
			{
				IdProduct = s.IdProduct,
				IdPost = s.IdPost,
				Title = s.Content,
				Description = s.Detail,
				Img = s.Img,
				CreateDate = s.Date.HasValue ? s.Date.Value : DateTime.Now
			}).OrderByDescending(o => o.IdPost);

			var pageList = result.ToPagedList(pageNumber, pageSize);

			ViewBag.Query = query;
			return View(pageList);
		}

		public IActionResult Sale(String? query, int? page)
		{
			int pageSize = 12; // số lượng sản phẩm mỗi trang 
			int pageNumber = (page ?? 1);  // số trang hiện tại, mặc định là trang 1 nếu ko có page được chỉ định 
			var SalePost = db.Posts.AsQueryable();
			SalePost = SalePost.Where(p => p.Type == "Sale" && p.Status == "true");

			if (!string.IsNullOrEmpty(query))
			{
				SalePost = SalePost.Where(p => p.Content.Contains(query));
			}

			var result = SalePost.Select(s => new PostVM
			{
				IdPost = s.IdPost,
				Title = s.Content,
				Img = s.Img,
				Description = s.Detail,
				Quantity = (int)s.IdProductNavigation.Quantity,
				CreateDate = s.Date.HasValue ? s.Date.Value : DateTime.Now,
				Price = s.IdProductNavigation.Price ?? 0
			}).OrderByDescending(o => o.IdPost);

			var pageList = result.ToPagedList(pageNumber, pageSize);

			ViewBag.Query = query;
			return View(pageList);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
		[Authorize]
		[HttpPost]
		public ActionResult Create(PostVM model, IFormFile img)
		{
			try
			{
				var product = _mapper.Map<Product>(model);
				product.Quantity = 1;
				product.Price = model.Price;
				product.Status = "true";
				db.Add(product);
				db.SaveChanges();

				var post = _mapper.Map<Post>(model);
				post.Content = model.Title;
				post.Date = DateTime.Now;
				//Helpers.Util.UpLoadImg(model.Img, "")
				post.Img = Util.UpLoadImg(img, "postPic");
				post.Status = "false";
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
			}
			return PartialView("Create");
		}

		[Authorize]
		public IActionResult TransactionHistory(String? query, int? page)
		{
			int pageSize = 12; // số lượng sản phẩm mỗi trang 
			int pageNumber = (page ?? 1);   // số trang hiện tại, mặc định là trang 1 nếu ko có page được chỉ định 

			var transaction = db.Transactions.AsQueryable();
			transaction = transaction.Where(p => p.IdUserBuyer == User.FindFirst("UserID").Value);

			if (!string.IsNullOrEmpty(query))
			{
				transaction = transaction.Where(p => p.IdPostNavigation.Content.Contains(query));
				//db.Posts.FirstOrDefault(x => x.Content.Contains(query)).IdPost
			}
			var result = transaction.Select(s => new TransactionVM
			{
				TransactionId = s.IdTransaction,
				IdPost = s.IdPost,
				Content = s.IdPostNavigation.Content,
				Detail = s.IdPostNavigation.Detail,
				TypePost = s.IdPostNavigation.Type,
				DeliveryDate = db.Payments.SingleOrDefault(p => p.IdTransaction == s.IdTransaction && p.Type == "Seller").PayTime,
				Img = s.IdPostNavigation.Img,
				Status = s.Status,
				UnitPrice = s.UnitPrice,
				Quantity = s.Quantity,
				Amount = s.Amount,
				IdUserSeller = s.IdUserSeller,
				CodeTransaction = s.CodeTransaction,
				UserName = db.Users.FirstOrDefault(p => p.IdUser == s.IdUserBuyer).Name ?? null,
				IdSeller = s.IdUserSeller,
				SellerImg = db.Users.SingleOrDefault(p => p.IdUser == s.IdUserSeller).AvatarImg,
				SellerName = db.Users.SingleOrDefault(p => p.IdUser == s.IdUserSeller).Name,
				CheckFeedback = (db.Feedbacks.FirstOrDefault(p => p.IdPost == s.IdPost) != null),
			}).OrderByDescending(o => o.TransactionId);

			var pageList = result.ToPagedList(pageNumber, pageSize);

			ViewBag.Query = query;
			return View(pageList);
		}
		[Authorize]
		[HttpGet]
		public ActionResult TransactionDetail(int id)
		{
			return View(new TransactionVM()
			{
				TransactionId = id,
				IdPost = db.Transactions.FirstOrDefault(p => p.IdTransaction == id).IdPost,
				Content = db.Posts.FirstOrDefault(p => p.IdPost == db.Transactions.FirstOrDefault(p => p.IdTransaction == id).IdPost).Content,
				Detail = db.Posts.FirstOrDefault(p => p.IdPost == db.Transactions.FirstOrDefault(p => p.IdTransaction == id).IdPost).Detail,
				TypePost = db.Posts.FirstOrDefault(p => p.IdPost == db.Transactions.FirstOrDefault(p => p.IdTransaction == id).IdPost).Type,
				DeliveryDate = db.Transactions.FirstOrDefault(p => p.IdTransaction == id).Date,
				Img = db.Posts.FirstOrDefault(p => p.IdPost == db.Transactions.FirstOrDefault(p => p.IdTransaction == id).IdPost).Img,
				UnitPrice = db.Transactions.FirstOrDefault(p => p.IdTransaction == id).UnitPrice,
				Quantity = db.Transactions.FirstOrDefault(p => p.IdTransaction == id).Quantity,
				Amount = db.Transactions.FirstOrDefault(p => p.IdTransaction == id).Amount,
				IdUserSeller = db.Transactions.FirstOrDefault(p => p.IdTransaction == id).IdUserSeller,
				CodeTransaction = db.Transactions.FirstOrDefault(p => p.IdTransaction == id).CodeTransaction,
				UserName = db.Users.FirstOrDefault(p => p.IdUser == db.Transactions.FirstOrDefault(p => p.IdTransaction == id).IdUserBuyer).Name ?? null,
			});
		}
	}
}
