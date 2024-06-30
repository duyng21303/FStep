using FStep.Data;
using FStep.Helpers;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Security.Claims;

namespace FStep.ViewComponents
{
    public class ChatViewComponent : ViewComponent
	{
		private readonly FstepDbContext db;

		public ChatViewComponent(FstepDbContext context)
		{
			db = context;
		}

		[HttpGet]
		[Authorize]
		public IViewComponentResult Invoke(string? userid)
		{
			var chat = db.Chats.AsQueryable();
			var userIdString = HttpContext.User.FindFirstValue("UserID");

			if (string.IsNullOrEmpty(userIdString))
			{
				return View(new List<ChatVM>());
			}

			var confirmDbHistory = db.Confirms
				.Where(m => m.IdUserConnect == userIdString || m.IdUserConfirm == userIdString)
				.OrderByDescending(m => m.IdConfirm)
				.ToList();

			var chatUserIds = chat
				.Where(p => p.SenderUserId == userIdString || p.RecieverUserId == userIdString)
				.Select(p => p.SenderUserId == userIdString ? p.RecieverUserId : p.SenderUserId)
				.Distinct()
				.ToList();

			var confirmedUserIds = confirmDbHistory
				.Select(m => m.IdUserConnect == userIdString ? m.IdUserConfirm : m.IdUserConnect)
				.Distinct()
				.ToList();

			var allUserIds = chatUserIds.Union(confirmedUserIds).Distinct().ToList();

			var result = chat
				.Where(p => p.SenderUserId == userIdString || p.RecieverUserId == userIdString)
				.Select(p => new ChatVM
				{
					ChatDate = p.ChatDate,
					SenderUser = db.Users.SingleOrDefault(user => user.IdUser == p.SenderUserId),
					RecieverUser = db.Users.SingleOrDefault(user => user.IdUser == p.RecieverUserId),
					ChatMsg = p.ChatMsg,
					IdPost = p.IdPost
				})
				.OrderBy(p => p.ChatDate)
				.ToList();

			foreach (var userId in allUserIds)
			{
				if (!result.Any(r => r.SenderUser.IdUser == userId || r.RecieverUser.IdUser == userId))
				{
					result.Add(new ChatVM
					{
						ChatDate = DateTime.Now,
						SenderUser = db.Users.SingleOrDefault(user => user.IdUser == userIdString),
						RecieverUser = db.Users.SingleOrDefault(user => user.IdUser == userId),
						ChatMsg = null,
						IdPost = null
					});
				}
			}
			if (!string.IsNullOrEmpty(userid))
			{
				var newUser = db.Users.SingleOrDefault(user => user.IdUser == userid);
				if (newUser != null)
				{
					result.Add(new ChatVM
					{
						ChatDate = DateTime.Now,
						SenderUser = db.Users.SingleOrDefault(user => user.IdUser == userIdString),
						RecieverUser = newUser,
						ChatMsg = null,
						IdPost = null
					});
				}
			}
			result = result.OrderBy(p => p.ChatDate).Reverse().ToList();

			return View(result);
		}
	}
}
