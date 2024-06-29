using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Notification
{
    public int IdNotification { get; set; }

    public DateTime? Date { get; set; }

    public string? Content { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? IdUser { get; set; }

    public int? IdReport { get; set; }

    public int? IdComment { get; set; }

    public int? IdPayment { get; set; }

    public int? IdTransaction { get; set; }

    public bool? AlreadySeen { get; set; }

    public virtual Comment? IdCommentNavigation { get; set; }

    public virtual Payment? IdPaymentNavigation { get; set; }

    public virtual Report? IdReportNavigation { get; set; }

    public virtual Transaction? IdTransactionNavigation { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}
