using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FStep.ViewComponents
{
	public class ChatViewComponent : ViewComponent
	{
		private readonly FstepDBContext db;

		public ChatViewComponent(FstepDBContext context)
		{
			db = context;
		}
		[HttpGet]
		[Authorize]
		public IViewComponentResult Invoke()
		{
			var chat = db.Chats.AsQueryable();
			var userIdString = HttpContext.User.FindFirstValue("UserID");

			// Kiểm tra xem userIdString có giá trị không rỗng
			if (!string.IsNullOrEmpty(userIdString))
			{
				// Chuyển đổi userIdString sang kiểu int
					chat = chat.Where(p => p.SenderUserId == userIdString || p.RecieverUserId == userIdString);
			}
			var result = chat.Select(p => new ChatVM
			{
				ChatDate = p.ChatDate,
				SenderUser = db.Users.SingleOrDefault(user => user.IdUser.Equals(p.SenderUserId)),
				RecieverUser = db.Users.SingleOrDefault(user => user.IdUser.Equals(p.RecieverUserId)),
				ChatMsg = p.ChatMsg,
				IdPost = p.IdPost 
			}).OrderBy(p => p.ChatDate);
			return View(result);
		}
	}
}
