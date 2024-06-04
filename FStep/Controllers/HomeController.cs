using FStep.Data;
using FStep.Models;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using X.PagedList;

namespace FStep.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly FstepDBContext db;

		public HomeController(FstepDBContext context) => db = context;

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
				idPost = s.IdPost,
				Title = s.Content,
				Description = s.Detail,
				Image = s.Img,
				CreateDate = s.Date.HasValue ? s.Date.Value : DateTime.Now
			}).OrderByDescending(o => o.idPost) ;

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
				idPost = s.IdPost,
				Title = s.Content,
				Image = s.Img,
				Description = s.Detail,
				CreateDate = s.Date.HasValue ? s.Date.Value : DateTime.Now,
				Price = s.IdProductNavigation.Price ?? 0
			}).OrderByDescending(o => o.idPost);

            var pageList = result.ToPagedList(pageNumber, pageSize);

            ViewBag.Query = query;
            return View(pageList);
        }
		public IActionResult Product(int? type)
		{
			var product = db.Products.AsQueryable();

			if (type.HasValue)
			{
				product = product.Where(p => p.IdProduct == type.Value);
			}
			var result = product.Select(p => new ExchangePostVM
			{
				IdProduct = p.IdProduct,
				Name = p.Name,
				Price = p.Price ?? 0,
				Detail = p.Detail ?? "",
				RecieveImg = p.RecieveImg ?? "",

			});
			return View(result);
		}
		public IActionResult Detail(int id)
		{
			var data = db.Products.SingleOrDefault(p => p.IdProduct == id);

			return View(data);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
