using FStep.Data;
using FStep.ViewModels.WareHouse;
using System.ComponentModel.DataAnnotations.Schema;
using X.PagedList;

namespace FStep.ViewModels
{
	public class TransactionVM
	{
		public Transaction? Transaction { get; set; }
		public Post? Post { get; set; }
		public User? UserBuyer { get; set; }
		public User? UserSeller { get; set; }
		public CommentExchangeVM? CommentExchangeVM { get; set; }

		//other info
		public int TransactionId { get; set; }
		public DateTime? CreateDate { get; set; }
		public DateTime? DeliveryDate { get; set; }
		public DateTime? CancelDate { get; set; }
		public bool CheckFeedback { get; set; }

		public float Revenues { get; set; }

		[NotMapped]
		public bool IsReported { get; set; } = false;

		// Add this property to hold a list of transactions
		public IPagedList<TransactionVM>? PagedTransactions { get; set; }
	}
	public class TransactionServiceVM
	{
		public IPagedList<TransactionVM>? ExchangeList { get; set; }
		public IPagedList<TransactionVM>? SaleList { get; set; }
	}
}
