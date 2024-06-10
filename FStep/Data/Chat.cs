using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Chat
{
    public int IdChat { get; set; }

    public string? ChatMsg { get; set; }

    public DateTime? ChatDate { get; set; }

    public string? RecieverUserId { get; set; }

    public string SenderUserId { get; set; } = null!;

    public int? IdPost { get; set; }

    public virtual Post? IdPostNavigation { get; set; }

    public virtual User SenderUser { get; set; } = null!;
}
