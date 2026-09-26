using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence.Records;

/// <summary>
/// Persistence record for the users table. Mapped to/from
/// <see cref="User"/> by <see cref="Repositories.UserRepository"/>.
/// </summary>
public class UserRecord : Record
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// Login handle. Null when the user has no login credentials (a
    /// point-of-sale customer that never registered), which also makes the
    /// row unreachable by the login lookup.
    /// </summary>
    [MaxLength(50)]
    public string? Email { get; set; }

    /// <summary>
    /// Argon2 hash of the login password. Null when the user has no login
    /// credentials.
    /// </summary>
    [Column("password")]
    public string? PasswordHash { get; set; }
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = Roles.Customer;

    /// <summary>
    /// FK to the branch this user belongs to (many users to one branch).
    /// Nullable so pre-existing / central users without a branch keep working.
    /// </summary>
    public int? BranchId { get; set; }

    /// <summary>
    /// When the account was banned. Null means not banned; the value doubles
    /// as the reason the account is disabled, so it is kept as a timestamp
    /// rather than a bare flag.
    /// </summary>
    public DateTime? BannedAt { get; set; }

    /// <summary>
    /// Branch this user belongs to. Inverse of <see cref="BranchRecord.Users"/>.
    /// </summary>
    [ForeignKey(nameof(BranchId))]
    public BranchRecord? Branch { get; set; }
}
