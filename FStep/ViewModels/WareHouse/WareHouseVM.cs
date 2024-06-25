using X.PagedList;

namespace FStep.ViewModels.WareHouse
{
    public class WareHouseTransactionVM
    {
        public int IdPost { get; set; }

		public string CodeTransaction {  get; set; }
		public string NameProduct { get; set; }

        public String IdUserB {  get; set; }
        public String IdUserE { get; set; }
		public String IdUserBuyer { get; set; }

		public DateTime Date { get; set; }
		public float? Amount { get; set; }
		public int Quantity { get; set; }

		public String Location { get; set; }
	}
    public class WareHouseVM
    {
        public IPagedList<WareHouseTransactionVM> ExchangeList { get; set; }
        public IPagedList<WareHouseTransactionVM> SaleList { get; set; }

        public int ProcessCount { get; set; }
        public int FinishCount { get; set; }
        public int CancelCount { get; set; }
    }
}
