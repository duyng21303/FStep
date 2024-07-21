using AutoMapper;
using FStep.Data;
using FStep.Models;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FStep.Controllers.ManagePost
{
	public class ModeManageUserController : Controller
	{

		private readonly FstepDBContext _context;
		private readonly IMapper _mapper;
		private static readonly string[] defaultRole = new[] { "Customer", "Moderator", "Administrator" };

		public ModeManageUserController(FstepDBContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		[Authorize(Roles = "Moderator")]
		public IActionResult UserManager(int page = 1, int pageSize = 10, string? search = null)
		{
			string id = User.FindFirst("UserID")?.Value;
			var query = _context.Users.Where(user => user.IdUser != id).AsQueryable();
			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()) || x.Email.ToLower().Contains(search.ToLower()));
			}

			var users = query.Skip((page - 1) * pageSize).Take(pageSize).Select(x => _mapper.Map<ProfileVM>(x)).ToList();
			PagingModel<ProfileVM> pagingModel = new()
			{
				Items = users,
				PagingInfo = new PagingInfo
				{
					CurrentPage = page,
					ItemsPerPage = pageSize,
					TotalItems = query.Count(),
					Search = search
				}
			};

			return View("UserManager", pagingModel);
		}
		[Authorize(Roles = "Admin, Moderator")]
		public IActionResult UserDetail(string id)
		{
			// Lấy thông tin người dùng
			var user = _context.Users.FirstOrDefault(user => user.IdUser == id);

			if (user != null)
			{
				// Đếm số bài đăng của người dùng có type là Sale
				var saleCount = _context.Posts.Count(post => post.IdUser == id && post.Type == "Sale");

				// Đếm số bài đăng của người dùng có type là Exchange
				var exchangeCount = _context.Posts.Count(post => post.IdUser == id && post.Type == "Exchange");

				var totalPost = saleCount + exchangeCount;

				var totalTransaction = _context.Transactions.Count(t => t.IdUserBuyer == id && t.Status != "Processing");
				// Map thông tin người dùng sang ProfileVM
				var profileVM = _mapper.Map<ProfileVM>(user);

				// Truyền số bài đăng vào ViewModel hoặc ViewData
				ViewBag.TotalPost = totalPost;
				ViewBag.TotalTransaction = totalTransaction;

				return View("UserDetail", profileVM);
			}

			return View("UserDetail");
		}

		[Authorize(Roles = "Moderator")]
		public IActionResult ProcessPoint(String? id)
		{
			var user = _context.Users.FirstOrDefault(user => user.IdUser == id);
			if (user == null)
			{
				TempData["SuccessMessage"] = $"Người dùng {user.Name} không tồn tại.";
			}
			return View();
		}

	}
}