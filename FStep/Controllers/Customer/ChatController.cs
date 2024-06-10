using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FStep.Controllers.Customer
{
    public class ChatController : Controller
    {
        private readonly FstepDbContext db;

        public ChatController(FstepDbContext context) 
        {
            db = context;
        }
        [HttpPost]
        public IActionResult GetChat(string idUser)
        {
			var chatMessages = new List<ChatVM>
			{
                // Thêm các tin nhắn mẫu
                new ChatVM { ChatMsg = "Hi, how are you?"},
				new ChatVM { ChatMsg = "I'm fine, thank you!"},
				new ChatVM { ChatMsg = "What about our meeting?"}
			};

			return PartialView("_ChatMessages", chatMessages);
		}
    }
}
