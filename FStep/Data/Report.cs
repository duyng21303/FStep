using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Report
{
    public int IdReport { get; set; }

    public string? Content { get; set; }

    public DateTime? Date { get; set; }

    public int IdPost { get; set; }

    public int IdComment { get; set; }

    public virtual Comment IdCommentNavigation { get; set; } = null!;

    public virtual Post IdPostNavigation { get; set; } = null!;

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
