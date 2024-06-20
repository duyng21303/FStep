using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Report
{
    public int IdReport { get; set; }

    public string? Content { get; set; }

    public DateTime? Date { get; set; }

    public int? IdComment { get; set; }

    public int? IdPost { get; set; }

    public virtual Comment? IdCommentNavigation { get; set; }

    public virtual Post? IdPostNavigation { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
