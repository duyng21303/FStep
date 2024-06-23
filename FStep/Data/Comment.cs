using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Comment
{
    public int IdComment { get; set; }

    public string? Content { get; set; }

    public DateTime? Date { get; set; }

    public string? Img { get; set; }

    public string? Type { get; set; }

    public int IdPost { get; set; }

    public string IdUser { get; set; } = null!;

    public virtual ICollection<Confirm> Confirms { get; set; } = new List<Confirm>();

    public virtual Post IdPostNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
