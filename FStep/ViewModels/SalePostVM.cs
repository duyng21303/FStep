using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels
{
	public class SalePostVM
	{
		public int Id { get; set; }

		[Display(Name = "Title")]
		[Required(ErrorMessage = "This is required")]
		[MaxLength(250, ErrorMessage = "Maximun 250 characters")]
		public string? Title { get; set; }
		[Display(Name = "Upload Picture")]
		[Required(ErrorMessage = "This is required")]
		[DataType(DataType.Upload)]
		[FileExtensions(Extensions = "png,jgp,jpeg,gif")]
		public string Img { get; set; }

		[Display(Name = "Description")]
		[Required(ErrorMessage = "This is required")]
		[MaxLength(250, ErrorMessage = "Maximun 250 characters")]
		public string? Description { get; set; }
		public DateTime CreateDate { get; set; }
		public string Type { get; set; } = "Sale";
		[Display(Name = "Name of product")]
		[Required(ErrorMessage = "This is required")]
		public string NameProduct { get; set; } = string.Empty;

		[Display(Name = "Quantity")]
		[Required(ErrorMessage = "This is required")]
		public int Quantity { get; set; }

		[Display(Name = "Unit Price")]
		[Required(ErrorMessage = "This is required")]
		public float Price { get; set; }
		[Display(Name = "Detail information of Product")]
		public string? DetailProduct { get; set; }
	}
}
