using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class CoverType
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name ="Cover type")]
        [StringLength(50, ErrorMessage = "The length of the {0} cannot be more than {1} characters")]
        public string Name { get; set; }
    }
}
