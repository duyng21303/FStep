using FStep.Data;
using Microsoft.AspNetCore.Mvc;

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


	}
}
