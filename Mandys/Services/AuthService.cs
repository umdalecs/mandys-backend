using Isopoh.Cryptography.Argon2;
using Mandys.Configuration;
using Mandys.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Mandys.Services;

public class AuthService(
    ApplicationDbContext db,
    ITokenService tokenService,
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

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email!.ToLower());

        if (user is null || !Argon2.Verify(user.Password, password))
        {
            throw ServiceException.Unauthorized();
        }

        var accessToken = tokenService.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpireMinutes);

        await db.SaveChangesAsync();

        return new AuthTokenSet(accessToken, refreshToken, _jwt.ExpireMinutes * 60);
    }

    public async Task<AuthTokenSet> RefreshAsync(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw ServiceException.BadRequest("Refresh token is required.");
        }

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user is null || user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw ServiceException.Unauthorized();
        }

        var newAccessToken = tokenService.GenerateToken(user);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwt.RefreshTokenExpireMinutes);

        await db.SaveChangesAsync();

        return new AuthTokenSet(newAccessToken, refreshToken, _jwt.ExpireMinutes * 60);
    }

    public async Task LogoutAsync(string? refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw ServiceException.BadRequest("Refresh token is required.");
        }

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user is null || user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw ServiceException.Unauthorized();
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await db.SaveChangesAsync();
    }
}
