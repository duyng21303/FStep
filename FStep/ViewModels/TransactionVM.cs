namespace FStep.ViewModels
{
	public class TransactionVM
	{
		public int TransactionId { get; set; }
		public DateTime? DeliveryDate { get; set; }
		public DateTime? CreateDate { get; set; }
		public DateTime? CancelDate { get; set; }
		public string Img { get; set; }
		public string Status { get; set; }
		public float? UnitPrice { get; set; }
		public int? Quantity { get; set; }
		public float? Amount { get; set; }
		public string IdUserSeller { get; set; }
		public string? CodeTransaction { get; set; }
		public string? Content { get; set; }
		public string? Detail { get; set; }
		public int? IdPost { get; set; }
		public string? TypePost { get; set; }
		public string IdSeller { get; set; }
		public string? SellerName { get; set; }
		public string? UserName {  get; set; }
		public bool CheckFeedback { get; set; }
	}
}
