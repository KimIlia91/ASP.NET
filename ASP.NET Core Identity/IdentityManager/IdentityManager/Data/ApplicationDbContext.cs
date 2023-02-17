using IdentityManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityManager.Data
{
    /// <summary>
    /// Класс для подключения к БД MS SQL Server 
    /// Наследуется от IdentityDbContext который содержит таблицы для идентификации с ролями и политиками доступа
    /// IdentityDbCintext неследуется от DbContext
    /// Свойство DbSet нужно для добавления доп колонок в таблицу IdentityUser
    /// Для этого модель ApplicationDbContext должен наследоваться от IdentityUser
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
