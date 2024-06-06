using System.ComponentModel.DataAnnotations;


namespace FStep.ViewModels
{
	public class PostVM
	{
		[Display(Name = "Content")]
		[Required(ErrorMessage = "*")]
		[MaxLength(250, ErrorMessage = "Maximun 250 characters")]
		public string? Content { get; set; }

		[Display(Name = "Upload Picture")]
		[DataType(DataType.Upload)]
		[FileExtensions(Extensions ="png,jgp,jpeg,gif")]
		[Required(ErrorMessage = "*")]
		public string Img { get; set; }

		[Display(Name = "Detail")]
		[MaxLength(250, ErrorMessage = "Maximun 250 characters")]
		public string? Detail { get; set; }
		[Display(Name = "Type of post")]
		[Required(ErrorMessage = "*")]
		public string Type { get; set; } = string.Empty;
		[Display(Name = "Name of product")]
		
		public string NameProduct { get; set; } = string.Empty;
		[Display(Name = "Quantity")]
		public int Quantity { get; set; }
		[Display(Name = "Unit Price")]
		public float Price { get; set; }
		[Display(Name = "Detail information of Product")]
		public string? DetailProduct { get; set; }
	}
}
