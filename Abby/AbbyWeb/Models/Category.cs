using System.ComponentModel.DataAnnotations;

namespace AbbyWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(20, ErrorMessage = "The length of the {0} cannot be more than {1} characters")]
        public string Name { get; set; }

        [Display(Name = "Display order")]
        [Range(1, 1000, ErrorMessage = "Display order must be between 1 and 100 only!")]
        public int DisplayOrder { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
