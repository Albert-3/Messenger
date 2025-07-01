using Messenger.Domain;
using Messenger.Domain.Interface;
using Messenger.Infrastructure.Data;
using Messenger.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Messegner.Infrastructure.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;

        }

        async Task<List<Message>> IMessageRepository.GetMessage(Guid senderId, Guid recipientId)
        {
            return await _context.Messages
                         .Include(m => m.Sender)
                         .Where(p => p.SenderId == senderId && p.RecipientId == recipientId
                         || p.RecipientId == recipientId && p.SenderId == senderId)
                         .AsNoTracking().ToListAsync();
        }
    }

}
