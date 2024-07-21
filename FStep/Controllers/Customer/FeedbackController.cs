using FStep.Data;
using FStep.Helpers;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Security.Policy;

namespace FStep.Controllers.Customer
{
	public class FeedbackController : Controller
	{
		private readonly FstepDBContext db;


		public FeedbackController(FstepDBContext context) => db = context;
		
		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		[HttpGet]
		public IActionResult Feedback(int idPost, int idTransaction, string url)
		{
			var feedback = new FeedbackVM();
			feedback.IdPost = idPost;
			feedback.Img = db.Posts.FirstOrDefault(p => p.IdPost == idPost).Img;
			feedback.Title = db.Posts.FirstOrDefault(p => p.IdPost == idPost).Content;
			feedback.Description = db.Posts.FirstOrDefault(p => p.IdPost == idPost).Detail;
			feedback.TypePost = db.Posts.FirstOrDefault(p => p.IdPost == idPost).Type;
			feedback.Quantity = db.Transactions.FirstOrDefault(p => p.IdTransaction == idTransaction).Quantity;
			feedback.UnitPrice = db.Transactions.FirstOrDefault(p => p.IdTransaction == idTransaction).UnitPrice;
			feedback.Amount = db.Transactions.FirstOrDefault(p => p.IdTransaction == idTransaction).Amount;
			feedback.ReturnUrl = url;

			HttpContext.Session.Set<FeedbackVM>("FEEDBACK_INFO", feedback);

			return View("Feedback", feedback);
		}
		[Authorize]
		[HttpPost]
		public ActionResult Feedback(FeedbackVM model)
		{
			try
			{
				FeedbackVM info = HttpContext.Session.Get<FeedbackVM>("FEEDBACK_INFO");
				var feedback = new Feedback();
				feedback.Content = model.Content;
				feedback.Rating = int.Parse(model.Rating);
				feedback.IdUser = User.FindFirst("UserID").Value;
				feedback.IdPost = info.IdPost;
				db.Add(feedback);
				db.SaveChanges();

				// Thực hiện tính điểm
				if (feedback.Rating != 3)
				{
					var idUser = db.Posts.FirstOrDefault(x => x.IdPost == feedback.IdPost).IdUser;
					var user = db.Users.FirstOrDefault(x => x.IdUser == idUser);
					var oldPoint = user.PointRating;
					user.PointRating = feedback.Rating switch
					{
						1 => oldPoint - 2,
						2 => oldPoint - 1,
						4 => oldPoint + 1,
						5 => oldPoint + 2,
					};

					if (user.PointRating < 20)
					{
						user.Status = false;
					}

					db.Users.Update(user);
					db.SaveChanges();
				}
				return Redirect(info.ReturnUrl);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return Redirect("/Home/TransactionHistory");
		}
		

	}
}
