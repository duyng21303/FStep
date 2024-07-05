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
using Microsoft.AspNetCore.Authentication.Google;
using FStep.ViewModels.Email;
using FStep.Repostory.Interface;
using System.Text;
using NuGet.Protocol;
using X.PagedList;
using System.Data;


namespace FStep.Controllers.Auth
{
	public class AccountController : Controller
	{

		private readonly IMapper _mapper;
		private readonly FstepDbContext db;
		private const string PASSWORD_GOOGLE = "KJDHF";
		private readonly IEmailSender emailSender;
		public AccountController(FstepDbContext context, IMapper mapper, IEmailSender emailSender)

		{
			db = context;
			_mapper = mapper;
			this.emailSender = emailSender;
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
		public IActionResult LoginGoogle()
		{
			var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };

			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}
		public async Task<IActionResult> GoogleResponse()
		{
			var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			if (!authenticateResult.Succeeded)
			{
				return BadRequest(); // Authentication failed
			}
			var state = HttpContext.Request.Query["state"].ToString();

			var claims = authenticateResult.Principal?.Identities.FirstOrDefault()?.Claims;
			// Log claims for debugging
			foreach (var claim in claims)
			{
				Console.WriteLine($"{claim.Type}: {claim.Value}");
			}
			var userID = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
			var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			var img = claims.FirstOrDefault(c => c.Type == "IMG_RAW")?.Value;
			string downloadedImgPath = await Util.DownloadImgGoogle(img, userID, "wwwroot/img/userAvar");
			if (!db.Users.Any(user => user.IdUser == userID))
			{
				var haskKey = Util.GenerateRandomKey(5);
				var user = new User()
				{
					IdUser = userID,
					Name = name,
					Email = email,
					HashKey = haskKey,
					Password = PASSWORD_GOOGLE.ToMd5Hash(haskKey),
					AvatarImg = downloadedImgPath,
					Role = "Customer"
				};
				db.Add(user);
				db.SaveChanges();
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, Util.ClaimsHelper(user)); ;
				return Redirect("/");
			}
			else
			{
				var user = db.Users.SingleOrDefault(user => user.IdUser == userID);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, Util.ClaimsHelper(user)); ;
			}
			// Trả về thông tin người dùng hoặc xử lý theo nhu cầu
			return Redirect("/");
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
			var user = db.Users
				.Include(u => u.Posts)
				.SingleOrDefault(user => user.IdUser == userID);
			var profile = new ProfileVM()
			{
				IdUser = user.IdUser,
				Address = user.Address,
				AvatarImg = User.FindFirstValue("IMG"),
				Email = user.Email,
				Name = user.Name,
				PointRating = user.PointRating,
				StudentId = user.StudentId,
				Posts = user.Posts.Select(p => new PostVM()
				{
					IdPost = p.IdPost,
					Title = p.Content,
					Status = p.Status,
					Description = p.Detail,
					Img = p.Img,
					Type = p.Type,
					Price = p.IdProductNavigation != null && p.IdProductNavigation.Price.HasValue ? p.IdProductNavigation.Price.Value : 0f,
					CreateDate = p.Date.HasValue ? p.Date.Value : DateTime.Now
				}).ToList()

			};
			return View(profile);
		}

		
		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Profile(ProfileVM model)
		{
			try
			{
				string userID = User.FindFirstValue("UserID");
				var user = db.Users.SingleOrDefault(user => user.IdUser == userID);
				user.Address = model.Address;
				user.Email = model.Email;
				user.Name = model.Name;
				user.PointRating = model.PointRating;
				user.StudentId = model.StudentId;
				db.Update(user);
				db.SaveChanges();
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, Util.ClaimsHelper(user)); ;
				return RedirectToAction("Profile");
			}
			catch (Exception ex) { }
			return View();
		}
		[Authorize]
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
			}
			catch (Exception ex) { }
			return View();
		}

		public async Task<IActionResult> ForgetPassword()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forget)
		{
			var user = await db.Users.FirstOrDefaultAsync(user => user.Email == forget.Email);
			if (user != null)
			{

				var code = GenerateToken();
				user.ResetToken = code;

				db.Users.Update(user);
				await db.SaveChangesAsync();

				var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.IdUser, Token = code }, protocol: HttpContext.Request.Scheme);
				bool isSendEmail = await emailSender.EmailSendAsync(forget.Email, "ResetPassword", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">Click Here</a>");
				if (isSendEmail)
				{
					Response response = new Response();
					response.Message = "Reset Password Link";
					return RedirectToAction("ForgetPasswordConfirmation", "Account", response);
				}
			}
			return View();
		}

		public IActionResult ForgetPasswordConfirmation(Response response)
		{

			return View(response);
		}
		[HttpGet]
		public IActionResult ResetPassword(string userId, string Token)
		{
			var user = db.Users.FirstOrDefault(user => user.IdUser == userId);
			var model = new ForgetPasswordVM
			{
				Token = Token,
				UserId = userId,

			};
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ResetPassword(ForgetPasswordVM forget)
		{
			ModelState.Remove("Email");

			var user = await db.Users.FirstOrDefaultAsync(user => user.IdUser == forget.UserId);
			if (user == null || forget.Token != user.ResetToken)
			{
				ModelState.AddModelError("Error", "Invalid token or token has expired!");
				return View(forget);
			}

			//Update the password
			user.Password = forget.Password.ToMd5Hash(user.HashKey);
			user.ResetToken = null;

			db.Users.Update(user);
			await db.SaveChangesAsync();


			ViewBag.Message = "Password updated successfully.";
			return View("Login");
		}

		public static string GenerateToken()
		{
			var pattern = @"ksfjsdkfjhskfnskdfnskdfskvbkxcjvnkcvnosfoxcvnxcivnkjnLSKDLKNGLKFNVLCXNVKCBKJDNGDKOLJVNXCLJVNXLCVN!LSKDFX";
			var sb = new StringBuilder();
			var rd = new Random();
			for (int i = 0; i < 10; i++)
			{
				sb.Append(pattern[rd.Next(0, pattern.Length)]);
			}
			return sb.ToString();
		}


		[HttpGet]
		[Authorize]
		public IActionResult UpdatePost(int id)
		{
			var post = db.Posts
				.Include(p => p.IdProductNavigation)
				.FirstOrDefault(p => p.IdPost == id);

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
				ProductStatus = post.IdProductNavigation.Quantity ?? 1,
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
				var post = db.Posts
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
						post.IdProductNavigation.Quantity = model.ProductStatus;
						post.IdProductNavigation.Price = model.Price;
					}
					post.Detail = model.Description;
					post.Status = "False";

					db.SaveChanges();
					TempData["SuccessMessage"] = $"Bài đăng của bạn đã được sửa thành công.Chúng tôi sẽ xem duyệt và duyệt!";
					return RedirectToAction("Profile", "Account");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật bài đăng của bạn.";
			}
			return View(model);
		}

		public IActionResult FinishPost(int id)
		{
			var post = db.Posts.FirstOrDefault(p => p.IdPost == id);
			if (post != null)
			{
				post.Status = "Finish";
				db.Posts.Update(post);
				db.SaveChanges();
				TempData["SuccessMessage"] = $"Bài đăng {post.Content} đã được xóa thành công.";
			}
			else
			{
				TempData["ErrorMessage"] = $"Bài đăng {post.Content} không được tìm thấy.";
			}
			return RedirectToAction("Profile");
		}
		public IActionResult HiddenPost(int id)
		{
			var post = db.Posts.FirstOrDefault(p => p.IdPost == id);
			if (post != null)
			{
				post.Status = "Hidden";
				db.Posts.Update(post);
				db.SaveChanges();
				TempData["SuccessMessage"] = $"Bài đăng {post.Content} đã được ẩn thành công.";
			}
			else
			{
				TempData["ErrorMessage"] = $"Bài đăng {post.Content} không được tìm thấy.";
			}
			return RedirectToAction("Profile");

		}
		// GET: Account/VerifyInfo
		[HttpGet]
		[Authorize]
		public IActionResult VerifyInfo()
		{
			return View();
		}

		// POST: Account/VerifyInfo
		[HttpPost]
		[Authorize]
		public IActionResult VerifyInfo(VerifyInfoVM model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var userId = User.FindFirst("UserID")?.Value;
					var user = db.Users.FirstOrDefault(p => p.IdUser == userId);
					if (db.Users.Any(p => p.StudentId == model.StudentId))
					{
						ModelState.AddModelError("Error", "MSSV đã được sử dụng");
					}
					else
					{
						user.StudentId = model.StudentId;
						user.BankName = model.BankName;
						user.AccountHolderName = model.AccountHolderName;
						user.BankAccountNumber = model.AccountNumber;
						switch (model.BankName)
						{
							case "TPBANK":
								user.SwiftCode = "TPBVVNVX";
								break;
							case "VIETCOMBANK":
								user.SwiftCode = "BFTVVNVX";
								break;
							case "VIETINBANK":
								user.SwiftCode = "ICBVVNVX";
								break;
							case "TECHCOMBANK":
								user.SwiftCode = "VTCBVNVX";
								break;
							case "MB BANK":
								user.SwiftCode = "MSCBVNVX";
								break;
							case "BIDV":
								user.SwiftCode = "BIDVVNVX";
								break;
						}
						db.Update(user);
						db.SaveChanges();
						return RedirectToAction("Profile", "Account");
					}
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("Error", "An error occurred while processing your request.");

				}
			}
			return View(model);
		}
	}
}
