namespace Messenger.Domain.Interface
{
    public interface IGenericRepository<T> where T : class
    {
       public Task<List<T>> GetAllAsync();
       public Task<T> GetByIdAsync(Guid id);
       public Task CreateAsync(T entity);                
       public Task<bool> UpdateAsync(T entity);          
       public Task<bool> DeleteAsync(T entity);           
    }

}

