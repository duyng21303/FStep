using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels
{
	public class LoginVM
	{
		[Display(Name ="Tên đăng nhập")]
		[Required(ErrorMessage = "Thông tin này không được để trống")]
		public string userName { get; set; }
		[Display(Name = "Mật khẩu")]
		[Required(ErrorMessage = "Thông tin này không được để trống")]
		[DataType(DataType.Password)]
		public string password { get; set; }
	}
}
