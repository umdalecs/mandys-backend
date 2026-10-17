namespace Mandys.Application.Services.Interfaces;

public sealed record AuthTokenSet(
    string AccessToken,
    string RefreshToken,
    int ExpiresInSeconds
);

public interface IAuthService
{
    Task<AuthTokenSet> LoginAsync(string email, string password);

    Task<AuthTokenSet> RefreshAsync(string? refreshToken);

    Task LogoutAsync(string? refreshToken);
}
