using System.ComponentModel.DataAnnotations;

namespace IdentityManager.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be least {2} characters long.",  MinimumLength = 6)]  
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password don't match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
