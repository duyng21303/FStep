using FStep.Data;
using Microsoft.AspNetCore.Mvc;

namespace FStep.Controllers.Customer
{
	public class FeedbackController : Controller
	{
		private readonly FstepDbContext db;

		public FeedbackController(FstepDbContext context) => db = context;

		public IActionResult Index()
		{
			return View();
		}


	}
}
