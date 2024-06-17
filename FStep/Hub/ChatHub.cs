using System;
using System.Net.WebSockets;
using System.Web;
using AutoMapper;
using FStep.Data;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace FStep
{
    public class ChatHub : Hub
	{
		private readonly FstepDbContext _context;

		public ChatHub(FstepDbContext context)
		{
			_context = context;
		}
		public async Task SendMessage(string toUser, string fromUser, string massage, string img)
		{
			var chat = new Chat()
			{
				ChatMsg = massage,
				ChatDate = DateTime.Now,	
				RecieverUserId = toUser,
				SenderUserId = fromUser,
				IdPost = 1
			};
			await _context.Chats.AddAsync(chat);
			await _context.SaveChangesAsync();
			await Clients.All.SendAsync("ReceiveMessage", toUser, fromUser, massage, img, DateTime.Now.ToString("HH:mm"));
		}
		public async Task LoadMessages(string userId)
		{
			var currentUser = Context.User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			var messages = await _context.Chats
				.Where(m => (m.SenderUserId == currentUser && m.RecieverUserId == userId) ||
							(m.SenderUserId == userId && m.RecieverUserId == currentUser))
				.OrderBy(m => m.ChatDate)
				.ToListAsync();
			var recieverUser = await _context.Users
							.Where(u => u.IdUser == userId)
							.Select(u => new { u.IdUser, u.AvatarImg, u.Name })
							.FirstOrDefaultAsync();
			await Clients.Caller.SendAsync("LoadMessages", messages, currentUser, recieverUser);
		}
	}
}