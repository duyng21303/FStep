﻿using System;
using System.Collections.Generic;

namespace FStep.Data;

public partial class User
{
    public string IdUser { get; set; } = null!;

    public bool? Status { get; set; }

    public string? Name { get; set; }

    public string? AvatarImg { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public string? StudentId { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? PointRating { get; set; }

    public string Role { get; set; } = null!;

    public string? TokenGoogle { get; set; }

    public string? HashKey { get; set; }

    public string? Gender { get; set; }

    public string? ResetToken { get; set; }

    public string? BankName { get; set; }

    public string? AccountHolderName { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? SwiftCode { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<HistoryPoint> HistoryPoints { get; set; } = new List<HistoryPoint>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
