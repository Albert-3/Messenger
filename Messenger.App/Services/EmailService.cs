using Messenger.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace Messenger.App.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Here you would implement the logic to send an email.
            // This is a placeholder implementation.
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = _configuration["EmailSettings:SmtpPort"];
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            // Use an SMTP client to send the email
            using (var message = new MailMessage(smtpUser, toEmail, subject, body))
            {
                message.IsBodyHtml = true;
                using (var client = new SmtpClient(smtpServer, int.Parse(smtpPort)))
                {
                    client.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPassword);
                    client.EnableSsl = true; // Use SSL if required
                    await client.SendMailAsync(message);
                }
            }
        }
    }
}