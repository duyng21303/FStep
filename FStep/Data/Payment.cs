using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Payment
{
    public int IdPayment { get; set; }

    public DateTime? PayTime { get; set; }

    public float? Amount { get; set; }

    public string? ExternalMomoTransactionCode { get; set; }

    public int IdTransaction { get; set; }

    public virtual Transaction IdTransactionNavigation { get; set; } = null!;

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
