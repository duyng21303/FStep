﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FStep.Data;

public partial class FstepContext : DbContext
{
    public FstepContext()
    {
    }

    public FstepContext(DbContextOptions<FstepContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.IdChat).HasName("PK__Chat__68D484D191D09E4D");

            entity.ToTable("Chat");

            entity.Property(e => e.IdChat).HasColumnName("id_chat");
            entity.Property(e => e.ChatDate)
                .HasColumnType("datetime")
                .HasColumnName("chat_date");
            entity.Property(e => e.ChatMsg)
                .HasMaxLength(255)
                .HasColumnName("chat_msg");
            entity.Property(e => e.IdPost).HasColumnName("id_post");
            entity.Property(e => e.RecieverUserId)
                .HasMaxLength(7)
                .HasColumnName("reciever_user_id");
            entity.Property(e => e.SenderUserId)
                .HasMaxLength(50)
                .HasColumnName("sender_user_id");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Chats)
                .HasForeignKey(d => d.IdPost)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKChat970520");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.Chats)
                .HasForeignKey(d => d.SenderUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKChat407738");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.IdComment).HasName("PK__Comment__7E14AC8544DD6F5A");

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
                .HasMaxLength(50)
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
            entity.HasKey(e => e.IdFeedback).HasName("PK__Feedback__36BC863008A12645");

            entity.ToTable("Feedback");

            entity.Property(e => e.IdFeedback).HasColumnName("id_feedback");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.IdPost).HasColumnName("id_post");
            entity.Property(e => e.IdUser)
                .HasMaxLength(50)
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
            entity.HasKey(e => e.IdNotification).HasName("PK__Notifica__925C842F83E90DD9");

            entity.ToTable("Notification");

            entity.Property(e => e.IdNotification).HasColumnName("id_notification");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.IdPayment).HasName("PK__Payment__862FEFE0F49A21E1");

            entity.ToTable("Payment");

            entity.Property(e => e.IdPayment).HasColumnName("id_payment");
            entity.Property(e => e.Amount).HasColumnName("amount");
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
            entity.HasKey(e => e.IdPost).HasName("PK__Post__3840C79DDF11D8BC");

            entity.ToTable("Post");

            entity.Property(e => e.IdPost).HasColumnName("id_post");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Detail)
                .HasMaxLength(255)
                .HasColumnName("detail");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.IdUser)
                .HasMaxLength(50)
                .HasColumnName("id_user");
            entity.Property(e => e.Img)
                .HasMaxLength(255)
                .HasColumnName("img");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FKPost405877");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPost118863");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__Product__BA39E84F9E980E9E");

            entity.ToTable("Product");

            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.Detail)
                .HasMaxLength(255)
                .HasColumnName("detail");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
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
            entity.HasKey(e => e.IdReport).HasName("PK__Report__D8639F52DF1AA624");

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
                .HasConstraintName("FKReport93927");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Reports)
                .HasForeignKey(d => d.IdPost)
                .HasConstraintName("FKReport245773");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.IdTransaction).HasName("PK__Transact__E8E1732D12BAEEAB");

            entity.ToTable("Transaction");

            entity.Property(e => e.IdTransaction).HasColumnName("id_transaction");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.IdPost).HasColumnName("id_post");
            entity.Property(e => e.IdUserBuyer)
                .HasMaxLength(50)
                .HasColumnName("id_user_buyer");
            entity.Property(e => e.IdUserSeller)
                .HasMaxLength(7)
                .HasColumnName("id_user_seller");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.IdPostNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.IdPost)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTransactio922554");

            entity.HasOne(d => d.IdUserBuyerNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.IdUserBuyer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKTransactio22282");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__User__D2D1463716293E68");

            entity.ToTable("User");

            entity.Property(e => e.IdUser)
                .HasMaxLength(50)
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
            entity.Property(e => e.HashKey)
                .HasMaxLength(255)
                .HasColumnName("hash_key");
            entity.Property(e => e.Name)
                .HasMaxLength(40)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasColumnName("role");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .HasColumnName("student_id");
            entity.Property(e => e.TokenGoogle)
                .HasMaxLength(255)
                .HasColumnName("token_google");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.IdNotification }).HasName("PK__User_Not__3BF48E75588FF471");

            entity.ToTable("User_Notification");

            entity.Property(e => e.IdUser)
                .HasMaxLength(50)
                .HasColumnName("id_user");
            entity.Property(e => e.IdNotification).HasColumnName("id_notification");
            entity.Property(e => e.IdComment).HasColumnName("id_comment");
            entity.Property(e => e.IdPayment).HasColumnName("id_payment");
            entity.Property(e => e.IdReport).HasColumnName("id_report");
            entity.Property(e => e.IdTransaction).HasColumnName("id_transaction");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

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

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKUser_Notif453825");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}