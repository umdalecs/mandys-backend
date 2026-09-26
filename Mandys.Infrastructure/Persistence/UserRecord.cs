using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence;

/// <summary>
/// Persistence record for the users table. Mapped to/from
/// <see cref="User"/> by <see cref="UserRepository"/>.
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
    [Required]
    [MaxLength(50)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [Column("password")]
    public string PasswordHash { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = Roles.Customer;

    /// <summary>
    /// FK to the branch this user belongs to (many users to one branch).
    /// Nullable so pre-existing / central users without a branch keep working.
    /// </summary>
    public int? BranchId { get; set; }

    /// <summary>
    /// Branch this user belongs to. Inverse of <see cref="BranchRecord.Users"/>.
    /// </summary>
    [ForeignKey(nameof(BranchId))]
    public BranchRecord? Branch { get; set; }
}
