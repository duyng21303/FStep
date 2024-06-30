using AutoMapper;
using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FStep.Controllers.Customer
{
	public class UserController : Controller
	{
		private readonly FstepDBContext _context;
		private readonly IMapper _mapper;
		public UserController(FstepDBContext context, IMapper mapper)


		{
			_context = context;
			_mapper = mapper;
		}

		public IActionResult Detail(string id)
		{
			// Lấy thông tin người dùng
			var user = _context.Users.FirstOrDefault(user => user.IdUser == id);

			if (user != null)
			{
				// Đếm số bài đăng của người dùng có type là Sale
				var saleCount = _context.Posts.Count(post => post.IdUser == id && post.Type == "Sale");

				// Đếm số bài đăng của người dùng có type là Exchange
				var exchangeCount = _context.Posts.Count(post => post.IdUser == id && post.Type == "Exchange");

				// Map thông tin người dùng sang ProfileVM
				var profileVM = _mapper.Map<ProfileVM>(user);

				// Truyền số bài đăng vào ViewModel hoặc ViewData
				ViewBag.SalePostCount = saleCount;
				ViewBag.ExchangePostCount = exchangeCount;
                return View(profileVM);
            }

            return View();

        }
	}
}
