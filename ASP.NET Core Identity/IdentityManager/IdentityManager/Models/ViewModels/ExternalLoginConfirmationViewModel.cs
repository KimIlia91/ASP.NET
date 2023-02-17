using System.ComponentModel.DataAnnotations;

namespace IdentityManager.Models.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        public string Name { get; set; }
    }
}
