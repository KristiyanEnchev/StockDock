namespace Application.Interfaces.Identity
{
    using Domain.Entities.Identity;

    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(User user, string ipAddress);

        Task<string> GenerateRefreshToken(User user);
    }
}