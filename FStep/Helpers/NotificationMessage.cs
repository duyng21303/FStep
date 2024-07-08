using FStep.Data;
using FStep.ViewModels;

namespace FStep.Helpers
{
	public class NotificationMessage
	{
		public static class NotificationMessages
		{
			// Transaction Notifications
			public static string TransactionSuccess(string bankName) =>
				$"Giao dịch cho đơn hàng của {bankName} đã thành công.";
			public static string TransactionExchangeSuccess(string name) =>
				$"Giao dịch trao đổi với {name} đã được tạo.";

			public static string TransactionReceived(string senderName) =>
				$"Bạn đã nhận được một giao dịch mới từ {senderName}.";

			public static string TransactionPending() =>
				"Giao dịch rút tiền từ tài khoản của bạn đang chờ xử lý.";

			public static string TransactionFailed(string storeName) =>
				$"Giao dịch mua hàng tại {storeName} đã bị hủy.";

			public static string TransactionExchangeAlready(string transactionID) =>
				$"Người trao đổi với bạn trong giao dịch #{transactionID} đã mang hàng tới, mời bạn tới lấy hàng.";
			public static string TransactionRecieveGoods(string transactionID) =>
				$"Chúng tôi đã nhận được hàng từ giao dịch #{transactionID} của bạn.";
			public static string TransactionExchangeRecieveSuccess(string transactionID) =>
				$"Bạn đã nhận hàng thành công từ giao dịch #{transactionID}";
			public static string TransactionSellSent(string transactionID) =>
				$"Bạn đã gửi hàng thành công cho đơn hàng #{transactionID}";
			public static string TransactionSellRecieve(string transactionID) =>
				$"Bạn đã nhận hàng thành công #{transactionID}";
			// Payment Notifications
			public static string PaymentConfirmed(string invoiceNumber) =>
				$"Thanh toán hóa đơn #{invoiceNumber} của bạn đã được xác nhận.";

			public static string PaymentReceived(string companyName) =>
				$"Bạn đã nhận được một khoản thanh toán mới từ {companyName}.";

			public static string PaymentFailed(string orderNumber) =>
				$"Thanh toán cho đơn hàng #{orderNumber} của bạn không thành công.";

			public static string PaymentProcessing() =>
				"Thanh toán của bạn cho dịch vụ Internet đang được xử lý.";

			public static string PaymentRefunded(string paymentNumber) =>
				$"Bạn đã nhận được khoản hoàn tiền cho thanh toán #{paymentNumber}.";

			// Comment Notifications
			public static string CommentOnPost(string commenterName) =>
				$"{commenterName} đã bình luận trên bài viết của bạn.";

			public static string NewComment(string commenterName) =>
				$"{commenterName} đã thêm một bình luận mới trong bài post của bạn.";

			public static string CommentDeleted(string articleTitle) =>
				$"Bìnsh luận của bạn về bài viết \"{articleTitle}\" đã bị xóa do vi phạm chính sách.";

			public static string CommentApproved(string articleTitle) =>
				$"Bình luận của bạn trên bài viết về \"{articleTitle}\" đã được duyệt.";

			// Report Notifications
			public static string ReportSubmitted(string articleTitle) =>
				$"Báo cáo về bài viết \"{articleTitle}\" đã được gửi thành công.";

			public static string ReportUnderReview(string reporterName) =>
				$"Báo cáo của bạn về {reporterName} đang được xem xét.";

			public static string ReportResolved(string commentDetails) =>
				$"Báo cáo về bình luận \"{commentDetails}\" đã được giải quyết.";

			public static string ReportRejected(string topic) =>
				$"Báo cáo của bạn về chủ đề \"{topic}\" không hợp lệ và đã bị từ chối.";

			public static string NewReportOnContent(string contentTitle) =>
				$"Có một báo cáo mới về nội dung bạn đã đăng: \"{contentTitle}\".";
			public static string TransactionExchangeFail(string contentTitle) =>
				$"Rất tiếc, phiên trao đổi của sản phẩm \"{contentTitle}\" đã bị hủy vì người bán đã có giao dịch";
			public static string TypeMessageFillter(string typeMessage, string parameters)
			{
				return typeMessage switch
				{
					"TransactionSuccess" => TransactionSuccess(parameters),
					"TransactionExchangeSuccess" => TransactionExchangeSuccess(parameters),
					"TransactionExchangeFail" => TransactionExchangeFail(parameters),
					"TransactionReceived" => TransactionReceived(parameters),
					"TransactionPending" => TransactionPending(),
					"TransactionFailed" => TransactionFailed(parameters),
					"TransactionExchangeAlready" => TransactionExchangeAlready(parameters),
					"TransactionRecieveGoods" => TransactionRecieveGoods(parameters),
					"TransactionExchangeRecieveSuccess" => TransactionExchangeRecieveSuccess(parameters),
					"TransactionSellSent" => TransactionSellSent(parameters),
					"TransactionSellRecieve" => TransactionSellRecieve(parameters),
					"PaymentReceived" => PaymentReceived(parameters),
					"PaymentFailed" => PaymentFailed(parameters),
					"PaymentProcessing" => PaymentProcessing(),
					"PaymentRefunded" => PaymentRefunded(parameters),
					"NewComment" => NewComment(parameters),
					"CommentDeleted" => CommentDeleted(parameters),
					"CommentApproved" => CommentApproved(parameters),
					"ReportSubmitted" => ReportSubmitted(parameters),
					"ReportUnderReview" => ReportUnderReview(parameters),
					"ReportResolved" => ReportResolved(parameters),
					"ReportRejected" => ReportRejected(parameters),
					"NewReportOnContent" => NewReportOnContent(parameters),
					_ => "Unknown notification type."
				} ;
			}
			public static User? UserOther(FstepDbContext context, Notification notif)
			{
				string type = notif.Type;
				return type switch
				{
					"Payment" => (from payment in context.Payments
								  join transaction in context.Transactions
								  on payment.IdTransaction equals transaction.IdTransaction
								  join user in context.Users
								  on transaction.IdUserBuyer equals user.IdUser
								  where payment.IdPayment == EventEntity<Payment>(context, notif).IdPayment// paymentId là id của payment bạn có
								  select user).FirstOrDefault(),
					"Transaction" => context.Users
						.Where(p => p.IdUser == EventEntity<Transaction>(context, notif).IdUserBuyer)
						.FirstOrDefault(),
					"Comment" => context.Users
						.Where(p => p.IdUser == EventEntity<Comment>(context, notif).IdUser)
						.FirstOrDefault(),
					"Report" => null,
					_ => null
				};
			}
			public static T EventEntity<T>(FstepDbContext context, Notification notif)
			{
				string type = notif.Type;

				return type switch
				{
					"Payment" => (T)(object)context.Payments
						.Where(p => p.IdPayment == notif.IdPayment)
						.FirstOrDefault(),
					"Transaction" => (T)(object)context.Transactions
						.Where(p => p.IdTransaction == notif.IdTransaction)
						.FirstOrDefault(),
					"Comment" => (T)(object)context.Comments
						.Where(p => p.IdComment == notif.IdComment)
						.FirstOrDefault(),
					"Report" => (T)(object)context.Reports
						.Where(p => p.IdReport == notif.IdReport)
						.FirstOrDefault(),
					_ => default(T)
				};
			}
			public static int? EventNotifID(Notification notif)
			{
				string type = notif.Type;
				return type switch
				{
					"Payment" => notif.IdPayment,
					"Transaction" => notif.IdTransaction,
					"Comment" => notif.IdComment,
					"Report" => notif.IdReport,
					_ => 0
				};
			}
		}
	}
}