using System;
using System.Net.WebSockets;
using System.Web;
using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace FStep
{
	public class ChatHub : Hub
	{
		private readonly FstepDBContext _context;

		public ChatHub(FstepDBContext context)
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
				IdPost = null
			};
			await _context.Chats.AddAsync(chat);
			await _context.SaveChangesAsync();
			await Clients.All.SendAsync("ReceiveMessage", toUser, fromUser, massage, img, DateTime.Now);
		}
		public async Task LoadMessagesDetail(string userId, string? postID, string? commentID)
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
			var commentDto = commentID != "" ? await _context.Comments
				.Where(m => m.IdComment == int.Parse(commentID))
				.Select(c => new Comment { IdComment = c.IdComment, Content = c.Content })
				.FirstOrDefaultAsync() : null;

			var postDto = postID != "" ? await _context.Posts
				.Where(m => m.IdPost == int.Parse(postID))
				.Select(p => new Post { IdPost = p.IdPost, Content = p.Content, Img = p.Img, Detail = p.Detail })
				.FirstOrDefaultAsync() : null;
			var confirmDb = new Confirm();
			if (postDto != null)
			{
				if(commentDto != null)
				{
					confirmDb = await _context.Confirms
						.Where(m => m.IdPost == postDto.IdPost && m.IdComment == commentDto.IdComment && (m.IdUserConnect == userId || m.IdUserConfirm == userId))
						.FirstOrDefaultAsync();
				}
				else
				{
					confirmDb = await _context.Confirms
						.Where(m => m.IdPost == postDto.IdPost && (m.IdUserConnect == userId || m.IdUserConfirm == userId))
						.FirstOrDefaultAsync();
				}
			}
			var confirm = new ConfirmVM()
			{
				IdUserConfirm = currentUser,
				IdUserConnect = userId,
				Comment = commentDto,
				Post = postDto,
				CheckConfirm = confirmDb != null ? confirmDb.Confirm1 : false
			};
			await Clients.Caller.SendAsync("LoadMessages", messages, currentUser, recieverUser, confirm);
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
			await Clients.Caller.SendAsync("LoadMessages", messages, currentUser, recieverUser, null);
		}
		public async Task HandleAccept(string message, string? userID, string? postID, string? commentID)
		{
			var currentUser = Context.User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			//check người dùng hiện tại đã từng confirm bài post này chưa
			var confirmDbCurrent = await _context.Confirms
						.Where(m => m.IdPost == int.Parse(postID) && m.IdUserConnect == userID && m.IdUserConfirm == currentUser)
						.FirstOrDefaultAsync();
			//check xem đã ai confirm với người dùng hiện tại hay chưa
			var confirmDbOrder = await _context.Confirms
						.Where(m => m.IdPost == int.Parse(postID) && m.IdUserConnect == currentUser && m.IdUserConfirm == userID)
						.FirstOrDefaultAsync();
			//-----------------------------------------------------------------------------
			if (confirmDbCurrent != null)
			{
				confirmDbCurrent.Confirm1 = true;
				_context.Confirms.Update(confirmDbCurrent);
				await _context.SaveChangesAsync();
			}
			else
			{
				var confirm = new Confirm()
				{
					Confirm1 = true,
					IdComment = commentID != "" ? int.Parse(commentID) : null,
					IdPost = int.Parse(postID),
					IdUserConfirm = currentUser,
					IdUserConnect = userID
				};
				await _context.Confirms.AddAsync(confirm);
				await _context.SaveChangesAsync();
			}
			// Xử lý logic khi người dùng nhấp vào "Đồng ý"
			await Clients.Caller.SendAsync("ReceiveNotification", message);
		}

		public async Task HandleDecline(string message, string? userID, string? postID, string? commentID)
		{
			var currentUser = Context.User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			//check người dùng hiện tại đã từng confirm bài post này chưa
			var confirmDbCurrent = await _context.Confirms
						.Where(m => m.IdPost == int.Parse(postID) && m.IdUserConnect == userID && m.IdUserConfirm == currentUser)
						.FirstOrDefaultAsync();
			//check xem đã ai confirm với người dùng hiện tại hay chưa
			var confirmDbOrder = await _context.Confirms
						.Where(m => m.IdPost == int.Parse(postID) && m.IdUserConnect == currentUser && m.IdUserConfirm == userID)
						.FirstOrDefaultAsync();
			//-----------------------------------------------------------------------------
			if (confirmDbCurrent != null)
			{
				confirmDbCurrent.Confirm1 = false;
				_context.Confirms.Update(confirmDbCurrent);
				await _context.SaveChangesAsync();
			}
			else
			{
				var confirm = new Confirm()
				{
					Confirm1 = false,
					IdComment = commentID != "" ? int.Parse(commentID) : null,
					IdPost = int.Parse(postID),
					IdUserConfirm = currentUser,
					IdUserConnect = userID
				};
				await _context.Confirms.AddAsync(confirm);
				await _context.SaveChangesAsync();
			}
			// Xử lý logic khi người dùng nhấp vào "Không đồng ý"
			await Clients.Caller.SendAsync("ReceiveNotification", message);
		}
	}
}