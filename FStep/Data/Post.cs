using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Post
{
    public int IdPost { get; set; }

    public string? Content { get; set; }

    public DateTime? Date { get; set; }

    public string? Img { get; set; }

    public bool? Status { get; set; }

    public int? Feedback { get; set; }

    public string IdUser { get; set; } = null!;

    public int IdType { get; set; }

    public int? IdProduct { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Product? IdProductNavigation { get; set; }

    public virtual PostType IdTypeNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
