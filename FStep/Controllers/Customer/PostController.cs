using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FStep.Controllers.Customer
{
	public class PostController : Controller
	{
		private readonly FstepDbContext db;
		private readonly IMapper _mapper;

		public PostController(FstepDbContext context, IMapper mapper)
		{
			db = context;
			_mapper = mapper;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult CreateExchangePost()
		{
			return View();
		}
		[HttpGet]
		public IActionResult CreateSalePost()
		{
			return View();
		}

		//Create post
		[Authorize]
		[HttpPost]
		public IActionResult CreateExchangePost(ExchangePostVM model, IFormFile img)
		{
			try
			{
				var product = _mapper.Map<Product>(model);
				product.Name = model.NameProduct;
				product.Status = true;
				product.Detail = model.DetailProduct;
				db.Add(product);
				db.SaveChanges();

				var post = _mapper.Map<Post>(model);
				post.Content = model.Title;
				post.Date = DateTime.Now;
				//Helpers.Util.UpLoadImg(model.Img, "")
				post.Img = Util.UpLoadImg(img, "postPic");
				post.Status = true;
				post.Type = model.Type;
				post.Detail = model.Description;
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
		
		[Authorize]
		[HttpPost]
		public IActionResult CreateSalePost(SalePostVM model, IFormFile img)
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
				post.Content = model.Title;
				post.Date = DateTime.Now;
				//Helpers.Util.UpLoadImg(model.Img, "")
				post.Img = Util.UpLoadImg(img, "postPic");
				post.Status = true;
				post.Type = model.Type;
				post.Detail = model.Description;
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

