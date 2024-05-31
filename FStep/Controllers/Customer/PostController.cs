using AutoMapper;
using FStep.Data;
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

		//[HttpGet]
		//public IActionResult CreatePost()
		//{
		//	return View();
		//}
		[HttpGet]
		public IActionResult CreateProduct()
		{
			return View();
		}

		//Generate product before generate Post
		[Authorize]
		[HttpPost]
		public IActionResult CreateProduct(PostVM model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var temp = model;
					var product = _mapper.Map<Product>(temp);
					product.Name = temp.NameProduct;
					product.Quantity = temp.Quantity;
					product.Price = temp.Price;
					product.Status = true;
					product.Detail = temp.DetailProduct;
					db.Add(product);
					db.SaveChanges();

					return RedirectToAction("CreatePost",model); //return model for createPost action to create new post
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
			return View();
		}

		//Create post
		[Authorize]
		public IActionResult CreatePost(PostVM model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var post = _mapper.Map<Post>(model);
					post.Content = model.Content;
					post.Date = DateTime.Now;
					//Helpers.Util.UpLoadImg(model.Img, "")
					post.Img = Helpers.Util.UpLoadImg(model.Img, "pic");
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
			}

			return View();
		}

		//[HttpPost]
		//public IActionResult CreateExchangePost(ExchangePostVM model)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		try
		//		{
		//			var post = _mapper.Map<Post>(model);
		//			post.Content = model.Content;
		//			post.Date = DateTime.Now;
		//			post.Img = "";
		//			post.Status = true;
		//			post.Type = "Exchange";
		//			post.Detail = model.Detail;
		//			post.IdUser = User.FindFirst("UserID").Value;
		//				/*User.FindFirst("UserID").ToString();*/

		//			db.Add(post);
		//			db.SaveChanges();
		//			return Redirect("/"); 
		//		}
		//		catch (Exception ex)
		//		{
		//			Console.WriteLine(ex);
		//		}
		//	}

		//	return View();
		//}
		//public IActionResult CreateSalePost(SalePostVM model)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		try
		//		{
		//			var post = _mapper.Map<Post>(model);
		//			post.Content = model.Content;
		//			post.Date = DateTime.Now;
		//			post.Img = "";
		//			post.Status = true;
		//			post.Type = "Exchange";
		//			post.Detail = model.Detail;

		//			var userid = User.FindFirst("UserID");
		//			post.IdUser = userid.ToString();

		//			var product = _mapper.Map<Product>(model);
		//			product.Name = model.NameProduct;
		//			product.Quantity = model.Quantity;
		//			product.Price = model.Price;
		//			product.Detail = model.DetailProduct;

		//			db.Add(product);
		//			//post.IdProduct = db.Products.Select(p =>  p.IdProduct).FirstOrDefault();
		//			db.Add(post);
		//			db.SaveChanges();

		//			return Redirect("/"); 
		//		}
		//		catch (Exception ex)
		//		{
		//			Console.WriteLine(ex);
		//		}
		//	}

		//	return View();
		//}


	}
}
