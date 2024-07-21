using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace FStep.ViewModels
{
    public class CommentVM
    {
        public int IdComment { get; set; }

        public string? Content { get; set; }

        public DateTime? Date { get; set; }

        public int IdPost { get; set; }
        public string? PostName { get; set; }

        public string IdUser { get; set; } = null!;
        public string? Name { get; set; } = null!;
        public string? avarImg { get; set; }
        public string? Img { get; set; }
        public string? Type { get; set; }
		[NotMapped]
		public bool IsReported { get; set; } = false;
	}
}
