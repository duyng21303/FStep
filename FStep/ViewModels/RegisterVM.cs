using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels
{
	public class RegisterVM
	{
		[Display(Name ="Tên đăng nhập")]
		[Required(ErrorMessage ="Không được bỏ trống mục này")]
		[MaxLength(50, ErrorMessage ="Tối đa 50 kí tự")]
		public string username { get; set; }
		[Required(ErrorMessage = "Không được bỏ trống mục này")]
		[MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
		[DataType(DataType.Password)]
		[Display(Name = "Mật khẩu")]
		public string password { get; set; }
		[Required(ErrorMessage = "Không được bỏ trống mục này")]
		[EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
		[Display(Name = "Địa chỉ Email")]
		public string email { get; set; }
	}
}
