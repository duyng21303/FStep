namespace FStep.ViewModels.WareHouse
{
	public class TransactionVM
	{
		public int IdTransaction { get; set; }

		public DateTime Date { get; set; }

		public int IdPost { get; set; }

		public String IdUserBuyer { get; set; }

		public String CodeTransaction { get; set; }

		public int Quantity {  get; set; }

		public float Amount { get; set; }

	}
}
