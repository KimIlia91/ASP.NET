using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityManager.Models
{
    /// <summary>
    /// Модель которая добавляет дополнительный колонки в таблицу IdentityUsers
    /// Для этого также класс должен наследоваться от IdentityUser
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        //Атрибут который позволяет не отправлять свойство в таблицу
        [NotMapped]
        public string RoleId { get; set; }

        [NotMapped]
        public string? Role { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? RoleList { get; set; }
    }
}
