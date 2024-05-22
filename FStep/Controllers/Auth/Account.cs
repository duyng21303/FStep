using FStep.Data;
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
		public IActionResult Login(string? ReturnUrl)
		{
			ViewBag.ReturnUrl = ReturnUrl;
			return View();
		}
		[HttpPost]
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
							var role = db.Roles.SingleOrDefault(role => role.IdRole == user.IdRole).RoleName;
							var claims = new List<Claim>
							{
								new Claim(ClaimTypes.Email, user.Email),
								new Claim(ClaimTypes.Name, user.Name),
								new Claim("UserID", user.IdUser),
                                //claim-role động
                                new Claim(ClaimTypes.Role, role)
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
