using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
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
			var post = db.Posts.Include(x => x.IdProductNavigation).Include(x => x.IdUserNavigation).SingleOrDefault(p => p.IdPost == id);
			var user = db.Users.SingleOrDefault(user => user.IdUser == post.IdUser);

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
				Type = x.Type,
				Img = x.Img,
				avarImg = x.IdUserNavigation.AvatarImg // Adjust this property name to match your actual property name for the user's image
			}).ToList();

			ViewData["comments"] = comments;

			return View(post);
		}
		public IActionResult DetailSalePost(int id)
		{
			var post = db.Posts.SingleOrDefault(post => post.IdPost == id);

			var product = db.Products.SingleOrDefault(product => product.IdProduct == post.IdProduct);
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
				avarImg = x.IdUserNavigation.AvatarImg // Adjust this property name to match your actual property name for the user's image
			}).ToList();

			ViewData["comments"] = comments;

			var result = new PostVM()
			{
				IdPost = post.IdPost,
				Title = post.Content,
				Quantity = product.Quantity,
				Img = post.Img,
				Description = post.Detail,
				CreateDate = post.Date,
				Price = product.Price ?? 0,
				SoldQuantity = product.SoldQuantity ?? 0,
				FeedbackNum = feedback,
				IdUser = post.IdUser
			};

			return View(result);
		}
		[Authorize]
		[HttpPost]
		public IActionResult PostComment([FromForm] CommentVM comment)
		{
			string refererUrl = Request.Headers["Referer"].ToString();
			try
			{
				comment.IdUser = User.FindFirst("UserID")?.Value;
				comment.Date = DateTime.Now;
				comment.Type = "Comment";
				var saveComment = _mapper.Map<Comment>(comment);
				saveComment.Reports = null;
				db.Comments.Add(saveComment);
				db.SaveChanges();
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
		[Authorize]
		[HttpPost]
		public IActionResult PostCommentExchange([FromForm] CommentVM comment, IFormFile img)
		{
			string refererUrl = Request.Headers["Referer"].ToString();
			try
			{
				comment.IdUser = User.FindFirst("UserID")?.Value;
				comment.Date = DateTime.Now;
				comment.Type = "Exchange";
				comment.Img = Util.UpLoadImg(img, "postPic");
				var saveComment = _mapper.Map<Comment>(comment);
				db.Comments.Add(saveComment);
				db.SaveChanges();
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

		public ActionResult _CreatePost()
		{
			return PartialView();
		}
		[HttpPost]
		public async Task<IActionResult> CreateAnonymousExchage(IFormFile img, string content, int idPost)
		{
			// Xử lý dữ liệu ở đây, ví dụ: lưu ảnh vào thư mục, lưu thông tin vào database
			var message = "Nội dung trao đổi đã được gửi.";

			// Ví dụ lưu file vào thư mục
			if (img != null && img.Length > 0)
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", img.FileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await img.CopyToAsync(stream);
				}
			}

			// Lưu thông tin vào database hoặc xử lý logic khác ở đây

			// Trả về JSON để AJAX có thể sử dụng
			return Json(new { message = message });
		}
	}
}
