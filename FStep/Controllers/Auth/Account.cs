﻿using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FStep.Controllers.Auth
{
	public class Account : Controller
	{
		private readonly Fstep1Context db;

		public Account(Fstep1Context context) 
		{
			db = context;
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
			if(ModelState.IsValid)
			{
				var user = db.Users.SingleOrDefault(user => user.IdUser == model.userName);
				if(user == null)
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
						if(user.Password != model.password)
						{
							ModelState.AddModelError("Error", "Sai tên đăng nhập hoặc mật khẩu");
						}
						else
						{

							var claims = new List<Claim>
							{
								new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
								new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
								new Claim("UserID", user.IdUser ?? string.Empty),
                                //claim-role động
                                new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
							};
							var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
							var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
							await HttpContext.SignInAsync(claimsPrincipal);
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
		public IActionResult Profile()
		{
			return View();
		}
	}
}