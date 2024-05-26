using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Notification
{
    public int IdNotification { get; set; }

    public string? Name { get; set; }

    public string? Content { get; set; }

    public DateTime? Date { get; set; }

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
