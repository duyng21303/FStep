namespace FStep.ViewModels
{
	public class ReportVM
	{
		public int? IdReport { get; set; }

		public string? Content { get; set; }
		public string? CommentContent { get; set; }

		public DateTime? Date { get; set; }

		public int? IdPost { get; set; }
		public string? PostName { get; set; }
		public string? UserAvatar { get; set; }

		public string? IdUser { get; set; }
		public string? UserFullName { get; set; }

		public string? IdUserComment { get; set; }
		public string? UserCommentFullName { get; set; }
		public string? UserCommentAvatar { get; set; }

		public int? IdTransaction { get; set; }
		public int? PointRating { get; set; }
		public int? IdComment { get; set; }
	}

}
