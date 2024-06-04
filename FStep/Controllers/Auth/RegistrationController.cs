using AutoMapper;
using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace FStep.Controllers.Auth
{
    public class RegistrationController : Controller
    {
        private readonly FstepDBContext db;
        private readonly IMapper _mapper;

        public RegistrationController(FstepDBContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterVM model)
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
                catch (Exception ex)
                {
                    // Log the exception (use a logging framework)
                    ModelState.AddModelError("Error", "An error occurred while processing your request.");
                }
            }
            return View(model); ;
        }
    }
}
