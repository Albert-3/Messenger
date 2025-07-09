namespace Messenger.Domain.Interfaces
{
    public interface IChatClient
    {
        public Task ReceiveMessage(Guid senderIdstring, string senderName, string messageText, DateTime dateTime);

    }
}