namespace Mandys.Services.Interfaces;

/// <summary>
/// Tokens issued by <see cref="IAuthService"/>. The handler decides how to
/// deliver them (cookies vs. JSON body).
/// </summary>
public sealed record AuthTokenSet(
    string AccessToken,
    string RefreshToken,
    int ExpiresInSeconds
);

/// <summary>
/// Authentication business logic. Failures are reported via
/// <see cref="ServiceException"/> for the handler to map to HTTP responses.
/// </summary>
public interface IAuthService
{
    Task<AuthTokenSet> LoginAsync(string? email, string password);

    Task<AuthTokenSet> RefreshAsync(string? refreshToken);

    Task LogoutAsync(string? refreshToken);
}
