using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class UserNotification
{
    public string IdUser { get; set; } = null!;

    public int IdNotification { get; set; }

    public string? Type { get; set; }

    public int? IdReport { get; set; }

    public int? IdComment { get; set; }

    public int? IdPayment { get; set; }

    public int? IdTransaction { get; set; }

    public virtual Comment? IdCommentNavigation { get; set; }

    public virtual Notification IdNotificationNavigation { get; set; } = null!;

    public virtual Payment? IdPaymentNavigation { get; set; }

    public virtual Report? IdReportNavigation { get; set; }

    public virtual Transaction? IdTransactionNavigation { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
