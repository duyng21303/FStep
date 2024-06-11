using Microsoft.AspNetCore.Mvc;

namespace FStep.Controllers.Chat
{
	public class ChatController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
