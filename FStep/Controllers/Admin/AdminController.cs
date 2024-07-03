﻿using AutoMapper;
using FStep.Data;
using FStep.Models;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FStep.Controllers.Admin
{
	public class AdminController : Controller
	{
		private readonly FstepDbContext _context;
		private readonly IMapper _mapper;
		private static readonly string[] defaultRole = new[] { "Customer", "Moderator", "Administrator" };

		public AdminController(FstepDbContext context, IMapper mapper)


		{
			_context = context;
			_mapper = mapper;
		}
		public IActionResult Index()
		{
			var totalPost = _context.Posts.Count(p => p.Status == "true" || p.Status == "finish");

			var totalUser = _context.Users.Count(u => u.Status == true);

			var totalTransaction = _context.Transactions.Count(t => t.Status == "Finish" || t.Status == "Processing");

			var totalRevenue = _context.Transactions
				.Where(t => t.Status == "Finish")
				.Sum(t => t.Amount);

			
			ViewBag.TotalPost = totalPost;
			ViewBag.TotalTransaction = totalTransaction;
			ViewBag.TotalUser = totalUser;
			ViewBag.TotalRevenue = totalRevenue;
			return View();
		}

        /// <summary>
        /// This method is used to manage users
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <param name="search">The search string: Name, Email</param>
        /// 
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult CommentManager(int page = 1, int pageSize = 10, string? search = null)
		{
			var query = _context.Comments.Include(x => x.IdUserNavigation).Include(x => x.IdPostNavigation).AsQueryable();
			if (!string.IsNullOrEmpty(search))
			{
				query = query.Where(x => x.Content.Contains(search) || x.Type.Contains(search));
			}

			var comments = query.Skip((page - 1) * pageSize).Take(pageSize).Select(x => new CommentVM
			{
				IdPost = x.IdPost,
				IdUser = x.IdUser,
				Content = x.Content,
				Date = x.Date,
				IdComment = x.IdComment,
				PostName = x.IdPostNavigation.Content,
				Name = x.IdUserNavigation.Name,
				Type = x.Type,
				Img = x.Img,
				avarImg = x.IdUserNavigation.AvatarImg
			}).ToList();

			PagingModel<CommentVM> pagingModel = new()
			{
				Items = comments,
				PagingInfo = new PagingInfo
				{
					CurrentPage = page,
					ItemsPerPage = pageSize,
					TotalItems = query.Count(),
					Search = search
				}
			};

			return View("CommentManager", pagingModel);
		}
        [Authorize(Roles = "Admin")]
        [HttpPost]
		public IActionResult LockUnlock([FromBody] ProfileVM user)
		{
			var userFound = _context.Users.FirstOrDefault(x => x.IdUser == user.IdUser);
			if (userFound != null)
			{
				userFound.Status = user.Status;
				_context.Users.Update(userFound);
				_context.SaveChanges();
				return Ok();

			}
			return BadRequest();
		}
        [Authorize(Roles = "Admin")]
        [HttpPost]
		public IActionResult ChangeRole([FromBody] ProfileVM user)
		{
			var userFound = _context.Users.FirstOrDefault(x => x.IdUser == user.IdUser);
			if (userFound != null && defaultRole.Contains(user.Role))
			{
				userFound.Role = user.Role;
				_context.Users.Update(userFound);
				_context.SaveChanges();
				return Ok();
			}
			return BadRequest();
		}
        [Authorize(Roles = "Admin")]
        [HttpPost]
		public IActionResult DeleteComment([FromBody] CommentVM comment)
		{
			var commentExisted = _context.Comments.FirstOrDefault(x => x.IdComment == comment.IdComment);
			if (commentExisted != null)
			{
				_context.Comments.Remove(commentExisted);
				_context.SaveChanges();
				return Ok();
			}
			return BadRequest();
		}
	}
}
