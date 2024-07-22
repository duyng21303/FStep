using FStep.Data;
using FStep.Repostory.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using FStep.Models;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using X.PagedList;
using FStep.Services;

namespace FStep.Controllers.ManagePost
{
	public class ModeManagePostController : Controller
	{
		private readonly FstepDBContext _db;
		private readonly NotificationServices notificationServices;

		public ModeManagePostController(FstepDBContext context)
		{
			_db = context;
			notificationServices = new NotificationServices(_db);
		}
		[Authorize(Roles = "Moderator")]
		[HttpGet]
		public IActionResult ManagePosts(string pendingQuery, string approvedQuery, String currentTab, int pendingPage = 1, int approvedPage = 1, int pageSize = 30)
		{
			var pendingPostsResult = GetPendingPosts(pendingQuery, pendingPage, pageSize);
			var approvedPostsResult = GetApprovedPosts(approvedQuery, approvedPage, pageSize);

			var viewModel = new ManagePostsVM
			{
				PendingPosts = pendingPostsResult.Posts,
				ApprovedPosts = approvedPostsResult.Posts,
				PendingPostsCount = pendingPostsResult.Count,
				ApprovedPostsCount = approvedPostsResult.Count,
				PendingQuery = pendingQuery,
				ApprovedQuery = approvedQuery
			};

			ViewBag.PageSize = pageSize;
			ViewBag.CurrentTab = currentTab; // Add this line
			ViewBag.PendingPostsCount = pendingPostsResult.Count;
			ViewBag.ApprovedPostsCount = approvedPostsResult.Count;
			ViewBag.PendingQuery = pendingQuery;
			ViewBag.ApprovedQuery = approvedQuery;
			ViewBag.PendingPage = pendingPage;
			ViewBag.ApprovedPage = approvedPage;
			return View(viewModel);
		}

		[Authorize(Roles = "Moderator")]
		private PendingPostsResultVM GetPendingPosts(string query, int pageNumber, int pageSize)
		{
			var queryable = _db.Posts
				.Include(p => p.IdUserNavigation)
				.Include(p => p.IdProductNavigation)
				.Where(p => p.Status == "Waiting");


			if (!string.IsNullOrEmpty(query))
			{
				queryable = queryable.Where(p => p.IdUserNavigation.StudentId.Contains(query));
			}

			var result = queryable
				.OrderBy(p => p.Date)
				.Select(s => new ListPostVM
				{
					PostId = s.IdPost,
					PostTitle = s.Content ?? string.Empty,
					PostBody = s.Detail ?? string.Empty,
					Type = s.Type ?? string.Empty,
					StudentId = s.IdUserNavigation.StudentId ?? string.Empty,
					Quantity = s.IdProductNavigation != null && s.IdProductNavigation.Quantity.HasValue ? s.IdProductNavigation.Quantity.Value : 0,
					Price = s.IdProductNavigation != null && s.IdProductNavigation.Price.HasValue ? s.IdProductNavigation.Price.Value : 0f,
					Image = s.Img ?? string.Empty,
					CreateDate = s.Date ?? DateTime.Now,
					Status = s.Status ?? string.Empty,
					Category = s.Category ?? string.Empty,
					Description = s.Detail ?? string.Empty,

				})
				.ToPagedList(pageNumber, pageSize);

			return new PendingPostsResultVM
			{
				Posts = result,
				Count = result.TotalItemCount
			};

		}

		[Authorize(Roles = "Moderator")]
		private ApprovedPostsResultVM GetApprovedPosts(string query, int pageNumber, int pageSize)
		{
			var queryable = _db.Posts
				.Include(p => p.IdUserNavigation)
				.Include(p => p.IdProductNavigation)
				.Where(p => p.Status != "Waiting" && p.Status != "Rejected" && p.Status != "Hidden");


			if (!string.IsNullOrEmpty(query))
			{
				queryable = queryable.Where(p => p.IdUserNavigation.StudentId.Contains(query));
			}

			var result = queryable
				.OrderBy(p => p.Date)
				.Select(s => new ListPostVM
				{
					PostId = s.IdPost,
					PostTitle = s.Content ?? string.Empty,
					StudentId = s.IdUserNavigation.StudentId ?? string.Empty,
					Quantity = s.IdProductNavigation != null && s.IdProductNavigation.Quantity.HasValue ? s.IdProductNavigation.Quantity.Value : 0,
					Image = s.Img ?? string.Empty,
					CreateDate = s.Date ?? DateTime.Now,
					Location = s.Location ?? string.Empty,
					Status = s.Status ?? string.Empty,
					Category = s.Category ?? string.Empty,
				})
			.ToPagedList(pageNumber, pageSize);

			return new ApprovedPostsResultVM
			{
				Posts = result,
				Count = result.TotalItemCount
			};

		}
		[Authorize(Roles = "Moderator")]
		[HttpPost]
		public IActionResult UpdateStatus(int id)
		{
			var post = _db.Posts.FirstOrDefault(p => p.IdPost == id);
			if (post != null)
			{
				post.Status = "True";
				post.Date = DateTime.Now;

				_db.Posts.Update(post);
				_db.SaveChanges();
				TempData["SuccessMessage"] = $"Bài đăng {post.Content} đã được duyệt thành công.";
			}
			else
			{
				TempData["ErrorMessage"] = $"Bài đăng {post.Content} không được tìm thấy.";
			}
			string encodedUrl = Url.Action("ManagePosts", new { currentTab = "pendingPosts" });
			return Redirect(encodedUrl);

		}


		[Authorize(Roles = "Moderator")]
		[HttpPost]
		public async Task<IActionResult> DeletePost(int id)
		{
			var post = _db.Posts.FirstOrDefault(p => p.IdPost == id);
			if (post != null)
			{
				post.Status = "Rejected";
				_db.Posts.Update(post);
				await _db.SaveChangesAsync();
				await notificationServices.CreateNotification(post.IdUser, "RejectPost", "None", post.Content, 0);
				TempData["SuccessMessage"] = $"Bài đăng {post.Content} đã được xóa thành công.";
			}
			else
			{
				TempData["ErrorMessage"] = $"Bài đăng {post.Content} không được tìm thấy.";
			}
			string encodedUrl = Url.Action("ManagePosts", new { currentTab = "pendingPosts" });
			return Redirect(encodedUrl);
		}


		[Authorize(Roles = "Moderator")]
		[HttpPost]
		public IActionResult UpdateStatusAgain(int id)
		{
			var post = _db.Posts.FirstOrDefault(p => p.IdPost == id);
			if (post != null)
			{
				post.Status = "True";
				post.Date = DateTime.Now;

				_db.Posts.Update(post);
				_db.SaveChanges();
				TempData["SuccessMessage"] = $"Bài đăng {post.Content} đã được duyệt thành công.";
			}
			else
			{
				TempData["ErrorMessage"] = $"Bài đăng {post.Content} không được tìm thấy.";
			}
			string encodedUrl = Url.Action("ManagePosts", new { currentTab = "approvedPosts" });
			return Redirect(encodedUrl);

		}

		[Authorize]
		[HttpPost]
		public IActionResult FinishPost(int id)
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
			string encodedUrl = Url.Action("ManagePosts", new { currentTab = "approvedPosts" });
			return Redirect(encodedUrl);

		}
		[Authorize]
		[HttpPost]
		public IActionResult AddLocation(string location, int id)
		{
			var post = _db.Posts.FirstOrDefault(p => p.IdPost == id);
			if (post != null && location != null)
			{
				post.Location = location;
				_db.Posts.Update(post);
				_db.SaveChanges();
				TempData["SuccessMessage"] = $"Đã thêm vị trí cho bài post {post.Content} thành công !";
			}
			else
			{
				TempData["ErrorMessage"] = $"Thêm vị trí cho bài post {post.Content} thất bại !";
			}

			string encodedUrl = Url.Action("ManagePosts", new { currentTab = "approvedPosts" });
			return Redirect(encodedUrl);
		}

	}

}
