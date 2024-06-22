using FStep.Data;
using FStep.Helpers;
using FStep.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FStep.Services
{
	public class NotificationServices
	{
		private FstepDBContext _context;

		public NotificationServices(FstepDBContext context) 
		{
			_context = context;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="typeMessage">type filter in notificationmessage</param>
		/// <param name="type">type notification</param>
		/// <param name="parameter">Like name, id, img you want to add message</param>
		/// <param name="idEvent">idcomment, idtransaction, idreport or idpayment</param>
		/// <returns></returns>

		public async Task CreateNotification(string userID, string typeMessage, string type, string parameter, int idEvent)
		{
			var message = NotificationMessage.NotificationMessages.TypeMessageFillter(typeMessage, parameter);
			var notification = new Notification()
			{
				IdUser = userID,
				Type = type,
				Content = message,
				Date = DateTime.Now,
			};
			switch (type)
			{
				case "Payment":
					notification.IdPayment = idEvent;
					break;
				case "Transaction":
					notification.IdTransaction = idEvent;
					break;
				case "Comment":
					notification.IdComment = idEvent;
					break;
				case "Report":
					notification.IdReport = idEvent;
					break;
				default:
					// Do nothing or handle unknown type if needed
					break;
			}
			await _context.Notifications.AddAsync(notification);
			await _context.SaveChangesAsync();

			// Gửi cập nhật số lượng thông báo tới client
		}
	}
}
