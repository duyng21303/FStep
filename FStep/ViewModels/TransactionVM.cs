using FStep.Data;
using FStep.ViewModels.WareHouse;
using X.PagedList;

namespace FStep.ViewModels
{
	public class TransactionVM
	{
		//transaction info

		//public string Status { get; set; }
		//public int? Quantity { get; set; }
		//public float? Amount { get; set; }
		//public float? UnitPrice { get; set; }
		//public int? IdPost { get; set; }
		//public string IdUserBuyer { get; set; }
		//public string IdUserSeller { get; set; }
		//public string? CodeTransaction { get; set; }
		//public string? RecieveImg { get; set; }
		//public string? RecieveBuyerImg { get; set; }
		//public string? SentImg { get; set; }
		//public string? SentBuyerImg { get; set; }
		//public DateTime? ReceivedSellerDate { get; set; }
		//public DateTime? ReceivedBuyerDate { get; set; }
		//public DateTime? SentSellerDate { get; set; }
		//public DateTime? SentBuyerDate { get; set; }

		//post info
		//public string? Content { get; set; }
		//public string Img { get; set; }
		//public string? Detail { get; set; }
		//public string? TypePost { get; set; }
		//public string IdSeller { get; set; }

		//user info
		//public string? SellerName { get; set; }
		//public string? UserName {  get; set; }

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
	}
	public class TransactionServiceVM
	{
		public IPagedList<TransactionVM> ExchangeList { get; set; }
		public IPagedList<TransactionVM> SaleList { get; set; }
	}
}
