using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Comment
{
    public int IdComment { get; set; }

    public string? Content { get; set; }

    public DateTime? Date { get; set; }

    public string IdUser { get; set; } = null!;

    public int IdPost { get; set; }

    public virtual Post IdPostNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
