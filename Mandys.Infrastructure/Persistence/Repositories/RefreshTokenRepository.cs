using System.Security.Cryptography;
using System.Text;
using Mandys.Infrastructure.Persistence.Records;
using Mandys.Services;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(ApplicationDbContext db) : IRefreshTokenRepository
{
    public async Task<RefreshTokenInfo?> FindByTokenAsync(string rawToken)
    {
        var hash = Hash(rawToken);
        var record = await db.RefreshTokens
            .FirstOrDefaultAsync(r => r.TokenHash == hash);
        return record is null ? null : ToInfo(record);
    }

    public async Task IssueAsync(int userId, string rawToken, DateTime expiresAt)
    {
        db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = Hash(rawToken),
            ExpiresAt = expiresAt,
        });
        await db.SaveChangesAsync();
    }

    public async Task RevokeAsync(Guid id)
    {
        var record = await db.RefreshTokens.FindAsync(id);
        if (record is null || record.RevokedAt.HasValue)
        {
            return;
        }

        record.RevokedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task RevokeAllAsync(int userId)
    {
        var records = await db.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null)
            .ToListAsync();

        foreach (var record in records)
        {
            record.RevokedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
    }

    internal static string Hash(string rawToken) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

    private static RefreshTokenInfo ToInfo(RefreshToken record) =>
        new(record.Id, record.UserId, record.ExpiresAt, record.RevokedAt);
}
