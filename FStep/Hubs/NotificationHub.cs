using FStep.Data;
using FStep.Helpers;
using FStep.ViewModels;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FStep.Hubs
{
	public class NotificationHub : Hub
	{
		private FstepDBContext _context;
		private static Dictionary<string, string> userConnections = new Dictionary<string, string>();
		public NotificationHub(FstepDBContext context)
		{
			_context = context;
		}
		public override async Task OnConnectedAsync()
		{
			var userId = Context.User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			if (userId != null)
			{
				userConnections[userId] = Context.ConnectionId;
			}
			await base.OnConnectedAsync();
		}
		public override async Task OnDisconnectedAsync(Exception exception)
		{
			var userId = Context.User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			if (userId != null && userConnections.ContainsKey(userId))
			{
				userConnections.Remove(userId);
			}
			await base.OnDisconnectedAsync(exception);
		}
		public async Task LoadNotification()
		{
			var currentUser = Context.User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			var notificationUserDB = await _context.Notifications
				.Where(m => (m.IdUser == currentUser))
				.OrderByDescending(m => m.Date)
				.ToListAsync();
			if (notificationUserDB != null)
			{
				List<NotificationVM> notificationList = new List<NotificationVM>();
				foreach (var notificationUser in notificationUserDB)
				{
					var userOther = NotificationMessage.NotificationMessages.UserOther(_context, notificationUser);
					if (userOther != null)
					{
						notificationList.Add(new NotificationVM
						{
							Content = notificationUser.Content,
							Date = notificationUser.Date,
							Name = notificationUser.Name,
							IdUser = currentUser,
							IDEvent = NotificationMessage.NotificationMessages.EventNotifID(notificationUser),
							Type = notificationUser.Type,
							IdUserOther = userOther.IdUser,
							UserOtherImg = userOther.AvatarImg
						});
					}
					else
					{
						notificationList.Add(new NotificationVM
						{
							Content = notificationUser.Content,
							Date = notificationUser.Date,
							Name = notificationUser.Name,
							IdUser = currentUser,
							IDEvent = NotificationMessage.NotificationMessages.EventNotifID(notificationUser),
							Type = notificationUser.Type,
						});
						Console.WriteLine(notificationList);
					}
				}
				
				await Clients.Caller.SendAsync("LoadNotification", notificationList);
			}
		}

	}
}