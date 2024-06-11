namespace FStep.ViewModels
{
    public class TransactionVM
    {
        public string TransactionID { get; set; }

        public string CreateDate { get; set; }

        public String Image { get; set; }

        public float Price { get; set; }

        public int Quantity { get; set; }

        public float Amount { get; set; }

        public string TransactionType { get; set; }

        public String IdUserBuyer { get; set; }

        public string IdUserSeler { get; set; }

        public bool Status { get; set; }

        public String? codeTransaction { get; set; }

        public String? NameProduct { get; set; }

        public int? IdPost { get; set; }
    }
}
