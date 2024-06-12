using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
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
		public IViewComponentResult Invoke(string? userid)
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
			}).OrderBy(p => p.ChatDate).ToList();
			if(userid != null)
			{
				var newUser = db.Users.SingleOrDefault(user => user.IdUser.Equals(userid));
				result.Add(new ChatVM()
				{
					ChatDate = DateTime.Now,
					SenderUser = db.Users.SingleOrDefault(user => user.IdUser.Equals(userIdString)),
					RecieverUser = db.Users.SingleOrDefault(user => user.IdUser.Equals(userid)),
					ChatMsg = null,
					IdPost = null
				});
			}
			result = result.OrderBy(p => p.ChatDate).Reverse().ToList();
			return View(result);
		}
	}
}
