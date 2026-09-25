namespace Mandys.Services;

/// <summary>
/// Refresh token grant data. A plain carrier, not a domain entity: token
/// grants are an infrastructure/auth-session concern.
/// </summary>
public sealed record RefreshTokenInfo(
    Guid Id,
    int UserId,
    DateTime ExpiresAt,
    DateTime? RevokedAt);
