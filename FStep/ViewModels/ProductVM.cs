using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels
{
	public class ProductVM
	{
		[Display(Name = "Name of product")]
		[Required(ErrorMessage = "Required")]
		public string? Name { get; set; }
		[Display(Name = "Quantity")]
		public int? Quantity { get; set; }
		[Display(Name = "Price")]
		public float? Price { get; set; }
		[Display(Name = "Detail")]
		public string? Detail { get; set; }
	}
}
