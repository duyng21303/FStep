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
<<<<<<< HEAD
=======
using Newtonsoft.Json;
using System.Net.Http.Headers;

>>>>>>> develop
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Google;
using FStep.ViewModels.Email;
using FStep.Repostory.Interface;
using System.Text;
using NuGet.Versioning;
using Microsoft.SqlServer.Server;


namespace FStep.Controllers.Auth
{
    public class AccountController : Controller
    {

        private readonly IMapper _mapper;
        private readonly FstepDbContext db;
        private const string PASSWORD_GOOGLE = "KJDHF";
        private readonly IEmailSender emailSender;
        public AccountController(FstepDbContext context, IMapper mapper, IEmailSender emailSender)

<<<<<<< HEAD
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

=======

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

>>>>>>> develop
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
                var haskKey = Util.GenerateRandomKey();
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
                user.Rating = model.Rating;
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
<<<<<<< HEAD
               
=======

>>>>>>> develop
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
            for (int i = 0; i < 5; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }

    }
}

