using Messenger.Domain.Interface;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Messenger.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public DbSet<T> table;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            table = _context.Set<T>();
        }
        public async Task<List<T>> GetAllAsync() => await table.AsNoTracking().ToListAsync();


        public async Task<T> GetByIdAsync(Guid id) => await table.FindAsync(id);

        public async Task<int> CreateAsync(T entity)
        {
            table.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            table.Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            table.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
