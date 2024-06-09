using System;
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

    public int? Rating { get; set; }

    public string Role { get; set; } = null!;

    public string? TokenGoogle { get; set; }

    public string? HashKey { get; set; }

    public string? Gender { get; set; }

    public string? ResetToken { get; set; }


    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
