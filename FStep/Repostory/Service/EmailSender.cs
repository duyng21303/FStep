using FStep.Repostory.Interface;
using FStep.ViewModels.Email;
using System.Net;
using System.Net.Mail;

namespace FStep.Repostory.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;

        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<bool> EmailSendAsync(string email, string Subject, string message)
        {
           bool status = false;
            try
            {
                GetEmailSetting getEmailSetting = new GetEmailSetting()
                {
                    SecretKey = configuration.GetValue<String>("AppSettings:SecretKey"),
                    From = configuration.GetValue<String>("AppSettings:EmailSettings:From"),
                    SmtpServer = configuration.GetValue<String>("AppSettings:EmailSettings:SmtpServer"),
                    Port = configuration.GetValue<int>("AppSettings:EmailSettings:Port"),
                    EnablSSL = configuration.GetValue<bool>("AppSettings:EmailSettings:EnablSSL"),
                };

                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(getEmailSetting.From),
                    Subject = Subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                SmtpClient smtpClient = new SmtpClient(getEmailSetting.SmtpServer)
                {
                    Port = getEmailSetting.Port,
                    Credentials = new NetworkCredential(getEmailSetting.From, getEmailSetting.SecretKey),
                    EnableSsl = getEmailSetting.EnablSSL
                };

                await smtpClient.SendMailAsync(mailMessage);
                status = true;
            }catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
                //status = false;
            }
            return status;
        }
    }
}
