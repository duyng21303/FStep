using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using FStep.Repostory.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text;
namespace FStep.Controllers.Auth
{
    public class RegistrationController : Controller
    {
        private readonly FstepDbContext db;
        private readonly IMapper _mapper;
        private readonly IEmailSender emailSender;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(FstepDBContext context, IMapper mapper, IEmailSender emailSender, ILogger<RegistrationController> logger)
        {
            db = context;
            _mapper = mapper;
            this.emailSender = emailSender;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //sử dụng Anyđể kiểm tra sự tồn tại.
                    if (db.Users.Any(user => user.IdUser == model.username))
                    {
                        ModelState.AddModelError("Error", "Tên đăng nhập đã tồn tại");
                    }
                    else if (db.Users.Any(user => user.Email == model.email))
                    {
                        ModelState.AddModelError("Error", "Địa chỉ email đã được sử dụng");
                    }
                    else
                    {
                        bool emailSent = await emailSender.EmailSendAsync(model.email, "Account Created", "Congratulations, Your account has been successfully created");

                        if (!emailSent)
                        {
                            ModelState.AddModelError("Error", "There was an error sending the email. Please try again later.");
                        }
                        else
                        {
                            // Only save the user if the email was sent successfully
                            var user = _mapper.Map<User>(model);
                            user.IdUser = model.username;
                            user.HashKey = Util.GenerateRandomKey();
                            user.Password = model.password.ToMd5Hash(user.HashKey);
                            user.Role = "Customer";
                            db.Add(user);
                            db.SaveChanges();

                            return RedirectToAction("Login", "Account");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Can't send your email ");
                    ModelState.AddModelError("Error", "An error occurred while processing your request.");
                }
            }
            return View(model); ;
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

