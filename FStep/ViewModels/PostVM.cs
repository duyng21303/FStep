using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels
{
	public class PostVM
	{
		public int IdPost { get; set; }

		[Display(Name = "Tiêu đề")]
		[Required(ErrorMessage = "Bắt buộc")]
		[MaxLength(250, ErrorMessage = "Maximun 250 characters")]
		public string? Title { get; set; }
		public int? IdProduct { get; set; }

		[Display(Name = "Hình ảnh")]
		[Required(ErrorMessage = "Bắt buộc")]
		[DataType(DataType.Upload)]
		public string Img { get; set; }

		[Display(Name = "Mô tả")]
		[Required(ErrorMessage = "Bắt buộc")]
		[MaxLength(65500, ErrorMessage = "Vượt quá giới hạn ký tự cho phép")]
		public string? Description { get; set; }
		public DateTime? CreateDate { get; set; }
		public string Type { get; set; }
		

		[Display(Name = "Số lượng")]
		[Required(ErrorMessage = "Bắt buộc")]
		public int? Quantity { get; set; }

		[Display(Name = "Giá")]
		[DataType(DataType.Currency)]
		[Required(ErrorMessage = "Bắt buộc")]
		public float? Price { get; set; }
		[Display(Name = "Thông tin chi tiết sản phẩm")]
		public string? DetailProduct { get; set; }
		public string IdUser { get; set; }
		public int? SoldQuantity { get; set; }
		public int? FeedbackNum { get; set; }
	}
}
