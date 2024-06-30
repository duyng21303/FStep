namespace FStep.ViewModels
{
    public class ProfileVM
    {
        public string IdUser { get; set; } = null!;

        public string? Name { get; set; }

        public string? AvatarImg { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public string? StudentId { get; set; }

        public int? Rating { get; set; }

        public string? Gender { get; set; }
        public string? Role { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreateDate { get; set; }

       public List<PostVM> Posts { get; set; }
    }
}
