using Messenger.Domain;
using Messenger.Domain.Interface;
using Messenger.Infrastructure.Data;
using Messenger.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Messaner.Infrastructure.Repositories
{

    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<User> GetByUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Username cannot be empty", nameof(userName));
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
        public async Task<bool> CheckUserExist(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("Username cannot be empty", nameof(userName));
            return await _context.Users.AsNoTracking().AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
        public async Task<List<User>> GetUsers(Guid recipientId)
        {
            return await _context.Users
            .Where(p => p.Id != recipientId)
            .AsNoTracking().ToListAsync();
        }

        public async Task<User> GetUserById(Guid guid)
        {
            return await _context.Users.AsNoTracking().Where(x=>x.Id==guid).FirstOrDefaultAsync();
        }
    }
}
