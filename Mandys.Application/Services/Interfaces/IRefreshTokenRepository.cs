namespace Mandys.Application.Services.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenInfo?> FindByTokenAsync(string rawToken);

    Task IssueAsync(int userId, string rawToken, DateTime expiresAt);

    Task RevokeAsync(Guid id);

    Task RevokeAllAsync(int userId);
}
