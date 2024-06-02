namespace FStep.ViewModels.Email
{
    public class GetEmailSetting
    {
        public String SecretKey { get; set; } = default!;

        public String From { get; set; } = default!;

        public String SmtpServer { get; set; } = default!;

        public int Port { get; set; }


        public bool EnablSSL { get; set; }
    }
}
