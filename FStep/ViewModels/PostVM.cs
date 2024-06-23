using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels
{
	public class PostVM
	{
		public int IdPost { get; set; }

		[Display(Name = "Title")]
		[Required(ErrorMessage = "This is required")]
		[MaxLength(250, ErrorMessage = "Maximun 250 characters")]
		public string? Title { get; set; }
		public int? IdProduct { get; set; }

		[Display(Name = "Img")]
		[Required(ErrorMessage = "This is required")]
		[DataType(DataType.Upload)]
		public string Img { get; set; }

		[Display(Name = "Description")]
		[Required(ErrorMessage = "This is required")]
		[MaxLength(65500, ErrorMessage = "Exceeds character limit")]
		public string? Description { get; set; }
		public DateTime? CreateDate { get; set; }
		public string Type { get; set; }
		

		[Display(Name = "Quantity")]
		[Required(ErrorMessage = "This is required")]
		public int? Quantity { get; set; }

		[Display(Name = "Price")]
		[DataType(DataType.Currency)]
		[Required(ErrorMessage = "This is required")]
		public float? Price { get; set; }
		[Display(Name = "Detail information of Product")]
		public string? DetailProduct { get; set; }
		public string IdUser { get; set; }
		public int? SoldQuantity { get; set; }
		public int? FeedbackNum { get; set; }
	}
}
