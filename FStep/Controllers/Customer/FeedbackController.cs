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
		public IActionResult Feedback(int idPost, int idTransaction)
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
			return View("Feedback", feedback);
		}
		[Authorize]
		[HttpPost]
		public IActionResult Feedback(FeedbackVM model)
		{
			try
			{
				var feedback = new Feedback(); 
				feedback.Content = model.Content;
				feedback.Rating = int.Parse(model.Rating); 
				feedback.IdUser = User.FindFirst("UserID").Value;
				feedback.IdPost = model.IdPost;
				db.Add(feedback);
				db.SaveChanges();
				return Redirect("/");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return Redirect("/Home/TransactionHistory");
		}
	}
}
