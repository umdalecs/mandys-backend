using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Records;

namespace Mandys.Infrastructure.Persistence.Mappers;

/// <summary>
/// Maps between the users table record and the domain entity.
/// </summary>
internal static class UserMapper
{
    internal static User ToDomain(this UserRecord record) =>
        new(
            record.Id,
            record.FirstName,
            record.LastName,
            record.Email,
            record.PasswordHash,
            record.Role,
            record.BranchId,
            record.BannedAt,
            record.CreatedAt,
            record.UpdatedAt);

    internal static UserRecord ToRecord(this User user) =>
        new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            Role = user.Role,
            BranchId = user.BranchId,
            BannedAt = user.BannedAt,
        };
}
