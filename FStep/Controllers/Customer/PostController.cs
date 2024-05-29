using AutoMapper;
using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.ComponentModel;

namespace FStep.Controllers.Customer
{
	public class PostController : Controller
	{
		private readonly FstepContext db;
		private readonly IMapper _mapper;

		public PostController(FstepContext context, IMapper mapper)
		{
			db = context;
			_mapper = mapper;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult CreatePost()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CreatePost(PostVM model, ProductVM product)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var post = _mapper.Map<Post>(model);
					post.Content = model.Content;
					post.Date = DateTime.Now;
					post.Status = true;
					post.Type = model.Type;
					post.Detail = model.Detail;

					var UserName = HttpContext.Session.GetString("USER_ID");
					var userID = db.Users.Where(User => User.Name == UserName).Select(user => user.IdUser);

					post.IdUser = userID.ToString();

					db.Add(post);
					db.SaveChanges();

					return RedirectToAction("Index", "Home");
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
