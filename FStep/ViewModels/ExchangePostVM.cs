using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels
{
	public class ExchangePostVM
	{
		public int Id { get; set; }

		[Display(Name = "Title")]
		[Required(ErrorMessage = "This is required")]
		[MaxLength(255, ErrorMessage = "Exceeds character limit")]
		public string? Title { get; set; }
		[Display(Name = "Upload picture")]
		[DataType(DataType.Upload)]
		[FileExtensions(Extensions = "png,jgp,jpeg,gif")]
		public string Img { get; set; }

		[Display(Name = "Description")]
		[Required(ErrorMessage = "This is required")]
		[MaxLength(65500, ErrorMessage = "Exceeds character limit")]
		public string? Description { get; set; }
		public DateTime CreateDate { get; set; }
		public string Type { get; set; } = "Exchange";
		public int? IdProduct { get; set; }
		[Display(Name = "Name of product")]
		[Required(ErrorMessage = "This is required")]
		public string NameProduct { get; set; } = string.Empty;
		[Display(Name = "Detail information of Product")]
		public string? DetailProduct { get; set; }
	}
}
