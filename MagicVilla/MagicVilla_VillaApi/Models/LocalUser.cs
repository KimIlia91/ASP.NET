using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaApi.Models
{
    public class LocalUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
