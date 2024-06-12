using FStep.Data;
using FStep.Models;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing.Printing;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FStep.Controllers.ManagePost
{
	public class ModeManagePostController : Controller
	{
		private readonly FstepDbContext db;

		public ModeManagePostController(FstepDbContext context ) => db = context;
		public IActionResult ViewPost(String? query, int? page)
		{

			int pageSize = 12; // số lượng sản phẩm mỗi trang 
			int pageNumber = (page ?? 1);   // số trang hiện tại, mặc định là trang 1 nếu ko có page được chỉ định 
			var ListPost = db.Posts.AsQueryable();
			ListPost = ListPost.Where(p => p.Status == false || p.Status == true);    //check exchangePost là những post thuộc type "exhcange" và có status = 1
			if (!string.IsNullOrEmpty(query))
			{
				ListPost = ListPost.Where(p => p.IdUserNavigation.StudentId.Contains(query));
			}
			var result = ListPost.Select(s => new ListPostVM
			{
				PostId = s.IdPost,
				PostTitle = s.Content ?? string.Empty, // Default to empty string if Content is null
				PostBody = s.Detail ?? string.Empty, // Assuming PostBody should be included and default to empty string
				Type = s.Type ?? string.Empty, // Default to empty string if Type is null
				StudentId = s.IdUserNavigation.StudentId ?? string.Empty, // Default to empty string if StudentId is null
				Quantity = s.IdProductNavigation.Quantity ?? 0, // Default to 0 if Quantity is null
				Price = s.IdProductNavigation.Price ?? 0f, // Default to 0 if Price is null
				Image = s.Img ?? string.Empty, // Default to empty string if Image is null
				CreateDate = s.Date ?? DateTime.Now, // Default to current DateTime if Date is null
				Status = s.Status ?? false
			}).OrderByDescending(o => o.PostId);

			var pageList = result.ToPagedList(pageNumber, pageSize);
			ViewBag.Query = query;
			return View(pageList);
		}

		public IActionResult UpdateStatus(int PostId, string Status)
		{
			// Find the post by PostId
			var post = db.Posts.FirstOrDefault(p => p.IdPost == PostId);
			if (post != null)
			{
				// Update the status
				post.Status = !post.Status;
				post.Date = DateTime.Now;
				// Save changes to the database
				db.SaveChanges();
			}
			// Redirect back to the ViewPost action
			return RedirectToAction("ViewPost");

		}
	}
}
