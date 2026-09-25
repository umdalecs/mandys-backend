using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence;

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
        };
}
