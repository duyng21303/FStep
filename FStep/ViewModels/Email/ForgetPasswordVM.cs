using System.ComponentModel.DataAnnotations;

namespace FStep.ViewModels.Email
{
    public class ForgetPasswordVM
    {
        [Required]
        public string Email { get; set; } = default!;

        public string UserId { get; set; } = default!;

        public DateTime ResetTokenExpires { get; set; } = default!;
        public string Token { get; set; } = default!;

        [Required(ErrorMessage = "please enter password")]
        [DataType(DataType.Password)]
        public String Password { get; set; } = default!;

        [Required(ErrorMessage = "please enter password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password not matched.")]
        [Display(Name = "Confirm Password")]
        public String ConfirmPassword { get; set; }

    }
}
