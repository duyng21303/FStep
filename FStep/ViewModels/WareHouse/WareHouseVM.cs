using FStep.Data;
using X.PagedList;

namespace FStep.ViewModels.WareHouse
{
	public class WareHouseVM
	{

		public CommentExchangeVM? CommentExchangeVM { get; set; }
		public PostVM? PostVM { get; set; }

		public TransactionVM? TransactionVM { get; set; }
		public User? UserBuyer { get; set; }
		public User? UserSeller { get; set; }
		public string? Type { get; set; }
		public string? Location { get; set; }
	}
	public class WareHouseServiceVM
	{
		public IPagedList<WareHouseVM> ExchangeList { get; set; }
		public IPagedList<WareHouseVM> SaleList { get; set; }

		public int ProcessCount { get; set; }
		public int FinishCount { get; set; }
		public int CancelCount { get; set; }
	}
}
