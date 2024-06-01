using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FStep.Helpers;
using Newtonsoft.Json;
using System.Net.Http.Headers;

using AutoMapper;
using System.IO;

namespace FStep.Controllers.Auth
{
	public class AccountController : Controller
	{

		private readonly IMapper _mapper;

		private readonly FstepDBContext db;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;

		public AccountController(FstepDBContext context, IMapper mapper)

		{
			db = context;

			_mapper = mapper;
		}
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login(string? ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;

			return View();
		}
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;
			if (ModelState.IsValid)
			{
				var user = db.Users.SingleOrDefault(user => user.IdUser == model.userName);
				if (user == null)
				{
					ModelState.AddModelError("Error", "Sai tên đăng nhập hoặc mật khẩu");
				}
				else
				{
					if (user.Status == false)
					{
						ModelState.AddModelError("Error", "Tài khoản đã bị khóa, vui lòng liên hệ Admin hoặc Moder");
					}
					else
					{
						if (user.Password != model.password.ToMd5Hash(user.HashKey))
						{
							ModelState.AddModelError("Error", "Sai tên đăng nhập hoặc mật khẩu");
						}
						else
						{
							await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, Util.ClaimsHelper(user));
							if (Url.IsLocalUrl(ReturnUrl))
							{
								return Redirect(ReturnUrl);
							}
							else
							{
								return Redirect("/");
							}
						}
					}
				}
			}
			return View();
		}
		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			return Redirect("/");
		}
		[Authorize]
		[HttpGet]
		public IActionResult Profile()
		{
			string userID = User.FindFirstValue("UserID");
            var user = db.Users.SingleOrDefault(user => user.IdUser == userID);
			var profile = new ProfileVM()
			{
				IdUser = user.IdUser,
				Address = user.Address,
				AvatarImg = User.FindFirstValue("IMG"),
				Email = user.Email,
				Name = user.Name,
				Rating = user.Rating,
				StudentId = user.StudentId
			};
            return View(profile);
		}

		[HttpPost]
		public async Task<IActionResult> ProfileImg(IFormFile img)
		{
			try
			{
				string userID = User.FindFirstValue("UserID");
				var user = db.Users.SingleOrDefault(user => user.IdUser == userID);
				if (img != null)
				{
					FileInfo fileInfo = new FileInfo("wwwroot/img/userAvar/" + user.AvatarImg);
					if (fileInfo.Exists)
					{
						fileInfo.Delete();
					}
					user.AvatarImg = Util.UpLoadImg(img, "userAvar");
				}
				db.Update(user);
				db.SaveChanges();
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, Util.ClaimsHelper(user));
				return RedirectToAction("Profile");
			}catch(Exception ex) { }
			return View();
		}

		[HttpGet]
		public IActionResult ForgotPassword()
		{
			return View();
		}
	}
}