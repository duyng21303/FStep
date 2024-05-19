using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FStep.Data;

public partial class Fstep1Context : DbContext
{
    public Fstep1Context()
    {
    }

    public Fstep1Context(DbContextOptions<Fstep1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostType> PostTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=nguyenduy;database=FStep_1;User ID=sa;Password=12345;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.IdComment).HasName("PK__Comment__7E14AC85A3DCFCD2");

            entity.ToTable("Comment");

            entity.Property(e => e.IdComment).HasColumnName("id_comment");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.IdPost).HasColumnName("id_post");
            entity.Property(e => e.IdUser)
                .HasMaxLength(7)
                .HasColumnName("id_user");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdPost)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKComment984866");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKComment441956");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.IdFeedback).HasName("PK__Feedback__36BC86304CB3D5EC");

            entity.ToTable("Feedback");

            entity.Property(e => e.IdFeedback).HasColumnName("id_feedback");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.IdPost).HasColumnName("id_post");
            entity.Property(e => e.IdUser)
                .HasMaxLength(7)
                .HasColumnName("id_user");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.IdPost)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKFeedback927020");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKFeedback469931");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.IdNotification).HasName("PK__Notifica__925C842FE2C7C2BA");

            entity.ToTable("Notification");

            entity.Property(e => e.IdNotification).HasColumnName("id_notification");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.IdTypeNotif).HasName("PK__Notifica__EF224864DC28B21D");

            entity.ToTable("Notification_Type");

            entity.Property(e => e.IdTypeNotif).HasColumnName("id_type_Notif");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.IdPayment).HasName("PK__Payment__862FEFE071EF7570");

            entity.ToTable("Payment");

            entity.Property(e => e.IdPayment).HasColumnName("id_payment");
            entity.Property(e => e.Amount)
                .HasColumnType("money")
                .HasColumnName("amount");
            entity.Property(e => e.ExternalMomoTransactionCode)
                .HasMaxLength(255)
                .HasColumnName("external_momo_transaction_code");
            entity.Property(e => e.IdTransaction).HasColumnName("id_transaction");
            entity.Property(e => e.PayTime)
                .HasColumnType("datetime")
                .HasColumnName("pay_time");

            entity.HasOne(d => d.IdTransactionNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.IdTransaction)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPayment371031");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.IdPost).HasName("PK__Post__3840C79D500008DB");

            entity.ToTable("Post");

            entity.Property(e => e.IdPost).HasColumnName("id_post");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Feedback).HasColumnName("feedback");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.IdType).HasColumnName("id_type");
            entity.Property(e => e.IdUser)
                .HasMaxLength(7)
                .HasColumnName("id_user");
            entity.Property(e => e.Img)
                .HasMaxLength(255)
                .HasColumnName("img");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FKPost405877");

            entity.HasOne(d => d.IdTypeNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.IdType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPost783430");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPost118863");
        });

        modelBuilder.Entity<PostType>(entity =>
        {
            entity.HasKey(e => e.IdType).HasName("PK__Post_Typ__C3F091E05BBA915D");

            entity.ToTable("Post_Type");

            entity.Property(e => e.IdType).HasColumnName("id_type");
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__Product__BA39E84FEF0232B5");

            entity.ToTable("Product");

            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ReceivedSellerDate)
                .HasColumnType("datetime")
                .HasColumnName("received_seller_date");
            entity.Property(e => e.RecieveImg)
                .HasMaxLength(255)
                .HasColumnName("recieve_img");
            entity.Property(e => e.SentBuyerDate)
                .HasColumnType("datetime")
                .HasColumnName("sent_buyer_date");
            entity.Property(e => e.SentImg)
                .HasMaxLength(255)
                .HasColumnName("sent_img");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.IdReport).HasName("PK__Report__D8639F52BC2A68BA");

            entity.ToTable("Report");

            entity.Property(e => e.IdReport).HasColumnName("id_report");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.IdComment).HasColumnName("id_comment");
            entity.Property(e => e.IdPost).HasColumnName("id_post");

            entity.HasOne(d => d.IdCommentNavigation).WithMany(p => p.Reports)
                .HasForeignKey(d => d.IdComment)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKReport93927");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Reports)
                .HasForeignKey(d => d.IdPost)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKReport245773");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PK__Role__3D48441D3155CA46");

            entity.ToTable("Role");

            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.RoleName).HasColumnName("role_name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.IdTransaction).HasName("PK__Transact__E8E1732D33EFFD9B");

            entity.ToTable("Transaction");

            entity.Property(e => e.IdTransaction).HasColumnName("id_transaction");
            entity.Property(e => e.Amount)
                .HasColumnType("money")
                .HasColumnName("amount");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.IdPost).HasColumnName("id_post");
            entity.Property(e => e.IdUserBuyer)
                .HasMaxLength(7)
                .HasColumnName("id_user _buyer");
            entity.Property(e => e.IdUserSeller)
                .HasMaxLength(7)
                .HasColumnName("id_user_seller");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.IdPost)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTransactio922554");

            entity.HasOne(d => d.IdUserBuyerNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.IdUserBuyer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTransactio267620");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__User__D2D146375E63E90D");

            entity.ToTable("User");

            entity.HasIndex(e => e.StudentId, "UQ__User__2A33069BD491BE56").IsUnique();

            entity.Property(e => e.IdUser)
                .HasMaxLength(7)
                .HasColumnName("id_user");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .HasColumnName("address");
            entity.Property(e => e.AvatarImg)
                .HasMaxLength(255)
                .HasColumnName("avatar_img");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.Name)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .HasColumnName("student_id");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUser245539");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.IdNotification }).HasName("PK__User_Not__3BF48E754B53EC5B");

            entity.ToTable("User_Notification");

            entity.Property(e => e.IdUser)
                .HasMaxLength(7)
                .HasColumnName("id_user");
            entity.Property(e => e.IdNotification).HasColumnName("id_notification");
            entity.Property(e => e.IdComment).HasColumnName("id_comment");
            entity.Property(e => e.IdPayment).HasColumnName("id_payment");
            entity.Property(e => e.IdReport).HasColumnName("id_report");
            entity.Property(e => e.IdTransaction).HasColumnName("id_transaction");
            entity.Property(e => e.IdTypeNotif).HasColumnName("id_type_Notif");

            entity.HasOne(d => d.IdCommentNavigation).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.IdComment)
                .HasConstraintName("FKUser_Notif720975");

            entity.HasOne(d => d.IdNotificationNavigation).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.IdNotification)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUser_Notif974340");

            entity.HasOne(d => d.IdPaymentNavigation).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.IdPayment)
                .HasConstraintName("FKUser_Notif552335");

            entity.HasOne(d => d.IdReportNavigation).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.IdReport)
                .HasConstraintName("FKUser_Notif804938");

            entity.HasOne(d => d.IdTransactionNavigation).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.IdTransaction)
                .HasConstraintName("FKUser_Notif556883");

            entity.HasOne(d => d.IdTypeNotifNavigation).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.IdTypeNotif)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUser_Notif621415");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUser_Notif453825");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
