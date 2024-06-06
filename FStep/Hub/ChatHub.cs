using System;
using System.Web;
using FStep.Data;
using FStep.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
namespace FStep
{
    public class ChatHub : Hub
    {
		private readonly FstepDBContext _context;

		public ChatHub(FstepDBContext context)
		{
			_context = context;
		}
		public async Task SendMessage(string fromUser, string massage)
        {
            Clients.All.SendAsync("ReceiveMessage", fromUser, massage);
        }
		public async Task LoadMessages(string userId)
		{
			var currentUser = Context.User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			var messages = _context.Chats
				.Where(m => (m.SenderUserId == currentUser && m.RecieverUserId == userId) ||
							(m.SenderUserId == userId && m.RecieverUserId == currentUser))
				.OrderBy(m => m.ChatDate)
				.ToList();
		
			await Clients.Caller.SendAsync("LoadMessages", messages, currentUser);
		}
	}
}