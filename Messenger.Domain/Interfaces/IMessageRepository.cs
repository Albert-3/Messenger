namespace Messenger.Domain.Interface
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        public Task<List<Message>> GetMessage(Guid senderId, Guid recipientId);

    }
}
