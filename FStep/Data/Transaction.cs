using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Transaction
{
    public int IdTransaction { get; set; }

    public DateTime? Date { get; set; }

    public string? Status { get; set; }

    public int? Quantity { get; set; }

    public float? Amount { get; set; }

    public float? UnitPrice { get; set; }

    public int IdPost { get; set; }

    public string IdUserBuyer { get; set; } = null!;

    public string IdUserSeller { get; set; } = null!;

    public string? Type { get; set; }

    public string? CodeTransaction { get; set; }

    public int? IdComment { get; set; }

    public string? RecieveImg { get; set; }

    public string? RecieveBuyerImg { get; set; }

    public string? SentImg { get; set; }

    public string? SentBuyerImg { get; set; }

    public DateTime? ReceivedSellerDate { get; set; }

    public DateTime? ReceivedBuyerDate { get; set; }

    public DateTime? SentSellerDate { get; set; }

    public DateTime? SentBuyerDate { get; set; }

    public virtual Comment? IdCommentNavigation { get; set; }

    public virtual Post IdPostNavigation { get; set; } = null!;

    public virtual User IdUserBuyerNavigation { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
