using IdentityManager.Models;
using IdentityManager.Services.IServices;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace IdentityManager.Services
{
    /// <summary>
    /// Класс для отправки сообщений
    /// </summary>
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(MailData mailData, CancellationToken ct)
        {
            var mailSettings = _configuration.GetSection("Google").Get<MailSettings>();
            var mail = new MimeMessage();

            #region Sender / Receiver
            //Sender
            mail.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.From));
            mail.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.From);

            //Receiver
            foreach (string mailAddress in mailData.To)
                mail.To.Add(MailboxAddress.Parse(mailAddress));

            // Set Reply to if specified in mail data
            if (!string.IsNullOrEmpty(mailData.ReplyTo))
                mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

            // BCC
            // Check if a BCC was supplied in the request
            if (mailData.Bcc != null)
            {
                // Get only addresses where value is not null or with whitespace. x = value of address
                foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
            }

            // CC
            // Check if a CC address was supplied in the request
            if (mailData.Cc != null)
            {
                foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
            }

            #endregion

            #region Content

            var body = new BodyBuilder();
            mail.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            mail.Body = body.ToMessageBody();

            // Check if we got any attachments and add the to the builder for our message
            if (mailData.Attachments != null)
            {
                byte[] attachmentFileByteArray;

                foreach (IFormFile attachment in mailData.Attachments)
                {
                    // Check if length of the file in bytes is larger than 0
                    if (attachment.Length > 0)
                    {
                        // Create a new memory stream and attach attachment to mail body
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            // Copy the attachment to the stream
                            attachment.CopyTo(memoryStream);
                            attachmentFileByteArray = memoryStream.ToArray();
                        }
                        // Add the attachment from the byte array
                        body.Attachments.Add(attachment.FileName, attachmentFileByteArray, ContentType.Parse(attachment.ContentType));
                    }
                }
            }

            #endregion

            #region Send Mail

            using var smtp = new SmtpClient();

            if (mailSettings.UseSSL)
            {
                await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.SslOnConnect, ct);
            }
            else if (mailSettings.UseStartTls)
            {
                await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls, ct);
            }
            await smtp.AuthenticateAsync(mailSettings.UserName, mailSettings.Password, ct);
            await smtp.SendAsync(mail, ct);
            await smtp.DisconnectAsync(true, ct);

            #endregion
        }
    }
}