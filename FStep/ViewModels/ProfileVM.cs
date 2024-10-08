﻿namespace FStep.ViewModels
{
    public class ProfileVM
    {
        public string IdUser { get; set; } = null!;

        public string? Name { get; set; }

        public string? AvatarImg { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public string? StudentId { get; set; }

        public int? PointRating { get; set; }

        public string? Gender { get; set; }
        public string? Role { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? Total { get; set; }
        public int? TotalEx { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public List<PostVM> Posts { get; set; }
    }
}