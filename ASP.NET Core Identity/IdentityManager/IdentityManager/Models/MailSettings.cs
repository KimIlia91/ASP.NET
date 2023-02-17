namespace IdentityManager.Models
{
    /// <summary>
    /// Класс для сохранения ключей подключения
    /// ключи сохранены в файле appsettings
    /// </summary>
    public class MailSettings
    {
        public string? UserName { get; set; }

        public string? DisplayName { get; set; }

        public string? From { get; set; }

        public string? Password { get; set; }

        public int Port { get; set; }

        public string? Host { get; set; }

        public bool UseSSL { get; set; }

        public bool UseStartTls { get; set; }
    }
}
