using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.ComponentModel;
using System.Security.Claims;

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
		//Create post
		[Authorize]
		[HttpPost]
		public IActionResult CreatePost(PostVM model, IFormFile img)
		{
			try
			{
				var product = _mapper.Map<Product>(model);
				product.Name = model.NameProduct;
				product.Quantity = model.Quantity;
				product.Price = model.Price;
				product.Status = true;
				product.Detail = model.DetailProduct;
				db.Add(product);
				db.SaveChanges();

				var post = _mapper.Map<Post>(model);
				post.Content = model.Content;
				post.Date = DateTime.Now;
				//Helpers.Util.UpLoadImg(model.Img, "")
				post.Img = Util.UpLoadImg(img, "postPic");
				post.Status = true;
				post.Type = model.Type;
				post.Detail = model.Detail;
				post.IdUser = User.FindFirst("UserID").Value;
				//get IdProduct from database map to Post
				post.IdProduct = db.Products.Max(p => p.IdProduct);
				db.Add(post);
				db.SaveChanges();
				return Redirect("/");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return View();
		}
	}
}
