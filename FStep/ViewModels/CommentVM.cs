using Microsoft.AspNetCore.Mvc;

namespace FStep.ViewModels
{
	public class CommentVM
	{
		public int IdComment { get; set; }

		public string? Content { get; set; }

		public DateTime? Date { get; set; }

		public int IdPost { get; set; }

		public string IdUser { get; set; } = null!;
		public string? Name { get; set; } = null!;
		public string? AvatarImg { get; set; } = null!;

		public string? Img { get; set; } = null!;
		public IFormFile? img { get; set; } = null!;
	}
}
