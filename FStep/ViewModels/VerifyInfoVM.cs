using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels
{
	public class VerifyInfoVM
	{

		[Required(ErrorMessage = "Bắt buộc")]
		[RegularExpression(@"[A-Z]{2}\d{6}", ErrorMessage = "MSSV không hợp lệ")]
		public string? StudentId { get; set; }
		[Required(ErrorMessage = "Bắt buộc")]
		public string? BankName { get; set; }
		[RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số tài khoản ngân hàng không hợp lệ")]
		public string? AccountNumber { get; set; }

		[RegularExpression(@"[A-Z\s]*", ErrorMessage = "Tên chủ tài khoản phải viết in hoa")]
		public string? AccountHolderName { get; set; }

		public string? SwiftCode { get; set; }
	}
}
