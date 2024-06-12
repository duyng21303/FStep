﻿namespace FStep.ViewModels
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string? IdUser { get; set; }
        public float Amount { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
    }

    public class VnPaymentRequestModel
    {
        //public string ReceiveUseId { get; set; }
        //public string SendUseId { get; set; }
        public string? IdUser { get; set; }
        public int TransactionId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public float Amount { get; set; } 
        public DateTime CreatedDate { get; set; }
    }
}
