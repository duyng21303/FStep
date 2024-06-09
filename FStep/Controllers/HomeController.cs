using AutoMapper;
using FStep.Data;
using FStep.Models;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
			ExchangePost = ExchangePost.Where(p => p.Type == "Exchange" && !(p.Status == false));    //check exchangePost là những post thuộc type "exhcange" và có status = 1

			if (!string.IsNullOrEmpty(query))
			{
				ExchangePost = ExchangePost.Where(p => p.Content.Contains(query));
			}
			var result = ExchangePost.Select(s => new ExchangePostVM
			{
				IdProduct = s.IdProduct,
				Id = s.IdPost,
				Title = s.Content,
				Description = s.Detail,
				Img = s.Img,
				CreateDate = s.Date.HasValue ? s.Date.Value : DateTime.Now
			}).OrderByDescending(o => o.Id);

			var pageList = result.ToPagedList(pageNumber, pageSize);

			ViewBag.Query = query;
			return View(pageList);
		}


		public IActionResult Sale(String? query, int? page)
		{
			int pageSize = 12; // số lượng sản phẩm mỗi trang 
			int pageNumber = (page ?? 1);  // số trang hiện tại, mặc định là trang 1 nếu ko có page được chỉ định 
			var SalePost = db.Posts.AsQueryable();
			SalePost = SalePost.Where(p => p.Type == "Sale" && !(p.Status == false));

			if (!string.IsNullOrEmpty(query))
			{
				SalePost = SalePost.Where(p => p.Content.Contains(query));
			}

			var result = SalePost.Select(s => new SalePostVM
			{
				Id = s.IdPost,
				Title = s.Content,
				Img = s.Img,
				Description = s.Detail,
				CreateDate = s.Date.HasValue ? s.Date.Value : DateTime.Now,
				Price = s.IdProductNavigation.Price ?? 0
			}).OrderByDescending(o => o.Id);

			var pageList = result.ToPagedList(pageNumber, pageSize);

			ViewBag.Query = query;
			return View(pageList);
		}

		[HttpGet]
		public IActionResult DetailPost(int id)
		{
			var data = db.Posts.Include(x => x.IdProductNavigation).Include(x => x.IdUserNavigation).SingleOrDefault(p => p.IdPost == id);

			// lấy thêm comment sản phẩm
			var comments = db.Comments.Where(x => x.IdPost == id).Include(x => x.IdUserNavigation).Select(x => new CommentDTO
			{
				IdPost = id,
				IdUser = x.IdUser,
				Content = x.Content,
				Date = x.Date,
				IdComment = x.IdComment,
				Name = x.IdUserNavigation.Name
			}).ToList();

			ViewData["comments"] = comments;

			return View(data);
		}

		[HttpPost]
		public IActionResult PostComment([FromForm] CommentDTO comment)
		{
			string refererUrl = Request.Headers["Referer"].ToString();
			try
			{
				var isAuthenticated = User?.Identity?.IsAuthenticated;
				if (isAuthenticated == true)
				{
					comment.IdUser = User.FindFirst("UserID")?.Value;
					comment.Date = DateTime.Now;
					var saveComment = _mapper.Map<Comment>(comment);
					saveComment.Reports = null;
					saveComment.UserNotifications = null;
					db.Comments.Add(saveComment);
					db.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				var exc = ex;
			}
			if (!string.IsNullOrEmpty(refererUrl))
			{
				return Redirect(refererUrl);
			}
			return RedirectToAction("DetailPost", "Home", new { id = comment.IdPost });

		}

		public IActionResult DetailSalePost(int id)
		{
			var SalePost = db.Posts.Include(x => x.IdProductNavigation).AsQueryable();
			SalePost = SalePost.Where(p => p.IdPost == id);
			var result = SalePost.Select(s => new SalePostVM
			{
				Id = s.IdPost,
				Title = s.Content,
				Quantity = (int)s.IdProductNavigation.Quantity,
				Img = s.Img,
				Description = s.Detail,
				NameProduct = s.IdProductNavigation.Name,
				DetailProduct = s.IdProductNavigation.Detail,
				CreateDate = s.Date.HasValue ? s.Date.Value : DateTime.Now,
				Price = s.IdProductNavigation.Price ?? 0
			}).SingleOrDefault();

			// lấy thêm comment sản phẩm
			var comments = db.Comments.Where(x => x.IdPost == id).Include(x => x.IdUserNavigation).Select(x => new CommentDTO
			{
				IdPost = id,
				IdUser = x.IdUser,
				Content = x.Content,
				Date = x.Date,
				IdComment = x.IdComment,
				Name = x.IdUserNavigation.Name
			}).ToList();

			ViewData["comments"] = comments;

			return View(result);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
