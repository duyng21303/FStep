using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class Post
{
    public int IdPost { get; set; }

    public string? Content { get; set; }

    public DateTime? Date { get; set; }

    public string? Img { get; set; }

    public string? Status { get; set; }

    public string Type { get; set; } = null!;

    public string? Detail { get; set; }

    public string? Location { get; set; }

    public int? IdProduct { get; set; }

    public string IdUser { get; set; } = null!;

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Confirm> Confirms { get; set; } = new List<Confirm>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Product? IdProductNavigation { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
