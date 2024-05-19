using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class NotificationType
{
    public int IdTypeNotif { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
