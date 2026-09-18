using Mandys.Configuration;
using Mandys.DTOs;
using Microsoft.Extensions.Options;

namespace Mandys.Services;

public class AuthService(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    ITokenService tokenService,
    IPasswordHasher passwordHasher,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwt = jwtOptions.Value;
    private readonly LoginRequestValidator _validator = new();

    public async Task<AuthTokenSet> LoginAsync(string? email, string password)
    {
        var validation = _validator.Validate(new LoginRequest(email, password));

        if (!validation.IsValid)
        {
            throw ServiceException.BadRequest(validation.Errors.First().ErrorMessage);
        }

        var user = await users.FindByEmailAsync(email!);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, password))
        {
            throw ServiceException.Unauthorized();
        }

        var accessToken = tokenService.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        await refreshTokens.IssueAsync(
            user.Id, refreshToken, DateTime.UtcNow.AddMinutes(_jwt.RefreshTokenExpireMinutes));

        return new AuthTokenSet(accessToken, refreshToken, _jwt.ExpireMinutes * 60);
    }

    public async Task<AuthTokenSet> RefreshAsync(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw ServiceException.BadRequest("Refresh token is required.");
        }

        var stored = await refreshTokens.FindByTokenAsync(refreshToken);

        if (stored is null)
        {
            throw ServiceException.Unauthorized();
        }

        if (stored.RevokedAt.HasValue)
        {
            // A revoked token presented again suggests theft: revoke all grants.
            await refreshTokens.RevokeAllAsync(stored.UserId);
            throw ServiceException.Unauthorized();
        }

        if (stored.ExpiresAt <= DateTime.UtcNow)
        {
            throw ServiceException.Unauthorized();
        }

        var user = await users.GetByIdAsync(stored.UserId);
        if (user is null)
        {
            throw ServiceException.Unauthorized();
        }

        var newAccessToken = tokenService.GenerateToken(user);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        await refreshTokens.RevokeAsync(stored.Id);
        await refreshTokens.IssueAsync(
            user.Id, newRefreshToken, DateTime.UtcNow.AddMinutes(_jwt.RefreshTokenExpireMinutes));

        return new AuthTokenSet(newAccessToken, newRefreshToken, _jwt.ExpireMinutes * 60);
    }

    public async Task LogoutAsync(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw ServiceException.BadRequest("Refresh token is required.");
        }

        var stored = await refreshTokens.FindByTokenAsync(refreshToken);

        if (stored is null || stored.RevokedAt.HasValue || stored.ExpiresAt <= DateTime.UtcNow)
        {
            throw ServiceException.Unauthorized();
        }

        await refreshTokens.RevokeAsync(stored.Id);
    }
}
