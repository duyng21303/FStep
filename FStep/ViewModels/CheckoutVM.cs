namespace FStep.ViewModels
{
    public class CheckoutVM
    {
		public int IdPost { get; set; }
		public string Title { get; set; }
        public string Img { get; set; }
        public string IdUserBuyer { get; set; } = String.Empty;
        public string IdUserSeller { get; set; } = String.Empty;
        public int ProductId {  get; set; }
        public float? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public string? PaymentMethod {  get; set; }
        public float? Amount { get; set; }
        public string? Note { get; set; }
		public string Type { get; set; }
	}
}
