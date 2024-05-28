using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels
{
    public class PostVM
    {
        [Display(Name = "Content")]
        [Required(ErrorMessage = "Required")]
        [MaxLength(250, ErrorMessage ="Tối đa 250 ký tự")]
        public string? Content { get; set; }
        [Display(Name = "Picture")]
        public string? Img { get; set; }
        [Display(Name = "Categories")]
        [Required(ErrorMessage ="*")]
        public string Type { get; set; } = null!;
        [MaxLength(250, ErrorMessage ="Tối đa 250 ký tự")]
        public string? Detail { get; set; }
    }
}
