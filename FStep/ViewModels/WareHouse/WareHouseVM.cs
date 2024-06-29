using FStep.Data;

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
}
