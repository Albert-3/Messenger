namespace Messenger.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body , CancellationToken cancellation);
    }

}