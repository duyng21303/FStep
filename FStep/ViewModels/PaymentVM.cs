namespace FStep.ViewModels
{
    public class PaymentVM
    {
        public int IdPayment { get; set; }

        public DateTime? PayTime { get; set; }

        public float? Amount { get; set; }

        public string? VnpayTransactionCode { get; set; }

        public string? Type { get; set; }

        public int IdTransaction { get; set; }

        public string? Status { get; set; }

        public string? Note { get; set; }

        public DateTime? CancelDate { get; set; }
    }
}
