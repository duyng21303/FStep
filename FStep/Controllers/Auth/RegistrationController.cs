using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.Repostory.Interface;
using FStep.Repostory.Service;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace FStep.Controllers.Auth
{
    public class RegistrationController : Controller
    {
        private readonly FstepDBContext db;
        private readonly IMapper _mapper;
        private readonly IEmailSender emailSender;

        public RegistrationController(FstepDBContext context, IMapper mapper, IEmailSender emailSender)
        {
            db = context;
            _mapper = mapper;
            this.emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
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
                            ModelState.AddModelError("Error", "There was an error sending the email.Please try again later.");

                        }
                        else
                        {
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
                    // Log the exception (use a logging framework)
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("Error", "An error occurred while processing your request.");
                }
            }
            return View(model); ;
        }
    }
}
