namespace FStep.ViewModels
{
	public class ListPostVM
	{
		public int PostId { get; set; }

		public string PostTitle { get; set; } = default!;

		public string PostBody { get; set; }

		public int Quantity { get; set; }

		public bool Status { get; set; }

		public float Price { get; set; }

		public String Type { get; set; } = default!;

		public String StudentId { get; set; } = default!;

		public String Image { get; set; } = default!;

        public DateTime CreateDate { get; set; }
    }
}
