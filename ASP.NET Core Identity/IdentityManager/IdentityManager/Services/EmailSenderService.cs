using IdentityManager.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Emit;
using MimeKit;

namespace IdentityManager.Services
{
    public class EmailSenderService : IEmailSender
    {
        /// <summary>
        /// Для того чтобы доставть строку подключения с секретным ключом от сервиса MailJet
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Подключение сервиса MailJet из пакета
        /// </summary>
        //private EmailOptions _emailOptions;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Метод колученный из наследования IEmailSender для отправки сообщений пользователям
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="htmlMessage"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            #region <ailJet sender mail
            //_mailJetOptions = _configuration.GetSection("MailJet").Get<MailJetOptions>()!;
            //var client = new MailjetClient(_mailJetOptions.ApiKey, _mailJetOptions.SecretKey);
            //MailjetRequest request = new MailjetRequest
            //{
            //    Resource = Send.Resource,
            //}
            //.Property(Send.FromEmail, "paterfamelias@proton.me")
            //.Property(Send.FromName, "Kim&Co.")
            //.Property(Send.Subject, subject)
            //.Property(Send.HtmlPart, htmlMessage)
            //.Property(Send.To, email);
            //var response = await client.PostAsync(request);
            #endregion

            var mailSettings = _configuration.GetSection("Google").Get<MailSettings>()!;
            var emailToSend = new MimeMessage();
            emailToSend.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.From));
            emailToSend.To.Add(new MailboxAddress(email, email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect(mailSettings.Host, mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate(mailSettings.UserName, mailSettings.Password);
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);
            }

            #region
            //if (response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
            //    Console.WriteLine(response.GetData());
            //}
            //else
            //{
            //    Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
            //    Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
            //    Console.WriteLine(response.GetData());
            //    Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
            //}
            #endregion
        }
    }
}
