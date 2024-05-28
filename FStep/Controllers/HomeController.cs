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

		public IActionResult Index(int? page)
		{ 
			int pageSize = 12; // số lượng sản phẩm mỗi trang 
            int pageNumber = (page ?? 1);   // số trang hiện tại, mặc định là trang 1 nếu ko có page được chỉ định 
            var ExchangePost = db.Posts.AsQueryable();
			ExchangePost = ExchangePost.Where(p => p.Type == "Exchange" && !(p.Status == false));    //check exchangePost là những post thuộc type "exhcange" và có status = 1

            var result = ExchangePost.Select(s => new ExchangePostVM
			{
				idPost = s.IdPost,
				Title = s.Content,
				Description = s.Detail,
				Image = s.Img,
				CreateDate = s.Date.HasValue ? s.Date.Value : DateTime.Now
			}).OrderByDescending(o => o.idPost) ;

			var pageList = result.ToPagedList(pageNumber, pageSize);
			return View(pageList);
		}


		public IActionResult Sale(int? page)
		{
			int pageSize = 12; // số lượng sản phẩm mỗi trang 
            int pageNumber = (page ?? 1);  // số trang hiện tại, mặc định là trang 1 nếu ko có page được chỉ định 
            var SalePost = db.Posts.AsQueryable();
			SalePost = SalePost.Where(p => p.Type == "Sale" && !(p.Status == false));

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
            return View(pageList);
        }

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
