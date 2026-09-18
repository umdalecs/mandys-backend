namespace Mandys.Services;

/// <summary>
/// Refresh token grants. Implemented by Infrastructure (stored hashed,
/// never raw).
/// </summary>
public interface IRefreshTokenRepository
{
    Task<RefreshTokenInfo?> FindByTokenAsync(string rawToken);

    Task IssueAsync(Guid userId, string rawToken, DateTime expiresAt);

    Task RevokeAsync(Guid id);

    Task RevokeAllAsync(Guid userId);
}
