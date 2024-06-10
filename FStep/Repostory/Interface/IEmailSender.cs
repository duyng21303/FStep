namespace FStep.Repostory.Interface
{
    public interface IEmailSender
    {
        Task<bool> EmailSendAsync(String email, String Subject, String message);

    }
}
