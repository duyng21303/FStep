using FStep.Data;

namespace FStep.ViewModels
{
	public class ProductVM
	{
		public int IdProduct { get; set; }

		public string? Name { get; set; }

		public int Quantity { get; set; }

		public float Price { get; set; }

		public bool Status { get; set; }

		public string Detail { get; set; }

		public string RecieveImg { get; set; }


	}
	public class ProductDetail
	{
		public int IdProduct { get; set; }

		public string Name { get; set; }

		public int Quantity { get; set; }

		public float Price { get; set; }

		public bool Status { get; set; }

		public string Detail { get; set; }

		public string RecieveImg { get; set; }

	}
}
