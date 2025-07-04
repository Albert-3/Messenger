namespace Messenger.Domain.Interfaces
{
    public interface IChatClient
    {
        public Task ReceiveMessage(string senderName, string messageText , DateTime dateTime ,Guid guid);
    }
}