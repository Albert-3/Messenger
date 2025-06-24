using Messaner.Infrastructure.Repositories;
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

        public async Task<List<Message>> IMessageRepositoryGetMessage(Guid senderId, Guid recipientId)
        {
            return await  _context.Messages
                         .Include(m => m.Sender)
                         .Where(p => p.SenderId == senderId && p.RecipientId == recipientId
                         || p.RecipientId == recipientId && p.SenderId == senderId)
                         .ToListAsync();
        }
    }
}
