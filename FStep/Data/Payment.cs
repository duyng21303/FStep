using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Payment
{
    public int IdPayment { get; set; }

    public DateTime? PayTime { get; set; }

    public float? Amount { get; set; }

    public string? VnpayTransactionCode { get; set; }

    public string? Type { get; set; }

    public int IdTransaction { get; set; }

    public string? Status { get; set; }

    public string? Note { get; set; }

    public virtual Transaction IdTransactionNavigation { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
