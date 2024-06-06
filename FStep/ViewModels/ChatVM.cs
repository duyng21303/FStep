using FStep.Data;

namespace FStep.ViewModels
{
    public class ChatVM
    {
        public string? ChatMsg { get; set; }

        public DateTime? ChatDate { get; set; }

        public User? RecieverUser { get; set; }

        public User? SenderUser { get; set; } = null!;

        public int IdPost { get; set; }
    }
}
