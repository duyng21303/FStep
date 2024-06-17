using System;
using System.ComponentModel.Design;
using System.Net.WebSockets;
using System.Web;
using System.Xml.Linq;
using AutoMapper;
using FStep.Data;
using FStep.Helpers;
using FStep.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
namespace FStep
{
	public class ChatHub : Hub
	{
		private readonly FstepDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ChatHub(FstepDbContext context, IHttpContextAccessor httpContextAccessor)

		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
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
				if (commentDto != null)
				{
					confirmDb = await _context.Confirms
						.Where(m => m.IdPost == postDto.IdPost && m.IdComment == commentDto.IdComment && (m.IdUserConnect == userId || m.IdUserConfirm == userId) && (m.IdUserConnect == currentUser || m.IdUserConfirm == currentUser))

						.FirstOrDefaultAsync();
				}
				else
				{
					confirmDb = await _context.Confirms
						.Where(m => m.IdPost == postDto.IdPost && (m.IdUserConnect == userId || m.IdUserConfirm == userId) && (m.IdUserConnect == currentUser || m.IdUserConfirm == currentUser))
						.FirstOrDefaultAsync();
				}
			}
			var confirm = new ConfirmVM()
			{
				IdUserConfirm = currentUser,
				IdUserConnect = userId,
				Comment = commentDto,
				Post = postDto,
				CheckConfirm = confirmDb != null ? confirmDb.Confirm1 : null
			};
			//_httpContextAccessor.HttpContext.Session.Set("USER_LIST", recieverUser);
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
			var confirmDbHistory = await _context.Confirms
				.Where(m => (m.IdUserConnect == userId || m.IdUserConfirm == userId) && (m.IdUserConnect == currentUser || m.IdUserConfirm == currentUser))
				.OrderByDescending(m => m.IdConfirm) // Sắp xếp theo Id giảm dần
				.FirstOrDefaultAsync();
			if (confirmDbHistory != null)
			{
				var commentDto = await _context.Comments
				.Where(m => m.IdComment == confirmDbHistory.IdComment)
				.Select(c => new Comment { IdComment = c.IdComment, Content = c.Content })
				.FirstOrDefaultAsync();

				var postDto = await _context.Posts
					.Where(m => m.IdPost == confirmDbHistory.IdPost)
					.Select(p => new Post { IdPost = p.IdPost, Content = p.Content, Img = p.Img, Detail = p.Detail })
					.FirstOrDefaultAsync();
				var confirmDbOrder = await _context.Confirms
						.Where(m => m.IdPost == postDto.IdPost && m.IdUserConnect == currentUser && m.IdUserConfirm == userId)
						.FirstOrDefaultAsync();
				var confirmDbCurrent = await _context.Confirms
						.Where(m => m.IdPost == postDto.IdPost && m.IdUserConnect == userId && m.IdUserConfirm == currentUser)
						.FirstOrDefaultAsync();
				var confirm = new ConfirmVM();
				if (confirmDbOrder != null)
				{
					if (confirmDbCurrent != null)
					{
						confirm = new ConfirmVM()
						{
							CheckConfirm = confirmDbCurrent.Confirm1,
							Comment = commentDto ?? null,
							Post = postDto,
							IdUserConfirm = currentUser,
							IdUserConnect = recieverUser.IdUser,
							CheckConfirmOrder = confirmDbOrder.Confirm1
						};
					}
					else
					{
						confirm = new ConfirmVM()
						{
							Comment = commentDto ?? null,
							Post = postDto,
							IdUserConfirm = currentUser,
							IdUserConnect = recieverUser.IdUser,
							CheckConfirmOrder = confirmDbOrder.Confirm1
						};
					}
				}
				else
				{
					confirm = new ConfirmVM()
					{
						CheckConfirm = confirmDbCurrent.Confirm1,
						Comment = commentDto ?? null,
						Post = postDto,
						IdUserConfirm = currentUser,
						IdUserConnect = recieverUser.IdUser
					};
				}

				await Clients.Caller.SendAsync("LoadMessages", messages, currentUser, recieverUser, confirm);
			}
			else
			{
				await Clients.Caller.SendAsync("LoadMessages", messages, currentUser, recieverUser, null);
			}
			//_httpContextAccessor.HttpContext.Session.Set("USER_LIST", recieverUser);
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
			var postDto = await _context.Posts
						.Where(m => m.IdPost == int.Parse(postID))
						.Select(p => new Post 
						{
							IdUser = p.IdUser
						})
						.FirstOrDefaultAsync();
			var idBuyer = "";
			var idSeller = "";
			if(userID != postDto.IdUser)
			{
				idBuyer = postDto.IdUser;
				idSeller = currentUser;
			}
			else
			{
				idBuyer = postDto.IdUser;
				idSeller = userID;
			}
			//-----------------------------------------------------------------------------
			if (confirmDbCurrent != null && confirmDbOrder == null)
			{
				confirmDbCurrent.Confirm1 = true;
				_context.Confirms.Update(confirmDbCurrent);
				await _context.SaveChangesAsync();
			}
			else
			{
				if (confirmDbOrder != null)
				{
					var checkConfirm = confirmDbOrder.Confirm1 ?? false;
					if (checkConfirm)
					{
						if (confirmDbCurrent != null)
						{
							//Thêm transaction
							var transaction = new Transaction()
							{
								Date = DateTime.Now,
								IdPost = int.Parse(postID),
								IdUserBuyer = idBuyer,
								IdUserSeller = idSeller,
								Status = "Waiting",
								CodeTransaction = Util.GenerateRandomKey(),
								Type = "Exchange"
							};
							await _context.Transactions.AddAsync(transaction);
							//--------------------------------------
							_context.Confirms.Remove(confirmDbCurrent);
							_context.Confirms.Remove(confirmDbOrder);
						}
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
					}
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
				}
				await _context.SaveChangesAsync();
			}
			await LoadMessages(userID);
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
			//nếu người khác từ chối thì hủy 2 confirm

			//-----------------------------------------------------------------------------
			if (confirmDbCurrent != null && confirmDbOrder == null)
			{
				_context.Confirms.Remove(confirmDbCurrent);
				message = "Bạn đã hủy trao đổi";
				await _context.SaveChangesAsync();
			}
			else
			{
				if (confirmDbOrder != null)
				{
					var checkConfirm = confirmDbOrder.Confirm1 ?? false;
					if (!checkConfirm)
					{
						_context.Confirms.Remove(confirmDbOrder);
						if (confirmDbCurrent != null)
						{
							_context.Confirms.Remove(confirmDbCurrent);
						}
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
					}
					message = "Bạn đã hủy trao đổi";
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
				}
				await _context.SaveChangesAsync();
			}
			await LoadMessages(userID);
			// Xử lý logic khi người dùng nhấp vào "Không đồng ý"
			await Clients.Caller.SendAsync("ReceiveNotification", message);
		}
		public async Task DeleteExchange(string currentUserId,string recieverUserId,string idPost)
		{
			var confirmDbCurrent = await _context.Confirms
						.Where(m => m.IdPost == int.Parse(idPost) && m.IdUserConnect == currentUserId && m.IdUserConfirm == recieverUserId)
						.FirstOrDefaultAsync();
			//check xem đã ai confirm với người dùng hiện tại hay chưa
			var confirmDbOrder = await _context.Confirms
						.Where(m => m.IdPost == int.Parse(idPost) && m.IdUserConnect == recieverUserId && m.IdUserConfirm == currentUserId)
						.FirstOrDefaultAsync();
			if(confirmDbCurrent != null)
			{
				_context.Confirms.Remove(confirmDbCurrent);
			}
			if(confirmDbOrder != null)
			{
				_context.Confirms.Remove(confirmDbOrder);
			}
			await _context.SaveChangesAsync();
			await LoadMessages(recieverUserId);
		}
	}
}