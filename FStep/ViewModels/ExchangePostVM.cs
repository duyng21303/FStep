namespace FStep.ViewModels
{
	public class ExchangePostVM
	{
		public int idPost { get; set; }

		public String Type { get; set; }

		public String Title { get; set; }

		public String Description { get; set; }

		public String Image { get; set; }

		public DateTime CreateDate { get; set; }

        public int IdProduct { get; set; }

        public string? Name { get; set; }

        public int Quantity { get; set; }

        public float Price { get; set; }

        public bool Status { get; set; }

        public string Detail { get; set; }

        public string RecieveImg { get; set; }
    }
}
