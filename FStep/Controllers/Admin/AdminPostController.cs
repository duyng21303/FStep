using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;
using X.PagedList;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using FStep.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FStep.Controllers.Admin
{
	public class AdminPostController : Controller
	{
		private readonly FstepDBContext _db;

		public AdminPostController(FstepDBContext context)
		{
			_db = context;
		}
		[Authorize]
		[HttpGet]
		public IActionResult ManagePost(string searchName, string searchType, string searchStatus, int page = 1)
		{
			var query = _db.Posts.AsQueryable();

			if (!string.IsNullOrEmpty(searchName))
			{
				query = query.Where(p => p.Content.Contains(searchName));
			}

			if (!string.IsNullOrEmpty(searchType))
			{
				query = query.Where(p => p.Type == searchType);
			}

			if (!string.IsNullOrEmpty(searchStatus))
			{
				query = query.Where(p => p.Status.ToLower() == searchStatus.ToLower());
			}

			var filteredCount = query
				.Where(p => p.Status != "Hidden")
				.Count();

			var result = query.Select(s => new ListPostVM
			{
				PostId = s.IdPost,
				PostTitle = s.Content ?? string.Empty,
				Type = s.Type ?? string.Empty,
				Quantity = s.IdProductNavigation != null && s.IdProductNavigation.Quantity.HasValue ? s.IdProductNavigation.Quantity.Value : 0,
				Price = s.IdProductNavigation != null && s.IdProductNavigation.Price.HasValue ? s.IdProductNavigation.Price.Value : 0f,
				Image = s.Img ?? string.Empty,
				CreateDate = s.Date ?? DateTime.Now,
				Status = s.Status ?? string.Empty,
				Category = s.Category ?? string.Empty,
			}).ToPagedList(page, 10);

			ViewBag.SearchName = searchName;
			ViewBag.SearchType = searchType;
			ViewBag.SearchStatus = searchStatus;
			ViewBag.Count = filteredCount;

			ViewBag.TypeOptions = new List<SelectListItem>
	{
		new SelectListItem { Value = "", Text = "Tất cả" },
		new SelectListItem { Value = "Exchange", Text = "Trao đổi" },
		new SelectListItem { Value = "Sale", Text = "Bán" }
	};
			ViewBag.StatusOptions = new List<SelectListItem>
	{
		new SelectListItem { Value = "", Text = "Tất cả" },
		new SelectListItem { Value = "True", Text = "Đã Duyệt" },
		new SelectListItem { Value = "Waiting", Text = "Chờ Duyệt" },
		new SelectListItem { Value = "Rejected", Text = "Từ Chối" },
		new SelectListItem { Value = "False", Text = "Hoàn Thành" },
		new SelectListItem { Value = "Trading", Text = "Giao Dịch" },
	};
			return View(result);

		}
		[HttpGet]
		[Authorize]
		public IActionResult UpdatePost(int id)
		{
			var post = _db.Posts
				.Include(p => p.IdProductNavigation)
				.FirstOrDefault(p => p.IdPost == id);
			if (post.Status == "Hidden" || post.Status == "Trading")
			{
				return Redirect("/AdminPost/ManagePost");
			}
			if (post == null)
			{
				return NotFound();
			}

			var postViewModel = new PostVM
			{
				IdPost = post.IdPost,
				Title = post.Content,
				Img = post.Img,
				Description = post.Detail,
				Type = post.Type,
				Quantity = post.IdProductNavigation.Quantity ?? 1,
				Price = post.IdProductNavigation.Price ?? 0
			};

			return View(postViewModel);
		}

		[HttpPost]
		[Authorize]
		public IActionResult UpdatePost(PostVM model, IFormFile img)
		{

			try
			{
				// Lưu thông tin bài đăng
				var post = _db.Posts
					.Include(p => p.IdProductNavigation)
					.FirstOrDefault(p => p.IdPost == model.IdPost);
				
				if (post == null)
				{
					return NotFound();
				}
				else
				{
					post.Content = model.Title;
					post.Date = DateTime.Now;
					if (img != null)
					{
						post.Img = Util.UpLoadImg(img, "postPic"); // Upload và lưu hình ảnh mới
					}
					if (post.Type == "Sale" && post.IdProductNavigation != null)
					{
						post.IdProductNavigation.Quantity = model.Quantity;
						post.IdProductNavigation.Price = model.Price;
					}
					post.Detail = model.Description;
					post.Status = "Waiting";

					_db.SaveChanges();
					TempData["SuccessMessage"] = $"Bài đăng của bạn đã được sửa thành công.";
					return RedirectToAction("ManagePost", "AdminPost");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật bài đăng.";
			}
			return View(model);
		}

		public IActionResult HiddenPost(int id)
		{
			var post = _db.Posts.FirstOrDefault(p => p.IdPost == id);
			if (post != null)
			{
				post.Status = "Hidden";
				_db.Posts.Update(post);
				_db.SaveChanges();
				TempData["SuccessMessage"] = $"Bài đăng {post.Content} đã được xóa thành công.";
			}
			else
			{
				TempData["ErrorMessage"] = $"Bài đăng {post.Content} không được tìm thấy.";
			}
			return RedirectToAction("ManagePost");

		}
		public IActionResult UpdateStatus(int id)
		{
			var post = _db.Posts.FirstOrDefault(p => p.IdPost == id);
			if (post != null)
			{
				post.Status = "True";
				post.Date = DateTime.Now;

				_db.Posts.Update(post);
				_db.SaveChanges();
				TempData["SuccessMessage"] = $"Bài đăng {id} đã được duyệt thành công.";
			}
			else
			{
				TempData["ErrorMessage"] = $"Bài đăng {id} không được tìm thấy.";
			}
			return RedirectToAction("ManagePost");
		}

		public IActionResult DeletePost(int id, int? page)
		{
			var post = _db.Posts.FirstOrDefault(p => p.IdPost == id);
			if (post != null)
			{
				post.Status = "Rejected";
				_db.Posts.Update(post);
				_db.SaveChanges();
				TempData["SuccessMessage"] = $"Bài đăng {id} đã được từ chối thành công.";
			}
			else
			{
				TempData["ErrorMessage"] = $"Bài đăng {id} không được tìm thấy.";
			}
			return RedirectToAction("ManagePost", page);
		}

	}
}