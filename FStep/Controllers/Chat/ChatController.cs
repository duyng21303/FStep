using FStep.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FStep.Controllers.Chat
{
	public class ChatController : Controller
	{
		
		[HttpGet]
		public IActionResult LoadChatComponent(string userId)
		{
			HttpContext.Session.SetString("USER_RECIEVE", userId);
			return View();
		}
	}
}
