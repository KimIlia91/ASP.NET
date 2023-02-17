using System.ComponentModel.DataAnnotations;

namespace IdentityManager.Models.ViewModels
{
    public class IdentityRoleViewModel
    {
        public string? Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
