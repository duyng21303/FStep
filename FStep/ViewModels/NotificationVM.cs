namespace FStep.ViewModels
{
	public class NotificationVM
	{
		public string IdUser { get; set; } = null!;

		public string? Type { get; set; }

		public int? IDEvent { get; set; }
		public string? Name { get; set; }

		public string? Content { get; set; }

		public DateTime? Date { get; set; }
		public string? IdUserOther {  get; set; }
		public string? UserOtherImg { get; set;}
	}
}
