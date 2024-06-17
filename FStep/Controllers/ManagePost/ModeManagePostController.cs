using FStep.Data;
using FStep.Models;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using X.PagedList;

namespace FStep.Controllers.ManagePost
{
    public class ModeManagePostController : Controller
	{
		private readonly FstepDbContext db;

		public ModeManagePostController(FstepDbContext context) => db = context;
		public IActionResult ViewPost()
		{

			// số lượng sản phẩm mỗi trang 
			// số trang hiện tại, mặc định là trang 1 nếu ko có page được chỉ định 
			var ListPost = db.Posts.AsQueryable();
			ListPost = ListPost.Where(p => p.Status == "false");    //check exchangePost là những post thuộc type "exhcange" và có status = 1
			var result = ListPost.Select(s => new ListPostVM
			{
				PostId = s.IdPost,
				PostTitle = s.Content,
				Type = s.Type,
				StudentId = s.IdUserNavigation.StudentId,
				Quantity = (int)s.IdProductNavigation.Quantity,
				Price = s.IdProductNavigation.Price ?? 0,
				Image = s.Img,
				CreateDate = s.Date.HasValue ? s.Date.Value : DateTime.Now,
				Status = s.Status
			}).OrderByDescending(o => o.PostId);




			return View(result);
		}
	}
}