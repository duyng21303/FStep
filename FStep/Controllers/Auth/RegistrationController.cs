using AutoMapper;
using FStep.Helpers;
using FStep.Data;
using FStep.Helpers;
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
					if (db.Users.SingleOrDefault(user => user.IdUser == model.username) != null)
					{
						ModelState.AddModelError("Error", "Tên đăng nhập đã tồn tại");
					}
					else
					{
						var user = _mapper.Map<User>(model);
						user.IdUser = model.username;
						user.HashKey = Util.GenerateRandomKey();
						user.Password = model.password.ToMd5Hash(user.HashKey);
						user.Email = model.email;
						user.Role = "Customer";
						db.Add(user);
						db.SaveChanges();
						return Redirect("/");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
			return View();
		}
	}
}
