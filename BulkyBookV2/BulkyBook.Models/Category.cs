using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The length of the {0} cannot be more than {1} characters")]
        public string Name { get; set; }

        [Range(1, 1000, ErrorMessage = "Display order must be between 1 and 100 only!")]
        [Display(Name = "Display order")]
        public int DisplayOrder { get; set; }

        public DateTime CreateDateTime { get; set; } = DateTime.Now;
    }
}
