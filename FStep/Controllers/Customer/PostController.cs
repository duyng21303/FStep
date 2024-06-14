using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FStep.Controllers.Customer
{
	public class PostController : Controller
	{
		private readonly FstepDBContext db;
		private readonly IMapper _mapper;

		public PostController(FstepDBContext context, IMapper mapper)
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
				product.Status = "true";
				product.Detail = model.DetailProduct;
				db.Add(product);
				db.SaveChanges();

				var post = _mapper.Map<Post>(model);
				post.Content = model.Title;
				post.Date = DateTime.Now;
				//Helpers.Util.UpLoadImg(model.Img, "")
				post.Img = Util.UpLoadImg(img, "postPic");
				post.Status = "false";
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
				product.Status = "true";
				product.Detail = model.DetailProduct;
				db.Add(product);
				db.SaveChanges();

				var post = _mapper.Map<Post>(model);
				post.Content = model.Title;
				post.Date = DateTime.Now;
				//Helpers.Util.UpLoadImg(model.Img, "")
				post.Img = Util.UpLoadImg(img, "postPic");
				post.Status = "true";
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
		public IActionResult DetailPost(int id)
		{
			// Lấy thông tin bài đăng
			var data = db.Posts
						 .Include(x => x.IdProductNavigation)
						 .Include(x => x.IdUserNavigation)
						 .SingleOrDefault(p => p.IdPost == id);

			if (data == null)
			{
				return NotFound();
			}

			// Thông tin người dùng tạo bài đăng
			ViewData["USER_CREATE"] = data.IdUserNavigation;


			// Lấy các bình luận cho bài đăng
			var comments = db.Comments
							 .Where(x => x.IdPost == id)
							 .Include(x => x.IdUserNavigation)
							 .Select(x => new CommentVM
							 {
								 IdPost = id,
								 IdUser = x.IdUser,
								 Content = x.Content,
								 Date = x.Date,
								 IdComment = x.IdComment,
								 Name = x.IdUserNavigation.Name,
								 AvatarImg = x.IdUserNavigation.AvatarImg
							 }).ToList();

			ViewData["comments"] = comments;

			// Lấy giá của sản phẩm hiện tại
			var currentProductPrice = data.IdProductNavigation?.Price;

			// Truy vấn các bài đăng chứa sản phẩm đề xuất trong khoảng giá ±1 triệu đồng
			var recommendedPosts = db.Posts
									 .Include(p => p.IdProductNavigation)
									 .Include(p => p.IdUserNavigation)
									 .Where(p => p.IdProductNavigation.Price >= currentProductPrice - 1000000
												 && p.IdProductNavigation.Price <= currentProductPrice + 1000000
												 && p.Type == "Exchange"
												 && p.IdPost != id)
									 .ToList();

			ViewData["recommendedPosts"] = recommendedPosts;

			return View(data);
		}
		public IActionResult DetailSalePost(int id)
		{
			var data = db.Posts
						 .Include(x => x.IdProductNavigation)
						 .Include(x => x.IdUserNavigation)
						 .SingleOrDefault(p => p.IdPost == id);

			if (data == null)
			{
				return NotFound();
			}

			ViewData["USER_CREATE"] = data.IdUserNavigation;

			// lấy thêm comment sản phẩm
			var comments = db.Comments.Where(x => x.IdPost == id).Include(x => x.IdUserNavigation).Select(x => new CommentVM
			{
				IdPost = id,
				IdUser = x.IdUser,
				Content = x.Content,
				Date = x.Date,
				IdComment = x.IdComment,
				Name = x.IdUserNavigation.Name,
				AvatarImg = x.IdUserNavigation.AvatarImg
			}).ToList();

			ViewData["comments"] = comments;

			var result = new SalePostVM()
			{
				Id = data.IdPost,
				Title = data.Content,
				Quantity = data.IdProductNavigation.Quantity,
				Img = data.Img,
				Description = data.Detail,
				NameProduct = data.IdProductNavigation.Name,
				DetailProduct = data.IdProductNavigation.Detail,
				CreateDate = data.Date,
				Price = data.IdProductNavigation.Price ?? 0
			};

			// Lấy giá của sản phẩm hiện tại
			var currentProductPrice = data.IdProductNavigation?.Price;

			// Truy vấn các bài đăng chứa sản phẩm đề xuất trong khoảng giá ±1 triệu đồng
			var recommendedSales = db.Posts
									 .Include(p => p.IdProductNavigation)
									 .Include(p => p.IdUserNavigation)
									 .Where(p => p.IdProductNavigation.Price >= currentProductPrice - 1000000
												 && p.IdProductNavigation.Price <= currentProductPrice + 1000000
												 && p.Type == "Sale"
												 && p.IdPost != id)
									 .ToList();

			ViewData["recommendedSales"] = recommendedSales;

			return View(result);
		}

		[HttpPost]
		public IActionResult PostComment([FromForm] CommentVM comment)
		{
			string refererUrl = Request.Headers["Referer"].ToString();
			try
			{
				var isAuthenticated = User?.Identity?.IsAuthenticated;
				if (isAuthenticated == true)
				{
					comment.IdUser = User.FindFirst("UserID")?.Value;
					comment.Date = DateTime.Now;
					var saveComment = _mapper.Map<Comment>(comment);
					saveComment.Reports = null;
					saveComment.UserNotifications = null;
					db.Comments.Add(saveComment);
					db.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				var exc = ex;
			}
			if (!string.IsNullOrEmpty(refererUrl))
			{
				return Redirect(refererUrl);
			}
			return RedirectToAction("DetailPost", "Post", new { id = comment.IdPost });
		}
	}
}

