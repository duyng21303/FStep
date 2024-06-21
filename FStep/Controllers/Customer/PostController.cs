using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

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
				product.Quantity = model.Quantity;
				product.Price = model.Price;
				product.Status = "true";
				db.Add(product);
				db.SaveChanges();

				var post = _mapper.Map<Post>(model);
				post.Content = model.Title;
				post.Date = DateTime.Now;
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
		public IActionResult DetailPost(int id)
		{
			var data = db.Posts.Include(x => x.IdProductNavigation).Include(x => x.IdUserNavigation).SingleOrDefault(p => p.IdPost == id);
			var user = db.Users.SingleOrDefault(user => user.IdUser == data.IdUser);

			ViewData["USER_CREATE"] = user;

			// lấy thêm comment sản phẩm
			var comments = db.Comments.Where(x => x.IdPost == id).Include(x => x.IdUserNavigation).Select(x => new CommentVM
			{
				IdPost = id,
				IdUser = x.IdUser,
				Content = x.Content,
				Date = x.Date,
				IdComment = x.IdComment,
				Name = x.IdUserNavigation.Name,
				AvatarImg = x.IdUserNavigation.AvatarImg // Adjust this property name to match your actual property name for the user's image
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
			var post = db.Posts.Include(x => x.IdProductNavigation).SingleOrDefault(post => post.IdPost == id);
			var user = db.Users.SingleOrDefault(user => user.IdUser == post.IdUser);

			var feedback = db.Feedbacks.Count(x => x.IdPost == id);
			ViewData["USER_CREATE"] = user;

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

			var result = new PostVM()
			{
				IdPost = post.IdPost,
				Title = post.Content,
				Quantity = post.IdProductNavigation?.Quantity,
				Img = post.Img,
				Description = post.Detail,
				CreateDate = post.Date,
				Price = post.IdProductNavigation?.Price ?? 0,
				SoldQuantity = post.IdProductNavigation?.SoldQuantity ?? 0,
				FeedbackNum = feedback
			};

			var currentProductPrice = post.IdProductNavigation?.Price;

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
		public IActionResult PostComment([FromForm] CommentVM comment, IFormFile image)
		{
			string refererUrl = Request.Headers["Referer"].ToString();
			try
			{
				if (string.IsNullOrWhiteSpace(comment.Content))
				{
					TempData["CommentError"] = "Comment content cannot be empty.";
					if (!string.IsNullOrEmpty(refererUrl))
					{
						return Redirect(refererUrl);
					}
					return RedirectToAction("DetailPost", "Post", new { id = comment.IdPost });
				}

				var isAuthenticated = User?.Identity?.IsAuthenticated;
				if (isAuthenticated == true)
				{
					comment.IdUser = User.FindFirst("UserID")?.Value;
					comment.Date = DateTime.Now;

					// Handle image upload
					if (image != null)
					{
						var folder = "comments";
						comment.Img = Util.UpLoadImg(image, folder);
					}

					var saveComment = _mapper.Map<Comment>(comment);
					saveComment.Reports = null;
					db.Comments.Add(saveComment);
					db.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error posting comment: {ex.Message}");

			}

			if (!string.IsNullOrEmpty(refererUrl))
			{
				return Redirect(refererUrl);
			}

			return RedirectToAction("DetailPost", "Post", new { id = comment.IdPost });
		}


		public ActionResult _CreatePost()
		{
			return PartialView();
		}

	}
}
