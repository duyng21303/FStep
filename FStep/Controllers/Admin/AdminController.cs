﻿using AutoMapper;
using FStep.Data;
using FStep.Models;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FStep.Controllers.Admin
{
	public class AdminController : Controller
	{
		private readonly FstepDBContext _context;
		private readonly IMapper _mapper;
		public AdminController(FstepDBContext context, IMapper mapper)

		{
			_context = context;
			_mapper = mapper;
		}

		public IActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// This method is used to manage users
		/// </summary>
		/// <param name="page">The page number</param>
		/// <param name="pageSize">The number of items per page</param>
		/// <param name="search">The search string: Name, Email</param>
		public IActionResult UserManager(int page = 1, int pageSize = 10, string? search = null)
		{
			var query = _context.Users.AsQueryable();
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

                // Map thông tin người dùng sang ProfileVM
                var profileVM = _mapper.Map<ProfileVM>(user);

                // Truyền số bài đăng vào ViewModel hoặc ViewData
                ViewBag.SalePostCount = saleCount;
                ViewBag.ExchangePostCount = exchangeCount;

                return View("UserDetail", profileVM);
            }

            return View("UserDetail");
        }


    }
}