using IdentityManager.Models;

namespace IdentityManager.Services.IServices
{
    public interface IMailService
    {
        Task SendEmailAsync(MailData mailData, CancellationToken ct = default);
    }
}
