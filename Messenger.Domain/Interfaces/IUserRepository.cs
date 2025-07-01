namespace Messenger.Domain.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public Task<bool> CheckUserExist(string userName);
        public Task<User> GetByUser(string userName);
        public Task<List<User>> GetUsers(Guid guid);
        Task<User> GetUserById(Guid guid);
    }
}
