using FStep.Data;

namespace FStep.ViewModels
{
    public class ConfirmVM
    {
		public string? IdUserConfirm { get; set; }

		public string? IdUserConnect { get; set; }

		public bool? CheckConfirm { get; set; }

		public Post? Post { get; set; }

		public Comment? Comment { get; set; }
	}
}
