using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels
{
    public class ProfileVM
    {
		[Display(Name = "Tên đăng nhập")]
		public string IdUser { get; set; } = null!;
		[Display(Name = "Họ và tên")]
		public string? Name { get; set; }
		public string? AvatarImg { get; set; }
		[Display(Name = "Địa chỉ")]
		public string? Address { get; set; }
        public string? Email { get; set; }
		[Display(Name = "Mã số sinh viên")]
		public string? StudentId { get; set; }
		[Display(Name = "Điểm đánh giá")]
		public int? Rating { get; set; }

    }
}
