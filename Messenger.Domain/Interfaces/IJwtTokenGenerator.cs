namespace Messenger.Domain.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);

    }
}
